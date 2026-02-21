using AutoMapper;
using Microsoft.Extensions.Configuration;
using SolarisPlatform.Application.Common.Interfaces;
using SolarisPlatform.Application.Common.Models;
using SolarisPlatform.Application.DTOs.Auth;
using SolarisPlatform.Domain.Entities.Seguridad;
using SolarisPlatform.Domain.Enums;
using SolarisPlatform.Domain.Interfaces;

namespace SolarisPlatform.Infrastructure.Services;

/// <summary>
/// Servicio de autenticación
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ISesionUsuarioRepository _sesionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly IPasswordService _passwordService;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly int _maxIntentosLogin;
    private readonly int _minutosBloqueo;
    private readonly int _refreshTokenExpirationDays;

    public AuthService(
        IUsuarioRepository usuarioRepository,
        ISesionUsuarioRepository sesionRepository,
        IUnitOfWork unitOfWork,
        ITokenService tokenService,
        IPasswordService passwordService,
        IMapper mapper,
        IConfiguration configuration)
    {
        _usuarioRepository = usuarioRepository;
        _sesionRepository = sesionRepository;
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _passwordService = passwordService;
        _mapper = mapper;
        _configuration = configuration;
        _maxIntentosLogin = int.Parse(_configuration["Security:MaxIntentosLogin"] ?? "5");
        _minutosBloqueo = int.Parse(_configuration["Security:MinutosBloqueo"] ?? "30");
        _refreshTokenExpirationDays = int.Parse(_configuration["Jwt:RefreshTokenExpirationDays"] ?? "7");
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        // Buscar usuario por email con roles
        var usuario = await _usuarioRepository.GetByEmailWithRolesAsync(request.Email, cancellationToken);

        if (usuario == null)
        {
            return new LoginResponse
            {
                Exitoso = false,
                Mensaje = "Credenciales inválidas"
            };
        }

        // Verificar si está activo
        if (!usuario.EstaActivo())
        {
            return new LoginResponse
            {
                Exitoso = false,
                Mensaje = "El usuario no está activo"
            };
        }

        // Verificar si está bloqueado
        if (usuario.EstaBloqueado())
        {
            if (usuario.FechaBloqueo.HasValue && usuario.FechaBloqueo > DateTime.UtcNow)
            {
                var minutosRestantes = (int)(usuario.FechaBloqueo.Value - DateTime.UtcNow).TotalMinutes;
                return new LoginResponse
                {
                    Exitoso = false,
                    Mensaje = $"Usuario bloqueado. Intente nuevamente en {minutosRestantes} minutos"
                };
            }
            else
            {
                // El bloqueo expiró, desbloquear
                usuario.Desbloquear();
            }
        }

        // Verificar contraseña
        if (!_passwordService.VerifyPassword(request.Password, usuario.PasswordHash))
        {
            usuario.IncrementarIntentosLogin();

            if (usuario.IntentosLoginFallidos >= _maxIntentosLogin)
            {
                usuario.Bloquear(_minutosBloqueo);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return new LoginResponse
                {
                    Exitoso = false,
                    Mensaje = $"Usuario bloqueado por {_minutosBloqueo} minutos debido a múltiples intentos fallidos"
                };
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            var intentosRestantes = _maxIntentosLogin - usuario.IntentosLoginFallidos;

            return new LoginResponse
            {
                Exitoso = false,
                Mensaje = $"Credenciales inválidas. {intentosRestantes} intentos restantes"
            };
        }

        // Verificar empresa activa
        if (!usuario.Empresa.EstaActiva())
        {
            return new LoginResponse
            {
                Exitoso = false,
                Mensaje = "La empresa no está activa"
            };
        }

        // Login exitoso - resetear intentos
        usuario.RegistrarAcceso();

        // Obtener permisos del usuario
        var permisos = await _usuarioRepository.GetPermisosUsuarioAsync(usuario.Id, cancellationToken);

        // Crear DTO de usuario autenticado
        var usuarioAuth = _mapper.Map<UsuarioAutenticadoDto>(usuario);
        usuarioAuth.Permisos = permisos.ToList();

        // Generar tokens
        var token = _tokenService.GenerarToken(usuarioAuth);
        var refreshToken = _tokenService.GenerarRefreshToken();
        var expiracion = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:ExpirationMinutes"] ?? "60"));

        // Crear sesión
        var sesion = new SesionUsuario
        {
            UsuarioId = usuario.Id,
            Token = token,
            RefreshToken = refreshToken,
            FechaInicio = DateTime.UtcNow,
            FechaExpiracion = expiracion,
            FechaUltimaActividad = DateTime.UtcNow,
            Activo = true
        };

        await _sesionRepository.AddAsync(sesion, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new LoginResponse
        {
            Exitoso = true,
            Mensaje = "Login exitoso",
            Token = token,
            RefreshToken = refreshToken,
            Expiracion = expiracion,
            Usuario = usuarioAuth
        };
    }

    public async Task<RegistroResponse> RegistrarAsync(RegistroRequest request, CancellationToken cancellationToken = default)
    {
        // Verificar que el email no exista
        if (await _usuarioRepository.EmailExisteAsync(request.Email, null, cancellationToken))
        {
            return new RegistroResponse
            {
                Exitoso = false,
                Mensaje = "El email ya está registrado"
            };
        }

        // Crear usuario
        var usuario = new Usuario
        {
            EmpresaId = request.EmpresaId,
            SucursalId = request.SucursalId,
            Email = request.Email,
            Nombres = request.Nombres,
            Apellidos = request.Apellidos,
            Telefono = request.Telefono,
            PasswordHash = _passwordService.HashPassword(request.Password),
            Estado = EstadoUsuario.Activo,
            Activo = true,
            EmailVerificado = false,
            FechaCreacion = DateTime.UtcNow
        };

        await _usuarioRepository.AddAsync(usuario, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Asignar roles si se proporcionaron
        if (request.RolIds != null && request.RolIds.Any())
        {
            // Los roles se asignarían aquí
        }

        return new RegistroResponse
        {
            Exitoso = true,
            Mensaje = "Usuario registrado exitosamente",
            UsuarioId = usuario.Id
        };
    }

    public async Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        // Buscar sesión por refresh token
        var sesion = await _sesionRepository.GetByRefreshTokenAsync(request.RefreshToken, cancellationToken);

        if (sesion == null || !sesion.Activo)
        {
            return new RefreshTokenResponse
            {
                Exitoso = false,
                Mensaje = "Refresh token inválido"
            };
        }

        // Verificar que el token original coincida
        var usuarioIdToken = _tokenService.ObtenerUsuarioIdDesdeToken(request.Token);
        if (usuarioIdToken != sesion.UsuarioId)
        {
            return new RefreshTokenResponse
            {
                Exitoso = false,
                Mensaje = "Token inválido"
            };
        }

        // Obtener usuario con roles
        var usuario = await _usuarioRepository.GetWithRolesAsync(sesion.UsuarioId, cancellationToken);
        if (usuario == null || !usuario.EstaActivo())
        {
            return new RefreshTokenResponse
            {
                Exitoso = false,
                Mensaje = "Usuario no encontrado o inactivo"
            };
        }

        // Obtener permisos
        var permisos = await _usuarioRepository.GetPermisosUsuarioAsync(usuario.Id, cancellationToken);

        // Crear DTO
        var usuarioAuth = _mapper.Map<UsuarioAutenticadoDto>(usuario);
        usuarioAuth.Permisos = permisos.ToList();

        // Generar nuevos tokens
        var nuevoToken = _tokenService.GenerarToken(usuarioAuth);
        var nuevoRefreshToken = _tokenService.GenerarRefreshToken();
        var expiracion = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:ExpirationMinutes"] ?? "60"));

        // Actualizar sesión
        sesion.Token = nuevoToken;
        sesion.RefreshToken = nuevoRefreshToken;
        sesion.FechaExpiracion = expiracion;
        sesion.FechaUltimaActividad = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new RefreshTokenResponse
        {
            Exitoso = true,
            Mensaje = "Token renovado exitosamente",
            Token = nuevoToken,
            RefreshToken = nuevoRefreshToken,
            Expiracion = expiracion
        };
    }

    public async Task<Result> LogoutAsync(string token, CancellationToken cancellationToken = default)
    {
        var sesion = await _sesionRepository.GetByTokenAsync(token, cancellationToken);

        if (sesion != null)
        {
            sesion.Cerrar();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result.Success();
    }

    public async Task<Result> CambiarPasswordAsync(long usuarioId, CambiarPasswordRequest request, CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(usuarioId, cancellationToken);

        if (usuario == null)
        {
            return Result.Failure("Usuario no encontrado");
        }

        // Verificar contraseña actual
        if (!_passwordService.VerifyPassword(request.PasswordActual, usuario.PasswordHash))
        {
            return Result.Failure("La contraseña actual es incorrecta");
        }

        // Actualizar contraseña
        usuario.PasswordHash = _passwordService.HashPassword(request.NuevoPassword);
        usuario.FechaUltimoCambioPassword = DateTime.UtcNow;
        usuario.RequiereCambioPassword = false;

        // Invalidar todas las sesiones
        await _sesionRepository.InvalidarSesionesUsuarioAsync(usuarioId, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> RecuperarPasswordAsync(RecuperarPasswordRequest request, CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarioRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (usuario == null)
        {
            // Por seguridad, no indicamos si el email existe o no
            return Result.Success();
        }

        // Generar token de recuperación
        usuario.TokenRecuperacion = Guid.NewGuid().ToString("N");
        usuario.FechaExpiracionRecuperacion = DateTime.UtcNow.AddHours(24);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Aquí se enviaría el email con el token
        // await _emailService.EnviarRecuperacionPasswordAsync(...)

        return Result.Success();
    }

    public async Task<Result> ResetearPasswordAsync(ResetearPasswordRequest request, CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarioRepository
            .FirstOrDefaultAsync(u => u.TokenRecuperacion == request.Token, cancellationToken);

        if (usuario == null)
        {
            return Result.Failure("Token inválido");
        }

        if (usuario.FechaExpiracionRecuperacion < DateTime.UtcNow)
        {
            return Result.Failure("El token ha expirado");
        }

        // Actualizar contraseña
        usuario.PasswordHash = _passwordService.HashPassword(request.NuevoPassword);
        usuario.TokenRecuperacion = null;
        usuario.FechaExpiracionRecuperacion = null;
        usuario.FechaUltimoCambioPassword = DateTime.UtcNow;

        // Invalidar sesiones
        await _sesionRepository.InvalidarSesionesUsuarioAsync(usuario.Id, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> VerificarEmailAsync(string token, CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarioRepository
            .FirstOrDefaultAsync(u => u.TokenVerificacionEmail == token, cancellationToken);

        if (usuario == null)
        {
            return Result.Failure("Token inválido");
        }

        if (usuario.FechaExpiracionToken < DateTime.UtcNow)
        {
            return Result.Failure("El token ha expirado");
        }

        usuario.EmailVerificado = true;
        usuario.TokenVerificacionEmail = null;
        usuario.FechaExpiracionToken = null;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<UsuarioAutenticadoDto?> GetUsuarioActualAsync(long usuarioId, CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarioRepository.GetWithRolesAsync(usuarioId, cancellationToken);

        if (usuario == null)
        {
            return null;
        }

        var permisos = await _usuarioRepository.GetPermisosUsuarioAsync(usuario.Id, cancellationToken);
        var usuarioAuth = _mapper.Map<UsuarioAutenticadoDto>(usuario);
        usuarioAuth.Permisos = permisos.ToList();

        return usuarioAuth;
    }
}

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SolarisPlatform.Application.Common.Interfaces;
using SolarisPlatform.Application.Common.Models;
using SolarisPlatform.Application.DTOs.Usuarios;
using SolarisPlatform.Domain.Entities.Seguridad;
using SolarisPlatform.Domain.Enums;
using SolarisPlatform.Domain.Interfaces;
using SolarisPlatform.Infrastructure.Persistence.Context;

namespace SolarisPlatform.Infrastructure.Services;

/// <summary>
/// Implementación del servicio de usuarios
/// </summary>
public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IRolRepository _rolRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordService _passwordService;
    private readonly IMapper _mapper;
    private readonly SolarisDbContext _context;

    public UsuarioService(
        IUsuarioRepository usuarioRepository,
        IRolRepository rolRepository,
        IUnitOfWork unitOfWork,
        IPasswordService passwordService,
        IMapper mapper,
        SolarisDbContext context)
    {
        _usuarioRepository = usuarioRepository;
        _rolRepository = rolRepository;
        _unitOfWork = unitOfWork;
        _passwordService = passwordService;
        _mapper = mapper;
        _context = context;
    }

    public async Task<UsuarioDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarioRepository.GetWithRolesAsync(id, cancellationToken);
        return usuario == null ? null : _mapper.Map<UsuarioDto>(usuario);
    }

    public async Task<UsuarioDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarioRepository.GetByEmailWithRolesAsync(email, cancellationToken);
        return usuario == null ? null : _mapper.Map<UsuarioDto>(usuario);
    }

    public async Task<PaginatedList<UsuarioListDto>> GetListAsync(
        FiltroUsuariosRequest filtro,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Usuarios
            .Include(u => u.UsuarioRoles.Where(ur => ur.Activo))
                .ThenInclude(ur => ur.Rol)
            .AsQueryable();

        // Filtros
        if (filtro.EmpresaId.HasValue)
            query = query.Where(u => u.EmpresaId == filtro.EmpresaId);

        if (filtro.SucursalId.HasValue)
            query = query.Where(u => u.SucursalId == filtro.SucursalId);

        if (!string.IsNullOrWhiteSpace(filtro.Busqueda))
        {
            var busqueda = filtro.Busqueda.ToLower();
            query = query.Where(u =>
                u.Email.ToLower().Contains(busqueda) ||
                u.Nombres.ToLower().Contains(busqueda) ||
                u.Apellidos.ToLower().Contains(busqueda) ||
                (u.NombreUsuario != null && u.NombreUsuario.ToLower().Contains(busqueda)));
        }

        if (filtro.Estado.HasValue)
            query = query.Where(u => (int)u.Estado == filtro.Estado);

        if (filtro.Activo.HasValue)
            query = query.Where(u => u.Activo == filtro.Activo);

        if (filtro.RolId.HasValue)
            query = query.Where(u => u.UsuarioRoles.Any(ur => ur.RolId == filtro.RolId && ur.Activo));

        // Ordenamiento
        query = filtro.OrdenarPor?.ToLower() switch
        {
            "email" => filtro.OrdenDescendente
                ? query.OrderByDescending(u => u.Email)
                : query.OrderBy(u => u.Email),
            "nombres" => filtro.OrdenDescendente
                ? query.OrderByDescending(u => u.Nombres)
                : query.OrderBy(u => u.Nombres),
            "ultimoacceso" => filtro.OrdenDescendente
                ? query.OrderByDescending(u => u.UltimoAcceso)
                : query.OrderBy(u => u.UltimoAcceso),
            "fechacreacion" => filtro.OrdenDescendente
                ? query.OrderByDescending(u => u.FechaCreacion)
                : query.OrderBy(u => u.FechaCreacion),
            _ => query.OrderBy(u => u.Apellidos).ThenBy(u => u.Nombres)
        };

        // Contar total
        var total = await query.CountAsync(cancellationToken);

        // Paginar
        var items = await query
            .Skip((filtro.Pagina - 1) * filtro.ElementosPorPagina)
            .Take(filtro.ElementosPorPagina)
            .ToListAsync(cancellationToken);

        var dtos = _mapper.Map<List<UsuarioListDto>>(items);

        return new PaginatedList<UsuarioListDto>(dtos, total, filtro.Pagina, filtro.ElementosPorPagina);
    }

    public async Task<Result<UsuarioDto>> CreateAsync(
        CrearUsuarioRequest request,
        long usuarioCreacion,
        CancellationToken cancellationToken = default)
    {
        // Verificar email único
        if (await _usuarioRepository.EmailExisteAsync(request.Email, null, cancellationToken))
        {
            return Result<UsuarioDto>.Failure("El email ya está registrado");
        }

        // ✅ FIX: Validar que todos los roles existen ANTES de insertar
        // Antes se insertaban los UsuarioRoles sin validar, causando FK violation → 500
        if (request.RolesIds.Any())
        {
            var rolesExistentes = await _context.Roles
                .Where(r => request.RolesIds.Contains(r.Id) && r.Activo)
                .Select(r => r.Id)
                .ToListAsync(cancellationToken);

            var noExisten = request.RolesIds.Except(rolesExistentes).ToList();
            if (noExisten.Any())
            {
                return Result<UsuarioDto>.Failure(
                    $"Los siguientes roles no existen o están inactivos: {string.Join(", ", noExisten)}");
            }
        }

        // Crear usuario usando el contexto directamente para evitar conflictos de tracking
        var usuario = new Usuario
        {
            EmpresaId = request.EmpresaId,
            SucursalId = request.SucursalId,
            Email = request.Email.Trim().ToLower(),
            NombreUsuario = request.NombreUsuario?.Trim(),
            Nombres = request.Nombres.Trim(),
            Apellidos = request.Apellidos.Trim(),
            NumeroIdentificacion = request.NumeroIdentificacion?.Trim(),
            Telefono = request.Telefono?.Trim(),
            Celular = request.Celular?.Trim(),
            FechaNacimiento = request.FechaNacimiento,
            Genero = !string.IsNullOrEmpty(request.Genero) ? request.Genero[0] : null,
            PasswordHash = _passwordService.HashPassword(request.Password),
            Estado = EstadoUsuario.Activo,
            Activo = true,
            FechaCreacion = DateTime.UtcNow,
            UsuarioCreacion = usuarioCreacion
        };

        await _context.Usuarios.AddAsync(usuario, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Asignar roles luego de tener el Id generado
        if (request.RolesIds.Any())
        {
            var esPrimero = true;
            foreach (var rolId in request.RolesIds)
            {
                var usuarioRol = new UsuarioRol
                {
                    UsuarioId = usuario.Id,
                    RolId = rolId,
                    EsPrincipal = esPrimero,
                    Activo = true,
                    FechaCreacion = DateTime.UtcNow,
                    UsuarioCreacion = usuarioCreacion
                };
                await _context.UsuarioRoles.AddAsync(usuarioRol, cancellationToken);
                esPrimero = false;
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        // Obtener usuario completo con roles cargados
        var usuarioCreado = await _usuarioRepository.GetWithRolesAsync(usuario.Id, cancellationToken);
        return Result<UsuarioDto>.Success(_mapper.Map<UsuarioDto>(usuarioCreado!));
    }

    public async Task<Result<UsuarioDto>> UpdateAsync(
        ActualizarUsuarioRequest request,
        long usuarioModificacion,
        CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarioRepository.GetWithRolesAsync(request.Id, cancellationToken);
        if (usuario == null)
        {
            return Result<UsuarioDto>.Failure("Usuario no encontrado");
        }

        // Actualizar campos
        usuario.SucursalId = request.SucursalId;
        usuario.NombreUsuario = request.NombreUsuario;
        usuario.Nombres = request.Nombres;
        usuario.Apellidos = request.Apellidos;
        usuario.NumeroIdentificacion = request.NumeroIdentificacion;
        usuario.Telefono = request.Telefono;
        usuario.Celular = request.Celular;
        usuario.FechaNacimiento = request.FechaNacimiento;
        usuario.Genero = !string.IsNullOrEmpty(request.Genero) ? request.Genero[0] : null;
        usuario.FotoUrl = request.FotoUrl;
        usuario.IdiomaPreferido = request.IdiomaPreferido;
        usuario.TemaPreferido = request.TemaPreferido;
        usuario.UsuarioModificacion = usuarioModificacion;
        usuario.FechaModificacion = DateTime.UtcNow;

        // ✅ FIX: No llamar _usuarioRepository.UpdateAsync.
        // GetWithRolesAsync ya trackea la entidad; llamar UpdateAsync después forzaba
        // EntityState.Modified sobre el grafo completo (incluyendo nav props Empresa,
        // Sucursal, UsuarioRoles) → DbUpdateException por conflictos de FK al guardar.
        // EF detecta los cambios automáticamente desde el change tracker.

        // Actualizar roles si se proporcionaron
        if (request.RolesIds.Any())
        {
            // Desactivar roles actuales
            foreach (var ur in usuario.UsuarioRoles.Where(ur => ur.Activo))
            {
                ur.Activo = false;
            }

            // Asignar nuevos roles
            var esPrimero = true;
            foreach (var rolId in request.RolesIds)
            {
                var existente = usuario.UsuarioRoles.FirstOrDefault(ur => ur.RolId == rolId);
                if (existente != null)
                {
                    existente.Activo = true;
                    existente.EsPrincipal = esPrimero;
                }
                else
                {
                    var usuarioRol = new UsuarioRol
                    {
                        UsuarioId = usuario.Id,
                        RolId = rolId,
                        EsPrincipal = esPrimero,
                        Activo = true,
                        FechaCreacion = DateTime.UtcNow,
                        UsuarioCreacion = usuarioModificacion
                    };
                    await _context.UsuarioRoles.AddAsync(usuarioRol, cancellationToken);
                }
                esPrimero = false;
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var usuarioActualizado = await _usuarioRepository.GetWithRolesAsync(usuario.Id, cancellationToken);
        return Result<UsuarioDto>.Success(_mapper.Map<UsuarioDto>(usuarioActualizado!));
    }

    public async Task<Result> DeleteAsync(
        long id,
        long usuarioEliminacion,
        CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(id, cancellationToken);
        if (usuario == null)
        {
            return Result.Failure("Usuario no encontrado");
        }

        // ✅ FIX: Soft delete en lugar de hard delete.
        // Usuario extiende SoftDeletableEntity — _usuarioRepository.DeleteAsync hacía
        // _dbSet.Remove() generando un hard DELETE que fallaba con DbUpdateException
        // (FK constraints desde SesionesUsuario, UsuarioRoles, etc.).
        usuario.Eliminado = true;
        usuario.FechaEliminacion = DateTime.UtcNow;
        usuario.UsuarioEliminacion = usuarioEliminacion;
        usuario.Activo = false;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> ActivarAsync(
        long id,
        long usuarioModificacion,
        CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(id, cancellationToken);
        if (usuario == null)
        {
            return Result.Failure("Usuario no encontrado");
        }

        usuario.Estado = EstadoUsuario.Activo;
        usuario.Activo = true;
        usuario.UsuarioModificacion = usuarioModificacion;
        usuario.FechaModificacion = DateTime.UtcNow;
        // ✅ FIX: GetByIdAsync (FindAsync) ya trackea la entidad. EF detecta los cambios
        // automáticamente — llamar UpdateAsync forzaba EntityState.Modified innecesariamente.
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DesactivarAsync(
        long id,
        long usuarioModificacion,
        CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(id, cancellationToken);
        if (usuario == null)
        {
            return Result.Failure("Usuario no encontrado");
        }

        usuario.Estado = EstadoUsuario.Inactivo;
        usuario.Activo = false;
        usuario.UsuarioModificacion = usuarioModificacion;
        usuario.FechaModificacion = DateTime.UtcNow;
        // ✅ FIX: EF change tracking automático — no se necesita UpdateAsync explícito.
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> BloquearAsync(
        long id,
        int minutos,
        long usuarioModificacion,
        CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(id, cancellationToken);
        if (usuario == null)
        {
            return Result.Failure("Usuario no encontrado");
        }

        // ✅ Usar el método helper de la entidad Usuario
        usuario.Bloquear(minutos);
        usuario.UsuarioModificacion = usuarioModificacion;
        usuario.FechaModificacion = DateTime.UtcNow;
        // ✅ FIX: EF change tracking automático — no se necesita UpdateAsync explícito.
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DesbloquearAsync(
        long id,
        long usuarioModificacion,
        CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(id, cancellationToken);
        if (usuario == null)
        {
            return Result.Failure("Usuario no encontrado");
        }

        // ✅ Usar el método helper de la entidad Usuario
        usuario.Desbloquear();
        usuario.UsuarioModificacion = usuarioModificacion;
        usuario.FechaModificacion = DateTime.UtcNow;
        // ✅ FIX: EF change tracking automático — no se necesita UpdateAsync explícito.
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> ResetearPasswordAsync(
        long id,
        long usuarioModificacion,
        CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(id, cancellationToken);
        if (usuario == null)
        {
            return Result.Failure("Usuario no encontrado");
        }

        var passwordTemporal = _passwordService.GenerarPasswordTemporal();
        usuario.PasswordHash = _passwordService.HashPassword(passwordTemporal);
        usuario.RequiereCambioPassword = true;
        usuario.UsuarioModificacion = usuarioModificacion;
        usuario.FechaModificacion = DateTime.UtcNow;
        // ✅ FIX: EF change tracking automático — no se necesita UpdateAsync explícito.
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // TODO: Enviar email con password temporal
        // await _emailService.EnviarPasswordTemporalAsync(usuario.Email, usuario.Nombres, passwordTemporal);

        return Result.Success();
    }
}

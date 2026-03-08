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

        query = filtro.OrdenarPor?.ToLower() switch
        {
            "email"        => filtro.OrdenDescendente ? query.OrderByDescending(u => u.Email)        : query.OrderBy(u => u.Email),
            "nombres"      => filtro.OrdenDescendente ? query.OrderByDescending(u => u.Nombres)      : query.OrderBy(u => u.Nombres),
            "ultimoacceso" => filtro.OrdenDescendente ? query.OrderByDescending(u => u.UltimoAcceso) : query.OrderBy(u => u.UltimoAcceso),
            "fechacreacion"=> filtro.OrdenDescendente ? query.OrderByDescending(u => u.FechaCreacion): query.OrderBy(u => u.FechaCreacion),
            _              => query.OrderBy(u => u.Apellidos).ThenBy(u => u.Nombres)
        };

        var total = await query.CountAsync(cancellationToken);

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
        if (await _usuarioRepository.EmailExisteAsync(request.Email, null, cancellationToken))
            return Result<UsuarioDto>.Failure("El email ya está registrado");

        if (request.RolesIds.Any())
        {
            var rolesExistentes = await _context.Roles
                .Where(r => request.RolesIds.Contains(r.Id) && r.Activo)
                .Select(r => r.Id)
                .ToListAsync(cancellationToken);

            var noExisten = request.RolesIds.Except(rolesExistentes).ToList();
            if (noExisten.Any())
                return Result<UsuarioDto>.Failure($"Los siguientes roles no existen o están inactivos: {string.Join(", ", noExisten)}");
        }

        var usuario = new Usuario
        {
            EmpresaId             = request.EmpresaId,
            SucursalId            = request.SucursalId,
            Email                 = request.Email.Trim().ToLower(),
            NombreUsuario         = request.NombreUsuario?.Trim(),
            Nombres               = request.Nombres.Trim(),
            Apellidos             = request.Apellidos.Trim(),
            NumeroIdentificacion  = request.NumeroIdentificacion?.Trim(),
            Telefono              = request.Telefono?.Trim(),
            Celular               = request.Celular?.Trim(),
            FechaNacimiento       = request.FechaNacimiento,
            Genero                = !string.IsNullOrEmpty(request.Genero) ? request.Genero[0] : null,
            PasswordHash          = _passwordService.HashPassword(request.Password),
            Estado                = EstadoUsuario.Activo,
            Activo                = true,
            FechaCreacion         = DateTime.UtcNow,
            UsuarioCreacion       = usuarioCreacion
        };

        await _context.Usuarios.AddAsync(usuario, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (request.RolesIds.Any())
        {
            var esPrimero = true;
            foreach (var rolId in request.RolesIds)
            {
                var usuarioRol = new UsuarioRol
                {
                    UsuarioId       = usuario.Id,
                    RolId           = rolId,
                    EsPrincipal     = esPrimero,
                    Activo          = true,
                    FechaCreacion   = DateTime.UtcNow,
                    UsuarioCreacion = usuarioCreacion
                };
                await _context.UsuarioRoles.AddAsync(usuarioRol, cancellationToken);
                esPrimero = false;
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var usuarioCreado = await _usuarioRepository.GetWithRolesAsync(usuario.Id, cancellationToken);
        return Result<UsuarioDto>.Success(_mapper.Map<UsuarioDto>(usuarioCreado!));
    }

    public async Task<Result<UsuarioDto>> UpdateAsync(
        ActualizarUsuarioRequest request,
        long usuarioModificacion,
        CancellationToken cancellationToken = default)
    {
        // ── Cargar entidad por su PK sin navegaciones para evitar que EF marque
        //    Empresa / Sucursal / Rol como Modified al hacer SaveChanges. ─────────
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

        if (usuario == null)
            return Result<UsuarioDto>.Failure("Usuario no encontrado");

        usuario.SucursalId           = request.SucursalId;
        usuario.NombreUsuario        = request.NombreUsuario;
        usuario.Nombres              = request.Nombres;
        usuario.Apellidos            = request.Apellidos;
        usuario.NumeroIdentificacion = request.NumeroIdentificacion;
        usuario.Telefono             = request.Telefono;
        usuario.Celular              = request.Celular;
        usuario.FechaNacimiento      = request.FechaNacimiento;
        usuario.Genero               = !string.IsNullOrEmpty(request.Genero) ? request.Genero[0] : null;
        usuario.FotoUrl              = request.FotoUrl;
        usuario.IdiomaPreferido      = request.IdiomaPreferido;
        usuario.TemaPreferido        = request.TemaPreferido;
        usuario.UsuarioModificacion  = usuarioModificacion;
        usuario.FechaModificacion    = DateTime.UtcNow;

        // EF detecta los cambios automáticamente desde el change tracker.
        // NO llamar _usuarioRepository.UpdateAsync → evita EntityState.Modified
        // sobre el grafo completo (causa DbUpdateException por FK a Empresa/Sucursal).

        // ── Actualizar roles ───────────────────────────────────────────────────
        if (request.RolesIds.Any())
        {
            // FIX: buscar en la BD con IgnoreQueryFilters para encontrar registros
            // previamente desactivados — evita violar el UNIQUE(usuario_id, rol_id).
            var rolesActuales = await _context.UsuarioRoles
                .IgnoreQueryFilters()
                .Where(ur => ur.UsuarioId == usuario.Id)
                .ToListAsync(cancellationToken);

            // Desactivar todos los roles actuales
            foreach (var ur in rolesActuales.Where(ur => ur.Activo))
                ur.Activo = false;

            var esPrimero = true;
            foreach (var rolId in request.RolesIds)
            {
                var existente = rolesActuales.FirstOrDefault(ur => ur.RolId == rolId);
                if (existente != null)
                {
                    // Reactivar el registro existente
                    existente.Activo      = true;
                    existente.EsPrincipal = esPrimero;
                }
                else
                {
                    // Solo crear si realmente no existe ningún registro previo
                    var nuevoRol = new UsuarioRol
                    {
                        UsuarioId       = usuario.Id,
                        RolId           = rolId,
                        EsPrincipal     = esPrimero,
                        Activo          = true,
                        FechaCreacion   = DateTime.UtcNow,
                        UsuarioCreacion = usuarioModificacion
                    };
                    await _context.UsuarioRoles.AddAsync(nuevoRol, cancellationToken);
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
        // FIX: cargar solo escalares — evita que UpdateAsync marque nav-props como Modified.
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (usuario == null)
            return Result.Failure("Usuario no encontrado");

        // Soft delete — no usar _dbSet.Remove() que genera hard-DELETE
        // y falla por FK desde SesionUsuario, UsuarioRoles, etc.
        usuario.Eliminado          = true;
        usuario.FechaEliminacion   = DateTime.UtcNow;
        usuario.UsuarioEliminacion = usuarioEliminacion;
        usuario.Activo             = false;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> ActivarAsync(
        long id,
        long usuarioModificacion,
        CancellationToken cancellationToken = default)
    {
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (usuario == null)
            return Result.Failure("Usuario no encontrado");

        usuario.Estado              = EstadoUsuario.Activo;
        usuario.Activo              = true;
        usuario.UsuarioModificacion = usuarioModificacion;
        usuario.FechaModificacion   = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DesactivarAsync(
        long id,
        long usuarioModificacion,
        CancellationToken cancellationToken = default)
    {
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (usuario == null)
            return Result.Failure("Usuario no encontrado");

        usuario.Estado              = EstadoUsuario.Inactivo;
        usuario.Activo              = false;
        usuario.UsuarioModificacion = usuarioModificacion;
        usuario.FechaModificacion   = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> BloquearAsync(
        long id,
        int minutos,
        long usuarioModificacion,
        CancellationToken cancellationToken = default)
    {
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (usuario == null)
            return Result.Failure("Usuario no encontrado");

        usuario.Bloquear(minutos);
        usuario.UsuarioModificacion = usuarioModificacion;
        usuario.FechaModificacion   = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DesbloquearAsync(
        long id,
        long usuarioModificacion,
        CancellationToken cancellationToken = default)
    {
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (usuario == null)
            return Result.Failure("Usuario no encontrado");

        usuario.Desbloquear();
        usuario.UsuarioModificacion = usuarioModificacion;
        usuario.FechaModificacion   = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> ResetearPasswordAsync(
        long id,
        long usuarioModificacion,
        CancellationToken cancellationToken = default)
    {
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (usuario == null)
            return Result.Failure("Usuario no encontrado");

        var passwordTemporal = _passwordService.GenerarPasswordTemporal();
        usuario.PasswordHash         = _passwordService.HashPassword(passwordTemporal);
        usuario.RequiereCambioPassword = true;
        usuario.UsuarioModificacion  = usuarioModificacion;
        usuario.FechaModificacion    = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // TODO: await _emailService.EnviarPasswordTemporalAsync(usuario.Email, usuario.Nombres, passwordTemporal);

        return Result.Success();
    }
}
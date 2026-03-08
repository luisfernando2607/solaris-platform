
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SolarisPlatform.Application.Common.Interfaces;
using SolarisPlatform.Application.Common.Models;
using SolarisPlatform.Application.DTOs.Roles;
using SolarisPlatform.Domain.Entities.Seguridad;
using SolarisPlatform.Domain.Interfaces;
using SolarisPlatform.Infrastructure.Persistence.Context;

namespace SolarisPlatform.Infrastructure.Services;

/// <summary>
/// Implementación del servicio de roles
/// </summary>
public class RolService : IRolService
{
    private readonly IRolRepository _rolRepository;
    private readonly IModuloRepository _moduloRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly SolarisDbContext _context;

    public RolService(
        IRolRepository rolRepository,
        IModuloRepository moduloRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        SolarisDbContext context)
    {
        _rolRepository = rolRepository;
        _moduloRepository = moduloRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _context = context;
    }

    public async Task<RolDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var rol = await _rolRepository.GetWithPermisosAsync(id, cancellationToken);
        return rol == null ? null : _mapper.Map<RolDto>(rol);
    }

    public async Task<IEnumerable<RolListDto>> GetAllAsync(
        long? empresaId = null,
        CancellationToken cancellationToken = default)
    {
        var roles = await _rolRepository.GetByEmpresaAsync(empresaId, cancellationToken);
        return _mapper.Map<IEnumerable<RolListDto>>(roles);
    }

    public async Task<IEnumerable<RolListDto>> GetDisponiblesAsync(
        long empresaId,
        CancellationToken cancellationToken = default)
    {
        var roles = await _rolRepository.GetRolesDisponiblesAsync(empresaId, cancellationToken);
        return _mapper.Map<IEnumerable<RolListDto>>(roles);
    }

    public async Task<Result<RolDto>> CreateAsync(
        CrearRolRequest request,
        long usuarioCreacion,
        CancellationToken cancellationToken = default)
    {
        // Verificar código único
        var existente = await _rolRepository.GetByCodigoAsync(request.Codigo, request.EmpresaId, cancellationToken);
        if (existente != null)
        {
            return Result<RolDto>.Failure("Ya existe un rol con ese código");
        }

        // Crear rol
        var rol = _mapper.Map<Rol>(request);
        rol.UsuarioCreacion = usuarioCreacion;

        await _rolRepository.AddAsync(rol, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Asignar permisos
        if (request.PermisosIds.Any())
        {
            foreach (var permisoId in request.PermisosIds)
            {
                var rolPermiso = new RolPermiso
                {
                    RolId = rol.Id,
                    PermisoId = permisoId,
                    FechaCreacion = DateTime.UtcNow,
                    UsuarioCreacion = usuarioCreacion
                };
                await _context.RolPermisos.AddAsync(rolPermiso, cancellationToken);
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        // Obtener rol completo
        var rolCreado = await _rolRepository.GetWithPermisosAsync(rol.Id, cancellationToken);
        return Result<RolDto>.Success(_mapper.Map<RolDto>(rolCreado!));
    }

    public async Task<Result<RolDto>> UpdateAsync(
        ActualizarRolRequest request,
        long usuarioModificacion,
        CancellationToken cancellationToken = default)
    {
        // ✅ FIX: Buscar directamente con tracking desde _context en lugar del repository
        // (GetWithPermisosAsync usa AsNoTracking internamente, lo que causaba el 500)
        var rol = await _context.Roles
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        if (rol == null)
            return Result<RolDto>.Failure("Rol no encontrado");

        if (rol.EsSistema)
            return Result<RolDto>.Failure("No se puede modificar un rol de sistema");

        // Actualizar campos — EF ya trackea este objeto, no hay que hacer Attach ni marcar Modified
        rol.Nombre = request.Nombre;
        rol.Descripcion = request.Descripcion;
        rol.Nivel = request.Nivel;
        rol.Color = request.Color;
        rol.Icono = request.Icono;
        rol.Activo = request.Activo;
        rol.UsuarioModificacion = usuarioModificacion;
        rol.FechaModificacion = DateTime.UtcNow;

        // Limpiar permisos actuales
        var permisosActuales = await _context.RolPermisos
            .Where(rp => rp.RolId == rol.Id)
            .ToListAsync(cancellationToken);
        _context.RolPermisos.RemoveRange(permisosActuales);

        // Agregar nuevos permisos
        foreach (var permisoId in request.PermisosIds)
        {
            _context.RolPermisos.Add(new RolPermiso
            {
                RolId = rol.Id,
                PermisoId = permisoId,
                FechaCreacion = DateTime.UtcNow,
                UsuarioCreacion = usuarioModificacion
            });
        }

        // ✅ FIX: Un solo SaveChanges — todo en la misma transacción
        // Antes había dos SaveChanges separados lo que causaba inconsistencias de tracking
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var rolActualizado = await _rolRepository.GetWithPermisosAsync(rol.Id, cancellationToken);
        return Result<RolDto>.Success(_mapper.Map<RolDto>(rolActualizado!));
    }

    public async Task<Result> DeleteAsync(
        long id,
        long usuarioEliminacion,
        CancellationToken cancellationToken = default)
    {
        // ✅ FIX: Buscar con tracking desde _context desde el inicio
        // Antes se llamaba GetWithPermisosAsync (AsNoTracking) y luego FindAsync por separado,
        // lo que generaba doble query y posibles conflictos de tracking
        var rol = await _context.Roles
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (rol == null)
            return Result.Failure("Rol no encontrado");

        if (rol.EsSistema)
            return Result.Failure("No se puede eliminar un rol de sistema");

        // Verificar usuarios asignados
        var tieneUsuarios = await _context.UsuarioRoles
            .AnyAsync(ur => ur.RolId == id && ur.Activo, cancellationToken);

        if (tieneUsuarios)
            return Result.Failure("No se puede eliminar el rol porque tiene usuarios asignados");

        // Eliminar permisos asociados (RolPermisos tiene FK a Roles — debe ir primero)
        var permisos = await _context.RolPermisos
            .Where(rp => rp.RolId == id)
            .ToListAsync(cancellationToken);
        _context.RolPermisos.RemoveRange(permisos);

        // ✅ FIX: Soft delete en lugar de Remove().
        // Rol extiende SoftDeletableEntity — el diseño exige soft delete.
        // _context.Roles.Remove() generaba un hard DELETE en la BD que fallaba con
        // DbUpdateException (FK constraint de UsuarioRoles → Roles, incluso registros inactivos).
        // Con soft delete EF genera un UPDATE limpio sin tocar las FK dependientes.
        rol.Eliminado = true;
        rol.FechaEliminacion = DateTime.UtcNow;
        rol.UsuarioEliminacion = usuarioEliminacion;
        rol.Activo = false;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<IEnumerable<ModuloConPermisosDto>> GetModulosConPermisosAsync(
        CancellationToken cancellationToken = default)
    {
        var modulos = await _moduloRepository.GetMenuPrincipalAsync(cancellationToken);
        return _mapper.Map<IEnumerable<ModuloConPermisosDto>>(modulos);
    }
}

using Microsoft.EntityFrameworkCore;
using SolarisPlatform.Domain.Entities.Seguridad;
using SolarisPlatform.Domain.Entities.Empresas;
using SolarisPlatform.Domain.Interfaces;
using SolarisPlatform.Infrastructure.Persistence.Context;

namespace SolarisPlatform.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repositorio de usuarios
/// </summary>
public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(SolarisDbContext context) : base(context)
    {
    }

    public async Task<Usuario?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower(), cancellationToken);
    }

    public async Task<Usuario?> GetByNombreUsuarioAsync(string nombreUsuario, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.NombreUsuario != null && u.NombreUsuario.ToLower() == nombreUsuario.ToLower(), cancellationToken);
    }

    public async Task<Usuario?> GetWithRolesAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.Empresa)
            .Include(u => u.Sucursal)
            .Include(u => u.UsuarioRoles.Where(ur => ur.Activo))
                .ThenInclude(ur => ur.Rol)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<Usuario?> GetByEmailWithRolesAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.Empresa)
            .Include(u => u.Sucursal)
            .Include(u => u.UsuarioRoles.Where(ur => ur.Activo))
                .ThenInclude(ur => ur.Rol)
                    .ThenInclude(r => r.RolPermisos)
                        .ThenInclude(rp => rp.Permiso)
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower(), cancellationToken);
    }

    public async Task<IEnumerable<Usuario>> GetByEmpresaAsync(long empresaId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.UsuarioRoles.Where(ur => ur.Activo))
                .ThenInclude(ur => ur.Rol)
            .Where(u => u.EmpresaId == empresaId)
            .OrderBy(u => u.Apellidos)
            .ThenBy(u => u.Nombres)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> EmailExisteAsync(string email, long? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(u => u.Email.ToLower() == email.ToLower());
        
        if (excludeId.HasValue)
            query = query.Where(u => u.Id != excludeId.Value);
        
        return await query.AnyAsync(cancellationToken);
    }

    public async Task<IEnumerable<string>> GetPermisosUsuarioAsync(long usuarioId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(u => u.Id == usuarioId)
            .SelectMany(u => u.UsuarioRoles.Where(ur => ur.Activo))
            .SelectMany(ur => ur.Rol.RolPermisos)
            .Select(rp => rp.Permiso.Codigo)
            .Distinct()
            .ToListAsync(cancellationToken);
    }
}

/// <summary>
/// Repositorio de roles
/// </summary>
public class RolRepository : Repository<Rol>, IRolRepository
{
    public RolRepository(SolarisDbContext context) : base(context)
    {
    }

    public async Task<Rol?> GetByCodigoAsync(string codigo, long? empresaId = null, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(r => r.Codigo == codigo && r.EmpresaId == empresaId, cancellationToken);
    }

    public async Task<Rol?> GetWithPermisosAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.RolPermisos)
                .ThenInclude(rp => rp.Permiso)
                    .ThenInclude(p => p.Modulo)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Rol>> GetByEmpresaAsync(long? empresaId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.UsuarioRoles)
            .Where(r => r.EmpresaId == empresaId || r.EmpresaId == null)
            .OrderBy(r => r.Nivel)
            .ThenBy(r => r.Nombre)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Rol>> GetRolesDisponiblesAsync(long empresaId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => (r.EmpresaId == empresaId || r.EmpresaId == null) && r.Activo)
            .OrderBy(r => r.Nivel)
            .ThenBy(r => r.Nombre)
            .ToListAsync(cancellationToken);
    }
}

/// <summary>
/// Repositorio de sesiones de usuario
/// </summary>
public class SesionUsuarioRepository : Repository<SesionUsuario>, ISesionUsuarioRepository
{
    public SesionUsuarioRepository(SolarisDbContext context) : base(context)
    {
    }

    public async Task<SesionUsuario?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.Usuario)
            .FirstOrDefaultAsync(s => s.Token == token && s.Activo, cancellationToken);
    }

    public async Task<SesionUsuario?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.Usuario)
            .FirstOrDefaultAsync(s => s.RefreshToken == refreshToken && s.Activo, cancellationToken);
    }

    public async Task<IEnumerable<SesionUsuario>> GetActivasByUsuarioAsync(long usuarioId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.UsuarioId == usuarioId && s.Activo)
            .OrderByDescending(s => s.FechaUltimaActividad)
            .ToListAsync(cancellationToken);
    }

    public async Task InvalidarSesionesUsuarioAsync(long usuarioId, CancellationToken cancellationToken = default)
    {
        var sesiones = await _dbSet
            .Where(s => s.UsuarioId == usuarioId && s.Activo)
            .ToListAsync(cancellationToken);

        foreach (var sesion in sesiones)
        {
            sesion.Cerrar();
        }
    }

    public async Task LimpiarSesionesExpiradasAsync(CancellationToken cancellationToken = default)
    {
        var sesionesExpiradas = await _dbSet
            .Where(s => s.Activo && s.FechaExpiracion < DateTime.UtcNow)
            .ToListAsync(cancellationToken);

        foreach (var sesion in sesionesExpiradas)
        {
            sesion.Activo = false;
            sesion.FechaCierre = DateTime.UtcNow;
        }
    }
}

/// <summary>
/// Repositorio de empresas
/// </summary>
public class EmpresaRepository : Repository<Empresa>, IEmpresaRepository
{
    public EmpresaRepository(SolarisDbContext context) : base(context)
    {
    }

    public async Task<Empresa?> GetByCodigoAsync(string codigo, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(e => e.Codigo == codigo, cancellationToken);
    }

    public async Task<Empresa?> GetByRucAsync(string ruc, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(e => e.NumeroIdentificacion == ruc, cancellationToken);
    }

    public async Task<Empresa?> GetWithSucursalesAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(e => e.Sucursales.Where(s => !s.Eliminado))
            .Include(e => e.MonedaPrincipal)
            .Include(e => e.Pais)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<bool> CodigoExisteAsync(string codigo, long? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(e => e.Codigo == codigo);
        
        if (excludeId.HasValue)
            query = query.Where(e => e.Id != excludeId.Value);
        
        return await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> RucExisteAsync(string ruc, long? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(e => e.NumeroIdentificacion == ruc);
        
        if (excludeId.HasValue)
            query = query.Where(e => e.Id != excludeId.Value);
        
        return await query.AnyAsync(cancellationToken);
    }
}

/// <summary>
/// Repositorio de sucursales
/// </summary>
public class SucursalRepository : Repository<Sucursal>, ISucursalRepository
{
    public SucursalRepository(SolarisDbContext context) : base(context)
    {
    }

    public async Task<Sucursal?> GetByCodigoAsync(long empresaId, string codigo, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(s => s.EmpresaId == empresaId && s.Codigo == codigo, cancellationToken);
    }

    public async Task<IEnumerable<Sucursal>> GetByEmpresaAsync(long empresaId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.Ciudad)
            .Where(s => s.EmpresaId == empresaId)
            .OrderBy(s => s.Nombre)
            .ToListAsync(cancellationToken);
    }

    public async Task<Sucursal?> GetPrincipalAsync(long empresaId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(s => s.EmpresaId == empresaId && s.EsPrincipal, cancellationToken);
    }
}

/// <summary>
/// Repositorio de módulos
/// </summary>
public class ModuloRepository : Repository<Modulo>, IModuloRepository
{
    public ModuloRepository(SolarisDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Modulo>> GetMenuPrincipalAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(m => m.SubModulos.Where(sm => sm.Activo && sm.EsMenu))
            .Include(m => m.Permisos.Where(p => p.Activo))
            .Where(m => m.ModuloPadreId == null && m.Activo && m.EsMenu)
            .OrderBy(m => m.Orden)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Modulo>> GetByModuloPadreAsync(long? moduloPadreId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(m => m.Permisos.Where(p => p.Activo))
            .Where(m => m.ModuloPadreId == moduloPadreId && m.Activo)
            .OrderBy(m => m.Orden)
            .ToListAsync(cancellationToken);
    }

    public async Task<Modulo?> GetByCodigoAsync(string codigo, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(m => m.Permisos)
            .FirstOrDefaultAsync(m => m.Codigo == codigo, cancellationToken);
    }
}

/// <summary>
/// Repositorio de permisos
/// </summary>
public class PermisoRepository : Repository<Permiso>, IPermisoRepository
{
    public PermisoRepository(SolarisDbContext context) : base(context)
    {
    }

    public async Task<Permiso?> GetByCodigoAsync(string codigo, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Modulo)
            .FirstOrDefaultAsync(p => p.Codigo == codigo, cancellationToken);
    }

    public async Task<IEnumerable<Permiso>> GetByModuloAsync(long moduloId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.ModuloId == moduloId && p.Activo)
            .OrderBy(p => p.Nombre)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Permiso>> GetByRolAsync(long rolId, CancellationToken cancellationToken = default)
    {
        return await _context.RolPermisos
            .Where(rp => rp.RolId == rolId)
            .Select(rp => rp.Permiso)
            .Include(p => p.Modulo)
            .ToListAsync(cancellationToken);
    }
}

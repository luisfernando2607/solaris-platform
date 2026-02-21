using SolarisPlatform.Domain.Entities.Seguridad;
using SolarisPlatform.Domain.Entities.Empresas;

namespace SolarisPlatform.Domain.Interfaces;

public interface IUsuarioRepository : IRepository<Usuario>
{
    Task<Usuario?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Usuario?> GetByNombreUsuarioAsync(string nombreUsuario, CancellationToken cancellationToken = default);
    Task<Usuario?> GetWithRolesAsync(long id, CancellationToken cancellationToken = default);
    Task<Usuario?> GetByEmailWithRolesAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<Usuario>> GetByEmpresaAsync(long empresaId, CancellationToken cancellationToken = default);
    Task<bool> EmailExisteAsync(string email, long? excludeId = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetPermisosUsuarioAsync(long usuarioId, CancellationToken cancellationToken = default);
}

public interface IRolRepository : IRepository<Rol>
{
    Task<Rol?> GetByCodigoAsync(string codigo, long? empresaId = null, CancellationToken cancellationToken = default);
    Task<Rol?> GetWithPermisosAsync(long id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Rol>> GetByEmpresaAsync(long? empresaId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Rol>> GetRolesDisponiblesAsync(long empresaId, CancellationToken cancellationToken = default);
}

public interface ISesionUsuarioRepository : IRepository<SesionUsuario>
{
    Task<SesionUsuario?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<SesionUsuario?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<IEnumerable<SesionUsuario>> GetActivasByUsuarioAsync(long usuarioId, CancellationToken cancellationToken = default);
    Task InvalidarSesionesUsuarioAsync(long usuarioId, CancellationToken cancellationToken = default);
    Task LimpiarSesionesExpiradasAsync(CancellationToken cancellationToken = default);
}

public interface IEmpresaRepository : IRepository<Empresa>
{
    Task<Empresa?> GetByCodigoAsync(string codigo, CancellationToken cancellationToken = default);
    Task<Empresa?> GetByRucAsync(string ruc, CancellationToken cancellationToken = default);
    Task<Empresa?> GetWithSucursalesAsync(long id, CancellationToken cancellationToken = default);
    Task<bool> CodigoExisteAsync(string codigo, long? excludeId = null, CancellationToken cancellationToken = default);
    Task<bool> RucExisteAsync(string ruc, long? excludeId = null, CancellationToken cancellationToken = default);
}

public interface ISucursalRepository : IRepository<Sucursal>
{
    Task<Sucursal?> GetByCodigoAsync(long empresaId, string codigo, CancellationToken cancellationToken = default);
    Task<IEnumerable<Sucursal>> GetByEmpresaAsync(long empresaId, CancellationToken cancellationToken = default);
    Task<Sucursal?> GetPrincipalAsync(long empresaId, CancellationToken cancellationToken = default);
}

public interface IModuloRepository : IRepository<Modulo>
{
    Task<IEnumerable<Modulo>> GetMenuPrincipalAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Modulo>> GetByModuloPadreAsync(long? moduloPadreId, CancellationToken cancellationToken = default);
    Task<Modulo?> GetByCodigoAsync(string codigo, CancellationToken cancellationToken = default);
}

public interface IPermisoRepository : IRepository<Permiso>
{
    Task<Permiso?> GetByCodigoAsync(string codigo, CancellationToken cancellationToken = default);
    Task<IEnumerable<Permiso>> GetByModuloAsync(long moduloId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Permiso>> GetByRolAsync(long rolId, CancellationToken cancellationToken = default);
}

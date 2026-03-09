// =====================================================
// DOMAIN LAYER - Interfaces de Repositorios
// Archivo: Domain/Interfaces/Catalogos/
// =====================================================

using SolarisPlatform.Domain.Entities.Catalogos;

namespace SolarisPlatform.Domain.Interfaces.Catalogos;

// ─────────────────────────────────────────────────────
// País
// ─────────────────────────────────────────────────────
public interface IPaisRepository
{
    Task<IEnumerable<Pais>> ObtenerTodosAsync(bool soloActivos = false);
    Task<Pais?> ObtenerPorIdAsync(long id);
    Task<Pais?> ObtenerPorCodigoAsync(string codigo);
    Task<bool> ExisteCodigoAsync(string codigo, long? excludeId = null);
    Task<Pais> CrearAsync(Pais pais);
    Task<Pais> ActualizarAsync(Pais pais);
    Task EliminarAsync(long id);
}

// ─────────────────────────────────────────────────────
// Estado / Provincia
// ─────────────────────────────────────────────────────
public interface IEstadoProvinciaRepository
{
    Task<IEnumerable<EstadoProvincia>> ObtenerTodosAsync(bool soloActivos = false);
    Task<IEnumerable<EstadoProvincia>> ObtenerPorPaisAsync(long paisId, bool soloActivos = false);
    Task<EstadoProvincia?> ObtenerPorIdAsync(long id);
    Task<bool> ExisteCodigoEnPaisAsync(long paisId, string codigo, long? excludeId = null);
    Task<EstadoProvincia> CrearAsync(EstadoProvincia estado);
    Task<EstadoProvincia> ActualizarAsync(EstadoProvincia estado);
    Task EliminarAsync(long id);
}

// ─────────────────────────────────────────────────────
// Ciudad
// ─────────────────────────────────────────────────────
public interface ICiudadRepository
{
    Task<IEnumerable<Ciudad>> ObtenerTodosAsync(bool soloActivos = false);
    Task<IEnumerable<Ciudad>> ObtenerPorEstadoAsync(long estadoId, bool soloActivos = false);
    Task<Ciudad?> ObtenerPorIdAsync(long id);
    Task<Ciudad> CrearAsync(Ciudad ciudad);
    Task<Ciudad> ActualizarAsync(Ciudad ciudad);
    Task EliminarAsync(long id);
}

// ─────────────────────────────────────────────────────
// Moneda
// ─────────────────────────────────────────────────────
public interface IMonedaRepository
{
    Task<IEnumerable<Moneda>> ObtenerTodosAsync(bool soloActivos = false);
    Task<Moneda?> ObtenerPorIdAsync(long id);
    Task<Moneda?> ObtenerPorCodigoAsync(string codigo);
    Task<bool> ExisteCodigoAsync(string codigo, long? excludeId = null);
    Task<Moneda> CrearAsync(Moneda moneda);
    Task<Moneda> ActualizarAsync(Moneda moneda);
    Task EliminarAsync(long id);
}

// ─────────────────────────────────────────────────────
// Tipo Identificación
// ─────────────────────────────────────────────────────
public interface ITipoIdentificacionRepository
{
    Task<IEnumerable<TipoIdentificacion>> ObtenerTodosAsync(bool soloActivos = false);
    Task<IEnumerable<TipoIdentificacion>> ObtenerPorPaisAsync(long? paisId, bool soloActivos = false);
    Task<TipoIdentificacion?> ObtenerPorIdAsync(long id);
    Task<bool> ExisteCodigoAsync(string codigo, long? excludeId = null);
    Task<TipoIdentificacion> CrearAsync(TipoIdentificacion tipo);
    Task<TipoIdentificacion> ActualizarAsync(TipoIdentificacion tipo);
    Task EliminarAsync(long id);
}

// ─────────────────────────────────────────────────────
// Impuesto
// ─────────────────────────────────────────────────────
public interface IImpuestoRepository
{
    Task<IEnumerable<Impuesto>> ObtenerTodosAsync(long? empresaId = null, bool soloActivos = false);
    Task<Impuesto?> ObtenerPorIdAsync(long id);
    Task<bool> ExisteCodigoAsync(string codigo, long? empresaId, long? excludeId = null);
    Task<Impuesto> CrearAsync(Impuesto impuesto);
    Task<Impuesto> ActualizarAsync(Impuesto impuesto);
    Task EliminarAsync(long id);
}

// ─────────────────────────────────────────────────────
// Forma de Pago
// ─────────────────────────────────────────────────────
public interface IFormaPagoRepository
{
    Task<IEnumerable<FormaPago>> ObtenerTodosAsync(long? empresaId = null, bool soloActivos = false);
    Task<FormaPago?> ObtenerPorIdAsync(long id);
    Task<bool> ExisteCodigoAsync(string codigo, long? empresaId, long? excludeId = null);
    Task<FormaPago> CrearAsync(FormaPago formaPago);
    Task<FormaPago> ActualizarAsync(FormaPago formaPago);
    Task EliminarAsync(long id);
}

// ─────────────────────────────────────────────────────
// Banco
// ─────────────────────────────────────────────────────
public interface IBancoRepository
{
    Task<IEnumerable<Banco>> ObtenerTodosAsync(long? paisId = null, bool soloActivos = false);
    Task<Banco?> ObtenerPorIdAsync(long id);
    Task<bool> ExisteCodigoAsync(string codigo, long? excludeId = null);
    Task<Banco> CrearAsync(Banco banco);
    Task<Banco> ActualizarAsync(Banco banco);
    Task EliminarAsync(long id);
}

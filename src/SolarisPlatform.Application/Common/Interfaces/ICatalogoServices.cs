using SolarisPlatform.Application.DTOs.Catalogos;
using SolarisPlatform.Application.Common.Models;

namespace SolarisPlatform.Application.Common.Interfaces;

public interface IPaisService
{
    Task<ApiResponse<IEnumerable<PaisDto>>> ObtenerTodosAsync(bool soloActivos = false);
    Task<ApiResponse<PaisDto>> ObtenerPorIdAsync(long id);
    Task<ApiResponse<PaisDto>> CrearAsync(CrearPaisRequest request);
    Task<ApiResponse<PaisDto>> ActualizarAsync(long id, ActualizarPaisRequest request);
    Task<ApiResponse<bool>> CambiarEstadoAsync(long id, bool activo);
    Task<ApiResponse<bool>> EliminarAsync(long id);
}

public interface IEstadoProvinciaService
{
    Task<ApiResponse<IEnumerable<EstadoProvinciaDto>>> ObtenerTodosAsync(bool soloActivos = false);
    Task<ApiResponse<IEnumerable<EstadoProvinciaDto>>> ObtenerPorPaisAsync(long paisId, bool soloActivos = false);
    Task<ApiResponse<EstadoProvinciaDto>> ObtenerPorIdAsync(long id);
    Task<ApiResponse<EstadoProvinciaDto>> CrearAsync(CrearEstadoProvinciaRequest request);
    Task<ApiResponse<EstadoProvinciaDto>> ActualizarAsync(long id, ActualizarEstadoProvinciaRequest request);
    Task<ApiResponse<bool>> CambiarEstadoAsync(long id, bool activo);
    Task<ApiResponse<bool>> EliminarAsync(long id);
}

public interface ICiudadService
{
    Task<ApiResponse<IEnumerable<CiudadDto>>> ObtenerTodosAsync(bool soloActivos = false);
    Task<ApiResponse<IEnumerable<CiudadDto>>> ObtenerPorEstadoAsync(long estadoId, bool soloActivos = false);
    Task<ApiResponse<CiudadDto>> ObtenerPorIdAsync(long id);
    Task<ApiResponse<CiudadDto>> CrearAsync(CrearCiudadRequest request);
    Task<ApiResponse<CiudadDto>> ActualizarAsync(long id, ActualizarCiudadRequest request);
    Task<ApiResponse<bool>> CambiarEstadoAsync(long id, bool activo);
    Task<ApiResponse<bool>> EliminarAsync(long id);
}

public interface IMonedaService
{
    Task<ApiResponse<IEnumerable<MonedaDto>>> ObtenerTodosAsync(bool soloActivos = false);
    Task<ApiResponse<MonedaDto>> ObtenerPorIdAsync(long id);
    Task<ApiResponse<MonedaDto>> CrearAsync(CrearMonedaRequest request);
    Task<ApiResponse<MonedaDto>> ActualizarAsync(long id, ActualizarMonedaRequest request);
    Task<ApiResponse<bool>> CambiarEstadoAsync(long id, bool activo);
    Task<ApiResponse<bool>> EliminarAsync(long id);
}

public interface ITipoIdentificacionService
{
    Task<ApiResponse<IEnumerable<TipoIdentificacionDto>>> ObtenerTodosAsync(bool soloActivos = false);
    Task<ApiResponse<IEnumerable<TipoIdentificacionDto>>> ObtenerPorPaisAsync(long? paisId, bool soloActivos = false);
    Task<ApiResponse<TipoIdentificacionDto>> ObtenerPorIdAsync(long id);
    Task<ApiResponse<TipoIdentificacionDto>> CrearAsync(CrearTipoIdentificacionRequest request);
    Task<ApiResponse<TipoIdentificacionDto>> ActualizarAsync(long id, ActualizarTipoIdentificacionRequest request);
    Task<ApiResponse<bool>> CambiarEstadoAsync(long id, bool activo);
    Task<ApiResponse<bool>> EliminarAsync(long id);
}

public interface IImpuestoService
{
    Task<ApiResponse<IEnumerable<ImpuestoDto>>> ObtenerTodosAsync(long? empresaId = null, bool soloActivos = false);
    Task<ApiResponse<ImpuestoDto>> ObtenerPorIdAsync(long id);
    Task<ApiResponse<ImpuestoDto>> CrearAsync(CrearImpuestoRequest request);
    Task<ApiResponse<ImpuestoDto>> ActualizarAsync(long id, ActualizarImpuestoRequest request);
    Task<ApiResponse<bool>> CambiarEstadoAsync(long id, bool activo);
    Task<ApiResponse<bool>> EliminarAsync(long id);
}

public interface IFormaPagoService
{
    Task<ApiResponse<IEnumerable<FormaPagoDto>>> ObtenerTodosAsync(long? empresaId = null, bool soloActivos = false);
    Task<ApiResponse<FormaPagoDto>> ObtenerPorIdAsync(long id);
    Task<ApiResponse<FormaPagoDto>> CrearAsync(CrearFormaPagoRequest request);
    Task<ApiResponse<FormaPagoDto>> ActualizarAsync(long id, ActualizarFormaPagoRequest request);
    Task<ApiResponse<bool>> CambiarEstadoAsync(long id, bool activo);
    Task<ApiResponse<bool>> EliminarAsync(long id);
}

public interface IBancoService
{
    Task<ApiResponse<IEnumerable<BancoDto>>> ObtenerTodosAsync(long? paisId = null, bool soloActivos = false);
    Task<ApiResponse<BancoDto>> ObtenerPorIdAsync(long id);
    Task<ApiResponse<BancoDto>> CrearAsync(CrearBancoRequest request);
    Task<ApiResponse<BancoDto>> ActualizarAsync(long id, ActualizarBancoRequest request);
    Task<ApiResponse<bool>> CambiarEstadoAsync(long id, bool activo);
    Task<ApiResponse<bool>> EliminarAsync(long id);
}

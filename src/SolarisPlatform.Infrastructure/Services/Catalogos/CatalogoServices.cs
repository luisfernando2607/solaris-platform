using AutoMapper;
using SolarisPlatform.Application.Common.Interfaces;
using SolarisPlatform.Application.Common.Models;
using SolarisPlatform.Application.DTOs.Catalogos;
using SolarisPlatform.Domain.Entities.Catalogos;
using SolarisPlatform.Domain.Interfaces.Catalogos;

namespace SolarisPlatform.Infrastructure.Services.Catalogos;

public class PaisService(IPaisRepository repo, IMapper mapper) : IPaisService
{
    public async Task<ApiResponse<IEnumerable<PaisDto>>> ObtenerTodosAsync(bool soloActivos = false)
    { var items = await repo.ObtenerTodosAsync(soloActivos); return ApiResponse<IEnumerable<PaisDto>>.Ok(mapper.Map<IEnumerable<PaisDto>>(items)); }

    public async Task<ApiResponse<PaisDto>> ObtenerPorIdAsync(long id)
    { var item = await repo.ObtenerPorIdAsync(id); return item == null ? ApiResponse<PaisDto>.Fail("País no encontrado") : ApiResponse<PaisDto>.Ok(mapper.Map<PaisDto>(item)); }

    public async Task<ApiResponse<PaisDto>> CrearAsync(CrearPaisRequest request)
    { var created = await repo.CrearAsync(mapper.Map<Pais>(request)); return ApiResponse<PaisDto>.Ok(mapper.Map<PaisDto>(created)); }

    public async Task<ApiResponse<PaisDto>> ActualizarAsync(long id, ActualizarPaisRequest request)
    { var entity = await repo.ObtenerPorIdAsync(id); if (entity == null) return ApiResponse<PaisDto>.Fail("País no encontrado"); mapper.Map(request, entity); return ApiResponse<PaisDto>.Ok(mapper.Map<PaisDto>(await repo.ActualizarAsync(entity))); }

    public async Task<ApiResponse<PaisDto>> CambiarEstadoAsync(long id, bool activo)
    { var entity = await repo.ObtenerPorIdAsync(id); if (entity == null) return ApiResponse<PaisDto>.Fail("País no encontrado"); entity.Activo = activo; return ApiResponse<PaisDto>.Ok(mapper.Map<PaisDto>(await repo.ActualizarAsync(entity))); }

    public async Task<ApiResponse> EliminarAsync(long id)
    { if (await repo.ObtenerPorIdAsync(id) == null) return ApiResponse.Fail("País no encontrado"); await repo.EliminarAsync(id); return ApiResponse.Ok(); }
}

public class EstadoProvinciaService(IEstadoProvinciaRepository repo, IMapper mapper) : IEstadoProvinciaService
{
    public async Task<ApiResponse<IEnumerable<EstadoProvinciaDto>>> ObtenerTodosAsync(bool soloActivos = false)
    { return ApiResponse<IEnumerable<EstadoProvinciaDto>>.Ok(mapper.Map<IEnumerable<EstadoProvinciaDto>>(await repo.ObtenerTodosAsync(soloActivos))); }

    public async Task<ApiResponse<IEnumerable<EstadoProvinciaDto>>> ObtenerPorPaisAsync(long paisId, bool soloActivos = false)
    { return ApiResponse<IEnumerable<EstadoProvinciaDto>>.Ok(mapper.Map<IEnumerable<EstadoProvinciaDto>>(await repo.ObtenerPorPaisAsync(paisId, soloActivos))); }

    public async Task<ApiResponse<EstadoProvinciaDto>> ObtenerPorIdAsync(long id)
    { var item = await repo.ObtenerPorIdAsync(id); return item == null ? ApiResponse<EstadoProvinciaDto>.Fail("Estado/Provincia no encontrado") : ApiResponse<EstadoProvinciaDto>.Ok(mapper.Map<EstadoProvinciaDto>(item)); }

    public async Task<ApiResponse<EstadoProvinciaDto>> CrearAsync(CrearEstadoProvinciaRequest request)
    { return ApiResponse<EstadoProvinciaDto>.Ok(mapper.Map<EstadoProvinciaDto>(await repo.CrearAsync(mapper.Map<EstadoProvincia>(request)))); }

    public async Task<ApiResponse<EstadoProvinciaDto>> ActualizarAsync(long id, ActualizarEstadoProvinciaRequest request)
    { var entity = await repo.ObtenerPorIdAsync(id); if (entity == null) return ApiResponse<EstadoProvinciaDto>.Fail("Estado/Provincia no encontrado"); mapper.Map(request, entity); return ApiResponse<EstadoProvinciaDto>.Ok(mapper.Map<EstadoProvinciaDto>(await repo.ActualizarAsync(entity))); }

    public async Task<ApiResponse<EstadoProvinciaDto>> CambiarEstadoAsync(long id, bool activo)
    { var entity = await repo.ObtenerPorIdAsync(id); if (entity == null) return ApiResponse<EstadoProvinciaDto>.Fail("Estado/Provincia no encontrado"); entity.Activo = activo; return ApiResponse<EstadoProvinciaDto>.Ok(mapper.Map<EstadoProvinciaDto>(await repo.ActualizarAsync(entity))); }

    public async Task<ApiResponse> EliminarAsync(long id)
    { if (await repo.ObtenerPorIdAsync(id) == null) return ApiResponse.Fail("Estado/Provincia no encontrado"); await repo.EliminarAsync(id); return ApiResponse.Ok(); }
}

public class CiudadService(ICiudadRepository repo, IMapper mapper) : ICiudadService
{
    public async Task<ApiResponse<IEnumerable<CiudadDto>>> ObtenerTodosAsync(bool soloActivos = false)
    { return ApiResponse<IEnumerable<CiudadDto>>.Ok(mapper.Map<IEnumerable<CiudadDto>>(await repo.ObtenerTodosAsync(soloActivos))); }

    public async Task<ApiResponse<IEnumerable<CiudadDto>>> ObtenerPorEstadoAsync(long estadoId, bool soloActivos = false)
    { return ApiResponse<IEnumerable<CiudadDto>>.Ok(mapper.Map<IEnumerable<CiudadDto>>(await repo.ObtenerPorEstadoAsync(estadoId, soloActivos))); }

    public async Task<ApiResponse<CiudadDto>> ObtenerPorIdAsync(long id)
    { var item = await repo.ObtenerPorIdAsync(id); return item == null ? ApiResponse<CiudadDto>.Fail("Ciudad no encontrada") : ApiResponse<CiudadDto>.Ok(mapper.Map<CiudadDto>(item)); }

    public async Task<ApiResponse<CiudadDto>> CrearAsync(CrearCiudadRequest request)
    { return ApiResponse<CiudadDto>.Ok(mapper.Map<CiudadDto>(await repo.CrearAsync(mapper.Map<Ciudad>(request)))); }

    public async Task<ApiResponse<CiudadDto>> ActualizarAsync(long id, ActualizarCiudadRequest request)
    { var entity = await repo.ObtenerPorIdAsync(id); if (entity == null) return ApiResponse<CiudadDto>.Fail("Ciudad no encontrada"); mapper.Map(request, entity); return ApiResponse<CiudadDto>.Ok(mapper.Map<CiudadDto>(await repo.ActualizarAsync(entity))); }

    public async Task<ApiResponse<CiudadDto>> CambiarEstadoAsync(long id, bool activo)
    { var entity = await repo.ObtenerPorIdAsync(id); if (entity == null) return ApiResponse<CiudadDto>.Fail("Ciudad no encontrada"); entity.Activo = activo; return ApiResponse<CiudadDto>.Ok(mapper.Map<CiudadDto>(await repo.ActualizarAsync(entity))); }

    public async Task<ApiResponse> EliminarAsync(long id)
    { if (await repo.ObtenerPorIdAsync(id) == null) return ApiResponse.Fail("Ciudad no encontrada"); await repo.EliminarAsync(id); return ApiResponse.Ok(); }
}

public class MonedaService(IMonedaRepository repo, IMapper mapper) : IMonedaService
{
    public async Task<ApiResponse<IEnumerable<MonedaDto>>> ObtenerTodosAsync(bool soloActivos = false)
    { return ApiResponse<IEnumerable<MonedaDto>>.Ok(mapper.Map<IEnumerable<MonedaDto>>(await repo.ObtenerTodosAsync(soloActivos))); }

    public async Task<ApiResponse<MonedaDto>> ObtenerPorIdAsync(long id)
    { var item = await repo.ObtenerPorIdAsync(id); return item == null ? ApiResponse<MonedaDto>.Fail("Moneda no encontrada") : ApiResponse<MonedaDto>.Ok(mapper.Map<MonedaDto>(item)); }

    public async Task<ApiResponse<MonedaDto>> CrearAsync(CrearMonedaRequest request)
    { return ApiResponse<MonedaDto>.Ok(mapper.Map<MonedaDto>(await repo.CrearAsync(mapper.Map<Moneda>(request)))); }

    public async Task<ApiResponse<MonedaDto>> ActualizarAsync(long id, ActualizarMonedaRequest request)
    { var entity = await repo.ObtenerPorIdAsync(id); if (entity == null) return ApiResponse<MonedaDto>.Fail("Moneda no encontrada"); mapper.Map(request, entity); return ApiResponse<MonedaDto>.Ok(mapper.Map<MonedaDto>(await repo.ActualizarAsync(entity))); }

    public async Task<ApiResponse<MonedaDto>> CambiarEstadoAsync(long id, bool activo)
    { var entity = await repo.ObtenerPorIdAsync(id); if (entity == null) return ApiResponse<MonedaDto>.Fail("Moneda no encontrada"); entity.Activo = activo; return ApiResponse<MonedaDto>.Ok(mapper.Map<MonedaDto>(await repo.ActualizarAsync(entity))); }

    public async Task<ApiResponse> EliminarAsync(long id)
    { if (await repo.ObtenerPorIdAsync(id) == null) return ApiResponse.Fail("Moneda no encontrada"); await repo.EliminarAsync(id); return ApiResponse.Ok(); }
}

public class TipoIdentificacionService(ITipoIdentificacionRepository repo, IMapper mapper) : ITipoIdentificacionService
{
    public async Task<ApiResponse<IEnumerable<TipoIdentificacionDto>>> ObtenerTodosAsync(bool soloActivos = false)
    { return ApiResponse<IEnumerable<TipoIdentificacionDto>>.Ok(mapper.Map<IEnumerable<TipoIdentificacionDto>>(await repo.ObtenerTodosAsync(soloActivos))); }

    public async Task<ApiResponse<IEnumerable<TipoIdentificacionDto>>> ObtenerPorPaisAsync(long paisId, bool soloActivos = false)
    { return ApiResponse<IEnumerable<TipoIdentificacionDto>>.Ok(mapper.Map<IEnumerable<TipoIdentificacionDto>>(await repo.ObtenerPorPaisAsync(paisId, soloActivos))); }

    public async Task<ApiResponse<TipoIdentificacionDto>> ObtenerPorIdAsync(long id)
    { var item = await repo.ObtenerPorIdAsync(id); return item == null ? ApiResponse<TipoIdentificacionDto>.Fail("Tipo de identificación no encontrado") : ApiResponse<TipoIdentificacionDto>.Ok(mapper.Map<TipoIdentificacionDto>(item)); }

    public async Task<ApiResponse<TipoIdentificacionDto>> CrearAsync(CrearTipoIdentificacionRequest request)
    { return ApiResponse<TipoIdentificacionDto>.Ok(mapper.Map<TipoIdentificacionDto>(await repo.CrearAsync(mapper.Map<TipoIdentificacion>(request)))); }

    public async Task<ApiResponse<TipoIdentificacionDto>> ActualizarAsync(long id, ActualizarTipoIdentificacionRequest request)
    { var entity = await repo.ObtenerPorIdAsync(id); if (entity == null) return ApiResponse<TipoIdentificacionDto>.Fail("Tipo de identificación no encontrado"); mapper.Map(request, entity); return ApiResponse<TipoIdentificacionDto>.Ok(mapper.Map<TipoIdentificacionDto>(await repo.ActualizarAsync(entity))); }

    public async Task<ApiResponse<TipoIdentificacionDto>> CambiarEstadoAsync(long id, bool activo)
    { var entity = await repo.ObtenerPorIdAsync(id); if (entity == null) return ApiResponse<TipoIdentificacionDto>.Fail("Tipo de identificación no encontrado"); entity.Activo = activo; return ApiResponse<TipoIdentificacionDto>.Ok(mapper.Map<TipoIdentificacionDto>(await repo.ActualizarAsync(entity))); }

    public async Task<ApiResponse> EliminarAsync(long id)
    { if (await repo.ObtenerPorIdAsync(id) == null) return ApiResponse.Fail("Tipo de identificación no encontrado"); await repo.EliminarAsync(id); return ApiResponse.Ok(); }
}

public class ImpuestoService(IImpuestoRepository repo, IMapper mapper) : IImpuestoService
{
    public async Task<ApiResponse<IEnumerable<ImpuestoDto>>> ObtenerTodosAsync(long? empresaId = null, bool soloActivos = false)
    { return ApiResponse<IEnumerable<ImpuestoDto>>.Ok(mapper.Map<IEnumerable<ImpuestoDto>>(await repo.ObtenerTodosAsync(empresaId, soloActivos))); }

    public async Task<ApiResponse<ImpuestoDto>> ObtenerPorIdAsync(long id)
    { var item = await repo.ObtenerPorIdAsync(id); return item == null ? ApiResponse<ImpuestoDto>.Fail("Impuesto no encontrado") : ApiResponse<ImpuestoDto>.Ok(mapper.Map<ImpuestoDto>(item)); }

    public async Task<ApiResponse<ImpuestoDto>> CrearAsync(CrearImpuestoRequest request)
    { return ApiResponse<ImpuestoDto>.Ok(mapper.Map<ImpuestoDto>(await repo.CrearAsync(mapper.Map<Impuesto>(request)))); }

    public async Task<ApiResponse<ImpuestoDto>> ActualizarAsync(long id, ActualizarImpuestoRequest request)
    { var entity = await repo.ObtenerPorIdAsync(id); if (entity == null) return ApiResponse<ImpuestoDto>.Fail("Impuesto no encontrado"); mapper.Map(request, entity); return ApiResponse<ImpuestoDto>.Ok(mapper.Map<ImpuestoDto>(await repo.ActualizarAsync(entity))); }

    public async Task<ApiResponse<ImpuestoDto>> CambiarEstadoAsync(long id, bool activo)
    { var entity = await repo.ObtenerPorIdAsync(id); if (entity == null) return ApiResponse<ImpuestoDto>.Fail("Impuesto no encontrado"); entity.Activo = activo; return ApiResponse<ImpuestoDto>.Ok(mapper.Map<ImpuestoDto>(await repo.ActualizarAsync(entity))); }

    public async Task<ApiResponse> EliminarAsync(long id)
    { if (await repo.ObtenerPorIdAsync(id) == null) return ApiResponse.Fail("Impuesto no encontrado"); await repo.EliminarAsync(id); return ApiResponse.Ok(); }
}

public class FormaPagoService(IFormaPagoRepository repo, IMapper mapper) : IFormaPagoService
{
    public async Task<ApiResponse<IEnumerable<FormaPagoDto>>> ObtenerTodosAsync(long? empresaId = null, bool soloActivos = false)
    { return ApiResponse<IEnumerable<FormaPagoDto>>.Ok(mapper.Map<IEnumerable<FormaPagoDto>>(await repo.ObtenerTodosAsync(empresaId, soloActivos))); }

    public async Task<ApiResponse<FormaPagoDto>> ObtenerPorIdAsync(long id)
    { var item = await repo.ObtenerPorIdAsync(id); return item == null ? ApiResponse<FormaPagoDto>.Fail("Forma de pago no encontrada") : ApiResponse<FormaPagoDto>.Ok(mapper.Map<FormaPagoDto>(item)); }

    public async Task<ApiResponse<FormaPagoDto>> CrearAsync(CrearFormaPagoRequest request)
    { return ApiResponse<FormaPagoDto>.Ok(mapper.Map<FormaPagoDto>(await repo.CrearAsync(mapper.Map<FormaPago>(request)))); }

    public async Task<ApiResponse<FormaPagoDto>> ActualizarAsync(long id, ActualizarFormaPagoRequest request)
    { var entity = await repo.ObtenerPorIdAsync(id); if (entity == null) return ApiResponse<FormaPagoDto>.Fail("Forma de pago no encontrada"); mapper.Map(request, entity); return ApiResponse<FormaPagoDto>.Ok(mapper.Map<FormaPagoDto>(await repo.ActualizarAsync(entity))); }

    public async Task<ApiResponse<FormaPagoDto>> CambiarEstadoAsync(long id, bool activo)
    { var entity = await repo.ObtenerPorIdAsync(id); if (entity == null) return ApiResponse<FormaPagoDto>.Fail("Forma de pago no encontrada"); entity.Activo = activo; return ApiResponse<FormaPagoDto>.Ok(mapper.Map<FormaPagoDto>(await repo.ActualizarAsync(entity))); }

    public async Task<ApiResponse> EliminarAsync(long id)
    { if (await repo.ObtenerPorIdAsync(id) == null) return ApiResponse.Fail("Forma de pago no encontrada"); await repo.EliminarAsync(id); return ApiResponse.Ok(); }
}

public class BancoService(IBancoRepository repo, IMapper mapper) : IBancoService
{
    public async Task<ApiResponse<IEnumerable<BancoDto>>> ObtenerTodosAsync(long? paisId = null, bool soloActivos = false)
    { return ApiResponse<IEnumerable<BancoDto>>.Ok(mapper.Map<IEnumerable<BancoDto>>(await repo.ObtenerTodosAsync(paisId, soloActivos))); }

    public async Task<ApiResponse<BancoDto>> ObtenerPorIdAsync(long id)
    { var item = await repo.ObtenerPorIdAsync(id); return item == null ? ApiResponse<BancoDto>.Fail("Banco no encontrado") : ApiResponse<BancoDto>.Ok(mapper.Map<BancoDto>(item)); }

    public async Task<ApiResponse<BancoDto>> CrearAsync(CrearBancoRequest request)
    { return ApiResponse<BancoDto>.Ok(mapper.Map<BancoDto>(await repo.CrearAsync(mapper.Map<Banco>(request)))); }

    public async Task<ApiResponse<BancoDto>> ActualizarAsync(long id, ActualizarBancoRequest request)
    { var entity = await repo.ObtenerPorIdAsync(id); if (entity == null) return ApiResponse<BancoDto>.Fail("Banco no encontrado"); mapper.Map(request, entity); return ApiResponse<BancoDto>.Ok(mapper.Map<BancoDto>(await repo.ActualizarAsync(entity))); }

    public async Task<ApiResponse<BancoDto>> CambiarEstadoAsync(long id, bool activo)
    { var entity = await repo.ObtenerPorIdAsync(id); if (entity == null) return ApiResponse<BancoDto>.Fail("Banco no encontrado"); entity.Activo = activo; return ApiResponse<BancoDto>.Ok(mapper.Map<BancoDto>(await repo.ActualizarAsync(entity))); }

    public async Task<ApiResponse> EliminarAsync(long id)
    { if (await repo.ObtenerPorIdAsync(id) == null) return ApiResponse.Fail("Banco no encontrado"); await repo.EliminarAsync(id); return ApiResponse.Ok(); }
}
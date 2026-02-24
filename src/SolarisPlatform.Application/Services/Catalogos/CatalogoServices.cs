// =====================================================
// APPLICATION LAYER - Servicios de Catálogos
// Archivo: Application/Services/Catalogos/
// =====================================================

using AutoMapper;
using SolarisPlatform.Application.Common.Interfaces;
using SolarisPlatform.Application.Common.Models;
using SolarisPlatform.Application.DTOs.Catalogos;
using SolarisPlatform.Domain.Entities.Catalogos;
using SolarisPlatform.Domain.Interfaces.Catalogos;

namespace SolarisPlatform.Application.Services.Catalogos;

// ══════════════════════════════════════════════════════
// SERVICIO DE PAÍSES
// ══════════════════════════════════════════════════════
public class PaisService(IPaisRepository repo, IMapper mapper) : IPaisService
{
    public async Task<ApiResponse<IEnumerable<PaisDto>>> ObtenerTodosAsync(bool soloActivos = false)
    {
        var paises = await repo.ObtenerTodosAsync(soloActivos);
        return ApiResponse<IEnumerable<PaisDto>>.Ok(mapper.Map<IEnumerable<PaisDto>>(paises));
    }

    public async Task<ApiResponse<PaisDto>> ObtenerPorIdAsync(long id)
    {
        var pais = await repo.ObtenerPorIdAsync(id);
        if (pais is null)
            return ApiResponse<PaisDto>.Fail("País no encontrado.");

        return ApiResponse<PaisDto>.Ok(mapper.Map<PaisDto>(pais));
    }

    public async Task<ApiResponse<PaisDto>> CrearAsync(CrearPaisRequest request)
    {
        if (await repo.ExisteCodigoAsync(request.Codigo))
            return ApiResponse<PaisDto>.Fail($"Ya existe un país con el código '{request.Codigo}'.");

        var pais = mapper.Map<Pais>(request);
        var creado = await repo.CrearAsync(pais);
        return ApiResponse<PaisDto>.Ok(mapper.Map<PaisDto>(creado), "País creado exitosamente.");
    }

    public async Task<ApiResponse<PaisDto>> ActualizarAsync(long id, ActualizarPaisRequest request)
    {
        var pais = await repo.ObtenerPorIdAsync(id);
        if (pais is null)
            return ApiResponse<PaisDto>.Fail("País no encontrado.");

        if (await repo.ExisteCodigoAsync(request.Codigo, id))
            return ApiResponse<PaisDto>.Fail($"Ya existe otro país con el código '{request.Codigo}'.");

        mapper.Map(request, pais);
        var actualizado = await repo.ActualizarAsync(pais);
        return ApiResponse<PaisDto>.Ok(mapper.Map<PaisDto>(actualizado), "País actualizado exitosamente.");
    }

    public async Task<ApiResponse<bool>> CambiarEstadoAsync(long id, bool activo)
    {
        var pais = await repo.ObtenerPorIdAsync(id);
        if (pais is null)
            return ApiResponse<bool>.Fail("País no encontrado.");

        pais.Activo = activo;
        await repo.ActualizarAsync(pais);
        return ApiResponse<bool>.Ok(true, activo ? "País activado." : "País desactivado.");
    }

    public async Task<ApiResponse<bool>> EliminarAsync(long id)
    {
        var pais = await repo.ObtenerPorIdAsync(id);
        if (pais is null)
            return ApiResponse<bool>.Fail("País no encontrado.");

        await repo.EliminarAsync(id);
        return ApiResponse<bool>.Ok(true, "País eliminado exitosamente.");
    }
}

// ══════════════════════════════════════════════════════
// SERVICIO DE ESTADOS / PROVINCIAS
// ══════════════════════════════════════════════════════
public class EstadoProvinciaService(IEstadoProvinciaRepository repo, IPaisRepository paisRepo, IMapper mapper) : IEstadoProvinciaService
{
    public async Task<ApiResponse<IEnumerable<EstadoProvinciaDto>>> ObtenerTodosAsync(bool soloActivos = false)
    {
        var estados = await repo.ObtenerTodosAsync(soloActivos);
        return ApiResponse<IEnumerable<EstadoProvinciaDto>>.Ok(mapper.Map<IEnumerable<EstadoProvinciaDto>>(estados));
    }

    public async Task<ApiResponse<IEnumerable<EstadoProvinciaDto>>> ObtenerPorPaisAsync(long paisId, bool soloActivos = false)
    {
        var estados = await repo.ObtenerPorPaisAsync(paisId, soloActivos);
        return ApiResponse<IEnumerable<EstadoProvinciaDto>>.Ok(mapper.Map<IEnumerable<EstadoProvinciaDto>>(estados));
    }

    public async Task<ApiResponse<EstadoProvinciaDto>> ObtenerPorIdAsync(long id)
    {
        var estado = await repo.ObtenerPorIdAsync(id);
        if (estado is null)
            return ApiResponse<EstadoProvinciaDto>.Fail("Provincia/Estado no encontrado.");

        return ApiResponse<EstadoProvinciaDto>.Ok(mapper.Map<EstadoProvinciaDto>(estado));
    }

    public async Task<ApiResponse<EstadoProvinciaDto>> CrearAsync(CrearEstadoProvinciaRequest request)
    {
        var pais = await paisRepo.ObtenerPorIdAsync(request.PaisId);
        if (pais is null)
            return ApiResponse<EstadoProvinciaDto>.Fail("El país especificado no existe.");

        if (await repo.ExisteCodigoEnPaisAsync(request.PaisId, request.Codigo))
            return ApiResponse<EstadoProvinciaDto>.Fail($"Ya existe una provincia con el código '{request.Codigo}' en ese país.");

        var estado = mapper.Map<EstadoProvincia>(request);
        var creado = await repo.CrearAsync(estado);
        // Recargar con navegación
        var conNav = await repo.ObtenerPorIdAsync(creado.Id);
        return ApiResponse<EstadoProvinciaDto>.Ok(mapper.Map<EstadoProvinciaDto>(conNav), "Provincia creada exitosamente.");
    }

    public async Task<ApiResponse<EstadoProvinciaDto>> ActualizarAsync(long id, ActualizarEstadoProvinciaRequest request)
    {
        var estado = await repo.ObtenerPorIdAsync(id);
        if (estado is null)
            return ApiResponse<EstadoProvinciaDto>.Fail("Provincia/Estado no encontrado.");

        if (await repo.ExisteCodigoEnPaisAsync(request.PaisId, request.Codigo, id))
            return ApiResponse<EstadoProvinciaDto>.Fail($"Ya existe otra provincia con el código '{request.Codigo}' en ese país.");

        mapper.Map(request, estado);
        await repo.ActualizarAsync(estado);
        var conNav = await repo.ObtenerPorIdAsync(id);
        return ApiResponse<EstadoProvinciaDto>.Ok(mapper.Map<EstadoProvinciaDto>(conNav), "Provincia actualizada exitosamente.");
    }

    public async Task<ApiResponse<bool>> CambiarEstadoAsync(long id, bool activo)
    {
        var estado = await repo.ObtenerPorIdAsync(id);
        if (estado is null)
            return ApiResponse<bool>.Fail("Provincia/Estado no encontrado.");

        estado.Activo = activo;
        await repo.ActualizarAsync(estado);
        return ApiResponse<bool>.Ok(true, activo ? "Provincia activada." : "Provincia desactivada.");
    }

    public async Task<ApiResponse<bool>> EliminarAsync(long id)
    {
        var estado = await repo.ObtenerPorIdAsync(id);
        if (estado is null)
            return ApiResponse<bool>.Fail("Provincia/Estado no encontrado.");

        await repo.EliminarAsync(id);
        return ApiResponse<bool>.Ok(true, "Provincia eliminada exitosamente.");
    }
}

// ══════════════════════════════════════════════════════
// SERVICIO DE CIUDADES
// ══════════════════════════════════════════════════════
public class CiudadService(ICiudadRepository repo, IEstadoProvinciaRepository estadoRepo, IMapper mapper) : ICiudadService
{
    public async Task<ApiResponse<IEnumerable<CiudadDto>>> ObtenerTodosAsync(bool soloActivos = false)
    {
        var ciudades = await repo.ObtenerTodosAsync(soloActivos);
        return ApiResponse<IEnumerable<CiudadDto>>.Ok(mapper.Map<IEnumerable<CiudadDto>>(ciudades));
    }

    public async Task<ApiResponse<IEnumerable<CiudadDto>>> ObtenerPorEstadoAsync(long estadoId, bool soloActivos = false)
    {
        var ciudades = await repo.ObtenerPorEstadoAsync(estadoId, soloActivos);
        return ApiResponse<IEnumerable<CiudadDto>>.Ok(mapper.Map<IEnumerable<CiudadDto>>(ciudades));
    }

    public async Task<ApiResponse<CiudadDto>> ObtenerPorIdAsync(long id)
    {
        var ciudad = await repo.ObtenerPorIdAsync(id);
        if (ciudad is null)
            return ApiResponse<CiudadDto>.Fail("Ciudad no encontrada.");

        return ApiResponse<CiudadDto>.Ok(mapper.Map<CiudadDto>(ciudad));
    }

    public async Task<ApiResponse<CiudadDto>> CrearAsync(CrearCiudadRequest request)
    {
        var estado = await estadoRepo.ObtenerPorIdAsync(request.EstadoProvinciaId);
        if (estado is null)
            return ApiResponse<CiudadDto>.Fail("La provincia/estado especificada no existe.");

        var ciudad = mapper.Map<Ciudad>(request);
        var creada = await repo.CrearAsync(ciudad);
        var conNav = await repo.ObtenerPorIdAsync(creada.Id);
        return ApiResponse<CiudadDto>.Ok(mapper.Map<CiudadDto>(conNav), "Ciudad creada exitosamente.");
    }

    public async Task<ApiResponse<CiudadDto>> ActualizarAsync(long id, ActualizarCiudadRequest request)
    {
        var ciudad = await repo.ObtenerPorIdAsync(id);
        if (ciudad is null)
            return ApiResponse<CiudadDto>.Fail("Ciudad no encontrada.");

        mapper.Map(request, ciudad);
        await repo.ActualizarAsync(ciudad);
        var conNav = await repo.ObtenerPorIdAsync(id);
        return ApiResponse<CiudadDto>.Ok(mapper.Map<CiudadDto>(conNav), "Ciudad actualizada exitosamente.");
    }

    public async Task<ApiResponse<bool>> CambiarEstadoAsync(long id, bool activo)
    {
        var ciudad = await repo.ObtenerPorIdAsync(id);
        if (ciudad is null)
            return ApiResponse<bool>.Fail("Ciudad no encontrada.");

        ciudad.Activo = activo;
        await repo.ActualizarAsync(ciudad);
        return ApiResponse<bool>.Ok(true, activo ? "Ciudad activada." : "Ciudad desactivada.");
    }

    public async Task<ApiResponse<bool>> EliminarAsync(long id)
    {
        var ciudad = await repo.ObtenerPorIdAsync(id);
        if (ciudad is null)
            return ApiResponse<bool>.Fail("Ciudad no encontrada.");

        await repo.EliminarAsync(id);
        return ApiResponse<bool>.Ok(true, "Ciudad eliminada exitosamente.");
    }
}

// ══════════════════════════════════════════════════════
// SERVICIO DE MONEDAS
// ══════════════════════════════════════════════════════
public class MonedaService(IMonedaRepository repo, IMapper mapper) : IMonedaService
{
    public async Task<ApiResponse<IEnumerable<MonedaDto>>> ObtenerTodosAsync(bool soloActivos = false)
    {
        var monedas = await repo.ObtenerTodosAsync(soloActivos);
        return ApiResponse<IEnumerable<MonedaDto>>.Ok(mapper.Map<IEnumerable<MonedaDto>>(monedas));
    }

    public async Task<ApiResponse<MonedaDto>> ObtenerPorIdAsync(long id)
    {
        var moneda = await repo.ObtenerPorIdAsync(id);
        if (moneda is null)
            return ApiResponse<MonedaDto>.Fail("Moneda no encontrada.");

        return ApiResponse<MonedaDto>.Ok(mapper.Map<MonedaDto>(moneda));
    }

    public async Task<ApiResponse<MonedaDto>> CrearAsync(CrearMonedaRequest request)
    {
        if (await repo.ExisteCodigoAsync(request.Codigo))
            return ApiResponse<MonedaDto>.Fail($"Ya existe una moneda con el código '{request.Codigo}'.");

        var moneda = mapper.Map<Moneda>(request);
        var creada = await repo.CrearAsync(moneda);
        return ApiResponse<MonedaDto>.Ok(mapper.Map<MonedaDto>(creada), "Moneda creada exitosamente.");
    }

    public async Task<ApiResponse<MonedaDto>> ActualizarAsync(long id, ActualizarMonedaRequest request)
    {
        var moneda = await repo.ObtenerPorIdAsync(id);
        if (moneda is null)
            return ApiResponse<MonedaDto>.Fail("Moneda no encontrada.");

        if (await repo.ExisteCodigoAsync(request.Codigo, id))
            return ApiResponse<MonedaDto>.Fail($"Ya existe otra moneda con el código '{request.Codigo}'.");

        mapper.Map(request, moneda);
        var actualizada = await repo.ActualizarAsync(moneda);
        return ApiResponse<MonedaDto>.Ok(mapper.Map<MonedaDto>(actualizada), "Moneda actualizada exitosamente.");
    }

    public async Task<ApiResponse<bool>> CambiarEstadoAsync(long id, bool activo)
    {
        var moneda = await repo.ObtenerPorIdAsync(id);
        if (moneda is null)
            return ApiResponse<bool>.Fail("Moneda no encontrada.");

        moneda.Activo = activo;
        await repo.ActualizarAsync(moneda);
        return ApiResponse<bool>.Ok(true, activo ? "Moneda activada." : "Moneda desactivada.");
    }

    public async Task<ApiResponse<bool>> EliminarAsync(long id)
    {
        var moneda = await repo.ObtenerPorIdAsync(id);
        if (moneda is null)
            return ApiResponse<bool>.Fail("Moneda no encontrada.");

        await repo.EliminarAsync(id);
        return ApiResponse<bool>.Ok(true, "Moneda eliminada exitosamente.");
    }
}

// ══════════════════════════════════════════════════════
// SERVICIO DE TIPOS DE IDENTIFICACIÓN
// ══════════════════════════════════════════════════════
public class TipoIdentificacionService(ITipoIdentificacionRepository repo, IMapper mapper) : ITipoIdentificacionService
{
    public async Task<ApiResponse<IEnumerable<TipoIdentificacionDto>>> ObtenerTodosAsync(bool soloActivos = false)
    {
        var tipos = await repo.ObtenerTodosAsync(soloActivos);
        return ApiResponse<IEnumerable<TipoIdentificacionDto>>.Ok(mapper.Map<IEnumerable<TipoIdentificacionDto>>(tipos));
    }

    public async Task<ApiResponse<IEnumerable<TipoIdentificacionDto>>> ObtenerPorPaisAsync(long? paisId, bool soloActivos = false)
    {
        var tipos = await repo.ObtenerPorPaisAsync(paisId, soloActivos);
        return ApiResponse<IEnumerable<TipoIdentificacionDto>>.Ok(mapper.Map<IEnumerable<TipoIdentificacionDto>>(tipos));
    }

    public async Task<ApiResponse<TipoIdentificacionDto>> ObtenerPorIdAsync(long id)
    {
        var tipo = await repo.ObtenerPorIdAsync(id);
        if (tipo is null)
            return ApiResponse<TipoIdentificacionDto>.Fail("Tipo de identificación no encontrado.");

        return ApiResponse<TipoIdentificacionDto>.Ok(mapper.Map<TipoIdentificacionDto>(tipo));
    }

    public async Task<ApiResponse<TipoIdentificacionDto>> CrearAsync(CrearTipoIdentificacionRequest request)
    {
        if (await repo.ExisteCodigoAsync(request.Codigo))
            return ApiResponse<TipoIdentificacionDto>.Fail($"Ya existe un tipo de identificación con el código '{request.Codigo}'.");

        var tipo = mapper.Map<TipoIdentificacion>(request);
        var creado = await repo.CrearAsync(tipo);
        var conNav = await repo.ObtenerPorIdAsync(creado.Id);
        return ApiResponse<TipoIdentificacionDto>.Ok(mapper.Map<TipoIdentificacionDto>(conNav), "Tipo de identificación creado exitosamente.");
    }

    public async Task<ApiResponse<TipoIdentificacionDto>> ActualizarAsync(long id, ActualizarTipoIdentificacionRequest request)
    {
        var tipo = await repo.ObtenerPorIdAsync(id);
        if (tipo is null)
            return ApiResponse<TipoIdentificacionDto>.Fail("Tipo de identificación no encontrado.");

        if (await repo.ExisteCodigoAsync(request.Codigo, id))
            return ApiResponse<TipoIdentificacionDto>.Fail($"Ya existe otro tipo de identificación con el código '{request.Codigo}'.");

        mapper.Map(request, tipo);
        await repo.ActualizarAsync(tipo);
        var conNav = await repo.ObtenerPorIdAsync(id);
        return ApiResponse<TipoIdentificacionDto>.Ok(mapper.Map<TipoIdentificacionDto>(conNav), "Tipo de identificación actualizado exitosamente.");
    }

    public async Task<ApiResponse<bool>> CambiarEstadoAsync(long id, bool activo)
    {
        var tipo = await repo.ObtenerPorIdAsync(id);
        if (tipo is null)
            return ApiResponse<bool>.Fail("Tipo de identificación no encontrado.");

        tipo.Activo = activo;
        await repo.ActualizarAsync(tipo);
        return ApiResponse<bool>.Ok(true, activo ? "Tipo activado." : "Tipo desactivado.");
    }

    public async Task<ApiResponse<bool>> EliminarAsync(long id)
    {
        var tipo = await repo.ObtenerPorIdAsync(id);
        if (tipo is null)
            return ApiResponse<bool>.Fail("Tipo de identificación no encontrado.");

        await repo.EliminarAsync(id);
        return ApiResponse<bool>.Ok(true, "Tipo de identificación eliminado exitosamente.");
    }
}

// ══════════════════════════════════════════════════════
// SERVICIO DE IMPUESTOS
// ══════════════════════════════════════════════════════
public class ImpuestoService(IImpuestoRepository repo, IMapper mapper) : IImpuestoService
{
    public async Task<ApiResponse<IEnumerable<ImpuestoDto>>> ObtenerTodosAsync(long? empresaId = null, bool soloActivos = false)
    {
        var impuestos = await repo.ObtenerTodosAsync(empresaId, soloActivos);
        return ApiResponse<IEnumerable<ImpuestoDto>>.Ok(mapper.Map<IEnumerable<ImpuestoDto>>(impuestos));
    }

    public async Task<ApiResponse<ImpuestoDto>> ObtenerPorIdAsync(long id)
    {
        var impuesto = await repo.ObtenerPorIdAsync(id);
        if (impuesto is null)
            return ApiResponse<ImpuestoDto>.Fail("Impuesto no encontrado.");

        return ApiResponse<ImpuestoDto>.Ok(mapper.Map<ImpuestoDto>(impuesto));
    }

    public async Task<ApiResponse<ImpuestoDto>> CrearAsync(CrearImpuestoRequest request)
    {
        if (await repo.ExisteCodigoAsync(request.Codigo, request.EmpresaId))
            return ApiResponse<ImpuestoDto>.Fail($"Ya existe un impuesto con el código '{request.Codigo}'.");

        var impuesto = mapper.Map<Impuesto>(request);
        var creado = await repo.CrearAsync(impuesto);
        return ApiResponse<ImpuestoDto>.Ok(mapper.Map<ImpuestoDto>(creado), "Impuesto creado exitosamente.");
    }

    public async Task<ApiResponse<ImpuestoDto>> ActualizarAsync(long id, ActualizarImpuestoRequest request)
    {
        var impuesto = await repo.ObtenerPorIdAsync(id);
        if (impuesto is null)
            return ApiResponse<ImpuestoDto>.Fail("Impuesto no encontrado.");

        if (await repo.ExisteCodigoAsync(request.Codigo, impuesto.EmpresaId, id))
            return ApiResponse<ImpuestoDto>.Fail($"Ya existe otro impuesto con el código '{request.Codigo}'.");

        mapper.Map(request, impuesto);
        var actualizado = await repo.ActualizarAsync(impuesto);
        return ApiResponse<ImpuestoDto>.Ok(mapper.Map<ImpuestoDto>(actualizado), "Impuesto actualizado exitosamente.");
    }

    public async Task<ApiResponse<bool>> CambiarEstadoAsync(long id, bool activo)
    {
        var impuesto = await repo.ObtenerPorIdAsync(id);
        if (impuesto is null)
            return ApiResponse<bool>.Fail("Impuesto no encontrado.");

        impuesto.Activo = activo;
        await repo.ActualizarAsync(impuesto);
        return ApiResponse<bool>.Ok(true, activo ? "Impuesto activado." : "Impuesto desactivado.");
    }

    public async Task<ApiResponse<bool>> EliminarAsync(long id)
    {
        var impuesto = await repo.ObtenerPorIdAsync(id);
        if (impuesto is null)
            return ApiResponse<bool>.Fail("Impuesto no encontrado.");

        await repo.EliminarAsync(id);
        return ApiResponse<bool>.Ok(true, "Impuesto eliminado exitosamente.");
    }
}

// ══════════════════════════════════════════════════════
// SERVICIO DE FORMAS DE PAGO
// ══════════════════════════════════════════════════════
public class FormaPagoService(IFormaPagoRepository repo, IMapper mapper) : IFormaPagoService
{
    public async Task<ApiResponse<IEnumerable<FormaPagoDto>>> ObtenerTodosAsync(long? empresaId = null, bool soloActivos = false)
    {
        var formas = await repo.ObtenerTodosAsync(empresaId, soloActivos);
        return ApiResponse<IEnumerable<FormaPagoDto>>.Ok(mapper.Map<IEnumerable<FormaPagoDto>>(formas));
    }

    public async Task<ApiResponse<FormaPagoDto>> ObtenerPorIdAsync(long id)
    {
        var forma = await repo.ObtenerPorIdAsync(id);
        if (forma is null)
            return ApiResponse<FormaPagoDto>.Fail("Forma de pago no encontrada.");

        return ApiResponse<FormaPagoDto>.Ok(mapper.Map<FormaPagoDto>(forma));
    }

    public async Task<ApiResponse<FormaPagoDto>> CrearAsync(CrearFormaPagoRequest request)
    {
        if (await repo.ExisteCodigoAsync(request.Codigo, request.EmpresaId))
            return ApiResponse<FormaPagoDto>.Fail($"Ya existe una forma de pago con el código '{request.Codigo}'.");

        var forma = mapper.Map<FormaPago>(request);
        var creada = await repo.CrearAsync(forma);
        return ApiResponse<FormaPagoDto>.Ok(mapper.Map<FormaPagoDto>(creada), "Forma de pago creada exitosamente.");
    }

    public async Task<ApiResponse<FormaPagoDto>> ActualizarAsync(long id, ActualizarFormaPagoRequest request)
    {
        var forma = await repo.ObtenerPorIdAsync(id);
        if (forma is null)
            return ApiResponse<FormaPagoDto>.Fail("Forma de pago no encontrada.");

        if (await repo.ExisteCodigoAsync(request.Codigo, forma.EmpresaId, id))
            return ApiResponse<FormaPagoDto>.Fail($"Ya existe otra forma de pago con el código '{request.Codigo}'.");

        mapper.Map(request, forma);
        var actualizada = await repo.ActualizarAsync(forma);
        return ApiResponse<FormaPagoDto>.Ok(mapper.Map<FormaPagoDto>(actualizada), "Forma de pago actualizada exitosamente.");
    }

    public async Task<ApiResponse<bool>> CambiarEstadoAsync(long id, bool activo)
    {
        var forma = await repo.ObtenerPorIdAsync(id);
        if (forma is null)
            return ApiResponse<bool>.Fail("Forma de pago no encontrada.");

        forma.Activo = activo;
        await repo.ActualizarAsync(forma);
        return ApiResponse<bool>.Ok(true, activo ? "Forma de pago activada." : "Forma de pago desactivada.");
    }

    public async Task<ApiResponse<bool>> EliminarAsync(long id)
    {
        var forma = await repo.ObtenerPorIdAsync(id);
        if (forma is null)
            return ApiResponse<bool>.Fail("Forma de pago no encontrada.");

        await repo.EliminarAsync(id);
        return ApiResponse<bool>.Ok(true, "Forma de pago eliminada exitosamente.");
    }
}

// ══════════════════════════════════════════════════════
// SERVICIO DE BANCOS
// ══════════════════════════════════════════════════════
public class BancoService(IBancoRepository repo, IMapper mapper) : IBancoService
{
    public async Task<ApiResponse<IEnumerable<BancoDto>>> ObtenerTodosAsync(long? paisId = null, bool soloActivos = false)
    {
        var bancos = await repo.ObtenerTodosAsync(paisId, soloActivos);
        return ApiResponse<IEnumerable<BancoDto>>.Ok(mapper.Map<IEnumerable<BancoDto>>(bancos));
    }

    public async Task<ApiResponse<BancoDto>> ObtenerPorIdAsync(long id)
    {
        var banco = await repo.ObtenerPorIdAsync(id);
        if (banco is null)
            return ApiResponse<BancoDto>.Fail("Banco no encontrado.");

        return ApiResponse<BancoDto>.Ok(mapper.Map<BancoDto>(banco));
    }

    public async Task<ApiResponse<BancoDto>> CrearAsync(CrearBancoRequest request)
    {
        if (await repo.ExisteCodigoAsync(request.Codigo))
            return ApiResponse<BancoDto>.Fail($"Ya existe un banco con el código '{request.Codigo}'.");

        var banco = mapper.Map<Banco>(request);
        var creado = await repo.CrearAsync(banco);
        var conNav = await repo.ObtenerPorIdAsync(creado.Id);
        return ApiResponse<BancoDto>.Ok(mapper.Map<BancoDto>(conNav), "Banco creado exitosamente.");
    }

    public async Task<ApiResponse<BancoDto>> ActualizarAsync(long id, ActualizarBancoRequest request)
    {
        var banco = await repo.ObtenerPorIdAsync(id);
        if (banco is null)
            return ApiResponse<BancoDto>.Fail("Banco no encontrado.");

        if (await repo.ExisteCodigoAsync(request.Codigo, id))
            return ApiResponse<BancoDto>.Fail($"Ya existe otro banco con el código '{request.Codigo}'.");

        mapper.Map(request, banco);
        await repo.ActualizarAsync(banco);
        var conNav = await repo.ObtenerPorIdAsync(id);
        return ApiResponse<BancoDto>.Ok(mapper.Map<BancoDto>(conNav), "Banco actualizado exitosamente.");
    }

    public async Task<ApiResponse<bool>> CambiarEstadoAsync(long id, bool activo)
    {
        var banco = await repo.ObtenerPorIdAsync(id);
        if (banco is null)
            return ApiResponse<bool>.Fail("Banco no encontrado.");

        banco.Activo = activo;
        await repo.ActualizarAsync(banco);
        return ApiResponse<bool>.Ok(true, activo ? "Banco activado." : "Banco desactivado.");
    }

    public async Task<ApiResponse<bool>> EliminarAsync(long id)
    {
        var banco = await repo.ObtenerPorIdAsync(id);
        if (banco is null)
            return ApiResponse<bool>.Fail("Banco no encontrado.");

        await repo.EliminarAsync(id);
        return ApiResponse<bool>.Ok(true, "Banco eliminado exitosamente.");
    }
}
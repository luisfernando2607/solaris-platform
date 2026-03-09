// =====================================================
// FIX: CatalogoMappingProfile.cs
// PROBLEMA: CiudadDto es un record posicional → AutoMapper no puede instanciarlo
//           sin ConstructUsing(). El error era:
//           "CiudadDto needs to have a constructor with 0 args or only optional args"
// SOLUCIÓN: Usar ConstructUsing() para todos los records posicionales.
//           Los records con campos calculados de navegación (EstadoProvinciaNombre,
//           PaisNombre) SIEMPRE necesitan ConstructUsing().
// =====================================================

using AutoMapper;
using SolarisPlatform.Application.DTOs.Catalogos;
using SolarisPlatform.Domain.Entities.Catalogos;

namespace SolarisPlatform.Application.Mappings;

public class CatalogoMappingProfile : Profile
{
    public CatalogoMappingProfile()
    {
        // ─── País ────────────────────────────────────────────────
        // PaisDto no tiene campos calculados → mapeo directo funciona
        CreateMap<Pais, PaisDto>();
        CreateMap<CrearPaisRequest, Pais>();
        CreateMap<ActualizarPaisRequest, Pais>();

        // ─── Estado / Provincia ──────────────────────────────────
        // EstadoProvinciaDto tiene PaisNombre (navegación) → ConstructUsing
        CreateMap<EstadoProvincia, EstadoProvinciaDto>()
            .ConstructUsing((s, _) => new EstadoProvinciaDto(
                s.Id,
                s.PaisId,
                s.Pais != null ? s.Pais.Nombre : string.Empty,
                s.Codigo,
                s.Nombre,
                s.Activo,
                s.Orden))
            .ForAllMembers(o => o.Ignore());

        CreateMap<CrearEstadoProvinciaRequest, EstadoProvincia>();
        CreateMap<ActualizarEstadoProvinciaRequest, EstadoProvincia>();

        // ─── Ciudad ──────────────────────────────────────────────
        // FIX PRINCIPAL: CiudadDto tiene EstadoProvinciaNombre y PaisNombre
        // (ambos son navegaciones de 2do nivel).
        // ConstructUsing es OBLIGATORIO para records posicionales con navegaciones.
        CreateMap<Ciudad, CiudadDto>()
            .ConstructUsing((s, _) => new CiudadDto(
                s.Id,
                s.EstadoProvinciaId,
                s.EstadoProvincia != null ? s.EstadoProvincia.Nombre : string.Empty,
                s.EstadoProvincia?.Pais != null ? s.EstadoProvincia.Pais.Nombre : string.Empty,
                s.Codigo,
                s.Nombre,
                s.Activo,
                s.Orden))
            .ForAllMembers(o => o.Ignore());

        CreateMap<CrearCiudadRequest, Ciudad>();
        CreateMap<ActualizarCiudadRequest, Ciudad>();

        // ─── Moneda ──────────────────────────────────────────────
        CreateMap<Moneda, MonedaDto>();
        CreateMap<CrearMonedaRequest, Moneda>();
        CreateMap<ActualizarMonedaRequest, Moneda>();

        // ─── Tipo Identificación ─────────────────────────────────
        // TipoIdentificacionDto tiene PaisNombre nullable → ConstructUsing
        CreateMap<TipoIdentificacion, TipoIdentificacionDto>()
            .ConstructUsing((s, _) => new TipoIdentificacionDto(
                s.Id,
                s.PaisId,
                s.Pais != null ? s.Pais.Nombre : null,
                s.Codigo,
                s.Nombre,
                s.Longitud,
                s.Patron,
                s.AplicaPersona,
                s.AplicaEmpresa,
                s.Activo,
                s.Orden))
            .ForAllMembers(o => o.Ignore());

        CreateMap<CrearTipoIdentificacionRequest, TipoIdentificacion>();
        CreateMap<ActualizarTipoIdentificacionRequest, TipoIdentificacion>();

        // ─── Impuesto ────────────────────────────────────────────
        CreateMap<Impuesto, ImpuestoDto>();
        CreateMap<CrearImpuestoRequest, Impuesto>();
        CreateMap<ActualizarImpuestoRequest, Impuesto>();

        // ─── Forma de Pago ───────────────────────────────────────
        CreateMap<FormaPago, FormaPagoDto>();
        CreateMap<CrearFormaPagoRequest, FormaPago>();
        CreateMap<ActualizarFormaPagoRequest, FormaPago>();

        // ─── Banco ───────────────────────────────────────────────
        // BancoDto tiene PaisNombre nullable → ConstructUsing
        CreateMap<Banco, BancoDto>()
            .ConstructUsing((s, _) => new BancoDto(
                s.Id,
                s.PaisId,
                s.Pais != null ? s.Pais.Nombre : null,
                s.Codigo,
                s.Nombre,
                s.NombreCorto,
                s.Activo,
                s.Orden))
            .ForAllMembers(o => o.Ignore());

        CreateMap<CrearBancoRequest, Banco>();
        CreateMap<ActualizarBancoRequest, Banco>();
    }
}
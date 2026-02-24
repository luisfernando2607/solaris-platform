using AutoMapper;
using SolarisPlatform.Application.DTOs.Catalogos;
using SolarisPlatform.Domain.Entities.Catalogos;

namespace SolarisPlatform.Application.Mappings;

public class CatalogosMappingProfile : Profile
{
    public CatalogosMappingProfile()
    {
        CreateMap<Pais, PaisDto>();
        CreateMap<CrearPaisRequest, Pais>();
        CreateMap<ActualizarPaisRequest, Pais>();

        CreateMap<EstadoProvincia, EstadoProvinciaDto>()
            .ForMember(d => d.PaisNombre, o => o.MapFrom(s => s.Pais != null ? s.Pais.Nombre : string.Empty));
        CreateMap<CrearEstadoProvinciaRequest, EstadoProvincia>();
        CreateMap<ActualizarEstadoProvinciaRequest, EstadoProvincia>();

        CreateMap<Ciudad, CiudadDto>()
            .ForMember(d => d.EstadoProvinciaNombre, o => o.MapFrom(s => s.EstadoProvincia != null ? s.EstadoProvincia.Nombre : string.Empty))
            .ForMember(d => d.PaisNombre, o => o.MapFrom(s => s.EstadoProvincia != null && s.EstadoProvincia.Pais != null ? s.EstadoProvincia.Pais.Nombre : string.Empty));
        CreateMap<CrearCiudadRequest, Ciudad>();
        CreateMap<ActualizarCiudadRequest, Ciudad>();

        CreateMap<Moneda, MonedaDto>();
        CreateMap<CrearMonedaRequest, Moneda>();
        CreateMap<ActualizarMonedaRequest, Moneda>();

        CreateMap<TipoIdentificacion, TipoIdentificacionDto>()
            .ForMember(d => d.PaisNombre, o => o.MapFrom(s => s.Pais != null ? s.Pais.Nombre : null));
        CreateMap<CrearTipoIdentificacionRequest, TipoIdentificacion>();
        CreateMap<ActualizarTipoIdentificacionRequest, TipoIdentificacion>();

        CreateMap<Impuesto, ImpuestoDto>();
        CreateMap<CrearImpuestoRequest, Impuesto>();
        CreateMap<ActualizarImpuestoRequest, Impuesto>();

        CreateMap<FormaPago, FormaPagoDto>();
        CreateMap<CrearFormaPagoRequest, FormaPago>();
        CreateMap<ActualizarFormaPagoRequest, FormaPago>();

        CreateMap<Banco, BancoDto>()
            .ForMember(d => d.PaisNombre, o => o.MapFrom(s => s.Pais != null ? s.Pais.Nombre : null));
        CreateMap<CrearBancoRequest, Banco>();
        CreateMap<ActualizarBancoRequest, Banco>();
    }
}

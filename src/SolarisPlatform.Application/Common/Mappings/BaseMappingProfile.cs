using AutoMapper;
using SolarisPlatform.Domain.Entities.Seguridad;
using SolarisPlatform.Domain.Entities.Empresas;
using SolarisPlatform.Domain.Entities.Catalogos;
using SolarisPlatform.Domain.Enums;
using SolarisPlatform.Application.DTOs.Auth;
using SolarisPlatform.Application.DTOs.Usuarios;
using SolarisPlatform.Application.DTOs.Roles;
using SolarisPlatform.Application.DTOs.Catalogos;

namespace SolarisPlatform.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // ==========================================
        // USUARIO
        // ==========================================

        CreateMap<Usuario, UsuarioDto>()
            .ForMember(dest => dest.EmpresaNombre, opt => opt.MapFrom(src => src.Empresa.RazonSocial))
            .ForMember(dest => dest.SucursalNombre, opt => opt.MapFrom(src => src.Sucursal != null ? src.Sucursal.Nombre : null))
            .ForMember(dest => dest.EstadoNombre, opt => opt.MapFrom(src => GetEstadoNombre(src.Estado)))
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.UsuarioRoles
                .Where(ur => ur.Activo)
                .Select(ur => new RolSimpleDto
                {
                    Id = ur.Rol.Id,
                    Codigo = ur.Rol.Codigo,
                    Nombre = ur.Rol.Nombre,
                    Color = ur.Rol.Color,
                    EsPrincipal = ur.EsPrincipal
                }).ToList()));

        CreateMap<Usuario, UsuarioListDto>()
            .ForMember(dest => dest.EstadoNombre, opt => opt.MapFrom(src => GetEstadoNombre(src.Estado)))
            .ForMember(dest => dest.RolPrincipal, opt => opt.MapFrom(src =>
                src.UsuarioRoles.Where(ur => ur.Activo && ur.EsPrincipal)
                    .Select(ur => ur.Rol.Nombre).FirstOrDefault() ??
                src.UsuarioRoles.Where(ur => ur.Activo)
                    .Select(ur => ur.Rol.Nombre).FirstOrDefault()));

        CreateMap<Usuario, UsuarioAutenticadoDto>()
            .ForMember(dest => dest.EmpresaNombre, opt => opt.MapFrom(src => src.Empresa.RazonSocial))
            .ForMember(dest => dest.SucursalNombre, opt => opt.MapFrom(src => src.Sucursal != null ? src.Sucursal.Nombre : null))
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.UsuarioRoles
                .Where(ur => ur.Activo)
                .Select(ur => ur.Rol.Codigo).ToList()))
            .ForMember(dest => dest.Permisos, opt => opt.Ignore());

        CreateMap<CrearUsuarioRequest, Usuario>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.Estado, opt => opt.MapFrom(_ => EstadoUsuario.Activo))
            .ForMember(dest => dest.Activo, opt => opt.MapFrom(_ => true))
            .ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(_ => DateTime.UtcNow));

        // ==========================================
        // ROL
        // ==========================================

        CreateMap<Rol, RolDto>()
            .ForMember(dest => dest.CantidadUsuarios, opt => opt.MapFrom(src => src.UsuarioRoles.Count(ur => ur.Activo)))
            .ForMember(dest => dest.Permisos, opt => opt.MapFrom(src => src.RolPermisos
                .Select(rp => new PermisoDto
                {
                    Id = rp.Permiso.Id,
                    ModuloId = rp.Permiso.ModuloId,
                    ModuloNombre = rp.Permiso.Modulo.Nombre,
                    Codigo = rp.Permiso.Codigo,
                    Nombre = rp.Permiso.Nombre,
                    Descripcion = rp.Permiso.Descripcion,
                    TipoPermiso = rp.Permiso.TipoPermiso,
                    Activo = rp.Permiso.Activo
                }).ToList()));

        CreateMap<Rol, RolListDto>()
            .ForMember(dest => dest.CantidadUsuarios, opt => opt.MapFrom(src => src.UsuarioRoles.Count(ur => ur.Activo)));

        CreateMap<CrearRolRequest, Rol>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.EsSistema, opt => opt.MapFrom(_ => false))
            .ForMember(dest => dest.Activo, opt => opt.MapFrom(_ => true))
            .ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(_ => DateTime.UtcNow));

        // ==========================================
        // PERMISO
        // ==========================================

        CreateMap<Permiso, PermisoDto>()
            .ForMember(dest => dest.ModuloNombre, opt => opt.MapFrom(src => src.Modulo.Nombre));

        // ==========================================
        // MÓDULO
        // ==========================================

        CreateMap<Modulo, ModuloConPermisosDto>()
            .ForMember(dest => dest.Permisos, opt => opt.MapFrom(src => src.Permisos
                .Where(p => p.Activo)
                .Select(p => new PermisoDto
                {
                    Id = p.Id,
                    ModuloId = p.ModuloId,
                    ModuloNombre = src.Nombre,
                    Codigo = p.Codigo,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    TipoPermiso = p.TipoPermiso,
                    Activo = p.Activo
                }).ToList()))
            .ForMember(dest => dest.SubModulos, opt => opt.MapFrom(src => src.SubModulos
                .Where(m => m.Activo)
                .OrderBy(m => m.Orden)
                .ToList()));

        // ==========================================
        // CATÁLOGOS
        // ==========================================

        // País
        CreateMap<Pais, PaisDto>()
            .ConstructUsing(s => new PaisDto(s.Id, s.Codigo, s.CodigoIso2, s.Nombre, s.NombreIngles, s.CodigoTelefonico, s.Bandera, s.Activo, s.Orden));
        CreateMap<CrearPaisRequest, Pais>().ForMember(d => d.Id, o => o.Ignore());
        CreateMap<ActualizarPaisRequest, Pais>().ForMember(d => d.Id, o => o.Ignore());

        // Estado/Provincia
        CreateMap<EstadoProvincia, EstadoProvinciaDto>()
            .ConstructUsing(s => new EstadoProvinciaDto(s.Id, s.PaisId, s.Pais != null ? s.Pais.Nombre : "", s.Codigo, s.Nombre, s.Activo, s.Orden));
        CreateMap<CrearEstadoProvinciaRequest, EstadoProvincia>().ForMember(d => d.Id, o => o.Ignore());
        CreateMap<ActualizarEstadoProvinciaRequest, EstadoProvincia>().ForMember(d => d.Id, o => o.Ignore());

        // Ciudad
        CreateMap<Ciudad, CiudadDto>()
            .ConstructUsing(s => new CiudadDto(
                s.Id,
                s.EstadoProvinciaId,
                s.EstadoProvincia != null ? s.EstadoProvincia.Nombre : "",
                s.EstadoProvincia != null && s.EstadoProvincia.Pais != null ? s.EstadoProvincia.Pais.Nombre : "",
                s.Codigo,
                s.Nombre,
                s.Activo,
                s.Orden));
        CreateMap<CrearCiudadRequest, Ciudad>().ForMember(d => d.Id, o => o.Ignore());
        CreateMap<ActualizarCiudadRequest, Ciudad>().ForMember(d => d.Id, o => o.Ignore());

        // Moneda
        CreateMap<Moneda, MonedaDto>()
            .ConstructUsing(s => new MonedaDto(s.Id, s.Codigo, s.Nombre, s.Simbolo, (byte)s.DecimalesPermitidos, s.Activo, s.Orden));
        CreateMap<CrearMonedaRequest, Moneda>().ForMember(d => d.Id, o => o.Ignore());
        CreateMap<ActualizarMonedaRequest, Moneda>().ForMember(d => d.Id, o => o.Ignore());

        // Tipo Identificación
        CreateMap<TipoIdentificacion, TipoIdentificacionDto>()
            .ConstructUsing(s => new TipoIdentificacionDto(
                s.Id, s.PaisId,
                s.Pais != null ? s.Pais.Nombre : null,
                s.Codigo, s.Nombre, s.Longitud, s.Patron,
                s.AplicaPersona, s.AplicaEmpresa, s.Activo, s.Orden));
        CreateMap<CrearTipoIdentificacionRequest, TipoIdentificacion>().ForMember(d => d.Id, o => o.Ignore());
        CreateMap<ActualizarTipoIdentificacionRequest, TipoIdentificacion>().ForMember(d => d.Id, o => o.Ignore());

        // Impuesto
        CreateMap<Impuesto, ImpuestoDto>()
            .ConstructUsing(s => new ImpuestoDto(s.Id, s.EmpresaId, s.Codigo, s.Nombre, s.Porcentaje, s.TipoImpuesto, s.EsRetencion, s.Activo, s.Orden));
        CreateMap<CrearImpuestoRequest, Impuesto>().ForMember(d => d.Id, o => o.Ignore());
        CreateMap<ActualizarImpuestoRequest, Impuesto>().ForMember(d => d.Id, o => o.Ignore());

        // Forma de Pago
        CreateMap<FormaPago, FormaPagoDto>()
            .ConstructUsing(s => new FormaPagoDto(s.Id, s.EmpresaId, s.Codigo, s.Nombre, s.Tipo, s.DiasCredito, s.RequiereBanco, s.RequiereReferencia, s.Activo, s.Orden));
        CreateMap<CrearFormaPagoRequest, FormaPago>().ForMember(d => d.Id, o => o.Ignore());
        CreateMap<ActualizarFormaPagoRequest, FormaPago>().ForMember(d => d.Id, o => o.Ignore());

        // Banco
        CreateMap<Banco, BancoDto>()
            .ConstructUsing(s => new BancoDto(s.Id, s.PaisId, s.Pais != null ? s.Pais.Nombre : null, s.Codigo, s.Nombre, s.NombreCorto, s.Activo, s.Orden));
        CreateMap<CrearBancoRequest, Banco>().ForMember(d => d.Id, o => o.Ignore());
        CreateMap<ActualizarBancoRequest, Banco>().ForMember(d => d.Id, o => o.Ignore());
    }

    private static string GetEstadoNombre(EstadoUsuario estado)
    {
        return estado switch
        {
            EstadoUsuario.Inactivo => "Inactivo",
            EstadoUsuario.Activo => "Activo",
            EstadoUsuario.Bloqueado => "Bloqueado",
            EstadoUsuario.Pendiente => "Pendiente",
            _ => "Desconocido"
        };
    }
}
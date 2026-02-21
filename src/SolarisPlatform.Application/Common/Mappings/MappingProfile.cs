using AutoMapper;
using SolarisPlatform.Domain.Entities.Seguridad;
using SolarisPlatform.Domain.Entities.Empresas;
using SolarisPlatform.Domain.Enums;
using SolarisPlatform.Application.DTOs.Auth;
using SolarisPlatform.Application.DTOs.Usuarios;
using SolarisPlatform.Application.DTOs.Roles;

namespace SolarisPlatform.Application.Common.Mappings;

/// <summary>
/// Perfiles de mapeo de AutoMapper
/// </summary>
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
            .ForMember(dest => dest.Permisos, opt => opt.Ignore()); // Se llena manualmente

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

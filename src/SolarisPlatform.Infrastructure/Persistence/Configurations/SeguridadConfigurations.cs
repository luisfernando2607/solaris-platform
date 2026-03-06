using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolarisPlatform.Domain.Entities.Seguridad;

namespace SolarisPlatform.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Usuario
/// </summary>
public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("usuario", "seg");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        builder.Property(e => e.SucursalId).HasColumnName("sucursal_id");
        builder.Property(e => e.Codigo).HasColumnName("codigo").HasMaxLength(20);
        builder.Property(e => e.Email).HasColumnName("email").HasMaxLength(256).IsRequired();
        builder.Property(e => e.NombreUsuario).HasColumnName("nombre_usuario").HasMaxLength(100);
        builder.Property(e => e.Nombres).HasColumnName("nombres").HasMaxLength(100).IsRequired();
        builder.Property(e => e.Apellidos).HasColumnName("apellidos").HasMaxLength(100).IsRequired();
        builder.Property(e => e.TipoIdentificacionId).HasColumnName("tipo_identificacion_id");
        builder.Property(e => e.NumeroIdentificacion).HasColumnName("numero_identificacion").HasMaxLength(20);
        builder.Property(e => e.Telefono).HasColumnName("telefono").HasMaxLength(20);
        builder.Property(e => e.Celular).HasColumnName("celular").HasMaxLength(20);
        builder.Property(e => e.FechaNacimiento).HasColumnName("fecha_nacimiento");
        builder.Property(e => e.Genero).HasColumnName("genero").HasMaxLength(1);
        builder.Property(e => e.FotoUrl).HasColumnName("foto_url").HasMaxLength(500);

        builder.Property(e => e.PasswordHash).HasColumnName("password_hash").HasMaxLength(256).IsRequired();
        builder.Property(e => e.PasswordSalt).HasColumnName("password_salt").HasMaxLength(256);
        builder.Property(e => e.RequiereCambioPassword).HasColumnName("requiere_cambio_password");
        builder.Property(e => e.FechaUltimoCambioPassword).HasColumnName("fecha_ultimo_cambio_password");
        builder.Property(e => e.IntentosLoginFallidos).HasColumnName("intentos_login_fallidos");
        builder.Property(e => e.FechaBloqueo).HasColumnName("fecha_bloqueo");

        builder.Property(e => e.EmailVerificado).HasColumnName("email_verificado");
        builder.Property(e => e.TokenVerificacionEmail).HasColumnName("token_verificacion_email").HasMaxLength(256);
        builder.Property(e => e.FechaExpiracionToken).HasColumnName("fecha_expiracion_token");

        builder.Property(e => e.TokenRecuperacion).HasColumnName("token_recuperacion").HasMaxLength(256);
        builder.Property(e => e.FechaExpiracionRecuperacion).HasColumnName("fecha_expiracion_recuperacion");

        builder.Property(e => e.TwoFactorHabilitado).HasColumnName("two_factor_habilitado");
        builder.Property(e => e.TwoFactorSecretKey).HasColumnName("two_factor_secret_key").HasMaxLength(256);

        builder.Property(e => e.Estado).HasColumnName("estado").HasConversion<int>();
        builder.Property(e => e.UltimoAcceso).HasColumnName("ultimo_acceso");

        builder.Property(e => e.IdiomaPreferido).HasColumnName("idioma_preferido").HasMaxLength(10);
        builder.Property(e => e.ZonaHoraria).HasColumnName("zona_horaria").HasMaxLength(50);
        builder.Property(e => e.TemaPreferido).HasColumnName("tema_preferido").HasMaxLength(20);

        builder.Property(e => e.Activo).HasColumnName("activo");
        builder.Property(e => e.Eliminado).HasColumnName("eliminado");
        builder.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");
        builder.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");
        builder.Property(e => e.FechaModificacion).HasColumnName("fecha_modificacion");
        builder.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");
        builder.Property(e => e.FechaEliminacion).HasColumnName("fecha_eliminacion");
        builder.Property(e => e.UsuarioEliminacion).HasColumnName("usuario_eliminacion");

        // Relaciones
        builder.HasOne(e => e.Empresa)
            .WithMany()
            .HasForeignKey(e => e.EmpresaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Sucursal)
            .WithMany()
            .HasForeignKey(e => e.SucursalId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices
        builder.HasIndex(e => e.Email).IsUnique();
        builder.HasIndex(e => e.EmpresaId);
        builder.HasIndex(e => e.Estado);

        // Ignorar propiedades calculadas
        builder.Ignore(e => e.NombreCompleto);

        // Filtro global para soft delete
        builder.HasQueryFilter(e => !e.Eliminado);
    }
}

/// <summary>
/// Configuración de Rol
/// </summary>
public class RolConfiguration : IEntityTypeConfiguration<Rol>
{
    public void Configure(EntityTypeBuilder<Rol> builder)
    {
        builder.ToTable("rol", "seg");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.EmpresaId).HasColumnName("empresa_id");
        builder.Property(e => e.Codigo).HasColumnName("codigo").HasMaxLength(50).IsRequired();
        builder.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
        builder.Property(e => e.Descripcion).HasColumnName("descripcion").HasMaxLength(500);
        builder.Property(e => e.EsSistema).HasColumnName("es_sistema");
        builder.Property(e => e.Nivel).HasColumnName("nivel");
        builder.Property(e => e.Color).HasColumnName("color").HasMaxLength(20);
        builder.Property(e => e.Icono).HasColumnName("icono").HasMaxLength(50);

        builder.Property(e => e.Activo).HasColumnName("activo");
        builder.Property(e => e.Eliminado).HasColumnName("eliminado");
        builder.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");
        builder.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");
        builder.Property(e => e.FechaModificacion).HasColumnName("fecha_modificacion");
        builder.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");
        builder.Property(e => e.FechaEliminacion).HasColumnName("fecha_eliminacion");
        builder.Property(e => e.UsuarioEliminacion).HasColumnName("usuario_eliminacion");

        builder.HasOne(e => e.Empresa)
            .WithMany()
            .HasForeignKey(e => e.EmpresaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => new { e.Codigo, e.EmpresaId }).IsUnique();

        builder.HasQueryFilter(e => !e.Eliminado);
    }
}

/// <summary>
/// Configuración de Módulo
/// </summary>
public class ModuloConfiguration : IEntityTypeConfiguration<Modulo>
{
    public void Configure(EntityTypeBuilder<Modulo> builder)
    {
        builder.ToTable("modulo", "seg");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.ModuloPadreId).HasColumnName("modulo_padre_id");
        builder.Property(e => e.Codigo).HasColumnName("codigo").HasMaxLength(50).IsRequired();
        builder.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
        builder.Property(e => e.Descripcion).HasColumnName("descripcion").HasMaxLength(500);
        builder.Property(e => e.Icono).HasColumnName("icono").HasMaxLength(50);
        builder.Property(e => e.Ruta).HasColumnName("ruta").HasMaxLength(200);
        builder.Property(e => e.Orden).HasColumnName("orden");
        builder.Property(e => e.EsMenu).HasColumnName("es_menu");
        builder.Property(e => e.Activo).HasColumnName("activo");
        builder.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");

        builder.HasOne(e => e.ModuloPadre)
            .WithMany(e => e.SubModulos)
            .HasForeignKey(e => e.ModuloPadreId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.Codigo).IsUnique();
    }
}

/// <summary>
/// Configuración de Permiso
/// </summary>
public class PermisoConfiguration : IEntityTypeConfiguration<Permiso>
{
    public void Configure(EntityTypeBuilder<Permiso> builder)
    {
        builder.ToTable("permiso", "seg");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.ModuloId).HasColumnName("modulo_id").IsRequired();
        builder.Property(e => e.Codigo).HasColumnName("codigo").HasMaxLength(100).IsRequired();
        builder.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
        builder.Property(e => e.Descripcion).HasColumnName("descripcion").HasMaxLength(500);
        builder.Property(e => e.TipoPermiso).HasColumnName("tipo_permiso").HasMaxLength(20);
        builder.Property(e => e.Activo).HasColumnName("activo");
        builder.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");

        builder.HasOne(e => e.Modulo)
            .WithMany(m => m.Permisos)
            .HasForeignKey(e => e.ModuloId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.Codigo).IsUnique();
    }
}

/// <summary>
/// Configuración de RolPermiso
/// </summary>
public class RolPermisoConfiguration : IEntityTypeConfiguration<RolPermiso>
{
    public void Configure(EntityTypeBuilder<RolPermiso> builder)
    {
        builder.ToTable("rol_permiso", "seg");
            builder.HasQueryFilter(rp => !rp.Rol!.Eliminado);

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.RolId).HasColumnName("rol_id").IsRequired();
        builder.Property(e => e.PermisoId).HasColumnName("permiso_id").IsRequired();
        builder.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");
        builder.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");

        builder.HasOne(e => e.Rol)
            .WithMany(r => r.RolPermisos)
            .HasForeignKey(e => e.RolId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Permiso)
            .WithMany(p => p.RolPermisos)
            .HasForeignKey(e => e.PermisoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.RolId, e.PermisoId }).IsUnique();
    }
}

/// <summary>
/// Configuración de UsuarioRol
/// </summary>
public class UsuarioRolConfiguration : IEntityTypeConfiguration<UsuarioRol>
{
    public void Configure(EntityTypeBuilder<UsuarioRol> builder)
    {
        builder.ToTable("usuario_rol", "seg");
            builder.HasQueryFilter(ur => !ur.Rol!.Eliminado);

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.UsuarioId).HasColumnName("usuario_id").IsRequired();
        builder.Property(e => e.RolId).HasColumnName("rol_id").IsRequired();
        builder.Property(e => e.EsPrincipal).HasColumnName("es_principal");
        builder.Property(e => e.FechaInicio).HasColumnName("fecha_inicio");
        builder.Property(e => e.FechaFin).HasColumnName("fecha_fin");
        builder.Property(e => e.Activo).HasColumnName("activo");
        builder.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");
        builder.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");

        builder.HasOne(e => e.Usuario)
            .WithMany(u => u.UsuarioRoles)
            .HasForeignKey(e => e.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Rol)
            .WithMany(r => r.UsuarioRoles)
            .HasForeignKey(e => e.RolId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.UsuarioId, e.RolId }).IsUnique();
    }
}

/// <summary>
/// Configuración de UsuarioSucursal
/// </summary>
public class UsuarioSucursalConfiguration : IEntityTypeConfiguration<UsuarioSucursal>
{
    public void Configure(EntityTypeBuilder<UsuarioSucursal> builder)
    {
        builder.ToTable("usuario_sucursal", "seg");
            builder.HasQueryFilter(us => us.Sucursal!.Activo);

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.UsuarioId).HasColumnName("usuario_id").IsRequired();
        builder.Property(e => e.SucursalId).HasColumnName("sucursal_id").IsRequired();
        builder.Property(e => e.EsPrincipal).HasColumnName("es_principal");
        builder.Property(e => e.Activo).HasColumnName("activo");
        builder.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");
        builder.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");

        builder.HasOne(e => e.Usuario)
            .WithMany(u => u.UsuarioSucursales)
            .HasForeignKey(e => e.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Sucursal)
            .WithMany()
            .HasForeignKey(e => e.SucursalId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.UsuarioId, e.SucursalId }).IsUnique();
    }
}

/// <summary>
/// Configuración de SesionUsuario
/// </summary>
public class SesionUsuarioConfiguration : IEntityTypeConfiguration<SesionUsuario>
{
    public void Configure(EntityTypeBuilder<SesionUsuario> builder)
    {
        builder.ToTable("sesion_usuario", "seg");
            builder.HasQueryFilter(s => s.Usuario!.Activo && !s.Usuario!.Eliminado);

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.UsuarioId).HasColumnName("usuario_id").IsRequired();
        builder.Property(e => e.Token).HasColumnName("token").HasMaxLength(512).IsRequired();
        builder.Property(e => e.RefreshToken).HasColumnName("refresh_token").HasMaxLength(256);
        builder.Property(e => e.FechaInicio).HasColumnName("fecha_inicio");
        builder.Property(e => e.FechaExpiracion).HasColumnName("fecha_expiracion");
        builder.Property(e => e.FechaUltimaActividad).HasColumnName("fecha_ultima_actividad");
        builder.Property(e => e.DireccionIp).HasColumnName("direccion_ip").HasMaxLength(50);
        builder.Property(e => e.UserAgent).HasColumnName("user_agent").HasMaxLength(500);
        builder.Property(e => e.Dispositivo).HasColumnName("dispositivo").HasMaxLength(100);
        builder.Property(e => e.Navegador).HasColumnName("navegador").HasMaxLength(100);
        builder.Property(e => e.SistemaOperativo).HasColumnName("sistema_operativo").HasMaxLength(100);
        builder.Property(e => e.PaisAcceso).HasColumnName("pais_acceso").HasMaxLength(100);
        builder.Property(e => e.CiudadAcceso).HasColumnName("ciudad_acceso").HasMaxLength(100);
        builder.Property(e => e.Activo).HasColumnName("activo");
        builder.Property(e => e.CerradaPorUsuario).HasColumnName("cerrada_por_usuario");
        builder.Property(e => e.FechaCierre).HasColumnName("fecha_cierre");

        builder.HasOne(e => e.Usuario)
            .WithMany(u => u.Sesiones)
            .HasForeignKey(e => e.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.Token);
        builder.HasIndex(e => e.RefreshToken);
        builder.HasIndex(e => e.UsuarioId);
    }
}

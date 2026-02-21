using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SolarisPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "cat");

            migrationBuilder.EnsureSchema(
                name: "emp");

            migrationBuilder.EnsureSchema(
                name: "seg");

            migrationBuilder.CreateTable(
                name: "modulo",
                schema: "seg",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    modulo_padre_id = table.Column<long>(type: "bigint", nullable: true),
                    codigo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    icono = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ruta = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    orden = table.Column<int>(type: "integer", nullable: false),
                    es_menu = table.Column<bool>(type: "boolean", nullable: false),
                    activo = table.Column<bool>(type: "boolean", nullable: false),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_modulo", x => x.id);
                    table.ForeignKey(
                        name: "FK_modulo_modulo_modulo_padre_id",
                        column: x => x.modulo_padre_id,
                        principalSchema: "seg",
                        principalTable: "modulo",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "moneda",
                schema: "cat",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    codigo = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    simbolo = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    decimales_permitidos = table.Column<short>(type: "smallint", nullable: false),
                    activo = table.Column<bool>(type: "boolean", nullable: false),
                    orden = table.Column<int>(type: "integer", nullable: false),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_moneda", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "pais",
                schema: "cat",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    codigo = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    codigo_iso2 = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    nombre_ingles = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    codigo_telefonico = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    bandera = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    activo = table.Column<bool>(type: "boolean", nullable: false),
                    orden = table.Column<int>(type: "integer", nullable: false),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pais", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "permiso",
                schema: "seg",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    modulo_id = table.Column<long>(type: "bigint", nullable: false),
                    codigo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    tipo_permiso = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    activo = table.Column<bool>(type: "boolean", nullable: false),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permiso", x => x.id);
                    table.ForeignKey(
                        name: "FK_permiso_modulo_modulo_id",
                        column: x => x.modulo_id,
                        principalSchema: "seg",
                        principalTable: "modulo",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "banco",
                schema: "cat",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    pais_id = table.Column<long>(type: "bigint", nullable: true),
                    codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    nombre_corto = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    activo = table.Column<bool>(type: "boolean", nullable: false),
                    orden = table.Column<int>(type: "integer", nullable: false),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_banco", x => x.id);
                    table.ForeignKey(
                        name: "FK_banco_pais_pais_id",
                        column: x => x.pais_id,
                        principalSchema: "cat",
                        principalTable: "pais",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "empresa",
                schema: "emp",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    razon_social = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    nombre_comercial = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    tipo_identificacion = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    numero_identificacion = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    telefono = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    direccion_fiscal = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    logo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    pagina_web = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    moneda_principal_id = table.Column<long>(type: "bigint", nullable: true),
                    pais_id = table.Column<long>(type: "bigint", nullable: true),
                    zona_horaria = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    plan_contratado = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    fecha_inicio_contrato = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    fecha_fin_contrato = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    max_usuarios = table.Column<int>(type: "integer", nullable: false),
                    estado = table.Column<int>(type: "integer", nullable: false),
                    activo = table.Column<bool>(type: "boolean", nullable: false),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    usuario_creacion = table.Column<long>(type: "bigint", nullable: true),
                    fecha_modificacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    usuario_modificacion = table.Column<long>(type: "bigint", nullable: true),
                    eliminado = table.Column<bool>(type: "boolean", nullable: false),
                    fecha_eliminacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    usuario_eliminacion = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_empresa", x => x.id);
                    table.ForeignKey(
                        name: "FK_empresa_moneda_moneda_principal_id",
                        column: x => x.moneda_principal_id,
                        principalSchema: "cat",
                        principalTable: "moneda",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_empresa_pais_pais_id",
                        column: x => x.pais_id,
                        principalSchema: "cat",
                        principalTable: "pais",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "estado_provincia",
                schema: "cat",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    pais_id = table.Column<long>(type: "bigint", nullable: false),
                    codigo = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    activo = table.Column<bool>(type: "boolean", nullable: false),
                    orden = table.Column<int>(type: "integer", nullable: false),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_estado_provincia", x => x.id);
                    table.ForeignKey(
                        name: "FK_estado_provincia_pais_pais_id",
                        column: x => x.pais_id,
                        principalSchema: "cat",
                        principalTable: "pais",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tipo_identificacion",
                schema: "cat",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    pais_id = table.Column<long>(type: "bigint", nullable: true),
                    codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    longitud = table.Column<int>(type: "integer", nullable: true),
                    patron = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    aplica_persona = table.Column<bool>(type: "boolean", nullable: false),
                    aplica_empresa = table.Column<bool>(type: "boolean", nullable: false),
                    activo = table.Column<bool>(type: "boolean", nullable: false),
                    orden = table.Column<int>(type: "integer", nullable: false),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tipo_identificacion", x => x.id);
                    table.ForeignKey(
                        name: "FK_tipo_identificacion_pais_pais_id",
                        column: x => x.pais_id,
                        principalSchema: "cat",
                        principalTable: "pais",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "forma_pago",
                schema: "cat",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    empresa_id = table.Column<long>(type: "bigint", nullable: true),
                    codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    tipo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    dias_credito = table.Column<int>(type: "integer", nullable: false),
                    requiere_banco = table.Column<bool>(type: "boolean", nullable: false),
                    requiere_referencia = table.Column<bool>(type: "boolean", nullable: false),
                    activo = table.Column<bool>(type: "boolean", nullable: false),
                    orden = table.Column<int>(type: "integer", nullable: false),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_forma_pago", x => x.id);
                    table.ForeignKey(
                        name: "FK_forma_pago_empresa_empresa_id",
                        column: x => x.empresa_id,
                        principalSchema: "emp",
                        principalTable: "empresa",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "impuesto",
                schema: "cat",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    empresa_id = table.Column<long>(type: "bigint", nullable: true),
                    codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    porcentaje = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    tipo_impuesto = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    es_retencion = table.Column<bool>(type: "boolean", nullable: false),
                    activo = table.Column<bool>(type: "boolean", nullable: false),
                    orden = table.Column<int>(type: "integer", nullable: false),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_impuesto", x => x.id);
                    table.ForeignKey(
                        name: "FK_impuesto_empresa_empresa_id",
                        column: x => x.empresa_id,
                        principalSchema: "emp",
                        principalTable: "empresa",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "rol",
                schema: "seg",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    empresa_id = table.Column<long>(type: "bigint", nullable: true),
                    codigo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    es_sistema = table.Column<bool>(type: "boolean", nullable: false),
                    nivel = table.Column<int>(type: "integer", nullable: false),
                    color = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    icono = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    activo = table.Column<bool>(type: "boolean", nullable: false),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    usuario_creacion = table.Column<long>(type: "bigint", nullable: true),
                    fecha_modificacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    usuario_modificacion = table.Column<long>(type: "bigint", nullable: true),
                    eliminado = table.Column<bool>(type: "boolean", nullable: false),
                    fecha_eliminacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    usuario_eliminacion = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rol", x => x.id);
                    table.ForeignKey(
                        name: "FK_rol_empresa_empresa_id",
                        column: x => x.empresa_id,
                        principalSchema: "emp",
                        principalTable: "empresa",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ciudad",
                schema: "cat",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    estado_provincia_id = table.Column<long>(type: "bigint", nullable: false),
                    codigo = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    activo = table.Column<bool>(type: "boolean", nullable: false),
                    orden = table.Column<int>(type: "integer", nullable: false),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ciudad", x => x.id);
                    table.ForeignKey(
                        name: "FK_ciudad_estado_provincia_estado_provincia_id",
                        column: x => x.estado_provincia_id,
                        principalSchema: "cat",
                        principalTable: "estado_provincia",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "rol_permiso",
                schema: "seg",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    rol_id = table.Column<long>(type: "bigint", nullable: false),
                    permiso_id = table.Column<long>(type: "bigint", nullable: false),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    usuario_creacion = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rol_permiso", x => x.id);
                    table.ForeignKey(
                        name: "FK_rol_permiso_permiso_permiso_id",
                        column: x => x.permiso_id,
                        principalSchema: "seg",
                        principalTable: "permiso",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_rol_permiso_rol_rol_id",
                        column: x => x.rol_id,
                        principalSchema: "seg",
                        principalTable: "rol",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sucursal",
                schema: "emp",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    empresa_id = table.Column<long>(type: "bigint", nullable: false),
                    codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    direccion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    telefono = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    es_principal = table.Column<bool>(type: "boolean", nullable: false),
                    pais_id = table.Column<long>(type: "bigint", nullable: true),
                    estado_provincia_id = table.Column<long>(type: "bigint", nullable: true),
                    ciudad_id = table.Column<long>(type: "bigint", nullable: true),
                    codigo_postal = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    latitud = table.Column<decimal>(type: "numeric(10,7)", precision: 10, scale: 7, nullable: true),
                    longitud = table.Column<decimal>(type: "numeric(10,7)", precision: 10, scale: 7, nullable: true),
                    activo = table.Column<bool>(type: "boolean", nullable: false),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    usuario_creacion = table.Column<long>(type: "bigint", nullable: true),
                    fecha_modificacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    usuario_modificacion = table.Column<long>(type: "bigint", nullable: true),
                    eliminado = table.Column<bool>(type: "boolean", nullable: false),
                    fecha_eliminacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    usuario_eliminacion = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sucursal", x => x.id);
                    table.ForeignKey(
                        name: "FK_sucursal_ciudad_ciudad_id",
                        column: x => x.ciudad_id,
                        principalSchema: "cat",
                        principalTable: "ciudad",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_sucursal_empresa_empresa_id",
                        column: x => x.empresa_id,
                        principalSchema: "emp",
                        principalTable: "empresa",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sucursal_estado_provincia_estado_provincia_id",
                        column: x => x.estado_provincia_id,
                        principalSchema: "cat",
                        principalTable: "estado_provincia",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_sucursal_pais_pais_id",
                        column: x => x.pais_id,
                        principalSchema: "cat",
                        principalTable: "pais",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "usuario",
                schema: "seg",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    empresa_id = table.Column<long>(type: "bigint", nullable: false),
                    sucursal_id = table.Column<long>(type: "bigint", nullable: true),
                    codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    nombre_usuario = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    nombres = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    apellidos = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    tipo_identificacion_id = table.Column<long>(type: "bigint", nullable: true),
                    numero_identificacion = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    telefono = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    celular = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    fecha_nacimiento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    genero = table.Column<char>(type: "character(1)", maxLength: 1, nullable: true),
                    foto_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    password_hash = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    password_salt = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    requiere_cambio_password = table.Column<bool>(type: "boolean", nullable: false),
                    fecha_ultimo_cambio_password = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    intentos_login_fallidos = table.Column<int>(type: "integer", nullable: false),
                    fecha_bloqueo = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    email_verificado = table.Column<bool>(type: "boolean", nullable: false),
                    token_verificacion_email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    fecha_expiracion_token = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    token_recuperacion = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    fecha_expiracion_recuperacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    two_factor_habilitado = table.Column<bool>(type: "boolean", nullable: false),
                    two_factor_secret_key = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    estado = table.Column<int>(type: "integer", nullable: false),
                    ultimo_acceso = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    idioma_preferido = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    zona_horaria = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    tema_preferido = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    activo = table.Column<bool>(type: "boolean", nullable: false),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    usuario_creacion = table.Column<long>(type: "bigint", nullable: true),
                    fecha_modificacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    usuario_modificacion = table.Column<long>(type: "bigint", nullable: true),
                    eliminado = table.Column<bool>(type: "boolean", nullable: false),
                    fecha_eliminacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    usuario_eliminacion = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuario", x => x.id);
                    table.ForeignKey(
                        name: "FK_usuario_empresa_empresa_id",
                        column: x => x.empresa_id,
                        principalSchema: "emp",
                        principalTable: "empresa",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_usuario_sucursal_sucursal_id",
                        column: x => x.sucursal_id,
                        principalSchema: "emp",
                        principalTable: "sucursal",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "sesion_usuario",
                schema: "seg",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    usuario_id = table.Column<long>(type: "bigint", nullable: false),
                    token = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    refresh_token = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    fecha_inicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    fecha_expiracion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    fecha_ultima_actividad = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    direccion_ip = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    user_agent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    dispositivo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    navegador = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    sistema_operativo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    pais_acceso = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ciudad_acceso = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    activo = table.Column<bool>(type: "boolean", nullable: false),
                    cerrada_por_usuario = table.Column<bool>(type: "boolean", nullable: false),
                    fecha_cierre = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sesion_usuario", x => x.id);
                    table.ForeignKey(
                        name: "FK_sesion_usuario_usuario_usuario_id",
                        column: x => x.usuario_id,
                        principalSchema: "seg",
                        principalTable: "usuario",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "usuario_rol",
                schema: "seg",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    usuario_id = table.Column<long>(type: "bigint", nullable: false),
                    rol_id = table.Column<long>(type: "bigint", nullable: false),
                    es_principal = table.Column<bool>(type: "boolean", nullable: false),
                    fecha_inicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    fecha_fin = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    activo = table.Column<bool>(type: "boolean", nullable: false),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    usuario_creacion = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuario_rol", x => x.id);
                    table.ForeignKey(
                        name: "FK_usuario_rol_rol_rol_id",
                        column: x => x.rol_id,
                        principalSchema: "seg",
                        principalTable: "rol",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_usuario_rol_usuario_usuario_id",
                        column: x => x.usuario_id,
                        principalSchema: "seg",
                        principalTable: "usuario",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "usuario_sucursal",
                schema: "seg",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    usuario_id = table.Column<long>(type: "bigint", nullable: false),
                    sucursal_id = table.Column<long>(type: "bigint", nullable: false),
                    es_principal = table.Column<bool>(type: "boolean", nullable: false),
                    activo = table.Column<bool>(type: "boolean", nullable: false),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    usuario_creacion = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuario_sucursal", x => x.id);
                    table.ForeignKey(
                        name: "FK_usuario_sucursal_sucursal_sucursal_id",
                        column: x => x.sucursal_id,
                        principalSchema: "emp",
                        principalTable: "sucursal",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_usuario_sucursal_usuario_usuario_id",
                        column: x => x.usuario_id,
                        principalSchema: "seg",
                        principalTable: "usuario",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_banco_codigo",
                schema: "cat",
                table: "banco",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_banco_pais_id",
                schema: "cat",
                table: "banco",
                column: "pais_id");

            migrationBuilder.CreateIndex(
                name: "IX_ciudad_estado_provincia_id",
                schema: "cat",
                table: "ciudad",
                column: "estado_provincia_id");

            migrationBuilder.CreateIndex(
                name: "IX_empresa_codigo",
                schema: "emp",
                table: "empresa",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_empresa_moneda_principal_id",
                schema: "emp",
                table: "empresa",
                column: "moneda_principal_id");

            migrationBuilder.CreateIndex(
                name: "IX_empresa_numero_identificacion",
                schema: "emp",
                table: "empresa",
                column: "numero_identificacion",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_empresa_pais_id",
                schema: "emp",
                table: "empresa",
                column: "pais_id");

            migrationBuilder.CreateIndex(
                name: "IX_estado_provincia_pais_id_codigo",
                schema: "cat",
                table: "estado_provincia",
                columns: new[] { "pais_id", "codigo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_forma_pago_empresa_id",
                schema: "cat",
                table: "forma_pago",
                column: "empresa_id");

            migrationBuilder.CreateIndex(
                name: "IX_impuesto_empresa_id",
                schema: "cat",
                table: "impuesto",
                column: "empresa_id");

            migrationBuilder.CreateIndex(
                name: "IX_modulo_codigo",
                schema: "seg",
                table: "modulo",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_modulo_modulo_padre_id",
                schema: "seg",
                table: "modulo",
                column: "modulo_padre_id");

            migrationBuilder.CreateIndex(
                name: "IX_moneda_codigo",
                schema: "cat",
                table: "moneda",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_pais_codigo",
                schema: "cat",
                table: "pais",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_pais_codigo_iso2",
                schema: "cat",
                table: "pais",
                column: "codigo_iso2",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_permiso_codigo",
                schema: "seg",
                table: "permiso",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_permiso_modulo_id",
                schema: "seg",
                table: "permiso",
                column: "modulo_id");

            migrationBuilder.CreateIndex(
                name: "IX_rol_codigo_empresa_id",
                schema: "seg",
                table: "rol",
                columns: new[] { "codigo", "empresa_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_rol_empresa_id",
                schema: "seg",
                table: "rol",
                column: "empresa_id");

            migrationBuilder.CreateIndex(
                name: "IX_rol_permiso_permiso_id",
                schema: "seg",
                table: "rol_permiso",
                column: "permiso_id");

            migrationBuilder.CreateIndex(
                name: "IX_rol_permiso_rol_id_permiso_id",
                schema: "seg",
                table: "rol_permiso",
                columns: new[] { "rol_id", "permiso_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sesion_usuario_refresh_token",
                schema: "seg",
                table: "sesion_usuario",
                column: "refresh_token");

            migrationBuilder.CreateIndex(
                name: "IX_sesion_usuario_token",
                schema: "seg",
                table: "sesion_usuario",
                column: "token");

            migrationBuilder.CreateIndex(
                name: "IX_sesion_usuario_usuario_id",
                schema: "seg",
                table: "sesion_usuario",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "IX_sucursal_ciudad_id",
                schema: "emp",
                table: "sucursal",
                column: "ciudad_id");

            migrationBuilder.CreateIndex(
                name: "IX_sucursal_empresa_id_codigo",
                schema: "emp",
                table: "sucursal",
                columns: new[] { "empresa_id", "codigo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sucursal_estado_provincia_id",
                schema: "emp",
                table: "sucursal",
                column: "estado_provincia_id");

            migrationBuilder.CreateIndex(
                name: "IX_sucursal_pais_id",
                schema: "emp",
                table: "sucursal",
                column: "pais_id");

            migrationBuilder.CreateIndex(
                name: "IX_tipo_identificacion_codigo",
                schema: "cat",
                table: "tipo_identificacion",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tipo_identificacion_pais_id",
                schema: "cat",
                table: "tipo_identificacion",
                column: "pais_id");

            migrationBuilder.CreateIndex(
                name: "IX_usuario_email",
                schema: "seg",
                table: "usuario",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuario_empresa_id",
                schema: "seg",
                table: "usuario",
                column: "empresa_id");

            migrationBuilder.CreateIndex(
                name: "IX_usuario_estado",
                schema: "seg",
                table: "usuario",
                column: "estado");

            migrationBuilder.CreateIndex(
                name: "IX_usuario_sucursal_id",
                schema: "seg",
                table: "usuario",
                column: "sucursal_id");

            migrationBuilder.CreateIndex(
                name: "IX_usuario_rol_rol_id",
                schema: "seg",
                table: "usuario_rol",
                column: "rol_id");

            migrationBuilder.CreateIndex(
                name: "IX_usuario_rol_usuario_id_rol_id",
                schema: "seg",
                table: "usuario_rol",
                columns: new[] { "usuario_id", "rol_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuario_sucursal_sucursal_id",
                schema: "seg",
                table: "usuario_sucursal",
                column: "sucursal_id");

            migrationBuilder.CreateIndex(
                name: "IX_usuario_sucursal_usuario_id_sucursal_id",
                schema: "seg",
                table: "usuario_sucursal",
                columns: new[] { "usuario_id", "sucursal_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "banco",
                schema: "cat");

            migrationBuilder.DropTable(
                name: "forma_pago",
                schema: "cat");

            migrationBuilder.DropTable(
                name: "impuesto",
                schema: "cat");

            migrationBuilder.DropTable(
                name: "rol_permiso",
                schema: "seg");

            migrationBuilder.DropTable(
                name: "sesion_usuario",
                schema: "seg");

            migrationBuilder.DropTable(
                name: "tipo_identificacion",
                schema: "cat");

            migrationBuilder.DropTable(
                name: "usuario_rol",
                schema: "seg");

            migrationBuilder.DropTable(
                name: "usuario_sucursal",
                schema: "seg");

            migrationBuilder.DropTable(
                name: "permiso",
                schema: "seg");

            migrationBuilder.DropTable(
                name: "rol",
                schema: "seg");

            migrationBuilder.DropTable(
                name: "usuario",
                schema: "seg");

            migrationBuilder.DropTable(
                name: "modulo",
                schema: "seg");

            migrationBuilder.DropTable(
                name: "sucursal",
                schema: "emp");

            migrationBuilder.DropTable(
                name: "ciudad",
                schema: "cat");

            migrationBuilder.DropTable(
                name: "empresa",
                schema: "emp");

            migrationBuilder.DropTable(
                name: "estado_provincia",
                schema: "cat");

            migrationBuilder.DropTable(
                name: "moneda",
                schema: "cat");

            migrationBuilder.DropTable(
                name: "pais",
                schema: "cat");
        }
    }
}

// =====================================================
// APPLICATION LAYER - DTOs de Catálogos
// Archivo: Application/DTOs/Catalogos/
// =====================================================

namespace SolarisPlatform.Application.DTOs.Catalogos;

// ══════════════════════════════════════════════════════
// PAÍS
// ══════════════════════════════════════════════════════
public record PaisDto(
    long Id,
    string Codigo,
    string CodigoIso2,
    string Nombre,
    string? NombreIngles,
    string? CodigoTelefonico,
    string? Bandera,
    bool Activo,
    int Orden
);

public record CrearPaisRequest(
    string Codigo,
    string CodigoIso2,
    string Nombre,
    string? NombreIngles,
    string? CodigoTelefonico,
    string? Bandera,
    int Orden = 0
);

public record ActualizarPaisRequest(
    string Nombre,
    string? NombreIngles,
    string? CodigoTelefonico,
    string? Bandera,
    bool Activo,
    int Orden,
    string? Codigo = null,     // FIX: opcional — se preserva si no viene
    string? CodigoIso2 = null  // FIX: opcional — se preserva si no viene
);

// ══════════════════════════════════════════════════════
// ESTADO / PROVINCIA
// ══════════════════════════════════════════════════════
public record EstadoProvinciaDto(
    long Id,
    long PaisId,
    string PaisNombre,
    string Codigo,
    string Nombre,
    bool Activo,
    int Orden
);

public record CrearEstadoProvinciaRequest(
    long PaisId,
    string Codigo,
    string Nombre,
    int Orden = 0
);

public record ActualizarEstadoProvinciaRequest(
    long PaisId,
    string Nombre,
    bool Activo,
    int Orden,
    string? Codigo = null  // FIX: opcional — se preserva si no viene
);

// ══════════════════════════════════════════════════════
// CIUDAD
// ══════════════════════════════════════════════════════
public record CiudadDto(
    long Id,
    long EstadoProvinciaId,
    string EstadoProvinciaNombre,
    string PaisNombre,
    string? Codigo,
    string Nombre,
    bool Activo,
    int Orden
);

public record CrearCiudadRequest(
    long EstadoProvinciaId,
    string? Codigo,
    string Nombre,
    int Orden = 0
);

public record ActualizarCiudadRequest(
    long EstadoProvinciaId,
    string? Codigo,
    string Nombre,
    bool Activo,
    int Orden
);

// ══════════════════════════════════════════════════════
// MONEDA
// ══════════════════════════════════════════════════════
public record MonedaDto(
    long Id,
    string Codigo,
    string Nombre,
    string Simbolo,
    byte DecimalesPermitidos,
    bool Activo,
    int Orden
);

public record CrearMonedaRequest(
    string Codigo,
    string Nombre,
    string Simbolo,
    byte DecimalesPermitidos = 2,
    int Orden = 0
);

public record ActualizarMonedaRequest(
    string Nombre,
    string Simbolo,
    byte DecimalesPermitidos,
    bool Activo,
    int Orden,
    string? Codigo = null  // FIX: opcional — se preserva si no viene
);

// ══════════════════════════════════════════════════════
// TIPO DE IDENTIFICACIÓN
// ══════════════════════════════════════════════════════
public record TipoIdentificacionDto(
    long Id,
    long? PaisId,
    string? PaisNombre,
    string Codigo,
    string Nombre,
    int? Longitud,
    string? Patron,
    bool AplicaPersona,
    bool AplicaEmpresa,
    bool Activo,
    int Orden
);

public record CrearTipoIdentificacionRequest(
    long? PaisId,
    string Nombre,
    int? Longitud,
    string? Patron,
    bool AplicaPersona = true,
    bool AplicaEmpresa = true,
    int Orden = 0,
    string? Codigo = null  // FIX: opcional — se autogenera si no se envía
);

public record ActualizarTipoIdentificacionRequest(
    long? PaisId,
    string Codigo,
    string Nombre,
    int? Longitud,
    string? Patron,
    bool AplicaPersona,
    bool AplicaEmpresa,
    bool Activo,
    int Orden
);

// ══════════════════════════════════════════════════════
// IMPUESTO
// ══════════════════════════════════════════════════════
public record ImpuestoDto(
    long Id,
    long? EmpresaId,
    string Codigo,
    string Nombre,
    decimal Porcentaje,
    string TipoImpuesto,
    bool EsRetencion,
    bool Activo,
    int Orden
);

public record CrearImpuestoRequest(
    long? EmpresaId,
    string Nombre,
    decimal Porcentaje,
    bool EsRetencion = false,
    int Orden = 0,
    string? Codigo = null,         // FIX: opcional — se autogenera si no se envía
    string? TipoImpuesto = null    // FIX: opcional — default "IVA"
);

public record ActualizarImpuestoRequest(
    string Codigo,
    string Nombre,
    decimal Porcentaje,
    string TipoImpuesto,
    bool EsRetencion,
    bool Activo,
    int Orden
);

// ══════════════════════════════════════════════════════
// FORMA DE PAGO
// ══════════════════════════════════════════════════════
public record FormaPagoDto(
    long Id,
    long? EmpresaId,
    string Codigo,
    string Nombre,
    string Tipo,
    int DiasCredito,
    bool RequiereBanco,
    bool RequiereReferencia,
    bool Activo,
    int Orden
);

public record CrearFormaPagoRequest(
    long? EmpresaId,
    string Nombre,
    int DiasCredito = 0,
    bool RequiereBanco = false,
    bool RequiereReferencia = false,
    int Orden = 0,
    string? Codigo = null,   // FIX: opcional — se autogenera si no se envía
    string? Tipo = null      // FIX: opcional — default "CONTADO"
);

public record ActualizarFormaPagoRequest(
    string Nombre,
    int DiasCredito,
    bool RequiereBanco,
    bool RequiereReferencia,
    bool Activo,
    int Orden,
    string? Codigo = null,  // FIX: opcional — no requerido en PUT
    string? Tipo = null     // FIX: opcional — no requerido en PUT
);

// ══════════════════════════════════════════════════════
// BANCO
// ══════════════════════════════════════════════════════
public record BancoDto(
    long Id,
    long? PaisId,
    string? PaisNombre,
    string Codigo,
    string Nombre,
    string? NombreCorto,
    bool Activo,
    int Orden
);

public record CrearBancoRequest(
    long? PaisId,
    string Nombre,
    string? NombreCorto,
    int Orden = 0,
    string? Codigo = null  // FIX: opcional — se autogenera si no se envía
);

public record ActualizarBancoRequest(
    long? PaisId,
    string Nombre,
    string? NombreCorto,
    bool Activo,
    int Orden,
    string? Codigo = null  // FIX: opcional — no requerido en PUT
);

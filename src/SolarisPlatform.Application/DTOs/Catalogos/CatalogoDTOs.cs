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
    string Codigo,
    string CodigoIso2,
    string Nombre,
    string? NombreIngles,
    string? CodigoTelefonico,
    string? Bandera,
    bool Activo,
    int Orden
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
    string Codigo,
    string Nombre,
    bool Activo,
    int Orden
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
    string Codigo,
    string Nombre,
    string Simbolo,
    byte DecimalesPermitidos,
    bool Activo,
    int Orden
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
    string Codigo,
    string Nombre,
    int? Longitud,
    string? Patron,
    bool AplicaPersona = true,
    bool AplicaEmpresa = true,
    int Orden = 0
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
    string Codigo,
    string Nombre,
    decimal Porcentaje,
    string TipoImpuesto,
    bool EsRetencion = false,
    int Orden = 0
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
    string Codigo,
    string Nombre,
    string Tipo,
    int DiasCredito = 0,
    bool RequiereBanco = false,
    bool RequiereReferencia = false,
    int Orden = 0
);

public record ActualizarFormaPagoRequest(
    string Codigo,
    string Nombre,
    string Tipo,
    int DiasCredito,
    bool RequiereBanco,
    bool RequiereReferencia,
    bool Activo,
    int Orden
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
    string Codigo,
    string Nombre,
    string? NombreCorto,
    int Orden = 0
);

public record ActualizarBancoRequest(
    long? PaisId,
    string Codigo,
    string Nombre,
    string? NombreCorto,
    bool Activo,
    int Orden
);

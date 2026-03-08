// =====================================================
// APPLICATION LAYER - Validadores FluentValidation
// Archivo: Application/Validators/Catalogos/
// =====================================================

using FluentValidation;
using SolarisPlatform.Application.DTOs.Catalogos;

namespace SolarisPlatform.Application.Validators.Catalogos;

// ══════════════════════════════════════════════════════
// PAÍS
// ══════════════════════════════════════════════════════
public class CrearPaisValidator : AbstractValidator<CrearPaisRequest>
{
    public CrearPaisValidator()
    {
        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("El código es requerido.")
            .Length(3).WithMessage("El código debe tener exactamente 3 caracteres (ISO 3166-1 alpha-3).")
            .Matches("^[A-Z]{3}$").WithMessage("El código debe contener solo letras mayúsculas.");

        RuleFor(x => x.CodigoIso2)
            .NotEmpty().WithMessage("El código ISO2 es requerido.")
            .Length(2).WithMessage("El código ISO2 debe tener exactamente 2 caracteres.")
            .Matches("^[A-Z]{2}$").WithMessage("El código ISO2 debe contener solo letras mayúsculas.");

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es requerido.")
            .MaximumLength(100).WithMessage("El nombre no puede superar 100 caracteres.");

        RuleFor(x => x.NombreIngles)
            .MaximumLength(100).WithMessage("El nombre en inglés no puede superar 100 caracteres.")
            .When(x => x.NombreIngles is not null);

        RuleFor(x => x.CodigoTelefonico)
            .MaximumLength(10).WithMessage("El código telefónico no puede superar 10 caracteres.")
            .When(x => x.CodigoTelefonico is not null);

        RuleFor(x => x.Orden)
            .GreaterThanOrEqualTo(0).WithMessage("El orden debe ser mayor o igual a 0.");
    }
}

public class ActualizarPaisValidator : AbstractValidator<ActualizarPaisRequest>
{
    public ActualizarPaisValidator()
    {
        // FIX: Codigo y CodigoIso2 opcionales en PUT — se preservan del registro existente si no vienen
        RuleFor(x => x.Codigo)
            .Length(3).WithMessage("El código debe tener exactamente 3 caracteres.")
            .Matches("^[A-Z]{3}$").WithMessage("El código debe contener solo letras mayúsculas.")
            .When(x => !string.IsNullOrEmpty(x.Codigo));

        RuleFor(x => x.CodigoIso2)
            .Length(2).WithMessage("El código ISO2 debe tener exactamente 2 caracteres.")
            .Matches("^[A-Z]{2}$").WithMessage("El código ISO2 debe contener solo letras mayúsculas.")
            .When(x => !string.IsNullOrEmpty(x.CodigoIso2));

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es requerido.")
            .MaximumLength(100).WithMessage("El nombre no puede superar 100 caracteres.");
    }
}

// ══════════════════════════════════════════════════════
// ESTADO / PROVINCIA
// ══════════════════════════════════════════════════════
public class CrearEstadoProvinciaValidator : AbstractValidator<CrearEstadoProvinciaRequest>
{
    public CrearEstadoProvinciaValidator()
    {
        RuleFor(x => x.PaisId)
            .GreaterThan(0).WithMessage("Debe seleccionar un país válido.");

        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("El código es requerido.")
            .MaximumLength(10).WithMessage("El código no puede superar 10 caracteres.");

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es requerido.")
            .MaximumLength(100).WithMessage("El nombre no puede superar 100 caracteres.");
    }
}

public class ActualizarEstadoProvinciaValidator : AbstractValidator<ActualizarEstadoProvinciaRequest>
{
    public ActualizarEstadoProvinciaValidator()
    {
        RuleFor(x => x.PaisId)
            .GreaterThan(0).WithMessage("Debe seleccionar un país válido.");

        // FIX: Codigo opcional en PUT
        RuleFor(x => x.Codigo)
            .MaximumLength(10).WithMessage("El código no puede superar 10 caracteres.")
            .When(x => !string.IsNullOrEmpty(x.Codigo));

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es requerido.")
            .MaximumLength(100).WithMessage("El nombre no puede superar 100 caracteres.");
    }
}

// ══════════════════════════════════════════════════════
// CIUDAD
// ══════════════════════════════════════════════════════
public class CrearCiudadValidator : AbstractValidator<CrearCiudadRequest>
{
    public CrearCiudadValidator()
    {
        RuleFor(x => x.EstadoProvinciaId)
            .GreaterThan(0).WithMessage("Debe seleccionar una provincia/estado válido.");

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es requerido.")
            .MaximumLength(100).WithMessage("El nombre no puede superar 100 caracteres.");

        RuleFor(x => x.Codigo)
            .MaximumLength(10).WithMessage("El código no puede superar 10 caracteres.")
            .When(x => x.Codigo is not null);
    }
}

public class ActualizarCiudadValidator : AbstractValidator<ActualizarCiudadRequest>
{
    public ActualizarCiudadValidator()
    {
        RuleFor(x => x.EstadoProvinciaId)
            .GreaterThan(0).WithMessage("Debe seleccionar una provincia/estado válido.");

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es requerido.")
            .MaximumLength(100).WithMessage("El nombre no puede superar 100 caracteres.");
    }
}

// ══════════════════════════════════════════════════════
// MONEDA
// ══════════════════════════════════════════════════════
public class CrearMonedaValidator : AbstractValidator<CrearMonedaRequest>
{
    public CrearMonedaValidator()
    {
        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("El código es requerido.")
            .Length(3).WithMessage("El código debe tener exactamente 3 caracteres (ISO 4217).")
            .Matches("^[A-Z]{3}$").WithMessage("El código debe contener solo letras mayúsculas.");

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es requerido.")
            .MaximumLength(100).WithMessage("El nombre no puede superar 100 caracteres.");

        RuleFor(x => x.Simbolo)
            .NotEmpty().WithMessage("El símbolo es requerido.")
            .MaximumLength(10).WithMessage("El símbolo no puede superar 10 caracteres.");

        RuleFor(x => x.DecimalesPermitidos)
            .InclusiveBetween((byte)0, (byte)6).WithMessage("Los decimales permitidos deben estar entre 0 y 6.");
    }
}

public class ActualizarMonedaValidator : AbstractValidator<ActualizarMonedaRequest>
{
    public ActualizarMonedaValidator()
    {
        // FIX: Codigo opcional en PUT — se preserva del registro existente si no viene
        RuleFor(x => x.Codigo)
            .Length(3).WithMessage("El código debe tener exactamente 3 caracteres.")
            .Matches("^[A-Z]{3}$").WithMessage("El código debe contener solo letras mayúsculas.")
            .When(x => !string.IsNullOrEmpty(x.Codigo));

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es requerido.")
            .MaximumLength(100).WithMessage("El nombre no puede superar 100 caracteres.");

        RuleFor(x => x.Simbolo)
            .NotEmpty().WithMessage("El símbolo es requerido.")
            .MaximumLength(10).WithMessage("El símbolo no puede superar 10 caracteres.");

        RuleFor(x => x.DecimalesPermitidos)
            .InclusiveBetween((byte)0, (byte)6).WithMessage("Los decimales permitidos deben estar entre 0 y 6.");
    }
}

// ══════════════════════════════════════════════════════
// TIPO IDENTIFICACIÓN
// ══════════════════════════════════════════════════════
public class CrearTipoIdentificacionValidator : AbstractValidator<CrearTipoIdentificacionRequest>
{
    public CrearTipoIdentificacionValidator()
    {
        // FIX: Codigo opcional en POST — se autogenera en el servicio si no viene
        RuleFor(x => x.Codigo)
            .MaximumLength(10).WithMessage("El código no puede superar 10 caracteres.")
            .Matches("^[A-Z0-9_]+$").WithMessage("El código solo puede contener letras mayúsculas, números y guion bajo.")
            .When(x => !string.IsNullOrEmpty(x.Codigo));

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es requerido.")
            .MaximumLength(100).WithMessage("El nombre no puede superar 100 caracteres.");

        RuleFor(x => x.Longitud)
            .GreaterThan(0).WithMessage("La longitud debe ser mayor a 0.")
            .When(x => x.Longitud.HasValue);

        RuleFor(x => x.Patron)
            .MaximumLength(100).WithMessage("El patrón no puede superar 100 caracteres.")
            .When(x => x.Patron is not null);

        RuleFor(x => x)
            .Must(x => x.AplicaPersona || x.AplicaEmpresa)
            .WithMessage("El tipo debe aplicar al menos a persona o empresa.")
            .WithName("AplicaPersonaOEmpresa");
    }
}

public class ActualizarTipoIdentificacionValidator : AbstractValidator<ActualizarTipoIdentificacionRequest>
{
    public ActualizarTipoIdentificacionValidator()
    {
        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("El código es requerido.")
            .MaximumLength(10).WithMessage("El código no puede superar 10 caracteres.")
            .Matches("^[A-Z0-9_]+$").WithMessage("El código solo puede contener letras mayúsculas, números y guion bajo.");

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es requerido.")
            .MaximumLength(100).WithMessage("El nombre no puede superar 100 caracteres.");

        RuleFor(x => x)
            .Must(x => x.AplicaPersona || x.AplicaEmpresa)
            .WithMessage("El tipo debe aplicar al menos a persona o empresa.")
            .WithName("AplicaPersonaOEmpresa");
    }
}

// ══════════════════════════════════════════════════════
// IMPUESTO
// ══════════════════════════════════════════════════════
public class CrearImpuestoValidator : AbstractValidator<CrearImpuestoRequest>
{
    private static readonly string[] TiposValidos = ["IVA", "RETENCION", "RETENCION_IVA", "ICE", "OTRO"];

    public CrearImpuestoValidator()
    {
        // FIX: Codigo y TipoImpuesto opcionales en POST — se autogeneran en el servicio
        RuleFor(x => x.Codigo)
            .MaximumLength(20).WithMessage("El código no puede superar 20 caracteres.")
            .When(x => !string.IsNullOrEmpty(x.Codigo));

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es requerido.")
            .MaximumLength(100).WithMessage("El nombre no puede superar 100 caracteres.");

        RuleFor(x => x.Porcentaje)
            .InclusiveBetween(0, 100).WithMessage("El porcentaje debe estar entre 0 y 100.");

        RuleFor(x => x.TipoImpuesto)
            .Must(t => t == null || TiposValidos.Contains(t))
            .WithMessage($"El tipo de impuesto debe ser uno de: {string.Join(", ", TiposValidos)}.")
            .When(x => x.TipoImpuesto != null);
    }
}

public class ActualizarImpuestoValidator : AbstractValidator<ActualizarImpuestoRequest>
{
    private static readonly string[] TiposValidos = ["IVA", "RETENCION", "RETENCION_IVA", "ICE", "OTRO"];

    public ActualizarImpuestoValidator()
    {
        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("El código es requerido.")
            .MaximumLength(20).WithMessage("El código no puede superar 20 caracteres.");

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es requerido.")
            .MaximumLength(100).WithMessage("El nombre no puede superar 100 caracteres.");

        RuleFor(x => x.Porcentaje)
            .InclusiveBetween(0, 100).WithMessage("El porcentaje debe estar entre 0 y 100.");

        RuleFor(x => x.TipoImpuesto)
            .NotEmpty().WithMessage("El tipo de impuesto es requerido.")
            .Must(t => TiposValidos.Contains(t))
            .WithMessage($"El tipo de impuesto debe ser uno de: {string.Join(", ", TiposValidos)}.");
    }
}

// ══════════════════════════════════════════════════════
// FORMA DE PAGO
// ══════════════════════════════════════════════════════
public class CrearFormaPagoValidator : AbstractValidator<CrearFormaPagoRequest>
{
    private static readonly string[] TiposValidos = ["EFECTIVO", "TARJETA", "TRANSFERENCIA", "CHEQUE", "CREDITO", "OTRO"];

    public CrearFormaPagoValidator()
    {
        // FIX: Codigo y Tipo opcionales en POST — se autogeneran en el servicio
        RuleFor(x => x.Codigo)
            .MaximumLength(20).WithMessage("El código no puede superar 20 caracteres.")
            .When(x => !string.IsNullOrEmpty(x.Codigo));

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es requerido.")
            .MaximumLength(100).WithMessage("El nombre no puede superar 100 caracteres.");

        RuleFor(x => x.Tipo)
            .Must(t => t == null || TiposValidos.Contains(t))
            .WithMessage($"El tipo debe ser uno de: {string.Join(", ", TiposValidos)}.")
            .When(x => x.Tipo != null);

        RuleFor(x => x.DiasCredito)
            .GreaterThanOrEqualTo(0).WithMessage("Los días de crédito no pueden ser negativos.");
    }
}

public class ActualizarFormaPagoValidator : AbstractValidator<ActualizarFormaPagoRequest>
{
    private static readonly string[] TiposValidos = ["EFECTIVO", "TARJETA", "TRANSFERENCIA", "CHEQUE", "CREDITO", "OTRO"];

    public ActualizarFormaPagoValidator()
    {
        // FIX: Codigo y Tipo opcionales en PUT — se preservan del registro si no vienen
        RuleFor(x => x.Codigo)
            .MaximumLength(20).WithMessage("El código no puede superar 20 caracteres.")
            .When(x => !string.IsNullOrEmpty(x.Codigo));

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es requerido.")
            .MaximumLength(100).WithMessage("El nombre no puede superar 100 caracteres.");

        RuleFor(x => x.Tipo)
            .Must(t => t == null || TiposValidos.Contains(t))
            .WithMessage($"El tipo debe ser uno de: {string.Join(", ", TiposValidos)}.")
            .When(x => x.Tipo != null);

        RuleFor(x => x.DiasCredito)
            .GreaterThanOrEqualTo(0).WithMessage("Los días de crédito no pueden ser negativos.");
    }
}

// ══════════════════════════════════════════════════════
// BANCO
// ══════════════════════════════════════════════════════
public class CrearBancoValidator : AbstractValidator<CrearBancoRequest>
{
    public CrearBancoValidator()
    {
        // FIX: Codigo opcional en POST — se autogenera en el servicio si no viene
        RuleFor(x => x.Codigo)
            .MaximumLength(20).WithMessage("El código no puede superar 20 caracteres.")
            .When(x => !string.IsNullOrEmpty(x.Codigo));

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es requerido.")
            .MaximumLength(100).WithMessage("El nombre no puede superar 100 caracteres.");

        RuleFor(x => x.NombreCorto)
            .MaximumLength(50).WithMessage("El nombre corto no puede superar 50 caracteres.")
            .When(x => x.NombreCorto is not null);
    }
}

public class ActualizarBancoValidator : AbstractValidator<ActualizarBancoRequest>
{
    public ActualizarBancoValidator()
    {
        // FIX: Codigo opcional en PUT — se preserva del registro si no viene
        RuleFor(x => x.Codigo)
            .MaximumLength(20).WithMessage("El código no puede superar 20 caracteres.")
            .When(x => !string.IsNullOrEmpty(x.Codigo));

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es requerido.")
            .MaximumLength(100).WithMessage("El nombre no puede superar 100 caracteres.");

        RuleFor(x => x.NombreCorto)
            .MaximumLength(50).WithMessage("El nombre corto no puede superar 50 caracteres.")
            .When(x => x.NombreCorto is not null);
    }
}

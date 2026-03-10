using FluentValidation.TestHelper;
using SolarisPlatform.Application.Validators.Catalogos;
using SolarisPlatform.Application.DTOs.Catalogos;

namespace SolarisPlatform.Tests.Unit.Application;

public class CatalogoValidatorTests
{
    private readonly CrearPaisValidator _paisValidator = new();

    [Fact]
    public void CrearPais_NombreVacio_DebefalFallar()
    {
        // FIX: "ECU" = 3 letras mayúsculas (ISO 3166-1 alpha-3), "EC" = ISO2 (2 letras)
        var req = new CrearPaisRequest("ECU", "EC", "", null, null, null, 0);
        _paisValidator.TestValidate(req).ShouldHaveValidationErrorFor(x => x.Nombre);
    }

    [Fact]
    public void CrearPais_CodigoVacio_DebefalFallar()
    {
        // Código vacío — el resto válido para que solo falle Codigo
        var req = new CrearPaisRequest("", "EC", "Ecuador", null, null, null, 0);
        _paisValidator.TestValidate(req).ShouldHaveValidationErrorFor(x => x.Codigo);
    }

    [Fact]
    public void CrearPais_DatosValidos_DebePasar()
    {
        // FIX: "ECU" = alpha-3 correcto (antes era "EC" de 2 letras → fallaba Length(3))
        var req = new CrearPaisRequest("ECU", "EC", "Ecuador", null, "+593", null, 0);
        _paisValidator.TestValidate(req).ShouldNotHaveAnyValidationErrors();
    }

    private readonly CrearMonedaValidator _monedaValidator = new();

    [Fact]
    public void CrearMoneda_NombreVacio_DebefalFallar()
    {
        var req = new CrearMonedaRequest("USD", "", "$", 2, 0);
        _monedaValidator.TestValidate(req).ShouldHaveValidationErrorFor(x => x.Nombre);
    }

    [Fact]
    public void CrearMoneda_DatosValidos_DebePasar()
    {
        var req = new CrearMonedaRequest("USD", "Dólar", "$", 2, 0);
        _monedaValidator.TestValidate(req).ShouldNotHaveAnyValidationErrors();
    }

    private readonly CrearImpuestoValidator _impuestoValidator = new();

    [Fact]
    public void CrearImpuesto_NombreVacio_DebefalFallar()
    {
        // FIX: Codigo es opcional en el validador (.When(x => !IsNullOrEmpty(Codigo)))
        // por lo tanto Codigo="" no genera error — el validador lo ignora intencionalmente.
        // El test correcto es verificar Nombre, que sí tiene NotEmpty().
        // Constructor: (EmpresaId, Nombre, Porcentaje, EsRetencion, Orden, Codigo, TipoImpuesto)
        var req = new CrearImpuestoRequest(null, "", 15m, true, 0, null, null);
        _impuestoValidator.TestValidate(req).ShouldHaveValidationErrorFor(x => x.Nombre);
    }

    [Fact]
    public void CrearImpuesto_DatosValidos_DebePasar()
    {
        var req = new CrearImpuestoRequest(null, "IVA 15%", 15m, true, 0, "IVA15", null);
        _impuestoValidator.TestValidate(req).ShouldNotHaveAnyValidationErrors();
    }

    private readonly CrearBancoValidator _bancoValidator = new();

    [Fact]
    public void CrearBanco_NombreVacio_DebefalFallar()
    {
        var req = new CrearBancoRequest(null, "", null, 0, null);
        _bancoValidator.TestValidate(req).ShouldHaveValidationErrorFor(x => x.Nombre);
    }

    [Fact]
    public void CrearBanco_DatosValidos_DebePasar()
    {
        var req = new CrearBancoRequest(null, "Banco Central", null, 0, null);
        _bancoValidator.TestValidate(req).ShouldNotHaveAnyValidationErrors();
    }
}
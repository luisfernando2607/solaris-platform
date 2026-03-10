using FluentValidation.TestHelper;
using SolarisPlatform.Application.Validators.RRHH;
using SolarisPlatform.Application.DTOs.RRHH;

namespace SolarisPlatform.Tests.Unit.Application;

public class EmpleadoValidatorTests
{
    private readonly CrearDepartamentoValidator _deptoValidator = new();

    [Fact]
    public void CrearDepartamento_NombreVacio_DebefalFallar()
    {
        var req = new CrearDepartamentoRequest("TI", "", null, null, null, null);
        var result = _deptoValidator.TestValidate(req);
        result.ShouldHaveValidationErrorFor(x => x.Nombre);
    }

    [Fact]
    public void CrearDepartamento_DatosValidos_DebePasar()
    {
        var req = new CrearDepartamentoRequest("TI", "Tecnología", null, null, null, null);
        var result = _deptoValidator.TestValidate(req);
        result.ShouldNotHaveAnyValidationErrors();
    }

    private readonly EgresarEmpleadoValidator _egresoValidator = new();

    [Fact]
    public void EgresarEmpleado_MotivoInvalido_DebefalFallar()
    {
        // MotivoEgreso=0 si el validator lo requiere mayor que 0
        var req = new EgresarEmpleadoRequest(DateOnly.FromDateTime(DateTime.Today), 0, null);
        var result = _egresoValidator.TestValidate(req);
        result.ShouldHaveAnyValidationError();
    }

    [Fact]
    public void EgresarEmpleado_DatosValidos_DebePasar()
    {
        var req = new EgresarEmpleadoRequest(DateOnly.FromDateTime(DateTime.Today), 1, "Renuncia voluntaria");
        var result = _egresoValidator.TestValidate(req);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

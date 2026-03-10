using FluentValidation.TestHelper;
using SolarisPlatform.Application.Validators;
using SolarisPlatform.Application.DTOs.Auth;
using Xunit;

namespace SolarisPlatform.Tests.Unit.Application;

public class AuthValidatorTests
{
    // ═══════════════════════════════════════════
    //  LOGIN — class LoginRequest { Email, Password, RecordarMe }
    // ═══════════════════════════════════════════
    private readonly LoginRequestValidator _loginValidator = new();

    [Fact]
    public void Login_EmailVacio_DebefalFallar()
    {
        var req = new LoginRequest { Email = "", Password = "Test@123" };
        var result = _loginValidator.TestValidate(req);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Login_EmailInvalido_DebefalFallar()
    {
        var req = new LoginRequest { Email = "no-es-email", Password = "Test@123" };
        var result = _loginValidator.TestValidate(req);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Login_PasswordVacio_DebefalFallar()
    {
        var req = new LoginRequest { Email = "admin@solaris.com", Password = "" };
        var result = _loginValidator.TestValidate(req);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Login_DatosValidos_DebePasar()
    {
        var req = new LoginRequest { Email = "admin@solaris.com", Password = "Admin@123456" };
        var result = _loginValidator.TestValidate(req);
        result.ShouldNotHaveAnyValidationErrors();
    }

    // ═══════════════════════════════════════════
    //  REGISTRO — class RegistroRequest { Email, Password, ConfirmarPassword, Nombres, Apellidos, EmpresaId, ... }
    // ═══════════════════════════════════════════
    private readonly RegistroRequestValidator _registroValidator = new();

    [Fact]
    public void Registro_EmailVacio_DebefalFallar()
    {
        var req = new RegistroRequest
        {
            Email = "", Password = "Test@123456", ConfirmarPassword = "Test@123456",
            Nombres = "Juan", Apellidos = "Perez", EmpresaId = 1
        };
        var result = _registroValidator.TestValidate(req);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Registro_PasswordsNoCoinciden_DebefalFallar()
    {
        var req = new RegistroRequest
        {
            Email = "juan@solaris.com", Password = "Test@123456", ConfirmarPassword = "Diferente@789",
            Nombres = "Juan", Apellidos = "Perez", EmpresaId = 1
        };
        var result = _registroValidator.TestValidate(req);
        result.ShouldHaveValidationErrorFor(x => x.ConfirmarPassword);
    }

    [Fact]
    public void Registro_DatosValidos_DebePasar()
    {
        var req = new RegistroRequest
        {
            Email = "juan@solaris.com", Password = "Test@123456", ConfirmarPassword = "Test@123456",
            Nombres = "Juan", Apellidos = "Perez", EmpresaId = 1
        };
        var result = _registroValidator.TestValidate(req);
        result.ShouldNotHaveAnyValidationErrors();
    }

    // ═══════════════════════════════════════════
    //  CAMBIAR PASSWORD
    // ═══════════════════════════════════════════
    private readonly CambiarPasswordRequestValidator _cambioValidator = new();

    [Fact]
    public void CambiarPassword_NuevoPasswordVacio_DebefalFallar()
    {
        var req = new CambiarPasswordRequest
        {
            PasswordActual = "Actual@123", NuevoPassword = "", ConfirmarPassword = ""
        };
        var result = _cambioValidator.TestValidate(req);
        result.ShouldHaveValidationErrorFor(x => x.NuevoPassword);
    }

    [Fact]
    public void CambiarPassword_DatosValidos_DebePasar()
    {
        var req = new CambiarPasswordRequest
        {
            PasswordActual = "Actual@123", NuevoPassword = "Nuevo@123456", ConfirmarPassword = "Nuevo@123456"
        };
        var result = _cambioValidator.TestValidate(req);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

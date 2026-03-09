using FluentValidation;
using SolarisPlatform.Application.DTOs.Auth;
using SolarisPlatform.Application.DTOs.Usuarios;
using SolarisPlatform.Application.DTOs.Roles;

namespace SolarisPlatform.Application.Validators;

/// <summary>
/// Validador para Login
/// </summary>
public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es requerido")
            .EmailAddress().WithMessage("El email no es válido")
            .MaximumLength(256).WithMessage("El email no puede exceder 256 caracteres");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es requerida")
            .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres");
    }
}

/// <summary>
/// Validador para Registro
/// </summary>
public class RegistroRequestValidator : AbstractValidator<RegistroRequest>
{
    public RegistroRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es requerido")
            .EmailAddress().WithMessage("El email no es válido")
            .MaximumLength(256).WithMessage("El email no puede exceder 256 caracteres");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es requerida")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres")
            .Matches("[A-Z]").WithMessage("La contraseña debe contener al menos una mayúscula")
            .Matches("[a-z]").WithMessage("La contraseña debe contener al menos una minúscula")
            .Matches("[0-9]").WithMessage("La contraseña debe contener al menos un número")
            .Matches("[^a-zA-Z0-9]").WithMessage("La contraseña debe contener al menos un carácter especial");

        RuleFor(x => x.ConfirmarPassword)
            .Equal(x => x.Password).WithMessage("Las contraseñas no coinciden");

        RuleFor(x => x.Nombres)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres");

        RuleFor(x => x.Apellidos)
            .NotEmpty().WithMessage("El apellido es requerido")
            .MaximumLength(100).WithMessage("El apellido no puede exceder 100 caracteres");

        RuleFor(x => x.EmpresaId)
            .GreaterThan(0).WithMessage("La empresa es requerida");
    }
}

/// <summary>
/// Validador para Cambio de Contraseña
/// </summary>
public class CambiarPasswordRequestValidator : AbstractValidator<CambiarPasswordRequest>
{
    public CambiarPasswordRequestValidator()
    {
        RuleFor(x => x.PasswordActual)
            .NotEmpty().WithMessage("La contraseña actual es requerida");

        RuleFor(x => x.NuevoPassword)
            .NotEmpty().WithMessage("La nueva contraseña es requerida")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres")
            .Matches("[A-Z]").WithMessage("La contraseña debe contener al menos una mayúscula")
            .Matches("[a-z]").WithMessage("La contraseña debe contener al menos una minúscula")
            .Matches("[0-9]").WithMessage("La contraseña debe contener al menos un número")
            .Matches("[^a-zA-Z0-9]").WithMessage("La contraseña debe contener al menos un carácter especial")
            .NotEqual(x => x.PasswordActual).WithMessage("La nueva contraseña debe ser diferente a la actual");

        RuleFor(x => x.ConfirmarPassword)
            .Equal(x => x.NuevoPassword).WithMessage("Las contraseñas no coinciden");
    }
}

/// <summary>
/// Validador para Recuperar Contraseña
/// </summary>
public class RecuperarPasswordRequestValidator : AbstractValidator<RecuperarPasswordRequest>
{
    public RecuperarPasswordRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es requerido")
            .EmailAddress().WithMessage("El email no es válido");
    }
}

/// <summary>
/// Validador para Resetear Contraseña
/// </summary>
public class ResetearPasswordRequestValidator : AbstractValidator<ResetearPasswordRequest>
{
    public ResetearPasswordRequestValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("El token es requerido");

        RuleFor(x => x.NuevoPassword)
            .NotEmpty().WithMessage("La nueva contraseña es requerida")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres")
            .Matches("[A-Z]").WithMessage("La contraseña debe contener al menos una mayúscula")
            .Matches("[a-z]").WithMessage("La contraseña debe contener al menos una minúscula")
            .Matches("[0-9]").WithMessage("La contraseña debe contener al menos un número")
            .Matches("[^a-zA-Z0-9]").WithMessage("La contraseña debe contener al menos un carácter especial");

        RuleFor(x => x.ConfirmarPassword)
            .Equal(x => x.NuevoPassword).WithMessage("Las contraseñas no coinciden");
    }
}

/// <summary>
/// Validador para Crear Usuario
/// </summary>
public class CrearUsuarioRequestValidator : AbstractValidator<CrearUsuarioRequest>
{
    public CrearUsuarioRequestValidator()
    {
        RuleFor(x => x.EmpresaId)
            .GreaterThan(0).WithMessage("La empresa es requerida");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es requerido")
            .EmailAddress().WithMessage("El email no es válido")
            .MaximumLength(256).WithMessage("El email no puede exceder 256 caracteres");

        RuleFor(x => x.Nombres)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres");

        RuleFor(x => x.Apellidos)
            .NotEmpty().WithMessage("El apellido es requerido")
            .MaximumLength(100).WithMessage("El apellido no puede exceder 100 caracteres");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es requerida")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres");

        RuleFor(x => x.Telefono)
            .MaximumLength(20).WithMessage("El teléfono no puede exceder 20 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Telefono));

        RuleFor(x => x.Genero)
            .Must(g => g == null || g == "M" || g == "F" || g == "O")
            .WithMessage("El género debe ser M, F u O");
    }
}

/// <summary>
/// Validador para Actualizar Usuario
/// </summary>
public class ActualizarUsuarioRequestValidator : AbstractValidator<ActualizarUsuarioRequest>
{
    public ActualizarUsuarioRequestValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El ID es requerido");

        RuleFor(x => x.Nombres)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres");

        RuleFor(x => x.Apellidos)
            .NotEmpty().WithMessage("El apellido es requerido")
            .MaximumLength(100).WithMessage("El apellido no puede exceder 100 caracteres");
    }
}

/// <summary>
/// Validador para Crear Rol
/// </summary>
public class CrearRolRequestValidator : AbstractValidator<CrearRolRequest>
{
    public CrearRolRequestValidator()
    {
        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("El código es requerido")
            .MaximumLength(50).WithMessage("El código no puede exceder 50 caracteres")
            .Matches("^[A-Z_]+$").WithMessage("El código solo puede contener mayúsculas y guiones bajos");

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres");

        RuleFor(x => x.Nivel)
            .InclusiveBetween(0, 100).WithMessage("El nivel debe estar entre 0 y 100");
    }
}

/// <summary>
/// Validador para Actualizar Rol
/// </summary>
public class ActualizarRolRequestValidator : AbstractValidator<ActualizarRolRequest>
{
    public ActualizarRolRequestValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El ID es requerido");

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres");

        RuleFor(x => x.Nivel)
            .InclusiveBetween(0, 100).WithMessage("El nivel debe estar entre 0 y 100");
    }
}

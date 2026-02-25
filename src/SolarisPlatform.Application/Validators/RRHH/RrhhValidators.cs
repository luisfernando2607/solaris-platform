using FluentValidation;
using SolarisPlatform.Application.DTOs.RRHH;

namespace SolarisPlatform.Application.Validators.RRHH;

public class CrearDepartamentoValidator : AbstractValidator<CrearDepartamentoRequest>
{
    public CrearDepartamentoValidator()
    {
        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("El código es requerido")
            .MaximumLength(20).WithMessage("El código no puede superar 20 caracteres")
            .Matches("^[A-Z0-9_-]+$").WithMessage("El código solo puede contener letras mayúsculas, números, guiones y guión bajo");

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(100).WithMessage("El nombre no puede superar 100 caracteres");

        RuleFor(x => x.Descripcion)
            .MaximumLength(500).When(x => x.Descripcion != null);

        RuleFor(x => x.PresupuestoAnual)
            .GreaterThanOrEqualTo(0).When(x => x.PresupuestoAnual.HasValue)
            .WithMessage("El presupuesto no puede ser negativo");
    }
}

public class CrearPuestoValidator : AbstractValidator<CrearPuestoRequest>
{
    public CrearPuestoValidator()
    {
        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("El código es requerido")
            .MaximumLength(20);

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(100);

        RuleFor(x => x.DepartamentoId)
            .GreaterThan(0).WithMessage("El departamento es requerido");

        RuleFor(x => x.NivelJerarquico)
            .InclusiveBetween((short)1, (short)4).WithMessage("Nivel jerárquico debe estar entre 1 y 4");

        RuleFor(x => x.BandaSalarialMax)
            .GreaterThanOrEqualTo(x => x.BandaSalarialMin ?? 0)
            .When(x => x.BandaSalarialMax.HasValue && x.BandaSalarialMin.HasValue)
            .WithMessage("El salario máximo debe ser mayor o igual al mínimo");
    }
}

public class CrearEmpleadoValidator : AbstractValidator<CrearEmpleadoRequest>
{
    public CrearEmpleadoValidator()
    {
        RuleFor(x => x.TipoIdentificacion)
            .NotEmpty().WithMessage("El tipo de identificación es requerido")
            .MaximumLength(10);

        RuleFor(x => x.NumeroIdentificacion)
            .NotEmpty().WithMessage("El número de identificación es requerido")
            .MaximumLength(30);

        RuleFor(x => x.PrimerNombre)
            .NotEmpty().WithMessage("El primer nombre es requerido")
            .MaximumLength(50);

        RuleFor(x => x.PrimerApellido)
            .NotEmpty().WithMessage("El primer apellido es requerido")
            .MaximumLength(50);

        RuleFor(x => x.EmailPersonal)
            .EmailAddress().When(x => !string.IsNullOrEmpty(x.EmailPersonal))
            .WithMessage("El email personal no tiene formato válido");

        RuleFor(x => x.EmailCorporativo)
            .EmailAddress().When(x => !string.IsNullOrEmpty(x.EmailCorporativo))
            .WithMessage("El email corporativo no tiene formato válido");

        RuleFor(x => x.FechaIngreso)
            .NotEmpty().WithMessage("La fecha de ingreso es requerida")
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("La fecha de ingreso no puede ser futura");

        RuleFor(x => x.SalarioBase)
            .GreaterThanOrEqualTo(0).WithMessage("El salario base no puede ser negativo");

        RuleFor(x => x.HorasSemanales)
            .InclusiveBetween(1, 80).WithMessage("Las horas semanales deben estar entre 1 y 80");

        RuleFor(x => x.TipoContrato)
            .InclusiveBetween((short)1, (short)4).WithMessage("Tipo de contrato inválido");

        RuleFor(x => x.ModalidadTrabajo)
            .InclusiveBetween((short)1, (short)3).WithMessage("Modalidad de trabajo inválida");
    }
}

public class EgresarEmpleadoValidator : AbstractValidator<EgresarEmpleadoRequest>
{
    public EgresarEmpleadoValidator()
    {
        RuleFor(x => x.FechaEgreso)
            .NotEmpty().WithMessage("La fecha de egreso es requerida");

        RuleFor(x => x.MotivoEgreso)
            .InclusiveBetween((short)1, (short)5).WithMessage("Motivo de egreso inválido");
    }
}

public class CrearSolicitudAusenciaValidator : AbstractValidator<CrearSolicitudAusenciaRequest>
{
    public CrearSolicitudAusenciaValidator()
    {
        RuleFor(x => x.EmpleadoId)
            .GreaterThan(0).WithMessage("El empleado es requerido");

        RuleFor(x => x.Tipo)
            .InclusiveBetween((short)1, (short)5).WithMessage("Tipo de ausencia inválido");

        RuleFor(x => x.FechaInicio)
            .NotEmpty().WithMessage("La fecha de inicio es requerida");

        RuleFor(x => x.FechaFin)
            .NotEmpty().WithMessage("La fecha de fin es requerida")
            .GreaterThanOrEqualTo(x => x.FechaInicio)
            .WithMessage("La fecha de fin debe ser igual o posterior a la fecha de inicio");
    }
}

public class CrearConceptoNominaValidator : AbstractValidator<CrearConceptoNominaRequest>
{
    public CrearConceptoNominaValidator()
    {
        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("El código es requerido")
            .MaximumLength(20);

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(100);

        RuleFor(x => x.Tipo)
            .InclusiveBetween((short)1, (short)3).WithMessage("Tipo de concepto inválido (1=Ingreso, 2=Descuento, 3=Aporte Patronal)");

        RuleFor(x => x.FormaCalculo)
            .InclusiveBetween((short)1, (short)5).WithMessage("Forma de cálculo inválida");

        RuleFor(x => x.Porcentaje)
            .InclusiveBetween(0, 100).When(x => x.Porcentaje.HasValue)
            .WithMessage("El porcentaje debe estar entre 0 y 100");

        RuleFor(x => x.ValorFijo)
            .GreaterThanOrEqualTo(0).When(x => x.ValorFijo.HasValue)
            .WithMessage("El valor fijo no puede ser negativo");
    }
}

public class CrearPeriodoNominaValidator : AbstractValidator<CrearPeriodoNominaRequest>
{
    public CrearPeriodoNominaValidator()
    {
        RuleFor(x => x.Anno)
            .InclusiveBetween(2020, 2099).WithMessage("Año inválido");

        RuleFor(x => x.NumeroPeriodo)
            .GreaterThan(0).WithMessage("El número de período debe ser mayor a 0");

        RuleFor(x => x.Descripcion)
            .NotEmpty().WithMessage("La descripción es requerida")
            .MaximumLength(100);

        RuleFor(x => x.FechaInicio)
            .NotEmpty().WithMessage("La fecha de inicio es requerida");

        RuleFor(x => x.FechaFin)
            .NotEmpty()
            .GreaterThanOrEqualTo(x => x.FechaInicio)
            .WithMessage("La fecha de fin debe ser posterior a la de inicio");
    }
}

public class CrearPrestamoValidator : AbstractValidator<CrearPrestamoRequest>
{
    public CrearPrestamoValidator()
    {
        RuleFor(x => x.EmpleadoId)
            .GreaterThan(0).WithMessage("El empleado es requerido");

        RuleFor(x => x.MontoSolicitado)
            .GreaterThan(0).WithMessage("El monto debe ser mayor a 0");

        RuleFor(x => x.NumeroCuotas)
            .InclusiveBetween(1, 60).WithMessage("El número de cuotas debe estar entre 1 y 60");

        RuleFor(x => x.FechaSolicitud)
            .NotEmpty().WithMessage("La fecha de solicitud es requerida");
    }
}

public class CrearCapacitacionValidator : AbstractValidator<CrearCapacitacionRequest>
{
    public CrearCapacitacionValidator()
    {
        RuleFor(x => x.Codigo)
            .NotEmpty().MaximumLength(20);

        RuleFor(x => x.Nombre)
            .NotEmpty().MaximumLength(200);

        RuleFor(x => x.FechaInicio)
            .NotEmpty();

        RuleFor(x => x.FechaFin)
            .NotEmpty()
            .GreaterThanOrEqualTo(x => x.FechaInicio)
            .WithMessage("La fecha de fin debe ser posterior a la de inicio");

        RuleFor(x => x.Costo)
            .GreaterThanOrEqualTo(0).When(x => x.Costo.HasValue);
    }
}

public class CompletarEvaluacionValidator : AbstractValidator<CompletarEvaluacionRequest>
{
    public CompletarEvaluacionValidator()
    {
        RuleFor(x => x.Respuestas)
            .NotNull().NotEmpty().WithMessage("Se requieren al menos una respuesta");

        RuleForEach(x => x.Respuestas).ChildRules(r =>
        {
            r.RuleFor(x => x.CriterioId).GreaterThan(0);
            r.RuleFor(x => x.Puntuacion)
                .InclusiveBetween(0, 10)
                .When(x => x.Puntuacion.HasValue);
        });
    }
}

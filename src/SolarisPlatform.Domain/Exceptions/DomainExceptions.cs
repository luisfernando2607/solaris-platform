namespace SolarisPlatform.Domain.Exceptions;

public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message) { }
    protected DomainException(string message, Exception innerException) : base(message, innerException) { }
}

public class NotFoundException : DomainException
{
    public string EntityName { get; }
    public object Key { get; }
    
    public NotFoundException(string entityName, object key)
        : base($"La entidad '{entityName}' con identificador '{key}' no fue encontrada.")
    {
        EntityName = entityName;
        Key = key;
    }
}

public class DuplicateException : DomainException
{
    public string EntityName { get; }
    public string Field { get; }
    public object Value { get; }
    
    public DuplicateException(string entityName, string field, object value)
        : base($"Ya existe un registro de '{entityName}' con {field} = '{value}'.")
    {
        EntityName = entityName;
        Field = field;
        Value = value;
    }
}

public class ValidationException : DomainException
{
    public IDictionary<string, string[]> Errors { get; }
    
    public ValidationException() : base("Se han producido errores de validación.")
    {
        Errors = new Dictionary<string, string[]>();
    }
    
    public ValidationException(IDictionary<string, string[]> errors) : base("Se han producido errores de validación.")
    {
        Errors = errors;
    }
    
    public ValidationException(string field, string message) : base(message)
    {
        Errors = new Dictionary<string, string[]> { { field, new[] { message } } };
    }
}

public class AuthenticationException : DomainException
{
    public AuthenticationException(string message = "Error de autenticación.") : base(message) { }
}

public class UserLockedException : DomainException
{
    public DateTime? FechaDesbloqueo { get; }
    
    public UserLockedException(DateTime? fechaDesbloqueo = null) : base("El usuario se encuentra bloqueado.")
    {
        FechaDesbloqueo = fechaDesbloqueo;
    }
}

public class InvalidCredentialsException : DomainException
{
    public int IntentosRestantes { get; }
    
    public InvalidCredentialsException(int intentosRestantes = 0) : base("Credenciales inválidas.")
    {
        IntentosRestantes = intentosRestantes;
    }
}

public class TokenException : DomainException
{
    public TokenException(string message = "Token inválido o expirado.") : base(message) { }
}

public class ForbiddenException : DomainException
{
    public ForbiddenException(string message = "No tiene permisos para esta acción.") : base(message) { }
}

public class BusinessRuleException : DomainException
{
    public string RuleCode { get; }
    
    public BusinessRuleException(string message, string? ruleCode = null) : base(message)
    {
        RuleCode = ruleCode ?? "BUSINESS_RULE";
    }
}

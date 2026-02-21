namespace SolarisPlatform.Application.Common.Models;

/// <summary>
/// Resultado de operación
/// </summary>
public class Result
{
    public bool Succeeded { get; protected set; }
    public string? Error { get; protected set; }
    public List<string> Errors { get; protected set; } = new();

    protected Result(bool succeeded, string? error = null)
    {
        Succeeded = succeeded;
        Error = error;
        if (!string.IsNullOrEmpty(error))
            Errors.Add(error);
    }

    protected Result(bool succeeded, IEnumerable<string> errors)
    {
        Succeeded = succeeded;
        Errors = errors.ToList();
        Error = Errors.FirstOrDefault();
    }

    public static Result Success() => new(true);
    public static Result Failure(string error) => new(false, error);
    public static Result Failure(IEnumerable<string> errors) => new(false, errors);
}

/// <summary>
/// Resultado con datos
/// </summary>
public class Result<T> : Result
{
    public T? Data { get; private set; }

    private Result(bool succeeded, T? data, string? error = null) : base(succeeded, error)
    {
        Data = data;
    }

    private Result(bool succeeded, T? data, IEnumerable<string> errors) : base(succeeded, errors)
    {
        Data = data;
    }

    public static Result<T> Success(T data) => new(true, data);
    public static new Result<T> Failure(string error) => new(false, default, error);
    public static new Result<T> Failure(IEnumerable<string> errors) => new(false, default, errors);
}

/// <summary>
/// Lista paginada
/// </summary>
public class PaginatedList<T>
{
    public List<T> Items { get; }
    public int PaginaActual { get; }
    public int TotalPaginas { get; }
    public int TotalElementos { get; }
    public int ElementosPorPagina { get; }

    public bool TienePaginaAnterior => PaginaActual > 1;
    public bool TienePaginaSiguiente => PaginaActual < TotalPaginas;

    public PaginatedList(List<T> items, int totalElementos, int paginaActual, int elementosPorPagina)
    {
        Items = items;
        TotalElementos = totalElementos;
        PaginaActual = paginaActual;
        ElementosPorPagina = elementosPorPagina;
        TotalPaginas = (int)Math.Ceiling(totalElementos / (double)elementosPorPagina);
    }

    public static PaginatedList<T> Empty(int paginaActual = 1, int elementosPorPagina = 10)
        => new(new List<T>(), 0, paginaActual, elementosPorPagina);
}

/// <summary>
/// Response de API estandarizado
/// </summary>
public class ApiResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public object? Data { get; set; }
    public List<string>? Errors { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static ApiResponse Ok(object? data = null, string? message = null)
        => new() { Success = true, Data = data, Message = message };

    public static ApiResponse Fail(string message, List<string>? errors = null)
        => new() { Success = false, Message = message, Errors = errors };
}

/// <summary>
/// Response de API tipado
/// </summary>
public class ApiResponse<T> : ApiResponse
{
    public new T? Data { get; set; }

    public static ApiResponse<T> Ok(T data, string? message = null)
        => new() { Success = true, Data = data, Message = message };

    public static new ApiResponse<T> Fail(string message, List<string>? errors = null)
        => new() { Success = false, Message = message, Errors = errors };
}

/// <summary>
/// Response paginado de API
/// </summary>
public class PaginatedApiResponse<T>
{
    public bool Success { get; set; } = true;
    public List<T> Items { get; set; } = new();
    public int PaginaActual { get; set; }
    public int TotalPaginas { get; set; }
    public int TotalElementos { get; set; }
    public int ElementosPorPagina { get; set; }
    public bool TienePaginaAnterior { get; set; }
    public bool TienePaginaSiguiente { get; set; }

    public static PaginatedApiResponse<T> FromPaginatedList(PaginatedList<T> list)
        => new()
        {
            Items = list.Items,
            PaginaActual = list.PaginaActual,
            TotalPaginas = list.TotalPaginas,
            TotalElementos = list.TotalElementos,
            ElementosPorPagina = list.ElementosPorPagina,
            TienePaginaAnterior = list.TienePaginaAnterior,
            TienePaginaSiguiente = list.TienePaginaSiguiente
        };
}

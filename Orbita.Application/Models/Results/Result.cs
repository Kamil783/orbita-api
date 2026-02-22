namespace Orbita.Application.Models.Results;

public class Result
{
    public bool IsSuccess { get; }
    public ErrorDetails? Error { get; }

    protected Result(bool isSuccess, ErrorDetails? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Ok() => new(true, null);

    public static Result Fail(string message, ErrorType type = ErrorType.Unexpected, string? code = null)
        => code is null ? new(false, new ErrorDetails(message, type)) : new (false, new ErrorDetails(message, type, Code: code));

    public static Result Validation(IDictionary<string, string[]> errors, string message = "Validation failed")
        => new(false, new ErrorDetails(message, ErrorType.Validation, ValidationErrors: new Dictionary<string, string[]>(errors)));

    public static Result NotFound(string message = "Not found")
        => new(false, new ErrorDetails(message, ErrorType.NotFound));

    public static Result Conflict(string message = "Conflict")
        => new(false, new ErrorDetails(message, ErrorType.Conflict));

    public static Result Unauthorized(string message = "Unauthorized")
        => new(false, new ErrorDetails(message, ErrorType.Unauthorized));

    public static Result Forbidden(string message = "Forbidden")
        => new(false, new ErrorDetails(message, ErrorType.Forbidden));
}

public sealed class Result<T> : Result
{
    public T? Value { get; }

    private Result(bool isSuccess, T? value, ErrorDetails? error)
        : base(isSuccess, error)
    {
        Value = value;
    }

    public static Result<T> Ok(T value) => new(true, value, null);

    public static Result<T> Fail(string message, ErrorType type = ErrorType.Unexpected, string? code = null)
        => code is null ? new(false, default, new ErrorDetails(message, type)) : new (false, default, new ErrorDetails(message, type, Code: code));

    public static Result<T> Validation(IDictionary<string, string[]> errors, string message = "Validation failed")
        => new(false, default, new ErrorDetails(message, ErrorType.Validation, ValidationErrors: new Dictionary<string, string[]>(errors)));

    public static Result<T> NotFound(string message = "Not found")
        => new(false, default, new ErrorDetails(message, ErrorType.NotFound));

    public static Result<T> Conflict(string message = "Conflict")
        => new(false, default, new ErrorDetails(message, ErrorType.Conflict));

    public static Result<T> Unauthorized(string message = "Unauthorized")
    => new(false, default, new ErrorDetails(message, ErrorType.Unauthorized));

    public static Result<T> Forbidden(string message = "Forbidden")
        => new(false, default, new ErrorDetails(message, ErrorType.Forbidden));
}



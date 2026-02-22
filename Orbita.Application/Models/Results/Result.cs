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

    public static Result Fail(string code, string message, ErrorType type = ErrorType.Unexpected)
        => new(false, new ErrorDetails(code, message, type));

    public static Result Validation(IDictionary<string, string[]> errors, string code = "validation_error", string message = "Validation failed")
        => new(false, new ErrorDetails(code, message, ErrorType.Validation, new Dictionary<string, string[]>(errors)));

    public static Result NotFound(string message = "Not found", string code = "not_found")
        => new(false, new ErrorDetails(code, message, ErrorType.NotFound));

    public static Result Conflict(string message = "Conflict", string code = "conflict")
        => new(false, new ErrorDetails(code, message, ErrorType.Conflict));

    public static Result Unauthorized(string message = "Unauthorized", string code = "unauthorized")
        => new(false, new ErrorDetails(code, message, ErrorType.Unauthorized));

    public static Result Forbidden(string message = "Forbidden", string code = "forbidden")
        => new(false, new ErrorDetails(code, message, ErrorType.Forbidden));
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

    public static new Result<T> Fail(string code, string message, ErrorType type = ErrorType.Unexpected)
        => new(false, default, new ErrorDetails(code, message, type));

    public static new Result<T> Validation(IDictionary<string, string[]> errors, string code = "validation_error", string message = "Validation failed")
        => new(false, default, new ErrorDetails(code, message, ErrorType.Validation, new Dictionary<string, string[]>(errors)));

    public static new Result<T> NotFound(string message = "Not found", string code = "not_found")
        => new(false, default, new ErrorDetails(code, message, ErrorType.NotFound));

    public static new Result<T> Conflict(string message = "Conflict", string code = "conflict")
        => new(false, default, new ErrorDetails(code, message, ErrorType.Conflict));
}



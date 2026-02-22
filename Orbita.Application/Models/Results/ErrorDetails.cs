namespace Orbita.Application.Models.Results;

public record ErrorDetails(
    string Message,
    ErrorType Type = ErrorType.Unexpected,
    string? Code = null,
    IReadOnlyDictionary<string, string[]>? ValidationErrors = null
);

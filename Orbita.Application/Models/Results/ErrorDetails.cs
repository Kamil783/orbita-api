namespace Orbita.Application.Models.Results;

public record ErrorDetails(
    string Code,
    string Message,
    ErrorType Type = ErrorType.Unexpected,
    IReadOnlyDictionary<string, string[]>? ValidationErrors = null
);

namespace Orbita.Application.Models.Results;

public static class ResultExtensions
{
    public static Result<TOut> Map<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> mapper)
    {
        if (!result.IsSuccess)
        {
            var error = result.Error!;

            return error.ValidationErrors is not null
                ? Result<TOut>.Validation(
                    new Dictionary<string, string[]>(error.ValidationErrors),
                    error.Message)
                : Result<TOut>.Fail(error.Message, error.Type, error.Code);
        }

        return Result<TOut>.Ok(mapper(result.Value!));
    }
}

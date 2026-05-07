using Microsoft.AspNetCore.Mvc;

namespace CoreService.Common;

public static class ControllerOperationResultExtensions
{
    public static ActionResult<ApiResult<T>> FromOperationResult<T>(
        this ControllerBase controller,
        OperationResult<T> result,
        Func<IReadOnlyDictionary<string, string[]>, IDictionary<string, string[]>>? errorMapper = null)
    {
        if (result.Success && result.Data is not null)
        {
            return controller.Ok(ApiResult<T>.Ok(result.Data));
        }

        var sourceErrors = result.ValidationErrors ?? new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
        var mappedErrors = errorMapper is null
            ? sourceErrors.ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase)
            : errorMapper(sourceErrors);
        return controller.ValidationProblem(new ValidationProblemDetails(mappedErrors));
    }

    public static ActionResult FromOperationResultError<T>(
        this ControllerBase controller,
        OperationResult<T> result,
        Func<IReadOnlyDictionary<string, string[]>, IDictionary<string, string[]>>? errorMapper = null)
    {
        var sourceErrors = result.ValidationErrors ?? new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
        var mappedErrors = errorMapper is null
            ? sourceErrors.ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase)
            : errorMapper(sourceErrors);
        return controller.ValidationProblem(new ValidationProblemDetails(mappedErrors));
    }
}


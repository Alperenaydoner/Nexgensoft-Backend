namespace CoreService.Common;

public sealed record OperationResult<T>(
    bool Success,
    T? Data,
    IReadOnlyDictionary<string, string[]>? ValidationErrors)
{
    public static OperationResult<T> Ok(T data) => new(true, data, null);

    public static OperationResult<T> Validation(IReadOnlyDictionary<string, string[]> errors) =>
        new(false, default, errors);
}


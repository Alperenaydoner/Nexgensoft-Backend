using System.Diagnostics;

namespace CoreService.Common;

public record ApiResult<T>(
    bool Success,
    T? Data,
    string? Message,
    string? TraceId,
    IReadOnlyList<string>? Errors)
{
    public static ApiResult<T> Ok(T data) =>
        new(true, data, null, null, null);

    public static ApiResult<T> Fail(string message, IEnumerable<string>? errors = null) =>
        new(false, default, message, Activity.Current?.Id, errors?.ToArray());
}

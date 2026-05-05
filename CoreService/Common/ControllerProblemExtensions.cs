using Microsoft.AspNetCore.Mvc;

namespace CoreService.Common;

public static class ControllerProblemExtensions
{
    public static ObjectResult ValidationFailed(this ControllerBase controller, string detail, string errorCode) =>
        controller.Problem(
            title: "Validation failed.",
            detail: detail,
            statusCode: StatusCodes.Status400BadRequest,
            extensions: new Dictionary<string, object?> { ["errorCode"] = errorCode });

    public static ObjectResult InternalError(this ControllerBase controller, string title, string errorCode) =>
        controller.Problem(
            title: title,
            statusCode: StatusCodes.Status500InternalServerError,
            extensions: new Dictionary<string, object?> { ["errorCode"] = errorCode });

    public static ObjectResult ProblemFromCatalog(this ControllerBase controller, ProblemDescriptor descriptor, string? detail = null) =>
        controller.Problem(
            title: descriptor.Title,
            detail: detail,
            statusCode: descriptor.StatusCode,
            extensions: new Dictionary<string, object?> { ["errorCode"] = descriptor.ErrorCode });
}

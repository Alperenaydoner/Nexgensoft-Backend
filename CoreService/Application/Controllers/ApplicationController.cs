using CoreService.Application.DTOs.Requests;
using CoreService.Application.Services;
using CoreService.Audit.Attributes;
using CoreService.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreService.Application.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/v1/application")]
[Produces("application/json")]
public class ApplicationController(IApplicationService applicationService) : ControllerBase
{
    [HttpGet("positions")]
    [ProducesResponseType(typeof(ApiResult<IReadOnlyList<string>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResult<IReadOnlyList<string>>>> GetPositionOptionsAsync(CancellationToken cancellationToken)
    {
        var items = await applicationService.GetPositionOptionsAsync(cancellationToken);
        return Ok(ApiResult<IReadOnlyList<string>>.Ok(items));
    }

    [HttpPost]
    [AuditAction("IsBasvurusuGonder", "İş başvurusu gönderildi", "İş başvurusu gönderimi tamamlandı.")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(ApiResult<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    public async Task<ActionResult<ApiResult<Guid>>> SubmitAsync(
        [FromBody] ApplicationSubmitRequest body,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var (applicationId, validationErrors) = await applicationService.SubmitAsync(body, cancellationToken);
        if (validationErrors is not null)
        {
            return ValidationProblem(new ValidationProblemDetails(validationErrors));
        }

        return Ok(ApiResult<Guid>.Ok(applicationId!.Value));
    }
}

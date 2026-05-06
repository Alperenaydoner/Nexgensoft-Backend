using CoreService.Application.DTOs.Requests;
using CoreService.Application.DTOs.Responses;
using CoreService.Application.Services;
using CoreService.Audit.Attributes;
using CoreService.Common;
using CoreService.Common.Localization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreService.Application.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/v1/application")]
[Produces("application/json")]
public class ApplicationController(
    IApplicationService applicationService,
    IApiTextLocalizer textLocalizer) : ControllerBase
{
    [HttpGet("positions")]
    [ProducesResponseType(typeof(ApiResult<IReadOnlyList<string>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResult<IReadOnlyList<string>>>> GetPositionOptionsAsync(CancellationToken cancellationToken)
    {
        var items = await applicationService.GetPositionOptionsAsync(cancellationToken);
        return Ok(ApiResult<IReadOnlyList<string>>.Ok(items));
    }

    [HttpGet("{applicationCode:guid}")]
    [ProducesResponseType(typeof(ApiResult<ApplicationByCodeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResult<ApplicationByCodeResponse>>> GetByCodeAsync(
        [FromRoute] Guid applicationCode,
        CancellationToken cancellationToken)
    {
        var item = await applicationService.GetByCodeAsync(applicationCode, cancellationToken);
        if (item is null)
        {
            return NotFound(new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                ["applicationCode"] = [textLocalizer.Get("Validation.Application.NotFound")],
            }));
        }

        return Ok(ApiResult<ApplicationByCodeResponse>.Ok(item));
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
            return ValidationProblem(new ValidationProblemDetails(LocalizeValidationErrors(validationErrors)));
        }

        return Ok(ApiResult<Guid>.Ok(applicationId!.Value));
    }

    [HttpPut("{applicationCode:guid}")]
    [AuditAction("IsBasvurusuDuzenle", "İş başvurusu düzenlendi", "İş başvurusu başvuru kodu ile güncellendi.")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(ApiResult<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    public async Task<ActionResult<ApiResult<Guid>>> UpdateByCodeAsync(
        [FromRoute] Guid applicationCode,
        [FromBody] ApplicationUpdateByCodeRequest body,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var (applicationId, validationErrors) = await applicationService.UpdateByCodeAsync(applicationCode, body, cancellationToken);
        if (validationErrors is not null)
        {
            return ValidationProblem(new ValidationProblemDetails(LocalizeValidationErrors(validationErrors)));
        }

        return Ok(ApiResult<Guid>.Ok(applicationId!.Value));
    }

    private Dictionary<string, string[]> LocalizeValidationErrors(IDictionary<string, string[]> source)
    {
        return source.ToDictionary(
            x => x.Key,
            x => x.Value.Select(v => v.StartsWith("Validation.", StringComparison.OrdinalIgnoreCase) ? textLocalizer.Get(v) : v).ToArray(),
            StringComparer.OrdinalIgnoreCase);
    }
}

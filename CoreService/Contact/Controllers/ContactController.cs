using CoreService.Audit.Attributes;
using CoreService.Common;
using CoreService.Contact.DTOs.Requests;
using CoreService.Contact.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreService.Contact.Controllers;

/// <summary>İletişim formu gönderimi.</summary>
[ApiController]
[AllowAnonymous]
[Route("api/v1/contact")]
[Produces("application/json")]
public class ContactController(IContactService contactService) : ControllerBase
{
    /// <summary>POST /api/v1/contact — JSON; isteğe bağlı <c>attachments[].base64</c> (çoklu dosya).</summary>
    [HttpPost]
    [AuditAction("IletisimFormuGonder", "İletişim formu gönderildi", "İletişim formu gönderimi tamamlandı.")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(ApiResult<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    public async Task<ActionResult<ApiResult<Guid>>> SubmitAsync(
        [FromBody] ContactSubmitRequest body,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var result = await contactService.SubmitAsync(body, cancellationToken);
        return this.FromOperationResult(result);
    }
}

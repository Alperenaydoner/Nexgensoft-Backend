using CoreService.Common;
using CoreService.Contact.DTOs;
using CoreService.Contact.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoreService.Contact.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ContactController(IContactService contactService) : ControllerBase
{
    /// <summary>POST /api/v1/contact — JSON; isteğe bağlı <c>attachments[].base64</c> (çoklu dosya).</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResult<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResult<Guid>>> Post(
        [FromBody] ContactSubmitRequest body,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var (messageId, validationErrors) = await contactService.SubmitAsync(body, cancellationToken);
        if (validationErrors is not null)
        {
            return ValidationProblem(new ValidationProblemDetails(validationErrors));
        }

        return Ok(ApiResult<Guid>.Ok(messageId!.Value));
    }
}

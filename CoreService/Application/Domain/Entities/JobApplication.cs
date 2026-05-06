using CoreService.Common;

namespace CoreService.Application.Domain.Entities;

public class JobApplication : BaseEntity
{
    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public string Position { get; set; } = string.Empty;

    public string? CoverLetter { get; set; }

    public ICollection<JobApplicationAttachment> Attachments { get; set; } = new List<JobApplicationAttachment>();
}

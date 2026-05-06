using CoreService.Common;

namespace CoreService.Application.Domain.Entities;

public class JobApplicationAttachment : BaseEntity
{
    public Guid JobApplicationId { get; set; }

    public string OriginalFileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public long SizeBytes { get; set; }

    public string ContentBase64 { get; set; } = string.Empty;

    public JobApplication? JobApplication { get; set; }
}

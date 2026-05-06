using CoreService.Common;

namespace CoreService.Application.Domain.Entities;

public class JobPosition : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public int SortOrder { get; set; }
}

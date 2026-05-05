namespace CoreService.Application.Domain.Entities;

public class JobPosition
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public int SortOrder { get; set; }
}

namespace CoreService.Application.DTOs;

public class ApplicationUploadOptions
{
    public const string SectionName = "Application:Uploads";

    public int MaxFilesPerApplication { get; set; } = 10;

    /// <summary>Yeni başvuruda zorunlu minimum ek dosya sayısı.</summary>
    public int MinFilesPerApplication { get; set; } = 3;

    public long MaxBytesPerFile { get; set; } = 50 * 1024 * 1024;

    public List<string> AllowedExtensions { get; set; } =
    [
        ".pdf", ".png", ".jpg", ".jpeg", ".webp", ".gif",
        ".doc", ".docx", ".xls", ".xlsx", ".txt", ".zip",
        ".mp4", ".webm", ".mov", ".m4v", ".mpeg", ".mpg",
    ];
}

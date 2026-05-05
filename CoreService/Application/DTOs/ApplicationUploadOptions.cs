namespace CoreService.Application.DTOs;

public class ApplicationUploadOptions
{
    public const string SectionName = "Application:Uploads";

    public int MaxFilesPerApplication { get; set; } = 10;

    public long MaxBytesPerFile { get; set; } = 10 * 1024 * 1024;

    public List<string> AllowedExtensions { get; set; } =
    [
        ".pdf", ".png", ".jpg", ".jpeg", ".webp", ".gif",
        ".doc", ".docx", ".xls", ".xlsx", ".txt", ".zip",
    ];
}

namespace CoreService.Contact.DTOs;

public class ContactUploadOptions
{
    public const string SectionName = "Contact:Uploads";

    public int MaxFilesPerMessage { get; set; } = 10;

    public long MaxBytesPerFile { get; set; } = 10 * 1024 * 1024;

    public List<string> AllowedExtensions { get; set; } =
    [
        ".pdf", ".png", ".jpg", ".jpeg", ".webp", ".gif",
        ".doc", ".docx", ".xls", ".xlsx", ".txt", ".zip",
    ];
}

namespace CoreService.Admin.DTOs;

public sealed record AdminAttachmentFileDto(
    byte[] Bytes,
    string ContentType,
    string DownloadName);


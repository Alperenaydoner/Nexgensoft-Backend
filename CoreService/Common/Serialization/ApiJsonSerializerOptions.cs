using System.Text.Json;

namespace CoreService.Common.Serialization;

/// <summary>API ve site JSON payloadları için ortak serializer ayarları (camelCase).</summary>
public static class ApiJsonSerializerOptions
{
    public static JsonSerializerOptions CamelCase { get; } = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = false,
    };
}

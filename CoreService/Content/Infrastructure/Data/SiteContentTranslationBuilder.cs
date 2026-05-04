using System.Text.Json;
using System.Text.Json.Nodes;

namespace CoreService.Content.Infrastructure.Data;

/// <summary>Düz anahtar sözlüğünü i18next uyumlu iç içe JSON nesnesine çevirir (API cevabı).</summary>
public static class SiteContentTranslationBuilder
{
    public static JsonElement ToJsonElement(IReadOnlyDictionary<string, string> flat)
    {
        var root = new JsonObject();
        foreach (var (path, value) in flat)
        {
            var parts = path.Split('.', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
            {
                continue;
            }

            Set(root, parts, value);
        }

        return JsonSerializer.SerializeToElement(root, (JsonSerializerOptions?)null);
    }

    private static void Set(JsonNode target, ReadOnlySpan<string> parts, string value)
    {
        if (parts.Length == 0)
        {
            return;
        }

        var key = parts[0];
        if (parts.Length == 1)
        {
            if (target is JsonObject o)
            {
                o[key] = value;
            }

            return;
        }

        if (target is JsonObject obj)
        {
            if (!obj.TryGetPropertyValue(key, out var child) || child is null)
            {
                var nextIsIndex = int.TryParse(parts[1], out _);
                child = nextIsIndex ? new JsonArray() : new JsonObject();
                obj[key] = child;
            }

            Set(child!, parts[1..], value);
            return;
        }

        if (target is JsonArray arr && int.TryParse(key, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var idx))
        {
            while (arr.Count <= idx)
            {
                arr.Add(new JsonObject());
            }

            Set(arr[idx]!, parts[1..], value);
        }
    }
}

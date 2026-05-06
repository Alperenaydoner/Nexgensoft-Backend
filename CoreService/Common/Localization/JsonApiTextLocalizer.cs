using System.Globalization;
using System.Reflection;
using System.Text.Json;

namespace CoreService.Common.Localization;

public class JsonApiTextLocalizer : IApiTextLocalizer
{
    private readonly Lazy<IReadOnlyDictionary<string, string>> _tr;
    private readonly Lazy<IReadOnlyDictionary<string, string>> _en;

    public JsonApiTextLocalizer()
    {
        _tr = new(() => LoadLocale("tr"));
        _en = new(() => LoadLocale("en"));
    }

    public string Get(string key, string? fallback = null)
    {
        var locale = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        var active = locale == "en" ? _en.Value : _tr.Value;
        if (active.TryGetValue(key, out var value))
        {
            return value;
        }

        if (_en.Value.TryGetValue(key, out var englishValue))
        {
            return englishValue;
        }

        return fallback ?? key;
    }

    private static IReadOnlyDictionary<string, string> LoadLocale(string locale)
    {
        var asm = Assembly.GetExecutingAssembly();
        var resourceSuffix = $".Content.Infrastructure.Data.SeedLocales.{locale}.json";
        var resourceName = asm.GetManifestResourceNames()
            .FirstOrDefault(x => x.EndsWith(resourceSuffix, StringComparison.OrdinalIgnoreCase));

        if (resourceName is null)
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        using var stream = asm.GetManifestResourceStream(resourceName);
        if (stream is null)
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        using var doc = JsonDocument.Parse(stream);
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        Flatten(doc.RootElement, null, result);
        return result;
    }

    private static void Flatten(JsonElement element, string? prefix, Dictionary<string, string> target)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            foreach (var prop in element.EnumerateObject())
            {
                var next = string.IsNullOrWhiteSpace(prefix) ? prop.Name : $"{prefix}.{prop.Name}";
                Flatten(prop.Value, next, target);
            }

            return;
        }

        if (element.ValueKind == JsonValueKind.String && !string.IsNullOrWhiteSpace(prefix))
        {
            target[prefix] = element.GetString() ?? string.Empty;
        }
    }
}

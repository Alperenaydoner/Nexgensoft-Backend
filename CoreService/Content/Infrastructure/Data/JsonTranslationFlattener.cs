using System.Text.Json.Nodes;

namespace CoreService.Content.Infrastructure.Data;

/// <summary>tr.json / en.json ağacını nokta anahtarlı düz listeye çevirir (seed için).</summary>
public static class JsonTranslationFlattener
{
    public static IReadOnlyList<(string Key, string Value)> Flatten(string json)
    {
        var root = JsonNode.Parse(json);
        if (root is not JsonObject obj)
        {
            return [];
        }

        var list = new List<(string, string)>();
        Walk(obj, string.Empty, list);
        return list;
    }

    private static void Walk(JsonNode? node, string prefix, List<(string, string)> list)
    {
        switch (node)
        {
            case JsonObject o:
                foreach (var prop in o)
                {
                    var path = string.IsNullOrEmpty(prefix) ? prop.Key : $"{prefix}.{prop.Key}";
                    Walk(prop.Value, path, list);
                }

                break;

            case JsonArray arr:
                for (var i = 0; i < arr.Count; i++)
                {
                    var path = $"{prefix}.{i}";
                    Walk(arr[i], path, list);
                }

                break;

            case JsonValue val:
                if (string.IsNullOrEmpty(prefix))
                {
                    return;
                }

                if (val.TryGetValue<string>(out var s))
                {
                    list.Add((prefix, s));
                }
                else
                {
                    list.Add((prefix, val.ToJsonString().Trim('"')));
                }

                break;
        }
    }
}

using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CoreService.Audit.Attributes;

/// <summary>Mapa <c>AuditActionAttribute</c> ile uyumlu <c>{locale}</c>, <c>{id}</c>, <c>{KullaniciEposta}</c> yer tutucuları.</summary>
public static partial class AuditTemplateHelper
{
    public static string? Render(string? template, HttpContext http, ActionExecutingContext ctx, ClaimsPrincipal user)
    {
        if (string.IsNullOrWhiteSpace(template))
        {
            return template;
        }

        var map = BuildMap(http, ctx, user);
        return PlaceholderRegex().Replace(template, m =>
        {
            var raw = m.Groups[1].Value;
            if (map.TryGetValue(raw, out var v) && !string.IsNullOrEmpty(v))
            {
                return v;
            }

            var kvp = map.FirstOrDefault(k => string.Equals(k.Key, raw, StringComparison.OrdinalIgnoreCase));
            return !string.IsNullOrEmpty(kvp.Value) ? kvp.Value : m.Value;
        });
    }

    private static Dictionary<string, string> BuildMap(HttpContext http, ActionExecutingContext ctx, ClaimsPrincipal user)
    {
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        void add(string key, string? value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            map[key] = value;
        }

        var uid = First(user, ClaimTypes.NameIdentifier, "sub");
        var email = First(user, ClaimTypes.Email, "email");
        add("KullaniciId", uid);
        add("KullaniciEposta", email);
        add("UserId", uid);
        add("UserEmail", email);

        foreach (var kv in ctx.RouteData.Values)
        {
            add(kv.Key, kv.Value?.ToString());
        }

        foreach (var kv in http.Request.Query)
        {
            add(kv.Key, kv.Value.FirstOrDefault());
        }

        foreach (var kv in ctx.ActionArguments)
        {
            add(kv.Key, ToPlaceholderValue(kv.Key, kv.Value));
        }

        return map;
    }

    private static string? First(ClaimsPrincipal user, params string[] types)
    {
        foreach (var t in types)
        {
            var v = user.FindFirst(t)?.Value;
            if (!string.IsNullOrWhiteSpace(v))
            {
                return v.Trim();
            }
        }

        return null;
    }

    private static string? ToPlaceholderValue(string key, object? value)
    {
        if (value is null)
        {
            return null;
        }

        if (IsSensitiveKey(key))
        {
            return "***";
        }

        return value switch
        {
            string s => s,
            Guid g => g.ToString("D"),
            int or long or short or byte or bool or decimal or double or float => value.ToString(),
            _ => null,
        };
    }

    private static bool IsSensitiveKey(string key)
    {
        var k = key.ToLowerInvariant();
        return k.Contains("password", StringComparison.Ordinal) || k.Contains("token", StringComparison.Ordinal)
               || k.Contains("secret", StringComparison.Ordinal) || k.Contains("base64", StringComparison.Ordinal);
    }

    [GeneratedRegex(@"\{([A-Za-z0-9_]+)\}", RegexOptions.CultureInvariant)]
    private static partial Regex PlaceholderRegex();
}

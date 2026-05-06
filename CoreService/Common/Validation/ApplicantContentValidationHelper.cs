namespace CoreService.Common.Validation;

public static class ApplicantContentValidationHelper
{
    public static Dictionary<string, string[]>? ValidateFullNameAndEmail(string fullName, string email)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
        var normalizedName = fullName.Trim();
        var nameTokens = normalizedName.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (nameTokens.Length < 2 || nameTokens.Any(x => x.Length < 2))
        {
            errors["fullName"] = ["Validation.Application.FullNameRequired"];
        }

        var lowerName = normalizedName.ToLowerInvariant();
        if (ContainsNoiseKeyword(lowerName))
        {
            errors["fullName"] = ["Validation.Application.FullNameMeaningful"];
        }

        var localPart = email.Split('@', 2, StringSplitOptions.TrimEntries)[0].ToLowerInvariant();
        if (ContainsNoiseKeyword(localPart))
        {
            errors["email"] = ["Validation.Application.EmailMeaningful"];
        }

        return errors.Count > 0 ? errors : null;
    }

    private static bool ContainsNoiseKeyword(string value)
    {
        if (value.Length < 3)
        {
            return true;
        }

        string[] blocked =
        [
            "asd",
            "qwe",
            "zxc",
            "test",
            "deneme",
            "dummy",
            "adsad",
            "sadasd",
            "example",
            "abc",
            "1234",
        ];

        return blocked.Any(value.Contains);
    }
}

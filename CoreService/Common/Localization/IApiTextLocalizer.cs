namespace CoreService.Common.Localization;

public interface IApiTextLocalizer
{
    string Get(string key, string? fallback = null);
}

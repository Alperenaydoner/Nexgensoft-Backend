namespace CoreService.Common;

public sealed record ProblemDescriptor(string Title, string ErrorCode, int StatusCode);

public static class ProblemCatalog
{
    public static class Auth
    {
        public static readonly ProblemDescriptor InvalidCredentials =
            new("Authentication failed.", "Auth.InvalidCredentials", StatusCodes.Status401Unauthorized);
    }

    public static class Admin
    {
        public static readonly ProblemDescriptor UsersCreateValidation =
            new("Validation failed.", "Admin.Users.CreateValidation", StatusCodes.Status400BadRequest);

        public static readonly ProblemDescriptor UsersUpdateValidation =
            new("Validation failed.", "Admin.Users.UpdateValidation", StatusCodes.Status400BadRequest);

        public static readonly ProblemDescriptor RolesCreateValidation =
            new("Validation failed.", "Admin.Roles.CreateValidation", StatusCodes.Status400BadRequest);

        public static readonly ProblemDescriptor RolesUpdateValidation =
            new("Validation failed.", "Admin.Roles.UpdateValidation", StatusCodes.Status400BadRequest);

        public static readonly ProblemDescriptor ContentSaveValidation =
            new("Validation failed.", "Admin.Content.SaveValidation", StatusCodes.Status400BadRequest);
    }

    public static class Content
    {
        public static readonly ProblemDescriptor SiteReadFailed =
            new("Content read failed.", "Content.SiteReadFailed", StatusCodes.Status500InternalServerError);
    }
}

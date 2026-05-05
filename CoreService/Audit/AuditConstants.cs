namespace CoreService.Audit;

public static class AuditConstants
{
    /// <summary><see cref="Attributes.AuditActionAttribute"/> kayıt yazdıysa middleware tekrar yazmasın.</summary>
    public const string AuditActionLoggedKey = "AuditActionLogged";
}

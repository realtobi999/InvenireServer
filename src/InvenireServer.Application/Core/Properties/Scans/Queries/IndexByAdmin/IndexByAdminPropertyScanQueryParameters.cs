namespace InvenireServer.Application.Core.Properties.Scans.Queries.IndexByAdmin;

/// <summary>
/// Represents parameters for indexing property scans for an admin.
/// </summary>
public record IndexByAdminPropertyScanQueryParameters
{
    public int Limit { get; set; }
    public int Offset { get; set; }

    public bool? Desc { get; set; }
    public string? Order { get; set; }

    public bool? Active { get; set; }
}
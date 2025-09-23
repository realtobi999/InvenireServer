namespace InvenireServer.Application.Core.Properties.Scans.Queries.IndexByAdmin;

public record IndexByAdminPropertyScanQueryParameters
{
    public int Limit { get; set; }
    public int Offset { get; set; }

    public bool? Active { get; set; }
}
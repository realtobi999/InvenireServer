namespace InvenireServer.Application.Core.Properties.Items.Queries.IndexByScan;

/// <summary>
/// Represents parameters for indexing property items for a scan.
/// </summary>
public record IndexByScanPropertyItemQueryParameters
{
    public int Limit { get; set; }
    public int Offset { get; set; }

    public bool? Desc { get; set; }
    public string? Order { get; set; }

    public int? MinPrice { get; set; }
    public int? MaxPrice { get; set; }

    public DateTimeOffset? DateOfPurchaseFrom { get; set; }
    public DateTimeOffset? DateOfPurchaseTo { get; set; }

    public DateTimeOffset? CreatedAtFrom { get; set; }
    public DateTimeOffset? CreatedAtTo { get; set; }

    public Guid? EmployeeId { get; set; }

    public string? Room { get; set; }
    public string? Building { get; set; }

    public bool? WithGeneratedCode { get; set; }

    public bool? Scanned { get; set; }

    public string? SearchQuery { get; set; }
}


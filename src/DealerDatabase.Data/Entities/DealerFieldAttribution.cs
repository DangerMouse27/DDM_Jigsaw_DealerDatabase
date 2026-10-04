namespace DealerDatabase.Data.Entities;

/// <summary>
/// Records which source supplied a specific field value on a consolidated dealer.
/// </summary>
public class DealerFieldAttribution
{
    public int Id { get; set; }

    public int DealerId { get; set; }

    public Dealer Dealer { get; set; } = null!;

    public string FieldName { get; set; } = string.Empty;

    public string SourceName { get; set; } = string.Empty;

    public string SourceRecordKey { get; set; } = string.Empty;

    public string? ValuePreview { get; set; }
}

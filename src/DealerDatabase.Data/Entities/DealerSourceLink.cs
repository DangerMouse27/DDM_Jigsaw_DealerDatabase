namespace DealerDatabase.Data.Entities;

/// <summary>
/// Links a consolidated dealer to a specific source-file record used during matching.
/// </summary>
public class DealerSourceLink
{
    public int Id { get; set; }

    public int DealerId { get; set; }

    public Dealer Dealer { get; set; } = null!;

    public string SourceName { get; set; } = string.Empty;

    public string SourceRecordKey { get; set; } = string.Empty;
}

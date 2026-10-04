namespace DealerDatabase.Data.Entities;

/// <summary>
/// A consolidated dealership record assembled from multiple source files.
/// </summary>
public class Dealer
{
    public int Id { get; set; }

    /// <summary>Best display name (trading name preferred, else legal name).</summary>
    public string Name { get; set; } = string.Empty;

    public string? LegalName { get; set; }
    public string? TradingName { get; set; }

    public string? CompanyNumber { get; set; }
    public DateOnly? IncorporationDate { get; set; }
    public string? CompanyStatus { get; set; }

    public string? VatNumber { get; set; }
    /// <summary>Valid, NotFound, or Unknown when only inferred from another source.</summary>
    public string? VatValidationStatus { get; set; }

    public string? FcaFrn { get; set; }
    public string? FcaStatus { get; set; }
    public string? FcaBusinessType { get; set; }
    public string? FcaPermissions { get; set; }

    public string? IcoRegistrationNumber { get; set; }
    public DateOnly? IcoRegistrationStart { get; set; }
    public DateOnly? IcoExpiry { get; set; }
    public string? IcoPaymentTier { get; set; }

    public string? SafMemberId { get; set; }
    public string? SafStatus { get; set; }
    public DateOnly? SafExpiry { get; set; }

    public string? MarketcheckDealerId { get; set; }

    public string? SellerType { get; set; }
    public string? FranchiseMake { get; set; }
    public string? SicCodes { get; set; }
    public string? Directors { get; set; }

    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? Town { get; set; }
    public string? County { get; set; }
    public string? Postcode { get; set; }
    public string? Country { get; set; }

    public string? RegisteredAddressLine1 { get; set; }
    public string? RegisteredAddressLine2 { get; set; }
    public string? RegisteredTown { get; set; }
    public string? RegisteredCounty { get; set; }
    public string? RegisteredPostcode { get; set; }
    public string? RegisteredCountry { get; set; }

    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? WebsiteDomain { get; set; }

    public int? InventoryCount { get; set; }
    public decimal? AvgListedPrice { get; set; }
    public decimal? AvgSoldPrice { get; set; }
    public int? AvgDaysInStock { get; set; }
    public int? SoldLast30Days { get; set; }
    public string? VehicleTypes { get; set; }
    public string? StockFeedProvider { get; set; }
    public DateOnly? MarketcheckLastSeen { get; set; }

    public decimal? GoogleRating { get; set; }
    public int? GoogleReviewCount { get; set; }
    public decimal? TrustpilotScore { get; set; }
    public string? WebsitePlatform { get; set; }
    public bool? OffersFinance { get; set; }
    public string? FinanceCalculator { get; set; }
    public string? FinanceLenders { get; set; }
    public string? RepresentativeApr { get; set; }

    public DateTimeOffset ImportedAt { get; set; }

    /// <summary>Comma-separated source names that contributed to this dealer.</summary>
    public string Sources { get; set; } = string.Empty;

    /// <summary>
    /// Originating source keys for this consolidated dealer (e.g. Marketcheck:MC123|FCA:913639).
    /// </summary>
    public string LineageTag { get; set; } = string.Empty;

    public ICollection<DealerSourceLink> SourceLinks { get; set; } = new List<DealerSourceLink>();

    public ICollection<DealerFieldAttribution> FieldAttributions { get; set; } = new List<DealerFieldAttribution>();
}

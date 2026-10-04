namespace DealerDatabase.Import.Models;

/// <summary>
/// Normalised intermediate record produced by a source reader before matching.
/// </summary>
public sealed class SourceRecord
{
    public required string SourceName { get; init; }

    public required string SourceRecordKey { get; init; }

    public string? LegalName { get; set; }
    public string? TradingName { get; set; }
    public string? DisplayName { get; set; }

    public string? CompanyNumber { get; set; }
    public DateOnly? IncorporationDate { get; set; }
    public string? CompanyStatus { get; set; }

    public string? VatNumber { get; set; }
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

    public string? NameKey { get; set; }

    public string LineageKey => $"{SourceName}:{SourceRecordKey}";

    public void ApplyDerivedKeys()
    {
        NameKey = Normalization.Normalizer.NameKey(DisplayName)
            ?? Normalization.Normalizer.NameKey(TradingName)
            ?? Normalization.Normalizer.NameKey(LegalName);

        WebsiteDomain ??= Normalization.Normalizer.NormalizeDomain(Website);
    }
}

public static class SourceNames
{
    public const string Marketcheck = "Marketcheck";
    public const string Ico = "ICO";
    public const string Crawled = "Crawled";
    public const string Saf = "SAF";
    public const string Fca = "FCA";
    public const string CompaniesHouse = "CompaniesHouse";
    public const string Vat = "VAT";
}

public static class VatStatuses
{
    public const string Valid = "Valid";
    public const string NotFound = "NotFound";
    public const string Unknown = "Unknown";
}

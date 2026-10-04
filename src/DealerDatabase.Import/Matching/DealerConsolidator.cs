using DealerDatabase.Data.Entities;
using DealerDatabase.Import.Models;
using DealerDatabase.Import.Normalization;

namespace DealerDatabase.Import.Matching;

/// <summary>
/// Resolves a matched cluster of source records into a single canonical <see cref="Dealer"/>.
/// When sources disagree, higher-trust sources win for that field, and provenance is recorded.
/// </summary>
public static class DealerConsolidator
{
    private static readonly string[] LegalIdentityPriority =
    [
        SourceNames.CompaniesHouse,
        SourceNames.Fca,
        SourceNames.Vat,
        SourceNames.Ico,
        SourceNames.Crawled,
        SourceNames.Saf,
        SourceNames.Marketcheck
    ];

    private static readonly string[] TradingPriority =
    [
        SourceNames.Marketcheck,
        SourceNames.Crawled,
        SourceNames.Saf,
        SourceNames.Fca,
        SourceNames.Ico,
        SourceNames.Vat,
        SourceNames.CompaniesHouse
    ];

    private static readonly string[] VatPriority =
    [
        SourceNames.Vat,
        SourceNames.Crawled,
        SourceNames.Marketcheck,
        SourceNames.Fca,
        SourceNames.CompaniesHouse,
        SourceNames.Ico,
        SourceNames.Saf
    ];

    private static readonly string[] FcaPriority =
    [
        SourceNames.Fca,
        SourceNames.Crawled,
        SourceNames.Marketcheck,
        SourceNames.CompaniesHouse,
        SourceNames.Ico,
        SourceNames.Saf,
        SourceNames.Vat
    ];

    private static readonly string[] RegisteredAddressPriority =
    [
        SourceNames.CompaniesHouse,
        SourceNames.Ico,
        SourceNames.Fca,
        SourceNames.Crawled,
        SourceNames.Vat,
        SourceNames.Marketcheck,
        SourceNames.Saf
    ];

    public static Dealer Consolidate(IReadOnlyList<SourceRecord> cluster, DateTimeOffset importedAt)
    {
        var attributions = new List<DealerFieldAttribution>();

        var legalName = Pick(cluster, r => r.LegalName, LegalIdentityPriority, "LegalName", attributions);
        var tradingName = Pick(cluster, r => r.TradingName, TradingPriority, "TradingName", attributions);
        string display;
        if (tradingName is not null)
        {
            display = tradingName;
            CopyAttribution(attributions, "TradingName", "Name");
        }
        else if (legalName is not null)
        {
            display = legalName;
            CopyAttribution(attributions, "LegalName", "Name");
        }
        else
        {
            display = Pick(cluster, r => r.DisplayName, TradingPriority, "Name", attributions) ?? "Unknown dealer";
        }

        var dealer = new Dealer
        {
            Name = display,
            LegalName = legalName,
            TradingName = tradingName,
            CompanyNumber = Pick(cluster, r => r.CompanyNumber, LegalIdentityPriority, "CompanyNumber", attributions),
            IncorporationDate = Pick(cluster, r => r.IncorporationDate, [SourceNames.CompaniesHouse], "IncorporationDate", attributions),
            CompanyStatus = Pick(cluster, r => r.CompanyStatus, LegalIdentityPriority, "CompanyStatus", attributions),
            VatNumber = Pick(cluster, r => r.VatNumber, VatPriority, "VatNumber", attributions),
            VatValidationStatus = ResolveVatStatus(cluster, attributions),
            FcaFrn = Pick(cluster, r => r.FcaFrn, FcaPriority, "FcaFrn", attributions),
            FcaStatus = Pick(cluster, r => r.FcaStatus, FcaPriority, "FcaStatus", attributions),
            FcaBusinessType = Pick(cluster, r => r.FcaBusinessType, FcaPriority, "FcaBusinessType", attributions),
            FcaPermissions = Pick(cluster, r => r.FcaPermissions, FcaPriority, "FcaPermissions", attributions),
            IcoRegistrationNumber = Pick(cluster, r => r.IcoRegistrationNumber, [SourceNames.Ico], "IcoRegistrationNumber", attributions),
            IcoRegistrationStart = Pick(cluster, r => r.IcoRegistrationStart, [SourceNames.Ico], "IcoRegistrationStart", attributions),
            IcoExpiry = Pick(cluster, r => r.IcoExpiry, [SourceNames.Ico], "IcoExpiry", attributions),
            IcoPaymentTier = Pick(cluster, r => r.IcoPaymentTier, [SourceNames.Ico], "IcoPaymentTier", attributions),
            SafMemberId = Pick(cluster, r => r.SafMemberId, [SourceNames.Saf], "SafMemberId", attributions),
            SafStatus = Pick(cluster, r => r.SafStatus, [SourceNames.Saf], "SafStatus", attributions),
            SafExpiry = Pick(cluster, r => r.SafExpiry, [SourceNames.Saf], "SafExpiry", attributions),
            MarketcheckDealerId = Pick(cluster, r => r.MarketcheckDealerId, [SourceNames.Marketcheck], "MarketcheckDealerId", attributions),
            SellerType = Pick(cluster, r => r.SellerType, TradingPriority, "SellerType", attributions),
            FranchiseMake = Pick(cluster, r => r.FranchiseMake, TradingPriority, "FranchiseMake", attributions),
            SicCodes = Pick(cluster, r => r.SicCodes, [SourceNames.CompaniesHouse], "SicCodes", attributions),
            Directors = Pick(cluster, r => r.Directors, [SourceNames.CompaniesHouse], "Directors", attributions),
            AddressLine1 = Pick(cluster, r => r.AddressLine1, TradingPriority, "AddressLine1", attributions),
            AddressLine2 = Pick(cluster, r => r.AddressLine2, TradingPriority, "AddressLine2", attributions),
            Town = Pick(cluster, r => r.Town, TradingPriority, "Town", attributions),
            County = Pick(cluster, r => r.County, TradingPriority, "County", attributions),
            Postcode = Pick(cluster, r => r.Postcode, TradingPriority, "Postcode", attributions),
            Country = Pick(cluster, r => r.Country, TradingPriority, "Country", attributions),
            RegisteredAddressLine1 = Pick(cluster, r => r.RegisteredAddressLine1, RegisteredAddressPriority, "RegisteredAddressLine1", attributions),
            RegisteredAddressLine2 = Pick(cluster, r => r.RegisteredAddressLine2, RegisteredAddressPriority, "RegisteredAddressLine2", attributions),
            RegisteredTown = Pick(cluster, r => r.RegisteredTown, RegisteredAddressPriority, "RegisteredTown", attributions),
            RegisteredCounty = Pick(cluster, r => r.RegisteredCounty, RegisteredAddressPriority, "RegisteredCounty", attributions),
            RegisteredPostcode = Pick(cluster, r => r.RegisteredPostcode, RegisteredAddressPriority, "RegisteredPostcode", attributions),
            RegisteredCountry = Pick(cluster, r => r.RegisteredCountry, RegisteredAddressPriority, "RegisteredCountry", attributions),
            Phone = Pick(cluster, r => r.Phone, TradingPriority, "Phone", attributions),
            Email = Pick(cluster, r => r.Email, TradingPriority, "Email", attributions),
            Website = Pick(cluster, r => r.Website, TradingPriority, "Website", attributions),
            WebsiteDomain = Pick(cluster, r => r.WebsiteDomain, TradingPriority, "WebsiteDomain", attributions),
            InventoryCount = Pick(cluster, r => r.InventoryCount, [SourceNames.Marketcheck, SourceNames.Crawled], "InventoryCount", attributions),
            AvgListedPrice = Pick(cluster, r => r.AvgListedPrice, [SourceNames.Marketcheck], "AvgListedPrice", attributions),
            AvgSoldPrice = Pick(cluster, r => r.AvgSoldPrice, [SourceNames.Marketcheck], "AvgSoldPrice", attributions),
            AvgDaysInStock = Pick(cluster, r => r.AvgDaysInStock, [SourceNames.Marketcheck], "AvgDaysInStock", attributions),
            SoldLast30Days = Pick(cluster, r => r.SoldLast30Days, [SourceNames.Marketcheck], "SoldLast30Days", attributions),
            VehicleTypes = Pick(cluster, r => r.VehicleTypes, [SourceNames.Marketcheck], "VehicleTypes", attributions),
            StockFeedProvider = Pick(cluster, r => r.StockFeedProvider, [SourceNames.Marketcheck], "StockFeedProvider", attributions),
            MarketcheckLastSeen = Pick(cluster, r => r.MarketcheckLastSeen, [SourceNames.Marketcheck], "MarketcheckLastSeen", attributions),
            GoogleRating = PickMax(cluster, r => r.GoogleRating, "GoogleRating", attributions),
            GoogleReviewCount = PickMax(cluster, r => r.GoogleReviewCount, "GoogleReviewCount", attributions),
            TrustpilotScore = PickMax(cluster, r => r.TrustpilotScore, "TrustpilotScore", attributions),
            WebsitePlatform = Pick(cluster, r => r.WebsitePlatform, [SourceNames.Crawled], "WebsitePlatform", attributions),
            OffersFinance = Pick(cluster, r => r.OffersFinance, [SourceNames.Crawled, SourceNames.Fca], "OffersFinance", attributions),
            FinanceCalculator = Pick(cluster, r => r.FinanceCalculator, [SourceNames.Crawled], "FinanceCalculator", attributions),
            FinanceLenders = Pick(cluster, r => r.FinanceLenders, [SourceNames.Crawled], "FinanceLenders", attributions),
            RepresentativeApr = Pick(cluster, r => r.RepresentativeApr, [SourceNames.Crawled], "RepresentativeApr", attributions),
            ImportedAt = importedAt,
            Sources = string.Join(", ", cluster.Select(r => r.SourceName).Distinct().OrderBy(s => s)),
            LineageTag = string.Join("|", cluster
                .Select(r => r.LineageKey)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(k => k, StringComparer.OrdinalIgnoreCase))
        };

        dealer.Postcode ??= dealer.RegisteredPostcode;
        dealer.Town ??= dealer.RegisteredTown;
        dealer.WebsiteDomain ??= Normalizer.NormalizeDomain(dealer.Website);

        if (dealer.VatNumber is not null && dealer.VatValidationStatus is null)
        {
            dealer.VatValidationStatus = VatStatuses.Unknown;
            attributions.Add(new DealerFieldAttribution
            {
                FieldName = "VatValidationStatus",
                SourceName = "Derived",
                SourceRecordKey = dealer.VatNumber,
                ValuePreview = VatStatuses.Unknown
            });
        }

        foreach (var attribution in attributions
                     .GroupBy(a => a.FieldName)
                     .Select(g => g.First()))
        {
            dealer.FieldAttributions.Add(attribution);
        }

        foreach (var source in cluster
                     .GroupBy(r => (r.SourceName, r.SourceRecordKey))
                     .Select(g => g.First()))
        {
            dealer.SourceLinks.Add(new DealerSourceLink
            {
                SourceName = source.SourceName,
                SourceRecordKey = source.SourceRecordKey
            });
        }

        return dealer;
    }

    private static void CopyAttribution(
        List<DealerFieldAttribution> attributions,
        string fromField,
        string toField)
    {
        var source = attributions.LastOrDefault(a => a.FieldName == fromField);
        if (source is null)
        {
            return;
        }

        attributions.Add(new DealerFieldAttribution
        {
            FieldName = toField,
            SourceName = source.SourceName,
            SourceRecordKey = source.SourceRecordKey,
            ValuePreview = source.ValuePreview
        });
    }

    private static string? ResolveVatStatus(
        IReadOnlyList<SourceRecord> cluster,
        List<DealerFieldAttribution> attributions)
    {
        var vatRecord = cluster
            .Where(r => r.SourceName == SourceNames.Vat && r.VatValidationStatus is not null)
            .OrderBy(r => r.VatValidationStatus == VatStatuses.Valid ? 0 : 1)
            .FirstOrDefault();

        if (vatRecord?.VatValidationStatus is not null)
        {
            attributions.Add(new DealerFieldAttribution
            {
                FieldName = "VatValidationStatus",
                SourceName = vatRecord.SourceName,
                SourceRecordKey = vatRecord.SourceRecordKey,
                ValuePreview = vatRecord.VatValidationStatus
            });
            return vatRecord.VatValidationStatus;
        }

        return null;
    }

    private static T? Pick<T>(
        IReadOnlyList<SourceRecord> cluster,
        Func<SourceRecord, T?> selector,
        IReadOnlyList<string> priority,
        string fieldName,
        List<DealerFieldAttribution> attributions)
    {
        foreach (var sourceName in priority)
        {
            foreach (var record in cluster.Where(r => r.SourceName == sourceName))
            {
                var value = selector(record);
                if (!HasValue(value))
                {
                    continue;
                }

                attributions.Add(new DealerFieldAttribution
                {
                    FieldName = fieldName,
                    SourceName = record.SourceName,
                    SourceRecordKey = record.SourceRecordKey,
                    ValuePreview = Truncate(Format(value))
                });
                return value;
            }
        }

        foreach (var record in cluster)
        {
            var value = selector(record);
            if (!HasValue(value))
            {
                continue;
            }

            attributions.Add(new DealerFieldAttribution
            {
                FieldName = fieldName,
                SourceName = record.SourceName,
                SourceRecordKey = record.SourceRecordKey,
                ValuePreview = Truncate(Format(value))
            });
            return value;
        }

        return default;
    }

    private static T? PickMax<T>(
        IReadOnlyList<SourceRecord> cluster,
        Func<SourceRecord, T?> selector,
        string fieldName,
        List<DealerFieldAttribution> attributions)
        where T : struct, IComparable<T>
    {
        T? best = null;
        SourceRecord? bestRecord = null;
        foreach (var record in cluster)
        {
            var value = selector(record);
            if (value is null)
            {
                continue;
            }

            if (best is null || value.Value.CompareTo(best.Value) > 0)
            {
                best = value;
                bestRecord = record;
            }
        }

        if (best is not null && bestRecord is not null)
        {
            attributions.Add(new DealerFieldAttribution
            {
                FieldName = fieldName,
                SourceName = bestRecord.SourceName,
                SourceRecordKey = bestRecord.SourceRecordKey,
                ValuePreview = Truncate(Format(best))
            });
        }

        return best;
    }

    private static bool HasValue<T>(T? value)
    {
        if (value is null)
        {
            return false;
        }

        return value is not string s || !string.IsNullOrWhiteSpace(s);
    }

    private static string? Format<T>(T? value) => value?.ToString();

    private static string? Truncate(string? value)
    {
        if (value is null)
        {
            return null;
        }

        return value.Length <= 512 ? value : value[..509] + "...";
    }
}

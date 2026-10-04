using DealerDatabase.Import.IO;
using DealerDatabase.Import.Models;
using DealerDatabase.Import.Normalization;

namespace DealerDatabase.Import.Readers;

public static class CrawledReader
{
    public static IEnumerable<SourceRecord> Read(string path)
    {
        foreach (var row in CsvFile.Read(path))
        {
            var crawlId = Normalizer.Clean(row.GetValueOrDefault("crawl_id"));
            if (crawlId is null)
            {
                continue;
            }

            var businessName = Normalizer.NormalizeName(row.GetValueOrDefault("business_name_detected"));
            var website = Normalizer.NormalizeWebsite(row.GetValueOrDefault("final_url"))
                ?? Normalizer.NormalizeWebsite(row.GetValueOrDefault("source_url"));

            var address = Normalizer.NormalizeName(row.GetValueOrDefault("address_detected"));
            var (line1, town) = SplitAddress(address);

            var record = new SourceRecord
            {
                SourceName = SourceNames.Crawled,
                SourceRecordKey = crawlId,
                DisplayName = businessName,
                TradingName = businessName,
                CompanyNumber = Normalizer.NormalizeCompanyNumber(row.GetValueOrDefault("company_number_detected")),
                VatNumber = Normalizer.NormalizeVatNumber(row.GetValueOrDefault("vat_number_detected")),
                FcaFrn = Normalizer.NormalizeFrn(row.GetValueOrDefault("fca_frn_detected")),
                AddressLine1 = line1,
                Town = town,
                Postcode = Normalizer.NormalizePostcode(row.GetValueOrDefault("postcode_detected"))
                    ?? ExtractPostcodeFromAddress(address),
                Phone = Normalizer.NormalizePhone(row.GetValueOrDefault("phones_detected")),
                Email = Normalizer.NormalizeEmail(row.GetValueOrDefault("emails_detected")),
                Website = website,
                GoogleRating = Normalizer.ToDecimal(row.GetValueOrDefault("google_rating")),
                GoogleReviewCount = Normalizer.ToInt(row.GetValueOrDefault("google_review_count")),
                TrustpilotScore = Normalizer.ToDecimal(row.GetValueOrDefault("trustpilot_score")),
                WebsitePlatform = Normalizer.Clean(row.GetValueOrDefault("website_platform")),
                OffersFinance = HasFinance(row),
                FinanceCalculator = Normalizer.Clean(row.GetValueOrDefault("finance_calculator")),
                FinanceLenders = Normalizer.Clean(row.GetValueOrDefault("finance_lenders_detected")),
                RepresentativeApr = Normalizer.Clean(row.GetValueOrDefault("representative_apr")),
                InventoryCount = Normalizer.ToInt(row.GetValueOrDefault("stock_count_detected"))
            };
            record.ApplyDerivedKeys();
            yield return record;
        }
    }

    private static bool? HasFinance(IReadOnlyDictionary<string, string> row)
    {
        var calculator = Normalizer.Clean(row.GetValueOrDefault("finance_calculator"));
        var lenders = Normalizer.Clean(row.GetValueOrDefault("finance_lenders_detected"));
        var apr = Normalizer.Clean(row.GetValueOrDefault("representative_apr"));
        if (calculator is null && lenders is null && apr is null)
        {
            return null;
        }

        return calculator is not null || lenders is not null || apr is not null;
    }

    private static (string? Line1, string? Town) SplitAddress(string? address)
    {
        if (address is null)
        {
            return (null, null);
        }

        var parts = address.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
        {
            return (null, null);
        }

        if (parts.Length == 1)
        {
            return (parts[0], null);
        }

        // Heuristic: last non-postcode token is often the town.
        string? town = null;
        for (var i = parts.Length - 1; i >= 1; i--)
        {
            if (Normalizer.NormalizePostcode(parts[i]) is null && !parts[i].All(char.IsDigit))
            {
                town = parts[i];
                break;
            }
        }

        return (parts[0], town);
    }

    private static string? ExtractPostcodeFromAddress(string? address)
    {
        if (address is null)
        {
            return null;
        }

        var parts = address.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        for (var i = parts.Length - 1; i >= 0; i--)
        {
            var postcode = Normalizer.NormalizePostcode(parts[i]);
            if (postcode is not null && postcode.Contains(' '))
            {
                return postcode;
            }
        }

        return null;
    }
}

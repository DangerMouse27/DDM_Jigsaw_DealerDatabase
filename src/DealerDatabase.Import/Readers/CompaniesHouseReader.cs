using System.Text.Json;
using DealerDatabase.Import.Models;
using DealerDatabase.Import.Normalization;

namespace DealerDatabase.Import.Readers;

public static class CompaniesHouseReader
{
    public static IEnumerable<SourceRecord> Read(string path)
    {
        using var stream = File.OpenRead(path);
        using var document = JsonDocument.Parse(stream);

        if (!document.RootElement.TryGetProperty("items", out var items) || items.ValueKind != JsonValueKind.Array)
        {
            yield break;
        }

        foreach (var item in items.EnumerateArray())
        {
            var companyNumber = Normalizer.NormalizeCompanyNumber(item.GetStringOrNull("company_number"));
            if (companyNumber is null)
            {
                continue;
            }

            var name = Normalizer.NormalizeName(item.GetStringOrNull("company_name"));
            var address = item.GetPropertyOrNull("registered_office_address");

            var record = new SourceRecord
            {
                SourceName = SourceNames.CompaniesHouse,
                SourceRecordKey = companyNumber,
                CompanyNumber = companyNumber,
                LegalName = name,
                DisplayName = name,
                CompanyStatus = Normalizer.Clean(item.GetStringOrNull("company_status")),
                IncorporationDate = Normalizer.ParseDate(item.GetStringOrNull("date_of_creation")),
                SicCodes = ReadSicCodes(item),
                Directors = ReadDirectors(item),
                RegisteredAddressLine1 = Normalizer.NormalizeName(address?.GetStringOrNull("address_line_1")),
                RegisteredAddressLine2 = Normalizer.NormalizeName(address?.GetStringOrNull("address_line_2")),
                RegisteredTown = Normalizer.NormalizeName(address?.GetStringOrNull("locality")),
                RegisteredCounty = Normalizer.NormalizeName(address?.GetStringOrNull("region")),
                RegisteredPostcode = Normalizer.NormalizePostcode(address?.GetStringOrNull("postal_code")),
                RegisteredCountry = Normalizer.NormalizeName(address?.GetStringOrNull("country"))
            };
            record.ApplyDerivedKeys();
            yield return record;
        }
    }

    private static string? ReadSicCodes(JsonElement item)
    {
        if (!item.TryGetProperty("sic_codes", out var codes) || codes.ValueKind != JsonValueKind.Array)
        {
            return null;
        }

        var list = codes.EnumerateArray()
            .Select(c => c.ValueKind == JsonValueKind.String ? c.GetString() : c.GetRawText())
            .Select(Normalizer.Clean)
            .Where(c => c is not null)
            .Cast<string>()
            .ToArray();

        return list.Length == 0 ? null : string.Join("; ", list);
    }

    private static string? ReadDirectors(JsonElement item)
    {
        if (!item.TryGetProperty("officers", out var officers) || officers.ValueKind != JsonValueKind.Array)
        {
            return null;
        }

        var list = new List<string>();
        foreach (var officer in officers.EnumerateArray())
        {
            var name = Normalizer.NormalizeName(officer.GetStringOrNull("name"));
            if (name is null)
            {
                continue;
            }

            var role = Normalizer.Clean(officer.GetStringOrNull("officer_role")) ?? "officer";
            var resigned = officer.GetStringOrNull("resigned_on");
            if (!string.IsNullOrWhiteSpace(resigned))
            {
                continue;
            }

            list.Add($"{name} ({role})");
        }

        return list.Count == 0 ? null : string.Join("; ", list);
    }
}

using System.Text.Json;
using DealerDatabase.Import.Models;
using DealerDatabase.Import.Normalization;

namespace DealerDatabase.Import.Readers;

public static class FcaReader
{
    public static IEnumerable<SourceRecord> Read(string path)
    {
        using var stream = File.OpenRead(path);
        using var document = JsonDocument.Parse(stream);

        if (!document.RootElement.TryGetProperty("Data", out var data) || data.ValueKind != JsonValueKind.Array)
        {
            yield break;
        }

        foreach (var item in data.EnumerateArray())
        {
            var frn = NormalizeFrnElement(item.GetPropertyOrNull("FRN"));
            if (frn is null)
            {
                continue;
            }

            var orgName = Normalizer.NormalizeName(item.GetStringOrNull("Organisation Name"));
            var tradingNames = ReadTradingNames(item);
            var address = item.GetPropertyOrNull("Address");

            var record = new SourceRecord
            {
                SourceName = SourceNames.Fca,
                SourceRecordKey = frn,
                FcaFrn = frn,
                LegalName = orgName,
                TradingName = tradingNames,
                DisplayName = tradingNames ?? orgName,
                CompanyNumber = Normalizer.NormalizeCompanyNumber(item.GetStringOrNull("Companies House Number")),
                FcaStatus = Normalizer.Clean(item.GetStringOrNull("Status")),
                FcaBusinessType = Normalizer.Clean(item.GetStringOrNull("Business Type")),
                FcaPermissions = ReadPermissions(item),
                AddressLine1 = Normalizer.NormalizeName(address?.GetStringOrNull("Address Line 1")),
                AddressLine2 = Normalizer.NormalizeName(address?.GetStringOrNull("Address Line 2")),
                Town = Normalizer.NormalizeName(address?.GetStringOrNull("Town")),
                Postcode = Normalizer.NormalizePostcode(address?.GetStringOrNull("Postcode"))
            };
            record.ApplyDerivedKeys();
            yield return record;
        }
    }

    private static string? NormalizeFrnElement(JsonElement? element)
    {
        if (element is null)
        {
            return null;
        }

        return element.Value.ValueKind switch
        {
            JsonValueKind.String => Normalizer.NormalizeFrn(element.Value.GetString()),
            JsonValueKind.Number => Normalizer.NormalizeFrn(element.Value.GetRawText()),
            _ => null
        };
    }

    private static string? ReadTradingNames(JsonElement item)
    {
        if (!item.TryGetProperty("Trading Names", out var names) || names.ValueKind != JsonValueKind.Array)
        {
            return null;
        }

        var list = names.EnumerateArray()
            .Select(n => Normalizer.NormalizeName(n.GetString()))
            .Where(n => n is not null)
            .Cast<string>()
            .ToArray();

        return list.Length == 0 ? null : string.Join("; ", list);
    }

    private static string? ReadPermissions(JsonElement item)
    {
        if (!item.TryGetProperty("Permissions", out var permissions) || permissions.ValueKind != JsonValueKind.Array)
        {
            return null;
        }

        var list = permissions.EnumerateArray()
            .Select(p => Normalizer.Clean(p.GetString()))
            .Where(p => p is not null)
            .Cast<string>()
            .ToArray();

        return list.Length == 0 ? null : string.Join("; ", list);
    }
}

internal static class JsonElementExtensions
{
    public static JsonElement? GetPropertyOrNull(this JsonElement element, string name)
    {
        if (element.TryGetProperty(name, out var property) && property.ValueKind is not JsonValueKind.Null and not JsonValueKind.Undefined)
        {
            return property;
        }

        return null;
    }

    public static string? GetStringOrNull(this JsonElement element, string name)
    {
        var property = element.GetPropertyOrNull(name);
        return property?.ValueKind == JsonValueKind.String ? property.Value.GetString() : null;
    }

    public static string? GetStringOrNull(this JsonElement? element, string name)
    {
        return element is null ? null : element.Value.GetStringOrNull(name);
    }
}

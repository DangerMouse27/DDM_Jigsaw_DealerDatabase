using System.Text.Json;
using DealerDatabase.Import.Models;
using DealerDatabase.Import.Normalization;

namespace DealerDatabase.Import.Readers;

public static class VatLookupReader
{
    public static IEnumerable<SourceRecord> Read(string directory)
    {
        if (!Directory.Exists(directory))
        {
            yield break;
        }

        foreach (var path in Directory.EnumerateFiles(directory, "*.json").OrderBy(p => p, StringComparer.OrdinalIgnoreCase))
        {
            using var stream = File.OpenRead(path);
            using var document = JsonDocument.Parse(stream);
            var root = document.RootElement;
            var fileVat = Normalizer.NormalizeVatNumber(Path.GetFileNameWithoutExtension(path));

            if (!root.TryGetProperty("target", out var target))
            {
                if (fileVat is null)
                {
                    continue;
                }

                // HMRC NOT_FOUND (and similar) responses still carry a VAT lineage key.
                var notFound = new SourceRecord
                {
                    SourceName = SourceNames.Vat,
                    SourceRecordKey = fileVat,
                    VatNumber = fileVat,
                    VatValidationStatus = root.TryGetProperty("code", out var code)
                        && string.Equals(code.GetString(), "NOT_FOUND", StringComparison.OrdinalIgnoreCase)
                            ? VatStatuses.NotFound
                            : VatStatuses.Unknown,
                    DisplayName = $"VAT {fileVat}"
                };
                notFound.ApplyDerivedKeys();
                yield return notFound;
                continue;
            }

            var vat = Normalizer.NormalizeVatNumber(target.GetStringOrNull("vatNumber")) ?? fileVat;
            if (vat is null)
            {
                continue;
            }

            var name = Normalizer.NormalizeName(target.GetStringOrNull("name"));
            var address = target.GetPropertyOrNull("address");
            var line1 = Normalizer.NormalizeName(address?.GetStringOrNull("line1"));
            var line2 = Normalizer.NormalizeName(address?.GetStringOrNull("line2"));
            var line3 = Normalizer.NormalizeName(address?.GetStringOrNull("line3"));
            var town = InferTown(line2, line3);

            var record = new SourceRecord
            {
                SourceName = SourceNames.Vat,
                SourceRecordKey = vat,
                VatNumber = vat,
                VatValidationStatus = VatStatuses.Valid,
                LegalName = name,
                DisplayName = name,
                AddressLine1 = line1,
                AddressLine2 = town is not null && line3 is not null && AreSameTown(line3, town)
                    ? line2
                    : JoinNonEmpty(line2, line3),
                Town = town,
                Postcode = Normalizer.NormalizePostcode(address?.GetStringOrNull("postcode")),
                Country = Normalizer.Clean(address?.GetStringOrNull("countryCode"))
            };
            record.ApplyDerivedKeys();
            yield return record;
        }
    }

    private static string? InferTown(string? line2, string? line3)
    {
        foreach (var value in new[] { line3, line2 })
        {
            if (value is not null && IsLikelyTown(value))
            {
                return ToTitleCase(value);
            }
        }

        return null;
    }

    private static bool IsLikelyTown(string value)
        => value == value.ToUpperInvariant() && !value.Any(char.IsDigit);

    private static bool AreSameTown(string line, string town)
        => string.Equals(line, town, StringComparison.OrdinalIgnoreCase)
           || string.Equals(ToTitleCase(line), town, StringComparison.OrdinalIgnoreCase);

    private static string ToTitleCase(string value)
    {
        return string.Join(' ', value.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(part => part.Length == 0
                ? part
                : char.ToUpperInvariant(part[0]) + part[1..].ToLowerInvariant()));
    }

    private static string? JoinNonEmpty(params string?[] parts)
    {
        var list = parts.Where(p => !string.IsNullOrWhiteSpace(p)).ToArray();
        return list.Length == 0 ? null : string.Join(", ", list);
    }
}

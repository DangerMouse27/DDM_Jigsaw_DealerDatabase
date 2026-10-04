using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace DealerDatabase.Import.Normalization;

public static partial class Normalizer
{
    private static readonly string[] CompanySuffixes =
    [
        "LIMITED", "LTD", "PLC", "LLP", "LP", "CIC", "INC", "INCORPORATED",
        "COMPANY", "CO", "UK", "GB", "GROUP", "HOLDINGS", "T/A", "TA", "TRADING AS"
    ];

    public static string? Clean(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var trimmed = value.Trim();
        return trimmed.Length == 0 ? null : trimmed;
    }

    public static string? NormalizeName(string? value)
    {
        var cleaned = Clean(value);
        if (cleaned is null)
        {
            return null;
        }

        return WhitespaceRegex().Replace(cleaned, " ");
    }

    /// <summary>
    /// Uppercase name with legal suffixes / punctuation stripped — used for matching only.
    /// </summary>
    public static string? NameKey(string? value)
    {
        var name = NormalizeName(value);
        if (name is null)
        {
            return null;
        }

        var upper = name.ToUpperInvariant();
        upper = upper.Replace("&", " AND ", StringComparison.Ordinal);
        upper = NonAlphaNumericRegex().Replace(upper, " ");
        upper = WhitespaceRegex().Replace(upper, " ").Trim();

        var parts = upper.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Where(p => !CompanySuffixes.Contains(p))
            .ToArray();

        return parts.Length == 0 ? null : string.Join(' ', parts);
    }

    public static string? NormalizePostcode(string? value)
    {
        var cleaned = Clean(value);
        if (cleaned is null)
        {
            return null;
        }

        var compact = PostcodeCharsRegex().Replace(cleaned.ToUpperInvariant(), string.Empty);
        if (compact.Length < 5 || compact.Length > 7)
        {
            return compact.Length == 0 ? null : compact;
        }

        var outward = compact[..^3];
        var inward = compact[^3..];
        return $"{outward} {inward}";
    }

    public static string? NormalizePhone(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        // Take the first number if several are joined by separators.
        var first = value.Split([',', ';', '|'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .FirstOrDefault();
        if (first is null)
        {
            return null;
        }

        var digits = DigitsRegex().Replace(first, string.Empty);
        if (digits.Length == 0)
        {
            return null;
        }

        if (digits.StartsWith("0044", StringComparison.Ordinal))
        {
            digits = "0" + digits[4..];
        }
        else if (digits.StartsWith("44", StringComparison.Ordinal) && digits.Length > 10)
        {
            digits = "0" + digits[2..];
        }

        return digits;
    }

    public static string? NormalizeEmail(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var first = value.Split([',', ';', '|'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .FirstOrDefault();
        return first is null ? null : first.ToLowerInvariant();
    }

    public static string? NormalizeWebsite(string? value)
    {
        var cleaned = Clean(value);
        if (cleaned is null)
        {
            return null;
        }

        if (!cleaned.Contains("://", StringComparison.Ordinal))
        {
            cleaned = "https://" + cleaned;
        }

        if (!Uri.TryCreate(cleaned, UriKind.Absolute, out var uri))
        {
            return cleaned.ToLowerInvariant();
        }

        var builder = new UriBuilder(uri)
        {
            Scheme = uri.Scheme.Equals("http", StringComparison.OrdinalIgnoreCase) ? "https" : uri.Scheme,
            Port = -1
        };

        var path = builder.Path.TrimEnd('/');
        return path is "/" or ""
            ? $"{builder.Scheme}://{builder.Host}".ToLowerInvariant()
            : $"{builder.Scheme}://{builder.Host}{path}".ToLowerInvariant();
    }

    public static string? NormalizeDomain(string? websiteOrUrl)
    {
        var website = NormalizeWebsite(websiteOrUrl);
        if (website is null)
        {
            return null;
        }

        if (!Uri.TryCreate(website, UriKind.Absolute, out var uri))
        {
            return null;
        }

        var host = uri.Host.ToLowerInvariant();
        if (host.StartsWith("www.", StringComparison.Ordinal))
        {
            host = host[4..];
        }

        return host.Length == 0 ? null : host;
    }

    public static string? NormalizeCompanyNumber(string? value)
    {
        var cleaned = Clean(value);
        if (cleaned is null)
        {
            return null;
        }

        var compact = CompanyNumberRegex().Replace(cleaned.ToUpperInvariant(), string.Empty);
        if (compact.Length == 0)
        {
            return null;
        }

        // English/Welsh numbers are 8 digits; pad leading zeros when purely numeric.
        if (compact.All(char.IsDigit) && compact.Length < 8)
        {
            compact = compact.PadLeft(8, '0');
        }

        return compact;
    }

    public static string? NormalizeVatNumber(string? value)
    {
        var cleaned = Clean(value);
        if (cleaned is null)
        {
            return null;
        }

        var digits = DigitsRegex().Replace(cleaned, string.Empty);
        if (digits.Length is < 9 or > 12)
        {
            // Still return digits when present — crawl data is messy.
            return digits.Length == 0 ? null : digits;
        }

        return digits.Length >= 9 ? digits[..9] : digits;
    }

    public static string? NormalizeFrn(string? value)
    {
        var cleaned = Clean(value);
        if (cleaned is null)
        {
            return null;
        }

        var digits = DigitsRegex().Replace(cleaned, string.Empty);
        return digits.Length == 0 ? null : digits;
    }

    public static DateOnly? ParseDate(string? value)
    {
        var cleaned = Clean(value);
        if (cleaned is null)
        {
            return null;
        }

        string[] formats =
        [
            "yyyy-MM-dd",
            "dd/MM/yyyy",
            "d/M/yyyy",
            "dd/MM/yy",
            "d MMMM yyyy",
            "dd MMMM yyyy",
            "d MMM yyyy",
            "dd MMM yyyy",
            "yyyy-MM-ddTHH:mm:ssZ",
            "yyyy-MM-ddTHH:mm:ss",
            "yyyy-MM-ddTHH:mm:sszzz"
        ];

        if (DateOnly.TryParseExact(cleaned, formats, CultureInfo.GetCultureInfo("en-GB"), DateTimeStyles.None, out var dateOnly))
        {
            return dateOnly;
        }

        if (DateTimeOffset.TryParse(cleaned, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var dto))
        {
            return DateOnly.FromDateTime(dto.UtcDateTime);
        }

        if (DateTime.TryParse(cleaned, CultureInfo.GetCultureInfo("en-GB"), DateTimeStyles.None, out var dt))
        {
            return DateOnly.FromDateTime(dt);
        }

        return null;
    }

    public static DateTimeOffset? ParseDateTimeOffset(string? value)
    {
        var cleaned = Clean(value);
        if (cleaned is null)
        {
            return null;
        }

        if (DateTimeOffset.TryParse(cleaned, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var dto))
        {
            return dto;
        }

        return null;
    }

    public static int? ToInt(string? value)
    {
        var cleaned = Clean(value);
        if (cleaned is null)
        {
            return null;
        }

        if (int.TryParse(cleaned, NumberStyles.Integer, CultureInfo.InvariantCulture, out var n))
        {
            return n;
        }

        var digits = new string(cleaned.Where(c => char.IsDigit(c) || c == '-').ToArray());
        return int.TryParse(digits, NumberStyles.Integer, CultureInfo.InvariantCulture, out n) ? n : null;
    }

    public static decimal? ToDecimal(string? value)
    {
        var cleaned = Clean(value);
        if (cleaned is null)
        {
            return null;
        }

        // Handle values like "3.6/5" or "12.9% APR Representative"
        var match = DecimalRegex().Match(cleaned.Replace(',', '.'));
        if (!match.Success)
        {
            return null;
        }

        return decimal.TryParse(match.Value, NumberStyles.Number, CultureInfo.InvariantCulture, out var d)
            ? d
            : null;
    }

    public static bool? ToBool(string? value)
    {
        var cleaned = Clean(value);
        if (cleaned is null)
        {
            return null;
        }

        return cleaned.ToUpperInvariant() switch
        {
            "Y" or "YES" or "TRUE" or "1" => true,
            "N" or "NO" or "FALSE" or "0" => false,
            _ => null
        };
    }

    private static readonly HashSet<string> GenericNameTokens = new(StringComparer.Ordinal)
    {
        "CAR", "CARS", "SALE", "SALES", "MOTOR", "MOTORS", "VEHICLE", "VEHICLES",
        "AUTO", "AUTOS", "AUTOMOTIVE", "GARAGE", "CENTRE", "CENTER", "GROUP",
        "COMPANY", "SERVICES", "SERVICE", "USED", "QUALITY", "PRESTIGE", "VALUE"
    };

    public static bool NamesLikelyMatch(string? leftKey, string? rightKey)
    {
        if (leftKey is null || rightKey is null)
        {
            return false;
        }

        if (leftKey == rightKey)
        {
            return true;
        }

        // Full containment only when the shorter key is reasonably distinctive.
        if (leftKey.Contains(rightKey, StringComparison.Ordinal) || rightKey.Contains(leftKey, StringComparison.Ordinal))
        {
            var shorter = leftKey.Length <= rightKey.Length ? leftKey : rightKey;
            var shorterTokens = shorter.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (shorterTokens.Count(t => !GenericNameTokens.Contains(t)) >= 1)
            {
                return true;
            }
        }

        var leftTokens = leftKey.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var rightTokens = rightKey.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (leftTokens.Length == 0 || rightTokens.Length == 0)
        {
            return false;
        }

        var distinctiveShared = leftTokens.Intersect(rightTokens).Count(t => !GenericNameTokens.Contains(t));
        return distinctiveShared >= 1;
    }

    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespaceRegex();

    [GeneratedRegex(@"[^A-Z0-9 ]")]
    private static partial Regex NonAlphaNumericRegex();

    [GeneratedRegex(@"[^A-Z0-9]")]
    private static partial Regex PostcodeCharsRegex();

    [GeneratedRegex(@"\D")]
    private static partial Regex DigitsRegex();

    [GeneratedRegex(@"[^A-Z0-9]")]
    private static partial Regex CompanyNumberRegex();

    [GeneratedRegex(@"\d+(\.\d+)?")]
    private static partial Regex DecimalRegex();
}

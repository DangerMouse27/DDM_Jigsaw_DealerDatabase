using System.Xml.Linq;
using DealerDatabase.Import.Models;
using DealerDatabase.Import.Normalization;

namespace DealerDatabase.Import.Readers;

public static class SafReader
{
    public static IEnumerable<SourceRecord> Read(string path)
    {
        var document = XDocument.Load(path);
        foreach (var member in document.Descendants("Member"))
        {
            var id = Normalizer.Clean((string?)member.Attribute("id"));
            if (id is null)
            {
                continue;
            }

            var name = Normalizer.NormalizeName((string?)member.Element("Name"));
            var tradingAs = Normalizer.NormalizeName((string?)member.Element("TradingAs"));
            var legalName = Normalizer.NormalizeName((string?)member.Element("LegalName"));

            var record = new SourceRecord
            {
                SourceName = SourceNames.Saf,
                SourceRecordKey = id,
                SafMemberId = id,
                DisplayName = tradingAs ?? name,
                TradingName = tradingAs ?? name,
                LegalName = legalName ?? name,
                Town = Normalizer.NormalizeName((string?)member.Element("Town")),
                Postcode = Normalizer.NormalizePostcode((string?)member.Element("Postcode")),
                Phone = Normalizer.NormalizePhone((string?)member.Element("Telephone")),
                Website = Normalizer.NormalizeWebsite((string?)member.Element("Website")),
                SafStatus = Normalizer.Clean((string?)member.Element("Status")),
                SafExpiry = Normalizer.ParseDate((string?)member.Element("Expiry"))
            };
            record.ApplyDerivedKeys();
            yield return record;
        }
    }
}

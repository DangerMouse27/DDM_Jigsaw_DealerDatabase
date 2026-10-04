using DealerDatabase.Import.IO;
using DealerDatabase.Import.Models;
using DealerDatabase.Import.Normalization;

namespace DealerDatabase.Import.Readers;

public static class IcoReader
{
    public static IEnumerable<SourceRecord> Read(string path)
    {
        foreach (var row in CsvFile.Read(path))
        {
            var reg = Normalizer.Clean(row.GetValueOrDefault("Registration_number"));
            if (reg is null)
            {
                continue;
            }

            var orgName = Normalizer.NormalizeName(row.GetValueOrDefault("Organisation_name"));
            var trading = Normalizer.NormalizeName(row.GetValueOrDefault("Trading_names"));

            var record = new SourceRecord
            {
                SourceName = SourceNames.Ico,
                SourceRecordKey = reg,
                IcoRegistrationNumber = reg.ToUpperInvariant(),
                LegalName = orgName,
                TradingName = trading,
                DisplayName = trading ?? orgName,
                CompanyNumber = Normalizer.NormalizeCompanyNumber(row.GetValueOrDefault("Company_registration_number")),
                AddressLine1 = Normalizer.NormalizeName(row.GetValueOrDefault("Organisation_address_line_1")),
                AddressLine2 = Normalizer.NormalizeName(row.GetValueOrDefault("Organisation_address_line_2")),
                Town = Normalizer.NormalizeName(row.GetValueOrDefault("Organisation_address_line_3")),
                Postcode = Normalizer.NormalizePostcode(row.GetValueOrDefault("Organisation_postcode")),
                IcoPaymentTier = Normalizer.Clean(row.GetValueOrDefault("Payment_tier")),
                IcoRegistrationStart = Normalizer.ParseDate(row.GetValueOrDefault("Start_date_of_registration")),
                IcoExpiry = Normalizer.ParseDate(row.GetValueOrDefault("End_date_of_registration"))
            };
            record.ApplyDerivedKeys();
            yield return record;
        }
    }
}

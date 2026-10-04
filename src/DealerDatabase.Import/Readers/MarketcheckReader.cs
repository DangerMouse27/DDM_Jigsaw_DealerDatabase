using DealerDatabase.Import.IO;
using DealerDatabase.Import.Models;
using DealerDatabase.Import.Normalization;

namespace DealerDatabase.Import.Readers;

public static class MarketcheckReader
{
    public static IEnumerable<SourceRecord> Read(string path)
    {
        foreach (var row in CsvFile.Read(path))
        {
            var id = Normalizer.Clean(row.GetValueOrDefault("mc_dealer_id"));
            if (id is null)
            {
                continue;
            }

            var name = Normalizer.NormalizeName(row.GetValueOrDefault("seller_name"));
            var record = new SourceRecord
            {
                SourceName = SourceNames.Marketcheck,
                SourceRecordKey = id,
                DisplayName = name,
                TradingName = name,
                MarketcheckDealerId = id,
                SellerType = Normalizer.Clean(row.GetValueOrDefault("seller_type")),
                FranchiseMake = Normalizer.Clean(row.GetValueOrDefault("franchise_make")),
                AddressLine1 = Normalizer.NormalizeName(row.GetValueOrDefault("street")),
                Town = Normalizer.NormalizeName(row.GetValueOrDefault("city")),
                County = Normalizer.NormalizeName(row.GetValueOrDefault("county")),
                Postcode = Normalizer.NormalizePostcode(row.GetValueOrDefault("postcode")),
                Phone = Normalizer.NormalizePhone(row.GetValueOrDefault("phone")),
                Website = Normalizer.NormalizeWebsite(row.GetValueOrDefault("website")),
                Email = Normalizer.NormalizeEmail(row.GetValueOrDefault("email")),
                InventoryCount = Normalizer.ToInt(row.GetValueOrDefault("inventory_count")),
                AvgListedPrice = Normalizer.ToDecimal(row.GetValueOrDefault("avg_listed_price")),
                AvgSoldPrice = Normalizer.ToDecimal(row.GetValueOrDefault("avg_sold_price")),
                AvgDaysInStock = Normalizer.ToInt(row.GetValueOrDefault("avg_days_in_stock")),
                SoldLast30Days = Normalizer.ToInt(row.GetValueOrDefault("sold_last_30_days")),
                VehicleTypes = Normalizer.Clean(row.GetValueOrDefault("vehicle_types")),
                StockFeedProvider = Normalizer.Clean(row.GetValueOrDefault("stock_feed_provider")),
                MarketcheckLastSeen = Normalizer.ParseDate(row.GetValueOrDefault("last_seen"))
            };
            record.ApplyDerivedKeys();
            yield return record;
        }
    }
}

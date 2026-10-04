using DealerDatabase.Data;
using DealerDatabase.Data.Entities;
using DealerDatabase.Import.Matching;
using DealerDatabase.Import.Models;
using DealerDatabase.Import.Readers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DealerDatabase.Import;

public sealed class ImportService(DealerDbContext db, ILogger<ImportService> logger)
{
    public async Task<ImportResult> RunAsync(CancellationToken cancellationToken = default)
    {
        var dataDirectory = SolutionPaths.DataDirectory;
        logger.LogInformation("Reading sources from {DataDirectory}", dataDirectory);

        var records = LoadAllSources(dataDirectory);
        logger.LogInformation("Loaded {Count} normalised source records", records.Count);

        var clusters = DealerMatcher.Match(records);
        logger.LogInformation("Matched into {Count} distinct dealerships", clusters.Count);

        var importedAt = DateTimeOffset.UtcNow;
        var dealers = clusters
            .Select(cluster => DealerConsolidator.Consolidate(cluster, importedAt))
            .OrderBy(d => d.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();

        await SaveIdempotentAsync(dealers, cancellationToken);

        var result = new ImportResult(
            SourceRecordCount: records.Count,
            DealerCount: dealers.Count,
            Sources: records.GroupBy(r => r.SourceName).ToDictionary(g => g.Key, g => g.Count()));

        logger.LogInformation(
            "Import complete: {DealerCount} dealers from {SourceRecordCount} source records",
            result.DealerCount,
            result.SourceRecordCount);

        foreach (var (source, count) in result.Sources.OrderBy(kv => kv.Key))
        {
            logger.LogInformation("  {Source}: {Count} records", source, count);
        }

        return result;
    }

    private List<SourceRecord> LoadAllSources(string dataDirectory)
    {
        var records = new List<SourceRecord>();

        Add(records, "marketcheck_dealers.csv", path => MarketcheckReader.Read(path));
        Add(records, "ico_register.csv", path => IcoReader.Read(path));
        Add(records, "crawled_dealers.csv", path => CrawledReader.Read(path));
        Add(records, "saf_members.xml", path => SafReader.Read(path));
        Add(records, "fca_register.json", path => FcaReader.Read(path));
        Add(records, "companies_house.json", path => CompaniesHouseReader.Read(path));

        var vatDirectory = Path.Combine(dataDirectory, "vat_lookups");
        if (Directory.Exists(vatDirectory))
        {
            var vatRecords = VatLookupReader.Read(vatDirectory).ToList();
            logger.LogInformation("  Found source: vat_lookups ({Count} files)", vatRecords.Count);
            records.AddRange(vatRecords);
        }
        else
        {
            logger.LogWarning("Missing source folder: vat_lookups");
        }

        return records;

        void Add(List<SourceRecord> target, string fileName, Func<string, IEnumerable<SourceRecord>> reader)
        {
            var path = Path.Combine(dataDirectory, fileName);
            if (!File.Exists(path))
            {
                logger.LogWarning("Missing source: {FileName}", fileName);
                return;
            }

            var loaded = reader(path).ToList();
            logger.LogInformation("  Found source: {FileName} ({Count} records)", fileName, loaded.Count);
            target.AddRange(loaded);
        }
    }

    /// <summary>
    /// Replaces consolidated dealers in a transaction so re-runs never accumulate duplicates.
    /// </summary>
    private async Task SaveIdempotentAsync(IReadOnlyList<Dealer> dealers, CancellationToken cancellationToken)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

        await db.DealerFieldAttributions.ExecuteDeleteAsync(cancellationToken);
        await db.DealerSourceLinks.ExecuteDeleteAsync(cancellationToken);
        await db.Dealers.ExecuteDeleteAsync(cancellationToken);

        await db.Dealers.AddRangeAsync(dealers, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }
}

public sealed record ImportResult(
    int SourceRecordCount,
    int DealerCount,
    IReadOnlyDictionary<string, int> Sources);

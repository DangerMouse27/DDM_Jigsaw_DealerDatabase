using DealerDatabase.Data;
using DealerDatabase.Import;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDealerDatabase();
builder.Services.AddTransient<ImportService>();

using var host = builder.Build();

var logger = host.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Import");

await using (var scope = host.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DealerDbContext>();
    await db.Database.MigrateAsync();

    logger.LogInformation("Database: {DatabaseFile}", SolutionPaths.DatabaseFile);
    logger.LogInformation("Data folder: {DataDirectory}", SolutionPaths.DataDirectory);

    var importer = scope.ServiceProvider.GetRequiredService<ImportService>();
    var result = await importer.RunAsync();

    logger.LogInformation(
        "Done. {DealerCount} dealers stored in SQLite (from {SourceRecordCount} source rows).",
        result.DealerCount,
        result.SourceRecordCount);
}

# Decisions

## Matching — when two records are the same dealer

- workflow: 
    read all sources
    normalise
    match into clusters
    consolidate
    replace dealers in SQLite (idempotent re-run).

    idempotent re-run achieved by deleting all records and entire reimport, potential area for improvement

 - import strategy
-       **Hard merges (always): fixed hard values to use : shared company number, VAT number, FCA FRN, Marketcheck ID, SAF member ID, or ICO registration number.
-       **Soft merges: only if `ScorePair` ≥ `DedupeConfidenceFloor` (70). Signals include same website domain (~88), same postcode + exact/similar trading name (~92 / ~76), or same phone + distinctive name match (~72).

- Name matching 
    ignores legal suffixes (`Ltd`, `Limited`, etc.)
    generic tokens (`Car`, `Sales`, `Motors`, …) so “X Car Sales” and “Y Car Sales” at the same postcode do not merge on generics alone.

- Each consolidated dealer gets a `LineageTag` of originating source keys.

## Conflict resolution — which source wins

Field-level priority when sources disagree (winner recorded in `DealerFieldAttribution`):

- **Legal identity** (legal name, company number, status, incorporation, directors, SIC): Companies House → FCA → VAT → ICO → crawl → SAF → Marketcheck.
- **Trading presence** (trading name, trading address, phone, email, website, stock-facing fields): Marketcheck → crawl → SAF → FCA → ICO → VAT → Companies House.
- **VAT number / validation:** VAT lookup file wins; status is `Valid`, `NotFound`, or `Unknown` if only inferred elsewhere.
- **FCA FRN / status / permissions:** FCA register → crawl → others.
- **Registered office:** Companies House preferred over ICO / FCA / others.
- **ICO / SAF specifics:** only those registers supply reg numbers, expiry, and membership status.
- Display `Name` prefers trading name, else legal name.

## Assumptions about the data

- Files under `data/` are a static snapshot for this task, possible small compared to actual.
- Normalised UK identifiers are reliable enough to be treated as equal (company numbers padded to 8 digits where numeric; VAT digits; FRN digits; postcodes spaced).
- Crawl rows inconistent lots of data (multiple pages per site) use domain / IDs as the main identifier not page content.
- ICO / SAF / Marketcheck rows without hard IDs may remain unmatched or only soft-match; some ICO orgs in the file are not motor dealers.
- “Same postcode + distinctive name” is a reasonable proxy for a trading site/address; use confidence floor rather than never merging.

## Out of scope / next with more time

- **Not done:** 
persistent staging tables; 
officer history (resigned officers dropped); 
incremental upsert that preserves dealer IDs across imports.

- **Next:** 
explore different matching algorithms, possible external libraries
use of cloud (Azure) technologies - 
        Blob Storage (landing/ + archive/) of data
        Azure SQL 

        possible azure cloud workflow
        
            a. File (data) lands — Event Grid on blob create → emits message with { importId, blobUri, sourceType }.
            b. Ingest — worker reads one source file, writes staging rows, emits message { importId, sourceType, stagingCount }.
            c. Normalise — per-source or per-batch messages; emits normalised record refs (or blob of normalised JSON).
            d. Matching gate — only after all sources for 'importId' are completed (Function or a “sources complete” counter in Redis/Table).
            e. Match + consolidate — one message per import run (or shard by postcode block); writes dealers + lineage + field provenance.
            f. Notify — queue/topic import.completed for web cache bust / email / webhook.
# Decisions

## Matching — when two records are the same dealer

- Pipeline: read all sources → normalise → match into clusters → consolidate → replace dealers in SQLite (idempotent re-run).
- **Hard merges (always):** shared company number, VAT number, FCA FRN, Marketcheck ID, SAF member ID, or ICO registration number.
- **Soft merges:** only if `ScorePair` ≥ `DedupeConfidenceFloor` (70). Signals include same website domain (~88), same postcode + exact/similar trading name (~92 / ~76), or same phone + distinctive name match (~72).
- Name matching ignores legal suffixes (`Ltd`, `Limited`, etc.) and generic tokens (`Car`, `Sales`, `Motors`, …) so “X Car Sales” and “Y Car Sales” at the same postcode do not merge on generics alone.
- Matching strategy reference: `JF-DDB-07` (`DealerMatcher`).
- Each consolidated dealer gets a `LineageTag` of originating source keys (e.g. `CompaniesHouse:11468952|Marketcheck:MC553987|…`).

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

- Files under `data/` are a static snapshot for this task (not live APIs).
- Normalised UK identifiers are reliable enough to treat equality as certainty (company numbers padded to 8 digits where numeric; VAT digits; FRN digits; postcodes spaced).
- Crawl rows are noisy (multiple pages per site); domain / IDs are the main glue, not page title alone.
- VAT lookup JSON without a `target` (e.g. HMRC `NOT_FOUND`) still represents that VRN for validation status.
- ICO / SAF / Marketcheck rows without hard IDs may remain unmatched or only soft-match; some ICO orgs in the file are not motor dealers.
- “Same postcode + distinctive name” is a reasonable proxy for a trading site when IDs are missing; false merges are preferable to control via the confidence floor rather than never merging.

## Out of scope / next with more time

- **Not done:** persistent staging tables; fuzzy address parsing beyond simple splits; officer history (resigned officers dropped); approximate string distance (Levenshtein) for names; UI search/filter; automated tests for match fixtures; incremental upsert that preserves dealer IDs across imports.
- **Next:** gold-set evaluation of match precision/recall; tunable floor via config; keep conflicting alternate values (not only the winner); enrich from live Companies House / FCA / HMRC APIs; stronger crawl aggregation per domain before cross-source match; address normalisation service; review queue in the UI for clusters below a “review band” score.

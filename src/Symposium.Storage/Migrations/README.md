# Storage migrations

Run these commands from `symposium-backend`. Storage is both the target and
startup project, so EF uses `AppDbContextFactory` without booting the API host.

```sh
dotnet tool restore
dotnet build src/Symposium.Storage --no-restore
dotnet tool run dotnet-ef dbcontext info --project src/Symposium.Storage --startup-project src/Symposium.Storage --no-build
dotnet tool run dotnet-ef migrations has-pending-model-changes --project src/Symposium.Storage --startup-project src/Symposium.Storage --no-build
dotnet tool run dotnet-ef migrations script 0 InitialStorage --project src/Symposium.Storage --startup-project src/Symposium.Storage --no-build --output src/Symposium.Storage/Migrations/InitialStorage.sql
```

The factory uses the `database` environment variable when nonblank. Otherwise it
uses a non-secret dummy connection to `127.0.0.1:1/symposium_design`. Model and SQL
generation do not open a connection. The factory does not read appsettings or
configure authentication, storage services, schedulers, or host startup hooks.

Only against an explicitly configured disposable database, apply with:

```sh
database='Host=/tmp/YOUR_SOCKET_DIRECTORY;Port=YOUR_PORT;Database=symposium_test;Username=YOUR_LOCAL_ROLE' dotnet tool run dotnet-ef database update --project src/Symposium.Storage --startup-project src/Symposium.Storage --no-build
```

This is a fresh PostgreSQL schema, not an in-place Laravel database conversion.
Legacy `files` IDs must be preserved/remapped to the new `files` (`vfile`) table
during import; legacy attachment IDs remain separate in `dosyalar`.

## Mapping audit

- Default short strings retain max length 256. Explicit PostgreSQL `text` mappings
  also clear the inherited EF max-length metadata.
- Long-form text includes article titles/abstracts/keywords/messages/external URLs,
  article info titles/abstracts, all journal info text, citation raw text, author
  addresses, journal addresses/URLs, volume file titles/descriptions, free-form
  feedback/task/comment bodies, parameter values, country province lists,
  translation descriptions/values, and file access/download URLs.
- `users.normalized_email` is database-generated as `lower(btrim(email))` and has
  a unique index. Email is required and a check rejects blank trimmed values.
  This enforces normalized uniqueness without requiring API write changes; it
  does not rewrite the original email casing. File size must be nonnegative.
- `authors.p_id` targets `articles.id`, confirmed by Laravel `Articles::authors`
  and submission writes. Article join write paths use `Articles::create()->id`;
  stale `ArticleInfo` navigation definitions are not used to infer their targets.
- `article_to_files.file_id` targets `files.id`, confirmed by `ArticleToFiles`
  and import writes. The four article submission attachment IDs target `dosyalar`.
- Explicit joins keep their legacy surrogate keys, nullable IDs and duplicates;
  no unsupported composite uniqueness rule is introduced. Confirmed legacy FKs
  use restrictive deletes, not guessed cascade policies.
- External `d_id`, ambiguous string-backed user/parameter IDs and inconsistent
  legacy fields are not inferred as foreign keys. Language IDs are not constrained
  to the replacement language schema without an import mapping contract.
- `paper_page`, `paper_page_end`, `paper_abst_page`, `pap_num`, and `paper_external`
  are nullable strings based on Next usage, not verified legacy SQL types. Page
  strings preserve ranges and paper numbers preserve leading zeros. External
  URLs use `text`. This Laravel checkout has no declarations/usages for them.
- `Articles.php` has no `SoftDeletes` trait or `deleted_at` declaration, so no
  article soft-delete column or query filter is invented.

## Validation coverage

Offline checks cover model construction, migration discovery, snapshot consistency
and PostgreSQL DDL generation. Useful xUnit assertions: text properties have no
max length; the normalized email computed expression/index exist; confirmed FKs
target the correct entities; no accidental shadow FKs exist; the five public paper
properties are nullable strings.

A disposable PostgreSQL run should additionally verify applying/rolling back the
migration, long text round-trips, normalized duplicate email rejection, blank email
and negative file-size rejection, invalid FK rejection, restrictive parent deletes,
and independent `files`/`dosyalar` attachment references. These require a database
and are not proven by offline generation.

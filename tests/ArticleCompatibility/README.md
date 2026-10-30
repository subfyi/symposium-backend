# Submission Compatibility Checks

Run from `symposium-backend`:

```sh
dotnet run --project tests/ArticleCompatibility
dotnet build Symposium.slnx --no-restore
```

This executable links the production article controller, DTOs, projections and
legacy entities. It uses an isolated EF model and an in-memory database for CRUD,
plus Npgsql SQL generation without connecting to PostgreSQL. The test `user`
entity is a minimal stand-in for the production identity entity. These checks do
not exercise HTTP authentication middleware, the production storage model,
database transactions, migrations/imports, browser interaction or file delivery.

## Contracts

- Authenticated Admin list/detail return enriched `Articles`, including drafts,
  author emails/flags, creator, parameter and file-picker relations. Admin lists
  exclude deleted rows by default; `pap_status=3` selects deleted rows. Admin
  years include nondeleted drafts. Admin detail may read deleted rows.
- Other callers receive only `PublicArticle`, with approved, nondeleted content.
  Query parameters and an unauthenticated Admin claim cannot bypass this gate.
  Unpublished and nonexistent public details both return 404.
- All mutations remain Admin-only. Create accepts POST or root PUT; update uses
  PATCH with an ID. Delete marks `pap_status=3`. Restore accepts the legacy GET
  path and POST, both Admin-only.
- Nested editor references override raw foreign keys, including explicit null
  for clearing a relation. Approval flags accept booleans and legacy 0/1 values.
- Supplied authors are synchronized by submission-owned IDs. Missing/null
  authors preserve existing rows; an empty array removes all rows. Duplicate or
  foreign author IDs are rejected. Relational article/author writes use a
  transaction. Correspond/presenter flags are returned as booleans.
- `paper_page`, `paper_page_end`, `paper_abst_page`, `paper_external` and
  `pap_num` are mapped as nullable strings in both read contracts and admin writes.
- The React Creator sort alias `bloglar.updated_at` sorts by creator name.

## Remaining Gaps

- Member-owned submission editing from Laravel is intentionally not enabled:
  Members cannot read drafts/private fields or mutate submissions. Restoring
  that flow requires a separate ownership-based authorization design.
- Laravel keyword-parameter auto-creation, approval/comment email notifications,
  `icerikler` comment persistence and arbitrary `dosyalar` attachment collections
  are not implemented here. The inspected main React editor uses the explicit
  abstract/full-paper/poster/video relations, not those collections.
- Authors are returned in ID order. Reordering the editor array is not persisted
  because the current legacy author entity has no order column.
- The editor's parameter, user and author lookup endpoints and upload flow are
  outside this change. Creator relations assume imported `pap_creater` IDs map
  to the production `user` entity; absent creators return an empty user object.
- Local file URLs preserve the legacy `/uploads/dosya/...` route; cloud files
  use Drive preview URLs. File hosting, access permissions, migrated metadata
  and working download links require real-data verification.
- Writes follow the existing full-editor payload contract, not general-purpose
  sparse PATCH semantics for all scalar fields. Slugs are preserved/supplied,
  not regenerated with Laravel's `Str::slug` rules.
- `article` remains an alias for symposium submissions, not unrelated Laravel
  journal/import APIs. Pagination retains the .NET envelope (`data`,
  `currentPage`, `pageCount`, `itemCount`), not Laravel's full pagination metadata.

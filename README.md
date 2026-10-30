# Symposium Backend

Layered .NET 10 backend created from the `Template.Backend` architecture.

## Projects

- `Symposium.Models`: API contract records.
- `Symposium.Storage`: EF Core entities, DbContext, storage services.
- `Symposium.Query.Projections`: Projectable entity-to-record mappings.
- `Symposium.Services`: business service registration.
- `Symposium.Api.Server`: ASP.NET Core API host and controllers.

## Configuration

Use .NET 10. The host requires `database` (PostgreSQL), independent random `jwt`
and `hash` values of at least 32 characters, and `authentication:clients`.
See `src/Symposium.Api.Server/appsettings.example.json` for all configuration keys.
It contains placeholders only and is deliberately rejected until configured.
Set production values through environment variables (`:` becomes `__`) or an
untracked `src/Symposium.Api.Server/appsettings.json` on the server.

Each authentication client has a stable GUID `id`, a public `client_id` alias,
and a BCrypt `password` hash. The browser sends the corresponding public client
value, not the hash. These values are bundled in browser JavaScript and must not
be treated as confidential credentials. Admin uses `symposium-admin`; website
uses `symposium-website`. Set frontend client values to match server configuration.
User passwords, JWT keys, request-signing keys and storage credentials stay private.

Production additionally requires SMTP settings. WebDAV/CDN settings are required
for uploads; there is no Google Drive adapter dependency. Legacy Drive URLs can
still be returned for already stored attachments.

Set `legacyFiles__documentsRoot` to an existing absolute directory containing only
the public Laravel `public/doc` tree (or serve that tree through the reverse proxy).
It is served at `/api/doc`, preserving book/template links. No document tree is
bundled or copied by the deployment workflow; provision it separately. Do not point
this setting at the application root or private upload directories.

Registration is disabled unless `registration__enabled=true`. Registration always
creates a member, never an administrator. Configure `cors:origins` for browser
origins; development defaults allow localhost ports 3000 and 4000.

## Build and Test

```bash
dotnet build Symposium.slnx
dotnet test Symposium.slnx
dotnet run --project tests/ArticleCompatibility
```

Run the configured API on the local endpoint used by the frontends:

```bash
ASPNETCORE_ENVIRONMENT=Development ASPNETCORE_URLS=http://127.0.0.1:5088 \
dotnet run --project src/Symposium.Api.Server
```

The API base is `/api`; database readiness is `/api/healthz`, authentication is
`/api/token`. Startup does not migrate by default. Apply reviewed migrations
explicitly using the commands in `src/Symposium.Storage/Migrations/README.md`.
`databaseOptions__migrateOnStartup=true` is an explicit opt-in, not a deployment
requirement. Never point the initial PostgreSQL migration at the live MySQL DB.

## First Administrator

After applying the schema, set `bootstrap__email` and `bootstrap__password` through
the environment, then run the configured host in one-shot mode:

```bash
dotnet run --project src/Symposium.Api.Server -- --bootstrap-admin=true
```

The password must contain at least 12 characters. Existing accounts are rejected;
this command never promotes an existing member. Remove bootstrap values from the
environment afterward. There is no public administrator bootstrap route.

## Real HTTP Smoke Test

The smoke test starts a temporary API process, applies migrations and writes
randomized test users/articles. It only accepts a local database ending `_test`.
Use a disposable PostgreSQL instance, not application or production data:

```bash
SYMPOSIUM_TEST_DATABASE='Host=127.0.0.1;Port=55438;Database=symposium_test;Username=YOUR_LOCAL_ROLE' \
node tests/http-smoke.mjs
```

Run a build first. The API process stops automatically; test records remain in
the disposable database. Upload delivery and SMTP delivery require separately
configured external services and are not covered by this test.

## Laravel Scaffold

The identified functional gaps have now been implemented. See
`FUNCTIONAL_WORKFLOWS.md` for member access rules, comments/notifications, profile
contracts, file ownership, the new migration and end-to-end verification commands.

`LEGACY_SCAFFOLD.md` lists the models generated from `../symposium-laravel/app/Models`.
The generated legacy controllers expose admin-protected CRUD endpoints for tables
inferred from Eloquent `$table` and `$fillable` values. Article read endpoints also
provide a deliberately restricted public response for approved, non-deleted papers.

Confirmed foreign keys and long-text fields are now configured explicitly. The
PostgreSQL schema is a new schema, not an automatic Laravel data conversion.
Before replacing PHP in production, import and reconcile legacy data/IDs, preserve
attachment delivery, and verify all admin/customer workflows against real data.
See `DEPLOYMENT.md` for the Autosurf-style versioned release workflow, safe tag
helper, required Gitea secrets and production release gates.

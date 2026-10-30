# Architecture

This template implements a layered .NET 10 backend built around PostgreSQL, EF Core, and WebDav-based file storage. The architecture is split across five projects with clear separation of concerns.

## Project Structure

```
MyApp.Backend/
├── MyApp.slnx
├── .template.config/          # dotnet new template configuration
├── src/
│   ├── MyApp.Models/           # Plain record DTOs (no DB dependency)
│   ├── MyApp.Storage/          # EF Core DbContext, entities, services
│   ├── MyApp.Query.Projections/ # EF Core Projectables (DB → DTO mappings)
│   ├── MyApp.Services/          # Business logic service registration
│   └── MyApp.Api.Server/       # ASP.NET Core host, controllers, config
└── Symposium.Backend.Symposium.csproj  # dotnet new packaging
```

### Dependency Flow

```
Api.Server ──→ Models
            ──→ Storage ──→ Models
            ──→ Query.Projections ──→ Models, Storage
            ──→ Services ──→ Models, Query.Projections, Storage
```

- **Models** has zero external project references — it defines the API contract.
- **Storage** references Models (for entity-to-record projection inputs) and owns the DbContext.
- **Query.Projections** maps Storage entities to Model records via `[Projectable]` extension methods.
- **Services** is an extension point for business logic (empty by default, wired via `AddServices()`).
- **Api.Server** is the composition root that ties everything together.

## Key Patterns

### Entity–DTO Projection with `[Projectable]`

The template uses [EntityFrameworkCore.Projectables](https://github.com/koenbeuk/EntityFrameworkCore.Projectables) to translate C# extension methods into SQL projections. This keeps queries efficient by projecting only the columns you need.

**Example** — `UserExtensions.ToUser()`:

```csharp
[Projectable(NullConditionalRewriteSupport = NullConditionalRewriteSupport.Rewrite)]
public static User ToUser(this user a) => new User
{
    id = a.id,
    first_name = a.first_name,
    last_name = a.last_name,
    email = a.email,
    type = a.type,
    created_at = a.created_at,
    is_password_present = a.password != null,
    password_updated_at = a.password_set_at
};
```

Used in controllers as:

```csharp
await db.users.Where(a => a.id == id).Select(a => a.ToUser()).SingleAsync();
```

This compiles to a `SELECT` that only fetches the mapped columns — no lazy-loading, no N+1.

### PUT = Create or Update (Unified Upsert)

Controllers follow a single-action pattern for both `PUT` (create) and `PATCH` (update):

```csharp
[HttpPut]
[HttpPatch("{id}")]
public async Task<User> update_user(int? id, [FromBody] User User)
{
    if (id == null)
    {
        // Create
        user = new user { ... };
        db.users.Add(user);
    }
    else
    {
        // Update
        user = await db.users.SingleAsync(a => a.id == id);
    }

    // Apply shared field assignments...
    await db.SaveChangesAsync();
    return await db.users.Where(a => a.id == user.id).Select(a => a.ToUser()).SingleAsync();
}
```

The same action method handles both creation and mutation. `id == null` signals creation; a provided `id` signals update. This eliminates duplicate logic between create/update endpoints.

### MultiString — i18n at the Database Level

The `multi_string` owned type stores localized text as separate columns per language. Currently only `en` (English) is defined; add additional language columns as needed.

```csharp
public class multi_string
{
    public string? en { get; set; }
}
```

EF Core configuration (`ModelBuilderExtensions`) owns `multi_string` as a value object with:

- **`HasMultiString`** — marks columns as unlimited length (`max: true`), optional GIN index, or full-text search index.
- **`HasContent`** — owns a `content` type that wraps `multi_string body` plus `is_html` flag.

The `MultiStringExtensions` in Storage provide composable query operators:

```csharp
[Projectable]
public static bool Like(this multi_string ms, string query)
    => EF.Functions.ILike(ms.en, query);

[Projectable]
public static bool FullTextSearch(this multi_string ms, string query) =>
    EF.Functions.ToTsVector("english", ms.en).Matches(TsQuery("english", query))
    || ms.en == query;
```

To add a new language, add a property to `multi_string`, add it to the `exps` list in `ModelBuilderExtensions`, and add the corresponding property in the `MultiString` record in Models.

### Language System — Hierarchical Translations

Languages form a tree via `parent_id`. When resolving translations, the system walks up the parent chain, filling missing keys from ancestor languages:

```csharp
int? parent_id = id;
do
{
    var lang = await db.languages.Include(a => a.values).Where(a => a.id == parent_id).SingleOrDefaultAsync();
    foreach (var value in lang.values)
    {
        if (!values.ContainsKey(value.key))
            values[value.key] = value.value;
    }
    parent_id = lang.parent_id;
} while (parent_id != null);
```

`LanguageService.UpdateLanguageJson` precompiles each language into a JSON file stored via the FileService, producing a CDN-accessible translation bundle.

### File Storage — WebDav with URL Signing

Files are stored through a WebDav client (`WebDavClient`) supporting:

- **Upload** — direct stream upload or two-phase (start → upload → end with hash verification)
- **URL signing** — `UrlSigner` generates HMAC-signed URLs for upload and download, including query parameters (`u=1` for upload, `dl=1` for download)
- **Hash verification** — MD5 hashing during upload, with optional client-provided hash validation on completion
- **CDN integration** — `FileConverter` prepends the CDN base URL to `access_url` and `download_url` during JSON serialization

### Pagination

`ControllerExtensions.Paginate` provides cursor-free pagination via query parameters:

- `page` — 1-based page number (default: 1)
- `itemPerPage` — items per page (default: 15, pass `-1` for all)
- `sort` — field name for ordering
- `desc` — sort direction (default: true/descending)

Supports both direct entity pagination and projection-based pagination:

```csharp
// Direct
return await this.Paginate(db.languages, ...);

// With projection
return await this.Paginate(db.users.Where(...), a => a.Select(a => a.ToUser()));
```

### Authentication — JustAuthenticator

Auth is handled by [JustAuthenticator](https://github.com/0x53A/JustAuthenticator) with:

- **JWT Bearer tokens** — symmetric key signing, 1-hour expiration
- **Single role** — `UserType.ADMIN = 1024` maps to the `"Admin"` claim
- **Token persistence** — tokens stored in the `tokens` DB table with optional single-use (`shouldDispose`)
- **Password reset** — via signed URL flow using `IRequestSigning` (Tamturk.AspNetCore.RequestSigning)
- **SMTP** — production requires config; development falls back to `localhost:1025` (e.g., Mailpit)

### OpenTelemetry

The template ships with OTLP exporters wired up for all three signals:

| Signal    | Instrumentation                          |
|-----------|------------------------------------------|
| Logging   | `AddOtlpExporter()` on `WithLogging`   |
| Metrics   | ASP.NET Core, HTTP client                |
| Tracing   | ASP.NET Core, HTTP client, Npgsql        |

Resource attributes include `service.name`, `deployment.environment`, and `service.instance.id`.

### Systemd Integration

The host uses `UseSystemd()` on both the generic host and Kestrel:

```csharp
builder.Host.UseSystemd();
builder.WebHost.UseKestrel(a => a.UseSystemd());
```

This enables:
- Notify-based readiness detection (`sd_notify`)
- Console logger integration with journald
- Socket activation via Kestrel

## Configuration Keys

The app reads these from `appsettings.json` / environment variables:

| Key               | Purpose                                    |
|-------------------|--------------------------------------------|
| `jwt`             | Symmetric key for JWT signing              |
| `database`        | PostgreSQL connection string                |
| `hash`            | HMAC key for URL signing                   |
| `storage:server`  | WebDav server URL                          |
| `storage:username`| WebDav auth username                       |
| `storage:password`| WebDav auth password                       |
| `storage:cdn:url` | CDN base URL (prepended to file URLs)      |
| `storage:cdn:token`| CDN signing key                           |
| `smtp:server`     | SMTP host (production only)                |
| `smtp:port`       | SMTP port (production only)                |
| `smtp:user`       | SMTP username (production only)            |
| `smtp:pass`       | SMTP password (production only)            |
| `smtp:from`       | Sender email address (production only)     |

## Database Migrations

EF Core migrations are configured to live in the Storage project:

```csharp
.MigrationsAssembly("Symposium.Storage")
```

Run migrations with:

```bash
dotnet ef migrations add InitialCreate --project src/Symposium.Storage
```

The app auto-applies pending migrations on startup:

```csharp
using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetService<AppDbContext>().Database.Migrate();
}
```

## Common Pitfalls & Conventions

### Models vs. EF Entities — Two Separate Worlds

The template enforces a strict split between **API contract types** (Models) and **database entities** (Storage). They look similar but must follow different rules:

| Concern | Models (`Symposium.Models`) | Storage (`Symposium.Storage`) |
|---|---|---|
| Type kind | `record` | `class` |
| Collection type | `IReadOnlyCollection<T>` | `virtual List<T>` |
| Navigation properties | None (no relations) | Must be `virtual` for EF Core proxying |
| Nullability | `?` for optional fields | `?` for optional; non-null for FK-backed required fields |

**Rule:** When adding a new entity, write the Storage `class` first with `virtual` navigation properties, then write the Models `record` with only the fields you want exposed in the API.

### Nullability — Two Different Meanings

With `<Nullable>enable</Nullable>`, nullability annotations mean different things in each project:

**In Storage (entities):** `string` = required column (NOT NULL), `string?` = nullable column.

```csharp
// Storage entity — database schema
public string email { get; set; }       // NOT NULL in PostgreSQL
public string? first_name { get; set; } // NULLABLE in PostgreSQL
```

**In Models (records):** Most fields are nullable even when their DB counterpart is required. This is intentional — these records double as **request bodies** for PUT/PATCH endpoints. On creation, fields like `id` and `created_at` don't exist yet, so they must be nullable in the input model:

```csharp
// Models record — API contract / request body
public record User
{
    public int? id { get; init; }                   // null on create, populated on read
    public string? first_name { get; init; }         // optional
    public string email { get; init; }               // always required
    public DateTimeOffset? created_at { get; init; } // null on create, populated on read
    public UserType? type { get; init; }             // null on create (set server-side)
}
```

The controller then maps selectively — on create, only the user-provided fields are written; on update, only non-null fields are applied. This avoids needing separate request/response DTOs.

**Key takeaway:** Don't make Model record fields non-nullable just because the DB column is NOT NULL. If the field isn't provided by the client on creation, it should be `?` in the record.

### EF Core Relations — Define Once

A common mistake is defining both sides of a relation separately in `OnModelCreating`. Each relationship should be configured **once**, on the side that holds the foreign key:

```csharp
// ✅ Correct — defined once on the dependent entity
modelBuilder.Entity<language_value>(language_value =>
{
    language_value
        .HasOne(a => a.language)
        .WithMany(a => a.values)
        .HasForeignKey(a => a.language_id)
        .OnDelete(DeleteBehavior.Cascade);
});

// ❌ Wrong — also defining it on the principal side duplicates the relation
// modelBuilder.Entity<language>(language =>
// {
//     language.HasMany(a => a.values)   // ← REMOVE — already defined above
//         .WithOne(a => a.language)
//         .HasForeignKey(a => a.language_id);
// });
```

When both `HasOne(...).WithMany()` and `HasMany(...).WithOne()` are present for the same pair, EF Core treats them as **two separate relationships**, causing migration errors or unexpected cascade behaviors.

### Many-to-Many — Always Use a Join Entity

Never use EF Core's implicit many-to-many (`HasMany().WithMany()`). Always model the join table explicitly with a join entity. This gives you control over ordering, additional columns, and clear FK configuration:

```csharp
// Storage entities
public class skill
{
    [Key] public int id { get; set; }
    public multi_string title { get; set; }
    public int priority { get; set; }
    public DateTimeOffset created_at { get; set; }
    public DateTimeOffset? updated_at { get; set; }

    // optional — only if you need to navigate from skill to its jobs
    public virtual List<job_skill>? jobs { get; set; }
}

public class job
{
    [Key] public int id { get; set; }
    // ...
    public DateTimeOffset created_at { get; set; }
    public DateTimeOffset? updated_at { get; set; }

    public virtual List<job_skill> skills { get; set; }
}

public class job_skill
{
    public int priority { get; set; }      // ordering column
    public int job_id { get; set; }
    public virtual job job { get; set; }
    public int skill_id { get; set; }
    public virtual skill skill { get; set; }
}

// Configuration — composite key, relations defined once each
modelBuilder.Entity<job_skill>(job_skill =>
{
    job_skill
        .HasKey(a => new { a.job_id, a.skill_id });

    job_skill
        .HasOne(a => a.job)
        .WithMany(a => a.skills)
        .HasForeignKey(a => a.job_id)
        .OnDelete(DeleteBehavior.Cascade);

    job_skill
        .HasOne(a => a.skill)
        .WithMany()                    // or .WithMany(a => a.jobs) if bidirectional
        .HasForeignKey(a => a.skill_id)
        .OnDelete(DeleteBehavior.Cascade);
});
```

**In Models and Projections**, the join entity is hidden. The record exposes `IReadOnlyCollection<Skill>` directly — consumers never see `job_skill`:

```csharp
public record Job
{
    public int? id { get; init; }
    public IReadOnlyCollection<Skill>? skills { get; init; }
    public DateTimeOffset? created_at { get; init; }
    public DateTimeOffset? updated_at { get; init; }
}

public record Skill
{
    public int? id { get; init; }
    public MultiString title { get; init; }
    public DateTimeOffset? created_at { get; init; }
    public DateTimeOffset? updated_at { get; init; }
}
```

The projection traverses the join entity and orders by priority:

```csharp
[Projectable(NullConditionalRewriteSupport = NullConditionalRewriteSupport.Rewrite)]
public static Job ToJob(this job a) => new Job
{
    id = a.id,
    skills = a.skills
        .OrderBy(s => s.priority)        // respect ordering
        .Select(s => s.skill.ToSkill())
        .ToList(),
    created_at = a.created_at,
    updated_at = a.updated_at
};
```

**Persisting relations** on create/update requires `.Include()` (since you're modifying the join collection, not projecting):

```csharp
if (id == null)
{
    job = new job
    {
        skills = [],                    // important: initialize empty collection
        created_at = DateTimeOffset.UtcNow
    };
}
else
{
    job = await db.jobs
        .Include(a => a.skills)         // important: load join entities
        .SingleAsync();
}

if (Job.skills != null)
{
    // remove skills no longer present
    job.skills.RemoveAll(a => !Job.skills.Any(b => b.id == a.skill_id));

    foreach (var (index, skill) in Job.skills.Index())
    {
        var existing = job.skills.SingleOrDefault(a => a.skill_id == skill.id);
        if (existing == null)
        {
            existing = new job_skill { skill_id = skill.id!.Value };
            job.skills.Add(existing);
        }

        existing.priority = index;   // always set, even on creation
    }
}
```

### Virtual Navigation Properties

All navigation properties on Storage entities must be marked `virtual`:

```csharp
// ✅ Correct
public virtual vfile? icon { get; set; }
public virtual language? parent { get; set; }
public virtual List<language_value> values { get; set; }

// ❌ Wrong — EF Core cannot create change-tracking proxies
public vfile? icon { get; set; }
```

Without `virtual`, EF Core cannot create change-tracking proxies, which breaks lazy loading and change tracking in certain scenarios.

### Projections Replace Includes

When using `[Projectable]` extension methods, **never** use `.Include()` or `.ThenInclude()`:

```csharp
// ✅ Correct — projection traverses relations in SQL automatically
return await db.languages
    .Where(a => a.id == id)
    .Select(a => a.ToLanguage())
    .SingleAsync();

// ❌ Wrong — .Include() forces eager loading, then .Select() re-projection causes N+1
return await db.languages
    .Include(a => a.icon)          // unnecessary
    .ThenInclude(a => a.precompiled_json) // unnecessary
    .Where(a => a.id == id)
    .Select(a => a.ToLanguage())
    .SingleAsync();
```

Projections are translated to SQL. The `[Projectable]` extension method `a.icon.ToFile()` already becomes a `LEFT JOIN` in the generated query. Including the navigation property separately causes EF Core to load the full entity graph first, discarding most of it during projection.

**Exception:** When persisting many-to-many relations, you must `.Include()` the join collection before modifying it (see the [Many-to-Many](#many-to-many--always-use-a-join-entity) section). This is the only case where `.Include()` is used in this template — for write operations, not reads.

### Projections Must Stay in IQueryable

Projection methods marked with `[Projectable]` only work inside `IQueryable<T>`. If you break out of the query pipeline, the projection cannot be translated to SQL:

```csharp
// ✅ Correct — stays in IQueryable throughout
return await this.Paginate(
    db.users.Where(a => query == null || a.name.Like($"%{query}%")),
    q => q.Select(a => a.ToUser())
);

// ❌ Wrong — .AsEnumerable() or .ToList() forces client-side evaluation
return db.users
    .Where(...)
    .AsEnumerable()    // ← breaks projection translation
    .Select(a => a.ToUser())  // this now runs in memory, not SQL
```

This causes N+1 queries, loads entire entities unnecessarily, and may throw runtime exceptions if the projection references navigation properties that weren't loaded.

### Creating Migrations

After modifying any Storage entity or `OnModelCreating` configuration, always create a migration:

```bash
dotnet ef migrations add DescribeYourChange --project src/Symposium.Storage
```

The migration assembly is set to `Symposium.Storage` via `.MigrationsAssembly("Symposium.Storage")` in the DbContext configuration. Migrations auto-apply on startup via `Database.Migrate()`, so the next app launch will apply any pending ones.

**Common mistake:** modifying entity classes but forgetting to create a migration means the database schema won't match the model, causing runtime errors.

## Using as a Symposium

```bash
# Install
dotnet new install Subfyi.Backend.Symposium

# Create a new project
dotnet new subfyi-backend -n MyProject

# Run
cd MyProject
dotnet run --project src/MyProject.Api.Server
```

The `SourceName` is `Symposium` — all namespaces, file names, and content references (`Symposium.*`) are automatically replaced with your project name.
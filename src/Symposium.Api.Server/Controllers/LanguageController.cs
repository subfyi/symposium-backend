using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace Symposium.Api.Server.Controllers;

public record LanguageKey(string key, string description);

[Authorize(Roles = "Admin")]
[Route("language")]
public class LanguageController : ControllerBase
{
    private readonly AppDbContext db;
    private readonly IOutputCacheStore cache;

    public LanguageController(AppDbContext db, IOutputCacheStore cache)
    {
        this.db = db;
        this.cache = cache;
    }

    [HttpGet("keys")]
    public async Task<List<LanguageKey>> keys()
    {
        return await db.language_keys
            .Select(a => new LanguageKey(a.key, a.description))
            .ToListAsync();
    }

    /// <summary>
    /// Anonymous endpoint that returns merged language messages
    /// (with parent fallback). Cached permanently (365 days) with the
    /// "languages" tag, evicted on any language update. Sets Last-Modified
    /// for client-side conditional requests (304 Not Modified).
    /// </summary>
    [AllowAnonymous]
    [HttpGet("{id}/messages.json")]
    [OutputCache(PolicyName = "Languages")]
    public async Task<IReadOnlyDictionary<string, string>> getMessagesJson(int id)
    {
        var keys = await db.language_keys
            .Select(a => a.key)
            .ToListAsync();

        Dictionary<string, string> values = new();
        DateTimeOffset lastModified = DateTimeOffset.MinValue;

        int? parent_id = id;
        do
        {
            var lang = await db.languages
                .Include(a => a.values)
                .Where(a => a.id == parent_id)
                .SingleOrDefaultAsync();

            var langDate = lang.updated_at ?? lang.created_at;
            if (langDate > lastModified)
                lastModified = langDate;

            foreach (var value in lang.values)
            {
                if (values.ContainsKey(value.key))
                    continue;

                values[value.key] = value.value;
                if (values.Count == keys.Count)
                    goto end;
            }

            parent_id = lang.parent_id;
        } while (parent_id != null);

        foreach (var key in keys)
        {
            if (!values.ContainsKey(key))
                values[key] = "";
        }

    end:
        Response.Headers.LastModified = lastModified.ToString("R");

        return values;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<Pagination<Language>> all(string query = null)
    {
        return await this.Paginate(db.languages
            .Where(a => query == null || EF.Functions.ILike(a.name, $"%{query}%"))
            , a => a.Select(a => a.ToLanguageWoParent()));
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<Language> get(int id)
    {
        var lang = await db.languages
            .Where(a => a.id == id)
            .Select(a => a.ToLanguage())
            .SingleAsync();

        var values = await db.language_values
            .Where(a => a.language_id == lang.id)
            .Select(a => new { a.language_key.key, a.value })
            .AsAsyncEnumerable()
            .ToDictionaryAsync(a => a.key, a => a.value);

        return lang with
        {
            values = values
        };
    }

    [HttpDelete("{id}")]
    public async Task delete(int? id)
    {
        await db.languages
            .Where(a => a.id == id)
            .ExecuteDeleteAsync();

        await cache.EvictByTagAsync("languages", default);
    }

    [HttpPut]
    [HttpPatch("{id}")]
    public async Task<Language> update(int? id, [FromBody] Language Language)
    {
        language language;
        bool changesMade = false;

        if (id == null)
        {
            language = new language
            {
                values = new List<language_value>(),
                created_at = DateTimeOffset.UtcNow
            };

            db.languages.Add(language);
            changesMade = true;
        }
        else
        {
            language = await db.languages
                .Include(a => a.values)
                .SingleAsync(a => a.id == id);

            language.updated_at = DateTimeOffset.UtcNow;
        }

        language.icon_id = Language.icon?.id;
        language.enabled = Language.enabled ?? false;
        language.priority = Language.priority ?? 0;

        if (Language.parent?.id != null)
        {
            language.parent_id = Language.parent.id.Value;
        }

        language.name = Language.name;
        language.language_name = Language.language_name;
        language.language_code = Language.language_code;

        if (Language.values != null)
        {
            var filtered = Language.values.Where(a => !string.IsNullOrWhiteSpace(a.Value))
                .ToDictionary(a => a.Key, a => a.Value);

            int del = language.values.RemoveAll(a => !filtered.ContainsKey(a.key));
            if (del > 0)
                changesMade = true;

            foreach (var (key, value) in filtered)
            {
                var _value = language.values.FirstOrDefault(a => a.key == key);
                if (_value != null)
                {
                    if (_value.value != value)
                    {
                        changesMade = true;
                        _value.value = value;
                        _value.updated_at = DateTimeOffset.UtcNow;
                    }
                }
                else
                {
                    changesMade = true;
                    language.values
                        .Add(new language_value
                        {
                            key = key,
                            value = value,
                            created_at = DateTimeOffset.UtcNow
                        });
                }
            }
        }

        await db.SaveChangesAsync();

        if (changesMade)
        {
            await cache.EvictByTagAsync("languages", default);
        }

        return await get(language.id);
    }
}
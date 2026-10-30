using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symposium.Api.Server.Utils;
using Symposium.Models;
using Symposium.Query.Projections;
using Symposium.Storage;

namespace Symposium.Api.Server.Controllers.Legacy;

[Authorize(Roles = "Admin")]
[Route("journal-to-countrie")]
public class JournalToCountrieController : ControllerBase
{
    private readonly AppDbContext db;

    public JournalToCountrieController(AppDbContext db)
    {
        this.db = db;
    }

    [HttpGet]
    public async Task<Pagination<JournalToCountrie>> all(string? query = null)
    {
        return await this.Paginate(db.Set<journal_to_countrie>(), a => a.Select(a => a.ToJournalToCountrie()));
    }

    [HttpGet("{id}")]
    public async Task<JournalToCountrie> get(int id)
    {
        return await db.Set<journal_to_countrie>()
            .Where(a => a.id == id)
            .Select(a => a.ToJournalToCountrie())
            .SingleAsync();
    }

    [HttpDelete("{id}")]
    public async Task delete(int id)
    {
        await db.Set<journal_to_countrie>()
            .Where(a => a.id == id)
            .ExecuteDeleteAsync();
    }

    [HttpPut]
    [HttpPatch("{id}")]
    public async Task<JournalToCountrie> upsert(int? id, [FromBody] JournalToCountrie model)
    {
        journal_to_countrie entity;

        if (id == null)
        {
            entity = new journal_to_countrie
            {
                created_at = DateTimeOffset.UtcNow
            };
            db.Set<journal_to_countrie>().Add(entity);
        }
        else
        {
            entity = await db.Set<journal_to_countrie>().SingleAsync(a => a.id == id);
            entity.updated_at = DateTimeOffset.UtcNow;
        }

        entity.journal_id = model.journal_id;
        entity.countrie_id = model.countrie_id;

        await db.SaveChangesAsync();
        return await get(entity.id);
    }
}

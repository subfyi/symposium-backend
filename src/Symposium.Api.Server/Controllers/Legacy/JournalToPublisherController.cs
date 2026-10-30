using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symposium.Api.Server.Utils;
using Symposium.Models;
using Symposium.Query.Projections;
using Symposium.Storage;

namespace Symposium.Api.Server.Controllers.Legacy;

[Authorize(Roles = "Admin")]
[Route("journal-to-publisher")]
public class JournalToPublisherController : ControllerBase
{
    private readonly AppDbContext db;

    public JournalToPublisherController(AppDbContext db)
    {
        this.db = db;
    }

    [HttpGet]
    public async Task<Pagination<JournalToPublisher>> all(string? query = null)
    {
        return await this.Paginate(db.Set<journal_to_publisher>(), a => a.Select(a => a.ToJournalToPublisher()));
    }

    [HttpGet("{id}")]
    public async Task<JournalToPublisher> get(int id)
    {
        return await db.Set<journal_to_publisher>()
            .Where(a => a.id == id)
            .Select(a => a.ToJournalToPublisher())
            .SingleAsync();
    }

    [HttpDelete("{id}")]
    public async Task delete(int id)
    {
        await db.Set<journal_to_publisher>()
            .Where(a => a.id == id)
            .ExecuteDeleteAsync();
    }

    [HttpPut]
    [HttpPatch("{id}")]
    public async Task<JournalToPublisher> upsert(int? id, [FromBody] JournalToPublisher model)
    {
        journal_to_publisher entity;

        if (id == null)
        {
            entity = new journal_to_publisher
            {
                created_at = DateTimeOffset.UtcNow
            };
            db.Set<journal_to_publisher>().Add(entity);
        }
        else
        {
            entity = await db.Set<journal_to_publisher>().SingleAsync(a => a.id == id);
            entity.updated_at = DateTimeOffset.UtcNow;
        }

        entity.journal_id = model.journal_id;
        entity.publisher_id = model.publisher_id;

        await db.SaveChangesAsync();
        return await get(entity.id);
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symposium.Api.Server.Utils;
using Symposium.Models;
using Symposium.Query.Projections;
using Symposium.Storage;

namespace Symposium.Api.Server.Controllers.Legacy;

[Authorize(Roles = "Admin")]
[Route("journal-to-subject")]
public class JournalToSubjectController : ControllerBase
{
    private readonly AppDbContext db;

    public JournalToSubjectController(AppDbContext db)
    {
        this.db = db;
    }

    [HttpGet]
    public async Task<Pagination<JournalToSubject>> all(string? query = null)
    {
        return await this.Paginate(db.Set<journal_to_subject>(), a => a.Select(a => a.ToJournalToSubject()));
    }

    [HttpGet("{id}")]
    public async Task<JournalToSubject> get(int id)
    {
        return await db.Set<journal_to_subject>()
            .Where(a => a.id == id)
            .Select(a => a.ToJournalToSubject())
            .SingleAsync();
    }

    [HttpDelete("{id}")]
    public async Task delete(int id)
    {
        await db.Set<journal_to_subject>()
            .Where(a => a.id == id)
            .ExecuteDeleteAsync();
    }

    [HttpPut]
    [HttpPatch("{id}")]
    public async Task<JournalToSubject> upsert(int? id, [FromBody] JournalToSubject model)
    {
        journal_to_subject entity;

        if (id == null)
        {
            entity = new journal_to_subject
            {
                created_at = DateTimeOffset.UtcNow
            };
            db.Set<journal_to_subject>().Add(entity);
        }
        else
        {
            entity = await db.Set<journal_to_subject>().SingleAsync(a => a.id == id);
            entity.updated_at = DateTimeOffset.UtcNow;
        }

        entity.journal_id = model.journal_id;
        entity.subject_id = model.subject_id;

        await db.SaveChangesAsync();
        return await get(entity.id);
    }
}

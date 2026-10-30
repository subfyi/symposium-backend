using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symposium.Api.Server.Utils;
using Symposium.Models;
using Symposium.Query.Projections;
using Symposium.Storage;

namespace Symposium.Api.Server.Controllers.Legacy;

[Authorize(Roles = "Admin")]
[Route("journal-info")]
public class JournalInfoController : ControllerBase
{
    private readonly AppDbContext db;

    public JournalInfoController(AppDbContext db)
    {
        this.db = db;
    }

    [HttpGet]
    public async Task<Pagination<JournalInfo>> all(string? query = null)
    {
        return await this.Paginate(db.Set<journal_info>().Where(a => query == null || EF.Functions.ILike(a.title, $"%{query}%")), a => a.Select(a => a.ToJournalInfo()));
    }

    [HttpGet("{id}")]
    public async Task<JournalInfo> get(int id)
    {
        return await db.Set<journal_info>()
            .Where(a => a.id == id)
            .Select(a => a.ToJournalInfo())
            .SingleAsync();
    }

    [HttpDelete("{id}")]
    public async Task delete(int id)
    {
        await db.Set<journal_info>()
            .Where(a => a.id == id)
            .ExecuteDeleteAsync();
    }

    [HttpPut]
    [HttpPatch("{id}")]
    public async Task<JournalInfo> upsert(int? id, [FromBody] JournalInfo model)
    {
        journal_info entity;

        if (id == null)
        {
            entity = new journal_info
            {
                created_at = DateTimeOffset.UtcNow
            };
            db.Set<journal_info>().Add(entity);
        }
        else
        {
            entity = await db.Set<journal_info>().SingleAsync(a => a.id == id);
            entity.updated_at = DateTimeOffset.UtcNow;
        }

        entity.journal_id = model.journal_id;
        entity.d_id = model.d_id;
        entity.lang_id = model.lang_id;
        entity.title = model.title;
        entity.subtitle = model.subtitle;
        entity.description = model.description;
        entity.title_abbr = model.title_abbr;
        entity.footer_text = model.footer_text;
        entity.mail_signature = model.mail_signature;
        entity.aim = model.aim;
        entity.scope = model.scope;
        entity.writing_rules = model.writing_rules;
        entity.policy = model.policy;

        await db.SaveChangesAsync();
        return await get(entity.id);
    }
}

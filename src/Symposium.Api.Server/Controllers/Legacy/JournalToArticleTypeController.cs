using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symposium.Api.Server.Utils;
using Symposium.Models;
using Symposium.Query.Projections;
using Symposium.Storage;

namespace Symposium.Api.Server.Controllers.Legacy;

[Authorize(Roles = "Admin")]
[Route("journal-to-article-type")]
public class JournalToArticleTypeController : ControllerBase
{
    private readonly AppDbContext db;

    public JournalToArticleTypeController(AppDbContext db)
    {
        this.db = db;
    }

    [HttpGet]
    public async Task<Pagination<JournalToArticleType>> all(string? query = null)
    {
        return await this.Paginate(db.Set<journal_to_article_type>(), a => a.Select(a => a.ToJournalToArticleType()));
    }

    [HttpGet("{id}")]
    public async Task<JournalToArticleType> get(int id)
    {
        return await db.Set<journal_to_article_type>()
            .Where(a => a.id == id)
            .Select(a => a.ToJournalToArticleType())
            .SingleAsync();
    }

    [HttpDelete("{id}")]
    public async Task delete(int id)
    {
        await db.Set<journal_to_article_type>()
            .Where(a => a.id == id)
            .ExecuteDeleteAsync();
    }

    [HttpPut]
    [HttpPatch("{id}")]
    public async Task<JournalToArticleType> upsert(int? id, [FromBody] JournalToArticleType model)
    {
        journal_to_article_type entity;

        if (id == null)
        {
            entity = new journal_to_article_type
            {
                created_at = DateTimeOffset.UtcNow
            };
            db.Set<journal_to_article_type>().Add(entity);
        }
        else
        {
            entity = await db.Set<journal_to_article_type>().SingleAsync(a => a.id == id);
            entity.updated_at = DateTimeOffset.UtcNow;
        }

        entity.journal_id = model.journal_id;
        entity.article_type_id = model.article_type_id;

        await db.SaveChangesAsync();
        return await get(entity.id);
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symposium.Api.Server.Utils;
using Symposium.Models;
using Symposium.Query.Projections;
using Symposium.Storage;

namespace Symposium.Api.Server.Controllers.Legacy;

[Authorize(Roles = "Admin")]
[Route("article-to-citation")]
public class ArticleToCitationController : ControllerBase
{
    private readonly AppDbContext db;

    public ArticleToCitationController(AppDbContext db)
    {
        this.db = db;
    }

    [HttpGet]
    public async Task<Pagination<ArticleToCitation>> all(string? query = null)
    {
        return await this.Paginate(db.Set<article_to_citation>(), a => a.Select(a => a.ToArticleToCitation()));
    }

    [HttpGet("{id}")]
    public async Task<ArticleToCitation> get(int id)
    {
        return await db.Set<article_to_citation>()
            .Where(a => a.id == id)
            .Select(a => a.ToArticleToCitation())
            .SingleAsync();
    }

    [HttpDelete("{id}")]
    public async Task delete(int id)
    {
        await db.Set<article_to_citation>()
            .Where(a => a.id == id)
            .ExecuteDeleteAsync();
    }

    [HttpPut]
    [HttpPatch("{id}")]
    public async Task<ArticleToCitation> upsert(int? id, [FromBody] ArticleToCitation model)
    {
        article_to_citation entity;

        if (id == null)
        {
            entity = new article_to_citation
            {
                created_at = DateTimeOffset.UtcNow
            };
            db.Set<article_to_citation>().Add(entity);
        }
        else
        {
            entity = await db.Set<article_to_citation>().SingleAsync(a => a.id == id);
            entity.updated_at = DateTimeOffset.UtcNow;
        }

        entity.article_id = model.article_id;
        entity.citation_id = model.citation_id;

        await db.SaveChangesAsync();
        return await get(entity.id);
    }
}

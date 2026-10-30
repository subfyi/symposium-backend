using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symposium.Api.Server.Utils;
using Symposium.Models;
using Symposium.Query.Projections;
using Symposium.Storage;

namespace Symposium.Api.Server.Controllers.Legacy;

[Authorize(Roles = "Admin")]
[Route("article-to-author")]
public class ArticleToAuthorController : ControllerBase
{
    private readonly AppDbContext db;

    public ArticleToAuthorController(AppDbContext db)
    {
        this.db = db;
    }

    [HttpGet]
    public async Task<Pagination<ArticleToAuthor>> all(string? query = null)
    {
        return await this.Paginate(db.Set<article_to_author>(), a => a.Select(a => a.ToArticleToAuthor()));
    }

    [HttpGet("{id}")]
    public async Task<ArticleToAuthor> get(int id)
    {
        return await db.Set<article_to_author>()
            .Where(a => a.id == id)
            .Select(a => a.ToArticleToAuthor())
            .SingleAsync();
    }

    [HttpDelete("{id}")]
    public async Task delete(int id)
    {
        await db.Set<article_to_author>()
            .Where(a => a.id == id)
            .ExecuteDeleteAsync();
    }

    [HttpPut]
    [HttpPatch("{id}")]
    public async Task<ArticleToAuthor> upsert(int? id, [FromBody] ArticleToAuthor model)
    {
        article_to_author entity;

        if (id == null)
        {
            entity = new article_to_author
            {
                created_at = DateTimeOffset.UtcNow
            };
            db.Set<article_to_author>().Add(entity);
        }
        else
        {
            entity = await db.Set<article_to_author>().SingleAsync(a => a.id == id);
            entity.updated_at = DateTimeOffset.UtcNow;
        }

        entity.article_id = model.article_id;
        entity.author_id = model.author_id;
        entity.order = model.order;

        await db.SaveChangesAsync();
        return await get(entity.id);
    }
}

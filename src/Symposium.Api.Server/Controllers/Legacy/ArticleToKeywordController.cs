using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symposium.Api.Server.Utils;
using Symposium.Models;
using Symposium.Query.Projections;
using Symposium.Storage;

namespace Symposium.Api.Server.Controllers.Legacy;

[Authorize(Roles = "Admin")]
[Route("article-to-keyword")]
public class ArticleToKeywordController : ControllerBase
{
    private readonly AppDbContext db;

    public ArticleToKeywordController(AppDbContext db)
    {
        this.db = db;
    }

    [HttpGet]
    public async Task<Pagination<ArticleToKeyword>> all(string? query = null)
    {
        return await this.Paginate(db.Set<article_to_keyword>(), a => a.Select(a => a.ToArticleToKeyword()));
    }

    [HttpGet("{id}")]
    public async Task<ArticleToKeyword> get(int id)
    {
        return await db.Set<article_to_keyword>()
            .Where(a => a.id == id)
            .Select(a => a.ToArticleToKeyword())
            .SingleAsync();
    }

    [HttpDelete("{id}")]
    public async Task delete(int id)
    {
        await db.Set<article_to_keyword>()
            .Where(a => a.id == id)
            .ExecuteDeleteAsync();
    }

    [HttpPut]
    [HttpPatch("{id}")]
    public async Task<ArticleToKeyword> upsert(int? id, [FromBody] ArticleToKeyword model)
    {
        article_to_keyword entity;

        if (id == null)
        {
            entity = new article_to_keyword
            {
                created_at = DateTimeOffset.UtcNow
            };
            db.Set<article_to_keyword>().Add(entity);
        }
        else
        {
            entity = await db.Set<article_to_keyword>().SingleAsync(a => a.id == id);
            entity.updated_at = DateTimeOffset.UtcNow;
        }

        entity.article_id = model.article_id;
        entity.keyword_id = model.keyword_id;

        await db.SaveChangesAsync();
        return await get(entity.id);
    }
}

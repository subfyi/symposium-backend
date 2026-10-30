using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symposium.Api.Server.Utils;
using Symposium.Models;
using Symposium.Query.Projections;
using Symposium.Storage;

namespace Symposium.Api.Server.Controllers.Legacy;

[Authorize(Roles = "Admin")]
[Route("article-to-article-type")]
public class ArticleToArticleTypeController : ControllerBase
{
    private readonly AppDbContext db;

    public ArticleToArticleTypeController(AppDbContext db)
    {
        this.db = db;
    }

    [HttpGet]
    public async Task<Pagination<ArticleToArticleType>> all(string? query = null)
    {
        return await this.Paginate(db.Set<article_to_article_type>(), a => a.Select(a => a.ToArticleToArticleType()));
    }

    [HttpGet("{id}")]
    public async Task<ArticleToArticleType> get(int id)
    {
        return await db.Set<article_to_article_type>()
            .Where(a => a.id == id)
            .Select(a => a.ToArticleToArticleType())
            .SingleAsync();
    }

    [HttpDelete("{id}")]
    public async Task delete(int id)
    {
        await db.Set<article_to_article_type>()
            .Where(a => a.id == id)
            .ExecuteDeleteAsync();
    }

    [HttpPut]
    [HttpPatch("{id}")]
    public async Task<ArticleToArticleType> upsert(int? id, [FromBody] ArticleToArticleType model)
    {
        article_to_article_type entity;

        if (id == null)
        {
            entity = new article_to_article_type
            {
                created_at = DateTimeOffset.UtcNow
            };
            db.Set<article_to_article_type>().Add(entity);
        }
        else
        {
            entity = await db.Set<article_to_article_type>().SingleAsync(a => a.id == id);
            entity.updated_at = DateTimeOffset.UtcNow;
        }

        entity.article_id = model.article_id;
        entity.article_type_id = model.article_type_id;

        await db.SaveChangesAsync();
        return await get(entity.id);
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symposium.Api.Server.Utils;
using Symposium.Models;
using Symposium.Query.Projections;
using Symposium.Storage;

namespace Symposium.Api.Server.Controllers.Legacy;

[Authorize(Roles = "Admin")]
[Route("article-types")]
[Route("articletype")]
public class ArticleTypesController : ControllerBase
{
    private readonly AppDbContext db;

    public ArticleTypesController(AppDbContext db)
    {
        this.db = db;
    }

    [HttpGet]
    public async Task<Pagination<ArticleTypes>> all(string? query = null)
    {
        return await this.Paginate(db.Set<article_types>().Where(a => query == null || EF.Functions.ILike(a.name, $"%{query}%")), a => a.Select(a => a.ToArticleTypes()));
    }

    [HttpGet("{id}")]
    public async Task<ArticleTypes> get(int id)
    {
        return await db.Set<article_types>()
            .Where(a => a.id == id)
            .Select(a => a.ToArticleTypes())
            .SingleAsync();
    }

    [HttpDelete("{id}")]
    public async Task delete(int id)
    {
        await db.Set<article_types>()
            .Where(a => a.id == id)
            .ExecuteDeleteAsync();
    }

    [HttpPut]
    [HttpPatch("{id}")]
    public async Task<ArticleTypes> upsert(int? id, [FromBody] ArticleTypes model)
    {
        article_types entity;

        if (id == null)
        {
            entity = new article_types
            {
                created_at = DateTimeOffset.UtcNow
            };
            db.Set<article_types>().Add(entity);
        }
        else
        {
            entity = await db.Set<article_types>().SingleAsync(a => a.id == id);
            entity.updated_at = DateTimeOffset.UtcNow;
        }

        entity.d_id = model.d_id;
        entity.lang_id = model.lang_id;
        entity.name = model.name;

        await db.SaveChangesAsync();
        return await get(entity.id);
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symposium.Api.Server.Utils;
using Symposium.Models;
using Symposium.Query.Projections;
using Symposium.Storage;

namespace Symposium.Api.Server.Controllers.Legacy;

[Authorize(Roles = "Admin")]
[Route("article-info")]
public class ArticleInfoController : ControllerBase
{
    private readonly AppDbContext db;

    public ArticleInfoController(AppDbContext db)
    {
        this.db = db;
    }

    [HttpGet]
    public async Task<Pagination<ArticleInfo>> all(string? query = null)
    {
        return await this.Paginate(db.Set<article_info>().Where(a => query == null || EF.Functions.ILike(a.title, $"%{query}%")), a => a.Select(a => a.ToArticleInfo()));
    }

    [HttpGet("{id}")]
    public async Task<ArticleInfo> get(int id)
    {
        return await db.Set<article_info>()
            .Where(a => a.id == id)
            .Select(a => a.ToArticleInfo())
            .SingleAsync();
    }

    [HttpDelete("{id}")]
    public async Task delete(int id)
    {
        await db.Set<article_info>()
            .Where(a => a.id == id)
            .ExecuteDeleteAsync();
    }

    [HttpPut]
    [HttpPatch("{id}")]
    public async Task<ArticleInfo> upsert(int? id, [FromBody] ArticleInfo model)
    {
        article_info entity;

        if (id == null)
        {
            entity = new article_info
            {
                created_at = DateTimeOffset.UtcNow
            };
            db.Set<article_info>().Add(entity);
        }
        else
        {
            entity = await db.Set<article_info>().SingleAsync(a => a.id == id);
            entity.updated_at = DateTimeOffset.UtcNow;
        }

        entity.d_id = model.d_id;
        entity.v_id = model.v_id;
        entity.article_id = model.article_id;
        entity.lang_id = model.lang_id;
        entity.title = model.title;
        entity.@abstract = model.@abstract;
        entity.created_by = model.created_by;

        await db.SaveChangesAsync();
        return await get(entity.id);
    }
}

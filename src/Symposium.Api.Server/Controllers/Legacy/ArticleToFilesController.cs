using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symposium.Api.Server.Utils;
using Symposium.Models;
using Symposium.Query.Projections;
using Symposium.Storage;

namespace Symposium.Api.Server.Controllers.Legacy;

[Authorize(Roles = "Admin")]
[Route("article-to-files")]
public class ArticleToFilesController : ControllerBase
{
    private readonly AppDbContext db;

    public ArticleToFilesController(AppDbContext db)
    {
        this.db = db;
    }

    [HttpGet]
    public async Task<Pagination<ArticleToFiles>> all(string? query = null)
    {
        return await this.Paginate(db.Set<article_to_files>(), a => a.Select(a => a.ToArticleToFiles()));
    }

    [HttpGet("{id}")]
    public async Task<ArticleToFiles> get(int id)
    {
        return await db.Set<article_to_files>()
            .Where(a => a.id == id)
            .Select(a => a.ToArticleToFiles())
            .SingleAsync();
    }

    [HttpDelete("{id}")]
    public async Task delete(int id)
    {
        await db.Set<article_to_files>()
            .Where(a => a.id == id)
            .ExecuteDeleteAsync();
    }

    [HttpPut]
    [HttpPatch("{id}")]
    public async Task<ArticleToFiles> upsert(int? id, [FromBody] ArticleToFiles model)
    {
        article_to_files entity;

        if (id == null)
        {
            entity = new article_to_files
            {
                created_at = DateTimeOffset.UtcNow
            };
            db.Set<article_to_files>().Add(entity);
        }
        else
        {
            entity = await db.Set<article_to_files>().SingleAsync(a => a.id == id);
            entity.updated_at = DateTimeOffset.UtcNow;
        }

        entity.article_id = model.article_id;
        entity.file_id = model.file_id;

        await db.SaveChangesAsync();
        return await get(entity.id);
    }
}

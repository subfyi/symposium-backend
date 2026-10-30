using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symposium.Api.Server.Utils;
using Symposium.Models;
using Symposium.Query.Projections;
using Symposium.Storage;

namespace Symposium.Api.Server.Controllers.Legacy;

[Authorize(Roles = "Admin")]
[Route("article-to-subject")]
public class ArticleToSubjectController : ControllerBase
{
    private readonly AppDbContext db;

    public ArticleToSubjectController(AppDbContext db)
    {
        this.db = db;
    }

    [HttpGet]
    public async Task<Pagination<ArticleToSubject>> all(string? query = null)
    {
        return await this.Paginate(db.Set<article_to_subject>(), a => a.Select(a => a.ToArticleToSubject()));
    }

    [HttpGet("{id}")]
    public async Task<ArticleToSubject> get(int id)
    {
        return await db.Set<article_to_subject>()
            .Where(a => a.id == id)
            .Select(a => a.ToArticleToSubject())
            .SingleAsync();
    }

    [HttpDelete("{id}")]
    public async Task delete(int id)
    {
        await db.Set<article_to_subject>()
            .Where(a => a.id == id)
            .ExecuteDeleteAsync();
    }

    [HttpPut]
    [HttpPatch("{id}")]
    public async Task<ArticleToSubject> upsert(int? id, [FromBody] ArticleToSubject model)
    {
        article_to_subject entity;

        if (id == null)
        {
            entity = new article_to_subject
            {
                created_at = DateTimeOffset.UtcNow
            };
            db.Set<article_to_subject>().Add(entity);
        }
        else
        {
            entity = await db.Set<article_to_subject>().SingleAsync(a => a.id == id);
            entity.updated_at = DateTimeOffset.UtcNow;
        }

        entity.article_id = model.article_id;
        entity.subject_id = model.subject_id;

        await db.SaveChangesAsync();
        return await get(entity.id);
    }
}

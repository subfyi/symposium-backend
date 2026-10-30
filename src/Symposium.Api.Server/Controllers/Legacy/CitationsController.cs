using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symposium.Api.Server.Utils;
using Symposium.Models;
using Symposium.Query.Projections;
using Symposium.Storage;

namespace Symposium.Api.Server.Controllers.Legacy;

[Authorize(Roles = "Admin")]
[Route("citations")]
[Route("citation")]
public class CitationsController : ControllerBase
{
    private readonly AppDbContext db;

    public CitationsController(AppDbContext db)
    {
        this.db = db;
    }

    [HttpGet]
    public async Task<Pagination<Citations>> all(string? query = null)
    {
        return await this.Paginate(db.Set<citations>().Where(a => query == null || EF.Functions.ILike(a.raw, $"%{query}%")), a => a.Select(a => a.ToCitations()));
    }

    [HttpGet("{id}")]
    public async Task<Citations> get(int id)
    {
        return await db.Set<citations>()
            .Where(a => a.id == id)
            .Select(a => a.ToCitations())
            .SingleAsync();
    }

    [HttpDelete("{id}")]
    public async Task delete(int id)
    {
        await db.Set<citations>()
            .Where(a => a.id == id)
            .ExecuteDeleteAsync();
    }

    [HttpPut]
    [HttpPatch("{id}")]
    public async Task<Citations> upsert(int? id, [FromBody] Citations model)
    {
        citations entity;

        if (id == null)
        {
            entity = new citations
            {
                created_at = DateTimeOffset.UtcNow
            };
            db.Set<citations>().Add(entity);
        }
        else
        {
            entity = await db.Set<citations>().SingleAsync(a => a.id == id);
            entity.updated_at = DateTimeOffset.UtcNow;
        }

        entity.article_id = model.article_id;
        entity.d_id = model.d_id;
        entity.raw = model.raw;
        entity.type = model.type;
        entity.order = model.order;

        await db.SaveChangesAsync();
        return await get(entity.id);
    }
}

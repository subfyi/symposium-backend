using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symposium.Api.Server.Utils;
using Symposium.Models;
using Symposium.Query.Projections;
using Symposium.Storage;

namespace Symposium.Api.Server.Controllers.Legacy;

[Authorize(Roles = "Admin")]
[Route("keywords")]
[Route("keyword")]
public class KeywordsController : ControllerBase
{
    private readonly AppDbContext db;

    public KeywordsController(AppDbContext db)
    {
        this.db = db;
    }

    [HttpGet]
    public async Task<Pagination<Keywords>> all(string? query = null)
    {
        return await this.Paginate(db.Set<keywords>().Where(a => query == null || EF.Functions.ILike(a.name, $"%{query}%")), a => a.Select(a => a.ToKeywords()));
    }

    [HttpGet("{id}")]
    public async Task<Keywords> get(int id)
    {
        return await db.Set<keywords>()
            .Where(a => a.id == id)
            .Select(a => a.ToKeywords())
            .SingleAsync();
    }

    [HttpDelete("{id}")]
    public async Task delete(int id)
    {
        await db.Set<keywords>()
            .Where(a => a.id == id)
            .ExecuteDeleteAsync();
    }

    [HttpPut]
    [HttpPatch("{id}")]
    public async Task<Keywords> upsert(int? id, [FromBody] Keywords model)
    {
        keywords entity;

        if (id == null)
        {
            entity = new keywords
            {
                created_at = DateTimeOffset.UtcNow
            };
            db.Set<keywords>().Add(entity);
        }
        else
        {
            entity = await db.Set<keywords>().SingleAsync(a => a.id == id);
            entity.updated_at = DateTimeOffset.UtcNow;
        }

        entity.lang_id = model.lang_id;
        entity.name = model.name;

        await db.SaveChangesAsync();
        return await get(entity.id);
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symposium.Api.Server.Utils;
using Symposium.Models;
using Symposium.Query.Projections;
using Symposium.Storage;

namespace Symposium.Api.Server.Controllers.Legacy;

[Authorize(Roles = "Admin")]
[Route("publishs")]
[Route("typepublish")]
public class PublishsController : ControllerBase
{
    private readonly AppDbContext db;

    public PublishsController(AppDbContext db)
    {
        this.db = db;
    }

    [HttpGet]
    public async Task<Pagination<Publishs>> all(string? query = null)
    {
        return await this.Paginate(db.Set<publishs>().Where(a => query == null || EF.Functions.ILike(a.name!, $"%{query}%")), a => a.Select(a => a.ToPublishs(db)));
    }

    [HttpGet("{id:int}")]
    public async Task<Publishs> get(int id)
    {
        return await db.Set<publishs>()
            .Where(a => a.id == id)
            .Select(a => a.ToPublishs(db))
            .SingleAsync();
    }

    [HttpDelete("{id:int}")]
    public async Task delete(int id)
    {
        await db.Set<publishs>()
            .Where(a => a.id == id)
            .ExecuteDeleteAsync();
    }

    [HttpPut]
    [HttpPost]
    [HttpPatch("{id:int}")]
    public async Task<Publishs> upsert([FromRoute] int? id, [FromBody] Publishs model)
    {
        publishs entity;

        if (id == null)
        {
            entity = new publishs
            {
                created_at = DateTimeOffset.UtcNow
            };
            db.Set<publishs>().Add(entity);
        }
        else
        {
            entity = await db.Set<publishs>().SingleAsync(a => a.id == id);
            entity.updated_at = DateTimeOffset.UtcNow;
        }

        entity.active = model.active;
        var languageId = model.lang?.id?.ToString() ?? model.type;
        if (languageId != null && !await db.Set<submission_language>().AnyAsync(a => a.id.ToString() == languageId))
            throw new ArgumentException("Invalid catalog language.");
        entity.type = languageId;
        entity.name = model.name;
        entity.created_by = model.created_by;
        entity.updated_by = model.updated_by;

        await db.SaveChangesAsync();
        return await get(entity.id);
    }
}

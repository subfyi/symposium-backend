using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symposium.Api.Server.Utils;
using Symposium.Models;
using Symposium.Query.Projections;
using Symposium.Storage;

namespace Symposium.Api.Server.Controllers.Legacy;

[Authorize(Roles = "Admin")]
[Route("presentations")]
[Route("typepresentation")]
public class PresentationsController : ControllerBase
{
    private readonly AppDbContext db;

    public PresentationsController(AppDbContext db)
    {
        this.db = db;
    }

    [HttpGet]
    public async Task<Pagination<Presentations>> all(string? query = null)
    {
        return await this.Paginate(db.Set<presentations>().Where(a => query == null || EF.Functions.ILike(a.name!, $"%{query}%")), a => a.Select(a => a.ToPresentations(db)));
    }

    [HttpGet("{id:int}")]
    public async Task<Presentations> get(int id)
    {
        return await db.Set<presentations>()
            .Where(a => a.id == id)
            .Select(a => a.ToPresentations(db))
            .SingleAsync();
    }

    [HttpDelete("{id:int}")]
    public async Task delete(int id)
    {
        await db.Set<presentations>()
            .Where(a => a.id == id)
            .ExecuteDeleteAsync();
    }

    [HttpPut]
    [HttpPost]
    [HttpPatch("{id:int}")]
    public async Task<Presentations> upsert([FromRoute] int? id, [FromBody] Presentations model)
    {
        presentations entity;

        if (id == null)
        {
            entity = new presentations
            {
                created_at = DateTimeOffset.UtcNow
            };
            db.Set<presentations>().Add(entity);
        }
        else
        {
            entity = await db.Set<presentations>().SingleAsync(a => a.id == id);
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

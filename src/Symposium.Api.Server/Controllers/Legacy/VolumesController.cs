using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symposium.Api.Server.Utils;
using Symposium.Models;
using Symposium.Query.Projections;
using Symposium.Storage;

namespace Symposium.Api.Server.Controllers.Legacy;

[Authorize(Roles = "Admin")]
[Route("volumes")]
[Route("volume")]
public class VolumesController : ControllerBase
{
    private readonly AppDbContext db;

    public VolumesController(AppDbContext db)
    {
        this.db = db;
    }

    [HttpGet]
    public async Task<Pagination<Volumes>> all(string? query = null)
    {
        return await this.Paginate(db.Set<volumes>().Where(a => query == null || EF.Functions.ILike(a.volume, $"%{query}%")), a => a.Select(a => a.ToVolumes()));
    }

    [HttpGet("{id}")]
    public async Task<Volumes> get(int id)
    {
        return await db.Set<volumes>()
            .Where(a => a.id == id)
            .Select(a => a.ToVolumes())
            .SingleAsync();
    }

    [HttpDelete("{id}")]
    public async Task delete(int id)
    {
        await db.Set<volumes>()
            .Where(a => a.id == id)
            .ExecuteDeleteAsync();
    }

    [HttpPut]
    [HttpPatch("{id}")]
    public async Task<Volumes> upsert(int? id, [FromBody] Volumes model)
    {
        volumes entity;

        if (id == null)
        {
            entity = new volumes
            {
                created_at = DateTimeOffset.UtcNow
            };
            db.Set<volumes>().Add(entity);
        }
        else
        {
            entity = await db.Set<volumes>().SingleAsync(a => a.id == id);
            entity.updated_at = DateTimeOffset.UtcNow;
        }

        entity.journal_id = model.journal_id;
        entity.d_id = model.d_id;
        entity.volume = model.volume;
        entity.issue = model.issue;
        entity.special = model.special;
        entity.year = model.year;
        entity.long_year = model.long_year;
        entity.short_year = model.short_year;
        entity.json_id = model.json_id;
        entity.created_by = model.created_by;
        entity.date_published = model.date_published;
        entity.season = model.season;
        entity.supplement = model.supplement;

        await db.SaveChangesAsync();
        return await get(entity.id);
    }
}

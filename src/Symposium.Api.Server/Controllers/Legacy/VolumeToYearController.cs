using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symposium.Api.Server.Utils;
using Symposium.Models;
using Symposium.Query.Projections;
using Symposium.Storage;

namespace Symposium.Api.Server.Controllers.Legacy;

[Authorize(Roles = "Admin")]
[Route("volume-to-year")]
public class VolumeToYearController : ControllerBase
{
    private readonly AppDbContext db;

    public VolumeToYearController(AppDbContext db)
    {
        this.db = db;
    }

    [HttpGet]
    public async Task<Pagination<VolumeToYear>> all(string? query = null)
    {
        return await this.Paginate(db.Set<volume_to_year>().Where(a => query == null || EF.Functions.ILike(a.volume_num, $"%{query}%")), a => a.Select(a => a.ToVolumeToYear()));
    }

    [HttpGet("{id}")]
    public async Task<VolumeToYear> get(int id)
    {
        return await db.Set<volume_to_year>()
            .Where(a => a.id == id)
            .Select(a => a.ToVolumeToYear())
            .SingleAsync();
    }

    [HttpDelete("{id}")]
    public async Task delete(int id)
    {
        await db.Set<volume_to_year>()
            .Where(a => a.id == id)
            .ExecuteDeleteAsync();
    }

    [HttpPut]
    [HttpPatch("{id}")]
    public async Task<VolumeToYear> upsert(int? id, [FromBody] VolumeToYear model)
    {
        volume_to_year entity;

        if (id == null)
        {
            entity = new volume_to_year
            {
                created_at = DateTimeOffset.UtcNow
            };
            db.Set<volume_to_year>().Add(entity);
        }
        else
        {
            entity = await db.Set<volume_to_year>().SingleAsync(a => a.id == id);
            entity.updated_at = DateTimeOffset.UtcNow;
        }

        entity.volume_num = model.volume_num;
        entity.year = model.year;
        entity.short_year = model.short_year;

        await db.SaveChangesAsync();
        return await get(entity.id);
    }
}

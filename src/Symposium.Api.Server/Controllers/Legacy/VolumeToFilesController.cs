using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symposium.Api.Server.Utils;
using Symposium.Models;
using Symposium.Query.Projections;
using Symposium.Storage;

namespace Symposium.Api.Server.Controllers.Legacy;

[Authorize(Roles = "Admin")]
[Route("volume-to-files")]
public class VolumeToFilesController : ControllerBase
{
    private readonly AppDbContext db;

    public VolumeToFilesController(AppDbContext db)
    {
        this.db = db;
    }

    [HttpGet]
    public async Task<Pagination<VolumeToFiles>> all(string? query = null)
    {
        return await this.Paginate(db.Set<volume_to_files>().Where(a => query == null || EF.Functions.ILike(a.title, $"%{query}%")), a => a.Select(a => a.ToVolumeToFiles()));
    }

    [HttpGet("{id}")]
    public async Task<VolumeToFiles> get(int id)
    {
        return await db.Set<volume_to_files>()
            .Where(a => a.id == id)
            .Select(a => a.ToVolumeToFiles())
            .SingleAsync();
    }

    [HttpDelete("{id}")]
    public async Task delete(int id)
    {
        await db.Set<volume_to_files>()
            .Where(a => a.id == id)
            .ExecuteDeleteAsync();
    }

    [HttpPut]
    [HttpPatch("{id}")]
    public async Task<VolumeToFiles> upsert(int? id, [FromBody] VolumeToFiles model)
    {
        volume_to_files entity;

        if (id == null)
        {
            entity = new volume_to_files
            {
                created_at = DateTimeOffset.UtcNow
            };
            db.Set<volume_to_files>().Add(entity);
        }
        else
        {
            entity = await db.Set<volume_to_files>().SingleAsync(a => a.id == id);
            entity.updated_at = DateTimeOffset.UtcNow;
        }

        entity.volume_id = model.volume_id;
        entity.d_id = model.d_id;
        entity.lang_id = model.lang_id;
        entity.title = model.title;
        entity.description = model.description;
        entity.file_id = model.file_id;
        entity.type = model.type;

        await db.SaveChangesAsync();
        return await get(entity.id);
    }
}

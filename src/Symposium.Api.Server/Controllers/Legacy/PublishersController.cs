using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symposium.Api.Server.Utils;
using Symposium.Models;
using Symposium.Query.Projections;
using Symposium.Storage;

namespace Symposium.Api.Server.Controllers.Legacy;

[Authorize(Roles = "Admin")]
[Route("publishers")]
[Route("publisher")]
public class PublishersController : ControllerBase
{
    private readonly AppDbContext db;

    public PublishersController(AppDbContext db)
    {
        this.db = db;
    }

    [HttpGet]
    public async Task<Pagination<Publishers>> all(string? query = null)
    {
        return await this.Paginate(db.Set<publishers>().Where(a => query == null || EF.Functions.ILike(a.name, $"%{query}%")), a => a.Select(a => a.ToPublishers()));
    }

    [HttpGet("{id}")]
    public async Task<Publishers> get(int id)
    {
        return await db.Set<publishers>()
            .Where(a => a.id == id)
            .Select(a => a.ToPublishers())
            .SingleAsync();
    }

    [HttpDelete("{id}")]
    public async Task delete(int id)
    {
        await db.Set<publishers>()
            .Where(a => a.id == id)
            .ExecuteDeleteAsync();
    }

    [HttpPut]
    [HttpPatch("{id}")]
    public async Task<Publishers> upsert(int? id, [FromBody] Publishers model)
    {
        publishers entity;

        if (id == null)
        {
            entity = new publishers
            {
                created_at = DateTimeOffset.UtcNow
            };
            db.Set<publishers>().Add(entity);
        }
        else
        {
            entity = await db.Set<publishers>().SingleAsync(a => a.id == id);
            entity.updated_at = DateTimeOffset.UtcNow;
        }

        entity.d_id = model.d_id;
        entity.lang_id = model.lang_id;
        entity.name = model.name;
        entity.email = model.email;
        entity.lft = model.lft;
        entity.address_lat = model.address_lat;
        entity.address_long = model.address_long;

        await db.SaveChangesAsync();
        return await get(entity.id);
    }
}

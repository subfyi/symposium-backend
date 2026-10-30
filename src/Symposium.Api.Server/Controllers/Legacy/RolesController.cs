using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symposium.Api.Server.Utils;
using Symposium.Models;
using Symposium.Query.Projections;
using Symposium.Storage;

namespace Symposium.Api.Server.Controllers.Legacy;

[Authorize(Roles = "Admin")]
[Route("roles")]
public class RolesController : ControllerBase
{
    private readonly AppDbContext db;

    public RolesController(AppDbContext db)
    {
        this.db = db;
    }

    [HttpGet]
    public async Task<Pagination<Roles>> all(string? query = null)
    {
        return await this.Paginate(db.Set<roles>().Where(a => query == null || EF.Functions.ILike(a.name, $"%{query}%")), a => a.Select(a => a.ToRoles()));
    }

    [HttpGet("{id}")]
    public async Task<Roles> get(int id)
    {
        return await db.Set<roles>()
            .Where(a => a.id == id)
            .Select(a => a.ToRoles())
            .SingleAsync();
    }

    [HttpDelete("{id}")]
    public async Task delete(int id)
    {
        await db.Set<roles>()
            .Where(a => a.id == id)
            .ExecuteDeleteAsync();
    }

    [HttpPut]
    [HttpPatch("{id}")]
    public async Task<Roles> upsert(int? id, [FromBody] Roles model)
    {
        roles entity;

        if (id == null)
        {
            entity = new roles
            {
                created_at = DateTimeOffset.UtcNow
            };
            db.Set<roles>().Add(entity);
        }
        else
        {
            entity = await db.Set<roles>().SingleAsync(a => a.id == id);
            entity.updated_at = DateTimeOffset.UtcNow;
        }

        entity.name = model.name;

        await db.SaveChangesAsync();
        return await get(entity.id);
    }
}

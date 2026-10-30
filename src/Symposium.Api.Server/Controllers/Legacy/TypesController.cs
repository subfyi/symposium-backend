using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symposium.Api.Server.Utils;
using Symposium.Models;
using Symposium.Query.Projections;
using Symposium.Storage;

namespace Symposium.Api.Server.Controllers.Legacy;

[Authorize(Roles = "Admin")]
[Route("types")]
public class TypesController : ControllerBase
{
    private readonly AppDbContext db;

    public TypesController(AppDbContext db)
    {
        this.db = db;
    }

    [HttpGet]
    public async Task<Pagination<Types>> all(string? query = null)
    {
        return await this.Paginate(db.Set<types>().Where(a => query == null || EF.Functions.ILike(a.value_tr, $"%{query}%")), a => a.Select(a => a.ToTypes()));
    }

    [HttpGet("{id:int}")]
    public async Task<Types> get(int id)
    {
        return await db.Set<types>()
            .Where(a => a.id == id)
            .Select(a => a.ToTypes())
            .SingleAsync();
    }

    [HttpDelete("{id:int}")]
    public async Task delete(int id)
    {
        await db.Set<types>()
            .Where(a => a.id == id)
            .ExecuteDeleteAsync();
    }

    [HttpPut]
    [HttpPatch("{id:int}")]
    public async Task<Types> upsert(int? id, [FromBody] Types model)
    {
        types entity;

        if (id == null)
        {
            entity = new types
            {
                created_at = DateTimeOffset.UtcNow
            };
            db.Set<types>().Add(entity);
        }
        else
        {
            entity = await db.Set<types>().SingleAsync(a => a.id == id);
            entity.updated_at = DateTimeOffset.UtcNow;
        }

        entity.d_id = model.d_id;
        entity.value_tr = model.value_tr;
        entity.value_en = model.value_en;

        await db.SaveChangesAsync();
        return await get(entity.id);
    }
}

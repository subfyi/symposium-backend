using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symposium.Api.Server.Utils;
using Symposium.Models;
using Symposium.Query.Projections;
using Symposium.Storage;

namespace Symposium.Api.Server.Controllers.Legacy;

[Authorize(Roles = "Admin")]
[Route("subjects")]
[Route("subject")]
public class SubjectsController : ControllerBase
{
    private readonly AppDbContext db;

    public SubjectsController(AppDbContext db)
    {
        this.db = db;
    }

    [HttpGet]
    public async Task<Pagination<Subjects>> all(string? query = null)
    {
        return await this.Paginate(db.Set<subjects>().Where(a => query == null || EF.Functions.ILike(a.name, $"%{query}%")), a => a.Select(a => a.ToSubjects()));
    }

    [HttpGet("{id}")]
    public async Task<Subjects> get(int id)
    {
        return await db.Set<subjects>()
            .Where(a => a.id == id)
            .Select(a => a.ToSubjects())
            .SingleAsync();
    }

    [HttpDelete("{id}")]
    public async Task delete(int id)
    {
        await db.Set<subjects>()
            .Where(a => a.id == id)
            .ExecuteDeleteAsync();
    }

    [HttpPut]
    [HttpPatch("{id}")]
    public async Task<Subjects> upsert(int? id, [FromBody] Subjects model)
    {
        subjects entity;

        if (id == null)
        {
            entity = new subjects
            {
                created_at = DateTimeOffset.UtcNow
            };
            db.Set<subjects>().Add(entity);
        }
        else
        {
            entity = await db.Set<subjects>().SingleAsync(a => a.id == id);
            entity.updated_at = DateTimeOffset.UtcNow;
        }

        entity.d_id = model.d_id;
        entity.lang_id = model.lang_id;
        entity.name = model.name;

        await db.SaveChangesAsync();
        return await get(entity.id);
    }
}

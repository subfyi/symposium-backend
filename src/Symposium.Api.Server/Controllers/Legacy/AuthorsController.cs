using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symposium.Api.Server.Utils;
using Symposium.Models;
using Symposium.Query.Projections;
using Symposium.Storage;

namespace Symposium.Api.Server.Controllers.Legacy;

[Authorize]
[Route("authors")]
[Route("author")]
public class AuthorsController : ControllerBase
{
    private readonly AppDbContext db;

    public AuthorsController(AppDbContext db)
    {
        this.db = db;
    }

    [HttpGet]
    public async Task<Pagination<Authors>> all(string? query = null)
    {
        var actor = User.FindFirst("user_id")!.Value;
        return await this.Paginate(db.Set<authors>().Where(a => User.IsInRole("Admin") || a.p_user == actor)
            .Where(a => query == null || EF.Functions.ILike(a.email!, $"%{query}%")), a => a.Select(a => a.ToAuthors()));
    }

    [HttpGet("{id}")]
    public async Task<Authors> get(int id)
    {
        var actor = User.FindFirst("user_id")!.Value;
        return await db.Set<authors>()
            .Where(a => a.id == id && (User.IsInRole("Admin") || a.p_user == actor))
            .Select(a => a.ToAuthors())
            .SingleAsync();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task delete(int id)
    {
        await db.Set<authors>()
            .Where(a => a.id == id)
            .ExecuteDeleteAsync();
    }

    [HttpPut]
    [Authorize(Roles = "Admin")]
    [HttpPatch("{id}")]
    public async Task<Authors> upsert(int? id, [FromBody] Authors model)
    {
        authors entity;

        if (id == null)
        {
            entity = new authors
            {
                created_at = DateTimeOffset.UtcNow
            };
            db.Set<authors>().Add(entity);
        }
        else
        {
            entity = await db.Set<authors>().SingleAsync(a => a.id == id);
            entity.updated_at = DateTimeOffset.UtcNow;
        }

        entity.p_id = model.p_id;
        entity.p_user = model.p_user;
        entity.first_name = model.first_name;
        entity.last_name = model.last_name;
        entity.adress = model.adress;
        entity.email = model.email;
        entity.correspond = model.correspond;
        entity.presenter = model.presenter;

        await db.SaveChangesAsync();
        return await get(entity.id);
    }
}

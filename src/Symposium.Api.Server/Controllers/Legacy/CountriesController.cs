using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symposium.Api.Server.Utils;
using Symposium.Models;
using Symposium.Query.Projections;
using Symposium.Storage;

namespace Symposium.Api.Server.Controllers.Legacy;

[Authorize(Roles = "Admin")]
[Route("countries")]
[Route("countrie")]
public class CountriesController : ControllerBase
{
    private readonly AppDbContext db;

    public CountriesController(AppDbContext db)
    {
        this.db = db;
    }

    [HttpGet]
    public async Task<Pagination<Countries>> all(string? query = null)
    {
        return await this.Paginate(db.Set<countries>().Where(a => query == null || EF.Functions.ILike(a.name, $"%{query}%")), a => a.Select(a => a.ToCountries()));
    }

    [HttpGet("{id}")]
    public async Task<Countries> get(int id)
    {
        return await db.Set<countries>()
            .Where(a => a.id == id)
            .Select(a => a.ToCountries())
            .SingleAsync();
    }

    [HttpDelete("{id}")]
    public async Task delete(int id)
    {
        await db.Set<countries>()
            .Where(a => a.id == id)
            .ExecuteDeleteAsync();
    }

    [HttpPut]
    [HttpPatch("{id}")]
    public async Task<Countries> upsert(int? id, [FromBody] Countries model)
    {
        countries entity;

        if (id == null)
        {
            entity = new countries
            {
                created_at = DateTimeOffset.UtcNow
            };
            db.Set<countries>().Add(entity);
        }
        else
        {
            entity = await db.Set<countries>().SingleAsync(a => a.id == id);
            entity.updated_at = DateTimeOffset.UtcNow;
        }

        entity.d_id = model.d_id;
        entity.name = model.name;
        entity.provinces = model.provinces;

        await db.SaveChangesAsync();
        return await get(entity.id);
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symposium.Api.Server.Utils;
using Symposium.Models;
using Symposium.Query.Projections;
using Symposium.Storage;

namespace Symposium.Api.Server.Controllers.Legacy;

[Authorize(Roles = "Admin")]
[Route("journals")]
[Route("journal")]
public class JournalsController : ControllerBase
{
    private readonly AppDbContext db;

    public JournalsController(AppDbContext db)
    {
        this.db = db;
    }

    [HttpGet]
    public async Task<Pagination<Journals>> all(string? query = null)
    {
        return await this.Paginate(db.Set<journals>().Where(a => query == null || EF.Functions.ILike(a.founded, $"%{query}%")), a => a.Select(a => a.ToJournals()));
    }

    [HttpGet("{id}")]
    public async Task<Journals> get(int id)
    {
        return await db.Set<journals>()
            .Where(a => a.id == id)
            .Select(a => a.ToJournals())
            .SingleAsync();
    }

    [HttpDelete("{id}")]
    public async Task delete(int id)
    {
        await db.Set<journals>()
            .Where(a => a.id == id)
            .ExecuteDeleteAsync();
    }

    [HttpPut]
    [HttpPatch("{id}")]
    public async Task<Journals> upsert(int? id, [FromBody] Journals model)
    {
        journals entity;

        if (id == null)
        {
            entity = new journals
            {
                created_at = DateTimeOffset.UtcNow
            };
            db.Set<journals>().Add(entity);
        }
        else
        {
            entity = await db.Set<journals>().SingleAsync(a => a.id == id);
            entity.updated_at = DateTimeOffset.UtcNow;
        }

        entity.json_id = model.json_id;
        entity.d_id = model.d_id;
        entity.image_id = model.image_id;
        entity.image_header_id = model.image_header_id;
        entity.founded = model.founded;
        entity.url = model.url;
        entity.address = model.address;
        entity.phone = model.phone;
        entity.email = model.email;
        entity.eissn = model.eissn;
        entity.pissn = model.pissn;
        entity.slug = model.slug;
        entity.printed = model.printed;
        entity.mandatory_lang = model.mandatory_lang;
        entity.mandatory_rtl = model.mandatory_rtl;
        entity.view_count = model.view_count;
        entity.download_count = model.download_count;
        entity.created_by = model.created_by;
        entity.updated_by = model.updated_by;

        await db.SaveChangesAsync();
        return await get(entity.id);
    }
}

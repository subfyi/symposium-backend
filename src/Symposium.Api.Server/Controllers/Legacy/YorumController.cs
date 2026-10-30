using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symposium.Api.Server.Utils;
using Symposium.Models;
using Symposium.Query.Projections;
using Symposium.Storage;

namespace Symposium.Api.Server.Controllers.Legacy;

[Authorize(Roles = "Admin")]
[Route("yorum")]
public class YorumController : ControllerBase
{
    private readonly AppDbContext db;

    public YorumController(AppDbContext db)
    {
        this.db = db;
    }

    [HttpGet]
    public async Task<Pagination<Yorum>> all(string? query = null)
    {
        return await this.Paginate(db.Set<yorumlar>().Where(a => query == null || EF.Functions.ILike(a.isim, $"%{query}%")), a => a.Select(a => a.ToYorum()));
    }

    [HttpGet("{id}")]
    public async Task<Yorum> get(int id)
    {
        return await db.Set<yorumlar>()
            .Where(a => a.id == id)
            .Select(a => a.ToYorum())
            .SingleAsync();
    }

    [HttpDelete("{id}")]
    public async Task delete(int id)
    {
        await db.Set<yorumlar>()
            .Where(a => a.id == id)
            .ExecuteDeleteAsync();
    }

    [HttpPut]
    [HttpPatch("{id}")]
    public async Task<Yorum> upsert(int? id, [FromBody] Yorum model)
    {
        yorumlar entity;

        if (id == null)
        {
            entity = new yorumlar
            {
                created_at = DateTimeOffset.UtcNow
            };
            db.Set<yorumlar>().Add(entity);
        }
        else
        {
            entity = await db.Set<yorumlar>().SingleAsync(a => a.id == id);
            entity.updated_at = DateTimeOffset.UtcNow;
        }

        entity.isim = model.isim;
        entity.mail = model.mail;
        entity.icerik = model.icerik;
        entity.blog = model.blog;
        entity.ust_yorum = model.ust_yorum;
        entity.kullanici_id = model.kullanici_id;

        await db.SaveChangesAsync();
        return await get(entity.id);
    }
}

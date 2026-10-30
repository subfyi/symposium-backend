using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symposium.Api.Server.Utils;
using Symposium.Models;
using Symposium.Query.Projections;
using Symposium.Storage;

namespace Symposium.Api.Server.Controllers.Legacy;

[Authorize(Roles = "Admin")]
[Route("gorevicerik")]
public class GorevicerikController : ControllerBase
{
    private readonly AppDbContext db;

    public GorevicerikController(AppDbContext db)
    {
        this.db = db;
    }

    [HttpGet]
    public async Task<Pagination<Gorevicerik>> all(string? query = null)
    {
        return await this.Paginate(db.Set<gorev_icerik>().Where(a => query == null || EF.Functions.ILike(a.isicerik, $"%{query}%")), a => a.Select(a => a.ToGorevicerik()));
    }

    [HttpGet("{id}")]
    public async Task<Gorevicerik> get(int id)
    {
        return await db.Set<gorev_icerik>()
            .Where(a => a.id == id)
            .Select(a => a.ToGorevicerik())
            .SingleAsync();
    }

    [HttpDelete("{id}")]
    public async Task delete(int id)
    {
        await db.Set<gorev_icerik>()
            .Where(a => a.id == id)
            .ExecuteDeleteAsync();
    }

    [HttpPut]
    [HttpPatch("{id}")]
    public async Task<Gorevicerik> upsert(int? id, [FromBody] Gorevicerik model)
    {
        gorev_icerik entity;

        if (id == null)
        {
            entity = new gorev_icerik
            {
                created_at = DateTimeOffset.UtcNow
            };
            db.Set<gorev_icerik>().Add(entity);
        }
        else
        {
            entity = await db.Set<gorev_icerik>().SingleAsync(a => a.id == id);
            entity.updated_at = DateTimeOffset.UtcNow;
        }

        entity.isicerik = model.isicerik;
        entity.slug = model.slug;
        entity.ust_icerik = model.ust_icerik;
        entity.grv_icerik_olusturan = model.grv_icerik_olusturan;
        entity.dosyaisimleri = model.dosyaisimleri;
        entity.gdosyaisimleri = model.gdosyaisimleri;
        entity.r_dosyaisimleri = model.r_dosyaisimleri;
        entity.r_gdosyaisimleri = model.r_gdosyaisimleri;

        await db.SaveChangesAsync();
        return await get(entity.id);
    }
}

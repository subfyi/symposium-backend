using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symposium.Api.Server.Utils;
using Symposium.Models;
using Symposium.Query.Projections;
using Symposium.Storage;

namespace Symposium.Api.Server.Controllers.Legacy;

[Authorize(Roles = "Admin")]
[Route("dosyalar")]
public class DosyalarController : ControllerBase
{
    private readonly AppDbContext db;

    public DosyalarController(AppDbContext db)
    {
        this.db = db;
    }

    [HttpGet]
    public async Task<Pagination<Dosyalar>> all(string? query = null)
    {
        return await this.Paginate(db.Set<dosyalar>().Where(a => query == null || EF.Functions.ILike(a.f_icerik_olusturan, $"%{query}%")), a => a.Select(a => a.ToDosyalar()));
    }

    [HttpGet("{id}")]
    public async Task<Dosyalar> get(int id)
    {
        return await db.Set<dosyalar>()
            .Where(a => a.id == id)
            .Select(a => a.ToDosyalar())
            .SingleAsync();
    }

    [HttpDelete("{id}")]
    public async Task delete(int id)
    {
        await db.Set<dosyalar>()
            .Where(a => a.id == id)
            .ExecuteDeleteAsync();
    }

    [HttpPut]
    [HttpPatch("{id}")]
    public async Task<Dosyalar> upsert(int? id, [FromBody] Dosyalar model)
    {
        dosyalar entity;

        if (id == null)
        {
            entity = new dosyalar
            {
                created_at = DateTimeOffset.UtcNow
            };
            db.Set<dosyalar>().Add(entity);
        }
        else
        {
            entity = await db.Set<dosyalar>().SingleAsync(a => a.id == id);
            entity.updated_at = DateTimeOffset.UtcNow;
        }

        entity.f_icerik_olusturan = model.f_icerik_olusturan;
        entity.f_dosyayeri = model.f_dosyayeri;
        entity.r_dosyaisimi = model.r_dosyaisimi;
        entity.f_dosyaisimi = model.f_dosyaisimi;
        entity.g_dosyaismi = model.g_dosyaismi;
        entity.f_klasorismi = model.f_klasorismi;
        entity.g_klasorismi = model.g_klasorismi;
        entity.f_dosya_uzanti = model.f_dosya_uzanti;
        entity.f_dosya_boyut = model.f_dosya_boyut;
        entity.width = model.width;
        entity.height = model.height;
        entity.f_sira = model.f_sira;

        await db.SaveChangesAsync();
        return await get(entity.id);
    }
}

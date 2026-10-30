using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symposium.Api.Server.Utils;
using Symposium.Models;
using Symposium.Query.Projections;
using Symposium.Storage;

namespace Symposium.Api.Server.Controllers.Legacy;

[Authorize(Roles = "Admin")]
[Route("gorevfiles")]
public class GorevfilesController : ControllerBase
{
    private readonly AppDbContext db;

    public GorevfilesController(AppDbContext db)
    {
        this.db = db;
    }

    [HttpGet]
    public async Task<Pagination<Gorevfiles>> all(string? query = null)
    {
        return await this.Paginate(db.Set<gorev_files>().Where(a => query == null || EF.Functions.ILike(a.f_icerik_olusturan, $"%{query}%")), a => a.Select(a => a.ToGorevfiles()));
    }

    [HttpGet("{id}")]
    public async Task<Gorevfiles> get(int id)
    {
        return await db.Set<gorev_files>()
            .Where(a => a.id == id)
            .Select(a => a.ToGorevfiles())
            .SingleAsync();
    }

    [HttpDelete("{id}")]
    public async Task delete(int id)
    {
        await db.Set<gorev_files>()
            .Where(a => a.id == id)
            .ExecuteDeleteAsync();
    }

    [HttpPut]
    [HttpPatch("{id}")]
    public async Task<Gorevfiles> upsert(int? id, [FromBody] Gorevfiles model)
    {
        gorev_files entity;

        if (id == null)
        {
            entity = new gorev_files
            {
                created_at = DateTimeOffset.UtcNow
            };
            db.Set<gorev_files>().Add(entity);
        }
        else
        {
            entity = await db.Set<gorev_files>().SingleAsync(a => a.id == id);
            entity.updated_at = DateTimeOffset.UtcNow;
        }

        entity.f_icerik_id = model.f_icerik_id;
        entity.f_icerik_olusturan = model.f_icerik_olusturan;
        entity.f_dosyayeri = model.f_dosyayeri;
        entity.r_dosyaisimi = model.r_dosyaisimi;
        entity.f_dosyaisimi = model.f_dosyaisimi;
        entity.f_malzeme_turu = model.f_malzeme_turu;
        entity.f_baski_olcusu = model.f_baski_olcusu;
        entity.f_urun_olcusu = model.f_urun_olcusu;
        entity.f_uygulama = model.f_uygulama;
        entity.f_adet = model.f_adet;
        entity.slug = model.slug;
        entity.f_dosya_uzanti = model.f_dosya_uzanti;
        entity.f_dosya_boyut = model.f_dosya_boyut;
        entity.f_urun_olcusu_b = model.f_urun_olcusu_b;

        await db.SaveChangesAsync();
        return await get(entity.id);
    }
}

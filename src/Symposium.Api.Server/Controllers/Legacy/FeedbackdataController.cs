using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symposium.Api.Server.Utils;
using Symposium.Models;
using Symposium.Query.Projections;
using Symposium.Storage;

namespace Symposium.Api.Server.Controllers.Legacy;

[Authorize(Roles = "Admin")]
[Route("feedbackdata")]
public class FeedbackdataController : ControllerBase
{
    private readonly AppDbContext db;

    public FeedbackdataController(AppDbContext db)
    {
        this.db = db;
    }

    [HttpGet]
    public async Task<Pagination<Feedbackdata>> all(string? query = null)
    {
        return await this.Paginate(db.Set<feedbackdata>().Where(a => query == null || EF.Functions.ILike(a.isicerik, $"%{query}%")), a => a.Select(a => a.ToFeedbackdata()));
    }

    [HttpGet("{id}")]
    public async Task<Feedbackdata> get(int id)
    {
        return await db.Set<feedbackdata>()
            .Where(a => a.id == id)
            .Select(a => a.ToFeedbackdata())
            .SingleAsync();
    }

    [HttpDelete("{id}")]
    public async Task delete(int id)
    {
        await db.Set<feedbackdata>()
            .Where(a => a.id == id)
            .ExecuteDeleteAsync();
    }

    [HttpPut]
    [HttpPatch("{id}")]
    public async Task<Feedbackdata> upsert(int? id, [FromBody] Feedbackdata model)
    {
        feedbackdata entity;

        if (id == null)
        {
            entity = new feedbackdata
            {
                created_at = DateTimeOffset.UtcNow
            };
            db.Set<feedbackdata>().Add(entity);
        }
        else
        {
            entity = await db.Set<feedbackdata>().SingleAsync(a => a.id == id);
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

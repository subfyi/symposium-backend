using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symposium.Api.Server.Utils;
using Symposium.Models;
using Symposium.Query.Projections;
using Symposium.Storage;

namespace Symposium.Api.Server.Controllers.Legacy;

[Authorize(Roles = "Admin")]
[Route("feedback")]
public class FeedbackController : ControllerBase
{
    private readonly AppDbContext db;

    public FeedbackController(AppDbContext db)
    {
        this.db = db;
    }

    [HttpGet]
    public async Task<Pagination<Feedback>> all(string? query = null)
    {
        return await this.Paginate(db.Set<feedbacks>().Where(a => query == null || EF.Functions.ILike(a.grv_icerik_yonetici, $"%{query}%")), a => a.Select(a => a.ToFeedback()));
    }

    [HttpGet("{id}")]
    public async Task<Feedback> get(int id)
    {
        return await db.Set<feedbacks>()
            .Where(a => a.id == id)
            .Select(a => a.ToFeedback())
            .SingleAsync();
    }

    [HttpDelete("{id}")]
    public async Task delete(int id)
    {
        await db.Set<feedbacks>()
            .Where(a => a.id == id)
            .ExecuteDeleteAsync();
    }

    [HttpPut]
    [HttpPatch("{id}")]
    public async Task<Feedback> upsert(int? id, [FromBody] Feedback model)
    {
        feedbacks entity;

        if (id == null)
        {
            entity = new feedbacks
            {
                created_at = DateTimeOffset.UtcNow
            };
            db.Set<feedbacks>().Add(entity);
        }
        else
        {
            entity = await db.Set<feedbacks>().SingleAsync(a => a.id == id);
            entity.updated_at = DateTimeOffset.UtcNow;
        }

        entity.grv_icerik_yonetici = model.grv_icerik_yonetici;
        entity.grv_bitis_tarih = model.grv_bitis_tarih;
        entity.grv_onemi = model.grv_onemi;
        entity.slug = model.slug;

        await db.SaveChangesAsync();
        return await get(entity.id);
    }
}

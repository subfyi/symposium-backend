using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using System.Linq.Expressions;
using Symposium.Services;

namespace Symposium.Api.Server.Controllers.Legacy;

[Authorize]
[Route("articles")]
[Route("article")]
[Route("submission")]
public class ArticlesController(AppDbContext db, SubmissionService submissions) : ControllerBase
{
    private bool IsAdmin => User.Identity?.IsAuthenticated == true && User.IsInRole("Admin");
    private int? Actor => int.TryParse(User.FindFirstValue("user_id"), out var id) ? id : null;

    [HttpGet, AllowAnonymous]
    public async Task<IActionResult> all(string? query = null, string? year = null, string? orals = null,
        string? posters = null, string? presentation = null, string? pap_status = null)
    {
        var privateView = Actor != null && (IsAdmin || !Enabled(presentation));
        var source = db.Set<articles>().AsNoTracking();
        if (privateView)
        {
            source = source.Where(a => pap_status == null ? a.pap_status != "3" : a.pap_status == pap_status)
                .Where(a => IsAdmin || a.pap_creater == Actor.ToString())
                .Where(a => year == null || a.title_year == year)
                .Where(a => !Enabled(orals) || a.pre_type == "463" || a.pre_type == "116" || a.pre_type == "97")
                .Where(a => !Enabled(posters) || a.pre_type == "98")
                .Where(a => !Enabled(presentation) || a.video_approved == true);
        }
        else source = source.PublishedArticles(year, Enabled(orals), Enabled(posters), Enabled(presentation));
        source = source.Where(a => query == null || EF.Functions.ILike(a.en_title!, $"%{query}%"));
        var sort = Request.Query["sort"].ToString();
        var descending = true;
        if (Request.Query.TryGetValue("desc", out var desc) && !bool.TryParse(desc, out descending)) return BadRequest();
        if (!privateView && sort is not ("" or "id" or "en_title" or "title_year")) return BadRequest();
        Expression<Func<articles, object>> key = sort switch
        {
            "" or "id" => a => a.id,
            "en_title" => a => a.en_title!, "title_year" => a => a.title_year!,
            "created_at" => a => a.created_at!, "updated_at" => a => a.updated_at!,
            "paper_approved" => a => a.paper_approved!, "video_approved" => a => a.video_approved!,
            "pap_status" => a => a.pap_status!,
            "topic.value" => a => db.Set<parameters>().Where(b => b.id.ToString() == a.pap_topic_p).Select(b => b.value).FirstOrDefault()!,
            "parampre.value" => a => db.Set<parameters>().Where(b => b.id.ToString() == a.pre_type).Select(b => b.value).FirstOrDefault()!,
            "parampap.value" => a => db.Set<parameters>().Where(b => b.id.ToString() == a.pap_type).Select(b => b.value).FirstOrDefault()!,
            "user.name" or "bloglar.updated_at" => a => db.users.Where(b => b.id.ToString() == a.pap_creater).Select(b => b.first_name).FirstOrDefault()!,
            "user.surname" => a => db.users.Where(b => b.id.ToString() == a.pap_creater).Select(b => b.last_name).FirstOrDefault()!,
            _ => throw new ArgumentException("Unsupported sort column.")
        };
        source = (descending ? source.OrderByDescending(key) : source.OrderBy(key)).ThenBy(a => a.id);
        return privateView ? Ok(await this.Paginate(source, a => a.ToAdminArticles(db), orderSupported: false))
            : Ok(await this.Paginate(source, a => a.ToPublicArticles(db), orderSupported: false));
    }

    [HttpGet("years"), AllowAnonymous]
    public async Task<IReadOnlyCollection<object>> years()
    {
        var source = db.Set<articles>().AsNoTracking();
        source = IsAdmin ? source.Where(a => a.pap_status != "3") : Actor != null
            ? source.Where(a => a.pap_creater == Actor.ToString() && a.pap_status != "3") : source.PublishedArticles();
        return (await source.Where(a => a.title_year != null && a.title_year != "").Select(a => a.title_year)
            .Distinct().OrderByDescending(a => a).ToListAsync()).Select(a => (object)new { title_year = a }).ToList();
    }

    [HttpGet("{id:int}"), AllowAnonymous]
    public async Task<IActionResult> get(int id)
    {
        var source = db.Set<articles>().AsNoTracking().Where(a => a.id == id);
        if (Actor != null && await source.AnyAsync(a => IsAdmin || (a.pap_creater == Actor.ToString() && a.pap_status != "3")))
            return Ok(await source.ToAdminArticles(db).SingleAsync());
        var result = await source.PublishedArticles().ToPublicArticles(db).SingleOrDefaultAsync();
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost, HttpPut, HttpPatch("{id:int}")]
    [EnableRateLimiting("authentication")]
    public async Task<ActionResult<Articles>> upsert(int? id, [FromBody] Articles model)
    {
        if (!ModelState.IsValid || model == null) return BadRequest(ModelState);
        if (Actor == null) return Unauthorized();
        var savedId = await submissions.Save(id, model, Actor.Value, IsAdmin);
        return await db.Set<articles>().Where(a => a.id == savedId).ToAdminArticles(db).SingleAsync();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> delete(int id)
    {
        var entity = await db.Set<articles>().SingleOrDefaultAsync(a => a.id == id);
        if (entity == null) return NotFound();
        entity.pap_status = "3"; entity.updated_at = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(); return NoContent();
    }

    [HttpPost("{id:int}/restore")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> restore(int id)
    {
        var entity = await db.Set<articles>().SingleOrDefaultAsync(a => a.id == id);
        if (entity == null) return NotFound();
        entity.pap_status = "0"; entity.updated_at = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(); return NoContent();
    }

    [HttpGet("{id:int}/comments")]
    public async Task<IActionResult> comments(int id)
    {
        if (!await CanReadComments(id)) return NotFound();
        return Ok(await db.Set<submission_comment>().Where(a => a.article_id == id).OrderBy(a => a.created_at).ThenBy(a => a.id)
            .Select(a => new SubmissionComment { id = a.id, isicerik = a.body, created_at = a.created_at, updated_at = a.updated_at,
                user = db.users.Where(b => b.id == a.author_id).Select(b => new SubmissionUser { id = b.id, name = b.first_name, surname = b.last_name }).FirstOrDefault() }).ToListAsync());
    }

    [HttpPost("{id:int}/comments"), EnableRateLimiting("authentication")]
    public async Task<IActionResult> comment(int id, [FromBody] SubmissionComment model)
    {
        if (!ModelState.IsValid || model == null) return BadRequest(ModelState);
        if (Actor == null) return Unauthorized();
        await submissions.AddComment(id, model.isicerik, Actor.Value, IsAdmin);
        return await comments(id);
    }

    private Task<bool> CanReadComments(int id) => db.Set<articles>().AnyAsync(a => a.id == id &&
        (IsAdmin || (a.pap_creater == Actor.ToString() && a.pap_status != "3")));
    private static bool Enabled(string? value) => value is "1" or "true";

    [HttpPatch("{id:int}/comments/{commentId:int}"), EnableRateLimiting("authentication")]
    public async Task<IActionResult> updateComment(int id, int commentId, [FromBody] SubmissionComment model)
    {
        if (!ModelState.IsValid || model == null) return BadRequest(ModelState);
        if (Actor == null) return Unauthorized();
        await submissions.ChangeComment(id, commentId, model.isicerik, Actor.Value, IsAdmin);
        return NoContent();
    }

    [HttpDelete("{id:int}/comments/{commentId:int}")]
    public async Task<IActionResult> deleteComment(int id, int commentId)
    {
        if (Actor == null) return Unauthorized();
        await submissions.ChangeComment(id, commentId, null, Actor.Value, IsAdmin, delete: true);
        return NoContent();
    }
}

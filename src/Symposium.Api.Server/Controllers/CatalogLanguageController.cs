using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Symposium.Api.Server.Controllers;

[Authorize(Roles = "Admin"), Route("typelanguage")]
public class CatalogLanguageController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public Task<Pagination<CatalogLanguage>> All(string? query = null) => this.Paginate(db.Set<submission_language>()
        .Where(a => query == null || EF.Functions.ILike(a.name, $"%{query}%")),
        q => q.Select(a => new CatalogLanguage { id = a.id, name = a.name, code = a.code, active = a.active, rtl = a.rtl }));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CatalogLanguage>> Get(int id)
    {
        var result = await db.Set<submission_language>().Where(a => a.id == id)
            .Select(a => new CatalogLanguage { id = a.id, name = a.name, code = a.code, active = a.active, rtl = a.rtl }).SingleOrDefaultAsync();
        return result == null ? NotFound() : result;
    }

    [HttpPut, HttpPost, HttpPatch("{id:int}")]
    public async Task<ActionResult<CatalogLanguage>> Save(int? id, [FromBody] CatalogLanguage model)
    {
        if (string.IsNullOrWhiteSpace(model.name) || string.IsNullOrWhiteSpace(model.code) || model.name.Length > 256 || model.code.Length > 32)
            return BadRequest(new { message = "A name and language code are required." });
        var code = model.code.Trim().ToLowerInvariant();
        if (await db.Set<submission_language>().AnyAsync(a => a.code == code && a.id != id))
            return Conflict(new { message = "This language code already exists." });
        var entity = id == null ? new submission_language() : await db.Set<submission_language>().SingleOrDefaultAsync(a => a.id == id);
        if (entity == null) return NotFound();
        if (id == null) db.Add(entity);
        entity.name = model.name.Trim(); entity.code = code; entity.active = model.active ?? true; entity.rtl = model.rtl ?? false;
        await db.SaveChangesAsync(); return await Get(entity.id);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (await db.Set<presentations>().AnyAsync(a => a.type == id.ToString()) || await db.Set<publishs>().AnyAsync(a => a.type == id.ToString()) ||
            await db.Set<topics>().AnyAsync(a => a.type == id.ToString())) return Conflict(new { message = "This language is in use." });
        var count = await db.Set<submission_language>().Where(a => a.id == id).ExecuteDeleteAsync();
        return count == 0 ? NotFound() : NoContent();
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symposium.Api.Server.Utils;
using Symposium.Models;
using Symposium.Query.Projections;
using Symposium.Storage;

namespace Symposium.Api.Server.Controllers.Legacy;

[Authorize]
[Route("parameters")]
[Route("parameters/{type}")]
public class ParametersController : ControllerBase
{
    private readonly AppDbContext db;

    public ParametersController(AppDbContext db)
    {
        this.db = db;
    }

    [HttpGet]
    public async Task<Pagination<Parameters>> all(string? query = null, [FromRoute] string? type = null)
    {
        return await this.Paginate(db.Set<parameters>()
            .Where(a => type == null || a.type == type)
            .Where(a => query == null || EF.Functions.ILike(a.value, $"%{query}%")),
            a => a.Select(a => a.ToParameters()));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Parameters>> get([FromRoute] int id, [FromRoute] string? type = null)
    {
        var model = await db.Set<parameters>()
            .Where(a => a.id == id && (type == null || a.type == type))
            .Select(a => a.ToParameters())
            .SingleOrDefaultAsync();
        return model == null ? NotFound() : model;
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> delete([FromRoute] int id, [FromRoute] string? type = null)
    {
        var deleted = await db.Set<parameters>()
            .Where(a => a.id == id && (type == null || a.type == type))
            .ExecuteDeleteAsync();
        return deleted == 0 ? NotFound() : NoContent();
    }

    [HttpPut]
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [HttpPatch("{id:int}")]
    public async Task<ActionResult<Parameters>> upsert([FromRoute] int? id, [FromBody] Parameters model,
        [FromRoute] string? type = null)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        if (type != null && model.type != null && model.type != type)
            return BadRequest("The parameter type must match the route scope.");

        if (string.IsNullOrWhiteSpace(type ?? model.type))
            return BadRequest("A parameter type is required.");

        parameters entity;

        if (id == null)
        {
            entity = new parameters
            {
                created_at = DateTimeOffset.UtcNow
            };
            db.Set<parameters>().Add(entity);
        }
        else
        {
            var existing = await db.Set<parameters>()
                .SingleOrDefaultAsync(a => a.id == id && (type == null || a.type == type));
            if (existing == null)
                return NotFound();
            entity = existing;
            entity.updated_at = DateTimeOffset.UtcNow;
        }

        entity.type = type ?? model.type;
        entity.value = model.value;
        entity.item_num = model.item_num;

        await db.SaveChangesAsync();
        return await get(entity.id, type);
    }
}

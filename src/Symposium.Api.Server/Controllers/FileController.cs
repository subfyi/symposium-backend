using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tamturk;
using Tamturk.AspNetCore;
using Symposium.Storage.Services;
using File = Symposium.Models.File;

namespace Symposium.Api.Server.Controllers;

[Authorize]
public class FileController : ControllerBase
{
    private readonly FileService fs;
    private readonly IRequestSigning requestSigning;
    private readonly AppDbContext db;
    private int Actor => int.Parse(User.FindFirst("user_id")!.Value);

    public FileController(FileService fs, IRequestSigning requestSigning, AppDbContext db)
    {
        this.fs = fs;
        this.requestSigning = requestSigning;
        this.db = db;
    }

    [HttpPut("file/{name}/start")]
    public async Task<IActionResult> StartUpload(string name, string hash)
    {
        ValidateName(name);
        var hashBytes = Convert.FromBase64String(hash);
        var file = await db.files.FirstOrDefaultAsync(a => a.hash == hashBytes && a.owner_id == Actor);
        if (file != null)
        {
            return Created(string.Empty, new File()
            {
                id = file.id,
                name = file.name,
                size = file.size,
                access_url = file.access_url,
            });
        }

        var url = await fs.StartUpload(name, out var id);

        return Ok(new StartUpload
        {
            upload_url = url.ToString(),
            complete_url = requestSigning.SignRequest("DELETE", $"/file/{Uri.EscapeDataString(name)}/end",
                new Dictionary<string, string>
                {
                    {"id", id.ToString()},
                    {"hash", hash}
                    , {"owner", Actor.ToString()}
                })
        });
    }

    [HttpDelete("file/{name}/end")]
    public async Task<File> upload(string name, string hash, Guid id, int owner)
    {
        Request.ValidateRequest();
        ValidateName(name);
        if (owner != Actor) throw new Symposium.Services.SubmissionPermissionException();
        var file = await fs.EndUpload(name, hash, id);
        file.owner_id = Actor;
        await db.SaveChangesAsync();

        return new File()
        {
            id = file.id,
            name = file.name,
            size = file.size,
            access_url = file.access_url,
        };
    }

    [HttpPut("file/{name}")]
    [DisableRequestSizeLimit]
    public async Task<File> upload(string name, CancellationToken ct)
    {
        ValidateName(name);
        var file = await fs.upload(Request.Body, name, Request.ContentType, ct);
        file.owner_id = Actor;
        await db.SaveChangesAsync(ct);

        return new File()
        {
            id = file.id,
            name = file.name,
            size = file.size,
            access_url = file.access_url
        };
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length > 180 || name is "." or ".." || name.IndexOfAny(['/', '\\', '\0']) >= 0)
            throw new ArgumentException("Invalid filename.");
    }
}

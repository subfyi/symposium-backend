using System.Net;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Symposium.Storage.Services;

public sealed class FileService
{
    private readonly AppDbContext db;
    private readonly WebDavClient _webDavClient;
    private readonly UrlSigner _urlSigner;
    private readonly ILogger _logger;
    private readonly string _cdnUrl;

    public FileService(AppDbContext db, WebDavClient webDavClient, string cdnUrl, string tokenKey, ILogger<FileService> logger)
    {
        this.db = db;
        this._urlSigner = new UrlSigner(tokenKey);
        this._webDavClient = webDavClient;
        this._logger = logger;
        this._cdnUrl = cdnUrl.TrimEnd('/');
    }

    public Task<vfile?> FindFileByHash(string hash)
    {
        var bytes = Convert.FromBase64String(hash);

        return db.files
            .AsQueryable()
            .Where(a => a.hash == bytes)
            .SingleOrDefaultAsync();
    }

    public Task DeleteFile(vfile file)
        => _webDavClient.Delete($"{file.folder_id}/{file.name}");

    public async Task<vfile> CopyFromUrl(Uri url, string name)
    {
        using var httpClient = new HttpClient();
        using var response = await httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStreamAsync();

        return await this.upload(body, name, response.Content.Headers.ContentType.MediaType, ct: CancellationToken.None);
    }

    public Task<Uri> StartUpload(string name, out Guid id)
    {
        id = Guid.NewGuid();

        var uploadUrl = $"{_cdnUrl}{_urlSigner.Sign($"/{id}/{name}", "u=1")}";

        return _webDavClient.Mkdir(id.ToString()).ContinueWith(_ => new Uri(uploadUrl), TaskContinuationOptions.OnlyOnRanToCompletion);
    }

    public async Task<vfile> EndUpload(string name, string? hash, Guid id)
    {
        var (contentType, size) = await _webDavClient.Head($"{id}/{name}");

        var computedHash = await Hash($"{id}/{name}");

        if (hash is not null)
        {
            var proposedHash = Convert.FromBase64String(hash);
            if (!proposedHash.SequenceEqual(computedHash))
            {
                await _webDavClient.Delete($"{id}/{name}");
                await _webDavClient.Delete($"{id}");

                throw new InvalidOperationException("Hash mismatch!");
            }
        }

        var file = new vfile()
        {
            folder_id = id.ToString(),
            name = name,
            size = size,
            content_type = contentType,
            hash = computedHash,
            created_at = DateTimeOffset.UtcNow,
            access_url = _cdnUrl + _urlSigner.Sign($"/{id}/{name}"),
            download_url = _cdnUrl + _urlSigner.Sign($"/{id}/{name}", "dl=1")
        };

        db.files.Add(file);

        await db.SaveChangesAsync();

        return file;
    }

    private async Task<byte[]> Hash(string path)
    {
        using var md5 = MD5.Create();
        await using var stream = await _webDavClient.Get(path);

        return await md5.ComputeHashAsync(stream);
    }

    public async Task<vfile> GetFile(int id)
    {
        return await db.files.SingleAsync(a => a.id == id);
    }

    public async Task<Stream> OpenRead(int id)
    {
        var file = await db.files.SingleAsync(a => a.id == id);
        return await OpenRead(file);
    }

    public Task<Stream> OpenRead(vfile file)
        => _webDavClient.Get($"{file.folder_id}/{file.name}");

    public async Task<vfile> upload(Stream data, string filename, string? mime = null, CancellationToken ct = default)
    {
        var folderId = Guid.NewGuid().ToString();

        await _webDavClient.Mkdir(folderId);

        byte[] hash;
        long size;

        using (var md5 = MD5.Create())
        {
            await using (var hashStream = new CryptoStream(data, md5, CryptoStreamMode.Read))
            {
                size = await _webDavClient.Upload($"{folderId}/{filename}", hashStream, mime, ct);
            }

            hash = md5.Hash!;
        }

        var file = new vfile()
        {
            folder_id = folderId,
            name = filename,
            size = size,
            content_type = mime,
            created_at = DateTimeOffset.UtcNow,
            access_url = _cdnUrl + _urlSigner.Sign($"/{folderId}/{filename}"),
            download_url = _cdnUrl + _urlSigner.Sign($"/{folderId}/{filename}", "dl=1"),
            hash = hash
        };

        db.files.Add(file);
        await db.SaveChangesAsync(ct);

        return file;
    }

    public async Task<vfile> UploadByUrl(Uri url)
    {
        try
        {
            using var client = new HttpClient();
            using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            await using var body = await response.Content.ReadAsStreamAsync();

            return await upload(body, WebUtility.UrlDecode(Path.GetFileName(url.AbsolutePath)), response.Content.Headers.ContentType.MediaType);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to upload file from URL: {Url}", url);
            throw;
        }
    }
}

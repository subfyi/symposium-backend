using System.Net.Http.Headers;

namespace Symposium.Storage.Services;

public sealed class WebDavClient : IDisposable
{
    private static readonly HttpMethod MKCOL = new("MKCOL");

    private readonly HttpMessageHandler _handler;
    private readonly HttpClient _httpClient;

    public WebDavClient(string server, string username, string password)
    {
        _handler = new SocketsHttpHandler()
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(5)
        };

        _httpClient = new HttpClient(_handler)
        {
            BaseAddress = new Uri(server),
            DefaultRequestHeaders =
            {
                Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{username}:{password}")))
            }
        };
    }

    public async Task<(string contentType, long size)> Head(string path)
    {
        using var request = new HttpRequestMessage(HttpMethod.Head, path);
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return (
            response.Content.Headers.ContentType?.MediaType ??
            throw new InvalidOperationException("Unexpected Content-Type"),
            response.Content.Headers.ContentLength ?? throw new InvalidOperationException("Unexpected Content-Length")
        );
    }

    public Task<Stream> Get(string path)
        => _httpClient.GetStreamAsync(path);

    public async Task Delete(string path)
    {
        var response = await _httpClient.DeleteAsync(path);
        response.EnsureSuccessStatusCode();
    }

    public async Task<long> Upload(string path, Stream body, string? mime = null, CancellationToken cancellationToken = default)
    {
        await using var countingStream = new CountingStream(body);
        using var request = new HttpRequestMessage(HttpMethod.Put, path);
        using var requestBody = new StreamContent(countingStream);

        requestBody.Headers.ContentType = new(mime ?? "application/octet-stream");
        request.Content = requestBody;

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return countingStream.BytesRead;
    }

    public async Task Mkdir(string folder)
    {
        using var request = new HttpRequestMessage(MKCOL, folder);
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    public void Dispose()
    {
        _httpClient.Dispose();
        _handler.Dispose();
    }

    private sealed class CountingStream : Stream
    {
        public override ValueTask DisposeAsync()
        {
            return _inner.DisposeAsync();
        }

        private readonly Stream _inner;
        private long _bytesRead;

        public CountingStream(Stream inner)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        public long BytesRead => Interlocked.Read(ref _bytesRead);

        public Stream InnerStream => _inner;

        public override bool CanRead => _inner.CanRead;
        public override bool CanSeek => _inner.CanSeek;
        public override bool CanWrite => _inner.CanWrite;

        public override long Length => _inner.Length;

        public override long Position
        {
            get => _inner.Position;
            set => _inner.Position = value;
        }

        public override void Flush() => _inner.Flush();

        public override int Read(byte[] buffer, int offset, int count)
        {
            int read = _inner.Read(buffer, offset, count);
            AddReadBytes(read);
            return read;
        }

        public override int ReadByte()
        {
            int b = _inner.ReadByte();
            if (b != -1) AddReadBytes(1);
            return b;
        }

        public override int Read(Span<byte> buffer)
        {
            int read = _inner.Read(buffer);
            AddReadBytes(read);
            return read;
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count,
                                                   CancellationToken cancellationToken = default)
        {
            int read = await _inner.ReadAsync(buffer, offset, count, cancellationToken);
            AddReadBytes(read);
            return read;
        }

        public override async ValueTask<int> ReadAsync(Memory<byte> buffer,
                                                        CancellationToken cancellationToken = default)
        {
            int read = await _inner.ReadAsync(buffer, cancellationToken);
            AddReadBytes(read);
            return read;
        }

        public override long Seek(long offset, SeekOrigin origin) => _inner.Seek(offset, origin);
        public override void SetLength(long value) => _inner.SetLength(value);

        public override void Write(byte[] buffer, int offset, int count) => _inner.Write(buffer, offset, count);

        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken) =>
            _inner.CopyToAsync(destination, bufferSize, cancellationToken);

        private void AddReadBytes(int count) => Interlocked.Add(ref _bytesRead, count);
    }
}

internal sealed class UrlSigner
{
    private readonly string _key;

    public UrlSigner(string key)
    {
        _key = key;
    }

    public string Sign(string path, params string[] queryParts)
    {
        var hmac = System.Security.Cryptography.HMACSHA256.HashData(
            System.Text.Encoding.UTF8.GetBytes(_key),
            System.Text.Encoding.UTF8.GetBytes(path));

        var token = Convert.ToBase64String(hmac).Replace('+', '-').Replace('/', '_').TrimEnd('=');

        var sb = new System.Text.StringBuilder();
        sb.Append(path);
        sb.Append("?token=");
        sb.Append(token);

        foreach (var part in queryParts)
        {
            sb.Append('&');
            sb.Append(part);
        }

        return sb.ToString();
    }
}
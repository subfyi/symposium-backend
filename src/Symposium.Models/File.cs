namespace Symposium.Models;

public record File
{
    public int? id { get; init; }

    public string name { get; init; }

    public long size { get; init; }

    public string access_url { get; init; }

    public string? download_url { get; init; }
}

public record StartUpload
{
    public string upload_url { get; init; }

    public string complete_url { get; init; }
}
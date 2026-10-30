namespace Symposium.Models;

public record VolumeToFiles
{
    public int? id { get; init; }

    public int? volume_id { get; init; }

    public int? d_id { get; init; }

    public int? lang_id { get; init; }

    public string? title { get; init; }

    public string? description { get; init; }

    public int? file_id { get; init; }

    public string? type { get; init; }

    public DateTimeOffset? created_at { get; init; }

    public DateTimeOffset? updated_at { get; init; }
}

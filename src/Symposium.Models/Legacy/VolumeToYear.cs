namespace Symposium.Models;

public record VolumeToYear
{
    public int? id { get; init; }

    public string? volume_num { get; init; }

    public string? year { get; init; }

    public string? short_year { get; init; }

    public DateTimeOffset? created_at { get; init; }

    public DateTimeOffset? updated_at { get; init; }
}

namespace Symposium.Models;

public record Citations
{
    public int? id { get; init; }

    public int? article_id { get; init; }

    public int? d_id { get; init; }

    public string? raw { get; init; }

    public string? type { get; init; }

    public int? order { get; init; }

    public DateTimeOffset? created_at { get; init; }

    public DateTimeOffset? updated_at { get; init; }
}

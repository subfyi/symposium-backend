namespace Symposium.Models;

public record Subjects
{
    public int? id { get; init; }

    public int? d_id { get; init; }

    public int? lang_id { get; init; }

    public string? name { get; init; }

    public DateTimeOffset? created_at { get; init; }

    public DateTimeOffset? updated_at { get; init; }
}

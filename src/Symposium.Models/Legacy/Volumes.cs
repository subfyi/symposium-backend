namespace Symposium.Models;

public record Volumes
{
    public int? id { get; init; }

    public int? journal_id { get; init; }

    public int? d_id { get; init; }

    public string? volume { get; init; }

    public string? issue { get; init; }

    public string? special { get; init; }

    public string? year { get; init; }

    public string? long_year { get; init; }

    public string? short_year { get; init; }

    public int? json_id { get; init; }

    public string? created_by { get; init; }

    public string? date_published { get; init; }

    public string? season { get; init; }

    public string? supplement { get; init; }

    public DateTimeOffset? created_at { get; init; }

    public DateTimeOffset? updated_at { get; init; }
}

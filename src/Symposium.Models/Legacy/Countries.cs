namespace Symposium.Models;

public record Countries
{
    public int? id { get; init; }

    public int? d_id { get; init; }

    public string? name { get; init; }

    public string? provinces { get; init; }

    public DateTimeOffset? created_at { get; init; }

    public DateTimeOffset? updated_at { get; init; }
}

namespace Symposium.Models;

public record Roles
{
    public int? id { get; init; }

    public string? name { get; init; }

    public DateTimeOffset? created_at { get; init; }

    public DateTimeOffset? updated_at { get; init; }
}

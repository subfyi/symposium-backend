namespace Symposium.Models;

public record Publishers
{
    public int? id { get; init; }

    public int? d_id { get; init; }

    public int? lang_id { get; init; }

    public string? name { get; init; }

    public string? email { get; init; }

    public string? lft { get; init; }

    public string? address_lat { get; init; }

    public string? address_long { get; init; }

    public DateTimeOffset? created_at { get; init; }

    public DateTimeOffset? updated_at { get; init; }
}

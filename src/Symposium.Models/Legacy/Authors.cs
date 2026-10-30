namespace Symposium.Models;

public record Authors
{
    public int? id { get; init; }

    public int? p_id { get; init; }

    public string? p_user { get; init; }

    public string? first_name { get; init; }

    public string? last_name { get; init; }

    public string? adress { get; init; }

    public string? email { get; init; }

    public string? correspond { get; init; }

    public string? presenter { get; init; }

    public DateTimeOffset? created_at { get; init; }

    public DateTimeOffset? updated_at { get; init; }
}

namespace Symposium.Models;

public record Parameters
{
    public int? id { get; init; }

    public string? type { get; init; }

    public string? value { get; init; }

    public string? item_num { get; init; }

    public DateTimeOffset? created_at { get; init; }

    public DateTimeOffset? updated_at { get; init; }
}

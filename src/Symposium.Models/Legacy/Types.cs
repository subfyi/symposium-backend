namespace Symposium.Models;

public record Types
{
    public int? id { get; init; }

    public int? d_id { get; init; }

    public string? value_tr { get; init; }

    public string? value_en { get; init; }

    public DateTimeOffset? created_at { get; init; }

    public DateTimeOffset? updated_at { get; init; }
}

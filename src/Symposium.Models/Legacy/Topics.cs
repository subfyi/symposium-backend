namespace Symposium.Models;

public record Topics
{
    public CatalogLanguage? lang { get; init; }
    public int? id { get; init; }

    public bool? active { get; init; }

    public string? type { get; init; }

    public string? name { get; init; }

    public string? created_by { get; init; }

    public string? updated_by { get; init; }

    public DateTimeOffset? created_at { get; init; }

    public DateTimeOffset? updated_at { get; init; }
}

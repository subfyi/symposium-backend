namespace Symposium.Models;

public record JournalToArticleType
{
    public int? id { get; init; }

    public int? journal_id { get; init; }

    public int? article_type_id { get; init; }

    public DateTimeOffset? created_at { get; init; }

    public DateTimeOffset? updated_at { get; init; }
}

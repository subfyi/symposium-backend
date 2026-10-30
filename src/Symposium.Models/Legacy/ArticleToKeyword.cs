namespace Symposium.Models;

public record ArticleToKeyword
{
    public int? id { get; init; }

    public int? article_id { get; init; }

    public int? keyword_id { get; init; }

    public DateTimeOffset? created_at { get; init; }

    public DateTimeOffset? updated_at { get; init; }
}

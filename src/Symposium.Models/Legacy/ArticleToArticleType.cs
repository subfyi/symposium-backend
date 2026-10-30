namespace Symposium.Models;

public record ArticleToArticleType
{
    public int? id { get; init; }

    public int? article_id { get; init; }

    public int? article_type_id { get; init; }

    public DateTimeOffset? created_at { get; init; }

    public DateTimeOffset? updated_at { get; init; }
}

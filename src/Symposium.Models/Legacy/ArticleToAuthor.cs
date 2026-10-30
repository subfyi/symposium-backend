namespace Symposium.Models;

public record ArticleToAuthor
{
    public int? id { get; init; }

    public int? article_id { get; init; }

    public int? author_id { get; init; }

    public int? order { get; init; }

    public DateTimeOffset? created_at { get; init; }

    public DateTimeOffset? updated_at { get; init; }
}

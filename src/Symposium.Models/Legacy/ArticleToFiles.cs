namespace Symposium.Models;

public record ArticleToFiles
{
    public int? id { get; init; }

    public int? article_id { get; init; }

    public int? file_id { get; init; }

    public DateTimeOffset? created_at { get; init; }

    public DateTimeOffset? updated_at { get; init; }
}

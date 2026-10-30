namespace Symposium.Models;

public record ArticleInfo
{
    public int? id { get; init; }

    public int? d_id { get; init; }

    public int? v_id { get; init; }

    public int? article_id { get; init; }

    public int? lang_id { get; init; }

    public string? title { get; init; }

    public string? @abstract { get; init; }

    public string? created_by { get; init; }

    public DateTimeOffset? created_at { get; init; }

    public DateTimeOffset? updated_at { get; init; }
}

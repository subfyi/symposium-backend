namespace Symposium.Models;

public record Yorum
{
    public int? id { get; init; }

    public string? isim { get; init; }

    public string? mail { get; init; }

    public string? icerik { get; init; }

    public string? blog { get; init; }

    public string? ust_yorum { get; init; }

    public int? kullanici_id { get; init; }

    public DateTimeOffset? created_at { get; init; }

    public DateTimeOffset? updated_at { get; init; }
}

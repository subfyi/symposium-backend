namespace Symposium.Models;

public record Feedback
{
    public int? id { get; init; }

    public string? grv_icerik_yonetici { get; init; }

    public string? grv_bitis_tarih { get; init; }

    public string? grv_onemi { get; init; }

    public string? slug { get; init; }

    public DateTimeOffset? created_at { get; init; }

    public DateTimeOffset? updated_at { get; init; }
}

namespace Symposium.Models;

public record Gorevicerik
{
    public int? id { get; init; }

    public string? isicerik { get; init; }

    public string? slug { get; init; }

    public string? ust_icerik { get; init; }

    public string? grv_icerik_olusturan { get; init; }

    public string? dosyaisimleri { get; init; }

    public string? gdosyaisimleri { get; init; }

    public string? r_dosyaisimleri { get; init; }

    public string? r_gdosyaisimleri { get; init; }

    public DateTimeOffset? created_at { get; init; }

    public DateTimeOffset? updated_at { get; init; }
}

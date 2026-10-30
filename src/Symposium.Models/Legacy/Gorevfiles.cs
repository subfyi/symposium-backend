namespace Symposium.Models;

public record Gorevfiles
{
    public int? id { get; init; }

    public int? f_icerik_id { get; init; }

    public string? f_icerik_olusturan { get; init; }

    public string? f_dosyayeri { get; init; }

    public string? r_dosyaisimi { get; init; }

    public string? f_dosyaisimi { get; init; }

    public string? f_malzeme_turu { get; init; }

    public string? f_baski_olcusu { get; init; }

    public string? f_urun_olcusu { get; init; }

    public string? f_uygulama { get; init; }

    public string? f_adet { get; init; }

    public string? slug { get; init; }

    public string? f_dosya_uzanti { get; init; }

    public string? f_dosya_boyut { get; init; }

    public string? f_urun_olcusu_b { get; init; }

    public DateTimeOffset? created_at { get; init; }

    public DateTimeOffset? updated_at { get; init; }
}

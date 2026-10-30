namespace Symposium.Models;

public record Dosyalar
{
    public int? id { get; init; }

    public string? f_icerik_olusturan { get; init; }

    public string? f_dosyayeri { get; init; }

    public string? r_dosyaisimi { get; init; }

    public string? f_dosyaisimi { get; init; }

    public string? g_dosyaismi { get; init; }

    public string? f_klasorismi { get; init; }

    public string? g_klasorismi { get; init; }

    public string? f_dosya_uzanti { get; init; }

    public string? f_dosya_boyut { get; init; }

    public int? width { get; init; }

    public int? height { get; init; }

    public int? f_sira { get; init; }

    public DateTimeOffset? created_at { get; init; }

    public DateTimeOffset? updated_at { get; init; }
}

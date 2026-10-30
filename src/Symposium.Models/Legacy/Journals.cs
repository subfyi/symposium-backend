namespace Symposium.Models;

public record Journals
{
    public int? id { get; init; }

    public int? json_id { get; init; }

    public int? d_id { get; init; }

    public int? image_id { get; init; }

    public int? image_header_id { get; init; }

    public string? founded { get; init; }

    public string? url { get; init; }

    public string? address { get; init; }

    public string? phone { get; init; }

    public string? email { get; init; }

    public string? eissn { get; init; }

    public string? pissn { get; init; }

    public string? slug { get; init; }

    public string? printed { get; init; }

    public string? mandatory_lang { get; init; }

    public string? mandatory_rtl { get; init; }

    public string? view_count { get; init; }

    public string? download_count { get; init; }

    public string? created_by { get; init; }

    public string? updated_by { get; init; }

    public DateTimeOffset? created_at { get; init; }

    public DateTimeOffset? updated_at { get; init; }
}

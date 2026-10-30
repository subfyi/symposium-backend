namespace Symposium.Models;

public record JournalInfo
{
    public int? id { get; init; }

    public int? journal_id { get; init; }

    public int? d_id { get; init; }

    public int? lang_id { get; init; }

    public string? title { get; init; }

    public string? subtitle { get; init; }

    public string? description { get; init; }

    public string? title_abbr { get; init; }

    public string? footer_text { get; init; }

    public string? mail_signature { get; init; }

    public string? aim { get; init; }

    public string? scope { get; init; }

    public string? writing_rules { get; init; }

    public string? policy { get; init; }

    public DateTimeOffset? created_at { get; init; }

    public DateTimeOffset? updated_at { get; init; }
}

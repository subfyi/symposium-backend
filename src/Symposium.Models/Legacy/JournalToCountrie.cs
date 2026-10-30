namespace Symposium.Models;

public record JournalToCountrie
{
    public int? id { get; init; }

    public int? journal_id { get; init; }

    public int? countrie_id { get; init; }

    public DateTimeOffset? created_at { get; init; }

    public DateTimeOffset? updated_at { get; init; }
}

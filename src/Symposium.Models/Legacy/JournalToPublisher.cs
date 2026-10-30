namespace Symposium.Models;

public record JournalToPublisher
{
    public int? id { get; init; }

    public int? journal_id { get; init; }

    public int? publisher_id { get; init; }

    public DateTimeOffset? created_at { get; init; }

    public DateTimeOffset? updated_at { get; init; }
}

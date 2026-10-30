namespace Symposium.Models;

public record Language
{
    public int? id { get; init; }

    public bool? enabled { get; set; }

    public int? priority { get; set; }

    public Language parent { get; init; }

    public string name { get; init; }

    public string language_name { get; init; }

    public string language_code { get; init; }

    public File icon { get; init; }

    public DateTimeOffset? created_at { get; init; }

    public DateTimeOffset? updated_at { get; init; }

    public Dictionary<string, string>? values { get; init; }
}
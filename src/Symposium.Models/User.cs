namespace Symposium.Models;

public record User
{
    public string? name { get; init; }
    public string? surname { get; init; }
    public int? role { get; init; }
    public string? address { get; init; }
    public string? institution { get; init; }
    public string? faculty { get; init; }
    public string? department { get; init; }
    public string? iletisim { get; init; }
    public int? gender { get; init; }
    public int? u_title { get; init; }
    public int? u_type { get; init; }
    public PublicArticleParameter? utitle { get; init; }
    public PublicArticleParameter? utype { get; init; }
    public PublicArticleParameter? ugender { get; init; }
    public int? id { get; init; }

    public string? first_name { get; init; }

    public string? last_name { get; init; }

    public string email { get; init; }

    public bool? is_password_present { get; init; }

    public string? password { get; init; }

    public DateTimeOffset? password_updated_at { get; init; }

    public UserType? type { get; init; }

    public DateTimeOffset? created_at { get; init; }
}

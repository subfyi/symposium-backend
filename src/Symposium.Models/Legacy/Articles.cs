using System.Text.Json;
using System.Text.Json.Serialization;

namespace Symposium.Models;

// Public responses deliberately exclude submitters, payments, comments and email addresses.
public record PublicArticle
{
    public int id { get; init; }
    public string? en_title { get; init; }
    public string? en_abstract { get; init; }
    public string? pap_keyword { get; init; }
    public string? slug { get; init; }
    public string? title_year { get; init; }
    public string? paper_page { get; init; }
    public string? paper_page_end { get; init; }
    public string? paper_abst_page { get; init; }
    public string? pap_num { get; init; }
    public string? paper_external { get; init; }
    public IReadOnlyCollection<PublicArticleAuthor> authors { get; init; } = [];
    public PublicArticleParameter? topic { get; init; }
    public PublicArticleParameter? parampre { get; init; }
    public PublicArticleParameter? parampap { get; init; }
    public PublicArticleFile? abstract_dosya { get; init; }
    public PublicArticleFile? full_paper_dosya { get; init; }
    public PublicArticleFile? poster_presentation_dosya { get; init; }
    public PublicArticleFile? video { get; init; }
}

public record PublicArticleAuthor
{
    public int id { get; init; }
    public string? first_name { get; init; }
    public string? last_name { get; init; }
    public string? adress { get; init; }
}

public record PublicArticleParameter
{
    public int id { get; init; }
    public string? value { get; init; }
}

public record PublicArticleFile
{
    public int id { get; init; }
    public string? name { get; init; }
    public string? size { get; init; }
    public string? access_url { get; init; }
}

public record Articles
{
    public IReadOnlyCollection<SubmissionComment>? icerikler { get; init; }
    public IReadOnlyCollection<SubmissionAuthor>? authors { get; init; }
    public PublicArticleParameter? topic { get; init { field = value; topic_specified = true; } }
    public PublicArticleParameter? parampre { get; init { field = value; parampre_specified = true; } }
    public PublicArticleParameter? parampap { get; init { field = value; parampap_specified = true; } }
    public SubmissionUser? user { get; init { field = value; user_specified = true; } }
    public SubmissionFile? abstract_dosya { get; init { field = value; abstract_specified = true; } }
    public SubmissionFile? full_paper_dosya { get; init { field = value; full_paper_specified = true; } }
    public SubmissionFile? poster_presentation_dosya { get; init { field = value; poster_specified = true; } }
    public SubmissionFile? video { get; init { field = value; video_specified = true; } }
    [JsonIgnore] public bool topic_specified { get; private set; }
    [JsonIgnore] public bool parampre_specified { get; private set; }
    [JsonIgnore] public bool parampap_specified { get; private set; }
    [JsonIgnore] public bool user_specified { get; private set; }
    [JsonIgnore] public bool abstract_specified { get; private set; }
    [JsonIgnore] public bool full_paper_specified { get; private set; }
    [JsonIgnore] public bool poster_specified { get; private set; }
    [JsonIgnore] public bool video_specified { get; private set; }

    public int? id { get; init; }

    public string? en_title { get; init; }

    public string? pap_topic { get; init; }

    public string? pap_creater { get; init; }

    public string? pap_type { get; init; }

    public string? pre_type { get; init; }

    public string? slug { get; init; }

    public string? pap_status { get; init; }

    public string? pap_payment { get; init; }

    public string? en_abstract { get; init; }

    public string? pap_keyword { get; init; }

    public string? pap_topic_p { get; init; }

    public string? mes_congress { get; init; }

    public int? abstract_dosya_id { get; init; }

    public int? full_paper_dosya_id { get; init; }

    public int? poster_presentation_id { get; init; }

    public int? video_id { get; init; }

    [JsonConverter(typeof(SubmissionBooleanConverter))]
    public bool? video_approved { get; init; }

    public string? title_year { get; init; }

    [JsonConverter(typeof(SubmissionBooleanConverter))]
    public bool? paper_approved { get; init; }

    public string? paper_page { get; init; }
    public string? paper_page_end { get; init; }
    public string? paper_abst_page { get; init; }
    public string? pap_num { get; init; }
    public string? paper_external { get; init; }

    public DateTimeOffset? created_at { get; init; }

    public DateTimeOffset? updated_at { get; init; }
}

public record SubmissionAuthor
{
    public int? id { get; init; }
    public string? first_name { get; init; }
    public string? last_name { get; init; }
    public string? adress { get; init; }
    public string? email { get; init; }
    [JsonConverter(typeof(SubmissionBooleanConverter))]
    public bool? correspond { get; init; }
    [JsonConverter(typeof(SubmissionBooleanConverter))]
    public bool? presenter { get; init; }
}

public record SubmissionUser
{
    public int? id { get; init; }
    public string? name { get; init; }
    public string? surname { get; init; }
    public string? email { get; init; }
}

public record SubmissionFile : PublicArticleFile
{
    public int? storage_file_id { get; init; }
    public string? f_klasorismi { get; init; }
    public string? f_dosyaisimi { get; init; }
    public string? g_dosyaismi { get; init; }
}

public record SubmissionComment
{
    public int? id { get; init; }
    public string? isicerik { get; init; }
    public SubmissionUser? user { get; init; }
    public DateTimeOffset? created_at { get; init; }
    public DateTimeOffset? updated_at { get; init; }
}

public record CatalogLanguage
{
    public int? id { get; init; }
    public string? name { get; init; }
    public string? code { get; init; }
    public bool? active { get; init; }
    public bool? rtl { get; init; }
}

// The legacy React editor sends approval flags as 0/1 and author flags as booleans.
public sealed class SubmissionBooleanConverter : JsonConverter<bool?>
{
    public override bool? Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options) => reader.TokenType switch
    {
        JsonTokenType.Null => null,
        JsonTokenType.True => true,
        JsonTokenType.False => false,
        JsonTokenType.Number when reader.TryGetInt32(out var number) && (number == 0 || number == 1) => number == 1,
        JsonTokenType.String when reader.GetString() is "0" or "false" => false,
        JsonTokenType.String when reader.GetString() is "1" or "true" => true,
        _ => throw new JsonException("Expected a boolean or 0/1.")
    };

    public override void Write(Utf8JsonWriter writer, bool? value, JsonSerializerOptions options)
    {
        if (value is bool flag) writer.WriteBooleanValue(flag);
        else writer.WriteNullValue();
    }
}

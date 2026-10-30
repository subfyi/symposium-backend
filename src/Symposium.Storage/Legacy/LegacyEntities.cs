#pragma warning disable CS8981
using System.ComponentModel.DataAnnotations;

namespace Symposium.Storage;

public class article_info
{
    [Key] public int id { get; set; }
    public int? d_id { get; set; }
    public int? v_id { get; set; }
    public int? article_id { get; set; }
    public int? lang_id { get; set; }
    public string? title { get; set; }
    public string? @abstract { get; set; }
    public string? created_by { get; set; }
    public DateTimeOffset? created_at { get; set; }
    public DateTimeOffset? updated_at { get; set; }
}

public class article_to_article_type
{
    [Key] public int id { get; set; }
    public int? article_id { get; set; }
    public int? article_type_id { get; set; }
    public DateTimeOffset? created_at { get; set; }
    public DateTimeOffset? updated_at { get; set; }
}

public class article_to_author
{
    [Key] public int id { get; set; }
    public int? article_id { get; set; }
    public int? author_id { get; set; }
    public int? order { get; set; }
    public DateTimeOffset? created_at { get; set; }
    public DateTimeOffset? updated_at { get; set; }
}

public class article_to_citation
{
    [Key] public int id { get; set; }
    public int? article_id { get; set; }
    public int? citation_id { get; set; }
    public DateTimeOffset? created_at { get; set; }
    public DateTimeOffset? updated_at { get; set; }
}

public class article_to_files
{
    [Key] public int id { get; set; }
    public int? article_id { get; set; }
    public int? file_id { get; set; }
    public DateTimeOffset? created_at { get; set; }
    public DateTimeOffset? updated_at { get; set; }
}

public class article_to_keyword
{
    [Key] public int id { get; set; }
    public int? article_id { get; set; }
    public int? keyword_id { get; set; }
    public DateTimeOffset? created_at { get; set; }
    public DateTimeOffset? updated_at { get; set; }
}

public class article_to_subject
{
    [Key] public int id { get; set; }
    public int? article_id { get; set; }
    public int? subject_id { get; set; }
    public DateTimeOffset? created_at { get; set; }
    public DateTimeOffset? updated_at { get; set; }
}

public class article_types
{
    [Key] public int id { get; set; }
    public int? d_id { get; set; }
    public int? lang_id { get; set; }
    public string? name { get; set; }
    public DateTimeOffset? created_at { get; set; }
    public DateTimeOffset? updated_at { get; set; }
}

public class articles
{
    [Key] public int id { get; set; }
    public string? en_title { get; set; }
    public string? pap_topic { get; set; }
    public string? pap_creater { get; set; }
    public string? pap_type { get; set; }
    public string? pre_type { get; set; }
    public string? slug { get; set; }
    public string? pap_status { get; set; }
    public string? pap_payment { get; set; }
    public string? en_abstract { get; set; }
    public string? pap_keyword { get; set; }
    public string? pap_topic_p { get; set; }
    public string? mes_congress { get; set; }
    public int? abstract_dosya_id { get; set; }
    public int? full_paper_dosya_id { get; set; }
    public int? poster_presentation_id { get; set; }
    public int? video_id { get; set; }
    public bool? video_approved { get; set; }
    public string? title_year { get; set; }
    public bool? paper_approved { get; set; }
    public string? paper_page { get; set; }
    public string? paper_page_end { get; set; }
    public string? paper_abst_page { get; set; }
    public string? paper_external { get; set; }
    public string? pap_num { get; set; }
    public DateTimeOffset? created_at { get; set; }
    public DateTimeOffset? updated_at { get; set; }
}

public class authors
{
    public int sort_order { get; set; }
    [Key] public int id { get; set; }
    public int? p_id { get; set; }
    public string? p_user { get; set; }
    public string? first_name { get; set; }
    public string? last_name { get; set; }
    public string? adress { get; set; }
    public string? email { get; set; }
    public string? correspond { get; set; }
    public string? presenter { get; set; }
    public DateTimeOffset? created_at { get; set; }
    public DateTimeOffset? updated_at { get; set; }
}

public class citations
{
    [Key] public int id { get; set; }
    public int? article_id { get; set; }
    public int? d_id { get; set; }
    public string? raw { get; set; }
    public string? type { get; set; }
    public int? order { get; set; }
    public DateTimeOffset? created_at { get; set; }
    public DateTimeOffset? updated_at { get; set; }
}

public class countries
{
    [Key] public int id { get; set; }
    public int? d_id { get; set; }
    public string? name { get; set; }
    public string? provinces { get; set; }
    public DateTimeOffset? created_at { get; set; }
    public DateTimeOffset? updated_at { get; set; }
}

public class dosyalar
{
    public int? storage_file_id { get; set; }
    public virtual vfile? storage_file { get; set; }
    [Key] public int id { get; set; }
    public string? f_icerik_olusturan { get; set; }
    public string? f_dosyayeri { get; set; }
    public string? r_dosyaisimi { get; set; }
    public string? f_dosyaisimi { get; set; }
    public string? g_dosyaismi { get; set; }
    public string? f_klasorismi { get; set; }
    public string? g_klasorismi { get; set; }
    public string? f_dosya_uzanti { get; set; }
    public string? f_dosya_boyut { get; set; }
    public int? width { get; set; }
    public int? height { get; set; }
    public int? f_sira { get; set; }
    public DateTimeOffset? created_at { get; set; }
    public DateTimeOffset? updated_at { get; set; }
}

public class feedbacks
{
    [Key] public int id { get; set; }
    public string? grv_icerik_yonetici { get; set; }
    public string? grv_bitis_tarih { get; set; }
    public string? grv_onemi { get; set; }
    public string? slug { get; set; }
    public DateTimeOffset? created_at { get; set; }
    public DateTimeOffset? updated_at { get; set; }
}

public class feedbackdata
{
    [Key] public int id { get; set; }
    public string? isicerik { get; set; }
    public string? slug { get; set; }
    public string? ust_icerik { get; set; }
    public string? grv_icerik_olusturan { get; set; }
    public string? dosyaisimleri { get; set; }
    public string? gdosyaisimleri { get; set; }
    public string? r_dosyaisimleri { get; set; }
    public string? r_gdosyaisimleri { get; set; }
    public DateTimeOffset? created_at { get; set; }
    public DateTimeOffset? updated_at { get; set; }
}

public class gorev_files
{
    [Key] public int id { get; set; }
    public int? f_icerik_id { get; set; }
    public string? f_icerik_olusturan { get; set; }
    public string? f_dosyayeri { get; set; }
    public string? r_dosyaisimi { get; set; }
    public string? f_dosyaisimi { get; set; }
    public string? f_malzeme_turu { get; set; }
    public string? f_baski_olcusu { get; set; }
    public string? f_urun_olcusu { get; set; }
    public string? f_uygulama { get; set; }
    public string? f_adet { get; set; }
    public string? slug { get; set; }
    public string? f_dosya_uzanti { get; set; }
    public string? f_dosya_boyut { get; set; }
    public string? f_urun_olcusu_b { get; set; }
    public DateTimeOffset? created_at { get; set; }
    public DateTimeOffset? updated_at { get; set; }
}

public class gorev_icerik
{
    [Key] public int id { get; set; }
    public string? isicerik { get; set; }
    public string? slug { get; set; }
    public string? ust_icerik { get; set; }
    public string? grv_icerik_olusturan { get; set; }
    public string? dosyaisimleri { get; set; }
    public string? gdosyaisimleri { get; set; }
    public string? r_dosyaisimleri { get; set; }
    public string? r_gdosyaisimleri { get; set; }
    public DateTimeOffset? created_at { get; set; }
    public DateTimeOffset? updated_at { get; set; }
}

public class journal_info
{
    [Key] public int id { get; set; }
    public int? journal_id { get; set; }
    public int? d_id { get; set; }
    public int? lang_id { get; set; }
    public string? title { get; set; }
    public string? subtitle { get; set; }
    public string? description { get; set; }
    public string? title_abbr { get; set; }
    public string? footer_text { get; set; }
    public string? mail_signature { get; set; }
    public string? aim { get; set; }
    public string? scope { get; set; }
    public string? writing_rules { get; set; }
    public string? policy { get; set; }
    public DateTimeOffset? created_at { get; set; }
    public DateTimeOffset? updated_at { get; set; }
}

public class journal_to_article_type
{
    [Key] public int id { get; set; }
    public int? journal_id { get; set; }
    public int? article_type_id { get; set; }
    public DateTimeOffset? created_at { get; set; }
    public DateTimeOffset? updated_at { get; set; }
}

public class journal_to_countrie
{
    [Key] public int id { get; set; }
    public int? journal_id { get; set; }
    public int? countrie_id { get; set; }
    public DateTimeOffset? created_at { get; set; }
    public DateTimeOffset? updated_at { get; set; }
}

public class journal_to_publisher
{
    [Key] public int id { get; set; }
    public int? journal_id { get; set; }
    public int? publisher_id { get; set; }
    public DateTimeOffset? created_at { get; set; }
    public DateTimeOffset? updated_at { get; set; }
}

public class journal_to_subject
{
    [Key] public int id { get; set; }
    public int? journal_id { get; set; }
    public int? subject_id { get; set; }
    public DateTimeOffset? created_at { get; set; }
    public DateTimeOffset? updated_at { get; set; }
}

public class journals
{
    [Key] public int id { get; set; }
    public int? json_id { get; set; }
    public int? d_id { get; set; }
    public int? image_id { get; set; }
    public int? image_header_id { get; set; }
    public string? founded { get; set; }
    public string? url { get; set; }
    public string? address { get; set; }
    public string? phone { get; set; }
    public string? email { get; set; }
    public string? eissn { get; set; }
    public string? pissn { get; set; }
    public string? slug { get; set; }
    public string? printed { get; set; }
    public string? mandatory_lang { get; set; }
    public string? mandatory_rtl { get; set; }
    public string? view_count { get; set; }
    public string? download_count { get; set; }
    public string? created_by { get; set; }
    public string? updated_by { get; set; }
    public DateTimeOffset? created_at { get; set; }
    public DateTimeOffset? updated_at { get; set; }
}

public class keywords
{
    [Key] public int id { get; set; }
    public int? lang_id { get; set; }
    public string? name { get; set; }
    public DateTimeOffset? created_at { get; set; }
    public DateTimeOffset? updated_at { get; set; }
}

public class parameters
{
    [Key] public int id { get; set; }
    public string? type { get; set; }
    public string? value { get; set; }
    public string? item_num { get; set; }
    public DateTimeOffset? created_at { get; set; }
    public DateTimeOffset? updated_at { get; set; }
}

public class presentations
{
    [Key] public int id { get; set; }
    public bool? active { get; set; }
    public string? type { get; set; }
    public string? name { get; set; }
    public string? created_by { get; set; }
    public string? updated_by { get; set; }
    public DateTimeOffset? created_at { get; set; }
    public DateTimeOffset? updated_at { get; set; }
}

public class publishers
{
    [Key] public int id { get; set; }
    public int? d_id { get; set; }
    public int? lang_id { get; set; }
    public string? name { get; set; }
    public string? email { get; set; }
    public string? lft { get; set; }
    public string? address_lat { get; set; }
    public string? address_long { get; set; }
    public DateTimeOffset? created_at { get; set; }
    public DateTimeOffset? updated_at { get; set; }
}

public class publishs
{
    [Key] public int id { get; set; }
    public bool? active { get; set; }
    public string? type { get; set; }
    public string? name { get; set; }
    public string? created_by { get; set; }
    public string? updated_by { get; set; }
    public DateTimeOffset? created_at { get; set; }
    public DateTimeOffset? updated_at { get; set; }
}

public class roles
{
    [Key] public int id { get; set; }
    public string? name { get; set; }
    public DateTimeOffset? created_at { get; set; }
    public DateTimeOffset? updated_at { get; set; }
}

public class subjects
{
    [Key] public int id { get; set; }
    public int? d_id { get; set; }
    public int? lang_id { get; set; }
    public string? name { get; set; }
    public DateTimeOffset? created_at { get; set; }
    public DateTimeOffset? updated_at { get; set; }
}

public class topics
{
    [Key] public int id { get; set; }
    public bool? active { get; set; }
    public string? type { get; set; }
    public string? name { get; set; }
    public string? created_by { get; set; }
    public string? updated_by { get; set; }
    public DateTimeOffset? created_at { get; set; }
    public DateTimeOffset? updated_at { get; set; }
}

public class types
{
    [Key] public int id { get; set; }
    public int? d_id { get; set; }
    public string? value_tr { get; set; }
    public string? value_en { get; set; }
    public DateTimeOffset? created_at { get; set; }
    public DateTimeOffset? updated_at { get; set; }
}

public class volume_to_files
{
    [Key] public int id { get; set; }
    public int? volume_id { get; set; }
    public int? d_id { get; set; }
    public int? lang_id { get; set; }
    public string? title { get; set; }
    public string? description { get; set; }
    public int? file_id { get; set; }
    public string? type { get; set; }
    public DateTimeOffset? created_at { get; set; }
    public DateTimeOffset? updated_at { get; set; }
}

public class volume_to_year
{
    [Key] public int id { get; set; }
    public string? volume_num { get; set; }
    public string? year { get; set; }
    public string? short_year { get; set; }
    public DateTimeOffset? created_at { get; set; }
    public DateTimeOffset? updated_at { get; set; }
}

public class volumes
{
    [Key] public int id { get; set; }
    public int? journal_id { get; set; }
    public int? d_id { get; set; }
    public string? volume { get; set; }
    public string? issue { get; set; }
    public string? special { get; set; }
    public string? year { get; set; }
    public string? long_year { get; set; }
    public string? short_year { get; set; }
    public int? json_id { get; set; }
    public string? created_by { get; set; }
    public string? date_published { get; set; }
    public string? season { get; set; }
    public string? supplement { get; set; }
    public DateTimeOffset? created_at { get; set; }
    public DateTimeOffset? updated_at { get; set; }
}

public class yorumlar
{
    [Key] public int id { get; set; }
    public string? isim { get; set; }
    public string? mail { get; set; }
    public string? icerik { get; set; }
    public string? blog { get; set; }
    public string? ust_yorum { get; set; }
    public int? kullanici_id { get; set; }
    public DateTimeOffset? created_at { get; set; }
    public DateTimeOffset? updated_at { get; set; }
}

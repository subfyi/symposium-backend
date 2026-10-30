using EntityFrameworkCore.Projectables;
using Symposium.Models;
using Symposium.Storage;

namespace Symposium.Query.Projections;

public static class ArticlesExtensions
{
    public static IQueryable<articles> PublishedArticles(this IQueryable<articles> source,
        string? year = null, bool orals = false, bool posters = false, bool presentation = false)
    {
        return source
            .Where(a => a.paper_approved == true && a.pap_status != "3")
            .Where(a => year == null || a.title_year == year)
            .Where(a => !orals || a.pre_type == "463" || a.pre_type == "116" || a.pre_type == "97")
            .Where(a => !posters || a.pre_type == "98")
            .Where(a => !presentation || a.video_approved == true);
    }

    // Explicit correlated projections avoid adding relationships to the legacy storage model.
    public static IQueryable<PublicArticle> ToPublicArticles(this IQueryable<articles> source, AppDbContext db)
    {
        return source.Select(a => new PublicArticle
        {
            id = a.id,
            en_title = a.en_title,
            en_abstract = a.en_abstract,
            pap_keyword = a.pap_keyword,
            slug = a.slug,
            title_year = a.title_year,
            paper_page = a.paper_page,
            paper_page_end = a.paper_page_end,
            paper_abst_page = a.paper_abst_page,
            pap_num = a.pap_num,
            paper_external = a.paper_external,
            authors = db.Set<authors>().Where(b => b.p_id == a.id).OrderBy(b => b.sort_order).ThenBy(b => b.id)
                .Select(b => new PublicArticleAuthor
                {
                    id = b.id, first_name = b.first_name, last_name = b.last_name, adress = b.adress
                }).ToList(),
            topic = db.Set<parameters>().Where(b => b.id.ToString() == a.pap_topic_p)
                .Select(b => new PublicArticleParameter { id = b.id, value = b.value }).FirstOrDefault(),
            parampre = db.Set<parameters>().Where(b => b.id.ToString() == a.pre_type)
                .Select(b => new PublicArticleParameter { id = b.id, value = b.value }).FirstOrDefault(),
            parampap = db.Set<parameters>().Where(b => b.id.ToString() == a.pap_type)
                .Select(b => new PublicArticleParameter { id = b.id, value = b.value }).FirstOrDefault(),
            abstract_dosya = db.Set<dosyalar>().Where(b => b.id == a.abstract_dosya_id)
                .Select(b => b.ToPublicArticleFile()).FirstOrDefault(),
            full_paper_dosya = db.Set<dosyalar>().Where(b => b.id == a.full_paper_dosya_id)
                .Select(b => b.ToPublicArticleFile()).FirstOrDefault(),
            poster_presentation_dosya = db.Set<dosyalar>().Where(b => b.id == a.poster_presentation_id)
                .Select(b => b.ToPublicArticleFile()).FirstOrDefault(),
            video = db.Set<dosyalar>().Where(b => a.video_approved == true && b.id == a.video_id)
                .Select(b => b.ToPublicArticleFile()).FirstOrDefault()
        });
    }

    public static IQueryable<Articles> ToAdminArticles(this IQueryable<articles> source, AppDbContext db)
    {
        return source.Select(a => new Articles
        {
            id = a.id,
            en_title = a.en_title,
            pap_topic = a.pap_topic,
            pap_creater = a.pap_creater,
            pap_type = a.pap_type,
            pre_type = a.pre_type,
            slug = a.slug,
            pap_status = a.pap_status,
            pap_payment = a.pap_payment,
            en_abstract = a.en_abstract,
            pap_keyword = a.pap_keyword,
            pap_topic_p = a.pap_topic_p,
            mes_congress = a.mes_congress,
            abstract_dosya_id = a.abstract_dosya_id,
            full_paper_dosya_id = a.full_paper_dosya_id,
            poster_presentation_id = a.poster_presentation_id,
            video_id = a.video_id,
            video_approved = a.video_approved,
            title_year = a.title_year,
            paper_approved = a.paper_approved,
            paper_page = a.paper_page,
            paper_page_end = a.paper_page_end,
            paper_abst_page = a.paper_abst_page,
            pap_num = a.pap_num,
            paper_external = a.paper_external,
            created_at = a.created_at,
            updated_at = a.updated_at,
            icerikler = db.Set<submission_comment>().Where(b => b.article_id == a.id).OrderBy(b => b.created_at).ThenBy(b => b.id)
                .Select(b => new SubmissionComment { id = b.id, isicerik = b.body, created_at = b.created_at, updated_at = b.updated_at,
                    user = db.users.Where(c => c.id == b.author_id).Select(c => new SubmissionUser { id = c.id, name = c.first_name, surname = c.last_name }).FirstOrDefault() }).ToList(),
            authors = db.Set<authors>().Where(b => b.p_id == a.id).OrderBy(b => b.sort_order).ThenBy(b => b.id)
                .Select(b => new SubmissionAuthor
                {
                    id = b.id, first_name = b.first_name, last_name = b.last_name,
                    email = b.email, adress = b.adress,
                    correspond = b.correspond == "1" || b.correspond == "true",
                    presenter = b.presenter == "1" || b.presenter == "true"
                }).ToList(),
            user = db.Set<user>().Where(b => b.id.ToString() == a.pap_creater)
                .Select(b => new SubmissionUser { id = b.id, name = b.first_name, surname = b.last_name, email = b.email })
                .FirstOrDefault() ?? new SubmissionUser(),
            topic = db.Set<parameters>().Where(b => b.id.ToString() == a.pap_topic_p)
                .Select(b => new PublicArticleParameter { id = b.id, value = b.value }).FirstOrDefault(),
            parampre = db.Set<parameters>().Where(b => b.id.ToString() == a.pre_type)
                .Select(b => new PublicArticleParameter { id = b.id, value = b.value }).FirstOrDefault(),
            parampap = db.Set<parameters>().Where(b => b.id.ToString() == a.pap_type)
                .Select(b => new PublicArticleParameter { id = b.id, value = b.value }).FirstOrDefault(),
            abstract_dosya = db.Set<dosyalar>().Where(b => b.id == a.abstract_dosya_id)
                .Select(b => b.ToSubmissionFile()).FirstOrDefault(),
            full_paper_dosya = db.Set<dosyalar>().Where(b => b.id == a.full_paper_dosya_id)
                .Select(b => b.ToSubmissionFile()).FirstOrDefault(),
            poster_presentation_dosya = db.Set<dosyalar>().Where(b => b.id == a.poster_presentation_id)
                .Select(b => b.ToSubmissionFile()).FirstOrDefault(),
            video = db.Set<dosyalar>().Where(b => b.id == a.video_id)
                .Select(b => b.ToSubmissionFile()).FirstOrDefault()
        });
    }

    [Projectable]
    public static SubmissionFile ToSubmissionFile(this dosyalar file) => new SubmissionFile
    {
        id = file.id,
        storage_file_id = file.storage_file_id,
        name = file.r_dosyaisimi,
        size = file.f_dosya_boyut,
        f_klasorismi = file.f_klasorismi,
        f_dosyaisimi = file.f_dosyaisimi,
        g_dosyaismi = file.g_dosyaismi,
        access_url = file.storage_file != null ? file.storage_file.access_url : file.f_dosyayeri == "2"
            ? (file.g_dosyaismi == null ? null : "https://drive.google.com/file/d/" + file.g_dosyaismi + "/preview")
            : (file.f_klasorismi == null || file.f_dosyaisimi == null ? null
                : "/uploads/dosya/" + file.f_klasorismi + "/" + file.f_dosyaisimi)
    };

    [Projectable]
    public static PublicArticleFile ToPublicArticleFile(this dosyalar file) => new PublicArticleFile
    {
        id = file.id,
        name = file.r_dosyaisimi,
        size = file.f_dosya_boyut,
        access_url = file.storage_file != null ? file.storage_file.access_url : file.f_dosyayeri == "2"
            ? (file.g_dosyaismi == null ? null : "https://drive.google.com/file/d/" + file.g_dosyaismi + "/preview")
            : (file.f_klasorismi == null || file.f_dosyaisimi == null ? null
                : "/uploads/dosya/" + file.f_klasorismi + "/" + file.f_dosyaisimi)
    };

    [Projectable(NullConditionalRewriteSupport = NullConditionalRewriteSupport.Rewrite)]
    public static Articles ToArticles(this articles a) => new Articles
    {
        id = a.id,
        en_title = a.en_title,
        pap_topic = a.pap_topic,
        pap_creater = a.pap_creater,
        pap_type = a.pap_type,
        pre_type = a.pre_type,
        slug = a.slug,
        pap_status = a.pap_status,
        pap_payment = a.pap_payment,
        en_abstract = a.en_abstract,
        pap_keyword = a.pap_keyword,
        pap_topic_p = a.pap_topic_p,
        mes_congress = a.mes_congress,
        abstract_dosya_id = a.abstract_dosya_id,
        full_paper_dosya_id = a.full_paper_dosya_id,
        poster_presentation_id = a.poster_presentation_id,
        video_id = a.video_id,
        video_approved = a.video_approved,
        title_year = a.title_year,
        paper_approved = a.paper_approved,
        paper_page = a.paper_page,
        paper_page_end = a.paper_page_end,
        paper_abst_page = a.paper_abst_page,
        pap_num = a.pap_num,
        paper_external = a.paper_external,
        created_at = a.created_at,
        updated_at = a.updated_at
    };
}

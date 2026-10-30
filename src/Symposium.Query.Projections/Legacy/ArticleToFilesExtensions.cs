using EntityFrameworkCore.Projectables;
using Symposium.Models;
using Symposium.Storage;

namespace Symposium.Query.Projections;

public static class ArticleToFilesExtensions
{
    [Projectable(NullConditionalRewriteSupport = NullConditionalRewriteSupport.Rewrite)]
    public static ArticleToFiles ToArticleToFiles(this article_to_files a) => new ArticleToFiles
    {
        id = a.id,
        article_id = a.article_id,
        file_id = a.file_id,
        created_at = a.created_at,
        updated_at = a.updated_at
    };
}

using EntityFrameworkCore.Projectables;
using Symposium.Models;
using Symposium.Storage;

namespace Symposium.Query.Projections;

public static class LanguageExtensions
{
    [Projectable(NullConditionalRewriteSupport = NullConditionalRewriteSupport.Rewrite)]
    public static Language ToLanguage(this language a) => new Language
    {
        id = a.id,
        enabled = a.enabled,
        priority = a.priority,
        name = a.name,
        language_code = a.language_code,
        language_name = a.language_name,
        icon = a.icon.ToFile(),
        parent = a.parent?.ToLanguageWoParent(),
        created_at = a.created_at,
        updated_at = a.updated_at
    };

    [Projectable(NullConditionalRewriteSupport = NullConditionalRewriteSupport.Rewrite)]
    public static Language ToLanguageWoParent(this language a) => new Language
    {
        id = a.id,
        enabled = a.enabled,
        priority = a.priority,
        name = a.name,
        language_code = a.language_code,
        language_name = a.language_name,
        icon = a.icon.ToFile(),
        created_at = a.created_at,
        updated_at = a.updated_at
    };
}
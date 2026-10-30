using EntityFrameworkCore.Projectables;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;

namespace Symposium.Storage.Extensions;

public static class MultiStringExtensions
{
    [Projectable]
    public static bool Like(this multi_string ms, string query)
        => EF.Functions.ILike(ms.en, query);

    [Projectable]
    public static NpgsqlTsQuery TsQuery(string config, string query) =>
        EF.Functions.ToTsQuery((string)(object)EF.Functions.WebSearchToTsQuery(config, query) + ":*");

    [Projectable]
    public static bool FullTextSearch(this multi_string ms, string query) =>
        EF.Functions.ToTsVector("english", ms.en).Matches(TsQuery("english", query))
        || ms.en == query;

    [Projectable]
    public static float FullTextSearchRank(this multi_string ms, string query) =>
       EF.Functions.ToTsVector("english", ms.en).Rank(TsQuery("english", query));

    public static multi_string Join(this IEnumerable<multi_string> ms, string space)
    {
        return new multi_string()
        {
            en = string.Join(space, ms.Select(a => a.en)),
        };
    }
}
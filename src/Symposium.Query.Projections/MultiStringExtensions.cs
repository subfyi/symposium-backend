using EntityFrameworkCore.Projectables;
using Symposium.Models;
using Symposium.Storage;

namespace Symposium.Query.Projections;

public static class MultiStringExtensions
{
    [Projectable]
    public static MultiString ToMultiString(this multi_string ms) => new MultiString
    {
        en = ms.en
    };

    public static multi_string ToMultiString(this MultiString ms)
    {
        return new multi_string
        {
            en = ms.en
        };
    }
}
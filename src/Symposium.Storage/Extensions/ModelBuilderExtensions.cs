using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Linq.Expressions;

namespace Symposium.Storage.Extensions;

public static class ModelBuilderExtensions
{
    private static readonly IEnumerable<(string, Expression<Func<multi_string, object?>>)> exps =
    [
        ("english", a => a.en)
    ];

    public static EntityTypeBuilder<T> HasContent<T>(this EntityTypeBuilder<T> model,
        Expression<Func<T, content?>> expression) where T : class
    {
        var builder = model.OwnsOne(expression);

        builder.WithOwner();

        builder.HasMultiString(a => a.body, max: true);

        return model;
    }

    public static EntityTypeBuilder<T> HasMultiString<T>(this EntityTypeBuilder<T> model,
        Expression<Func<T, multi_string?>> expression, bool max = false, bool index = false, bool uniq = false, bool fullText = false) where T : class
    {
        var builder = model.OwnsOne(expression);

        builder.WithOwner();

        if (max)
        {
            foreach (var exp in exps)
            {
                builder.Property(exp.Item2)
                    .HasMaxLength(int.MaxValue);
            }
        }

        if (uniq || index || fullText)
        {
            foreach (var exp in exps)
            {
                var idx = builder
                    .HasIndex(exp.Item2)
                    .IsUnique(uniq);

                if (fullText)
                {
                    idx.HasMethod("GIN")
                        .IsTsVectorExpressionIndex(exp.Item1);
                }
            }
        }

        return model;
    }

    public static OwnedNavigationBuilder<T, X> HasMultiString<T, X>(this OwnedNavigationBuilder<T, X> model,
        Expression<Func<X, multi_string>> expression, bool max = false, bool index = false, bool uniq = false, bool fullText = false) where T : class where X : class
    {
        var builder = model.OwnsOne(expression);

        builder.WithOwner();

        if (max)
        {
            foreach (var exp in exps)
            {
                builder.Property(exp.Item2)
                    .HasMaxLength(int.MaxValue);
            }
        }

        if (uniq || index || fullText)
        {
            foreach (var exp in exps)
            {
                var idx = builder
                    .HasIndex(exp.Item2)
                    .IsUnique(uniq);

                if (fullText)
                {
                    idx.HasMethod("GIN")
                        .IsTsVectorExpressionIndex(exp.Item1);
                }
            }
        }

        return model;
    }
}
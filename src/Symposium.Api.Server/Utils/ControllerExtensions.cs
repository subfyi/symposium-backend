using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Reflection;

namespace Symposium.Api.Server.Utils;

public record Pagination<T>
{
    public int currentPage { get; init; }

    public int pageCount { get; init; }

    public int itemCount { get; init; }

    public IReadOnlyCollection<T> data { get; init; }

    public Pagination<Y> Select<Y>(Func<T, Y> item)
    {
        return new Pagination<Y>
        {
            currentPage = this.currentPage,
            pageCount = this.pageCount,
            itemCount = this.itemCount,
            data = data
                .Select(item)
                .ToList()
        };
    }
}

public static class ControllerBaseExtensions
{
    public static async Task<Pagination<T>> Paginate<X, T>(
        this ControllerBase controller,
        IQueryable<X> queryable,
        Func<IQueryable<X>, IQueryable<T>> after,
        bool orderSupported = true,
        IReadOnlyDictionary<string, Expression<Func<T, object>>> orderMapping = null,
        CancellationToken token = default)
    {
        if (!int.TryParse(controller.Request.Query["page"], out int currentPage))
        {
            currentPage = 1;
        }

        if (!int.TryParse(controller.Request.Query["itemPerPage"], out int itemPerPage))
        {
            itemPerPage = 15;
        }

        IQueryable<T> q = after(queryable);
        ValidatePagination(currentPage, itemPerPage);

        if (orderSupported && controller.Request.Query.TryGetValue("sort", out var order))
        {
            bool desc = true;

            if (controller.Request.Query.TryGetValue("desc", out var strDesc))
            {
                if (!bool.TryParse(strDesc.ToString(), out desc))
                    throw new ArgumentException("desc must be true or false.");
            }

            if (orderMapping?.TryGetValue(order, out var map) == true)
            {
                if (desc)
                {
                    q = q.OrderByDescending(map);
                }
                else
                {
                    q = q.OrderBy(map);
                }
            }
            else
            {
                q = q.OrderBy(order, desc);
            }
        }

        if (itemPerPage == -1)
        {
            var data = await q
                .ToListAsync(token);

            return new Pagination<T>()
            {
                currentPage = 1,
                pageCount = 1,
                itemCount = data.Count,
                data = data
            };
        }

        var count = await queryable.CountAsync(token);

        return new Pagination<T>()
        {
            currentPage = currentPage,
            pageCount = (int)Math.Ceiling(count / (decimal)itemPerPage),
            itemCount = count,
            data = await q
                .Skip((currentPage - 1) * itemPerPage)
                .Take(itemPerPage)
                .ToListAsync(token)
        };
    }

    public static async Task<Pagination<T>> Paginate<T>(this ControllerBase controller, IQueryable<T> queryable,
        bool orderSupported = true)
    {
        if (!int.TryParse(controller.Request.Query["page"], out int currentPage))
        {
            currentPage = 1;
        }

        if (!int.TryParse(controller.Request.Query["itemPerPage"], out int itemPerPage))
        {
            itemPerPage = 15;
        }

        if (orderSupported == true && controller.Request.Query.TryGetValue("sort", out var order))
        {
            bool desc = true;

            if (controller.Request.Query.TryGetValue("desc", out var strDesc))
            {
                if (!bool.TryParse(strDesc.ToString(), out desc))
                    throw new ArgumentException("desc must be true or false.");
            }

            queryable = queryable.OrderBy(order, desc);
        }

        ValidatePagination(currentPage, itemPerPage);

        if (itemPerPage == -1)
        {
            return new Pagination<T>()
            {
                currentPage = 1,
                pageCount = 1,
                itemCount = await queryable.CountAsync(),
                data = await queryable
                    .ToListAsync()
            };
        }

        var count = await queryable.CountAsync();
        return new Pagination<T>()
        {
            currentPage = currentPage,
            pageCount = (int)Math.Ceiling(count / (decimal)itemPerPage),
            itemCount = count,
            data = await queryable
                .Skip((currentPage - 1) * itemPerPage)
                .Take(itemPerPage)
                .ToListAsync()
        };
    }

    public static IQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> source, string property,
        bool desc)
    {
        string methodName = desc ? "OrderByDescending" : "OrderBy";

        string[] props = property.Split('.');
        Type type = typeof(TEntity);
        ParameterExpression arg = Expression.Parameter(type, "x");
        Expression expr = arg;
        foreach (string prop in props)
        {
            PropertyInfo pi = type.GetProperty(prop);
            if (pi is null)
                throw new ArgumentException("Sorting by this column is not supported: " + property);

            expr = Expression.Property(expr, pi);
            type = pi.PropertyType;
        }
        Type delegateType = typeof(Func<,>).MakeGenericType(typeof(TEntity), type);
        LambdaExpression lambda = Expression.Lambda(delegateType, expr, arg);

        object result = typeof(Queryable).GetMethods().Single(
                method => method.Name == methodName
                          && method.IsGenericMethodDefinition
                          && method.GetGenericArguments().Length == 2
                          && method.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(TEntity), type)
            .Invoke(null, new object[] { source, lambda });
        return (IOrderedQueryable<TEntity>)result;
    }

    private static void ValidatePagination(int page, int itemPerPage)
    {
        if (page < 1 || (itemPerPage != -1 && (itemPerPage < 1 || itemPerPage > 1000)))
            throw new ArgumentException("page must be positive; itemPerPage must be -1 or between 1 and 1000.");
    }
}

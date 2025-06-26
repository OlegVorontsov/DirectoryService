using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Application.Shared.Extensions;

public static class IQueryableExtensions
{
    public static IQueryable<T> ILike<T, TProp>(
        this IQueryable<T> collection,
        Expression<Func<T, TProp>> propSelector,
        string search) // where TProp : convertable to string
    {
        // Where(d => EF.Functions.ILike((string)d.Name, $"%{query.Search}%")
        // ILike(d => d.Name, query.Search)

        // d
        var parameter = propSelector.Parameters[0];

        // EF.Functions.ILike
        var method = typeof(NpgsqlDbFunctionsExtensions)
            .GetMethod("ILike", [typeof(DbFunctions), typeof(string), typeof(string)]);

        // d.Name
        var property = propSelector.Body;

        // (string)d.Name
        var parameterAsString = Expression.Convert(property, typeof(string));

        var searchString = Expression.Constant($"%{search}%");

        // required parameter for ILike function (discarded in declaration)
        var efFunctionsProperty = Expression.Property(null, typeof(EF).GetProperty("Functions")!);

        var methodCall = Expression.Call(method!, efFunctionsProperty, parameterAsString, searchString);

        var lambdaExpression = Expression.Lambda<Func<T, bool>>(methodCall, parameter);
        return collection.Where(lambdaExpression);
    }
}
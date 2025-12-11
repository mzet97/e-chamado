using System.Linq.Expressions;

namespace EChamado.Shared.Domain.Patterns;

public static class ExpressionExtensions
{
    public static Expression<Func<T, bool>> AndAlso<T>(
        this Expression<Func<T, bool>> left,
        Expression<Func<T, bool>> right)
    {
        var param = Expression.Parameter(typeof(T));
        var body = Expression.AndAlso(
            Expression.Invoke(left, param),
            Expression.Invoke(right, param));

        return Expression.Lambda<Func<T, bool>>(body, param);
    }

    public static Expression<Func<T, bool>> OrElse<T>(
        this Expression<Func<T, bool>> left,
        Expression<Func<T, bool>> right)
    {
        var param = Expression.Parameter(typeof(T));
        var body = Expression.OrElse(
            Expression.Invoke(left, param),
            Expression.Invoke(right, param));

        return Expression.Lambda<Func<T, bool>>(body, param);
    }

    public static Expression<Func<T, bool>> Not<T>(
        this Expression<Func<T, bool>> expr)
    {
        var param = Expression.Parameter(typeof(T));
        var body = Expression.Not(Expression.Invoke(expr, param));
        return Expression.Lambda<Func<T, bool>>(body, param);
    }
}

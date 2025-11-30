using System.Linq.Expressions;

namespace CodeOps.Domain.Abstractions.Specifications
{
    internal static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> left,
        Expression<Func<T, bool>> right)
        {
            var param = Expression.Parameter(typeof(T), "x");


            var leftBody = ReplaceParameter(left.Body, left.Parameters[0], param);
            var rightBody = ReplaceParameter(right.Body, right.Parameters[0], param);


            var body = Expression.AndAlso(leftBody, rightBody);
            return Expression.Lambda<Func<T, bool>>(body, param);
        }

        public static Expression<Func<T, bool>> OrElse<T>(this Expression<Func<T, bool>> left,
        Expression<Func<T, bool>> right)
        {
            var param = Expression.Parameter(typeof(T), "x");


            var leftBody = ReplaceParameter(left.Body, left.Parameters[0], param);
            var rightBody = ReplaceParameter(right.Body, right.Parameters[0], param);


            var body = Expression.OrElse(leftBody, rightBody);
            return Expression.Lambda<Func<T, bool>>(body, param);
        }

        private static Expression ReplaceParameter(Expression expression, ParameterExpression source, ParameterExpression target)
        {
            return new ParameterReplaceVisitor(source, target).Visit(expression)!;
        }
    }
}

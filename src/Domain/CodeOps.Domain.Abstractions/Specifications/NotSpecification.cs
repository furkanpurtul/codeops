using System.Linq.Expressions;

namespace CodeOps.Domain.Abstractions.Specifications
{
    internal sealed class NotSpecification<T> : Specification<T>
    {
        private readonly ISpecification<T> _inner;

        public NotSpecification(ISpecification<T> inner)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        public override Expression<Func<T, bool>> ToExpression()
        {
            var expr = _inner.ToExpression();
            var parameter = expr.Parameters[0];
            var body = Expression.Not(expr.Body);
            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }
    }
}

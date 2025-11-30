using System.Linq.Expressions;

namespace CodeOps.Domain.Abstractions.Specifications
{
    public sealed class ExpressionSpecification<T> : Specification<T>
    {
        private readonly Expression<Func<T, bool>> _expression;

        public ExpressionSpecification(Expression<Func<T, bool>> expression)
        {
            _expression = expression ?? throw new ArgumentNullException(nameof(expression));
        }

        public override Expression<Func<T, bool>> ToExpression() => _expression;
    }
}

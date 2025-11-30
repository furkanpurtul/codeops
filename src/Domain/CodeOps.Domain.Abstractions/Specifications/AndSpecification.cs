using System.Linq.Expressions;

namespace CodeOps.Domain.Abstractions.Specifications
{
    internal sealed class AndSpecification<T> : Specification<T>
    {
        private readonly ISpecification<T> _left;
        private readonly ISpecification<T> _right;

        public AndSpecification(ISpecification<T> left, ISpecification<T> right)
        {
            _left = left ?? throw new ArgumentNullException(nameof(left));
            _right = right ?? throw new ArgumentNullException(nameof(right));
        }

        public override Expression<Func<T, bool>> ToExpression()
        {
            return _left.ToExpression().AndAlso(_right.ToExpression());
        }
    }
}

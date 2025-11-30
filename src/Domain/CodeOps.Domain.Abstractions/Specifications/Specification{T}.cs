using System.Linq.Expressions;

namespace CodeOps.Domain.Abstractions.Specifications
{
    public abstract class Specification<T> : ISpecification<T>
    {
        private Func<T, bool>? _compiled; // lazy compiled delegate for IsSatisfiedBy

        public abstract Expression<Func<T, bool>> ToExpression();

        public bool IsSatisfiedBy(T entity)
        {
            // compile once per instance
            _compiled ??= ToExpression().Compile();


            return _compiled(entity);
        }

        public ISpecification<T> And(ISpecification<T> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            return new AndSpecification<T>(this, other);
        }

        public ISpecification<T> Or(ISpecification<T> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            return new OrSpecification<T>(this, other);
        }

        public ISpecification<T> Not()
        {
            return new NotSpecification<T>(this);
        }

        // Implicit conversion to Expression for convenience
        public static implicit operator Expression<Func<T, bool>>(Specification<T> spec) => spec.ToExpression();
    }
}

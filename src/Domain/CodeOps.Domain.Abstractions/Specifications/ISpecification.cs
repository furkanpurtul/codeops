using System.Linq.Expressions;

namespace CodeOps.Domain.Abstractions.Specifications
{
    public interface ISpecification<T>
    {
        /// <summary>
        /// Predicate as an Expression (suitable for IQueryable providers).
        /// </summary>
        Expression<Func<T, bool>> ToExpression();

        /// <summary>
        /// Evaluates the specification in-memory (compiled expression).
        /// </summary>
        bool IsSatisfiedBy(T entity);

        /// <summary>
        /// Combine with another specification using logical AND.
        /// </summary>
        ISpecification<T> And(ISpecification<T> other);

        /// <summary>
        /// Combine with another specification using logical OR.
        /// </summary>
        ISpecification<T> Or(ISpecification<T> other);

        /// <summary>
        /// Negates this specification.
        /// </summary>
        ISpecification<T> Not();
    }
}

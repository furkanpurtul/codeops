using System.Linq.Expressions;

namespace CodeOps.Domain.Abstractions.Specifications
{
    public static class Specification
    {
        /// <summary>
        /// Create from expression. Call: Specification.Of&lt;T&gt;(x => x.IsActive)
        /// </summary>
        public static ISpecification<T> Of<T>(Expression<Func<T, bool>> expr) => new ExpressionSpecification<T>(expr);


        /// <summary>
        /// Always true - useful for building dynamic query scaffolding.
        /// </summary>
        public static ISpecification<T> True<T>() => new ExpressionSpecification<T>(x => true);


        /// <summary>
        /// Always false - useful for building dynamic query scaffolding.
        /// </summary>
        public static ISpecification<T> False<T>() => new ExpressionSpecification<T>(x => false);
    }
}

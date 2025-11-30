namespace CodeOps.Domain.Abstractions.Specifications
{
    public static class LinqSpecificationExtensions
    {
        public static IQueryable<T> Where<T>(this IQueryable<T> query, ISpecification<T> specification)
        {
            if (specification == null) throw new ArgumentNullException(nameof(specification));
            return query.Where(specification.ToExpression());
        }
        public static IEnumerable<T> Where<T>(this IEnumerable<T> source, ISpecification<T> specification)
        {
            if (specification == null) throw new ArgumentNullException(nameof(specification));
            var predicate = specification.ToExpression().Compile();
            return source.Where(predicate);
        }
    }
}

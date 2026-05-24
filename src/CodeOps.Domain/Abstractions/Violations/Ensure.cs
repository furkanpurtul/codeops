namespace CodeOps.Domain.Abstractions.Violations
{
    public static class Ensure
    {
        public static EnsureContext<TSource> For<TSource>(string? method = null)
        {
            return new EnsureContext<TSource>(new DomainViolationSource(typeof(TSource).Name, method));
        }
    }
}
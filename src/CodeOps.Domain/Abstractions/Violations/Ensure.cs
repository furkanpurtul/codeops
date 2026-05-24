using System.Runtime.CompilerServices;

namespace CodeOps.Domain.Abstractions.Violations
{
    public static class Ensure
    {
        public static EnsureContext<TSource> For<TSource>([CallerMemberName] string operationName = "")
        {
            var source = new DomainViolationSource
            (
                typeof(TSource).Name,
                string.IsNullOrWhiteSpace(operationName) ? null : operationName
            );

            var context = new EnsureContext<TSource>(source);

            return context;
        }
    }
}
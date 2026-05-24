namespace CodeOps.Domain.Abstractions.Violations.Extensions
{
    public static class PredicateExtensions
    {
        public static IConstraintContext<TSource, TValue> Must<TSource, TValue>
        (
            this IConstraintContext<TSource, TValue> context,
            Func<TValue, bool> predicate,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(predicate);

            if (!predicate(context.Value))
                context.AddViolation(message ?? $"'{context.MemberName}' is invalid.");

            return context;
        }

        public static IConstraintContext<TSource, TValue> MustNot<TSource, TValue>
        (
            this IConstraintContext<TSource, TValue> context,
            Func<TValue, bool> predicate,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(predicate);

            if (predicate(context.Value))
                context.AddViolation(message ?? $"'{context.MemberName}' is invalid.");

            return context;
        }
    }
}
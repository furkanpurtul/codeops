namespace CodeOps.Domain.Abstractions.Violations.Extensions
{
    public static class EqualityExtensions
    {
        public static IConstraintContext<TSource, TValue> EqualTo<TSource, TValue>
        (
            this IConstraintContext<TSource, TValue> context,
            TValue expected,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);

            if (!EqualityComparer<TValue>.Default.Equals(context.Value, expected))
                context.AddViolation(message ?? $"'{context.MemberName}' must be equal to '{expected}'.");

            return context;
        }

        public static IConstraintContext<TSource, TValue> NotEqualTo<TSource, TValue>
        (
            this IConstraintContext<TSource, TValue> context,
            TValue unexpected,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);

            if (EqualityComparer<TValue>.Default.Equals(context.Value, unexpected))
                context.AddViolation(message ?? $"'{context.MemberName}' cannot be equal to '{unexpected}'.");

            return context;
        }
    }
}
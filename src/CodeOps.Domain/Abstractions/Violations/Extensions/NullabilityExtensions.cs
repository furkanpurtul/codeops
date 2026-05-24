namespace CodeOps.Domain.Abstractions.Violations.Extensions
{
    public static class NullabilityExtensions
    {
        public static IConstraintContext<TSource, TValue> NotNull<TSource, TValue>
        (
            this IConstraintContext<TSource, TValue> context,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.Value is null)
                context.AddViolation(message ?? $"'{context.MemberName}' cannot be null.");

            return context;
        }

        public static IConstraintContext<TSource, TValue> Null<TSource, TValue>
        (
            this IConstraintContext<TSource, TValue> context,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.Value is not null)
                context.AddViolation(message ?? $"'{context.MemberName}' must be null.");

            return context;
        }
    }
}
namespace CodeOps.Domain.Abstractions.Violations.Extensions
{
    public static class DefaultExtensions
    {
        public static IConstraintContext<TSource, TValue> NotDefault<TSource, TValue>
        (
            this IConstraintContext<TSource, TValue> context,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);

            if (EqualityComparer<TValue>.Default.Equals(context.Value, default))
                context.AddViolation(message ?? $"'{context.MemberName}' cannot be default value.");

            return context;
        }

        public static IConstraintContext<TSource, TValue> Default<TSource, TValue>
        (
            this IConstraintContext<TSource, TValue> context,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);

            if (!EqualityComparer<TValue>.Default.Equals(context.Value, default))
                context.AddViolation(message ?? $"'{context.MemberName}' must be default value.");

            return context;
        }
    }
}
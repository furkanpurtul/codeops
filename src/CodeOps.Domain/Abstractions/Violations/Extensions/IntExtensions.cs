namespace CodeOps.Domain.Abstractions.Violations.Extensions
{
    public static class IntExtensions
    {
        public static IConstraintContext<TSource, int> Positive<TSource>
        (
            this IConstraintContext<TSource, int> context,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.Value <= 0)
                context.AddViolation(message ?? $"'{context.MemberName}' must be positive.");

            return context;
        }

        public static IConstraintContext<TSource, int> Negative<TSource>
        (
            this IConstraintContext<TSource, int> context,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.Value >= 0)
                context.AddViolation(message ?? $"'{context.MemberName}' must be negative.");

            return context;
        }

        public static IConstraintContext<TSource, int> Zero<TSource>
        (
            this IConstraintContext<TSource, int> context,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.Value != 0)
                context.AddViolation(message ?? $"'{context.MemberName}' must be zero.");

            return context;
        }

        public static IConstraintContext<TSource, int> NotZero<TSource>
        (
            this IConstraintContext<TSource, int> context,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.Value == 0)
                context.AddViolation(message ?? $"'{context.MemberName}' cannot be zero.");

            return context;
        }
    }
}
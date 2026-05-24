namespace CodeOps.Domain.Abstractions.Violations.Extensions
{
    public static class TimeSpanExtensions
    {
        public static IConstraintContext<TSource, TimeSpan> Positive<TSource>
        (
            this IConstraintContext<TSource, TimeSpan> context,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.Value <= TimeSpan.Zero)
                context.AddViolation(message ?? $"'{context.MemberName}' must be positive.");

            return context;
        }

        public static IConstraintContext<TSource, TimeSpan> Negative<TSource>
        (
            this IConstraintContext<TSource, TimeSpan> context,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.Value >= TimeSpan.Zero)
                context.AddViolation(message ?? $"'{context.MemberName}' must be negative.");

            return context;
        }

        public static IConstraintContext<TSource, TimeSpan> Zero<TSource>
        (
            this IConstraintContext<TSource, TimeSpan> context,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.Value != TimeSpan.Zero)
                context.AddViolation(message ?? $"'{context.MemberName}' must be zero.");

            return context;
        }

        public static IConstraintContext<TSource, TimeSpan> NotZero<TSource>
        (
            this IConstraintContext<TSource, TimeSpan> context,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.Value == TimeSpan.Zero)
                context.AddViolation(message ?? $"'{context.MemberName}' cannot be zero.");

            return context;
        }
    }
}
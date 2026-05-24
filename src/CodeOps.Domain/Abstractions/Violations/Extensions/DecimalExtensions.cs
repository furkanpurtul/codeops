namespace CodeOps.Domain.Abstractions.Violations.Extensions
{
    public static class DecimalExtensions
    {
        public static IConstraintContext<TSource, decimal> Positive<TSource>
        (
            this IConstraintContext<TSource, decimal> context,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.Value <= 0)
                context.AddViolation(message ?? $"'{context.MemberName}' must be positive.");

            return context;
        }

        public static IConstraintContext<TSource, decimal> NotPositive<TSource>
        (
            this IConstraintContext<TSource, decimal> context,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.Value > 0)
                context.AddViolation(message ?? $"'{context.MemberName}' cannot be positive.");

            return context;
        }

        public static IConstraintContext<TSource, decimal> Negative<TSource>
        (
            this IConstraintContext<TSource, decimal> context,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.Value >= 0)
                context.AddViolation(message ?? $"'{context.MemberName}' must be negative.");

            return context;
        }

        public static IConstraintContext<TSource, decimal> NotNegative<TSource>
        (
            this IConstraintContext<TSource, decimal> context,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.Value < 0)
                context.AddViolation(message ?? $"'{context.MemberName}' cannot be negative.");

            return context;
        }

        public static IConstraintContext<TSource, decimal> Zero<TSource>
        (
            this IConstraintContext<TSource, decimal> context,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.Value != 0)
                context.AddViolation(message ?? $"'{context.MemberName}' must be zero.");

            return context;
        }

        public static IConstraintContext<TSource, decimal> NotZero<TSource>
        (
            this IConstraintContext<TSource, decimal> context,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.Value == 0)
                context.AddViolation(message ?? $"'{context.MemberName}' cannot be zero.");

            return context;
        }

        public static IConstraintContext<TSource, decimal> Scale<TSource>
        (
            this IConstraintContext<TSource, decimal> context,
            int maximumScale,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentOutOfRangeException.ThrowIfNegative(maximumScale);

            var scale = decimal.GetBits(context.Value)[3] >> 16 & 0x7F;

            if (scale > maximumScale)
                context.AddViolation(message ?? $"'{context.MemberName}' cannot have more than {maximumScale} decimal places.");

            return context;
        }
    }
}
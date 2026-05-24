namespace CodeOps.Domain.Abstractions.Violations.Extensions
{
    public static class GuardDecimalExtensions
    {
        public static GuardContext<TSource, decimal> NotNegative<TSource>(this GuardContext<TSource, decimal> context, string? message = null)
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.Value < 0)
                context.AddViolation(nameof(NotNegative), message ?? $"'{context.MemberName}' cannot be negative.");

            return context;
        }

        public static GuardContext<TSource, decimal> GreaterThanZero<TSource>(this GuardContext<TSource, decimal> context, string? message = null)
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.Value <= 0)
                context.AddViolation(nameof(GreaterThanZero), message ?? $"'{context.MemberName}' must be greater than zero.");

            return context;
        }

        public static GuardContext<TSource, decimal> Between<TSource>(this GuardContext<TSource, decimal> context, decimal min, decimal max, string? message = null)
        {
            ArgumentNullException.ThrowIfNull(context);

            if (max < min)
                throw new ArgumentOutOfRangeException(nameof(max), "Max cannot be less than min.");

            if (context.Value < min || context.Value > max)
                context.AddViolation(nameof(Between), message ?? $"'{context.MemberName}' must be between {min} and {max}.");

            return context;
        }
    }
}
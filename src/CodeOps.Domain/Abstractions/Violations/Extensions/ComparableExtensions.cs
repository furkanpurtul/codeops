namespace CodeOps.Domain.Abstractions.Violations.Extensions
{
    public static class ComparableExtensions
    {
        public static IConstraintContext<TSource, TValue> GreaterThan<TSource, TValue>
        (
            this IConstraintContext<TSource, TValue> context,
            TValue minimum,
            string? message = null
        ) where TValue : IComparable<TValue>
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.Value.CompareTo(minimum) <= 0)
                context.AddViolation(message ?? $"'{context.MemberName}' must be greater than '{minimum}'.");

            return context;
        }

        public static IConstraintContext<TSource, TValue> GreaterThanOrEqualTo<TSource, TValue>
        (
            this IConstraintContext<TSource, TValue> context,
            TValue minimum,
            string? message = null
        ) where TValue : IComparable<TValue>
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.Value.CompareTo(minimum) < 0)
                context.AddViolation(message ?? $"'{context.MemberName}' must be greater than or equal to '{minimum}'.");

            return context;
        }

        public static IConstraintContext<TSource, TValue> LessThan<TSource, TValue>
        (
            this IConstraintContext<TSource, TValue> context,
            TValue maximum,
            string? message = null
        ) where TValue : IComparable<TValue>
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.Value.CompareTo(maximum) >= 0)
                context.AddViolation(message ?? $"'{context.MemberName}' must be less than '{maximum}'.");

            return context;
        }

        public static IConstraintContext<TSource, TValue> LessThanOrEqualTo<TSource, TValue>
        (
            this IConstraintContext<TSource, TValue> context,
            TValue maximum,
            string? message = null
        ) where TValue : IComparable<TValue>
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.Value.CompareTo(maximum) > 0)
                context.AddViolation(message ?? $"'{context.MemberName}' must be less than or equal to '{maximum}'.");

            return context;
        }

        public static IConstraintContext<TSource, TValue> Between<TSource, TValue>
        (
            this IConstraintContext<TSource, TValue> context,
            TValue minimum,
            TValue maximum,
            string? message = null
        ) where TValue : IComparable<TValue>
        {
            ArgumentNullException.ThrowIfNull(context);

            if (maximum.CompareTo(minimum) < 0)
                throw new ArgumentOutOfRangeException(nameof(maximum), "Maximum cannot be less than minimum.");

            if (context.Value.CompareTo(minimum) < 0 || context.Value.CompareTo(maximum) > 0)
                context.AddViolation(message ?? $"'{context.MemberName}' must be between '{minimum}' and '{maximum}'.");

            return context;
        }
    }
}
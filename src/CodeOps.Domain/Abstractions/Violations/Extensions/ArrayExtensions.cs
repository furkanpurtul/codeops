namespace CodeOps.Domain.Abstractions.Violations.Extensions
{
    public static class ArrayExtensions
    {
        public static IConstraintContext<TSource, TItem[]> NotEmpty<TSource, TItem>
        (
            this IConstraintContext<TSource, TItem[]> context,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.Value.Length == 0)
                context.AddViolation(message ?? $"'{context.MemberName}' cannot be empty.");

            return context;
        }

        public static IConstraintContext<TSource, TItem[]> MinimumLength<TSource, TItem>
        (
            this IConstraintContext<TSource, TItem[]> context,
            int minimumLength,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentOutOfRangeException.ThrowIfNegative(minimumLength);

            if (context.Value.Length < minimumLength)
                context.AddViolation(message ?? $"'{context.MemberName}' must contain at least {minimumLength} items.");

            return context;
        }

        public static IConstraintContext<TSource, TItem[]> MaximumLength<TSource, TItem>
        (
            this IConstraintContext<TSource, TItem[]> context,
            int maximumLength,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentOutOfRangeException.ThrowIfNegative(maximumLength);

            if (context.Value.Length > maximumLength)
                context.AddViolation(message ?? $"'{context.MemberName}' cannot contain more than {maximumLength} items.");

            return context;
        }

        public static IConstraintContext<TSource, TItem[]> LengthBetween<TSource, TItem>
        (
            this IConstraintContext<TSource, TItem[]> context,
            int minimumLength,
            int maximumLength,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentOutOfRangeException.ThrowIfNegative(minimumLength);
            ArgumentOutOfRangeException.ThrowIfNegative(maximumLength);

            if (maximumLength < minimumLength)
                throw new ArgumentOutOfRangeException(nameof(maximumLength), "Maximum length cannot be less than minimum length.");

            if (context.Value.Length < minimumLength || context.Value.Length > maximumLength)
            {
                context.AddViolation(
                    message ?? $"'{context.MemberName}' must contain between {minimumLength} and {maximumLength} items.");
            }

            return context;
        }
    }
}
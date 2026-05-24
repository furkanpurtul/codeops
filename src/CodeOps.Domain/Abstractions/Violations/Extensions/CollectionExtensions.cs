namespace CodeOps.Domain.Abstractions.Violations.Extensions
{
    public static class CollectionExtensions
    {
        public static IConstraintContext<TSource, IReadOnlyCollection<TItem>> NotEmpty<TSource, TItem>
        (
            this IConstraintContext<TSource, IReadOnlyCollection<TItem>> context,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.Value.Count == 0)
                context.AddViolation(message ?? $"'{context.MemberName}' cannot be empty.");

            return context;
        }

        public static IConstraintContext<TSource, IReadOnlyCollection<TItem>> MinimumCount<TSource, TItem>
        (
            this IConstraintContext<TSource, IReadOnlyCollection<TItem>> context,
            int minimumCount,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentOutOfRangeException.ThrowIfNegative(minimumCount);

            if (context.Value.Count < minimumCount)
                context.AddViolation(message ?? $"'{context.MemberName}' must contain at least {minimumCount} items.");

            return context;
        }

        public static IConstraintContext<TSource, IReadOnlyCollection<TItem>> MaximumCount<TSource, TItem>
        (
            this IConstraintContext<TSource, IReadOnlyCollection<TItem>> context,
            int maximumCount,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentOutOfRangeException.ThrowIfNegative(maximumCount);

            if (context.Value.Count > maximumCount)
                context.AddViolation(message ?? $"'{context.MemberName}' cannot contain more than {maximumCount} items.");

            return context;
        }

        public static IConstraintContext<TSource, IReadOnlyCollection<TItem>> CountBetween<TSource, TItem>
        (
            this IConstraintContext<TSource, IReadOnlyCollection<TItem>> context,
            int minimumCount,
            int maximumCount,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentOutOfRangeException.ThrowIfNegative(minimumCount);
            ArgumentOutOfRangeException.ThrowIfNegative(maximumCount);

            if (maximumCount < minimumCount)
                throw new ArgumentOutOfRangeException(nameof(maximumCount), "Maximum count cannot be less than minimum count.");

            if (context.Value.Count < minimumCount || context.Value.Count > maximumCount)
            {
                context.AddViolation(
                    message ?? $"'{context.MemberName}' must contain between {minimumCount} and {maximumCount} items.");
            }

            return context;
        }
    }
}
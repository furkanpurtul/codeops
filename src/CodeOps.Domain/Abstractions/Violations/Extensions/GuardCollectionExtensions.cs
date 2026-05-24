namespace CodeOps.Domain.Abstractions.Violations.Extensions
{
    public static class GuardCollectionExtensions
    {
        public static GuardContext<TSource, IReadOnlyCollection<TItem>> NotEmpty<TSource, TItem>
        (
            this GuardContext<TSource, 
            IReadOnlyCollection<TItem>> context, string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.Value.Count == 0)
                context.AddViolation(nameof(NotEmpty), message ?? $"'{context.MemberName}' cannot be empty.");

            return context;
        }

        public static GuardContext<TSource, IReadOnlyCollection<TItem>> MinCount<TSource, TItem>
        (
            this GuardContext<TSource, IReadOnlyCollection<TItem>> context,
            int minCount,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentOutOfRangeException.ThrowIfNegative(minCount);

            if (context.Value.Count < minCount)
                context.AddViolation(nameof(MinCount), message ?? $"'{context.MemberName}' must contain at least {minCount} items.");

            return context;
        }

        public static GuardContext<TSource, IReadOnlyCollection<TItem>> MaxCount<TSource, TItem>
        (
            this GuardContext<TSource, IReadOnlyCollection<TItem>> context,
            int maxCount,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentOutOfRangeException.ThrowIfNegative(maxCount);

            if (context.Value.Count > maxCount)
                context.AddViolation(nameof(MaxCount), message ?? $"'{context.MemberName}' cannot contain more than {maxCount} items.");

            return context;
        }

        public static GuardContext<TSource, IReadOnlyCollection<TItem>> CountBetween<TSource, TItem>
        (
            this GuardContext<TSource, IReadOnlyCollection<TItem>> context,
            int minCount,
            int maxCount,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentOutOfRangeException.ThrowIfNegative(minCount);
            ArgumentOutOfRangeException.ThrowIfNegative(maxCount);

            if (maxCount < minCount)
                throw new ArgumentOutOfRangeException(nameof(maxCount), "Max count cannot be less than min count.");

            if (context.Value.Count < minCount || context.Value.Count > maxCount)
                context.AddViolation(nameof(CountBetween), message ?? $"'{context.MemberName}' must contain between {minCount} and {maxCount} items.");

            return context;
        }
    }
}
using CodeOps.Domain.Abstractions.Violations.Extensions;

namespace CodeOps.Domain.Abstractions.Violations.Extensions
{
    public static class EnumerableExtensions
    {
        public static IConstraintContext<TSource, IEnumerable<TItem>> NotEmpty<TSource, TItem>
        (
            this IConstraintContext<TSource, IEnumerable<TItem>> context,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);

            if (!context.Value.Any())
                context.AddViolation(message ?? $"'{context.MemberName}' cannot be empty.");

            return context;
        }

        public static IConstraintContext<TSource, IEnumerable<TItem>> All<TSource, TItem>
        (
            this IConstraintContext<TSource, IEnumerable<TItem>> context,
            Func<TItem, bool> predicate,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(predicate);

            if (!context.Value.All(predicate))
                context.AddViolation(message ?? $"All items in '{context.MemberName}' must satisfy the condition.");

            return context;
        }

        public static IConstraintContext<TSource, IEnumerable<TItem>> Any<TSource, TItem>
        (
            this IConstraintContext<TSource, IEnumerable<TItem>> context,
            Func<TItem, bool> predicate,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(predicate);

            if (!context.Value.Any(predicate))
                context.AddViolation(message ?? $"At least one item in '{context.MemberName}' must satisfy the condition.");

            return context;
        }

        public static IConstraintContext<TSource, IEnumerable<TItem>> None<TSource, TItem>
        (
            this IConstraintContext<TSource, IEnumerable<TItem>> context,
            Func<TItem, bool> predicate,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(predicate);

            if (context.Value.Any(predicate))
                context.AddViolation(message ?? $"No item in '{context.MemberName}' may satisfy the condition.");

            return context;
        }
    }
}
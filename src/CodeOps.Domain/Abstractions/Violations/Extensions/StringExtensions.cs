namespace CodeOps.Domain.Abstractions.Violations.Extensions
{
    public static class StringExtensions
    {
        public static IConstraintContext<TSource, string> NotNullOrEmpty<TSource>
        (
            this IConstraintContext<TSource, string> context,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);

            if (string.IsNullOrEmpty(context.Value))
                context.AddViolation(message ?? $"'{context.MemberName}' cannot be null or empty.");

            return context;
        }

        public static IConstraintContext<TSource, string> NotNullOrWhiteSpace<TSource>
        (
            this IConstraintContext<TSource, string> context,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);

            if (string.IsNullOrWhiteSpace(context.Value))
                context.AddViolation(message ?? $"'{context.MemberName}' cannot be null or white space.");

            return context;
        }

        public static IConstraintContext<TSource, string> Empty<TSource>
        (
            this IConstraintContext<TSource, string> context,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.Value.Length != 0)
                context.AddViolation(message ?? $"'{context.MemberName}' must be empty.");

            return context;
        }

        public static IConstraintContext<TSource, string> MinimumLength<TSource>
        (
            this IConstraintContext<TSource, string> context,
            int minimumLength,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentOutOfRangeException.ThrowIfNegative(minimumLength);

            if (context.Value.Length < minimumLength)
                context.AddViolation(message ?? $"'{context.MemberName}' must be at least {minimumLength} characters long.");

            return context;
        }

        public static IConstraintContext<TSource, string> MaximumLength<TSource>
        (
            this IConstraintContext<TSource, string> context,
            int maximumLength,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentOutOfRangeException.ThrowIfNegative(maximumLength);

            if (context.Value.Length > maximumLength)
                context.AddViolation(message ?? $"'{context.MemberName}' cannot be more than {maximumLength} characters long.");

            return context;
        }

        public static IConstraintContext<TSource, string> LengthBetween<TSource>
        (
            this IConstraintContext<TSource, string> context,
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
                    message ?? $"'{context.MemberName}' must be between {minimumLength} and {maximumLength} characters long.");
            }

            return context;
        }

        public static IConstraintContext<TSource, string> StartsWith<TSource>
        (
            this IConstraintContext<TSource, string> context,
            string expectedPrefix,
            string? message = null,
            StringComparison comparisonType = StringComparison.Ordinal
        )
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(expectedPrefix);

            if (!context.Value.StartsWith(expectedPrefix, comparisonType))
                context.AddViolation(message ?? $"'{context.MemberName}' must start with '{expectedPrefix}'.");

            return context;
        }

        public static IConstraintContext<TSource, string> EndsWith<TSource>
        (
            this IConstraintContext<TSource, string> context,
            string expectedSuffix,
            string? message = null,
            StringComparison comparisonType = StringComparison.Ordinal
        )
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(expectedSuffix);

            if (!context.Value.EndsWith(expectedSuffix, comparisonType))
                context.AddViolation(message ?? $"'{context.MemberName}' must end with '{expectedSuffix}'.");

            return context;
        }

        public static IConstraintContext<TSource, string> Contains<TSource>
        (
            this IConstraintContext<TSource, string> context,
            string expectedValue,
            string? message = null,
            StringComparison comparisonType = StringComparison.Ordinal
        )
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(expectedValue);

            if (!context.Value.Contains(expectedValue, comparisonType))
                context.AddViolation(message ?? $"'{context.MemberName}' must contain '{expectedValue}'.");

            return context;
        }

        public static IConstraintContext<TSource, string> DoesNotContain<TSource>
        (
            this IConstraintContext<TSource, string> context,
            string unexpectedValue,
            string? message = null,
            StringComparison comparisonType = StringComparison.Ordinal
        )
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(unexpectedValue);

            if (context.Value.Contains(unexpectedValue, comparisonType))
                context.AddViolation(message ?? $"'{context.MemberName}' cannot contain '{unexpectedValue}'.");

            return context;
        }
    }
}
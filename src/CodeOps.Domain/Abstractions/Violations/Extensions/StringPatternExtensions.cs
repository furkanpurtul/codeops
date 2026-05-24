using System.Text.RegularExpressions;

namespace CodeOps.Domain.Abstractions.Violations.Extensions
{
    public static class StringPatternExtensions
    {
        public static IConstraintContext<TSource, string> Matches<TSource>
        (
            this IConstraintContext<TSource, string> context,
            Regex pattern,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(pattern);

            if (!pattern.IsMatch(context.Value))
                context.AddViolation(message ?? $"'{context.MemberName}' has an invalid format.");

            return context;
        }

        public static IConstraintContext<TSource, string> DoesNotMatch<TSource>
        (
            this IConstraintContext<TSource, string> context,
            Regex pattern,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(pattern);

            if (pattern.IsMatch(context.Value))
                context.AddViolation(message ?? $"'{context.MemberName}' has an invalid format.");

            return context;
        }
    }
}
using CodeOps.Domain.Abstractions.Ensuring;
using CodeOps.Domain.Abstractions.Violations;

namespace CodeOps.Domain.Abstractions.Ensuring.Extensions
{
    public static class GuardArrayExtensions
    {
        public static GuardContext<TSource, TItem[]> NotEmpty<TSource, TItem>(this GuardContext<TSource, TItem[]> context, string? message = null)
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.Value.Length == 0)
                context.AddViolation(nameof(NotEmpty), message ?? $"'{context.MemberName}' cannot be empty.");

            return context;
        }

        public static GuardContext<TSource, TItem[]> MinLength<TSource, TItem>(this GuardContext<TSource, TItem[]> context, int minLength, string? message = null)
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentOutOfRangeException.ThrowIfNegative(minLength);

            if (context.Value.Length < minLength)
                context.AddViolation(nameof(MinLength), message ?? $"'{context.MemberName}' must contain at least {minLength} items.");

            return context;
        }

        public static GuardContext<TSource, TItem[]> MaxLength<TSource, TItem>(this GuardContext<TSource, TItem[]> context, int maxLength, string? message = null)
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentOutOfRangeException.ThrowIfNegative(maxLength);

            if (context.Value.Length > maxLength)
                context.AddViolation(nameof(MaxLength), message ?? $"'{context.MemberName}' cannot contain more than {maxLength} items.");

            return context;
        }
    }
}
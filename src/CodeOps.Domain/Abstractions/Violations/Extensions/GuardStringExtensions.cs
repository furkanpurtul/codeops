namespace CodeOps.Domain.Abstractions.Violations.Extensions
{
    public static class GuardStringExtensions
    {
        public static GuardContext<TSource, string> NotNull<TSource>(this GuardContext<TSource, string> context, string? message = null)
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.Value is null)
                context.AddViolation(nameof(NotNull), message ?? $"'{context.MemberName}' cannot be null.");

            return context;
        }

        public static GuardContext<TSource, string> NotNullOrEmpty<TSource>(this GuardContext<TSource, string> context, string? message = null)
        {
            ArgumentNullException.ThrowIfNull(context);

            if (string.IsNullOrEmpty(context.Value))
                context.AddViolation(nameof(NotNullOrEmpty), message ?? $"'{context.MemberName}' cannot be empty.");

            return context;
        }

        public static GuardContext<TSource, string> NotNullOrWhiteSpace<TSource>(this GuardContext<TSource, string> context, string? message = null)
        {
            ArgumentNullException.ThrowIfNull(context);

            if (string.IsNullOrWhiteSpace(context.Value))
                context.AddViolation(nameof(NotNullOrWhiteSpace), message ?? $"'{context.MemberName}' cannot be null, empty, or whitespace.");

            return context;
        }

        public static GuardContext<TSource, string> MinLength<TSource>(this GuardContext<TSource, string> context, int minLength, string? message = null)
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentOutOfRangeException.ThrowIfNegative(minLength);

            if (context.Value is not null && context.Value.Length < minLength)
                context.AddViolation(nameof(MinLength), message ?? $"'{context.MemberName}' must be at least {minLength} characters long.");

            return context;
        }

        public static GuardContext<TSource, string> MaxLength<TSource>(this GuardContext<TSource, string> context, int maxLength, string? message = null)
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentOutOfRangeException.ThrowIfNegative(maxLength);

            if (context.Value is not null && context.Value.Length > maxLength)
                context.AddViolation(nameof(MaxLength), message ?? $"'{context.MemberName}' cannot be longer than {maxLength} characters.");

            return context;
        }

        public static GuardContext<TSource, string> LengthBetween<TSource>(this GuardContext<TSource, string> context, int minLength, int maxLength, string? message = null)
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentOutOfRangeException.ThrowIfNegative(minLength);
            ArgumentOutOfRangeException.ThrowIfNegative(maxLength);

            if (maxLength < minLength)
                throw new ArgumentOutOfRangeException(nameof(maxLength), "Max length cannot be less than min length.");

            if (context.Value is not null && (context.Value.Length < minLength || context.Value.Length > maxLength))
                context.AddViolation(nameof(LengthBetween), message ?? $"'{context.MemberName}' must be between {minLength} and {maxLength} characters long.");

            return context;
        }
    }
}
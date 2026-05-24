namespace CodeOps.Domain.Abstractions.Violations.Extensions
{
    public static class DateTimeOffsetExtensions
    {
        public static IConstraintContext<TSource, DateTimeOffset> Utc<TSource>
        (
            this IConstraintContext<TSource, DateTimeOffset> context,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.Value.Offset != TimeSpan.Zero)
                context.AddViolation(message ?? $"'{context.MemberName}' must be in UTC.");

            return context;
        }

        public static IConstraintContext<TSource, DateTimeOffset> NotInPast<TSource>
        (
            this IConstraintContext<TSource, DateTimeOffset> context,
            DateTimeOffset now,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.Value < now)
                context.AddViolation(message ?? $"'{context.MemberName}' cannot be in the past.");

            return context;
        }

        public static IConstraintContext<TSource, DateTimeOffset> NotInFuture<TSource>
        (
            this IConstraintContext<TSource, DateTimeOffset> context,
            DateTimeOffset now,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.Value > now)
                context.AddViolation(message ?? $"'{context.MemberName}' cannot be in the future.");

            return context;
        }

        public static IConstraintContext<TSource, DateTimeOffset> Before<TSource>
        (
            this IConstraintContext<TSource, DateTimeOffset> context,
            DateTimeOffset maximum,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.Value >= maximum)
                context.AddViolation(message ?? $"'{context.MemberName}' must be before '{maximum}'.");

            return context;
        }

        public static IConstraintContext<TSource, DateTimeOffset> After<TSource>
        (
            this IConstraintContext<TSource, DateTimeOffset> context,
            DateTimeOffset minimum,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.Value <= minimum)
                context.AddViolation(message ?? $"'{context.MemberName}' must be after '{minimum}'.");

            return context;
        }
    }
}
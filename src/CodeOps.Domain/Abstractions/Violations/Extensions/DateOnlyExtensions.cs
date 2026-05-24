namespace CodeOps.Domain.Abstractions.Violations.Extensions
{
    public static class DateOnlyExtensions
    {
        public static IConstraintContext<TSource, DateOnly> NotInPast<TSource>
        (
            this IConstraintContext<TSource, DateOnly> context,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);

            var today = DateOnly.FromDateTime(DateTime.Today);
            if (context.Value < today)
                context.AddViolation(message ?? $"'{context.MemberName}' cannot be in the past.");

            return context;
        }

        public static IConstraintContext<TSource, DateOnly> NotInPast<TSource>
        (
            this IConstraintContext<TSource, DateOnly> context,
            DateOnly today,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.Value < today)
                context.AddViolation(message ?? $"'{context.MemberName}' cannot be in the past.");

            return context;
        }

        public static IConstraintContext<TSource, DateOnly> NotInFuture<TSource>
        (
            this IConstraintContext<TSource, DateOnly> context,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);

            var today = DateOnly.FromDateTime(DateTime.Today);
            if (context.Value > today)
                context.AddViolation(message ?? $"'{context.MemberName}' cannot be in the future.");

            return context;
        }

        public static IConstraintContext<TSource, DateOnly> NotInFuture<TSource>
        (
            this IConstraintContext<TSource, DateOnly> context,
            DateOnly today,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.Value > today)
                context.AddViolation(message ?? $"'{context.MemberName}' cannot be in the future.");

            return context;
        }

        public static IConstraintContext<TSource, DateOnly> Before<TSource>
        (
            this IConstraintContext<TSource, DateOnly> context,
            DateOnly maximum,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.Value >= maximum)
                context.AddViolation(message ?? $"'{context.MemberName}' must be before '{maximum}'.");

            return context;
        }

        public static IConstraintContext<TSource, DateOnly> After<TSource>
        (
            this IConstraintContext<TSource, DateOnly> context,
            DateOnly minimum,
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
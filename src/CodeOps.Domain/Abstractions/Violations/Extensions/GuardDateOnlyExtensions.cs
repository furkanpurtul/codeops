namespace CodeOps.Domain.Abstractions.Violations.Extensions
{
    public static class GuardDateOnlyExtensions
    {
        public static GuardContext<TSource, DateOnly> NotInFuture<TSource>(this GuardContext<TSource, DateOnly> context, DateOnly today, string? message = null)
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.Value > today)
                context.AddViolation(nameof(NotInFuture), message ?? $"'{context.MemberName}' cannot be in the future.");

            return context;
        }

        public static GuardContext<TSource, DateOnly> NotInPast<TSource>(this GuardContext<TSource, DateOnly> context, DateOnly today, string? message = null)
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.Value < today)
                context.AddViolation(nameof(NotInPast), message ?? $"'{context.MemberName}' cannot be in the past.");

            return context;
        }
    }
}
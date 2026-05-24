namespace CodeOps.Domain.Abstractions.Violations.Extensions
{
    public static class GuardValueTypeExtensions
    {
        public static GuardContext<TSource, TValue> NotDefault<TSource, TValue>(this GuardContext<TSource, TValue> context, string? message = null)
            where TValue : struct
        {
            ArgumentNullException.ThrowIfNull(context);

            if (EqualityComparer<TValue>.Default.Equals(context.Value, default))
                context.AddViolation(nameof(NotDefault), message ?? $"'{context.MemberName}' cannot be default.");

            return context;
        }
    }
}
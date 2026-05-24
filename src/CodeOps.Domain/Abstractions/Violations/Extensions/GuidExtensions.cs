namespace CodeOps.Domain.Abstractions.Violations.Extensions
{
    public static class GuidExtensions
    {
        public static IConstraintContext<TSource, Guid> NotEmpty<TSource>
        (
            this IConstraintContext<TSource, Guid> context,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.Value == Guid.Empty)
                context.AddViolation(message ?? $"'{context.MemberName}' cannot be empty.");

            return context;
        }

        public static IConstraintContext<TSource, Guid> Empty<TSource>
        (
            this IConstraintContext<TSource, Guid> context,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.Value != Guid.Empty)
                context.AddViolation(message ?? $"'{context.MemberName}' must be empty.");

            return context;
        }
    }
}
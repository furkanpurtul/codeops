namespace CodeOps.Domain.Abstractions.Violations.Extensions
{
    public static class EnumExtensions
    {
        public static IConstraintContext<TSource, TValue> Defined<TSource, TValue>
        (
            this IConstraintContext<TSource, TValue> context,
            string? message = null
        ) where TValue : struct, Enum
        {
            ArgumentNullException.ThrowIfNull(context);

            if (!Enum.IsDefined(context.Value))
                context.AddViolation(message ?? $"'{context.MemberName}' must be a defined enum value.");

            return context;
        }

        public static IConstraintContext<TSource, TValue> NotDefined<TSource, TValue>
        (
            this IConstraintContext<TSource, TValue> context,
            string? message = null
        ) where TValue : struct, Enum
        {
            ArgumentNullException.ThrowIfNull(context);

            if (Enum.IsDefined(context.Value))
                context.AddViolation(message ?? $"'{context.MemberName}' must not be a defined enum value.");

            return context;
        }
    }
}
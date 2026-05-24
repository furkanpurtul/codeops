namespace CodeOps.Domain.Abstractions.Violations.Extensions
{
    public static class TypeExtensions
    {
        public static IConstraintContext<TSource, TValue> IsAssignableTo<TSource, TValue>
        (
            this IConstraintContext<TSource, TValue> context,
            Type expectedType,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(expectedType);

            if (context.Value is null)
                return context;

            if (!expectedType.IsInstanceOfType(context.Value))
                context.AddViolation(message ?? $"'{context.MemberName}' must be assignable to type '{expectedType.Name}'.");

            return context;
        }

        public static IConstraintContext<TSource, TValue> IsNotAssignableTo<TSource, TValue>
        (
            this IConstraintContext<TSource, TValue> context,
            Type unexpectedType,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(unexpectedType);

            if (context.Value is null)
                return context;

            if (unexpectedType.IsInstanceOfType(context.Value))
                context.AddViolation(message ?? $"'{context.MemberName}' cannot be assignable to type '{unexpectedType.Name}'.");

            return context;
        }

        public static IConstraintContext<TSource, TValue> IsExactly<TSource, TValue>
        (
            this IConstraintContext<TSource, TValue> context,
            Type expectedType,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(expectedType);

            if (context.Value is null)
                return context;

            if (context.Value.GetType() != expectedType)
                context.AddViolation(message ?? $"'{context.MemberName}' must be exactly of type '{expectedType.Name}'.");

            return context;
        }

        public static IConstraintContext<TSource, TValue> IsNotExactly<TSource, TValue>
        (
            this IConstraintContext<TSource, TValue> context,
            Type unexpectedType,
            string? message = null
        )
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(unexpectedType);

            if (context.Value is null)
                return context;

            if (context.Value.GetType() == unexpectedType)
                context.AddViolation(message ?? $"'{context.MemberName}' cannot be exactly of type '{unexpectedType.Name}'.");

            return context;
        }
    }
}
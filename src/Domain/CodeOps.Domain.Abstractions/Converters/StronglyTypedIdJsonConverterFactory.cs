using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CodeOps.Domain.Abstractions.Converters
{
    public sealed class StronglyTypedIdJsonConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            if (!typeToConvert.IsClass || typeToConvert.IsAbstract)
                return false;

            return typeToConvert.BaseType is { IsGenericType: true } baseType &&
                   baseType.GetGenericTypeDefinition() == typeof(StronglyTypedId<>);
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var baseType = typeToConvert.BaseType!;
            var valueType = baseType.GetGenericArguments()[0];

            // Protected/public constructor with single parameter TValue required
            var ctor = typeToConvert.GetConstructor(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                binder: null,
                new[] { valueType },
                modifiers: null);

            if (ctor is null)
                throw new InvalidOperationException(
                    $"Type '{typeToConvert.Name}' must have a constructor with a single parameter of type '{valueType.Name}'.");

            // Create factory delegate efficiently
            var factory = CreateFactoryDelegate(typeToConvert, valueType, ctor);

            var converterType = typeof(StronglyTypedIdJsonConverter<,>).MakeGenericType(typeToConvert, valueType);
            return (JsonConverter)Activator.CreateInstance(converterType, factory)!;
        }

        private static object CreateFactoryDelegate(Type idType, Type valueType, ConstructorInfo ctor)
        {
            // Equivalent to: (TValue v) => (TStronglyTypedId)Activator.CreateInstance(idType, v)!
            var parameter = System.Linq.Expressions.Expression.Parameter(valueType, "value");
            var newExpr = System.Linq.Expressions.Expression.New(ctor, parameter);
            var lambda = System.Linq.Expressions.Expression.Lambda(newExpr, parameter);
            return lambda.Compile();
        }
    }
}

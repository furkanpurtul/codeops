using System.Text.Json;
using System.Text.Json.Serialization;

namespace CodeOps.Domain.Abstractions.Converters
{
    /// <summary>
    /// System.Text.Json converter for <see cref="StronglyTypedId{TValue}"/> types.
    /// Supports serialization as raw value and deserialization from compatible primitive types.
    /// </summary>
    /// <typeparam name="TStronglyTypedId">Concrete strongly typed ID type.</typeparam>
    /// <typeparam name="TValue">Underlying raw value type (e.g. Guid, int, string).</typeparam>
    public sealed class StronglyTypedIdJsonConverter<TStronglyTypedId, TValue> : JsonConverter<TStronglyTypedId>
        where TStronglyTypedId : StronglyTypedId<TValue>
        where TValue : notnull
    {
        private readonly Func<TValue, TStronglyTypedId> _factory;

        /// <summary>
        /// Creates a new converter for a given strongly typed ID.
        /// </summary>
        /// <param name="factory">Factory delegate that constructs the ID from a raw value.</param>
        public StronglyTypedIdJsonConverter(Func<TValue, TStronglyTypedId> factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public override TStronglyTypedId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Deserialize underlying TValue first.
            var value = JsonSerializer.Deserialize<TValue>(ref reader, options);

            if (value is null)
                throw new JsonException($"Cannot deserialize null into {typeof(TStronglyTypedId).Name}.");

            return _factory(value);
        }

        public override void Write(Utf8JsonWriter writer, TStronglyTypedId value, JsonSerializerOptions options)
        {
            // Serialize only the raw value.
            JsonSerializer.Serialize(writer, value.Value, options);
        }
    }
}

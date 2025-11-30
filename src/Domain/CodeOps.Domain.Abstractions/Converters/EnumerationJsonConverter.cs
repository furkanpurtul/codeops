using System.Text.Json;
using System.Text.Json.Serialization;

namespace CodeOps.Domain.Abstractions.Converters
{
    public sealed class EnumerationJsonConverter<TEnumeration> : JsonConverter<TEnumeration>
        where TEnumeration : Enumeration<TEnumeration>
    {
        public override TEnumeration? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                if (reader.TryGetInt32(out var intValue))
                {
                    return Enumeration<TEnumeration>.TryFromValue(intValue, out var result)
                        ? result
                        : throw new JsonException($"Invalid value for {typeof(TEnumeration).Name}: {intValue}");
                }
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                var stringValue = reader.GetString();
                if (!string.IsNullOrWhiteSpace(stringValue))
                {
                    if (int.TryParse(stringValue, out var numeric))
                    {
                        if (Enumeration<TEnumeration>.TryFromValue(numeric, out var numericResult))
                            return numericResult;
                    }

                    if (Enumeration<TEnumeration>.TryFromName(stringValue, false, out var namedResult))
                        return namedResult;

                    throw new JsonException($"Unknown name for {typeof(TEnumeration).Name}: {stringValue}");
                }
            }

            throw new JsonException($"Unsupported token type for {typeof(TEnumeration).Name}: {reader.TokenType}");
        }

        public override void Write(Utf8JsonWriter writer, TEnumeration value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Name);
        }
    }
}

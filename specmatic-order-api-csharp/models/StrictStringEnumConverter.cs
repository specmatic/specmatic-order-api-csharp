using System.Text.Json;
using System.Text.Json.Serialization;

public class StrictStringEnumConverter<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum
{
    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Reject if the token is a number
        if (reader.TokenType == JsonTokenType.Number)
        {
            throw new JsonException($"Integer values are not allowed for {typeof(TEnum).Name}. Use string values instead.");
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            var stringValue = reader.GetString();
            if (Enum.TryParse<TEnum>(stringValue, ignoreCase: true, out var result))
            {
                return result;
            }
            // Get valid enum values for error message
            var validValues = string.Join("', '", Enum.GetNames(typeof(TEnum)));
            throw new JsonException($"Invalid value '{stringValue}' for {typeof(TEnum).Name}. Valid values are: '{validValues}'.");
        }

        throw new JsonException($"Expected a string value for {typeof(TEnum).Name}.");
    }

    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
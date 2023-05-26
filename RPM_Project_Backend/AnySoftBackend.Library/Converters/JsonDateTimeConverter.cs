using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AnySoftBackend.Library.Converters;

public class JsonDateTimeConverter : JsonConverter<DateTime?>
{
    private const string Format = "dd.MM.yyyy";
    
    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.GetString() is { } value 
        && DateTime.TryParseExact(value, Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result) 
            ? result : null;

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value is { }) {
            writer.WriteStringValue(value.Value.ToString(Format, CultureInfo.InvariantCulture));
        }
        else {
            writer.WriteNullValue();
        }
    }
}
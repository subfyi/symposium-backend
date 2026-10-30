using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Symposium.Api.Server.Converters;

public class DateTimeOffsetConverter : JsonConverter<DateTimeOffset?>
{
    private readonly TimeZoneInfo tZone;

    public DateTimeOffsetConverter(TimeZoneInfo tZone)
    {
        this.tZone = tZone;
    }

    public override DateTimeOffset? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        var str = reader.GetString()!;
        if (string.IsNullOrWhiteSpace(str))
            return null;

        var dateTime = DateTime.Parse(str, CultureInfo.InvariantCulture);
        return new DateTimeOffset(dateTime, tZone.GetUtcOffset(dateTime)).ToUniversalTime();
    }

    public override void Write(
        Utf8JsonWriter writer,
        DateTimeOffset? dateTimeValue,
        JsonSerializerOptions options)
    {
        if (dateTimeValue == null || dateTimeValue.Value == default)
            writer.WriteNullValue();
        else
            writer.WriteStringValue(RemoveOffset(dateTimeValue.Value.ToOffset(tZone.GetUtcOffset(dateTimeValue.Value)).DateTime));
    }

    private DateTime RemoveOffset(DateTime dateTime)
    {
        return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute,
            dateTime.Second);
    }
}
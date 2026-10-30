using System.Text.Json;
using System.Text.Json.Serialization;
using File = Symposium.Models.File;

namespace Symposium.Api.Server.Converters;

public class FileConverter : System.Text.Json.Serialization.JsonConverter<File>
{
    private readonly string cdn;

    public FileConverter(string cdn)
    {
        this.cdn = cdn;
    }

    private readonly JsonSerializerOptions options
        = new JsonSerializerOptions()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

    public override File Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return JsonSerializer.Deserialize<File>(ref reader, this.options);
    }

    public override void Write(Utf8JsonWriter writer, File value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value with
        {
            access_url = Url(value.access_url),
            download_url = Url(value.download_url)
        }, this.options);
    }

    private string? Url(string? value) => value == null ? null :
        value.StartsWith("https://", StringComparison.OrdinalIgnoreCase) || value.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
            ? value : cdn.TrimEnd('/') + "/" + value.TrimStart('/');
}

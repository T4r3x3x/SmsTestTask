using System.Text.Json;

namespace Sms.Shared.Configuration;

public sealed class JsonSettingsLoader : ISettingsLoader
{
    private static readonly JsonSerializerOptions s_options = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    public T Load<T>(string path)
    {
        using var stream = File.OpenRead(path);
        return JsonSerializer.Deserialize<T>(stream, s_options)
            ?? throw new InvalidDataException($"Settings file '{path}' is empty.");
    }
}
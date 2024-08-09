using System.Text.Json;

namespace Newsletter;

public static class JsonExtensions
{
    public static JsonElement Get(this JsonElement element, string key)
    {
        try
        {
            return element.GetProperty(key);
        }
        catch (KeyNotFoundException)
        {
            throw new KeyNotFoundException($"'{key}' not found");
        }
    }

    public static Iterable<JsonElement> Enumerate(this JsonElement element) =>
        element
            .EnumerateArray()
            .AsIterable();
}

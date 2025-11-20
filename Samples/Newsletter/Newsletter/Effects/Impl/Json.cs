using System.Text.Json;
using Newsletter.Effects.Traits;

namespace Newsletter.Effects.Impl;

public record Json : JsonIO
{
    public static readonly JsonIO Default = new Json();
    
    public JsonDocument Parse(string text) =>
        JsonDocument.Parse(text);
}

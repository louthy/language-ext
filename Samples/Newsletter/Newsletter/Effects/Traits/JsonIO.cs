using System.Text.Json;

namespace Newsletter.Effects.Traits;

public interface JsonIO
{
    public JsonDocument Parse(string text);
}

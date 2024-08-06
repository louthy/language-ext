using System.Text.Json;
using Newsletter.Effects.Traits;

namespace Newsletter.Effects;

/// <summary>
/// Json parser
/// </summary>
public static class Json<M, RT>
    where RT : 
        Has<M, JsonIO>
    where M :
        Monad<M>
{
    static readonly K<M, JsonIO> trait =
        Has<M, RT, JsonIO>.ask;

    public static K<M, JsonDocument> readJson(string text) =>
        trait.Map(t => t.Parse(text));
}

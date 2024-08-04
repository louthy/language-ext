using System.Text.Json;
using Newsletter.Effects.Traits;

namespace Newsletter.Effects;

/// <summary>
/// Json parser
/// </summary>
public static class Json<RT>
    where RT : Has<Eff<RT>, JsonIO>
{
    public static readonly Eff<RT, JsonIO> trait =
        Stateful.getsM<Eff<RT>, RT, JsonIO>(rt => rt.Trait).As();

    public static Eff<RT, JsonDocument> readJson(string text) =>
        trait.Map(t => t.Parse(text));
}

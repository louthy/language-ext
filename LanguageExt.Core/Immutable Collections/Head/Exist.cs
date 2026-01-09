namespace LanguageExt;

/// <summary>
/// Existent value
/// </summary>
/// <param name="Value">Value</param>
/// <typeparam name="A">Value type</typeparam>
public sealed record Exist<A>(A Value) : Head<A>
{
    public void Deconstruct(out A head) => 
        head = Value;
}

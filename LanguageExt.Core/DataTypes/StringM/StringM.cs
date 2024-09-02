using System;
using LanguageExt.Traits.Domain;

namespace LanguageExt;

/// <summary>
/// Simple wrapper around String
/// </summary>
/// <param name="Value">Contained value</param>
public readonly record struct StringM(string Value) : StringM<StringM> 
{
    static Fin<StringM> DomainType<StringM, string>.From(string repr) =>
        new StringM(repr);

    public static StringM From(string repr) => 
        new (repr);

    public static StringM FromUnsafe(string repr) => 
        new (repr);

    public string To() =>
        Value;

    public static StringComparison Comparison =>
        StringComparison.Ordinal;

    public static implicit operator string(StringM value) =>
        value.Value;

    public static implicit operator StringM(string value) =>
        new(value);
}

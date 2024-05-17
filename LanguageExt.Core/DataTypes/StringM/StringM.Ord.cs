using System;

namespace LanguageExt;

public readonly record struct StringOrdinalM(string Value) : StringM<StringOrdinalM>
{
    public static StringOrdinalM From(string repr) =>
        new(repr);

    public string To() =>
        Value;

    public static StringComparison Comparison =>
        StringComparison.Ordinal;
    
    public static implicit operator string(StringOrdinalM value) =>
        value.Value;

    public static implicit operator StringOrdinalM(string value) =>
        new(value);

}

public readonly record struct StringOrdinalIgnoreCaseM(string Value) : StringM<StringOrdinalIgnoreCaseM>
{
    public static StringOrdinalIgnoreCaseM From(string repr) =>
        new(repr);

    public string To() =>
        Value;

    public static StringComparison Comparison =>
        StringComparison.OrdinalIgnoreCase;
    
    public static implicit operator string(StringOrdinalIgnoreCaseM value) =>
        value.Value;

    public static implicit operator StringOrdinalIgnoreCaseM(string value) =>
        new(value);

}

public readonly record struct StringCultureM(string Value) : StringM<StringCultureM>
{
    public static StringCultureM From(string repr) =>
        new(repr);

    public string To() =>
        Value;

    public static StringComparison Comparison =>
        StringComparison.CurrentCulture;
    
    public static implicit operator string(StringCultureM value) =>
        value.Value;

    public static implicit operator StringCultureM(string value) =>
        new(value);

}

public readonly record struct StringCultureIgnoreCaseM(string Value) : StringM<StringCultureIgnoreCaseM>
{
    public static StringCultureIgnoreCaseM From(string repr) =>
        new(repr);

    public string To() =>
        Value;

    public static StringComparison Comparison =>
        StringComparison.CurrentCultureIgnoreCase;
    
    public static implicit operator string(StringCultureIgnoreCaseM value) =>
        value.Value;

    public static implicit operator StringCultureIgnoreCaseM(string value) =>
        new(value);
}

public readonly record struct StringInvariantM(string Value) : StringM<StringInvariantM>
{
    public static StringInvariantM From(string repr) =>
        new(repr);

    public string To() =>
        Value;

    public static StringComparison Comparison =>
        StringComparison.InvariantCulture;

    public static implicit operator string(StringInvariantM value) =>
        value.Value;

    public static implicit operator StringInvariantM(string value) =>
        new(value);
}

public readonly record struct StringInvariantIgnoreCaseM(string Value) : StringM<StringInvariantIgnoreCaseM>
{
    public static StringInvariantIgnoreCaseM From(string repr) =>
        new(repr);

    public string To() =>
        Value;

    public static StringComparison Comparison =>
        StringComparison.InvariantCultureIgnoreCase;

    public static implicit operator string(StringInvariantIgnoreCaseM value) =>
        value.Value;

    public static implicit operator StringInvariantIgnoreCaseM(string value) =>
        new(value);
}

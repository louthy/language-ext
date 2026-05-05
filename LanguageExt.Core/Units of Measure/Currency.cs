using LanguageExt.Traits.Domain;
using static LanguageExt.Prelude;

namespace LanguageExt;

public interface Currency : 
    DomainType<Currency, string>
{
    string Code { get; }
    string Name { get; }
    string Symbol { get; }
    int Decimals { get; }

    string DomainType<Currency, string>.To() =>
        Code;
}

/// <summary>
/// Cached type-level currency value.
/// </summary>
public static class Currency<C>
    where C : Currency, new()
{
    public static C Value { get; } =
        new();
}

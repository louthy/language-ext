#nullable enable
using LanguageExt.Attributes;

namespace LanguageExt.Traits;

/// <summary>
/// Constant value trait
/// </summary>
/// <typeparam name="TYPE"></typeparam>
[Trait("Const*")]
public interface Const<out TYPE> : Trait
{
    public static abstract TYPE Value { get; }
}

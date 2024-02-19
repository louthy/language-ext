using System.Diagnostics.Contracts;
using LanguageExt.Attributes;

namespace LanguageExt.TypeClasses;

/// <summary>
/// Fractional number trait
/// </summary>
/// <typeparam name="A">The type for which fractional 
/// operations are being defined.</typeparam>
[Trait("Fraction*")]
public interface Fraction<A> : Num<A>
{
    /// <summary>
    /// Generates a fractional value from an integer ratio.
    /// </summary>
    /// <param name="x">The ratio to convert</param>
    /// <returns>The equivalent of x in the implementing type.</returns>
    [Pure]
    public static abstract A FromRational(Ratio<int> x);
}

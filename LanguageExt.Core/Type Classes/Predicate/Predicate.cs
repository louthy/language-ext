#nullable enable
using System.Diagnostics.Contracts;
using LanguageExt.Attributes;

namespace LanguageExt.Traits;

/// <summary>
/// Predicate trait
/// </summary>
/// <typeparam name="A">Type of value to run the predication operation against</typeparam>
[Trait("Pred*")]
public interface Pred<in A> : Trait
{
    /// <summary>
    /// The predicate operation.  Returns true if the source value
    /// fits the predicate.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [Pure]
    public static abstract bool True(A value);
}

using System.Diagnostics.Contracts;
using LanguageExt.Common;

namespace LanguageExt;

/// <summary>
/// Meant to represent `void`, but we can't construct a `System.Void`.
/// 
/// A `Void` is the initial object in a category, equivalent to an empty set, and because there are no values in an
/// empty set there's no way to construct a type of `Void`.
/// </summary>
/// <remarks>
/// Usages:
///   * Used in the pipes system to represent a 'closed' path.
///   * Used in `Decidable` contravariant functors to 'lose' information.  
/// </remarks>
public record Void
{
    /// <summary>
    /// Voids can't be constructed, as they're the 'uninhabited type', i.e. an empty set, with no values.
    /// </summary>
    Void() => 
        throw new BottomException();
    
    [Pure]
    public override string ToString() => 
        "void";
}

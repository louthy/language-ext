#nullable enable
using System.Diagnostics.Contracts;
using LanguageExt.Attributes;

namespace LanguageExt.Traits;

/// <summary>
/// Liftable trait
/// <para>
/// Can lift a value A into a 
/// </para>
/// </summary>
/// <typeparam name="A">The type to be lifted</typeparam>
[Trait("Lift*")]
public interface Liftable<out LA, in A> : Trait
{
    /// <summary>
    /// Lift value A into a Liftable〈A〉
    /// </summary>
    [Pure]
    public static abstract LA Lift(A x);
}

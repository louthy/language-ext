using System.Diagnostics.Contracts;

namespace LanguageExt.Traits;

/// <summary>
/// Countable structures are structures that already contain a count of the number of elements in the structure, and
/// therefore accessing the `Count` property is a constant time operation.
/// </summary>
/// <typeparam name="F">Trait self</typeparam>
public interface Countable<out F>
    where F : Countable<F>
{
    /// <summary>
    /// Count of the number of elements in the structure
    /// </summary>
    [Pure]
    public static abstract long Count<A>(K<F, A> fa);
}

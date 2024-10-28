using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// A monoid on applicative functors
/// </summary>
/// <typeparam name="F">Applicative functor</typeparam>
public static class AlternativeExtensions
{
    /// <summary>
    /// Given a set of applicative functors, return the first one to succeed.
    /// </summary>
    /// <remarks>
    /// If none succeed, the last applicative functor will be returned.
    /// </remarks>
    public static K<F, A> OneOf<F, A>(this Seq<K<F, A>> ms)
        where F : Alternative<F> =>
        Alternative.oneOf(ms);
}

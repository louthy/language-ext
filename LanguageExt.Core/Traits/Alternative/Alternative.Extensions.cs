using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// A monoid on applicative functors
/// </summary>
/// <typeparam name="F">Applicative functor</typeparam>
public static class AlternativeExtensions
{
    /// <summary>
    /// Where `F` defines some notion of failure or choice, this function picks the
    /// first argument that succeeds.  So, if `fa` succeeds, then `fa` is returned;
    /// if it fails, then `fb` is returned.
    /// </summary>
    /// <param name="fa">First structure to test</param>
    /// <param name="fb">Second structure to return if the first one fails</param>
    /// <typeparam name="F">Alternative structure type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>First argument to succeed</returns>
    public static K<F, A> Choice<F, A>(this K<F, A> fa, K<F, A> fb)
        where F : Alternative<F> =>
        F.Choice(fa, fb);
    
    /// <summary>
    /// Given a set of applicative functors, return the first one to succeed.
    /// </summary>
    /// <remarks>
    /// If none succeed, the last applicative functor will be returned.
    /// </remarks>
    public static K<F, A> OneOf<F, A>(this Seq<K<F, A>> ms)
        where F : Alternative<F> =>
        Alternative.oneOf(ms);

    /// <summary>
    /// One or more...
    /// </summary>
    /// <remarks>
    /// Run the applicative functor repeatedly, collecting the results, until failure.
    ///
    /// Will always succeed if at least one item has been yielded.
    /// </remarks>
    /// <param name="v">Applicative functor</param>
    /// <returns>One or more values</returns>
    public static K<F, Seq<A>> Some<F, A>(this K<F, A> v)
        where F : Alternative<F> =>
        Alternative.some(v);

    /// <summary>
    /// Zero or more...
    /// </summary>
    /// <remarks>
    /// Run the applicative functor repeatedly, collecting the results, until failure.
    ///
    /// Will always succeed.
    /// </remarks>
    /// <param name="v">Applicative functor</param>
    /// <returns>Zero or more values</returns>
    public static K<F, Seq<A>> Many<F, A>(this K<F, A> v)
        where F : Alternative<F> =>
        Alternative.many(v);
}

using System;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class Prelude
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
    public static K<F, A> choose<F, A>(K<F, A> fa, K<F, A> fb)
        where F : Choice<F> =>
        Choice.choose(fa, fb);
    
    /// <summary>
    /// One or more...
    /// </summary>
    /// <remarks>
    /// Run the applicative functor repeatedly, collecting the results, until failure.
    ///
    /// Will always succeed if at least one item has been yielded.
    /// </remarks>
    /// <remarks>
    /// NOTE: It is important that the `F` applicative-type overrides `Apply` (the one with `Func` laziness) in its
    /// trait-implementations otherwise this will likely result in a stack-overflow. 
    /// </remarks>
    /// <param name="fa">Applicative functor</param>
    /// <returns>One or more values</returns>
    [Pure]
    public static K<F, Seq<A>> some<F, A>(K<F, A> fa)
        where F : Choice<F>, Applicative<F> =>
        Choice.some(fa);

    /// <summary>
    /// Zero or more...
    /// </summary>
    /// <remarks>
    /// Run the applicative functor repeatedly, collecting the results, until failure.
    /// Will always succeed.
    /// </remarks>
    /// <remarks>
    /// NOTE: It is important that the `F` applicative-type overrides `ApplyLazy` in its trait-implementations
    /// otherwise this will likely result in a stack-overflow. 
    /// </remarks>
    /// <param name="fa">Applicative functor</param>
    /// <returns>Zero or more values</returns>
    [Pure]
    public static K<F, Seq<A>> many<F, A>(K<F, A> fa)
        where F : Choice<F>, Applicative<F> =>
        Choice.many(fa);
}

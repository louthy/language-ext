using System;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;

namespace LanguageExt.Traits;

public static class Alternative
{
    /// <summary>
    /// Empty / none state for the `F` structure 
    /// </summary>
    /// <typeparam name="F">Alternative trait implementation</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Empty</returns>
    public static K<F, A> empty<F, A>()
        where F : Alternative<F> =>
        F.Empty<A>();
    
    /// <summary>
    /// Given a set of applicative functors, return the first one to succeed.
    /// </summary>
    /// <remarks>
    /// If none succeed, the last applicative functor will be returned.
    /// </remarks>
    [Pure]
    public static K<F, A> choice<F, A>(params K<F, A>[] ms)
        where F : Alternative<F> =>
        F.Choice(ms);

    /// <summary>
    /// Given a set of applicative functors, return the first one to succeed.
    /// </summary>
    /// <remarks>
    /// If none succeed, the last applicative functor will be returned.
    /// </remarks>
    [Pure]
    public static K<F, A> choice<F, A>(Seq<K<F, A>> ms)
        where F : Alternative<F> =>
        F.Choice(ms);

    /// <summary>
    /// Given a set of applicative functors, return the first one to succeed.
    /// </summary>
    /// <remarks>
    /// If none succeed, the last applicative functor will be returned.
    /// </remarks>
    [Pure]
    public static K<F, A> choice<F, A>(ReadOnlySpan<K<F, A>> ms)
        where F : Alternative<F> =>
        F.Choice(ms);

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
        where F : Alternative<F> =>
        F.Some(fa);
    
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
        where F : Alternative<F> =>
        F.Many(fa);

    /// <summary>
    /// Skip zero or more...
    /// </summary>
    /// <remarks>
    /// Run the applicative functor repeatedly until failure.
    /// Will always succeed.
    /// </remarks>
    /// <param name="fa">Applicative functor</param>
    /// <returns>Unit</returns>
    [Pure]
    public static K<F, Unit> skipMany<F, A>(K<F, A> fa)
        where F : Alternative<F> =>
        F.SkipMany(fa);

    /// <summary>
    /// Combine two alternatives
    /// </summary>
    /// <param name="ma">Left alternative</param>
    /// <param name="mb">Right alternative</param>
    /// <typeparam name="A">Left value type</typeparam>
    /// <typeparam name="B">Right value type</typeparam>
    /// <returns>Alternative structure with an `Either` lifted into it</returns>
    [Pure]
    public static K<F, Either<A, B>> either<F, A, B>(K<F, A> ma, K<F, B> mb) 
        where F : Alternative<F> =>
        F.Either(ma, mb); 
}

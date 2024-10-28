using System;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;

namespace LanguageExt.Traits;

public static class Choice
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
        F.Choose(fa, fb);
    
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
        where F : Choice<F>, Applicative<F>
    {
        return some_v();
        
        K<F, Seq<A>> many_v() =>
            F.Choose(some_v(), F.Pure(Seq<A>()));

        K<F, Seq<A>> some_v() =>
            Append<A>.cons.Map(fa).Apply(many_v);
    }
    
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
        where F : Choice<F>, Applicative<F>
    {
        return many_v();
        
        K<F, Seq<A>> many_v() =>
            F.Choose(some_v(), F.Pure(Seq<A>()));

        K<F, Seq<A>> some_v() =>
            Append<A>.cons.Map(fa).Apply(many_v);
    }
        
    static class Append<A>
    {
        public static readonly Func<A, Func<Seq<A>, Seq<A>>> cons =
            x => xs => x.Cons(xs);
    }    
}

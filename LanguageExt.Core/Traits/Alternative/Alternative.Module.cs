using System;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;

namespace LanguageExt.Traits;

public static class Alternative
{
    /// <summary>
    /// Given a set of applicative functors, return the first one to succeed.
    /// </summary>
    /// <remarks>
    /// If none succeed, the last applicative functor will be returned.
    /// </remarks>
    [Pure]
    public static K<F, A> oneOf<F, A>(params K<F, A>[] ms)
        where F : Alternative<F> =>
        oneOf(toSeq(ms));

    /// <summary>
    /// Given a set of applicative functors, return the first one to succeed.
    /// </summary>
    /// <remarks>
    /// If none succeed, the last applicative functor will be returned.
    /// </remarks>
    [Pure]
    public static K<F, A> oneOf<F, A>(Seq<K<F, A>> ms)
        where F : Alternative<F>
    {
        if(ms.IsEmpty()) return F.Empty<A>();
        var r = ms[0];
        foreach (var m in ms.Tail)
        {
            r = F.Choose(r, m);
        }
        return r;
    }
    
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
        where F : Alternative<F>, Applicative<F>
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
        where F : Alternative<F>, Applicative<F>
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

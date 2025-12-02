using System;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;

namespace LanguageExt.Traits;

public interface Alternative<F> : Choice<F>, Applicative<F>
    where F : Alternative<F>
{
    /// <summary>
    /// Identity
    /// </summary>
    [Pure]
    public static abstract K<F, A> Empty<A>(); 
    
    /// <summary>
    /// Given a set of applicative functors, return the first one to succeed.
    /// </summary>
    /// <remarks>
    /// If none succeed, the last applicative functor will be returned.
    /// </remarks>
    [Pure]
    public static virtual K<F, A> OneOf<A>(in Seq<K<F, A>> ms)
    {
        if(ms.IsEmpty) return F.Empty<A>();
        var r = ms[0];
        foreach (var m in ms.Tail)
        {
            r = F.Choose(r, m);
        }
        return r;
    }

    /// <summary>
    /// Given a set of applicative functors, return the first one to succeed.
    /// </summary>
    /// <remarks>
    /// If none succeed, the last applicative functor will be returned.
    /// </remarks>
    [Pure]
    public static virtual K<F, A> OneOf<A>(in ReadOnlySpan<K<F, A>> ms)
    {
        if(ms.Length == 0) return F.Empty<A>();
        var r = ms[0];
        foreach (var m in ms)
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
    /// <param name="fa">Applicative functor</param>
    /// <returns>One or more values</returns>
    [Pure]
    public static virtual K<F, Seq<A>> Some<A>(K<F, A> fa)
    {
        return some_v();
        
        K<F, Seq<A>> many_v() =>
            F.Choose(some_v(), F.Pure(Seq<A>()));

        K<F, Seq<A>> some_v() =>
            (Cached<A>.cons * fa).Apply(memoK(many_v));
    }
    
    /// <summary>
    /// Zero or more...
    /// </summary>
    /// <remarks>
    /// Run the applicative functor repeatedly, collecting the results, until failure.
    /// Will always succeed.
    /// </remarks>
    /// <param name="fa">Applicative functor</param>
    /// <returns>Zero or more values</returns>
    [Pure]
    public static virtual K<F, Seq<A>> Many<A>(K<F, A> fa)
    {
        return many_v();
        
        K<F, Seq<A>> many_v() =>
            some_v() | F.Pure(Seq<A>());

        K<F, Seq<A>> some_v() =>
            Cached<A>.cons * fa * memoK(many_v);
    }
    
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
    public static virtual K<F, Unit> SkipMany<A>(K<F, A> fa)
    {
        return many_v();
        
        K<F, Unit> many_v() =>
            some_v() | F.Pure(unit);

        K<F, Unit> some_v() =>
            Cached<A>.ignore * fa * memoK(many_v);
    }
        
    static class Cached<A>
    {
        public static readonly Func<A, Func<Unit, Unit>> ignore =
            static _ => _ => default;
        
        public static readonly Func<A, Func<Seq<A>, Seq<A>>> cons =
            static x => xs => x.Cons(xs);
    }
}

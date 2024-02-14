using System;
using static LanguageExt.Prelude;

namespace LanguageExt.Traits;

/// <summary>
/// A monoid on applicative functors
/// </summary>
/// <typeparam name="F">Applicative functor</typeparam>
public interface Alternative<F> : Applicative<F>
    where F : Alternative<F>
{
    /// <summary>
    /// Identity
    /// </summary>
    /// <typeparam name="A"></typeparam>
    /// <returns></returns>
    public static abstract K<F, A> Empty<A>();
    
    /// <summary>
    /// Associative binary operator
    /// </summary>
    public static abstract K<F, A> Or<A>(K<F, A> ma, K<F, A> mb);

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
    public static virtual K<F, Seq<A>> Some<A>(K<F, A> v)
    {
        return some_v();
        
        K<F, Seq<A>> many_v() =>
            F.Or(some_v(), F.Pure(Seq<A>()));

        K<F, Seq<A>> some_v() =>
            F.Apply(Append<F, A>.cons, v, many_v());
    }
    
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
    public static virtual K<F, Seq<A>> Many<A>(K<F, A> v)
    {
        return many_v();
        
        K<F, Seq<A>> many_v() =>
            F.Or(some_v(), F.Pure(Seq<A>()));

        K<F, Seq<A>> some_v() =>
            F.Apply(Append<F, A>.cons, v, many_v());
    }
}

static class Append<F, A> where F : Applicative<F>
{
    public static readonly K<F, Func<A, Seq<A>, Seq<A>>> cons =
        F.Pure((A x, Seq<A> y) => x.Cons(y));
}

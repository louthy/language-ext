using System;
using static LanguageExt.Prelude;

namespace LanguageExt.HKT;

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
    public static abstract Alternative<F, A> Empty<A>();
    
    /// <summary>
    /// Associative binary operator
    /// </summary>
    public static abstract Alternative<F, A> Or<A>(Alternative<F, A> ma, Alternative<F, A> mb);

    /// <summary>
    /// Associative binary operator
    /// </summary>
    public static virtual Alternative<F, A> Or<A>(Alternative<F, A> ma, Applicative<F, A> mb) =>
        F.Or(ma, (Alternative<F, A>)mb);

    /// <summary>
    /// Associative binary operator
    /// </summary>
    public static virtual Alternative<F, A> Or<A>(Applicative<F, A> ma, Alternative<F, A> mb) =>
        F.Or((Alternative<F, A>)ma, mb);

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
    public static virtual Alternative<F, Seq<A>> Some<A>(Alternative<F, A> v)
    {
        return some_v();
        
        Alternative<F, Seq<A>> many_v() =>
            some_v() | F.Pure(Seq<A>());

        Alternative<F, Seq<A>> some_v() =>
            F.Apply(Append<F, A>.cons, v, many_v()).AsAlternative();
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
    public static virtual Alternative<F, Seq<A>> Many<A>(Alternative<F, A> v)
    {
        return many_v();
        
        Alternative<F, Seq<A>> many_v() =>
            some_v() | F.Pure(Seq<A>());

        Alternative<F, Seq<A>> some_v() =>
            F.Apply(Append<F, A>.cons, v, many_v()).AsAlternative();
    }
}

static class Append<F, A> where F : Applicative<F>
{
    public static readonly Applicative<F, Func<A, Seq<A>, Seq<A>>> cons =
        F.Pure((A x, Seq<A> y) => x.Cons(y));
}

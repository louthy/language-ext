using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;
using NSE = System.NotSupportedException;

namespace LanguageExt;

public static class TheseExtensions
{
    public static These<A, B> As<A, B>(this K<These<A>, B> ma) =>
        (These<A, B>)ma;
    
    /// <summary>
    /// Coalesce with the provided operation
    /// </summary>
    /// <param name="f">Coalesce operation</param>
    /// <returns>Coalesced value</returns>
    public static A Merge<A>(this K<These<A>, A> these, Func<A, A, A> f) =>
        these.As().Match(x => x, x => x, f);

    /// <summary>
    /// Select each constructor and partition them into separate lists.
    /// </summary>
    /// <param name="theses">Selection</param>
    /// <typeparam name="F">Foldable structure</typeparam>
    /// <returns>Partitioned sequences</returns>
    public static (Seq<A> This, Seq<B> That, Seq<(A, B)> Both) Partition<F, A, B>(
        this K<F, These<A, B>> theses)
        where F : Foldable<F> =>
        theses.Fold((This: Seq<A>(), That: Seq<B>(), Both: Seq<(A First, B Second)>()),
                    (s, ts) => ts switch
                               {
                                   These.This<A, B> (var x)        => (s.This.Add(x), s.That, s.Both),
                                   These.That<A, B> (var y)        => (s.This, s.That.Add(y), s.Both),
                                   These.Both<A, B> (var x, var y) => (s.This, s.That, s.Both.Add((x, y))),
                                   _                               => throw new NSE()
                               });


    /// <summary>
    /// Select each constructor and partition them into separate lists.
    /// </summary>
    /// <param name="theses">Selection</param>
    /// <typeparam name="F">Foldable structure</typeparam>
    /// <returns>Partitioned sequences</returns>
    public static (Seq<A> This, Seq<B> That) Partition2<F, A, B>(
        this K<F, These<A, B>> theses)
        where F : Foldable<F> =>
        theses.Fold((This: Seq<A>(), That: Seq<B>()),
                    (state, these) => these switch
                                      {
                                          These.This<A, B> (var x)        => (state.This.Add(x), state.That),
                                          These.That<A, B> (var y)        => (state.This, state.That.Add(y)),
                                          These.Both<A, B> (var x, var y) => (state.This.Add(x), state.That.Add(y)),
                                          _                               => throw new NSE()
                                      });

    /// <summary>
    /// Semigroup combine
    /// </summary>
    /// <param name="first">First `These`</param>
    /// <param name="second">Second `These`</param>
    /// <returns>`These` combined using semigroup rules</returns>
    public static These<A, B> Combine<A, B>(this K<These<A>, B> first, K<These<A>, B> second)
        where A : Semigroup<A>
        where B : Semigroup<B> =>
        (first, second) switch
        {
            (These.This<A, B> (var fx), These.This<A, B> (var sx))                 => This<A, B>(fx + sx),
            (These.This<A, B> (var fx), These.That<A, B> (var sx))                 => Both(fx, sx),
            (These.This<A, B> (var fx), These.Both<A, B> (var s1, var s2))         => Both(fx + s1, s2),
            (These.That<A, B> (var fx), These.This<A, B> (var sx))                 => Both(sx, fx),
            (These.That<A, B> (var fx), These.That<A, B> (var sx))                 => That<A, B>(fx + sx),
            (These.That<A, B> (var fx), These.Both<A, B> (var s1, var s2))         => Both(s1, fx   + s2),
            (These.Both<A, B> (var f1, var f2), These.This<A, B> (var sx))         => Both(f1       + sx, f2),
            (These.Both<A, B> (var f1, var f2), These.That<A, B> (var sx))         => Both(f1, f2   + sx),
            (These.Both<A, B> (var f1, var f2), These.Both<A, B> (var s1, var s2)) => Both(f1       + s1, f2 + s2),
            _                                                                      => throw new NSE()
        };

    public static These<A, C> Apply<A, B, C>(this K<These<A>, Func<B, C>> mf, K<These<A>, B> ma)
        where A : Semigroup<A> =>
        (mf, ma) switch
        {
            (These.This<A, Func<B, C>> (var a), _)                                       => This<A, C>(a),
            (These.That<A, Func<B, C>>, These.This<A, B> (var a))                        => This<A, C>(a),
            (These.That<A, Func<B, C>> (var f), These.That<A, B> (var b))                => That<A, C>(f(b)),
            (These.That<A, Func<B, C>> (var f), These.Both<A, B> (var a, var b))         => Both(a, f(b)),
            (These.Both<A, Func<B, C>> (var a1, _), These.This<A, B> (var a2))           => This<A, C>(a1 + a2),
            (These.Both<A, Func<B, C>> (var a1, var f), These.That<A, B> (var b))        => Both(a1, f(b)),
            (These.Both<A, Func<B, C>> (var a1, var f), These.Both<A, B> (var a, var b)) => Both(a1 + a, f(b)),
            _                                                                            => throw new NSE()
        };

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Chaining function</param>
    public static These<A, C> Bind<A, B, C>(this K<These<A>, B> mb, Func<B, K<These<A>, C>> f)
        where A : Semigroup<A> =>
        mb switch
        {
            These.This<A, B> (var v) => This<A, C>(v),
            These.That<A, B> (var v) => f(v).As(),
            These.Both<A, B> (var x, var y) => f(y) switch
                                               {
                                                   These.This<A, C> (var a)        => This<A, C>(x + a),
                                                   These.That<A, C> (var b)        => Both(x, b),
                                                   These.Both<A, C> (var a, var b) => Both(x + a, b),
                                                   _                               => throw new NSE()
                                               },
            _ => throw new NSE()
        };
    
    public static These<A, D> SelectMany<A, B, C, D>(
        this K<These<A>, B> mb, 
        Func<B, K<These<A>, C>> bind, 
        Func<B, C, D> project) 
        where A : Semigroup<A> =>
        mb.Bind(b => bind(b).Map(c => project(b, c)));
}

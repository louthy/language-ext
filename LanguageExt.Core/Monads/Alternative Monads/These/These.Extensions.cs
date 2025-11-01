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
                                   These<A, B>.This (var x)              => (s.This.Add(x), s.That, s.Both),
                                   These<A, B>.That (var y)        => (s.This, s.That.Add(y), s.Both),
                                   These<A, B>.Both (var x, var y) => (s.This, s.That, s.Both.Add((x, y))),
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
                                          These<A, B>.This (var x)        => (state.This.Add(x), state.That),
                                          These<A, B>.That (var y)        => (state.This, state.That.Add(y)),
                                          These<A, B>.Both (var x, var y) => (state.This.Add(x), state.That.Add(y)),
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
        Combine(first, second, Semigroup.combine, Semigroup.combine);

    /// <summary>
    /// Semigroup combine
    /// </summary>
    /// <param name="first">First `These`</param>
    /// <param name="second">Second `These`</param>
    /// <returns>`These` combined using semigroup rules</returns>
    public static These<A, B> Combine<A, B>(
        this K<These<A>, B> first,
        K<These<A>, B> second,
        Func<A, A, A> combineFst,
        Func<B, B, B> combineSnd) =>
        (first, second) switch
        {
            (These<A, B>.This (var fx), These<A, B>.This (var sx))         => This<A, B>(combineFst(fx, sx)),
            (These<A, B>.This (var fx), These<A, B>.That (var sx))         => Both(fx, sx),
            (These<A, B>.This (var fx), These<A, B>.Both (var s1, var s2)) => Both(combineFst(fx, s1), s2),
            (These<A, B>.That (var fx), These<A, B>.This (var sx))         => Both(sx, fx),
            (These<A, B>.That (var fx), These<A, B>.That (var sx))         => That<A, B>(combineSnd(fx, sx)),
            (These<A, B>.That (var fx), These<A, B>.Both (var s1, var s2)) => Both(s1, combineSnd(fx, s2)),
            (These<A, B>.Both (var f1, var f2), These<A, B>.This (var sx)) => Both(combineFst(f1, sx), f2),
            (These<A, B>.Both (var f1, var f2), These<A, B>.That (var sx)) => Both(f1, combineSnd(f2, sx)),
            (These<A, B>.Both (var f1, var f2), These<A, B>.Both (var s1, var s2)) => Both(combineFst(f1, s1), combineSnd(f2, s2)),
            _ => throw new NSE()
        };

    public static These<A, C> Apply<A, B, C>(this K<These<A>, Func<B, C>> mf, K<These<A>, B> ma)
        where A : Semigroup<A> =>
        mf.Apply(ma, Semigroup.combine);

    public static These<A, C> Apply<A, B, C>(
        this K<These<A>, Func<B, C>> mf,
        K<These<A>, B> ma,
        Func<A, A, A> combine) =>
        (mf, ma) switch
        {
            (These<A, Func<B, C>>.This (var a), _)                                       => This<A, C>(a),
            (These<A, Func<B, C>>.That, These<A, B>.This (var a))                        => This<A, C>(a),
            (These<A, Func<B, C>>.That (var f), These<A, B>.That (var b))                => That<A, C>(f(b)),
            (These<A, Func<B, C>>.That (var f), These<A, B>.Both (var a, var b))         => Both(a, f(b)),
            (These<A, Func<B, C>>.Both (var a1, _), These<A, B>.This (var a2))           => This<A, C>(combine(a1, a2)),
            (These<A, Func<B, C>>.Both (var a1, var f), These<A, B>.That (var b))        => Both(a1, f(b)),
            (These<A, Func<B, C>>.Both (var a1, var f), These<A, B>.Both (var a, var b)) => Both(combine(a1, a), f(b)),
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
            These<A, B>.This (var v) => This<A, C>(v),
            These<A, B>.That (var v) => f(v).As(),
            These<A, B>.Both (var x, var y) => f(y) switch
                                               {
                                                   These<A, C>.This (var a)        => This<A, C>(x + a),
                                                   These<A, C>.That (var b)        => Both(x, b),
                                                   These<A, C>.Both (var a, var b) => Both(x + a, b),
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

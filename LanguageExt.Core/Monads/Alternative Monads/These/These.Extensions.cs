using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static class TheseExtensions
{
    public static These<A, B> As<A, B>(this K<These<A>, B> ma) 
        where A : Semigroup<A> =>
        (These<A, B>)ma;
    
    /// <summary>
    /// Coalesce with the provided operation
    /// </summary>
    /// <param name="f">Coalesce operation</param>
    /// <returns>Coalesced value</returns>
    public static A Merge<A>(this K<These<A>, A> these, Func<A, A, A> f) 
        where A : Semigroup<A> =>
        these.As().Match(x => x, x => x, f);
    
    /// <summary>
    /// Select each constructor and partition them into separate lists.
    /// </summary>
    /// <param name="theses">Selection</param>
    /// <typeparam name="F">Foldable structure</typeparam>
    /// <returns>Partitioned sequences</returns>
    public static (Seq<A> This, Seq<B> That, Seq<(A, B)> Pair) Partition<F, A, B>(
        this K<F, These<A, B>> theses)
        where F : Foldable<F> 
        where A : Semigroup<A> =>
        theses.Fold((This: Seq<A>(), That: Seq<B>(), Pair: Seq<(A First, B Second)>()),
                    (state, these) => these switch
                                      {
                                          This<A, B> (var x)        => (state.This.Add(x), state.That, state.Pair),
                                          That<A, B> (var y)        => (state.This, state.That.Add(y), state.Pair),
                                          Pair<A, B> (var x, var y) => (state.This, state.That, state.Pair.Add((x, y))),
                                          _                         => throw new NotSupportedException()
                                      });


    /// <summary>
    /// Select each constructor and partition them into separate lists.
    /// </summary>
    /// <param name="theses">Selection</param>
    /// <typeparam name="F">Foldable structure</typeparam>
    /// <returns>Partitioned sequences</returns>
    public static (Seq<A> This, Seq<B> That) Partition2<F, A, B>(
        this K<F, These<A, B>> theses)
        where F : Foldable<F> 
        where A : Semigroup<A> =>
        theses.Fold((This: Seq<A>(), That: Seq<B>()),
                    (state, these) => these switch
                                      {
                                          This<A, B> (var x)        => (state.This.Add(x), state.That),
                                          That<A, B> (var y)        => (state.This, state.That.Add(y)),
                                          Pair<A, B> (var x, var y) => (state.This.Add(x), state.That.Add(y)),
                                          _                         => throw new NotSupportedException()
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
            (This<A, B> (var fx), This<A, B> (var sx))                 => These.This<A, B>(fx + sx),
            (This<A, B> (var fx), That<A, B> (var sx))                 => These.Pair(fx, sx),
            (This<A, B> (var fx), Pair<A, B> (var s1, var s2))         => These.Pair(fx + s1, s2),
            (That<A, B> (var fx), This<A, B> (var sx))                 => These.Pair(sx, fx),
            (That<A, B> (var fx), That<A, B> (var sx))                 => These.That<A, B>(fx + sx),
            (That<A, B> (var fx), Pair<A, B> (var s1, var s2))         => These.Pair(s1, fx   + s2),
            (Pair<A, B> (var f1, var f2), This<A, B> (var sx))         => These.Pair(f1       + sx, f2),
            (Pair<A, B> (var f1, var f2), That<A, B> (var sx))         => These.Pair(f1, f2   + sx),
            (Pair<A, B> (var f1, var f2), Pair<A, B> (var s1, var s2)) => These.Pair(f1       + s1, f2 + s2),
            _                                                          => throw new NotSupportedException()
        };
}

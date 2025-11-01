using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;
using NSE = System.NotSupportedException;

namespace LanguageExt;

/// <summary>
/// The `ChronicleT` monad transformer. 
/// </summary>
/// <remarks>
/// Hybrid error/writer monad class that allows both accumulating outputs and aborting computation with a final output.
/// 
/// The expected use case is for computations with a notion of fatal vs. non-fatal errors.
/// </remarks>
/// <remarks>
/// The 'pure' function produces a computation with no output, and `Bind` combines multiple
/// outputs with semigroup combine.
/// </remarks>
/// <param name="runChronicleT">Composed monadic type</param>
/// <typeparam name="Ch">Chronicle type</typeparam>
/// <typeparam name="M">Monad type</typeparam>
/// <typeparam name="A">Bound value type</typeparam>
public record ChronicleT<Ch, M, A>(Func<SemigroupInstance<Ch>, K<M, These<Ch, A>>> runChronicleT) : 
    K<ChronicleT<Ch, M>, A>,
    K<ChronicleT<M>, Ch, A>
    where M : Monad<M>
{
    /// <summary>
    /// Run the chronicle to yield its inner monad
    /// </summary>
    /// <param name="f">Semigroup combine operation</param>
    public K<M, These<Ch, A>> Run(SemigroupInstance<Ch> trait) =>
        runChronicleT(trait);    
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Resulting bound value type</typeparam>
    /// <returns>Mapped structure</returns>
    public ChronicleT<Ch, M, B> Map<B>(Func<A, B> f) =>
        new(c => runChronicleT(c).Map(these => these.Map(f)));

    /// <summary>
    /// Bifunctor map operation
    /// </summary>
    /// <param name="This">Chronicle mapping function</param>
    /// <param name="That">Dictation mapping function</param>
    /// <typeparam name="Ch1">Chronicle type to map to</typeparam>
    /// <returns></returns>
    public ChronicleT<Ch1, M, B> BiMap<Ch1, B>(Func<Ch, Ch1> This, Func<A, B> That)
        where Ch1 : Semigroup<Ch1> =>
        Bifunctor.bimap(This, That, this).As2();
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Resulting bound value type</typeparam>
    /// <returns>Mapped structure</returns>
    public ChronicleT<Ch, M, B> Select<B>(Func<A, B> f) =>
        new(c => runChronicleT(c).Map(these => these.Map(f)));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Chaining function</param>
    /// <returns>Chained structure</returns>
    public ChronicleT<Ch, M, B> Bind<B>(Func<A, K<ChronicleT<Ch, M>, B>> f) =>
        new(t =>
                Run(t)
                   .Bind(cx => cx switch
                               {
                                   These<Ch, A>.This (var c) =>
                                       M.Pure(This<Ch, B>(c)),

                                   These<Ch, A>.That (var x) =>
                                       f(x).As().Run(t),

                                   These<Ch, A>.Both (var c1, var x) =>
                                       f(x).As()
                                           .Run(t)
                                           .Map(cy => cy switch
                                                      {
                                                          These<Ch, B>.This (var c2) => This<Ch, B>(t.Combine(c1, c2)),
                                                          These<Ch, B>.That (var b)  => Both(c1, b),
                                                          These<Ch, B>.Both (var c2, var b) => Both(t.Combine(c1, c2), b),
                                                          _ => throw new NSE()
                                                      }),

                                   _ => throw new NSE()
                               }));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Chaining function</param>
    /// <returns>Chained structure</returns>
    public ChronicleT<Ch1, M, A> BindFirst<Ch1>(Func<Ch, K<ChronicleT<M>, Ch1, A>> f)
        where Ch1 : Semigroup<Ch1> =>
        Bimonad.bindFirst(this, f).As2();

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Chaining function</param>
    /// <returns>Chained structure</returns>
    public ChronicleT<Ch, M, B> BindSecond<B>(Func<A, K<ChronicleT<M>, Ch, B>> f) =>
        Bimonad.bindSecond(this, f).As2();

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Chaining function</param>
    /// <typeparam name="B">Resulting bound value type</typeparam>
    /// <returns>Chained structure</returns>
    public ChronicleT<Ch, M, C> SelectMany<B, C>(Func<A, K<ChronicleT<Ch, M>, B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Chaining function</param>
    /// <typeparam name="B">Resulting bound value type</typeparam>
    /// <returns>Chained structure</returns>
    public ChronicleT<Ch, M, C> SelectMany<B, C>(Func<A, IO<B>> bind, Func<A, B, C> project) =>
        Bind(x => ChronicleT.liftIO<Ch, M, B>(bind(x)).Map(y => project(x, y)));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Chaining function</param>
    /// <typeparam name="B">Resulting bound value type</typeparam>
    /// <returns>Chained structure</returns>
    public ChronicleT<Ch, M, C> SelectMany<C>(Func<A, Guard<Ch, Unit>> bind, Func<A, Unit, C> project) =>
        Bind(x => bind(x).ToChronicleT<Ch, M>().Map(y => project(x, y)));

    /// <summary>
    /// `Memento` is an action that executes the action within this structure, returning either
    /// its record, if it ended with `Confess`, or its final value otherwise, with any record
    /// added to the current record.
    ///
    /// Similar to 'Catch' in the 'Fallible' trait, but with a notion of non-fatal errors (which
    /// are accumulated) vs. fatal errors (which are caught without accumulating).
    /// </summary>
    public ChronicleT<Ch, M, Either<Ch, A>> Memento() =>
        new(combine =>
            Run(combine).Map(these => these switch
                                      {
                                          These<Ch, A>.This (var c)        => That<Ch, Either<Ch, A>>(Either<Ch, A>.Left(c)),
                                          These<Ch, A>.That (var x)        => That<Ch, Either<Ch, A>>(Either<Ch, A>.Right(x)),
                                          These<Ch, A>.Both (var c, var x) => Both(c, Either<Ch, A>.Right(x)),
                                          _                                => throw new NSE()
                                      }));

    /// <summary>
    /// `Absolve` is an action that executes this structure and discards any record it had.
    /// The `defaultValue` will be used if the action ended via `Confess`. 
    /// </summary>
    /// <param name="defaultValue"></param>
    public ChronicleT<Ch, M, A> Absolve(A defaultValue) =>
        new(combine => Run(combine).Map(these => these switch
                                                 {
                                                     These<Ch, A>.This            => That<Ch, A>(defaultValue),
                                                     These<Ch, A>.That (var x)    => That<Ch, A>(x),
                                                     These<Ch, A>.Both (_, var x) => That<Ch, A>(x),
                                                     _                            => throw new NSE()
                                                 }));

    /// <summary>
    /// This can be seen as converting non-fatal errors into fatal ones.
    /// 
    /// `Condemn` is an action that executes the structure and keeps its value
    /// only if it had no record. Otherwise, the value (if any) will be discarded
    /// and only the record kept.
    /// </summary>
    public ChronicleT<Ch, M, A> Condemn() =>
        new(combine => Run(combine).Map(these => these switch
                                                 {
                                                     These<Ch, A>.This (var c)    => This<Ch, A>(c),
                                                     These<Ch, A>.That (var x)    => That<Ch, A>(x),
                                                     These<Ch, A>.Both (var c, _) => This<Ch, A>(c),
                                                     _                            => throw new NSE()
                                                 }));

    /// <summary>
    /// An action that executes the structure and applies the function `f` to its output, leaving
    /// the return value unchanged.-
    /// </summary>
    /// <remarks>
    /// Equivalent to `censor` for the 'Writable` trait.
    /// </remarks>
    /// <param name="f">Censoring function</param>
    public ChronicleT<Ch, M, A> Censor(Func<Ch, Ch> f) =>
        new(combine => Run(combine).Map(these => these switch
                                                 {
                                                     These<Ch, A>.This (var c)        => This<Ch, A>(f(c)),
                                                     These<Ch, A>.That (var x)        => That<Ch, A>(x),
                                                     These<Ch, A>.Both (var c, var x) => Both(f(c), x),
                                                     _                                => throw new NSE()
                                                 }));
    
    /// <summary>
    /// Coalescing operation
    /// </summary>
    public ChronicleT<Ch, M, A> Choose(K<ChronicleT<Ch, M>, A> rhs) =>
        Memento()
           .Bind(x => x switch
                      {
                          Either.Left<Ch, A>         => rhs,
                          Either.Right<Ch, A>(var r) => dictate<Ch, M, A>(r),
                          _                          => throw new NSE()
                      });
    
    /// <summary>
    /// Coalescing operation
    /// </summary>
    public ChronicleT<Ch, M, A> Choose(Func<K<ChronicleT<Ch, M>, A>> rhs) =>
        Memento()
           .Bind(x => x switch
                      {
                          Either.Left<Ch, A>         => rhs(),
                          Either.Right<Ch, A>(var r) => dictate<Ch, M, A>(r),
                          _                          => throw new NSE()
                      });
    
    /// <summary>
    /// Fallible error catching operation
    /// </summary>
    /// <param name="Predicate"></param>
    /// <param name="Fail"></param>
    /// <returns></returns>
    public ChronicleT<Ch, M, A> Catch(Func<Ch, bool> Predicate, Func<Ch, K<ChronicleT<Ch, M>, A>> Fail) =>
        Memento()
           .Bind(x => x switch
                      {
                          Either.Left<Ch, A> (var e) => Predicate(e) ? Fail(e) : confess<Ch, M, A>(e),
                          Either.Right<Ch, A>(var r) => dictate<Ch, M, A>(r),
                          _                          => throw new NSE()
                      });

    /// <summary>
    /// Coalescing operator
    /// </summary>
    public static ChronicleT<Ch, M, A> operator |(ChronicleT<Ch, M, A> lhs, K<ChronicleT<Ch, M>, A> rhs) =>
        lhs.Choose(rhs);

    /// <summary>
    /// Coalescing operator
    /// </summary>
    public static ChronicleT<Ch, M, A> operator |(ChronicleT<Ch, M, A> lhs, Pure<A> rhs) =>
        lhs.Choose(dictate<Ch, M, A>(rhs.Value));    

    /// <summary>
    /// Coalescing operator
    /// </summary>
    public static ChronicleT<Ch, M, A> operator |(ChronicleT<Ch, M, A> lhs, Fail<Ch> rhs) =>
        lhs.Choose(confess<Ch, M, A>(rhs.Value));    
}

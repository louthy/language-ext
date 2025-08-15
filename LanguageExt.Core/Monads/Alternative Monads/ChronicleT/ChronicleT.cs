using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;
using NSE = System.NotSupportedException;

namespace LanguageExt;

/// <summary>
/// The `ChronicleT` monad transformer. 
/// </summary>
/// <remarks>
/// The 'pure' function produces a computation with no output, and `Bind` combines multiple
/// outputs with semigroup combine.
/// </remarks>
/// <param name="runChronicleT">Composed monadic type</param>
/// <typeparam name="Ch">Chronicle type</typeparam>
/// <typeparam name="M">Monad type</typeparam>
/// <typeparam name="A">Bound value type</typeparam>
public record ChronicleT<Ch, M, A>(K<M, These<Ch, A>> runChronicleT) : K<ChronicleT<Ch, M>, A>
    where Ch : Semigroup<Ch>
    where M : Monad<M>
{
    /// <summary>
    /// Run the chronicle to yield its inner monad
    /// </summary>
    public K<M, These<Ch, A>> Run() =>
        runChronicleT;    
    
    /// <summary>
    /// `Dictate` is an action that records the output `value`.
    /// Equivalent to `tell` for the `Writable` traits.
    /// </summary>
    /// <param name="value">Value to construct with</param>
    /// <returns>Chronicle structure</returns>
    public static ChronicleT<Ch, M, A> Dictate(A value) =>
        new(M.Pure(That<Ch, A>(value)));
    
    /// <summary>
    /// `Confess` is an action that ends with a final output `value`.
    /// Equivalent to `fail` for the 'Fallible' trait.
    /// </summary>
    /// <param name="value">Value to construct with</param>
    /// <returns>Chronicle structure</returns>
    public static ChronicleT<Ch, M, A> Confess(Ch value) => 
        new(M.Pure(This<Ch, A>(value)));
    
    /// <summary>
    /// Construct a new chronicle with `this` and `that`.
    /// </summary>
    /// <param name="@this">Value to construct with</param>
    /// <param name="that">Value to construct with</param>
    /// <returns>Chronicle structure</returns>
    public static ChronicleT<Ch, M, A> Chronicle(Ch @this, A that) => 
        new(M.Pure(Both(@this, that)));
    
    /// <summary>
    /// Construct a new chronicle with `these`.
    /// </summary>
    /// <param name="these">What to chronicle</param>
    /// <returns>Chronicle structure</returns>
    public static ChronicleT<Ch, M, A> Chronicle(These<Ch, A> these) => 
        new(M.Pure(these));
    
    /// <summary>
    /// Lift a monad `M` into the monad-transformer
    /// </summary>
    /// <param name="ma">Monad to lift</param>
    /// <returns>Chronicle structure</returns>
    public static ChronicleT<Ch, M, A> Lift(K<M, A> ma) =>
        new(ma.Map(That<Ch, A>));
    
    /// <summary>
    /// Lift an `IO` monad into the monad-transformer
    /// </summary>
    /// <param name="ma">Monad to lift</param>
    /// <returns>Chronicle structure</returns>
    public static ChronicleT<Ch, M, A> LiftIO(K<IO, A> ma) =>
        Lift(M.LiftIOMaybe(ma));
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Resulting bound value type</typeparam>
    /// <returns>Mapped structure</returns>
    public ChronicleT<Ch, M, B> Map<B>(Func<A, B> f) =>
        new(runChronicleT.Map(these => these.Map(f)));
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Resulting bound value type</typeparam>
    /// <returns>Mapped structure</returns>
    public ChronicleT<Ch, M, B> Select<B>(Func<A, B> f) =>
        new(runChronicleT.Map(these => these.Map(f)));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Chaining function</param>
    /// <typeparam name="B">Resulting bound value type</typeparam>
    /// <returns>Chained structure</returns>
    public ChronicleT<Ch, M, B> Bind<B>(Func<A, K<ChronicleT<Ch, M>, B>> f) =>
        new(Run().Bind(cx => cx switch
                             {
                                 These.This<Ch, A> (var c) =>
                                     M.Pure(This<Ch, B>(c)),

                                 These.That<Ch, A> (var x) =>
                                     f(x).Run(),

                                 These.Both<Ch, A> (var c1, var x) =>
                                     f(x).Run()
                                         .Map(cy => cy switch
                                                    {
                                                        These.This<Ch, B> (var c2)        => This<Ch, B>(c1 + c2),
                                                        These.That<Ch, B> (var b)         => Both(c1, b),
                                                        These.Both<Ch, B> (var c2, var b) => Both(c1 + c2, b),
                                                        _                                 => throw new NSE()
                                                    }),

                                 _ => throw new NSE()
                             }));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Chaining function</param>
    /// <typeparam name="B">Resulting bound value type</typeparam>
    /// <returns>Chained structure</returns>
    public ChronicleT<Ch, M, C> SelectMany<B, C>(Func<A, K<ChronicleT<Ch, M>, B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    /// <summary>
    /// `Memento` is an action that executes the action within this structure, returning either
    /// its record, if it ended with `Confess`, or its final value otherwise, with any record
    /// added to the current record.
    ///
    /// Similar to 'Catch' in the 'Fallible' trait, but with a notion of non-fatal errors (which
    /// are accumulated) vs. fatal errors (which are caught without accumulating).
    /// </summary>
    public ChronicleT<Ch, M, Either<Ch, A>> Memento() =>
        new(Run().Map(these => these switch
                               {
                                   These.This<Ch, A> (var c)        => That<Ch, Either<Ch, A>>(Either<Ch, A>.Left(c)),
                                   These.That<Ch, A> (var x)        => That<Ch, Either<Ch, A>>(Either<Ch, A>.Right(x)),
                                   These.Both<Ch, A> (var c, var x) => Both(c, Either<Ch, A>.Right(x)),
                                   _                                => throw new NSE()
                               }));

    /// <summary>
    /// `Absolve` is an action that executes this structure and discards any record it had.
    /// The `defaultValue` will be used if the action ended via `Confess`. 
    /// </summary>
    /// <param name="defaultValue"></param>
    public ChronicleT<Ch, M, A> Absolve(A defaultValue) =>
        new(Run().Map(these => these switch
                               {
                                   These.This<Ch, A>            => That<Ch, A>(defaultValue),
                                   These.That<Ch, A> (var x)    => That<Ch, A>(x),
                                   These.Both<Ch, A> (_, var x) => That<Ch, A>(x),
                                   _                            => throw new NSE()
                               }));

    /// <summary>
    /// `Condemn` is an action that executes the structure and keeps its value
    /// only if it had no record. Otherwise, the value (if any) will be discarded
    /// and only the record kept.
    /// 
    /// This can be seen as converting non-fatal errors into fatal ones.
    /// </summary>
    public ChronicleT<Ch, M, A> Condemn() =>
        new(Run().Map(these => these switch
                               {
                                   These.This<Ch, A> (var c)    => This<Ch, A>(c),
                                   These.That<Ch, A> (var x)    => That<Ch, A>(x),
                                   These.Both<Ch, A> (var c, _) => This<Ch, A>(c),
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
        new(Run().Map(these => these switch
                               {
                                   These.This<Ch, A> (var c)        => This<Ch, A>(f(c)),
                                   These.That<Ch, A> (var x)        => That<Ch, A>(x),
                                   These.Both<Ch, A> (var c, var x) => Both(f(c), x),
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
                          Either.Right<Ch, A>(var r) => Dictate(r),
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
                          Either.Right<Ch, A>(var r) => Dictate(r),
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
                          Either.Left<Ch, A> (var e) => Predicate(e) ? Fail(e) : Confess(e),
                          Either.Right<Ch, A>(var r) => Dictate(r),
                          _                          => throw new NSE()
                      });

    /// <summary>
    /// Coalescing operator
    /// </summary>
    public static ChronicleT<Ch, M, A> operator |(ChronicleT<Ch, M, A> lhs, K<ChronicleT<Ch, M>, A> rhs) =>
        lhs.Choose(rhs);
}

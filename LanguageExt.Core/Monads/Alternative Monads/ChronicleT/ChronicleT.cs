using System;
using LanguageExt.Traits;

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
        new(M.Pure(These.That<Ch, A>(value)));
    
    /// <summary>
    /// `Confess` is an action that ends with a final output `value`.
    /// Equivalent to `fail` for the 'Fallible' trait.
    /// </summary>
    /// <param name="value">Value to construct with</param>
    /// <returns>Chronicle structure</returns>
    public static ChronicleT<Ch, M, A> Confess(Ch value) => 
        new(M.Pure(These.This<Ch, A>(value)));
    
    /// <summary>
    /// Construct a new chronicle with `this` and `that`.
    /// </summary>
    /// <param name="@this">Value to construct with</param>
    /// <param name="that">Value to construct with</param>
    /// <returns>Chronicle structure</returns>
    public static ChronicleT<Ch, M, A> Chronicle(Ch @this, A that) => 
        new(M.Pure(These.Pair(@this, that)));
    
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
        new(ma.Map(These.That<Ch, A>));
    
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
                                 This<Ch, A> (var c) =>
                                     M.Pure(These.This<Ch, B>(c)),

                                 That<Ch, A> (var x) =>
                                     f(x).Run(),

                                 Pair<Ch, A> (var c1, var x) =>
                                     f(x).Run()
                                         .Map(cy => cy switch
                                                    {
                                                        This<Ch, B> (var c2)        => These.This<Ch, B>(c1 + c2),
                                                        That<Ch, B> (var b)         => These.Pair(c1, b),
                                                        Pair<Ch, B> (var c2, var b) => These.Pair(c1 + c2, b),
                                                        _                          => throw new NotSupportedException()
                                                    }),

                                 _ => throw new NotSupportedException()
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
                                   This<Ch, A> (var c)        => These.That<Ch, Either<Ch, A>>(Either<Ch, A>.Left(c)),
                                   That<Ch, A> (var x)        => These.That<Ch, Either<Ch, A>>(Either<Ch, A>.Right(x)),
                                   Pair<Ch, A> (var c, var x) => These.Pair(c, Either<Ch, A>.Right(x)),
                                   _                          => throw new NotSupportedException()
                               }));
    
    /// <summary>
    /// `Absolve` is an action that executes this structure and discards any record it had.
    /// The `defaultValue` will be used if the action ended via `Confess`. 
    /// </summary>
    /// <param name="defaultValue"></param>
    public ChronicleT<Ch, M, A> Absolve(A defaultValue) =>
        new(Run().Map(these => these switch
                               {
                                   This<Ch, A>            => These.That<Ch, A>(defaultValue),
                                   That<Ch, A> (var x)    => These.That<Ch, A>(x),
                                   Pair<Ch, A> (_, var x) => These.That<Ch, A>(x),
                                   _                      => throw new NotSupportedException()
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
                                   This<Ch, A> (var c)    => These.This<Ch, A>(c),
                                   That<Ch, A> (var x)    => These.That<Ch, A>(x),
                                   Pair<Ch, A> (var c, _) => These.This<Ch, A>(c),
                                   _                      => throw new NotSupportedException()
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
                                   This<Ch, A> (var c)        => These.This<Ch, A>(f(c)),
                                   That<Ch, A> (var x)        => These.That<Ch, A>(x),
                                   Pair<Ch, A> (var c, var x) => These.Pair(f(c), x),
                                   _                          => throw new NotSupportedException()
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
                          _                          => throw new NotSupportedException()
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
                          _                          => throw new NotSupportedException()
                      });

    /// <summary>
    /// Coalescing operator
    /// </summary>
    public static ChronicleT<Ch, M, A> operator |(ChronicleT<Ch, M, A> lhs, K<ChronicleT<Ch, M>, A> rhs) =>
        lhs.Choose(rhs);
}

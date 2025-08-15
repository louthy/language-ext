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
    where Ch : Monoid<Ch>
    where M : Monad<M>
{
    /// <summary>
    /// Run the chronicle to yield its inner monad
    /// </summary>
    public K<M, These<Ch, A>> Run() =>
        runChronicleT;    
    
    /// <summary>
    /// Construct a new chronicle with a pure value
    /// </summary>
    /// <param name="value">Value to construct with</param>
    /// <returns>Chronicle structure</returns>
    public static ChronicleT<Ch, M, A> That(A value) =>
        new(M.Pure(These.That<Ch, A>(value)));
    
    /// <summary>
    /// Construct a new chronicle with a 'failure' value
    /// </summary>
    /// <param name="value">Value to construct with</param>
    /// <returns>Chronicle structure</returns>
    public static ChronicleT<Ch, M, A> This(Ch value) =>
        new(M.Pure(These.This<Ch, A>(value)));
    
    /// <summary>
    /// Construct a new chronicle with both a 'failure' and a success value.
    /// </summary>
    /// <param name="@this">Value to construct with</param>
    /// <param name="that">Value to construct with</param>
    /// <returns>Chronicle structure</returns>
    public static ChronicleT<Ch, M, A> Pair(Ch @this, A that) => 
        new(M.Pure(These.Pair(@this, that)));
    
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
}

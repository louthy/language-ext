using System;
using LanguageExt.Common;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

/// <summary>
/// `TryT` monad transformer, which allows for an optional result. 
/// </summary>
/// <param name="runEither">Transducer that represents the transformer operation</param>
/// <typeparam name="M">Given monad trait</typeparam>
/// <typeparam name="A">Bound value type</typeparam>
public record TryT<M, A>(K<M, Try<A>> runTry) : K<TryT<M>, A>, Semigroup<TryT<M, A>>
    where M : Monad<M>
{
    /// <summary>
    /// Lift a pure value into the monad-transformer
    /// </summary>
    /// <param name="value">Value to lift</param>
    /// <returns>`TryT`</returns>
    public static TryT<M, A> Succ(A value) =>
        Lift(M.Pure(value));
    
    /// <summary>
    /// Lift a fail value into the monad-transformer
    /// </summary>
    /// <param name="value">Value to lift</param>
    /// <returns>`TryT`</returns>
    public static TryT<M, A> Fail(Error value) =>
        Lift(Fin<A>.Fail(value));

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <returns>`TryT`</returns>
    public static TryT<M, A> Lift(Pure<A> pure) =>
        Succ(pure.Value);

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <returns>`TryT`</returns>
    public static TryT<M, A> Lift(Fin<A> result) =>
        new(M.Pure(Try.lift(result)));

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <returns>`TryT`</returns>
    public static TryT<M, A> Lift(Func<Fin<A>> result) =>
        new(M.Pure(Try.lift(result)));

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="fail">Monad to lift</param>
    /// <returns>`TryT`</returns>
    public static TryT<M, A> Lift(Fail<Error> fail) =>
        Lift(Fin<A>.Fail(fail.Value));

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="monad">Monad to lift</param>
    /// <returns>`TryT`</returns>
    public static TryT<M, A> Lift(K<M, A> monad) =>
        new(M.Map(Try<A>.Succ, monad));

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="monad">Monad to lift</param>
    /// <returns>`TryT`</returns>
    public static TryT<M, A> LiftIO(IO<A> monad) =>
        new(M.LiftIO(monad.Try()).Map(Try<A>.Lift));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Match
    //

    /// <summary>
    /// Match the bound value and return a result (which gets packages back up inside the inner monad)
    /// </summary>
    /// <param name="Succ">Success branch</param>
    /// <param name="Fail">Fail branch</param>
    /// <returns>Inner monad with the result of the `Succ` or `Fail` branches</returns>
    public K<M, B> Match<B>(Func<A, B> Succ, Func<Error, B> Fail) =>
        M.Map(mx => mx.Match(Succ, Fail), Run());

    /// <summary>
    /// Match the bound value and return a result (which gets packages back up inside the inner monad)
    /// </summary>
    /// <param name="Succ">Success branch</param>
    /// <param name="Fail">Fail branch</param>
    /// <returns>Inner monad with the result of the `Succ` or `Fail` branches</returns>
    public K<M, A> IfFail(Func<Error, A> Fail) =>
        Match(identity, Fail);

    /// <summary>
    /// Match the bound value and return a result (which gets packages back up inside the inner monad)
    /// </summary>
    /// <param name="Succ">Success branch</param>
    /// <param name="Fail">Fail branch</param>
    /// <returns>Inner monad with the result of the `Succ` or `Fail` branches</returns>
    public K<M, A> IfFailM(Func<Error, K<M, A>> Fail) =>
        Match(M.Pure, Fail).Flatten();

    /// <summary>
    /// Run the transformer
    /// </summary>
    /// <remarks>
    /// This is where the exceptions are caught
    /// </remarks>
    public K<M, Fin<A>> Run() =>
        runTry.Map(t => t.Run());

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Map
    //

    /// <summary>
    /// Maps the bound monad
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="M1">Target monad type</typeparam>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>Mapped monad</returns>
    public TryT<M1, B> MapT<M1, B>(Func<K<M, Fin<A>>, K<M1, Fin<B>>> f)
        where M1 : Monad<M1> =>
        new(f(Run()).Map(Try.lift));

    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`TryT`</returns>
    public TryT<M, B> Map<B>(Func<A, B> f) =>
        new(M.Map(mx => mx.Map(f), runTry));
    
    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <param name="f">Mapping transducer</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`TryT`</returns>
    public TryT<M, B> Select<B>(Func<A, B> f) =>
        new(M.Map(mx => mx.Map(f), runTry));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Bind
    //

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`TryT`</returns>
    public TryT<M, B> Bind<B>(Func<A, K<TryT<M>, B>> f) =>
        Map(f).Flatten();

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`TryT`</returns>
    public TryT<M, B> Bind<B>(Func<A, TryT<M, B>> f) =>
        Map(f).Flatten();

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`TryT`</returns>
    public TryT<M, B> Bind<B>(Func<A, IO<B>> f) =>
        Bind(a => TryT<M, B>.LiftIO(f(a)));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`TryT`</returns>
    public TryT<M, B> Bind<B>(Func<A, Pure<B>> f) =>
        Map(a => f(a).Value);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  SelectMany
    //

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`TryT`</returns>
    public TryT<M, C> SelectMany<B, C>(Func<A, K<TryT<M>, B>> bind, Func<A, B, C> project) =>
        SelectMany(x => bind(x).As(), project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`TryT`</returns>
    public TryT<M, C> SelectMany<B, C>(Func<A, TryT<M, B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`TryT`</returns>
    public TryT<M, C> SelectMany<B, C>(Func<A, K<M, B>> bind, Func<A, B, C> project) =>
        SelectMany(x => TryT<M, B>.Lift(bind(x)), project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`TryT`</returns>
    public TryT<M, C> SelectMany<B, C>(Func<A, Fin<B>> bind, Func<A, B, C> project) =>
        SelectMany(x => TryT<M, B>.Lift(bind(x)), project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`TryT`</returns>
    public TryT<M, C> SelectMany<B, C>(Func<A, Pure<B>> bind, Func<A, B, C> project) =>
        Map(x => project(x, bind(x).Value));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`TryT`</returns>
    public TryT<M, C> SelectMany<B, C>(Func<A, IO<B>> bind, Func<A, B, C> project) =>
        SelectMany(x => M.LiftIO(bind(x)), project);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Conversion operators
    //

    public static implicit operator TryT<M, A>(Pure<A> ma) =>
        Succ(ma.Value);
    
    public static implicit operator TryT<M, A>(Error ma) =>
        Lift(Fin<A>.Fail(ma));
    
    public static implicit operator TryT<M, A>(Fail<Error> ma) =>
        Lift(Fin<A>.Fail(ma.Value));
    
    public static implicit operator TryT<M, A>(Fail<Exception> ma) =>
        Lift(Fin<A>.Fail(ma.Value));
    
    public static implicit operator TryT<M, A>(IO<A> ma) =>
        LiftIO(ma);
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Trait implementation
    //

    public static TryT<M, A> operator |(TryT<M, A> ma, Pure<A> mb) =>
        ma.Combine(mb);

    public static TryT<M, A> operator |(TryT<M, A> ma, Fail<Error> mb) =>
        ma.Combine(mb);

    public static TryT<M, A> operator |(TryT<M, A> ma, Fail<Exception> mb) =>
        ma.Combine(mb);

    public static TryT<M, A> operator |(TryT<M, A> ma, Error mb) =>
        ma.Combine(mb);

    public static TryT<M, A> operator |(TryT<M, A> ma, CatchError mb) =>
        ma | mb.As();

    public static TryT<M, A> operator |(TryT<M, A> ma, CatchValue<A> mb) =>
        ma | mb.As();

    public static TryT<M, A> operator |(TryT<M, A> ma, CatchIO<A> mb) =>
        new(ma.Run()
              .Map(fa => fa switch
                         {
                             Fin.Fail<A> (var err) when mb.Match(err) => LiftIO(mb.Value(err)),
                             Fin.Fail<A> (var err)                    => Fail(err),
                             Fin.Succ<A> (var val)                    => Succ(val),
                             _                                        => throw new NotSupportedException()
                         })
              .Map(ta => ta.runTry)
              .Flatten());

    public static TryT<M, A> operator |(TryT<M, A> ma, CatchM<TryT<M>, A> mb) =>
        new(ma.Run()
              .Map(fa => fa switch
                         {
                             Fin.Fail<A> (var err) when mb.Match(err) => mb.Value(err).As(),
                             Fin.Fail<A> (var err)                    => Fail(err),
                             Fin.Succ<A> (var val)                    => Succ(val),
                             _                                        => throw new NotSupportedException()
                         })
              .Map(ta => ta.runTry)
              .Flatten());

    public static TryT<M, A> operator |(TryT<M, A> ma, CatchError<Error> mb) =>
        new(ma.Run()
              .Map(fa => fa switch
                         {
                             Fin.Fail<A> (var err) when mb.Match(err) => Try.Fail<A>(mb.Value(err)),
                             Fin.Fail<A> (var err)                    => Try.Fail<A>(err),
                             Fin.Succ<A> (var val)                    => Try.Succ(val),
                             _                                        => throw new NotSupportedException()
                         }));

    public static TryT<M, A> operator |(TryT<M, A> ma, CatchError<Exception> mb) =>
        new(ma.Run()
              .Map(fa => fa switch
                         {
                             Fin.Fail<A> (var err) when mb.Match(err) => Try.Fail<A>(mb.Value(err.ToException())),
                             Fin.Fail<A> (var err)                    => Try.Fail<A>(err),
                             Fin.Succ<A> (var val)                    => Try.Succ(val),
                             _                                        => throw new NotSupportedException()
                         }));

    public static TryT<M, A> operator |(TryT<M, A> ma, CatchValue<Error, A> mb) =>
        new(ma.Run()
              .Map(fa => fa switch
                         {
                             Fin.Fail<A> (var err) when mb.Match(err) => Try.Succ(mb.Value(err)),
                             Fin.Fail<A> (var err)                    => Try.Fail<A>(err),
                             Fin.Succ<A> (var val)                    => Try.Succ(val),
                             _                                        => throw new NotSupportedException()
                         }));

    public static TryT<M, A> operator |(TryT<M, A> ma, CatchValue<Exception, A> mb) =>
        new(ma.Run()
              .Map(fa => fa switch
                         {
                             Fin.Fail<A> (var err) when mb.Match(err) => Try.Succ(mb.Value(err.ToException())),
                             Fin.Fail<A> (var err)                    => Try.Fail<A>(err),
                             Fin.Succ<A> (var val)                    => Try.Succ(val),
                             _                                        => throw new NotSupportedException()
                         }));
    
    public static TryT<M, A> operator |(TryT<M, A> lhs, TryT<M, A> rhs) =>
        lhs.Combine(rhs);

    public TryT<M, A> Combine(TryT<M, A> rhs) =>
        new(Run().Bind(
                lhs => lhs switch
                       {
                           Fin.Succ<A> (var x) => M.Pure(Try.Succ(x)),
                           Fin.Fail<A>         => rhs.runTry,
                           _                   => throw new NotSupportedException()
                       }));
}

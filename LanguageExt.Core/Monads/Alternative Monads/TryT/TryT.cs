using System;
using LanguageExt.Common;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

/// <summary>
/// `TryT` monad transformer, which allows for an  optional `Error` result and catches exceptions,
/// converting them to `Error`. 
/// </summary>
/// <typeparam name="M">Given monad trait</typeparam>
/// <typeparam name="A">Bound value type</typeparam>
public record TryT<M, A>(K<M, Try<A>> runTry) : 
    K<TryT<M>, A>
    where M : Monad<M>
{
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
        M.Map(mx => mx.Match(Succ, Fail), this.Run());

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
        new(f(this.Run()).Map(Try.lift));

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
    /// <param name="f">Mapping function</param>
    /// <returns>`TryT`</returns>
    public TryT<M, A> MapFail(Func<Error, Error> f) =>
        new(M.Map(mx => mx.MapFail(f), runTry));
    
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
    /// <param name="Succ">Success mapping function</param>
    /// <param name="Fail">Failure mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`TryT`</returns>
    public TryT<M, B> BiBind<B>(Func<A, TryT<M, B>> Succ, Func<Error, TryT<M, B>> Fail) =>
        new (runTry.Bind(
                 ta => ta.runTry()
                         .Match(Succ: Succ, Fail: Fail)
                         .runTry));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="Fail">Failure mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`TryT`</returns>
    public TryT<M, A> BindFail(Func<Error, TryT<M, A>> Fail) =>
        BiBind(TryT.Succ<M, A>, Fail);

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
        SelectMany(x => TryT.lift(bind(x)), project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`TryT`</returns>
    public TryT<M, C> SelectMany<B, C>(Func<A, Fin<B>> bind, Func<A, B, C> project) =>
        SelectMany(x => TryT.lift<M, B>(bind(x)), project);

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

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Operators
    //

    public static implicit operator TryT<M, A>(Pure<A> ma) =>
        TryT.Succ<M, A>(ma.Value);
    
    public static implicit operator TryT<M, A>(Error ma) =>
        TryT.lift<M, A>(Fin.Fail<A>(ma));
    
    public static implicit operator TryT<M, A>(Fail<Error> ma) =>
        TryT.lift<M, A>(Fin.Fail<A>(ma.Value));
    
    public static implicit operator TryT<M, A>(Fail<Exception> ma) =>
        TryT.lift<M, A>(Fin.Fail<A>(ma.Value));
    
    public static implicit operator TryT<M, A>(IO<A> ma) =>
        TryT.liftIOMaybe<M, A>(ma);
    
    public TryT<M, A> Combine(TryT<M, A> rhs) =>
        this.Kind().Combine(rhs).As();
}

using System;
using System.Diagnostics.Contracts;
using LanguageExt.Common;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

/// <summary>
/// `FinT` monad transformer, which allows for either an `Error` or `R` result value to be carried. 
/// </summary>
/// <typeparam name="M">Given monad trait</typeparam>
/// <typeparam name="A">Bound value type</typeparam>
public record FinT<M, A>(K<M, Fin<A>> runFin) : 
    K<FinT<M>, A>
    where M : Monad<M>
{
    /// <summary>
    /// Is the `FinT` in a `Succ` state?
    /// </summary>
    public K<M, bool> IsSucc =>
        runFin.Map(f => f.IsSucc);

    /// <summary>
    /// Is the `FinT` in a `Fail` state?
    /// </summary>
    public K<M, bool> IsFail =>
        runFin.Map(f => f.IsFail);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Match
    //

    /// <summary>
    /// Invokes the `Succ` or `Fail` function depending on the state of the `FinT`
    /// </summary>
    /// <typeparam name="B">Return type</typeparam>
    /// <param name="Fail">Function to invoke if in a Fail state</param>
    /// <param name="Succ">Function to invoke if in a Succ state</param>
    /// <returns>The return value of the invoked function</returns>
    [Pure]
    public K<M, B> Match<B>(Func<A, B> Succ, Func<Error, B> Fail) =>
        M.Map(mx => mx.Match(Fail: Fail, Succ: Succ), runFin);

    /// <summary>
    /// Invokes the `Succ` or `Fail` function depending on the state of the `FinT`
    /// </summary>
    /// <param name="Succ">Action to invoke if in a `Succ` state</param>
    /// <param name="Fail">Action to invoke if in a `Fail` state</param>
    /// <returns>Unit</returns>
    [Pure]
    public K<M, Unit> Match(Action<A> Succ, Action<Error> Fail) =>
        M.Map(mx => mx.Match(Fail: Fail, Succ: Succ), runFin);
 
    /// <summary>
    /// Executes the `Fail` function if the `Fin` is in a `Fail` state.
    /// Returns the `Succ` value if the Fin is in a `Succ` state.
    /// </summary>
    /// <param name="Fail">Function to generate a `Succ` value if in the `Fail` state</param>
    /// <returns>Returns an unwrapped `Succ` value</returns>
    [Pure]
    public K<M, A> IfFail(Func<A> Fail) =>
        IfFail(_ => Fail());

    /// <summary>
    /// Executes the `f` function if the `Fin` is in a `Fail` state.
    /// Returns the `Succ` value if the `Fin` is in a `Succ` state.
    /// </summary>
    /// <param name="f">Function to generate a `Succ` value if in the `Fail` state</param>
    /// <returns>Returns an unwrapped `Succ` value</returns>
    [Pure]
    public K<M, A> IfFail(Func<Error, A> f) =>
        Match(Fail: f, Succ: identity);

    /// <summary>
    /// Returns the `value` if the `Fin` is in a `Fail` state.
    /// Returns the `Succ` value if the `Fin` is in a `Succ` state.
    /// </summary>
    /// <param name="value">Value to return if in the Fail state</param>
    /// <returns>Returns an unwrapped `Succ` value</returns>
    [Pure]
    public K<M, A> IfFail(A value) =>
        IfFail(_ => value);

    /// <summary>
    /// Executes the `Fail` action if the `Fin` is in a `Fail` state.
    /// </summary>
    /// <param name="Fail">Function to generate a `Succ` value if in the `Fail` state</param>
    /// <returns>Returns an unwrapped Succ value</returns>
    public K<M, Unit> IfFail(Action<Error> Fail) =>
        Match(Fail: Fail, Succ: _ => {});

    /// <summary>
    /// Invokes the `Succ` action if the `Fin` is in a `Succ` state, otherwise does nothing
    /// </summary>
    /// <param name="Succ">Action to invoke</param>
    /// <returns>Unit</returns>
    public K<M, Unit> IfSucc(Action<A> Succ) =>
        Match(Fail: _ => { }, Succ: Succ);

    /// <summary>
    /// Returns the `fail` value if the `Fin` is in a `Succ` state.
    /// Returns the `Fail` value if the `Fin` is in a `Fail` state.
    /// </summary>
    /// <param name="fail">Value to return if in the `Succ` state</param>
    /// <returns>Returns an unwrapped `Fail` value</returns>
    [Pure]
    public K<M, Error> IfSucc(Error fail) =>
        Match(Fail: identity, Succ: _ => fail);

    /// <summary>
    /// Returns the result of `Succ()` if the `Fin` is in a `Succ` state.
    /// Returns the `Fail` value if the `Fin` is in a `Fail` state.
    /// </summary>
    /// <param name="Succ">Function to generate a `Fail` value if in the `Succ` state</param>
    /// <returns>Returns an unwrapped Fail value</returns>
    [Pure]
    public K<M, Error> IfSucc(Func<Error> Succ) =>
        Match(Fail: identity, Succ: _ => Succ());

    /// <summary>
    /// Returns the result of `f` if the `Fin` is in a `Succ` state.
    /// Returns the `Fail` value if the `Fin` is in a `Fail` state.
    /// </summary>
    /// <param name="f">Function to generate a `Fail` value if in the `Succ` state</param>
    /// <returns>Returns an unwrapped Fail value</returns>
    [Pure]
    public K<M, Error> IfSucc(Func<A, Error> f) =>
        Match(Fail: identity, Succ: f);
    
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
    public FinT<M1, B> MapT<M1, B>(Func<K<M, Fin<A>>, K<M1, Fin<B>>> f)
        where M1 : Monad<M1> =>
        new (f(runFin));

    /// <summary>
    /// Maps the given monad
    /// </summary>
    /// <param name="f">Mapping function</param>
    public FinT<M, B> MapM<B>(Func<K<M, A>, K<M, B>> f) =>
        new(runFin
               .Bind(fv => fv switch
                           {
                               Fin<A>.Succ (var v) => f(M.Pure(v)).Map(Fin.Succ),
                               Fin<A>.Fail (var e) => M.Pure<Fin<B>>(e),
                               _                   => throw new NotSupportedException()
                           }));
    
    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`FinT`</returns>
    public FinT<M, B> Map<B>(Func<A, B> f) =>
        new(M.Map(mx => mx.Map(f), runFin));
    
    /// <summary>
    /// Maps the `Error` value
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`FinT`</returns>
    public FinT<M, A> MapFail(Func<Error, Error> f) =>
        new(M.Map(mx => mx.MapFail(f), runFin));
    
    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <param name="f">Mapping transducer</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`FinT`</returns>
    public FinT<M, B> Select<B>(Func<A, B> f) =>
        new(M.Map(mx => mx.Map(f), runFin));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Bind
    //

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`FinT`</returns>
    public FinT<M, B> Bind<B>(Func<A, K<FinT<M>, B>> f) =>
        Bind(x => f(x).As());

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`FinT`</returns>
    public FinT<M, B> Bind<B>(Func<A, Fin<B>> f) =>
        Bind(x => FinT.lift<M, B>(f(x)));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`FinT`</returns>
    public FinT<M, B> Bind<B>(Func<A, FinT<M, B>> f) =>
        new(M.Bind(runFin, 
                   ex => ex.Match(
                       Succ: x => f(x).runFin,
                       Fail: e => M.Pure(Fin.Fail<B>(e)))));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`FinT`</returns>
    public FinT<M, B> Bind<B>(Func<A, Pure<B>> f) =>
        Map(a => f(a).Value);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`FinT`</returns>
    public FinT<M, B> Bind<B>(Func<A, Fail<Error>> f) =>
        Bind(a => FinT.lift<M, B>(f(a).Value));

    /// <summary>
    /// Monad bi-bind operation
    /// </summary>
    /// <param name="Fail">Fail state mapping function</param>
    /// <param name="Succ">Fail state mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`FinT`</returns>
    public FinT<M, B> BiBind<B>(Func<Error, FinT<M, B>> Fail, Func<A, FinT<M, B>> Succ) =>
        new(M.Bind(runFin, 
                   ex => ex.Match(
                       Succ: x => Succ(x).runFin,
                       Fail: e => Fail(e).runFin)));

    /// <summary>
    /// Monad bi-bind operation
    /// </summary>
    /// <param name="Fail">Fail state mapping function</param>
    /// <returns>`FinT`</returns>
    public FinT<M, A> BindFail(Func<Error, FinT<M, A>> Fail) =>
        BiBind(Fail, FinT.Succ<M, A>);

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
    /// <returns>`FinT`</returns>
    public FinT<M, C> SelectMany<B, C>(Func<A, K<FinT<M>, B>> bind, Func<A, B, C> project) =>
        SelectMany(x => bind(x).As(), project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`FinT`</returns>
    public FinT<M, C> SelectMany<B, C>(Func<A, FinT<M, B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`FinT`</returns>
    public FinT<M, C> SelectMany<B, C>(Func<A, K<M, B>> bind, Func<A, B, C> project) =>
        SelectMany(x => FinT.lift(bind(x)), project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`FinT`</returns>
    public FinT<M, C> SelectMany<B, C>(Func<A, Fin<B>> bind, Func<A, B, C> project) =>
        SelectMany(x => FinT.lift<M, B>(bind(x)), project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`FinT`</returns>
    public FinT<M, C> SelectMany<B, C>(Func<A, Pure<B>> bind, Func<A, B, C> project) =>
        Map(x => project(x, bind(x).Value));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Operators
    //
    
    public static implicit operator FinT<M, A>(Fin<A> ma) =>
        FinT.lift<M, A>(ma);
    
    public static implicit operator FinT<M, A>(Pure<A> ma) =>
        FinT.Succ<M, A>(ma.Value);
    
    public static implicit operator FinT<M, A>(Fail<Error> ma) =>
        FinT.lift<M, A>(new Fin<A>.Fail(ma.Value));
    
    public static implicit operator FinT<M, A>(Error ma) =>
        FinT.Fail<M, A>(ma);
    
    public static implicit operator FinT<M, A>(IO<A> ma) =>
        FinT.liftIOMaybe<M, A>(ma);
    
    public static implicit operator FinT<M, A>(Lift<A> ma) =>
        FinT.liftIOMaybe<M, A>(ma);
    
    public static implicit operator FinT<M, A>(Lift<EnvIO, A> ma) =>
        FinT.liftIOMaybe<M, A>(ma);
    
    public static implicit operator FinT<M, A>(IO<Fin<A>> ma) =>
        FinT.liftIOMaybe<M, A>(ma);
    
    public OptionT<M, A> ToOption() =>
        new(runFin.Map(ma => ma.ToOption()));

    public EitherT<Error, M, A> ToEither() =>
        new(runFin.Map(ma => ma.ToEither()));

    public ValidationT<Error, M, A> ToValidation() =>
        new(_ => runFin.Map(ma => ma.ToValidation()));
}

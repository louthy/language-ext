using System;
using System.Diagnostics.Contracts;
using LanguageExt.Common;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

/// <summary>
/// `FinT` monad transformer, which allows for either an `Error` or `R` result value to be carried. 
/// </summary>
/// <param name="runFin">Transducer that represents the transformer operation</param>
/// <typeparam name="M">Given monad trait</typeparam>
/// <typeparam name="A">Bound value type</typeparam>
public record FinT<M, A>(K<M, Fin<A>> runFin) : 
    Fallible<FinT<M, A>, FinT<M>, Error, A>
    where M : Monad<M>
{
    /// <summary>
    /// Lift a pure value into the monad-transformer
    /// </summary>
    /// <param name="value">Value to lift</param>
    /// <returns>`FinT`</returns>
    public static FinT<M, A> Succ(A value) =>
        Lift(M.Pure(value));
    
    /// <summary>
    /// Lift a fail value into the monad-transformer
    /// </summary>
    /// <param name="value">Value to lift</param>
    /// <returns>`FinT`</returns>
    public static FinT<M, A> Fail(Error value) =>
        Lift(Fin<A>.Fail(value));

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
    
    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="pure">Monad to lift</param>
    /// <returns>`FinT`</returns>
    public static FinT<M, A> Lift(Pure<A> pure) =>
        Succ(pure.Value);

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="ma">Monad to lift</param>
    /// <returns>`FinT`</returns>
    public static FinT<M, A> Lift(Fin<A> ma) =>
        new(M.Pure(ma));

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="fail">Monad to lift</param>
    /// <returns>`FinT`</returns>
    public static FinT<M, A> Lift(Fail<Error> fail) =>
        Lift(Fin<A>.Fail(fail.Value));

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="fail">Fail value</param>
    /// <returns>`FinT`</returns>
    public static FinT<M, A> Lift(Error fail) =>
        Lift(Fin<A>.Fail(fail));

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="monad">Monad to lift</param>
    /// <returns>`FinT`</returns>
    public static FinT<M, A> Lift(K<M, A> monad) =>
        new(M.Map(Fin<A>.Succ, monad));

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="monad">Monad to lift</param>
    /// <returns>`FinT`</returns>
    public static FinT<M, A> Lift(K<M, Fin<A>> monad) =>
        new(monad);

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="monad">Monad to lift</param>
    /// <returns>`FinT`</returns>
    public static FinT<M, A> LiftIO(IO<A> monad) =>
        Lift(M.LiftIO(monad));

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="monad">Monad to lift</param>
    /// <returns>`FinT`</returns>
    public static FinT<M, A> LiftIO(IO<Fin<A>> monad) =>
        Lift(M.LiftIO(monad));

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
                               Fin.Succ<A> (var v) => f(M.Pure(v)).Map(Fin<B>.Succ),
                               Fin.Fail<A> (var e)  => M.Pure<Fin<B>>(e),
                               _                    => throw new NotSupportedException()
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
        Bind(x => FinT<M, B>.Lift(f(x)));

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
                       Fail: e => M.Pure(Fin<B>.Fail(e)))));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`FinT`</returns>
    public FinT<M, B> Bind<B>(Func<A, IO<B>> f) =>
        Bind(a => FinT<M, B>.LiftIO(f(a)));

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
        Bind(a => FinT<M, B>.Lift(f(a).Value));

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
        BiBind(Fail, Succ);

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
        SelectMany(x => FinT<M, B>.Lift(bind(x)), project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`FinT`</returns>
    public FinT<M, C> SelectMany<B, C>(Func<A, Fin<B>> bind, Func<A, B, C> project) =>
        SelectMany(x => FinT<M, B>.Lift(bind(x)), project);

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

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`FinT`</returns>
    public FinT<M, C> SelectMany<B, C>(Func<A, IO<B>> bind, Func<A, B, C> project) =>
        SelectMany(x => M.LiftIO(bind(x)), project);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Operators
    //

    /// <summary>
    /// Sequentially compose two actions, discarding any value produced by the first, like sequencing operators (such
    /// as the semicolon) in C#.
    /// </summary>
    /// <param name="lhs">First action to run</param>
    /// <param name="rhs">Second action to run</param>
    /// <returns>Result of the second action</returns>
    public static FinT<M, A> operator >> (FinT<M, A> lhs, FinT<M, A> rhs) =>
        lhs.Bind(_ => rhs);
    
    /// <summary>
    /// Sequentially compose two actions, discarding any value produced by the first, like sequencing operators (such
    /// as the semicolon) in C#.
    /// </summary>
    /// <param name="lhs">First action to run</param>
    /// <param name="rhs">Second action to run</param>
    /// <returns>Result of the second action</returns>
    public static FinT<M, A> operator >> (FinT<M, A> lhs, K<FinT<M>, A> rhs) =>
        lhs.Bind(_ => rhs);

    /// <summary>
    /// Sequentially compose two actions.  The second action is a unit returning action, so the result of the
    /// first action is propagated. 
    /// </summary>
    /// <param name="lhs">First action to run</param>
    /// <param name="rhs">Second action to run</param>
    /// <returns>Result of the first action</returns>
    public static FinT<M, A> operator >> (FinT<M, A> lhs, FinT<M, Unit> rhs) =>
        lhs.Bind(x => rhs.Map(_ => x));
    
    /// <summary>
    /// Sequentially compose two actions.  The second action is a unit returning action, so the result of the
    /// first action is propagated. 
    /// </summary>
    /// <param name="lhs">First action to run</param>
    /// <param name="rhs">Second action to run</param>
    /// <returns>Result of the first action</returns>
    public static FinT<M, A> operator >> (FinT<M, A> lhs, K<FinT<M>, Unit> rhs) =>
        lhs.Bind(x => rhs.Map(_ => x));
    
    public static implicit operator FinT<M, A>(Fin<A> ma) =>
        Lift(ma);
    
    public static implicit operator FinT<M, A>(Pure<A> ma) =>
        Succ(ma.Value);
    
    public static implicit operator FinT<M, A>(Fail<Error> ma) =>
        Lift(Fin<A>.Fail(ma.Value));
    
    public static implicit operator FinT<M, A>(Error ma) =>
        Lift(Fin<A>.Fail(ma));
    
    public static implicit operator FinT<M, A>(IO<A> ma) =>
        LiftIO(ma);
    
    public static implicit operator FinT<M, A>(Lift<A> ma) =>
        LiftIO(ma);
    
    public static implicit operator FinT<M, A>(Lift<EnvIO, A> ma) =>
        LiftIO(ma);
    
    public static implicit operator FinT<M, A>(IO<Fin<A>> ma) =>
        LiftIO(ma);

    public static FinT<M, A> operator +(FinT<M, A> lhs, FinT<M, A> rhs) =>
        lhs.Combine(rhs).As();

    public static FinT<M, A> operator +(K<FinT<M>, A> lhs, FinT<M, A> rhs) =>
        lhs.As().Combine(rhs).As();

    public static FinT<M, A> operator +(FinT<M, A> lhs, K<FinT<M>, A> rhs) =>
        lhs.Combine(rhs.As()).As();

    public static FinT<M, A> operator +(FinT<M, A> lhs, A rhs) => 
        lhs.Combine(pure<FinT<M>, A>(rhs)).As();

    public static FinT<M, A> operator +(FinT<M, A> ma, Pure<A> mb) =>
        ma.Combine(pure<FinT<M>, A>(mb.Value)).As();

    public static FinT<M, A> operator +(FinT<M, A> ma, Fail<Error> mb) =>
        ma.Combine(fail<Error, FinT<M>, A>(mb.Value)).As();

    public static FinT<M, A> operator +(FinT<M, A> ma, Fail<Exception> mb) =>
        ma.Combine(fail<Error, FinT<M>, A>(mb.Value)).As();
    
    public static FinT<M, A> operator |(FinT<M, A> lhs, FinT<M, A> rhs) =>
        lhs.Choose(rhs).As();

    public static FinT<M, A> operator |(K<FinT<M>, A> lhs, FinT<M, A> rhs) =>
        lhs.As().Choose(rhs).As();

    public static FinT<M, A> operator |(FinT<M, A> lhs, K<FinT<M>, A> rhs) =>
        lhs.Choose(rhs.As()).As();

    public static FinT<M, A> operator |(FinT<M, A> lhs, A rhs) => 
        lhs.Choose(pure<FinT<M>, A>(rhs)).As();

    public static FinT<M, A> operator |(FinT<M, A> ma, Pure<A> mb) =>
        ma.Choose(pure<FinT<M>, A>(mb.Value)).As();

    public static FinT<M, A> operator |(FinT<M, A> ma, Fail<Error> mb) =>
        ma.Choose(fail<Error, FinT<M>, A>(mb.Value)).As();

    public static FinT<M, A> operator |(FinT<M, A> ma, Fail<Exception> mb) =>
        ma.Choose(fail<Error, FinT<M>, A>(mb.Value)).As();

    public static FinT<M, A> operator |(FinT<M, A> ma, Error mb) =>
        ma.Choose(fail<Error, FinT<M>, A>(mb)).As();

    public static FinT<M, A> operator |(FinT<M, A> ma, CatchM<Error, FinT<M>, A> mb) =>
        (ma.Kind() | mb).As();
    
    public OptionT<M, A> ToOption() =>
        new(runFin.Map(ma => ma.ToOption()));

    public EitherT<Error, M, A> ToEither() =>
        new(runFin.Map(ma => ma.ToEither()));

    public ValidationT<Error, M, A> ToValidation() =>
        new(runFin.Map(ma => ma.ToValidation()));

    /*
    public StreamT<M, A> ToStream() =>
        from seq in StreamT<M, Seq<A>>.Lift(runFin.Map(ma => ma.IsSucc ? Seq((A)ma) : Seq<A>.Empty))
        from res in StreamT<M, A>.Lift(seq)
        select res;

    public StreamT<M, Error> FailToStream() =>
        from seq in StreamT<M, Seq<Error>>.Lift(runFin.Map(ma => ma.IsFail ? Seq((Error)ma) : Seq<Error>.Empty))
        from res in StreamT<M,Error>.Lift(seq)
        select res;*/
}

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
public record Try<A>(Func<Fin<A>> runTry) : K<Try, A>, Semigroup<Try<A>>
{
    /// <summary>
    /// Lift a pure value into the monad-transformer
    /// </summary>
    /// <param name="value">Value to lift</param>
    /// <returns>`TryT`</returns>
    public static Try<A> Succ(A value) =>
        new (() => value);
    
    /// <summary>
    /// Lift a fail value into the monad-transformer
    /// </summary>
    /// <param name="value">Value to lift</param>
    /// <returns>`TryT`</returns>
    public static Try<A> Fail(Error value) =>
        new (() => value);

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="pure">Monad to lift</param>
    /// <returns>`TryT`</returns>
    public static Try<A> Lift(Pure<A> pure) =>
        new (() => pure.Value);

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="either">Monad to lift</param>
    /// <returns>`TryT`</returns>
    public static Try<A> Lift(Fin<A> result) =>
        new (() => result);

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="either">Monad to lift</param>
    /// <returns>`TryT`</returns>
    public static Try<A> Lift(Func<Fin<A>> result) =>
        new(() =>
            {
                try
                {
                    return result();
                }
                catch (Exception e)
                {
                    return FinFail<A>(e);
                }
            });

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="either">Monad to lift</param>
    /// <returns>`TryT`</returns>
    public static Try<A> Lift(Func<A> result) =>
        new(() =>
            {
                try
                {
                    return result();
                }
                catch (Exception e)
                {
                    return FinFail<A>(e);
                }
            });

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="fail">Monad to lift</param>
    /// <returns>`TryT`</returns>
    public static Try<A> Lift(Fail<Error> fail) =>
        new (() => fail.Value);

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
    public B Match<B>(Func<A, B> Succ, Func<Error, B> Fail) =>
        this.Run().Match(Succ, Fail);

    /// <summary>
    /// Match the bound value and return a result (which gets packages back up inside the inner monad)
    /// </summary>
    /// <param name="Succ">Success branch</param>
    /// <param name="Fail">Fail branch</param>
    /// <returns>Inner monad with the result of the `Succ` or `Fail` branches</returns>
    public A IfFail(Func<Error, A> Fail) =>
        Match(identity, Fail);

    /// <summary>
    /// Match the bound value and return a result (which gets packages back up inside the inner monad)
    /// </summary>
    /// <param name="Succ">Success branch</param>
    /// <param name="Fail">Fail branch</param>
    /// <returns>Inner monad with the result of the `Succ` or `Fail` branches</returns>
    public Fin<A> IfFailM(Func<Error, Fin<A>> Fail) =>
        Match(FinSucc, Fail);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Map
    //

    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`TryT`</returns>
    public Try<B> Map<B>(Func<A, B> f) =>
        new(() => runTry().Map(f));
    
    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <param name="f">Mapping transducer</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`TryT`</returns>
    public Try<B> Select<B>(Func<A, B> f) =>
        new(() => runTry().Map(f));

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
    public Try<B> Bind<B>(Func<A, K<Try, B>> f) =>
        new(() =>
            {
                var r = runTry();
                return r.IsFail
                           ? Fin<B>.Fail((Error)r)
                           : f((A)r).As().runTry();
            });

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`TryT`</returns>
    public Try<B> Bind<B>(Func<A, Try<B>> f) =>
        new(() =>
            {
                var r = runTry();
                return r.IsFail
                           ? Fin<B>.Fail((Error)r)
                           : f((A)r).runTry();
            });

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`TryT`</returns>
    public Try<B> Bind<B>(Func<A, Pure<B>> f) =>
        Map(a => f(a).Value);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="Succ">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`TryT`</returns>
    public Try<B> BiBind<B>(Func<A, Try<B>> Succ, Func<Error, Try<B>> Fail) =>
        new(() =>
            {
                var r = runTry();
                return r.IsFail
                           ? Fail((Error)r).runTry()
                           : Succ((A)r).runTry();
            });

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="Succ">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`TryT`</returns>
    public Try<A> BindFail(Func<Error, Try<A>> Fail) =>
        BiBind(Succ, Fail);

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
    public Try<C> SelectMany<B, C>(Func<A, K<Try, B>> bind, Func<A, B, C> project) =>
        SelectMany(x => bind(x).As(), project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`TryT`</returns>
    public Try<C> SelectMany<B, C>(Func<A, Try<B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`TryT`</returns>
    public Try<C> SelectMany<B, C>(Func<A, Fin<B>> bind, Func<A, B, C> project) =>
        SelectMany(x => Try<B>.Lift(bind(x)), project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`TryT`</returns>
    public Try<C> SelectMany<B, C>(Func<A, Pure<B>> bind, Func<A, B, C> project) =>
        Map(x => project(x, bind(x).Value));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Conversion operators
    //

    public static implicit operator Try<A>(Pure<A> ma) =>
        Succ(ma.Value);
    
    public static implicit operator Try<A>(Error ma) =>
        Lift(Fin<A>.Fail(ma));
    
    public static implicit operator Try<A>(Fail<Error> ma) =>
        Lift(Fin<A>.Fail(ma.Value));
    
    public static implicit operator Try<A>(Fail<Exception> ma) =>
        Lift(Fin<A>.Fail(ma.Value));
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Error catching operators
    //
    
    public static Try<A> operator |(Try<A> lhs, Try<A> rhs) =>
        lhs.Combine(rhs);
    
    public static Try<A> operator |(Try<A> lhs, K<Try, A> rhs) =>
        lhs.Combine(rhs.As());
    
    public static Try<A> operator |(K<Try, A> lhs, Try<A> rhs) =>
        lhs.As().Combine(rhs);

    public static Try<A> operator |(Try<A> ma, Pure<A> mb) =>
        ma.Combine(mb);

    public static Try<A> operator |(Try<A> ma, A mb) =>
        ma | pure<Try, A>(mb);

    public static Try<A> operator |(Try<A> ma, Fail<Error> mb) =>
        ma.Combine(mb);

    public static Try<A> operator |(Try<A> ma, Error mb) =>
        ma.Combine(mb);

    public static Try<A> operator |(Try<A> ma, Fail<Exception> mb) =>
        ma.Combine(mb);

    public static Try<A> operator |(Try<A> ma, Exception mb) =>
        ma.Combine((Error)mb);

    public static Try<A> operator |(Try<A> ma, CatchM<Error, Try, A> mb) =>
        new(() => ma.Run() switch
                  {
                      Fin.Fail<A> (var err) when mb.Match(err) => mb.Value(err).Run(),
                      var fa                                   => fa
                  });

    public Try<A> Combine(Try<A> rhs) =>
        new(() => this.Run() switch
                  {
                      Fin.Fail<A> => rhs.Run(),
                      var fa      => fa
                  });
}

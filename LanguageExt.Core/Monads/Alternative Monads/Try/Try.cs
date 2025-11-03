using System;
using System.Diagnostics.Contracts;
using LanguageExt.Common;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

/// <summary>
/// `Try` monad which allows for an optional `Error` result and catches exceptions, converting them to `Error`. 
/// </summary>
/// <typeparam name="M">Given monad trait</typeparam>
/// <typeparam name="A">Bound value type</typeparam>
public record Try<A>(Func<Fin<A>> runTry) : 
    Fallible<Try<A>, Try, Error, A>
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
                    return Fin.Fail<A>(e);
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
                    return Fin.Fail<A>(e);
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
        Match(Fin.Succ, Fail);

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
    /// <param name="f">Mapping function</param>
    /// <returns>`TryT`</returns>
    public Try<A> MapFail(Func<Error, Error> f) =>
        this.Catch(f).As();
    
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
                           ? Fin.Fail<B>((Error)r)
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
                           ? Fin.Fail<B>((Error)r)
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
        Bind(Succ).Catch(Fail).As();

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="Succ">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`TryT`</returns>
    public Try<A> BindFail(Func<Error, Try<A>> Fail) =>
        this.Catch(Fail).As();

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
        Lift(Fin.Fail<A>(ma));
    
    public static implicit operator Try<A>(Fail<Error> ma) =>
        Lift(Fin.Fail<A>(ma.Value));
    
    public static implicit operator Try<A>(Fail<Exception> ma) =>
        Lift(Fin.Fail<A>(ma.Value));

    public Option<A> ToOption() =>
        this.Run().ToOption(); 

    public Either<Error, A> ToEither() =>
        this.Run().ToEither(); 

    public Fin<A> ToFin() =>
        this.Run();

    public IO<A> ToIO() => 
        IO.lift(runTry);
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Error catching operators
    //
    
    public static Try<A> operator +(Try<A> lhs, Try<A> rhs) =>
        lhs.Combine(rhs);
    
    public static Try<A> operator +(Try<A> lhs, K<Try, A> rhs) =>
        lhs.Combine(rhs.As());
    
    public static Try<A> operator +(K<Try, A> lhs, Try<A> rhs) =>
        lhs.As().Combine(rhs);

    public static Try<A> operator +(Try<A> ma, Pure<A> mb) =>
        ma.Combine(mb);

    public static Try<A> operator +(Try<A> ma, Fail<Error> mb) =>
        ma.Combine(mb);

    public static Try<A> operator +(Try<A> ma, Fail<Exception> mb) =>
        ma.Combine(mb);

    public static Try<A> operator +(Try<A> ma, Exception mb) =>
        ma.Combine((Error)mb);
    
    public static Try<A> operator |(Try<A> lhs, Try<A> rhs) =>
        lhs.Choose(rhs).As();
    
    public static Try<A> operator |(Try<A> lhs, K<Try, A> rhs) =>
        lhs.Choose(rhs.As()).As();
    
    public static Try<A> operator |(K<Try, A> lhs, Try<A> rhs) =>
        lhs.As().Choose(rhs).As();

    public static Try<A> operator |(Try<A> ma, Pure<A> mb) =>
        ma.Choose(Succ(mb.Value)).As();

    public static Try<A> operator |(Try<A> ma, Fail<Error> mb) =>
        ma.Choose(Fail(mb.Value)).As();

    public static Try<A> operator |(Try<A> ma, Fail<Exception> mb) =>
        ma.Choose(Fail(mb.Value)).As();

    public static Try<A> operator |(Try<A> ma, Exception mb) =>
        ma.Choose(Fail(mb)).As();

    public static Try<A> operator |(Try<A> ma, CatchM<Error, Try, A> mb) =>
        (ma.Kind() | mb).As();

    [Pure]
    public Try<A> Combine(Try<A> rhs) =>
        new(() => this.Run() switch
                  {
                      Fin<A>.Fail fa => fa.Combine(rhs.Run()).As(),
                      var fa         => fa
                  });
}

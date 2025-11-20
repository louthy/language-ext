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
    K<Try, A>
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
        SelectMany(x => Try.lift(bind(x)), project);

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
        Try.Succ(ma.Value);
    
    public static implicit operator Try<A>(Error ma) =>
        Try.lift(Fin.Fail<A>(ma));
    
    public static implicit operator Try<A>(Fail<Error> ma) =>
        Try.lift(Fin.Fail<A>(ma.Value));
    
    public static implicit operator Try<A>(Fail<Exception> ma) =>
        Try.lift(Fin.Fail<A>(ma.Value));

    public Option<A> ToOption() =>
        this.Run().ToOption(); 

    public Either<Error, A> ToEither() =>
        this.Run().ToEither(); 

    public Fin<A> ToFin() =>
        this.Run();

    public IO<A> ToIO() => 
        IO.lift(runTry);

    [Pure]
    public Try<A> Combine(Try<A> rhs) =>
        new(() => this.Run() switch
                  {
                      Fin<A>.Fail fa => fa.Combine(rhs.Run()).As(),
                      var fa         => fa
                  });
}

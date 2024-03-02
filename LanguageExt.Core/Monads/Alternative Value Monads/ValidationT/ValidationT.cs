using System;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// `ValidationT` monad transformer, which allows for an optional result. 
/// </summary>
/// <param name="runValidation">Transducer that represents the transformer operation</param>
/// <typeparam name="M">Given monad trait</typeparam>
/// <typeparam name="L">Left value type</typeparam>
/// <typeparam name="A">Bound value type</typeparam>
public record ValidationT<L, M, A>(K<M, Validation<L, A>> runValidation) : K<ValidationT<L, M>, A>
    where M : Monad<M>
    where L : Monoid<L>
{
    /// <summary>
    /// Lift a pure value into the monad-transformer
    /// </summary>
    /// <param name="value">Value to lift</param>
    /// <returns>`ValidationT`</returns>
    public static ValidationT<L, M, A> Success(A value) =>
        Lift(M.Pure(value));
    
    /// <summary>
    /// Lift a fail value into the monad-transformer
    /// </summary>
    /// <param name="value">Value to lift</param>
    /// <returns>`ValidationT`</returns>
    public static ValidationT<L, M, A> Fail(L value) =>
        Lift(Validation<L, A>.Fail(value));

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="pure">Monad to lift</param>
    /// <returns>`ValidationT`</returns>
    public static ValidationT<L, M, A> Lift(Pure<A> pure) =>
        Success(pure.Value);

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="Validation">Monad to lift</param>
    /// <returns>`ValidationT`</returns>
    public static ValidationT<L, M, A> Lift(Validation<L, A> Validation) =>
        new(M.Pure(Validation));

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="fail">Monad to lift</param>
    /// <returns>`ValidationT`</returns>
    public static ValidationT<L, M, A> Lift(Fail<L> fail) =>
        Lift(Validation<L, A>.Fail(fail.Value));

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="monad">Monad to lift</param>
    /// <returns>`ValidationT`</returns>
    public static ValidationT<L, M, A> Lift(K<M, A> monad) =>
        new(M.Map(Validation<L, A>.Success, monad));

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="monad">Monad to lift</param>
    /// <returns>`ValidationT`</returns>
    public static ValidationT<L, M, A> LiftIO(IO<A> monad) =>
        Lift(M.LiftIO(monad));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Match
    //

    public K<M, B> Match<B>(Func<A, B> Succ, Func<L, B> Fail) =>
        M.Map(mx => mx.Match(Succ, Fail), runValidation);
 
    public K<M, Validation<L, A>> Run() =>
        runValidation;
 
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
    public ValidationT<L, M1, B> MapT<M1, B>(Func<K<M, Validation<L, A>>, K<M1, Validation<L, B>>> f)
        where M1 : Monad<M1> =>
        new (f(runValidation));

    /// <summary>
    /// Maps the given monad
    /// </summary>
    /// <param name="f">Mapping function</param>
    public ValidationT<L, M, B> MapM<B>(Func<K<M, A>, K<M, B>> f) =>
        new(runValidation
               .Bind(fv => fv switch
                           {
                               Validation.Success<L, A> (var v) => f(M.Pure(v)).Map(Validation<L, B>.Success),
                               Validation.Fail<L, A> (var e)    => M.Pure<Validation<L, B>>(e),
                               _                                => throw new NotSupportedException()
                           }));
    
    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`ValidationT`</returns>
    public ValidationT<L, M, B> Map<B>(Func<A, B> f) =>
        new(M.Map(mx => mx.Map(f), runValidation));
    
    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <param name="f">Mapping transducer</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`ValidationT`</returns>
    public ValidationT<L, M, B> Select<B>(Func<A, B> f) =>
        new(M.Map(mx => mx.Map(f), runValidation));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Bind
    //

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`ValidationT`</returns>
    public ValidationT<L, M, B> Bind<B>(Func<A, K<ValidationT<L, M>, B>> f) =>
        Bind(x => f(x).As());

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`ValidationT`</returns>
    public ValidationT<L, M, B> Bind<B>(Func<A, ValidationT<L, M, B>> f) =>
        new(M.Bind(runValidation,
                   ea => ea.IsSuccess switch
                         {
                             true  => f(ea.SuccessValue).runValidation,
                             false => M.Pure(Validation<L, B>.Fail(ea.FailValue))
                         }));        

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`ValidationT`</returns>
    public ValidationT<L, M, B> Bind<B>(Func<A, IO<B>> f) =>
        Bind(a => ValidationT<L, M, B>.LiftIO(f(a)));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`ValidationT`</returns>
    public ValidationT<L, M, B> Bind<B>(Func<A, Pure<B>> f) =>
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
    /// <returns>`ValidationT`</returns>
    public ValidationT<L, M, C> SelectMany<B, C>(Func<A, K<ValidationT<L, M>, B>> bind, Func<A, B, C> project) =>
        SelectMany(x => bind(x).As(), project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`ValidationT`</returns>
    public ValidationT<L, M, C> SelectMany<B, C>(Func<A, ValidationT<L, M, B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`ValidationT`</returns>
    public ValidationT<L, M, C> SelectMany<B, C>(Func<A, K<M, B>> bind, Func<A, B, C> project) =>
        SelectMany(x => ValidationT<L, M, B>.Lift(bind(x)), project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`ValidationT`</returns>
    public ValidationT<L, M, C> SelectMany<B, C>(Func<A, Validation<L, B>> bind, Func<A, B, C> project) =>
        SelectMany(x => ValidationT<L, M, B>.Lift(bind(x)), project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`ValidationT`</returns>
    public ValidationT<L, M, C> SelectMany<B, C>(Func<A, Pure<B>> bind, Func<A, B, C> project) =>
        Map(x => project(x, bind(x).Value));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`ValidationT`</returns>
    public ValidationT<L, M, C> SelectMany<B, C>(Func<A, IO<B>> bind, Func<A, B, C> project) =>
        SelectMany(x => M.LiftIO(bind(x)), project);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Operators
    //

    public static implicit operator ValidationT<L, M, A>(Pure<A> ma) =>
        Success(ma.Value);
    
    public static implicit operator ValidationT<L, M, A>(Fail<L> ma) =>
        Fail(ma.Value);
    
    public static implicit operator ValidationT<L, M, A>(IO<A> ma) =>
        LiftIO(ma);

    public static ValidationT<L, M, A> operator |(ValidationT<L, M, A> ma, ValidationT<L, M, A> mb) =>
        new(M.Bind(ma.runValidation, ea => M.Map(eb => ea | eb, mb.runValidation)));

    public static ValidationT<L, M, Seq<A>> operator &(ValidationT<L, M, A> ma, ValidationT<L, M, A> mb) =>
        new(M.Bind(ma.runValidation, ea => M.Map(eb => ea & eb, mb.runValidation)));
    
}

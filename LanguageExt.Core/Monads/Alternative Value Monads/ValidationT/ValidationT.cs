using System;
using LanguageExt.Traits;
using LanguageExt.TypeClasses;

namespace LanguageExt;

/// <summary>
/// `ValidationT` monad transformer, which allows for an optional result. 
/// </summary>
/// <param name="runValidation">Transducer that represents the transformer operation</param>
/// <typeparam name="M">Given monad trait</typeparam>
/// <typeparam name="L">Left value type</typeparam>
/// <typeparam name="A">Bound value type</typeparam>
public record ValidationT<L, M, A>(K<M, Either<L, A>> runValidation) : K<ValidationT<L, M>, A>
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
        Lift(Either<L, A>.Left(value));

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
    public static ValidationT<L, M, A> Lift(Either<L, A> Validation) =>
        new(M.Pure(Validation));

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="fail">Monad to lift</param>
    /// <returns>`ValidationT`</returns>
    public static ValidationT<L, M, A> Lift(Fail<L> fail) =>
        Lift(Either<L, A>.Left(fail.Value));

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="monad">Monad to lift</param>
    /// <returns>`ValidationT`</returns>
    public static ValidationT<L, M, A> Lift(K<M, A> monad) =>
        new(M.Map(Either<L, A>.Right, monad));

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

    public K<M, B> Match<B>(Func<L, B> Left, Func<A, B> Right) =>
        M.Map(mx => mx.Match(Left: Left, Right: Right), runValidation);
 
    public K<M, Either<L, A>> Run() =>
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
    public ValidationT<L, M1, B> MapT<M1, B>(Func<K<M, Either<L, A>>, K<M1, Either<L, B>>> f)
        where M1 : Monad<M1> =>
        new (f(runValidation));
    
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
                   ea => ea.State switch
                         {
                             EitherStatus.IsRight =>
                                 f(ea.RightValue).runValidation,

                             EitherStatus.IsLeft =>
                                 M.Pure(Either<L, B>.Left(ea.LeftValue)),

                             _ => M.Pure(default(Either<L, B>))
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
    public ValidationT<L, M, C> SelectMany<B, C>(Func<A, Either<L, B>> bind, Func<A, B, C> project) =>
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
        new(M.Bind(ma.runValidation,
                   ea => ea.State switch
                         {
                             EitherStatus.IsRight => M.Pure(ea),
                             EitherStatus.IsLeft =>
                                 M.Bind(mb.runValidation,
                                        eb => eb.State switch
                                              {
                                                  EitherStatus.IsRight => M.Pure(eb),
                                                  EitherStatus.IsLeft => M.Pure(
                                                      Either<L, A>.Left(ea.LeftValue.Append(eb.LeftValue))),
                                                  _ => M.Pure(ea)
                                              }),
                             _ => M.Pure(ea)
                         }));
}

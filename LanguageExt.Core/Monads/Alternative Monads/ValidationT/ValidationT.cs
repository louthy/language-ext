using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// `ValidationT` monad transformer, which allows for an optional result. 
/// </summary>
/// <param name="runValidation">Transducer that represents the transformer operation</param>
/// <typeparam name="M">Given monad trait</typeparam>
/// <typeparam name="F">Left value type</typeparam>
/// <typeparam name="A">Bound value type</typeparam>
public record ValidationT<F, M, A>(K<M, Validation<F, A>> runValidation) : 
    Fallible<ValidationT<F, M, A>, ValidationT<F, M>, F, A>
    where M : Monad<M>
    where F : Monoid<F>
{
    /// <summary>
    /// Lift a pure value into the monad-transformer
    /// </summary>
    /// <param name="value">Value to lift</param>
    /// <returns>`ValidationT`</returns>
    public static ValidationT<F, M, A> Success(A value) =>
        Lift(M.Pure(value));
    
    /// <summary>
    /// Lift a fail value into the monad-transformer
    /// </summary>
    /// <param name="value">Value to lift</param>
    /// <returns>`ValidationT`</returns>
    public static ValidationT<F, M, A> Fail(F value) =>
        Lift(Validation<F, A>.Fail(value));

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="pure">Monad to lift</param>
    /// <returns>`ValidationT`</returns>
    public static ValidationT<F, M, A> Lift(Pure<A> pure) =>
        Success(pure.Value);

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="Validation">Monad to lift</param>
    /// <returns>`ValidationT`</returns>
    public static ValidationT<F, M, A> Lift(Validation<F, A> Validation) =>
        new(M.Pure(Validation));

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="fail">Monad to lift</param>
    /// <returns>`ValidationT`</returns>
    public static ValidationT<F, M, A> Lift(Fail<F> fail) =>
        Lift(Validation<F, A>.Fail(fail.Value));

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="monad">Monad to lift</param>
    /// <returns>`ValidationT`</returns>
    public static ValidationT<F, M, A> Lift(K<M, A> monad) =>
        new(M.Map(Validation<F, A>.Success, monad));

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="monad">Monad to lift</param>
    /// <returns>`ValidationT`</returns>
    public static ValidationT<F, M, A> LiftIO(IO<A> monad) =>
        Lift(M.LiftIO(monad));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Match
    //

    public K<M, B> Match<B>(Func<A, B> Succ, Func<F, B> Fail) =>
        M.Map(mx => mx.Match(Succ, Fail), runValidation);
 
    public K<M, Validation<F, A>> Run() =>
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
    public ValidationT<F, M1, B> MapT<M1, B>(Func<K<M, Validation<F, A>>, K<M1, Validation<F, B>>> f)
        where M1 : Monad<M1> =>
        new (f(runValidation));

    /// <summary>
    /// Maps the given monad
    /// </summary>
    /// <param name="f">Mapping function</param>
    public ValidationT<F, M, B> MapM<B>(Func<K<M, A>, K<M, B>> f) =>
        new(runValidation
               .Bind(fv => fv switch
                           {
                               Validation.Success<F, A> (var v) => f(M.Pure(v)).Map(Validation<F, B>.Success),
                               Validation.Fail<F, A> (var e)    => M.Pure<Validation<F, B>>(e),
                               _                                => throw new NotSupportedException()
                           }));
    
    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`ValidationT`</returns>
    public ValidationT<F, M, B> Map<B>(Func<A, B> f) =>
        new(M.Map(mx => mx.Map(f), runValidation));
    
    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`ValidationT`</returns>
    public ValidationT<F1, M, A> MapFail<F1>(Func<F, F1> f) 
        where F1 : Monoid<F1> =>
        new(M.Map(mx => mx.MapFail(f), runValidation));
    
    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <param name="f">Mapping transducer</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`ValidationT`</returns>
    public ValidationT<F, M, B> Select<B>(Func<A, B> f) =>
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
    public ValidationT<F, M, B> Bind<B>(Func<A, K<ValidationT<F, M>, B>> f) =>
        Bind(x => f(x).As());

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`ValidationT`</returns>
    public ValidationT<F, M, B> Bind<B>(Func<A, ValidationT<F, M, B>> f) =>
        new(M.Bind(runValidation,
                   ea => ea.IsSuccess switch
                         {
                             true  => f(ea.SuccessValue).runValidation,
                             false => M.Pure(Validation<F, B>.Fail(ea.FailValue))
                         }));    

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="Succ">Success mapping function</param>
    /// <param name="Fail">Failure mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`ValidationT`</returns>
    public ValidationT<F, M, B> BiBind<B>(Func<A, ValidationT<F, M, B>> Succ, Func<F, ValidationT<F, M, B>> Fail) =>
        new(M.Bind(runValidation,
                   ea => ea.IsSuccess switch
                         {
                             true  => Succ(ea.SuccessValue).runValidation,
                             false => Fail(ea.FailValue).runValidation
                         }));

    /// <summary>
    /// Failure bind operation
    /// </summary>
    /// <param name="Fail">Failure mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`ValidationT`</returns>
    public ValidationT<F, M, A> BindFail(Func<F, ValidationT<F, M, A>> Fail) =>
        BiBind(Success, Fail);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`ValidationT`</returns>
    public ValidationT<F, M, B> Bind<B>(Func<A, IO<B>> f) =>
        Bind(a => ValidationT<F, M, B>.LiftIO(f(a)));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`ValidationT`</returns>
    public ValidationT<F, M, B> Bind<B>(Func<A, Pure<B>> f) =>
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
    public ValidationT<F, M, C> SelectMany<B, C>(Func<A, K<ValidationT<F, M>, B>> bind, Func<A, B, C> project) =>
        SelectMany(x => bind(x).As(), project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`ValidationT`</returns>
    public ValidationT<F, M, C> SelectMany<B, C>(Func<A, ValidationT<F, M, B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`ValidationT`</returns>
    public ValidationT<F, M, C> SelectMany<B, C>(Func<A, K<M, B>> bind, Func<A, B, C> project) =>
        SelectMany(x => ValidationT<F, M, B>.Lift(bind(x)), project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`ValidationT`</returns>
    public ValidationT<F, M, C> SelectMany<B, C>(Func<A, Validation<F, B>> bind, Func<A, B, C> project) =>
        SelectMany(x => ValidationT<F, M, B>.Lift(bind(x)), project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`ValidationT`</returns>
    public ValidationT<F, M, C> SelectMany<B, C>(Func<A, Pure<B>> bind, Func<A, B, C> project) =>
        Map(x => project(x, bind(x).Value));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`ValidationT`</returns>
    public ValidationT<F, M, C> SelectMany<B, C>(Func<A, IO<B>> bind, Func<A, B, C> project) =>
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
    public static ValidationT<F, M, A> operator >> (ValidationT<F, M, A> lhs, ValidationT<F, M, A> rhs) =>
        lhs.Bind(_ => rhs);
    
    /// <summary>
    /// Sequentially compose two actions, discarding any value produced by the first, like sequencing operators (such
    /// as the semicolon) in C#.
    /// </summary>
    /// <param name="lhs">First action to run</param>
    /// <param name="rhs">Second action to run</param>
    /// <returns>Result of the second action</returns>
    public static ValidationT<F, M, A> operator >> (ValidationT<F, M, A> lhs, K<ValidationT<F, M>, A> rhs) =>
        lhs.Bind(_ => rhs);

    /// <summary>
    /// Sequentially compose two actions.  The second action is a unit returning action, so the result of the
    /// first action is propagated. 
    /// </summary>
    /// <param name="lhs">First action to run</param>
    /// <param name="rhs">Second action to run</param>
    /// <returns>Result of the first action</returns>
    public static ValidationT<F, M, A> operator >> (ValidationT<F, M, A> lhs, ValidationT<F, M, Unit> rhs) =>
        lhs.Bind(x => rhs.Map(_ => x));
    
    /// <summary>
    /// Sequentially compose two actions.  The second action is a unit returning action, so the result of the
    /// first action is propagated. 
    /// </summary>
    /// <param name="lhs">First action to run</param>
    /// <param name="rhs">Second action to run</param>
    /// <returns>Result of the first action</returns>
    public static ValidationT<F, M, A> operator >> (ValidationT<F, M, A> lhs, K<ValidationT<F, M>, Unit> rhs) =>
        lhs.Bind(x => rhs.Map(_ => x));

    public static implicit operator ValidationT<F, M, A>(Pure<A> ma) =>
        Success(ma.Value);
    
    public static implicit operator ValidationT<F, M, A>(Fail<F> ma) =>
        Fail(ma.Value);

    public static implicit operator ValidationT<F, M, A>(F fail) => 
        Fail(fail);

    public static implicit operator ValidationT<F, M, A>(IO<A> ma) =>
        LiftIO(ma);

    [Pure, MethodImpl(Opt.Default)]
    public static ValidationT<F, M, A> operator +(ValidationT<F, M, A> lhs, ValidationT<F, M, A> rhs) =>
        lhs.Combine(rhs).As();

    [Pure, MethodImpl(Opt.Default)]
    public static ValidationT<F, M, A> operator +(K<ValidationT<F, M>, A> lhs, ValidationT<F, M, A> rhs) =>
        lhs.Combine(rhs).As();

    [Pure, MethodImpl(Opt.Default)]
    public static ValidationT<F, M, A> operator +(ValidationT<F, M, A> lhs, K<ValidationT<F, M>, A> rhs) =>
        lhs.Combine(rhs).As();

    [Pure, MethodImpl(Opt.Default)]
    public static ValidationT<F, M, A> operator +(ValidationT<F, M, A> lhs, Pure<A> rhs) =>
        lhs.Combine(Success(rhs.Value)).As();

    [Pure, MethodImpl(Opt.Default)]
    public static ValidationT<F, M, A> operator +(ValidationT<F, M, A> lhs, Fail<F> rhs) =>
        lhs.Combine(Fail(rhs.Value)).As();
    
    [Pure, MethodImpl(Opt.Default)]
    public static ValidationT<F, M, A> operator |(ValidationT<F, M, A> lhs, ValidationT<F, M, A> rhs) =>
        lhs.Choose(rhs).As();

    [Pure, MethodImpl(Opt.Default)]
    public static ValidationT<F, M, A> operator |(K<ValidationT<F, M>, A> lhs, ValidationT<F, M, A> rhs) =>
        lhs.Choose(rhs).As();

    [Pure, MethodImpl(Opt.Default)]
    public static ValidationT<F, M, A> operator |(ValidationT<F, M, A> lhs, K<ValidationT<F, M>, A> rhs) =>
        lhs.Choose(rhs).As();

    [Pure, MethodImpl(Opt.Default)]
    public static ValidationT<F, M, A> operator |(ValidationT<F, M, A> lhs, Pure<A> rhs) =>
        lhs.Choose(Success(rhs.Value)).As();

    [Pure, MethodImpl(Opt.Default)]
    public static ValidationT<F, M, A> operator |(ValidationT<F, M, A> lhs, Fail<F> rhs) =>
        lhs.Choose(Fail(rhs.Value)).As();

    [Pure, MethodImpl(Opt.Default)]
    public static ValidationT<F, M, A> operator |(ValidationT<F, M, A> lhs, CatchM<F, ValidationT<F, M>, A> rhs) =>
        lhs.Catch(rhs).As();

    public static ValidationT<F, M, Seq<A>> operator &(ValidationT<F, M, A> ma, ValidationT<F, M, A> mb) =>
        new(M.Bind(ma.runValidation, ea => M.Map(eb => ea & eb, mb.runValidation)));

    /*
    public StreamT<M, A> ToStream() =>
        from seq in StreamT<M, Seq<A>>.Lift(runValidation.Map(ma => ma.IsSuccess ? Prelude.Seq((A)ma) : Seq<A>.Empty))
        from res in StreamT<M, A>.Lift(seq)
        select res;

    public StreamT<M, F> FailToStream() =>
        from seq in StreamT<M, Seq<F>>.Lift(runValidation.Map(ma => ma.IsSuccess ? Prelude.Seq((F)ma) : Seq<F>.Empty))
        from res in StreamT<M, F>.Lift(seq)
        select res;
        */
    
}

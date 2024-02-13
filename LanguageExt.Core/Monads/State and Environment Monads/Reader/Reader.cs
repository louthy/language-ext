using System;
using LanguageExt.HKT;

namespace LanguageExt;

/// <summary>
/// Reader monad
/// </summary>
/// <remarks>
/// This is a composition of the `Reader` monad transformer and the `Identity` monad
/// </remarks>
/// <param name="runReader">Transducer that is the reader operation</param>
/// <typeparam name="Env">Reader environment type</typeparam>
/// <typeparam name="A">Bound value type</typeparam>
public record Reader<Env, A>(Func<Env, K<Identity, A>> runReader)
    : ReaderT<Env, Identity, A>(runReader)
{
    /// <summary>
    /// Lift a pure value into the monad-transformer
    /// </summary>
    /// <param name="value">Value to lift</param>
    /// <returns>`Reader`</returns>
    public new static Reader<Env, A> Pure(A value) =>
        ReaderT<Env, Identity, A>.Pure(value).As();

    /// <summary>
    /// Extracts the environment value and maps it to the bound value
    /// </summary>
    /// <param name="f">Environment mapping function</param>
    /// <returns>`Reader`</returns>
    public new static Reader<Env, A> Asks(Func<Env, A> f) =>
        ReaderT<Env, Identity, A>.Asks(f).As();
    
    /// <summary>
    /// Lifts a unit function into the transformer 
    /// </summary>
    /// <param name="t">Transformer to lift</param>
    /// <returns>`Reader`</returns>
    public new static Reader<Env, A> Lift(Func<A> t) =>
        ReaderT<Env, Identity, A>.Lift(t).As();

    /// <summary>
    /// Maps the Reader's environment value
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <returns>`Reader`</returns>
    public new Reader<Env1, A> With<Env1>(Func<Env1, Env> f) =>
        base.With(f).As();

    /// <summary>
    /// Maps the Reader's environment value
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <returns>`Reader`</returns>
    public new Reader<Env, A> Local(Func<Env, Env> f) =>
        base.Local(f).As();

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Map
    //

    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`Reader`</returns>
    public new Reader<Env, B> Map<B>(Func<A, B> f) =>
        base.Map(f).As();
    
    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <param name="f">Mapping transducer</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`Reader`</returns>
    public new Reader<Env, B> Select<B>(Func<A, B> f) =>
        Map(f);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Bind
    //

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`Reader`</returns>
    public new Reader<Env, B> Bind<B>(Func<A, K<ReaderT<Env, Identity>, B>> f) =>
        base.Bind(f).As();

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`Reader`</returns>
    public Reader<Env, B> Bind<B>(Func<A, Reader<Env, B>> f) =>
        base.Bind(f).As();
    
    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`Reader`</returns>
    public new Reader<Env, B> Bind<B>(Func<A, Ask<Env, B>> f) =>
        base.Bind(f).As();

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
    /// <returns>`Reader`</returns>
    public new Reader<Env, C> SelectMany<B, C>(Func<A, K<ReaderT<Env, Identity>, B>> bind, Func<A, B, C> project) =>
        (Reader<Env, C>)base.SelectMany(bind, project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`Reader`</returns>
    public Reader<Env, C> SelectMany<B, C>(Func<A, Reader<Env, B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`Reader`</returns>
    public new Reader<Env, C> SelectMany<B, C>(Func<A, K<Identity, B>> bind, Func<A, B, C> project) =>
        base.SelectMany(bind, project).As();

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`Reader`</returns>
    public new Reader<Env, C> SelectMany<B, C>(Func<A, Pure<B>> bind, Func<A, B, C> project) =>
        base.SelectMany(bind, project).As();
    
    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`Reader`</returns>
    public new Reader<Env, C> SelectMany<B, C>(Func<A, Ask<Env, B>> bind, Func<A, B, C> project) =>
        base.SelectMany(bind, project).As();

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Conversion operators
    //

    public static implicit operator Reader<Env, A>(Pure<A> ma) =>
        (Reader<Env, A>)(ReaderT<Env, Identity, A>)ma;
    
    public static implicit operator Reader<Env, A>(Ask<Env, A> ma) =>
        (Reader<Env, A>)(ReaderT<Env, Identity, A>)ma;
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Run the reader
    //

    /// <summary>
    /// Run the reader monad 
    /// </summary>
    /// <param name="env">Input environment</param>
    /// <returns>Bound value</returns>
    public new A Run(Env env) =>
        runReader(env).As().Value;
}

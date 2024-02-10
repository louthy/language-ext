using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.HKT;
using static LanguageExt.Prelude;

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
public record Reader<Env, A>(Transducer<Env, Monad<Identity, A>> runReader)
    : ReaderT<Env, Identity, A>(runReader)
{
    /// <summary>
    /// Lift a pure value into the monad-transformer
    /// </summary>
    /// <param name="value">Value to lift</param>
    /// <returns>`Reader`</returns>
    public new static Reader<Env, A> Pure(A value) =>
        (Reader<Env, A>)ReaderT<Env, Identity, A>.Pure(value);

    /// <summary>
    /// Extracts the environment value and maps it to the bound value
    /// </summary>
    /// <param name="f">Environment mapping function</param>
    /// <returns>`Reader`</returns>
    public new static Reader<Env, A> Asks(Func<Env, A> f) =>
        (Reader<Env, A>)ReaderT<Env, Identity, A>.Asks(f);

    /// <summary>
    /// Extracts the environment value and maps it to the bound value
    /// </summary>
    /// <param name="f">Environment mapping transducer</param>
    /// <returns>`Reader`</returns>
    public new static Reader<Env, A> Asks(Transducer<Env, A> f) =>
        (Reader<Env, A>)ReaderT<Env, Identity, A>.Asks(f);

    /// <summary>
    /// Lifts a unit transducer into the transformer 
    /// </summary>
    /// <param name="t">Transformer to lift</param>
    /// <returns>`Reader`</returns>
    public new static Reader<Env, A> Lift(Transducer<Unit, A> t) =>
        (Reader<Env, A>)ReaderT<Env, Identity, A>.Lift(t);
    
    /// <summary>
    /// Lifts a unit function into the transformer 
    /// </summary>
    /// <param name="t">Transformer to lift</param>
    /// <returns>`Reader`</returns>
    public new static Reader<Env, A> Lift(Func<Unit, A> t) =>
        (Reader<Env, A>)ReaderT<Env, Identity, A>.Lift(t);
    
    /// <summary>
    /// Lifts a environment transducer into the transformer 
    /// </summary>
    /// <param name="t">Transformer to lift</param>
    /// <returns>`Reader`</returns>
    public new static Reader<Env, A> Lift(Transducer<Env, A> t) =>
        (Reader<Env, A>)ReaderT<Env, Identity, A>.Lift(t);
    
    /// <summary>
    /// Lifts a environment function into the transformer 
    /// </summary>
    /// <param name="t">Transformer to lift</param>
    /// <returns>`Reader`</returns>
    public new static Reader<Env, A> Lift(Func<Env, A> f) =>
        (Reader<Env, A>)ReaderT<Env, Identity, A>.Lift(f);

    /// <summary>
    /// Maps the Reader's environment value
    /// </summary>
    /// <param name="f">Mapping transducer</param>
    /// <returns>`Reader`</returns>
    public new Reader<Env1, A> With<Env1>(Transducer<Env1, Env> f) =>
        (Reader<Env1, A>)base.With(f);

    /// <summary>
    /// Maps the Reader's environment value
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <returns>`Reader`</returns>
    public new Reader<Env1, A> With<Env1>(Func<Env1, Env> f) =>
        (Reader<Env1, A>)base.With(f);

    /// <summary>
    /// Maps the Reader's environment value
    /// </summary>
    /// <param name="f">Mapping transducer</param>
    /// <returns>`Reader`</returns>
    public new Reader<Env, A> Local(Transducer<Env, Env> f) =>
        (Reader<Env, A>)base.Local(f);

    /// <summary>
    /// Maps the Reader's environment value
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <returns>`Reader`</returns>
    public new Reader<Env, A> Local(Func<Env, Env> f) =>
        (Reader<Env, A>)base.Local(f);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Map
    //

    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <param name="f">Mapping transducer</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`Reader`</returns>
    public new Reader<Env, B> Map<B>(Transducer<A, B> f) =>
        (Reader<Env, B>)base.Map(f);

    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`Reader`</returns>
    public new Reader<Env, B> Map<B>(Func<A, B> f) =>
        (Reader<Env, B>)base.Map(f);
    
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
    /// <param name="f">Mapping transducer</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`Reader`</returns>
    public new Reader<Env, B> Bind<B>(Transducer<A, Monad<ReaderT<Env, Identity>, B>> f) =>
        (Reader<Env, B>)base.Bind(f);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`Reader`</returns>
    public new Reader<Env, B> Bind<B>(Func<A, Monad<ReaderT<Env, Identity>, B>> f) =>
        (Reader<Env, B>)base.Bind(f);
    
    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping transducer</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`Reader`</returns>
    public Reader<Env, B> Bind<B>(Transducer<A, Reader<Env, B>> f) =>
        (Reader<Env, B>)base.Bind(f.Map(mb => (ReaderT<Env, Identity, B>)mb));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`Reader`</returns>
    public Reader<Env, B> Bind<B>(Func<A, Reader<Env, B>> f) =>
        (Reader<Env, B>)base.Bind(f);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping transducer</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`Reader`</returns>
    public new Reader<Env, B> Bind<B>(Transducer<A, Ask<Env, B>> f) =>
        (Reader<Env, B>)base.Bind(f);
    
    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`Reader`</returns>
    public new Reader<Env, B> Bind<B>(Func<A, Ask<Env, B>> f) =>
        (Reader<Env, B>)base.Bind(f);

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
    public new Reader<Env, C> SelectMany<B, C>(Func<A, Monad<ReaderT<Env, Identity>, B>> bind, Func<A, B, C> project) =>
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
    public new Reader<Env, C> SelectMany<B, C>(Func<A, Monad<Identity, B>> bind, Func<A, B, C> project) =>
        (Reader<Env, C>)base.SelectMany(bind, project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`Reader`</returns>
    public new Reader<Env, C> SelectMany<B, C>(Func<A, Pure<B>> bind, Func<A, B, C> project) =>
        (Reader<Env, C>)base.SelectMany(bind, project);
    
    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`Reader`</returns>
    public new Reader<Env, C> SelectMany<B, C>(Func<A, Ask<Env, B>> bind, Func<A, B, C> project) =>
        (Reader<Env, C>)base.SelectMany(bind, project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`Reader`</returns>
    public new Reader<Env, C> SelectMany<B, C>(Func<A, Transducer<Env, B>> bind, Func<A, B, C> project) =>
        Bind(x => Reader<Env, B>.Lift(bind(x)).Map(y => project(x, y)));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`Reader`</returns>
    public new Reader<Env, C> SelectMany<B, C>(Func<A, Transducer<Unit, B>> bind, Func<A, B, C> project) =>
        Bind(x => Reader<Env, B>.Lift(bind(x)).Map(y => project(x, y)));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Conversion operators
    //

    public static implicit operator Reader<Env, A>(Transducer<Unit, A> t) =>
        (Reader<Env, A>)(ReaderT<Env, Identity, A>)t;
    
    public static implicit operator Reader<Env, A>(Transducer<Env, A> t) =>
        (Reader<Env, A>)(ReaderT<Env, Identity, A>)t;
    
    public static implicit operator Reader<Env, A>(Transducer<Env, Monad<Identity, A>> runReader) =>
        (Reader<Env, A>)(ReaderT<Env, Identity, A>)runReader;
    
    public static implicit operator Reader<Env, A>(Pure<A> ma) =>
        (Reader<Env, A>)(ReaderT<Env, Identity, A>)ma;
    
    public static implicit operator Reader<Env, A>(Fail<Error> ma) =>
        (Reader<Env, A>)(ReaderT<Env, Identity, A>)ma;
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Run the reader
    //

    /// <summary>
    /// Run the reader monad using transducer reduction
    /// </summary>
    /// <param name="env">Input environment</param>
    /// <param name="initialState">Initial state of the reduction</param>
    /// <param name="reducer">Reducer</param>
    /// <param name="token">Optional cancellation token</param>
    /// <param name="syncContext">Optional synchronisation context</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Reduced state</returns>
    public Fin<S> Run<S>(
        Env env,
        S initialState,
        Reducer<A, S> reducer,
        CancellationToken token = default,
        SynchronizationContext? syncContext = null) =>
        runReader.Run(env, initialState, new Reduce1<S>(reducer), token, syncContext)
                 .ToFin();

    /// <summary>
    /// Run the reader monad 
    /// </summary>
    /// <remarks>
    /// Because the internals are using transducers, this is using a built-in reducer that
    /// takes the last given monad value in the transducer stream.
    /// </remarks>
    /// <param name="env">Input environment</param>
    /// <param name="token">Optional cancellation token</param>
    /// <param name="syncContext">Optional synchronisation context</param>
    /// <returns>Latest given value</returns>
    public Fin<A> Run(
        Env env,
        CancellationToken token = default,
        SynchronizationContext? syncContext = null) =>
        Run(env, default, Reducer<A>.last, token, syncContext)
           .Bind(a => a is null ? Errors.None : FinSucc(a));

    /// <summary>
    /// Run the reader monad 
    /// </summary>
    /// <remarks>
    /// Because the internals are using transducers, this is using a built-in reducer that
    /// collects every given monad in the transducer stream.
    /// </remarks>
    /// <param name="env">Input environment</param>
    /// <param name="token">Optional cancellation token</param>
    /// <param name="syncContext">Optional synchronisation context</param>
    /// <returns>Sequence of given values</returns>
    public Fin<Seq<A>> RunMany(
        Env env,
        CancellationToken token = default,
        SynchronizationContext? syncContext = null) =>
        Run(env, default, Reducer<A>.seq, token, syncContext);

    /// <summary>
    /// Run the reader monad using transducer reduction
    /// </summary>
    /// <param name="env">Input environment</param>
    /// <param name="initialState">Initial state of the reduction</param>
    /// <param name="reducer">Reducer</param>
    /// <param name="token">Optional cancellation token</param>
    /// <param name="syncContext">Optional synchronisation context</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Reduced state</returns>
    public Task<Fin<S>> RunAsync<S>(
        Env env,
        S initialState,
        Reducer<A, S> reducer,
        CancellationToken token = default,
        SynchronizationContext? syncContext = null) =>
        runReader.RunAsync(env, initialState, new Reduce1<S>(reducer), null, token, syncContext)
                 .Map(r => r.ToFin());

    /// <summary>
    /// Run the reader monad 
    /// </summary>
    /// <remarks>
    /// Because the internals are using transducers, this is using a built-in reducer that
    /// takes the last given monad value in the transducer stream.
    /// </remarks>
    /// <param name="env">Input environment</param>
    /// <param name="token">Optional cancellation token</param>
    /// <param name="syncContext">Optional synchronisation context</param>
    /// <returns>Latest given value</returns>
    public new Task<Fin<A>> RunAsync(
        Env env,
        CancellationToken token = default,
        SynchronizationContext? syncContext = null) =>
        RunAsync(env, default, Reducer<A>.last, token, syncContext)
           .Map(r => r.Bind(a => a is null ? Errors.None : FinSucc(a)));

    /// <summary>
    /// Run the reader monad 
    /// </summary>
    /// <remarks>
    /// Because the internals are using transducers, this is using a built-in reducer that
    /// collects every given monad in the transducer stream.
    /// </remarks>
    /// <param name="env">Input environment</param>
    /// <param name="token">Optional cancellation token</param>
    /// <param name="syncContext">Optional synchronisation context</param>
    /// <returns>Sequence of given values</returns>
    public Task<Fin<Seq<A>>> RunManyAsync(
        Env env,
        CancellationToken token = default,
        SynchronizationContext? syncContext = null) =>
        RunAsync(env, default, Reducer<A>.seq, token, syncContext);

    
    record Reduce1<S>(Reducer<A, S> reducer) : Reducer<Monad<Identity, A>, S>
    {
        public override TResult<S> Run(TState state, S stateValue, Monad<Identity, A> value)
        {
            var id = (Identity<A>)value;
            return id.ToTransducer().Transform(reducer).Run(state, stateValue, default);
        }
    }
}

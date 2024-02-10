using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.HKT;
using static LanguageExt.Prelude;
using static LanguageExt.Transducer;

namespace LanguageExt;

/// <summary>
/// `ReaderT` monad transformer, which adds a static environment to a given monad. 
/// </summary>
/// <param name="runReader">Transducer that represents the transformer operation</param>
/// <typeparam name="Env">Reader environment type</typeparam>
/// <typeparam name="M">Given monad trait</typeparam>
/// <typeparam name="A">Bound value type</typeparam>
public record ReaderT<Env, M, A>(Transducer<Env, Monad<M, A>> runReader) :
    MonadReaderT<ReaderT<Env, M>, Env, M, A> 
    where M : Monad<M>
{
    /// <summary>
    /// Lift a pure value into the monad-transformer
    /// </summary>
    /// <param name="value">Value to lift</param>
    /// <returns>`ReaderT`</returns>
    public static ReaderT<Env, M, A> Pure(A value) =>
        Lift(M.Pure(value));

    /// <summary>
    /// Extracts the environment value and maps it to the bound value
    /// </summary>
    /// <param name="f">Environment mapping function</param>
    /// <returns>`ReaderT`</returns>
    public static ReaderT<Env, M, A> Asks(Func<Env, A> f) =>
        Asks(lift(f));

    /// <summary>
    /// Extracts the environment value and maps it to the bound value
    /// </summary>
    /// <param name="f">Environment mapping transducer</param>
    /// <returns>`ReaderT`</returns>
    public static ReaderT<Env, M, A> Asks(Transducer<Env, A> f) =>
        new (f.Map(M.Pure));

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="monad">Monad to lift</param>
    /// <returns>`ReaderT`</returns>
    public static ReaderT<Env, M, A> Lift(Monad<M, A> monad) => 
        new (lift<Env, Monad<M, A>>(_ => monad));

    /// <summary>
    /// Lifts a unit transducer into the transformer 
    /// </summary>
    /// <param name="t">Transformer to lift</param>
    /// <returns>`ReaderT`</returns>
    public static ReaderT<Env, M, A> Lift(Transducer<Unit, A> t) =>
        new(compose(Transducer.constant<Env, Unit>(default), t.Map(M.Pure)));
    
    /// <summary>
    /// Lifts a unit function into the transformer 
    /// </summary>
    /// <param name="t">Transformer to lift</param>
    /// <returns>`ReaderT`</returns>
    public static ReaderT<Env, M, A> Lift(Func<Unit, A> t) =>
        Lift(lift(t));
    
    /// <summary>
    /// Lifts a environment transducer into the transformer 
    /// </summary>
    /// <param name="t">Transformer to lift</param>
    /// <returns>`ReaderT`</returns>
    public static ReaderT<Env, M, A> Lift(Transducer<Env, A> t) =>
        new (t.Map(M.Pure));
    
    /// <summary>
    /// Lifts a environment function into the transformer 
    /// </summary>
    /// <param name="t">Transformer to lift</param>
    /// <returns>`ReaderT`</returns>
    public static ReaderT<Env, M, A> Lift(Func<Env, A> f) =>
        Lift(lift(f));

    /// <summary>
    /// Maps the Reader's environment value
    /// </summary>
    /// <param name="f">Mapping transducer</param>
    /// <returns>`ReaderT`</returns>
    public ReaderT<Env1, M, A> With<Env1>(Transducer<Env1, Env> f) =>
        new(compose(f, runReader));

    /// <summary>
    /// Maps the Reader's environment value
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <returns>`ReaderT`</returns>
    public ReaderT<Env1, M, A> With<Env1>(Func<Env1, Env> f) =>
        With(lift(f));

    /// <summary>
    /// Maps the Reader's environment value
    /// </summary>
    /// <param name="f">Mapping transducer</param>
    /// <returns>`ReaderT`</returns>
    public ReaderT<Env, M, A> Local(Transducer<Env, Env> f) =>
        With(f);

    /// <summary>
    /// Maps the Reader's environment value
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <returns>`ReaderT`</returns>
    public ReaderT<Env, M, A> Local(Func<Env, Env> f) =>
        With(lift(f));

    /// <summary>
    /// Maps the given monad
    /// </summary>
    /// <param name="f">Mapping transducer</param>
    /// <typeparam name="M1">Trait of the monad to map to</typeparam>
    /// <returns>`ReaderT`</returns>
    public ReaderT<Env, M1, A> MapT<M1>(Transducer<Monad<M, A>, Monad<M1, A>> f) 
        where M1 : Monad<M1> =>
        new(compose(runReader, f));

    /// <summary>
    /// Maps the given monad
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="M1">Trait of the monad to map to</typeparam>
    /// <returns>`ReaderT`</returns>
    public ReaderT<Env, M1, A> MapT<M1>(Func<Monad<M, A>, Monad<M1, A>> f) 
        where M1 : Monad<M1> =>
        MapT(lift(f));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Map
    //

    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <param name="f">Mapping transducer</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`ReaderT`</returns>
    public ReaderT<Env, M, B> Map<B>(Transducer<A, B> f) =>
        Functor.map(this, f).As();

    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`ReaderT`</returns>
    public ReaderT<Env, M, B> Map<B>(Func<A, B> f) =>
        Functor.map(this, f).As();
    
    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <param name="f">Mapping transducer</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`ReaderT`</returns>
    public ReaderT<Env, M, B> Select<B>(Func<A, B> f) =>
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
    /// <returns>`ReaderT`</returns>
    public ReaderT<Env, M, B> Bind<B>(Transducer<A, Monad<ReaderT<Env, M>, B>> f) =>
        Monad.bind(this, f).As();

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`ReaderT`</returns>
    public ReaderT<Env, M, B> Bind<B>(Func<A, Monad<ReaderT<Env, M>, B>> f) =>
        Monad.bind(this, f).As();
    
    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping transducer</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`ReaderT`</returns>
    public ReaderT<Env, M, B> Bind<B>(Transducer<A, ReaderT<Env, M, B>> f) =>
        Bind(f.Map(x => (Monad<ReaderT<Env, M>, B>)x));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`ReaderT`</returns>
    public ReaderT<Env, M, B> Bind<B>(Func<A, ReaderT<Env, M, B>> f) =>
        Bind(lift(f));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping transducer</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`ReaderT`</returns>
    public ReaderT<Env, M, B> Bind<B>(Transducer<A, Ask<Env, B>> f) =>
        Bind(f.Map(ask => ask.ToReaderT<M>()));
    
    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`ReaderT`</returns>
    public ReaderT<Env, M, B> Bind<B>(Func<A, Ask<Env, B>> f) =>
        Bind(lift(f));

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
    /// <returns>`ReaderT`</returns>
    public ReaderT<Env, M, C> SelectMany<B, C>(Func<A, Monad<ReaderT<Env, M>, B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).As().Map(y => project(x, y)));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`ReaderT`</returns>
    public ReaderT<Env, M, C> SelectMany<B, C>(Func<A, ReaderT<Env, M, B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`ReaderT`</returns>
    public ReaderT<Env, M, C> SelectMany<B, C>(Func<A, Monad<M, B>> bind, Func<A, B, C> project) =>
        Bind(x => ReaderT<Env, M, B>.Lift(bind(x)).Map(y => project(x, y)));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`ReaderT`</returns>
    public ReaderT<Env, M, C> SelectMany<B, C>(Func<A, Pure<B>> bind, Func<A, B, C> project) =>
        Map(x => project(x, bind(x).Value));
    
    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`ReaderT`</returns>
    public ReaderT<Env, M, C> SelectMany<B, C>(Func<A, Ask<Env, B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).ToReaderT<M>().Map(y => project(x, y)));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`ReaderT`</returns>
    public ReaderT<Env, M, C> SelectMany<B, C>(Func<A, Transducer<Env, B>> bind, Func<A, B, C> project) =>
        Bind(x => ReaderT<Env, M, B>.Lift(bind(x)).Map(y => project(x, y)));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`ReaderT`</returns>
    public ReaderT<Env, M, C> SelectMany<B, C>(Func<A, Transducer<Unit, B>> bind, Func<A, B, C> project) =>
        Bind(x => ReaderT<Env, M, B>.Lift(bind(x)).Map(y => project(x, y)));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Conversion operators
    //

    public static implicit operator ReaderT<Env, M, A>(Transducer<Unit, A> t) =>
        new(compose(Transducer.constant<Env, Unit>(default), t.Map(M.Pure)));
    
    public static implicit operator ReaderT<Env, M, A>(Transducer<Env, A> t) =>
        new (t.Map(M.Pure));
    
    public static implicit operator ReaderT<Env, M, A>(Transducer<Env, Monad<M, A>> runReaderT) =>
        new (runReaderT);
    
    public static implicit operator ReaderT<Env, M, A>(Pure<A> ma) =>
        Pure(ma.Value);
    
    public static implicit operator ReaderT<Env, M, A>(Fail<Error> ma) =>
        Lift(fail<Env, A>(ma.Value));
    
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
        Reducer<Monad<M, A>, S> reducer,
        CancellationToken token = default,
        SynchronizationContext? syncContext = null) =>
        runReader.Run(env, initialState, reducer, token, syncContext).ToFin();

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
    /// <returns>Latest given monad</returns>
    public Fin<Monad<M, A>> Run(
        Env env,
        CancellationToken token = default,
        SynchronizationContext? syncContext = null) =>
        runReader.Run(env, default, Reducer<Monad<M, A>>.last, token, syncContext)
                 .Bind(ma => ma is null ? TResult.None<Monad<M, A>>() : TResult.Continue(ma))
                 .ToFin(); 

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
    /// <returns>Sequence of given monads</returns>
    public Fin<Seq<Monad<M, A>>> RunMany(
        Env env,
        CancellationToken token = default,
        SynchronizationContext? syncContext = null) =>
        runReader.Run(env, default, Reducer<Monad<M, A>>.seq, token, syncContext).ToFin();

    /// <summary>
    /// Run the reader monad using transducer reduction asynchronously
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
        Reducer<Monad<M, A>, S> reducer,
        CancellationToken token = default,
        SynchronizationContext? syncContext = null) =>
        runReader.RunAsync(env, initialState, reducer, null, token, syncContext).Map(r => r.ToFin());

    /// <summary>
    /// Run the reader monad asynchronously
    /// </summary>
    /// <remarks>
    /// Because the internals are using transducers, this is using a built-in reducer that
    /// takes the last given monad value in the transducer stream.
    /// </remarks>
    /// <param name="env">Input environment</param>
    /// <param name="token">Optional cancellation token</param>
    /// <param name="syncContext">Optional synchronisation context</param>
    /// <returns>Latest given monad</returns>
    public Task<Fin<Monad<M, A>>> RunAsync(
        Env env,
        CancellationToken token = default,
        SynchronizationContext? syncContext = null) =>
        runReader.RunAsync(env, default, Reducer<Monad<M, A>>.last, null, token, syncContext)
                 .Map(r => r.Bind(ma => ma is null ? TResult.None<Monad<M, A>>() : TResult.Continue(ma))
                            .ToFin());

    /// <summary>
    /// Run the reader monad asynchronously
    /// </summary>
    /// <remarks>
    /// Because the internals are using transducers, this is using a built-in reducer that
    /// collects every given monad in the transducer stream.
    /// </remarks>
    /// <param name="env">Input environment</param>
    /// <param name="token">Optional cancellation token</param>
    /// <param name="syncContext">Optional synchronisation context</param>
    /// <returns>Sequence of given monads</returns>
    public Task<Fin<Seq<Monad<M, A>>>> RunManyAsync(
        Env env,
        CancellationToken token = default,
        SynchronizationContext? syncContext = null) =>
        runReader.RunAsync(env, default, Reducer<Monad<M, A>>.seq, null, token, syncContext).Map(r => r.ToFin());
}

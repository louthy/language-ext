using System;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// `ReaderT` monad transformer, which adds a static environment to a given monad. 
/// </summary>
/// <typeparam name="Env">Reader environment type</typeparam>
/// <typeparam name="M">Given monad trait</typeparam>
/// <typeparam name="A">Bound value type</typeparam>
public record ReaderT<Env, M, A>(Func<Env, K<M, A>> runReader) : K<ReaderT<Env, M>, A>
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
        new(env => M.Pure(f(env)));

    /// <summary>
    /// Extracts the environment value and maps it to the bound value
    /// </summary>
    /// <param name="f">Environment mapping function</param>
    /// <returns>`ReaderT`</returns>
    public static ReaderT<Env, M, A> AsksM(Func<Env, K<M, A>> f) =>
        new(f);

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="monad">Monad to lift</param>
    /// <returns>`ReaderT`</returns>
    public static ReaderT<Env, M, A> Lift(Pure<A> monad) =>
        Pure(monad.Value);
    
    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="monad">Monad to lift</param>
    /// <returns>`ReaderT`</returns>
    public static ReaderT<Env, M, A> Lift(K<M, A> monad) =>
        new(_ => monad);
    
    /// <summary>
    /// Lifts a unit function into the transformer 
    /// </summary>
    /// <param name="f">Function to lift</param>
    /// <returns>`ReaderT`</returns>
    public static ReaderT<Env, M, A> Lift(Func<A> f) =>
        new (_ => M.Pure(f()));
    
    /// <summary>
    /// Lifts a unit function into the transformer 
    /// </summary>
    /// <param name="f">Function to lift</param>
    /// <returns>`ReaderT`</returns>
    public static ReaderT<Env, M, A> LiftIO(IO<A> ma) =>
        new (_ => M.LiftIOMaybe(ma));

    /// <summary>
    /// Maps the Reader's environment value
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <returns>`ReaderT`</returns>
    public ReaderT<Env1, M, A> With<Env1>(Func<Env1, Env> f) =>
        new (env1 => runReader(f(env1)));

    /// <summary>
    /// Maps the Reader's environment value
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <returns>`ReaderT`</returns>
    public ReaderT<Env, M, A> Local(Func<Env, Env> f) =>
        new (env1 => runReader(f(env1)));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Map
    //

    /// <summary>
    /// Maps the given monad
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="M1">Trait of the monad to map to</typeparam>
    /// <returns>`ReaderT`</returns>
    public ReaderT<Env, M1, B> MapM<M1, B>(Func<K<M, A>, K<M1, B>> f)
        where M1 : Monad<M1>, Alternative<M1> =>
        new (env => f(runReader(env)));

    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`ReaderT`</returns>
    public ReaderT<Env, M, B> Map<B>(Func<A, B> f) =>
        new(env => M.Map(f, runReader(env)));
    
    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <param name="f">Mapping transducer</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`ReaderT`</returns>
    public ReaderT<Env, M, B> Select<B>(Func<A, B> f) =>
        new(env => M.Map(f, runReader(env)));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Bind
    //

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`ReaderT`</returns>
    public ReaderT<Env, M, B> Bind<B>(Func<A, K<ReaderT<Env, M>, B>> f) =>
        Bind(x => f(x).As());

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`ReaderT`</returns>
    public ReaderT<Env, M, B> Bind<B>(Func<A, ReaderT<Env, M, B>> f) =>
        new(env => M.Bind(runReader(env), x => f(x).runReader(env)));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`ReaderT`</returns>
    public ReaderT<Env, M, B> Bind<B>(Func<A, Ask<Env, B>> f) =>
        Bind(x => (ReaderT<Env, M, B>)f(x));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`ReaderT`</returns>
    public ReaderT<Env, M, B> Bind<B>(Func<A, IO<B>> f) =>
        Bind(x => ReaderT<Env, M, B>.LiftIO(f(x)));

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
    public ReaderT<Env, M, C> SelectMany<B, C>(Func<A, K<ReaderT<Env, M>, B>> bind, Func<A, B, C> project) =>
        SelectMany(x => bind(x).As(), project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`ReaderT`</returns>
    public ReaderT<Env, M, C> SelectMany<B, C>(Func<A, ReaderT<Env, M, B>> bind, Func<A, B, C> project) =>
        new(env => M.Bind(runReader(env), x => M.Map(y => project(x, y), bind(x).runReader(env))));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`ReaderT`</returns>
    public ReaderT<Env, M, C> SelectMany<B, C>(Func<A, K<M, B>> bind, Func<A, B, C> project) =>
        new(env => M.Bind(runReader(env), x => M.Map(y => project(x, y), bind(x))));

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
        SelectMany(x => bind(x).ToReaderT<M>(), project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`ReaderT`</returns>
    public ReaderT<Env, M, C> SelectMany<B, C>(Func<A, IO<B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Operators
    //
    
    public static implicit operator ReaderT<Env, M, A>(Pure<A> ma) =>
        Pure(ma.Value);
    
    public static implicit operator ReaderT<Env, M, A>(Ask<Env, A> ma) =>
        Asks(ma.F);

    public static implicit operator ReaderT<Env, M, A>(IO<A> ma) =>
        LiftIO(ma);
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Run the reader
    //

    /// <summary>
    /// Run the reader monad 
    /// </summary>
    /// <param name="env">Input environment</param>
    /// <returns>Bound monad</returns>
    public K<M, A> Run(Env env) =>
        runReader(env);
}

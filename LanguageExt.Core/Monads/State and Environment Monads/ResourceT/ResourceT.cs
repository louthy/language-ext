using System;
using LanguageExt.HKT;

namespace LanguageExt;

public record ResourceT<M, A>(Func<Resources, K<M, A>> runResource) : K<ResourceT<M>, A> 
    where M : MonadIO<M>
{
    public static ResourceT<M, A> Pure(A value) =>
        new(_ => M.Pure(value));
    
    /// <summary>
    /// Extracts the environment value and maps it to the bound value
    /// </summary>
    /// <param name="f">Environment mapping function</param>
    /// <returns>`ResourceT`</returns>
    internal static ResourceT<M, A> Asks(Func<Resources, A> f) =>
        new(env => M.Pure(f(env)));

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="monad">Monad to lift</param>
    /// <returns>`ResourceT`</returns>
    public static ResourceT<M, A> Lift(Pure<A> monad) =>
        Pure(monad.Value);
    
    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="monad">Monad to lift</param>
    /// <returns>`ResourceT`</returns>
    public static ResourceT<M, A> Lift(K<M, A> monad) =>
        new(_ => monad);
    
    /// <summary>
    /// Lifts a unit function into the transformer 
    /// </summary>
    /// <param name="f">Function to lift</param>
    /// <returns>`ResourceT`</returns>
    public static ResourceT<M, A> Lift(Func<A> f) =>
        new (_ => M.Pure(f()));

    /// <summary>
    /// Maps the given monad
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="M1">Trait of the monad to map to</typeparam>
    /// <returns>`ResourceT`</returns>
    public ResourceT<M1, A> MapT<M1>(Func<K<M, A>, K<M1, A>> f)
        where M1 : Monad<M1>, MonadIO<M1> =>
        new (env => f(runResource(env)));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Map
    //

    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`ResourceT`</returns>
    public ResourceT<M, B> Map<B>(Func<A, B> f) =>
        new(env => M.Map(f, runResource(env)));
    
    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <param name="f">Mapping transducer</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`ResourceT`</returns>
    public ResourceT<M, B> Select<B>(Func<A, B> f) =>
        new(env => M.Map(f, runResource(env)));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Bind
    //

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`ResourceT`</returns>
    public ResourceT<M, B> Bind<B>(Func<A, K<ResourceT<M>, B>> f) =>
        Bind(x => f(x).As());

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`ResourceT`</returns>
    public ResourceT<M, B> Bind<B>(Func<A, ResourceT<M, B>> f) =>
        new(env => M.Bind(runResource(env), x => f(x).runResource(env)));
    
    
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
    /// <returns>`ResourceT`</returns>
    public ResourceT<M, C> SelectMany<B, C>(Func<A, K<ResourceT<M>, B>> bind, Func<A, B, C> project) =>
        SelectMany(x => bind(x).As(), project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`ResourceT`</returns>
    public ResourceT<M, C> SelectMany<B, C>(Func<A, ResourceT<M, B>> bind, Func<A, B, C> project) =>
        new(env => M.Bind(runResource(env), x => M.Map(y => project(x, y), bind(x).runResource(env))));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`ResourceT`</returns>
    public ResourceT<M, C> SelectMany<B, C>(Func<A, K<M, B>> bind, Func<A, B, C> project) =>
        new(env => M.Bind(runResource(env), x => M.Map(y => project(x, y), bind(x))));

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`ResourceT`</returns>
    public ResourceT<M, C> SelectMany<B, C>(Func<A, Pure<B>> bind, Func<A, B, C> project) =>
        Map(x => project(x, bind(x).Value));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Conversion operators
    //

    public static implicit operator ResourceT<M, A>(Pure<A> ma) =>
        Pure(ma.Value);
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Run the resource monad
    //

    /// <summary>
    /// Run the resource monad and automatically clean up the resources after 
    /// </summary>
    /// <returns>Bound monad</returns>
    public A Run(EnvIO envIO, Func<K<M, A>, IO<A>> unliftIO)
    {
        using var env = new Resources();
        return unliftIO(runResource(env)).Run(envIO);
    }

    /// <summary>
    /// Run the resource monad and automatically clean up the resources after 
    /// </summary>
    /// <returns>Bound monad</returns>
    public A Run(Func<K<M, A>, IO<A>> unliftIO) =>
        Run(EnvIO.New(), unliftIO);
}

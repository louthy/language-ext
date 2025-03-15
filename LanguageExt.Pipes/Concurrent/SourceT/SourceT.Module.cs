using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using LanguageExt.Async.Linq;
using LanguageExt.Traits;
using static LanguageExt.Prelude;
namespace LanguageExt.Pipes.Concurrent;

public partial class SourceT
{
    /// <summary>
    /// Empty source
    /// </summary>
    /// <remarks>
    /// This is a 'void' source, it yields zero values. 
    /// </remarks>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Uninhabited source</returns>
    public static SourceT<M, A> empty<M, A>() 
        where M : Monad<M>, Alternative<M> =>
        EmptySourceT<M, A>.Default;
    
    /// <summary>
    /// Lift a pure value into the source
    /// </summary>
    /// <remarks>
    /// This is a singleton/unit source, it yields exactly one value. 
    /// </remarks>
    /// <param name="value">Value to lift</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Singleton source</returns>
    public static SourceT<M, A> pure<M, A>(A value) 
        where M : Monad<M>, Alternative<M> =>
        new PureSourceT<M, A>(value);
    
    /// <summary>
    /// Lift a structure into the source
    /// </summary>
    /// <remarks>
    /// This is a singleton/unit source, it yields exactly one structure. 
    /// </remarks>
    /// <param name="ma">Value to lift</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Singleton source</returns>
    public static SourceT<M, A> liftM<M, A>(K<M, A> ma) 
        where M : Monad<M>, Alternative<M> =>
        new LiftSourceT<M, A>(ma);
    
    /// <summary>
    /// Lift a structure into the source
    /// </summary>
    /// <remarks>
    /// This is a singleton/unit source, it yields exactly one structure. 
    /// </remarks>
    /// <param name="ma">Value to lift</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Singleton source</returns>
    public static SourceT<M, A> liftIO<M, A>(K<IO, A> ma) 
        where M : Monad<M>, Alternative<M> =>
        new LiftSourceT<M, A>(M.LiftIO(ma));
    
    /// <summary>
    /// Lift a pure value into the source and yield it for infinity
    /// </summary>
    /// <remarks>
    /// This is an infinite source, it repeatedly yields a value. 
    /// </remarks>
    /// <param name="value">Value to lift</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Infinite source</returns>
    public static SourceT<M, A> forever<M, A>(A value) 
        where M : Monad<M>, Alternative<M> =>
        new ForeverSourceT<M, A>(M.Pure(value));
    
    /// <summary>
    /// Lift a structure into the source and yield it for infinity
    /// </summary>
    /// <remarks>
    /// This is an infinite source, it repeatedly yields the provided structure. 
    /// </remarks>
    /// <param name="ma">Value to lift</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Infinite source</returns>
    public static SourceT<M, A> foreverM<M, A>(K<M, A> ma) 
        where M : Monad<M>, Alternative<M> =>
        new ForeverSourceT<M, A>(ma);

    /// <summary>
    /// Make a `System.Threading.Channels.Channel` into a source of values
    /// </summary>
    /// <param name="channel">Channel to lift</param>
    /// <param name="label">Label to help debugging</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Source of values</returns>
    public static SourceT<M, A> lift<M, A>(Channel<A> channel) 
        where M : Monad<M>, Alternative<M> =>
        new ReaderPureSourceT<M, A>(channel);

    /// <summary>
    /// Make a `System.Threading.Channels.Channel` into a source of values
    /// </summary>
    /// <param name="channel">Channel to lift</param>
    /// <param name="label">Label to help debugging</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Source of values</returns>
    public static SourceT<M, A> liftM<M, A>(Channel<K<M, A>> channel) 
        where M : Monad<M>, Alternative<M> =>
        new ReaderSourceT<M, A>(channel);

    /// <summary>
    /// Make an `IEnumerable` into a source of values
    /// </summary>
    /// <param name="items">Enumerable to lift</param>
    /// <param name="label">Label to help debugging</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Source of values</returns>
    public static SourceT<M, A> lift<M, A>(IEnumerable<A> items) 
        where M : Monad<M>, Alternative<M> =>
        new IteratorSyncSourceT<M, A>(items.Select(M.Pure));

    /// <summary>
    /// Make an `IEnumerable` into a source of values
    /// </summary>
    /// <param name="items">Enumerable to lift</param>
    /// <param name="label">Label to help debugging</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Source of values</returns>
    public static SourceT<M, A> liftM<M, A>(IEnumerable<K<M, A>> items) 
        where M : Monad<M>, Alternative<M> =>
        new IteratorSyncSourceT<M, A>(items);

    /// <summary>
    /// Make an `IEnumerable` into a source of values
    /// </summary>
    /// <param name="items">Enumerable to lift</param>
    /// <param name="label">Label to help debugging</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Source of values</returns>
    public static SourceT<M, A> lift<M, A>(IAsyncEnumerable<A> items) 
        where M : Monad<M>, Alternative<M> =>
        new IteratorAsyncSourceT<M, A>(items.Select(M.Pure));

    /// <summary>
    /// Make an `IEnumerable` into a source of values
    /// </summary>
    /// <param name="items">Enumerable to lift</param>
    /// <param name="label">Label to help debugging</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Source of values</returns>
    public static SourceT<M, A> liftM<M, A>(IAsyncEnumerable<K<M, A>> items) 
        where M : Monad<M>, Alternative<M> =>
        new IteratorAsyncSourceT<M, A>(items);
    
    /// <summary>
    /// Merge sources into a single source
    /// </summary>
    /// <param name="sources">Sources</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Source that is the combination of all provided sources</returns>
    public static SourceT<M, A> merge<M, A>(Seq<SourceT<M, A>> sources) 
        where M : Monad<M>, Alternative<M> =>
        sources.Fold(empty<M, A>(), (s, s2) => s.Combine(s2));
        
    /// <summary>
    /// Merge sources into a single source
    /// </summary>
    /// <param name="sources">Sources</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Source that is the combination of all provided sources</returns>
    public static SourceT<M, A> merge<M, A>(params SourceT<M, A>[] sources) 
        where M : Monad<M>, Alternative<M> =>
        merge(toSeq(sources));

    /// <summary>
    /// Zip two sources into one
    /// </summary>
    /// <param name="second">Stream to zip with this one</param>
    /// <typeparam name="B">Bound value type of the stream to zip with this one</typeparam>
    /// <returns>Stream of values where the items from two streams are paired together</returns>
    public SourceT<M, (A First, B Second)> zip<M, A, B>(SourceT<M, A> first, SourceT<M, B> second) 
        where M : Monad<M>, Alternative<M> =>
        new Zip2SourceT<M, A, B>(first, second);

    /// <summary>
    /// Zip three sources into one
    /// </summary>
    /// <param name="second">Stream to zip with this one</param>
    /// <param name="third">Stream to zip with this one</param>
    /// <typeparam name="B">Bound value type of the stream to zip with this one</typeparam>
    /// <returns>Stream of values where the items from two streams are paired together</returns>
    public SourceT<M, (A First, B Second, C Third)> zip<M, A, B, C>(SourceT<M, A> first, SourceT<M, B> second, SourceT<M, C> third) 
        where M : Monad<M>, Alternative<M> =>
        new Zip3SourceT<M, A, B, C>(first, second, third);

    /// <summary>
    /// Zip three sources into one
    /// </summary>
    /// <param name="second">Stream to zip with this one</param>
    /// <param name="third">Stream to zip with this one</param>
    /// <param name="fourth">Stream to zip with this one</param>
    /// <typeparam name="B">Bound value type of the stream to zip with this one</typeparam>
    /// <returns>Stream of values where the items from two streams are paired together</returns>
    public SourceT<M, (A First, B Second, C Third, D Fourth)> zip<M, A, B, C, D>(SourceT<M, A> first, SourceT<M, B> second, SourceT<M, C> third, SourceT<M, D> fourth) 
        where M : Monad<M>, Alternative<M> =>
        new Zip4SourceT<M, A, B, C, D>(first, second, third, fourth);
}

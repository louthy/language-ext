using System.Linq;
using LanguageExt.Traits;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using LanguageExt.Pipes.Concurrent;

namespace LanguageExt.Pipes;

public static class StreamT
{
    /// <summary>
    /// Construct a singleton stream
    /// </summary>
    /// <param name="value">Single value in the stream</param>
    /// <returns>Stream transformer</returns>
    public static StreamT<M, A> pure<M, A>(A value)
        where M : Monad<M>, Alternative<M> =>
        new (Source.pure(M.Pure(value)));

    /// <summary>
    /// Empty stream
    /// </summary>
    /// <typeparam name="M">`Monad` and `Alternative` traits</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Uninhabited stream</returns>
    public static StreamT<M, A> empty<M, A>()
        where M : Monad<M>, Alternative<M> =>
        new (Source.empty<K<M, A>>());
    
    /*
    /// <summary>
    /// Lift any foldable into the stream
    /// </summary>
    /// <remarks>This is likely to consume the foldable structure eagerly</remarks>
    /// <param name="foldable">Foldable structure to lift</param>
    /// <returns>Stream transformer</returns>
    public static StreamT<M, A> liftF<F, M, A>(K<F, A> items)
        where M : Monad<M>, Alternative<M>
        where F : Foldable<F> =>
        new (Conduit.spawn(Buffer<K<M, A>>.Latest(items)));
        */

    /// <summary>
    /// Lift any monad into the stream
    /// </summary>
    /// <typeparam name="M">`Monad` and `Alternative` traits</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream transformer</returns>
    public static StreamT<M, A> liftM<M, A>(K<M, A> ma)
        where M : Monad<M>, Alternative<M> =>
        new (Source.pure(ma));

    /// <summary>
    /// Lift any monad into the stream
    /// </summary>
    /// <typeparam name="M">`Monad` and `Alternative` traits</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream transformer</returns>
    public static StreamT<M, A> liftIO<M, A>(K<IO, A> ma)
        where M : Monad<M>, Alternative<M> =>
        new (Source.pure(M.LiftIO(ma)));

    /// <summary>
    /// Lift an async-enumerable into the stream
    /// </summary>
    /// <typeparam name="M">`Monad` and `Alternative` traits</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="stream">Sequence to lift</param>
    /// <returns>Stream transformer</returns>
    public static StreamT<M, A> lift<M, A>(IAsyncEnumerable<A> items)
        where M : Monad<M>, Alternative<M> =>
        liftM(items.MapAsync(M.Pure));

    /// <summary>
    /// Lift an async-enumerable into the stream
    /// </summary>
    /// <typeparam name="M">`Monad` and `Alternative` traits</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="stream">Sequence to lift</param>
    /// <returns>Stream transformer</returns>
    public static StreamT<M, A> lift<M, A>(IEnumerable<A> items)
        where M : Monad<M>, Alternative<M> =>
        liftM(items.Select(M.Pure));

    /// <summary>
    /// Lift an async-enumerable into the stream
    /// </summary>
    /// <typeparam name="M">`Monad` and `Alternative` traits</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="stream">Sequence to lift</param>
    /// <returns>Stream transformer</returns>
    public static StreamT<M, A> lift<M, A>(Seq<A> items)
        where M : Monad<M>, Alternative<M> =>
        liftM(items.Select(M.Pure));

    /// <summary>
    /// Lift an async-enumerable into the stream
    /// </summary>
    /// <typeparam name="M">`Monad` and `Alternative` traits</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="stream">Sequence to lift</param>
    /// <returns>Stream transformer</returns>
    public static StreamT<M, A> liftM<M, A>(IAsyncEnumerable<K<M, A>> items)
        where M : Monad<M>, Alternative<M> =>
        new (new IteratorAsyncSource<K<M, A>>(items));

    /// <summary>
    /// Lift an async-enumerable into the stream
    /// </summary>
    /// <typeparam name="M">`Monad` and `Alternative` traits</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="stream">Sequence to lift</param>
    /// <returns>Stream transformer</returns>
    public static StreamT<M, A> liftM<M, A>(IEnumerable<K<M, A>> items)
        where M : Monad<M>, Alternative<M> =>
        new (new IteratorSyncSource<K<M, A>>(items));
    
    /// <summary>
    /// Lift an async-enumerable into the stream
    /// </summary>
    /// <typeparam name="M">`Monad` and `Alternative` traits</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="stream">Sequence to lift</param>
    /// <returns>Stream transformer</returns>
    public static StreamT<M, A> liftM<M, A>(Seq<K<M, A>> items)
        where M : Monad<M>, Alternative<M> =>
        new (new IteratorSyncSource<K<M, A>>(items));

    /// <summary>
    /// Merge many streams into one
    /// </summary>
    /// <typeparam name="M">`Monad` and `Alternative` traits</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="streams">Streams to merge</param>
    /// <returns></returns>
    public static StreamT<M, A> merge<M, A>(Seq<StreamT<M, A>> streams)
        where M : Monad<M>, Alternative<M> =>
        streams.IsEmpty
            ? empty<M, A>()
            : new(streams.Tail.Fold(streams[0].runStreamT, (ss, s) => ss.Combine(s.runStreamT)));

    /// <summary>
    /// Merge many streams into one
    /// </summary>
    /// <typeparam name="M">`Monad` and `Alternative` traits</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="streams">Streams to merge</param>
    /// <returns></returns>
    public static StreamT<M, A> merge<M, A>(params StreamT<M, A>[] streams)
        where M : Monad<M>, Alternative<M> =>
        merge(toSeq(streams));

    /// <summary>
    /// Tuple the items from the streams provided together.  
    /// </summary>
    /// <remarks>
    /// This requires waiting for each stream to yield a value so that the zippedtuple can be constructed.  If any one
    /// of the streams ends then the zipped stream also ends.
    /// </remarks>
    /// <param name="first">First stream</param>
    /// <param name="second">Second stream</param>
    /// <typeparam name="M">`Monad` and `Alternative` traits</typeparam>
    /// <typeparam name="A">First bound value type</typeparam>
    /// <typeparam name="B">Second bound value type</typeparam>
    /// <returns>Zipped stream of values</returns>
    public static StreamT<M, (A First, B Second)> zip<M, A, B>(
        StreamT<M, A> first, 
        StreamT<M, B> second) 
        where M : Monad<M>, Alternative<M> =>
        first.Zip(second);
    
    /// <summary>
    /// Tuple the items from the streams provided together.  
    /// </summary>
    /// <remarks>
    /// This requires waiting for each stream to yield a value so that the zipped tuple can be constructed.  If any one
    /// of the streams ends then the zipped stream also ends.
    /// </remarks>
    /// <param name="first">First stream</param>
    /// <param name="second">Second stream</param>
    /// <param name="third">Third stream</param>
    /// <typeparam name="M">`Monad` and `Alternative` traits</typeparam>
    /// <typeparam name="A">First bound value type</typeparam>
    /// <typeparam name="B">Second bound value type</typeparam>
    /// <typeparam name="C">Third bound value type</typeparam>
    /// <returns>Zipped stream of values</returns>
    public static StreamT<M, (A First, B Second, C Third)> zip<M, A, B, C>(
        StreamT<M, A> first, 
        StreamT<M, B> second, 
        StreamT<M, C> third) 
        where M : Monad<M>, Alternative<M> =>
        first.Zip(second, third);
    
    /// <summary>
    /// Tuple the items from the streams provided together.  
    /// </summary>
    /// <remarks>
    /// This requires waiting for each stream to yield a value so that the zipped tuple can be constructed.  If any one
    /// of the streams ends then the zipped stream also ends.
    /// </remarks>
    /// <param name="first">First stream</param>
    /// <param name="second">Second stream</param>
    /// <param name="third">Third stream</param>
    /// <param name="fourth">Fourth stream</param>
    /// <typeparam name="M">`Monad` and `Alternative` traits</typeparam>
    /// <typeparam name="A">First bound value type</typeparam>
    /// <typeparam name="B">Second bound value type</typeparam>
    /// <typeparam name="C">Third bound value type</typeparam>
    /// <typeparam name="D">Fourth bound value type</typeparam>
    /// <returns>Zipped stream of values</returns>
    public static StreamT<M, (A First, B Second, C Third, D Fourth)> zip<M, A, B, C, D>(
        StreamT<M, A> first, 
        StreamT<M, B> second, 
        StreamT<M, C> third, 
        StreamT<M, D> fourth) 
        where M : Monad<M>, Alternative<M> =>
        first.Zip(second, third, fourth);
}

using LanguageExt.Traits;
using System.Collections.Generic;

namespace LanguageExt;

/// <summary>
/// StreamT module
/// </summary>
public static class StreamT
{
    /// <summary>
    /// Construct a singleton stream
    /// </summary>
    /// <param name="value">Single value in the stream</param>
    /// <returns>Stream transformer</returns>
    public static StreamT<M, A> pure<M, A>(A value)
        where M : Monad<M> =>
        new StreamPureT<M, A>(value);

    /// <summary>
    /// Lift any foldable into the stream
    /// </summary>
    /// <remarks>This is likely to consume the foldable structure eagerly</remarks>
    /// <param name="foldable">Foldable structure to lift</param>
    /// <returns>Stream transformer</returns>
    public static StreamT<M, A> liftF<F, M, A>(K<F, A> items)
        where M : Monad<M>
        where F : Foldable<F> =>
        StreamT<M, A>.LiftF(items);

    /// <summary>
    /// Lift an async-enumerable into the stream
    /// </summary>
    /// <param name="stream">Sequence to lift</param>
    /// <returns>Stream transformer</returns>
    public static StreamT<M, A> lift<M, A>(IAsyncEnumerable<A> items)
        where M : Monad<M> =>
        StreamT<M, A>.Lift(items);

    /// <summary>
    /// Lift an enumerable into the stream
    /// </summary>
    /// <param name="stream">Sequence to lift</param>
    /// <returns>Stream transformer</returns>
    public static StreamT<M, A> lift<M, A>(IEnumerable<A> items)
        where M : Monad<M> =>
        StreamT<M, A>.Lift(items);

    /// <summary>
    /// Lift a (possibly lazy) sequence into the stream
    /// </summary>
    /// <param name="list">Sequence to lift</param>
    /// <returns>Stream transformer</returns>
    public static StreamT<M, A> lift<M, A>(Seq<A> items)
        where M : Monad<M> =>
        StreamT<M, A>.Lift(items);

    /// <summary>
    /// Lift an effect into the stream
    /// </summary>
    /// <param name="ma">Effect to lift</param>
    /// <returns>Stream transformer</returns>
    public static StreamT<M, A> lift<M, A>(K<M, A> ma)
        where M : Monad<M> =>
        StreamT<M, A>.Lift(ma);

    /// <summary>
    /// Lift side effect into the stream
    /// </summary>
    /// <param name="ma">Side effect to lift</param>
    /// <returns>Stream transformer</returns>
    public static StreamT<M, A> liftIO<M, A>(IO<A> ma)
        where M : Monad<M> =>
        StreamT<M, A>.LiftIO(ma);
    
    /// <summary>
    /// Interleave the items of two streams
    /// </summary>
    /// <param name="second">Other stream to merge with</param>
    /// <returns>Stream transformer</returns>
    public static StreamT<M, A> merge<M, A>(K<StreamT<M>, A> first, K<StreamT<M>, A> second)
        where M : Monad<M> =>
        first.As().Merge(second);

    /// <summary>
    /// Interleave the items of many streams
    /// </summary>
    /// <param name="rhs">Other stream to merge with</param>
    /// <returns>Stream transformer</returns>
    public static StreamT<M, A> merge<M, A>(K<StreamT<M>, A> first, K<StreamT<M>, A> second, params K<StreamT<M>, A>[] rest)
        where M : Monad<M> =>
        first.As().Merge(second, rest);
 
    /// <summary>
    /// Merge the items of two streams into pairs
    /// </summary>
    /// <param name="second">Other stream to merge with</param>
    /// <returns>Stream transformer</returns>
    public static StreamT<M, (A First, B Second)> zip<M, A, B>(K<StreamT<M>, A> first, K<StreamT<M>, B> second)
        where M : Monad<M> =>
        first.As().Zip(second);

    /// <summary>
    /// Merge the items of two streams into 3-tuples
    /// </summary>
    /// <param name="second">Second stream to merge with</param>
    /// <param name="third">Third stream to merge with</param>
    /// <returns>Stream transformer</returns>
    public static StreamT<M, (A First, B Second, C Third)> zip<M, A, B, C>(
        K<StreamT<M>, A> first, 
        K<StreamT<M>, B> second,
        K<StreamT<M>, C> third) 
        where M : Monad<M> =>
        first.As().Zip(second, third);

    /// <summary>
    /// Merge the items of two streams into 4-tuples
    /// </summary>
    /// <param name="second">Second stream to merge with</param>
    /// <param name="third">Third stream to merge with</param>
    /// <param name="fourth">Fourth stream to merge with</param>
    /// <returns>Stream transformer</returns>
    public static StreamT<M, (A First, B Second, C Third, D Fourth)> zip<M, A, B, C, D>(
        K<StreamT<M>, A> first,
        K<StreamT<M>, B> second,
        K<StreamT<M>, C> third,
        K<StreamT<M>, D> fourth)
        where M : Monad<M> =>
        first.As().Zip(second, third, fourth);
}

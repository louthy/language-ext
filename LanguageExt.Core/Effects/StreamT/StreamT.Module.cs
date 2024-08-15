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
}

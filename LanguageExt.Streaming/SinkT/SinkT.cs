using System;
using LanguageExt.Common;
using LanguageExt.Pipes;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Entry point to a channel.  Sinks receive values and propagate them through the
/// channel they're attached to.  The behaviour depends on the `Buffer` type they
/// were created with.
/// </summary>
/// <typeparam name="M">Lifted type</typeparam>
/// <typeparam name="A">Value type</typeparam>
public abstract record SinkT<M, A> : 
    K<SinkT<M>, A>, 
    Monoid<SinkT<M, A>>
    where M : MonadIO<M>
{
    /// <summary>
    /// Post a value to the sink
    /// </summary>
    /// <remarks>
    /// Raises `Errors.NoSpaceInSink` if the sink is full or closed.
    /// </remarks>
    /// <param name="ma">Value to post</param>
    /// <returns>IO computation that represents the posting</returns>
    public abstract K<M, Unit> PostM(K<M, A> ma);
    
    /// <summary>
    /// Post a value to the sink
    /// </summary>
    /// <remarks>
    /// Raises `Errors.NoSpaceInSink` if the sink is full or closed.
    /// </remarks>
    /// <param name="value">Value to post</param>
    /// <returns>IO computation that represents the posting</returns>
    public K<M, Unit> Post(A value) =>
        PostM(M.Pure(value));
    
    /// <summary>
    /// Complete and close the sink
    /// </summary>
    public abstract K<M, Unit> Complete();
    
    /// <summary>
    /// Complete and close the sink with an `Error`
    /// </summary>
    public abstract K<M, Unit> Fail(Error Error);

    /// <summary>
    /// Contravariant functor map
    /// </summary>
    public abstract SinkT<M, X> Comap<X>(Func<X, A> f);

    /// <summary>
    /// Contravariant functor map
    /// </summary>
    public virtual SinkT<M, X> Comap<X>(TransducerM<M, X, A> f) =>
        new SinkTContraMapT<M, A, X>(f, this);

    /// <summary>
    /// Combine two Sinks: `lhs` and `rhs` into a single Sink that takes incoming
    /// values and then posts to the `lhs` and `rhs` Sinks. 
    /// </summary>
    public SinkT<M, A> Combine(SinkT<M, A> rhs) =>
        new SinkTCombine<M, A, A, A>(x => (x, x), this, rhs);

    /// <summary>
    /// Combine two Sinks: `lhs` and `rhs` into a single Sink that takes incoming
    /// values, maps them to an `(A, B)` tuple, and then posts the first and second
    /// elements to the `lhs` and `rhs` Sinks. 
    /// </summary>
    public SinkT<M, X> Combine<X, B>(SinkT<M, B> rhs, Func<X, (A Left, B Right)> f) =>
        new SinkTCombine<M, X, A, B>(f, this, rhs);
    
    /// <summary>
    /// Combine two Sinks into a single Source.  The values are both
    /// merged into a new Sink.  
    /// </summary>
    /// <param name="lhs">Left-hand side</param>
    /// <param name="rhs">Right-hand side</param>
    /// <returns>Merged stream of values</returns>
    public static SinkT<M, A> operator +(SinkT<M, A> lhs, SinkT<M, A> rhs) =>
        lhs.Combine(rhs);

    /// <summary>
    /// Sink that is closed and can't be posted to without an error being raised
    /// </summary>
    public static SinkT<M, A> Empty => 
        SinkTEmpty<M, A>.Default;

    /// <summary>
    /// Sink that swallows everything silently
    /// </summary>
    public static SinkT<M, A> Void => 
        SinkTVoid<M, A>.Default;

    /// <summary>
    /// Convert the `Sink` to a `ConsumerT` pipe component
    /// </summary>
    /// <typeparam name="M">Monad to lift (must support `IO`)</typeparam>
    /// <returns>`ConsumerT`</returns>
    public ConsumerT<A, M, Unit> ToConsumerT() =>
        ConsumerT.repeat(ConsumerT.awaiting<M, A>().Bind(x => PostM(M.Pure(x))));
}

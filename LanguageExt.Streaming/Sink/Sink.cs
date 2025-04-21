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
/// <typeparam name="A">Value type</typeparam>
public abstract record Sink<A> : 
    K<Sink, A>, 
    Monoid<Sink<A>>
{
    /// <summary>
    /// Post a value to the Sink
    /// </summary>
    /// <remarks>
    /// Raises `Errors.NoSpaceInSink` if the Sink is full or closed.
    /// </remarks>
    /// <param name="value">Value to post</param>
    /// <returns>IO computation that represents the posting</returns>
    public abstract IO<Unit> Post(A value);
    
    /// <summary>
    /// Complete and close the Sink
    /// </summary>
    public abstract IO<Unit> Complete();
    
    /// <summary>
    /// Complete and close the Sink with an `Error`
    /// </summary>
    public abstract IO<Unit> Fail(Error Error);

    /// <summary>
    /// Contravariant functor map
    /// </summary>
    public abstract Sink<B> Comap<B>(Func<B, A> f);

    /// <summary>
    /// Combine two Sinks: `lhs` and `rhs` into a single Sink that takes incoming
    /// values and then posts to the `lhs` and `rhs` Sinks. 
    /// </summary>
    public Sink<A> Combine(Sink<A> rhs) =>
        new SinkCombine<A, A, A>(x => (x, x), this, rhs);

    /// <summary>
    /// Combine two Sinks: `lhs` and `rhs` into a single Sink that takes incoming
    /// values, maps them to an `(A, B)` tuple, and then posts the first and second
    /// elements to the `lhs` and `rhs` Sinks. 
    /// </summary>
    public Sink<X> Combine<X, B>(Func<X, (A Left, B Right)> f, Sink<B> rhs) =>
        new SinkCombine<X, A, B>(f, this, rhs);
    
    /// <summary>
    /// Combine two Sinks into a single Source.  The values are both
    /// merged into a new Sink.  
    /// </summary>
    /// <param name="lhs">Left-hand side</param>
    /// <param name="rhs">Right-hand side</param>
    /// <returns>Merged stream of values</returns>
    public static Sink<A> operator +(Sink<A> lhs, Sink<A> rhs) =>
        lhs.Combine(rhs);

    /// <summary>
    /// Sink that is closed and can't be posted to without an error being raised
    /// </summary>
    public static Sink<A> Empty => 
        SinkEmpty<A>.Default;

    /// <summary>
    /// Sink that swallows everything silently
    /// </summary>
    public static Sink<A> Void => 
        SinkVoid<A>.Default;

    /// <summary>
    /// Convert the `Sink` to a `ConsumerT` pipe component
    /// </summary>
    /// <typeparam name="M">Monad to lift (must support `IO`)</typeparam>
    /// <returns>`ConsumerT`</returns>
    public ConsumerT<A, M, Unit> ToConsumerT<M>()
        where M : MonadIO<M> =>
        ConsumerT.repeat(ConsumerT.awaiting<M, A>().Bind(Post));

    /// <summary>
    /// Convert the `Sink` to a `Consumer` pipe component
    /// </summary>
    /// <returns>`Consumer`</returns>
    public Consumer<RT, A, Unit> ToConsumer<RT>() =>
        ToConsumerT<Eff<RT>>();
}

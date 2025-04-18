using System;
using LanguageExt.Common;
using LanguageExt.Pipes;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Represents a channel.  A channel has:
///
///   * A `Sink`: a queue of values that are its input.
///   * A `Source`:  a stream of values that are its output.
///
/// Both sides of the Conduit can be manipulated:
///
/// The `Sink` is a `Cofunctor` and can be mapped using `Contramap`, this
/// transforms values before they get to the channel.
/// 
/// The `Source` is a `Monad`, so you can `Map`, `Bind`, `Apply`, in the
/// usual way to map values on their way out.  They manipulate values as they
/// leave the channel. 
///
/// `Source` values can be both merged (using `+` or `Combine) and
/// 'chosen' using `|` or `Choose`.
///
/// Incoming `Sink` values can be split and passed to multiple `Sink`
/// channels using (using `+` or `Combine)
///
/// `ToProducer` and `ToConsumer` allows the `Conduit` components to be used
/// in composed pipe effects.
/// </summary>
/// <param name="Sink">Sink</param>
/// <param name="Source">Source</param>
/// <typeparam name="A">Input value type</typeparam>
/// <typeparam name="B">Output value type</typeparam>
public record ConduitT<M, A, B>(Sink<A> Sink, SourceT<M, B> Source)
    where M : MonadIO<M>, Monad<M>, Alternative<M> 
{
    /// <summary>
    /// Post a value to the Sink
    /// </summary>
    /// <remarks>
    /// Raises `Errors.NoSpaceInSink` if the Sink is full or closed.
    /// </remarks>
    /// <param name="value">Value to post</param>
    /// <returns>IO computation that represents the posting</returns>
    public K<M, Unit> Post(A value) =>
        M.LiftIO(Sink.Post(value));
    
    /// <summary>
    /// Iterate the stream, flowing values downstream to the reducer, which aggregates a
    /// result value.  This is returned lifted. 
    /// </summary>
    /// <remarks>Note, this is recursive, so `M` needs to be able to support recursion without
    /// blowing the stack.  If you have the `IO` monad in your stack then this will automatically
    /// be the case.</remarks>
    /// <param name="state">Initial state</param>
    /// <param name="reducer">Reducer</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Lifted aggregate state</returns>
    public K<M, S> Reduce<S>(S state, ReducerM<M, B, S> reducer) =>
        Source.Reduce(state, reducer);

    /// <summary>
    /// Iterate the stream, flowing values downstream to the reducer, which aggregates a
    /// result value.  This is returned lifted. 
    /// </summary>
    /// <remarks>Note, this is recursive, so `M` needs to be able to support recursion without
    /// blowing the stack.  If you have the `IO` monad in your stack then this will automatically
    /// be the case.</remarks>
    /// <param name="state">Initial state</param>
    /// <param name="reducer">Reducer</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Lifted aggregate state</returns>
    public K<M, S> ReduceM<S>(S state, ReducerM<M, K<M, B>, S> reducer) =>
        Source.ReduceM(state, reducer);
    
    /// <summary>
    /// Complete and close the Sink
    /// </summary>
    public K<M, Unit> Complete() =>
        M.LiftIO(Sink.Complete());
    
    /// <summary>
    /// Complete and close the Sink with an `Error`
    /// </summary>
    public K<M, Unit> Fail(Error Error) =>
        M.LiftIO(Sink.Fail(Error));
    
    /// <summary>
    /// Functor map
    /// </summary>
    public ConduitT<M, A, C> Map<C>(Func<B, C> f) =>
        new (Sink, Source.Map(f));
    
    /// <summary>
    /// Monad bind
    /// </summary>
    public ConduitT<M, A, C> Bind<C>(Func<B, SourceT<M, C>> f) =>
        new (Sink, Source.Bind(f));
    
    /// <summary>
    /// Applicative apply
    /// </summary>
    public ConduitT<M, A, C> ApplyBack<C>(SourceT<M, Func<B, C>> ff) =>
        new (Sink, Source.ApplyBack(ff));
    
    /// <summary>
    /// Contravariant functor map
    /// </summary>
    public ConduitT<M, X, B> Contramap<X>(Func<X, A> f) =>
        new (Sink.Contramap(f), Source);

    /// <summary>
    /// Convert the `Sink` to a `ConsumerT` pipe component
    /// </summary>
    /// <typeparam name="M">Monad to lift (must support `IO`)</typeparam>
    /// <returns>`ConsumerT`</returns>
    public ConsumerT<A, M, Unit> ToConsumer() =>
        Sink.ToConsumerT<M>();

    /// <summary>
    /// Convert `Source` to a `ProducerT` pipe component
    /// </summary>
    /// <typeparam name="M">Monad to lift (must support `IO`)</typeparam>
    /// <returns>`ProducerT`</returns>
    public ProducerT<B, M, Unit> ToProducer() =>
        Source.ToProducerT();
    
    /// <summary>
    /// Combine two Sinkes: `lhs` and `rhs` into a single Sink that takes incoming
    /// values and then posts the to the `lhs` and `rhs` Sinkes. 
    /// </summary>
    public ConduitT<M, A, B> Combine(Sink<A> rhs) =>
        this with { Sink = Sink.Combine(rhs) };
    
    /// <summary>
    /// Combine two Sinkes: `lhs` and `rhs` into a single Sink that takes incoming
    /// values, maps them to an `(A, B)` tuple, and the posts the first and second
    /// elements to the `lhs` and `rhs` Sinkes. 
    /// </summary>
    public ConduitT<M, X, B> Combine<X, C>(Func<X, (A Left, C Right)> f, Sink<C> rhs) =>
        new (Sink.Combine(f, rhs), Source);

    /// <summary>
    /// Combine two Sourcees into a single Source.  The value streams are both
    /// merged into a new stream.  Values are yielded as they become available.
    /// </summary>
    /// <param name="rhs">Right hand side</param>
    /// <returns>Merged stream of values</returns>
    public ConduitT<M, A, B> Combine(SourceT<M, B> rhs) =>
        this with { Source = Source.Combine(rhs) };
    
    /// <summary>
    /// Choose a value from the first `Source` to successfully yield 
    /// </summary>
    /// <param name="rhs"></param>
    /// <returns>Value from this `Source` if there are any available, if not, from `rhs`.  If
    /// `rhs` is also empty then `Errors.SourceClosed` is raised</returns>
    public ConduitT<M, A, B> Choose(SourceT<M, B> rhs) =>
        this with { Source = Source.Choose(rhs) };
    
    /// <summary>
    /// Combine two Sinkes into a single Source.  The values are both
    /// merged into a new Sink.  
    /// </summary>
    /// <param name="lhs">Left hand side</param>
    /// <param name="rhs">Right hand side</param>
    /// <returns>Merged stream of values</returns>
    public static ConduitT<M, A, B> operator +(Sink<A> lhs, ConduitT<M, A, B> rhs) =>
        rhs with { Sink = lhs.Combine(rhs.Sink) };
    
    /// <summary>
    /// Combine two Sourcees into a single Source.  The value streams are both
    /// merged into a new stream.  Values are yielded as they become available.
    /// </summary>
    /// <param name="lhs">Left hand side</param>
    /// <param name="rhs">Right hand side</param>
    /// <returns>Merged stream of values</returns>
    public static ConduitT<M, A, B> operator +(ConduitT<M, A, B> lhs, SourceT<M, B> rhs) =>
        lhs.Combine(rhs);

    /// <summary>
    /// Choose a value from the first `Source` to successfully yield 
    /// </summary>
    /// <param name="lhs">Left hand side</param>
    /// <param name="rhs">Right hand side</param>
    /// <returns>Value from the `lhs` `Source` if there are any available, if not, from `rhs`.  If
    /// `rhs` is also empty then `Errors.SourceChannelClosed` is raised</returns>
    public static ConduitT<M, A, B> operator |(ConduitT<M, A, B> lhs, SourceT<M, B> rhs) =>
        lhs.Choose(rhs);
}

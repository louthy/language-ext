using System;
using LanguageExt.Common;
using LanguageExt.Pipes;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// <para>
/// Represents a channel with an internal queue.  A channel has:
/// </para>
/// <para>
///   * A `Sink`: an input DSL that manipulates values before being placed into the internal queue.
///   * An internal queue: usually a `System.Threading.Channels.Channel`.
///   * A `Source`:  an output DSL that manipulates values after being taken from the internal queue.
/// </para>
/// <para>
/// Both sides of the `ConduitT` can be manipulated:
/// </para>
/// <para>
/// The `Sink` is a `Cofunctor` and can be mapped using `Comap`, this  transforms values _before_ they get to the
/// channel.
/// </para>
/// <para>
/// The `Source` is a monad-transformer, so you can `Map`, `Bind`, `Apply`, in the usual way to map values on their way
/// out.  They manipulate values as they leave the channel through the `Source`.
/// </para>
/// <para>
/// Control of the internal queue is provided by passing a `Buffer` value to `ConduitT.make`.  This allows you to set
/// various parameters for the internal queue, such as the maximum number of items to hold in the queue, and what
/// strategy to use when the queue is full.  The default is `Buffer.Unbounded`.
/// </para>
/// <para>
/// `ToProducer` and `ToConsumer` enable the `ConduitT` components to be used in composed pipe effects.
/// </para>
/// </summary>
/// <param name="Sink">Sink</param>
/// <param name="Source">Source</param>
/// <typeparam name="A">Input value type</typeparam>
/// <typeparam name="B">Output value type</typeparam>
public abstract class ConduitT<M, A, B> : K<ConduitT<M, A>, B>
    where M : MonadIO<M>, Monad<M>, Fallible<M>
{
    /// <summary>
    /// Post a value to the Sink
    /// </summary>
    /// <remarks>
    /// Raises `Errors.SinkFull` if the Sink is full or closed.
    /// </remarks>
    /// <param name="value">Value to post</param>
    /// <returns>IO computation that represents the posting</returns>
    public abstract K<M, Unit> Post(A value);

    /// <summary>
    /// Post a value to the Sink
    /// </summary>
    /// <remarks>
    /// Raises `Errors.SinkFull` if the sink is full or closed.
    /// </remarks>
    /// <param name="ma">Operation to post</param>
    /// <returns>IO computation that represents the posting</returns>
    public abstract K<M, Unit> PostM(K<M, A> ma);

    /// <summary>
    /// Complete and close the sink
    /// </summary>
    public abstract K<M, Unit> Complete();

    /// <summary>
    /// Complete and close the sink with an `Error`
    /// </summary>
    public abstract K<M, Unit> Fail(Error Error);

    /// <summary>
    /// Iterate the stream, flowing values downstream to the reducer, which aggregates a
    /// result value.  This is returned lifted. 
    /// </summary>
    /// <remarks>Note, this is recursive, so `M` needs to be able to support recursion without
    /// blowing the stack.  If you have the `IO` monad in your stack, then this will automatically
    /// be the case.</remarks>
    /// <param name="state">Initial state</param>
    /// <param name="reducer">Reducer</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Lifted aggregate state</returns>
    public abstract K<M, S> Reduce<S>(S state, ReducerM<M, B, S> reducer);

    /// <summary>
    /// Functor map
    /// </summary>
    public abstract ConduitT<M, A, C> Map<C>(Func<B, C> f);

    /// <summary>
    /// Functor map
    /// </summary>
    public ConduitT<M, A, C> Select<C>(Func<B, C> f) =>
        Map(f);

    /// <summary>
    /// Contravariant functor map
    /// </summary>
    public abstract ConduitT<M, X, B> Comap<X>(Func<X, A> f);

    /// <summary>
    /// Access the underlying `SinkT` for more direct manipulation.  
    /// </summary>
    /// <returns></returns>
    public abstract SinkT<M, A> Sink { get; }

    /// <summary>
    /// Access the underlying `SourceT` for more direct manipulation.  
    /// </summary>
    /// <returns></returns>
    public abstract SourceT<M, B> Source { get; }

    /// <summary>
    /// Convert the conduit's `Sink` to a `ConsumerT` pipe component
    /// </summary>
    /// <returns>`ConsumerT`</returns>
    public abstract ConsumerT<A, M, Unit> ToConsumerT();

    /// <summary>
    /// Convert the conduit's `Source` to a `ProducerT` pipe component
    /// </summary>
    /// <returns>`ProducerT`</returns>
    public abstract ProducerT<B, M, Unit> ToProducerT();

    /// <summary>
    /// Transform with a transducer
    /// </summary>
    /// <param name="transducer">Transducer to use to transform</param>
    /// <returns>Transformed source</returns>
    public abstract ConduitT<M, A, C> Transform<C>(TransducerM<M, B, C> transducer);

    /// <summary>
    /// Co-transform with a transducer
    /// </summary>
    /// <param name="transducer">Transducer to use to transform</param>
    /// <returns>Transformed source</returns>
    public abstract ConduitT<M, X, B> CoTransform<X>(TransducerM<M, X, A> transducer);

    /// <summary>
    /// Filter values.  Yielding downstream when `true`
    /// </summary>
    /// <param name="f">Filter function</param>
    /// <returns>SourceT where the only values yield are those that pass the predicate</returns>
    public ConduitT<M, A, B> Where(Func<B, bool> f) =>
        Filter(f);

    /// <summary>
    /// Filter values.  Yielding downstream when `true`
    /// </summary>
    /// <param name="f">Filter function</param>
    /// <returns>SourceT where the only values yield are those that pass the predicate</returns>
    public abstract ConduitT<M, A, B> Filter(Func<B, bool> f);

    /// <summary>
    /// Skip items in the source
    /// </summary>
    /// <param name="amount">Number to skip</param>
    /// <returns>Transformed source</returns>
    public abstract ConduitT<M, A, B> Skip(int amount);

    /// <summary>
    /// Limit the number of items processed 
    /// </summary>
    /// <param name="amount">Number to take</param>
    /// <returns>Transformed source</returns>
    public abstract ConduitT<M, A, B> Take(int amount);

    /// <summary>
    /// Fold the values flowing through.  Values are yielded downstream when either the predicate returns
    /// `false`, or the source completes. 
    /// </summary>
    /// <param name="Fold">Binary operator</param>
    /// <param name="Pred">Predicate</param>
    /// <param name="Init">Initial state</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Stream of aggregate states</returns>
    public abstract ConduitT<M, A, S> FoldWhile<S>(Func<S, B, S> Fold, Func<S, B, bool> Pred, S Init);

    /// <summary>
    /// Fold the values flowing through.  Values are yielded downstream when either the predicate returns
    /// `true`, or the source completes. 
    /// </summary>
    /// <param name="Fold">Binary operator</param>
    /// <param name="Pred">Predicate</param>
    /// <param name="Init">Initial state</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Stream of aggregate states</returns>
    public abstract ConduitT<M, A, S> FoldUntil<S>(Func<S, B, S> Fold, Func<S, B, bool> Pred, S Init);

    /// <summary>
    /// Fold the values flowing through.  Values are yielded downstream when either the schedule expires, the
    /// predicate returns `false`, or the source completes. 
    /// </summary>
    /// <param name="Time">Schedule to control the rate of processing</param>
    /// <param name="Fold">Binary operator</param>
    /// <param name="Pred">Predicate</param>
    /// <param name="Init">Initial state</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Stream of aggregate states</returns>
    public abstract ConduitT<M, A, S> FoldWhile<S>(
        Schedule Time,
        Func<S, B, S> Fold,
        Func<S, B, bool> Pred,
        S Init);

    /// <summary>
    /// Fold the values flowing through.  Values are yielded downstream when either the schedule expires, the
    /// predicate returns `true`, or the source completes. 
    /// </summary>
    /// <param name="Time">Schedule to control the rate of processing</param>
    /// <param name="Fold">Binary operator</param>
    /// <param name="Pred">Predicate</param>
    /// <param name="Init">Initial state</param>
    /// <typeparam name="S"></typeparam>
    /// <returns>Stream of aggregate states</returns>
    public abstract ConduitT<M, A, S> FoldUntil<S>(
        Schedule Time,
        Func<S, B, S> Fold,
        Func<S, B, bool> Pred,
        S Init);
}

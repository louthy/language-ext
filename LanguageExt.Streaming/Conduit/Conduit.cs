using System;
using LanguageExt.Common;
using LanguageExt.Pipes;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Represents a channel with an internal queue.  A channel has:
///
///   * A sink: an input transducer that manipulates values before being placed into the internal queue.
///   * A buffer: `System.Threading.Channels.Channel`.
///   * A source: an output transducer that manipulates values after being taken from the internal queue.
///
/// Both sides of the conduit can be manipulated:
///
/// The sink is a co-functor and can be mapped using `Comap` or `CoTransform`, these transform values _before_ they get
/// to the conduit's buffer.
/// 
/// The source is a functor, so you can `Map` or `Transform` in the usual way to map values on their way out of the
/// buffer.  
///
/// Control of the internal buffer is provided by passing a `Buffer` value to `Conduit.make`.  This allows you to set
/// various parameters for the internal queue, such as the maximum number of items to hold in the queue, and what
/// strategy to use when the queue is full.  The default is `Buffer.Unbounded`.
///
/// `ToProducer` and `ToConsumer` enable the `Conduit` components to be used in composed pipe effects.
/// </summary>
/// <typeparam name="A">Input value type</typeparam>
/// <typeparam name="B">Output value type</typeparam>
public abstract class Conduit<A, B>
{
    /// <summary>
    /// Access the underlying `Sink` for more direct manipulation.  
    /// </summary>
    /// <returns></returns>
    public abstract Sink<A> Sink { get; }

    /// <summary>
    /// Access the underlying `Source` for more direct manipulation.  
    /// </summary>
    /// <returns></returns>
    public abstract Source<B> Source  { get; }
    
    /// <summary>
    /// Post a value to the `Sink`
    /// </summary>
    /// <remarks>
    /// Raises `Errors.SinkFull` if the `Sink` is full or closed.
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
    /// Iterate the stream, flowing values downstream to the reducer, which aggregates a
    /// result value.   
    /// </summary>
    /// <param name="state">State to reduce</param>
    /// <param name="reducer">Reducer</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Reduced state</returns>
    public abstract IO<S> Reduce<S>(S state, ReducerAsync<B, S> reducer);
    
    /// <summary>
    /// Iterate the stream, flowing values downstream to the reducer, which aggregates a
    /// result value.   
    /// </summary>
    /// <param name="state">State to reduce</param>
    /// <param name="reducer">Reducer</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Reduced state</returns>
    public abstract K<M, S> Reduce<M, S>(S state, ReducerAsync<B, S> reducer) 
        where M : MonadIO<M>;
    
    /// <summary>
    /// Functor map
    /// </summary>
    public abstract Conduit<A, C> Map<C>(Func<B, C> f);

    /// <summary>
    /// Functor map
    /// </summary>
    public Conduit<A, C> Select<C>(Func<B, C> f) =>
        Map(f);
    
    /// <summary>
    /// Transform with a transducer
    /// </summary>
    /// <param name="transducer">Transducer to use to transform</param>
    /// <returns>Transformed source</returns>
    public abstract Conduit<A, C> Transform<C>(Transducer<B, C> transducer);
    
    /// <summary>
    /// Contravariant functor map
    /// </summary>
    public abstract Conduit<X, B> Comap<X>(Func<X, A> f);
    
    /// <summary>
    /// Co-transform with a transducer
    /// </summary>
    /// <param name="transducer">Transducer to use to transform</param>
    /// <returns>Transformed source</returns>
    public abstract Conduit<X, B> CoTransform<X>(Transducer<X, A> transducer);

    /// <summary>
    /// Convert the `Sink` to a `ConsumerT` pipe component
    /// </summary>
    /// <typeparam name="M">Monad to lift (must support `IO`)</typeparam>
    /// <returns>`ConsumerT`</returns>
    public abstract ConsumerT<A, M, Unit> ToConsumerT<M>()
        where M : MonadIO<M>;

    /// <summary>
    /// Convert the `Sink` to a `Consumer` pipe component
    /// </summary>
    /// <returns>`Consumer`</returns>
    public abstract Consumer<RT, A, Unit> ToConsumer<RT>();

    /// <summary>
    /// Convert `Source` to a `ProducerT` pipe component
    /// </summary>
    /// <typeparam name="M">Monad to lift (must support `IO`)</typeparam>
    /// <returns>`ProducerT`</returns>
    public abstract ProducerT<B, M, Unit> ToProducerT<M>()
        where M : MonadIO<M>;

    /// <summary>
    /// Convert `Source` to a `Producer` pipe component
    /// </summary>
    /// <returns>`Producer`</returns>
    public abstract Producer<RT, B, Unit> ToProducer<RT>();

    /// <summary>
    /// Filter values.  Yielding downstream when `true`
    /// </summary>
    /// <param name="f">Filter function</param>
    /// <returns>SourceT where the only values yield are those that pass the predicate</returns>
    public Conduit<A, B> Where(Func<B, bool> f) =>
        Filter(f);
    
    /// <summary>
    /// Filter values.  Yielding downstream when `true`
    /// </summary>
    /// <param name="f">Filter function</param>
    /// <returns>SourceT where the only values yield are those that pass the predicate</returns>
    public abstract Conduit<A, B> Filter(Func<B, bool> f); 

    /// <summary>
    /// Skip items in the source
    /// </summary>
    /// <param name="amount">Amount to skip</param>
    /// <returns>Transformed source</returns>
    public abstract Conduit<A, B> Skip(int amount); 

    /// <summary>
    /// Limit the number of items processed 
    /// </summary>
    /// <param name="amount">Number to take</param>
    /// <returns>Transformed source</returns>
    public abstract Conduit<A, B> Take(int amount); 

    /// <summary>
    /// Fold the values flowing through.  Values are yielded downstream when either the predicate returns
    /// `false`, or the source completes. 
    /// </summary>
    /// <param name="Fold">Binary operator</param>
    /// <param name="Pred">Predicate</param>
    /// <param name="Init">Initial state</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Stream of aggregate states</returns>
    public abstract Conduit<A, S> FoldWhile<S>(Func<S, B, S> Fold, Func<S, B, bool> Pred, S Init);

    /// <summary>
    /// Fold the values flowing through.  Values are yielded downstream when either the predicate returns
    /// `true`, or the source completes. 
    /// </summary>
    /// <param name="Fold">Binary operator</param>
    /// <param name="Pred">Predicate</param>
    /// <param name="Init">Initial state</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Stream of aggregate states</returns>
    public abstract Conduit<A, S> FoldUntil<S>(Func<S, B, S> Fold, Func<S, B, bool> Pred, S Init);

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
    public abstract Conduit<A, S> FoldWhile<S>(
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
    public abstract Conduit<A, S> FoldUntil<S>(
        Schedule Time,
        Func<S, B, S> Fold,
        Func<S, B, bool> Pred,
        S Init);
}

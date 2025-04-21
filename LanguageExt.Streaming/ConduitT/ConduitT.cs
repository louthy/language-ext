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
    /// blowing the stack.  If you have the `IO` monad in your stack, then this will automatically
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
    /// blowing the stack.  If you have the `IO` monad in your stack, then this will automatically
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
    /// Functor map
    /// </summary>
    public ConduitT<M, A, C> Select<C>(Func<B, C> f) =>
        Map(f); 
    
    /// <summary>
    /// Monad bind
    /// </summary>
    public ConduitT<M, A, C> Bind<C>(Func<B, SourceT<M, C>> f) =>
        new (Sink, Source.Bind(f));
    
    /// <summary>
    /// Monad bind
    /// </summary>
    public ConduitT<M, A, D> SelectMany<C, D>(Func<B, SourceT<M, C>> bind, Func<B, C, D> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));
    
    /// <summary>
    /// Applicative apply
    /// </summary>
    public ConduitT<M, A, C> ApplyBack<C>(SourceT<M, Func<B, C>> ff) =>
        new (Sink, Source.ApplyBack(ff));
    
    /// <summary>
    /// Contravariant functor map
    /// </summary>
    public ConduitT<M, X, B> Comap<X>(Func<X, A> f) =>
        new (Sink.Comap(f), Source);

    /// <summary>
    /// Convert the conduit's `Sink` to a `ConsumerT` pipe component
    /// </summary>
    /// <returns>`ConsumerT`</returns>
    public ConsumerT<A, M, Unit> ToConsumerT() =>
        Sink.ToConsumerT<M>();

    /// <summary>
    /// Convert the conduit's `Source` to a `ProducerT` pipe component
    /// </summary>
    /// <returns>`ProducerT`</returns>
    public ProducerT<B, M, Unit> ToProducerT() =>
        Source.ToProducerT();
    
    /// <summary>
    /// Combine two Sinks: `lhs` and `rhs` into a single Sink that takes incoming
    /// values and then posts to the `lhs` and `rhs` Sinks. 
    /// </summary>
    public ConduitT<M, A, B> Combine(Sink<A> rhs) =>
        this with { Sink = Sink.Combine(rhs) };
    
    /// <summary>
    /// Combine two Sinks: `lhs` and `rhs` into a single Sink that takes incoming
    /// values, maps them to an `(A, B)` tuple, and then posts the first and second
    /// elements to the `lhs` and `rhs` Sinks. 
    /// </summary>
    public ConduitT<M, X, B> Combine<X, C>(Func<X, (A Left, C Right)> f, Sink<C> rhs) =>
        new (Sink.Combine(f, rhs), Source);

    /// <summary>
    /// Combine two Sources into a single Source.  The value streams are both
    /// merged into a new stream.  Values are yielded as they become available.
    /// </summary>
    /// <param name="rhs">Right-hand side</param>
    /// <returns>Merged stream of values</returns>
    public ConduitT<M, A, B> Combine(SourceT<M, B> rhs) =>
        this with { Source = Source.Combine(rhs) };
    
    /// <summary>
    /// Choose a value from the first `Source` to successfully yield 
    /// </summary>
    /// <param name="rhs"></param>
    /// <returns>Value from this `Source` if there are any available, if not, from `rhs`.  If
    /// `rhs` is also empty, then `Errors.SourceClosed` is raised</returns>
    public ConduitT<M, A, B> Choose(SourceT<M, B> rhs) =>
        this with { Source = Source.Choose(rhs) };
    
    /// <summary>
    /// Transform with a transducer
    /// </summary>
    /// <param name="transducer">Transducer to use to transform</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>Transformed source</returns>
    public ConduitT<M, A, C> TransformM<C>(Transducer<K<M, B>, K<M, C>> transducer) =>
        With(Source.TransformM(transducer)); 
    
    /// <summary>
    /// Filter values.  Yielding downstream when `true`
    /// </summary>
    /// <param name="f">Filter function</param>
    /// <returns>SourceT where the only values yield are those that pass the predicate</returns>
    public ConduitT<M, A, B> Where(Func<B, bool> f) =>
        With(Source.Filter(f)); 
    
    /// <summary>
    /// Filter values.  Yielding downstream when `true`
    /// </summary>
    /// <param name="f">Filter function</param>
    /// <returns>SourceT where the only values yield are those that pass the predicate</returns>
    public ConduitT<M, A, B> Filter(Func<B, bool> f) =>
        With(Source.Filter(f)); 

    /// <summary>
    /// Zip two sources into one
    /// </summary>
    /// <param name="second">Stream to zip with this one</param>
    /// <typeparam name="B">Bound value-type of the stream to zip with this one</typeparam>
    /// <returns>Stream of values where the items from two streams are paired together</returns>
    public ConduitT<M, A, (B First, C Second)> Zip<C>(SourceT<M, C> second) =>
        With(Source.Zip(second)); 

    /// <summary>
    /// Zip three sources into one
    /// </summary>
    /// <param name="second">Stream to zip with this one</param>
    /// <param name="third">Stream to zip with this one</param>
    /// <typeparam name="B">Bound value-type of the stream to zip with this one</typeparam>
    /// <returns>Stream of values where the items from two streams are paired together</returns>
    public ConduitT<M, A, (B First, C Second, D Third)> Zip<C, D>(SourceT<M, C> second, SourceT<M, D> third) =>
        With(Source.Zip(second, third)); 

    /// <summary>
    /// Zip three sources into one
    /// </summary>
    /// <param name="second">Stream to zip with this one</param>
    /// <param name="third">Stream to zip with this one</param>
    /// <param name="fourth">Stream to zip with this one</param>
    /// <typeparam name="B">Bound value-type of the stream to zip with this one</typeparam>
    /// <returns>Stream of values where the items from two streams are paired together</returns>
    public ConduitT<M, A, (B First, C Second, D Third, E Fourth)> Zip<C, D, E>(
        SourceT<M, C> second, 
        SourceT<M, D> third, 
        SourceT<M, E> fourth) =>
        With(Source.Zip(second, third, fourth)); 

    /// <summary>
    /// Skip items in the source
    /// </summary>
    /// <param name="amount">Amount to skip</param>
    /// <returns>Transformed source</returns>
    public ConduitT<M, A, B> Skip(int amount) =>
        With(Source.Skip(amount)); 

    /// <summary>
    /// Limit the number of items processed 
    /// </summary>
    /// <param name="amount">Number to take</param>
    /// <returns>Transformed source</returns>
    public ConduitT<M, A, B> Take(int amount) =>
        With(Source.Take(amount)); 

    /// <summary>
    /// Fold the values flowing through.  A value is only yielded downstream upon completion of the stream.
    /// </summary>
    /// <param name="Fold">Binary operator</param>
    /// <param name="Init">Initial state</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Stream of aggregate state</returns>
    public ConduitT<M, A, S> Fold<S>(Func<S, B, S> Fold, S Init) =>
        With(Source.Fold(Fold, Init));
    
    /// <summary>
    /// Fold the values flowing through.  Values are yielded downstream when either the schedule expires, or the
    /// source completes. 
    /// </summary>
    /// <param name="Time">Schedule to control the rate of processing</param>
    /// <param name="Fold">Binary operator</param>
    /// <param name="Init">Initial state</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Stream of aggregate states</returns>
    public ConduitT<M, A, S> Fold<S>(Schedule Time, Func<S, B, S> Fold, S Init) =>
        With(Source.Fold(Time, Fold, Init));

    /// <summary>
    /// Fold the values flowing through.  Values are yielded downstream when either the predicate returns
    /// `false`, or the source completes. 
    /// </summary>
    /// <param name="Fold">Binary operator</param>
    /// <param name="Pred">Predicate</param>
    /// <param name="Init">Initial state</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Stream of aggregate states</returns>
    public ConduitT<M, A, S> FoldWhile<S>(Func<S, B, S> Fold, Func<S, B, bool> Pred, S Init) =>
        With(Source.FoldWhile(Fold, Pred, Init));

    /// <summary>
    /// Fold the values flowing through.  Values are yielded downstream when either the predicate returns
    /// `true`, or the source completes. 
    /// </summary>
    /// <param name="Fold">Binary operator</param>
    /// <param name="Pred">Predicate</param>
    /// <param name="Init">Initial state</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Stream of aggregate states</returns>
    public ConduitT<M, A, S> FoldUntil<S>(Func<S, B, S> Fold, Func<S, B, bool> Pred, S Init) =>
        With(Source.FoldUntil(Fold, Pred, Init));

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
    public ConduitT<M, A, S> FoldWhile<S>(
        Schedule Time,
        Func<S, B, S> Fold,
        Func<S, B, bool> Pred,
        S Init) =>
        With(Source.FoldWhile(Time, Fold, Pred, Init));

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
    public ConduitT<M, A, S> FoldUntil<S>(
        Schedule Time,
        Func<S, B, S> Fold,
        Func<S, B, bool> Pred,
        S Init) =>
        With(Source.FoldUntil(Time, Fold, Pred, Init));
    
    /// <summary>
    /// Combine two sinks into a single sink.  The values coming into both sinks are merged into a new sink.  
    /// </summary>
    /// <param name="lhs">Left-hand side</param>
    /// <param name="rhs">Right-hand side</param>
    /// <returns>Merged stream of values</returns>
    public static ConduitT<M, A, B> operator +(Sink<A> lhs, ConduitT<M, A, B> rhs) =>
        rhs with { Sink = lhs.Combine(rhs.Sink) };
    
    /// <summary>
    /// Combine two sources into a single source.  The values of the two sources are concatenated into a new source. 
    /// </summary>
    /// <param name="lhs">Left-hand side</param>
    /// <param name="rhs">Right-hand side</param>
    /// <returns>Conduit with a new Source which is a concatenation of its existing Source and the `rhs` source</returns>
    public static ConduitT<M, A, B> operator +(ConduitT<M, A, B> lhs, SourceT<M, B> rhs) =>
        lhs.Combine(rhs);

    /// <summary>
    /// Combine two sources into a single source.  The value streams are both merged into a new stream, where values are
    /// yielded as they become available on either stream. 
    /// </summary>
    /// <param name="lhs">Left-hand side</param>
    /// <param name="rhs">Right-hand side</param>
    /// <returns>Conduit with a new Source which is a composition of its existing Source and the `rhs` source</returns>
    public static ConduitT<M, A, B> operator |(ConduitT<M, A, B> lhs, SourceT<M, B> rhs) =>
        lhs.Choose(rhs);
    
    /// <summary>
    /// New conduit with all the same properties except the Source, which is provided as the argument.
    /// </summary>
    /// <param name="src">Source to use</param>
    /// <typeparam name="Src">Source bound-value type</typeparam>
    /// <returns>Transformed conduit</returns>
    internal ConduitT<M, A, Src> With<Src>(SourceT<M, Src> src) =>
        new (Sink, src);
    
    /// <summary>
    /// New conduit with all the same properties except the Sink, which is provided as the argument.
    /// </summary>
    /// <param name="sink">Sink to use</param>
    /// <typeparam name="Src">Source bound-value type</typeparam>
    /// <returns>Transformed conduit</returns>
    internal ConduitT<M, Snk, B> With<Snk>(Sink<Snk> sink) =>
        new (sink, Source);
}

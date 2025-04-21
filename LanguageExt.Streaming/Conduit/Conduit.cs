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
/// The `Sink` is a `Cofunctor` and can be mapped using `Comap`, this
/// transforms values before they get to the channel.
/// 
/// The `Source` is a `Monad`, so you can `Map`, `Bind`, `Apply`, in the
/// usual way to map values on their way out.  They manipulate values as they
/// leave the channel. 
///
/// `Source` values can be both merged (using `|` or `Choose`) and
/// concatenated using `+` or `Combine`.
///
/// Incoming `Sink` values can be split and passed to multiple `Sink`
/// channels using (using `+` or `Combine`)
///
/// `ToProducer` and `ToConsumer` allows the `Conduit` components to be used
/// in composed pipe effects.
/// </summary>
/// <param name="Sink">Sink</param>
/// <param name="Source">Source</param>
/// <typeparam name="A">Input value type</typeparam>
/// <typeparam name="B">Output value type</typeparam>
public record Conduit<A, B>(Sink<A> Sink, Source<B> Source)
{
    /// <summary>
    /// Post a value to the Sink
    /// </summary>
    /// <remarks>
    /// Raises `Errors.NoSpaceInSink` if the Sink is full or closed.
    /// </remarks>
    /// <param name="value">Value to post</param>
    /// <returns>IO computation that represents the posting</returns>
    public IO<Unit> Post(A value) =>
        Sink.Post(value);

    /// <summary>
    /// Iterate the stream, flowing values downstream to the reducer, which aggregates a
    /// result value.   
    /// </summary>
    /// <param name="state">State to reduce</param>
    /// <param name="reducer">Reducer</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Reduced state</returns>
    public IO<S> Reduce<S>(S state, ReducerAsync<B, S> reducer) =>
        Source.ReduceAsync(state, reducer);
    
    /// <summary>
    /// Iterate the stream, flowing values downstream to the reducer, which aggregates a
    /// result value.   
    /// </summary>
    /// <param name="state">State to reduce</param>
    /// <param name="reducer">Reducer</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Reduced state</returns>
    public K<M, S> Reduce<M, S>(S state, ReducerAsync<B, S> reducer) 
        where M : MonadIO<M> =>
        Source.ReduceAsync<M, S>(state, reducer);
    
    /// <summary>
    /// Complete and close the Sink
    /// </summary>
    public IO<Unit> Complete() =>
        Sink.Complete();
    
    /// <summary>
    /// Complete and close the Sink with an `Error`
    /// </summary>
    public IO<Unit> Fail(Error Error) =>
        Sink.Fail(Error);
    
    /// <summary>
    /// Functor map
    /// </summary>
    public Conduit<A, C> Map<C>(Func<B, C> f) =>
        new (Sink, Source.Map(f));
    
    /// <summary>
    /// Functor map
    /// </summary>
    public Conduit<A, C> Select<C>(Func<B, C> f) =>
        new (Sink, Source.Map(f));
    
    /// <summary>
    /// Monad bind
    /// </summary>
    public Conduit<A, C> Bind<C>(Func<B, Source<C>> f) =>
        new (Sink, Source.Bind(f));
    
    /// <summary>
    /// Monad bind
    /// </summary>
    public Conduit<A, D> SelectMany<C, D>(Func<B, Source<C>> bind, Func<B, C, D> project) =>
        With(Source.SelectMany(bind, project));
    
    /// <summary>
    /// Applicative apply
    /// </summary>
    public Conduit<A, C> ApplyBack<C>(Source<Func<B, C>> ff) =>
        new (Sink, Source.ApplyBack(ff));
    
    /// <summary>
    /// Contravariant functor map
    /// </summary>
    public Conduit<X, B> Comap<X>(Func<X, A> f) =>
        new (Sink.Comap(f), Source);

    /// <summary>
    /// Convert the `Sink` to a `ConsumerT` pipe component
    /// </summary>
    /// <typeparam name="M">Monad to lift (must support `IO`)</typeparam>
    /// <returns>`ConsumerT`</returns>
    public ConsumerT<A, M, Unit> ToConsumerT<M>()
        where M : MonadIO<M> =>
        Sink.ToConsumerT<M>();

    /// <summary>
    /// Convert the `Sink` to a `Consumer` pipe component
    /// </summary>
    /// <returns>`Consumer`</returns>
    public Consumer<RT, A, Unit> ToConsumer<RT>() =>
        Sink.ToConsumer<RT>();

    /// <summary>
    /// Convert `Source` to a `ProducerT` pipe component
    /// </summary>
    /// <typeparam name="M">Monad to lift (must support `IO`)</typeparam>
    /// <returns>`ProducerT`</returns>
    public ProducerT<B, M, Unit> ToProducerT<M>()
        where M : MonadIO<M> =>
        Source.ToProducerT<M>();

    /// <summary>
    /// Convert `Source` to a `Producer` pipe component
    /// </summary>
    /// <returns>`Producer`</returns>
    public Producer<RT, B, Unit> ToProducer<RT>() =>
        Source.ToProducer<RT>();
    
    /// <summary>
    /// Combine two Sinks: `lhs` and `rhs` into a single Sink that takes incoming
    /// values and then posts to the `lhs` and `rhs` Sinks. 
    /// </summary>
    public Conduit<A, B> Combine(Sink<A> rhs) =>
        this with { Sink = Sink.Combine(rhs) };
    
    /// <summary>
    /// Combine two Sinks: `lhs` and `rhs` into a single Sink that takes incoming
    /// values, maps them to an `(A, B)` tuple, and then posts the first and second
    /// elements to the `lhs` and `rhs` Sinks. 
    /// </summary>
    public Conduit<X, B> Combine<X, C>(Func<X, (A Left, C Right)> f, Sink<C> rhs) =>
        new (Sink.Combine(f, rhs), Source);

    /// <summary>
    /// Combine two Sources into a single Source.  The value streams are both
    /// merged into a new stream.  Values are yielded as they become available.
    /// </summary>
    /// <param name="rhs">Right-hand side</param>
    /// <returns>Merged stream of values</returns>
    public Conduit<A, B> Combine(Source<B> rhs) =>
        this with { Source = Source.Combine(rhs) };
    
    /// <summary>
    /// Choose a value from the first `Source` to successfully yield 
    /// </summary>
    /// <param name="rhs"></param>
    /// <returns>Value from this `Source` if there are any available, if not, from `rhs`.  If
    /// `rhs` is also empty, then `Errors.SourceClosed` is raised</returns>
    public Conduit<A, B> Choose(Source<B> rhs) =>
        this with { Source = Source.Choose(rhs) };
        
    /// <summary>
    /// Transform with a transducer
    /// </summary>
    /// <param name="transducer">Transducer to use to transform</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>Transformed source</returns>
    public Conduit<A, C> Transform<C>(Transducer<B, C> transducer) =>
        With(Source.Transform(transducer)); 
    
    /// <summary>
    /// Filter values.  Yielding downstream when `true`
    /// </summary>
    /// <param name="f">Filter function</param>
    /// <returns>SourceT where the only values yield are those that pass the predicate</returns>
    public Conduit<A, B> Where(Func<B, bool> f) =>
        With(Source.Filter(f)); 
    
    /// <summary>
    /// Filter values.  Yielding downstream when `true`
    /// </summary>
    /// <param name="f">Filter function</param>
    /// <returns>SourceT where the only values yield are those that pass the predicate</returns>
    public Conduit<A, B> Filter(Func<B, bool> f) =>
        With(Source.Filter(f)); 

    /// <summary>
    /// Zip two sources into one
    /// </summary>
    /// <param name="second">Stream to zip with this one</param>
    /// <typeparam name="B">Bound value-type of the stream to zip with this one</typeparam>
    /// <returns>Stream of values where the items from two streams are paired together</returns>
    public Conduit<A, (B First, C Second)> Zip<C>(Source<C> second) =>
        With(Source.Zip(second)); 

    /// <summary>
    /// Zip three sources into one
    /// </summary>
    /// <param name="second">Stream to zip with this one</param>
    /// <param name="third">Stream to zip with this one</param>
    /// <typeparam name="B">Bound value-type of the stream to zip with this one</typeparam>
    /// <returns>Stream of values where the items from two streams are paired together</returns>
    public Conduit<A, (B First, C Second, D Third)> Zip<C, D>(Source<C> second, Source<D> third) =>
        With(Source.Zip(second, third)); 

    /// <summary>
    /// Zip three sources into one
    /// </summary>
    /// <param name="second">Stream to zip with this one</param>
    /// <param name="third">Stream to zip with this one</param>
    /// <param name="fourth">Stream to zip with this one</param>
    /// <typeparam name="B">Bound value-type of the stream to zip with this one</typeparam>
    /// <returns>Stream of values where the items from two streams are paired together</returns>
    public Conduit<A, (B First, C Second, D Third, E Fourth)> Zip<C, D, E>(
        Source<C> second, 
        Source<D> third, 
        Source<E> fourth) =>
        With(Source.Zip(second, third, fourth)); 

    /// <summary>
    /// Skip items in the source
    /// </summary>
    /// <param name="amount">Amount to skip</param>
    /// <returns>Transformed source</returns>
    public Conduit<A, B> Skip(int amount) =>
        With(Source.Skip(amount)); 

    /// <summary>
    /// Limit the number of items processed 
    /// </summary>
    /// <param name="amount">Number to take</param>
    /// <returns>Transformed source</returns>
    public Conduit<A, B> Take(int amount) =>
        With(Source.Take(amount)); 

    /// <summary>
    /// Fold the values flowing through.  A value is only yielded downstream upon completion of the stream.
    /// </summary>
    /// <param name="Fold">Binary operator</param>
    /// <param name="Init">Initial state</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Stream of aggregate state</returns>
    public Conduit<A, S> Fold<S>(Func<S, B, S> Fold, S Init) =>
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
    public Conduit<A, S> Fold<S>(Schedule Time, Func<S, B, S> Fold, S Init) =>
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
    public Conduit<A, S> FoldWhile<S>(Func<S, B, S> Fold, Func<S, B, bool> Pred, S Init) =>
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
    public Conduit<A, S> FoldUntil<S>(Func<S, B, S> Fold, Func<S, B, bool> Pred, S Init) =>
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
    public Conduit<A, S> FoldWhile<S>(
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
    public Conduit<A, S> FoldUntil<S>(
        Schedule Time,
        Func<S, B, S> Fold,
        Func<S, B, bool> Pred,
        S Init) =>
        With(Source.FoldUntil(Time, Fold, Pred, Init));
    
    /// <summary>
    /// Combine two Sinks into a single Source.  The values are both
    /// merged into a new Sink.  
    /// </summary>
    /// <param name="lhs">Left-hand side</param>
    /// <param name="rhs">Right-hand side</param>
    /// <returns>Merged stream of values</returns>
    public static Conduit<A, B> operator +(Sink<A> lhs, Conduit<A, B> rhs) =>
        rhs with { Sink = lhs.Combine(rhs.Sink) };
    
    /// <summary>
    /// Combine two Sources into a single Source.  The value streams are both
    /// merged into a new stream.  Values are yielded as they become available.
    /// </summary>
    /// <param name="lhs">Left-hand side</param>
    /// <param name="rhs">Right-hand side</param>
    /// <returns>Merged stream of values</returns>
    public static Conduit<A, B> operator +(Conduit<A, B> lhs, Source<B> rhs) =>
        lhs.Combine(rhs);

    /// <summary>
    /// Choose a value from the first `Source` to successfully yield 
    /// </summary>
    /// <param name="lhs">Left-hand side</param>
    /// <param name="rhs">Right-hand side</param>
    /// <returns>Value from the `lhs` `Source` if there are any available, if not, from `rhs`.  If
    /// `rhs` is also empty, then `Errors.SourceChannelClosed` is raised</returns>
    public static Conduit<A, B> operator |(Conduit<A, B> lhs, Source<B> rhs) =>
        lhs.Choose(rhs);
    
    /// <summary>
    /// New conduit with all the same properties except the Source, which is provided as the argument.
    /// </summary>
    /// <param name="src">Source to use</param>
    /// <typeparam name="Src">Source bound-value type</typeparam>
    /// <returns>Transformed conduit</returns>
    internal Conduit<A, Src> With<Src>(Source<Src> src) =>
        new (Sink, src);
    
    /// <summary>
    /// New conduit with all the same properties except the Sink, which is provided as the argument.
    /// </summary>
    /// <param name="sink">Sink to use</param>
    /// <typeparam name="Src">Source bound-value type</typeparam>
    /// <returns>Transformed conduit</returns>
    internal Conduit<Snk, B> With<Snk>(Sink<Snk> sink) =>
        new (sink, Source);
}

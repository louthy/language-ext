using System;
using System.Collections.Concurrent;
using System.Threading.Channels;
using LanguageExt.Common;
using LanguageExt.Pipes;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

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
/// <typeparam name="B">Buffer value type</typeparam>
/// <typeparam name="C">Output value type</typeparam>
internal class Conduit<A, B, C> : Conduit<A, C>
{
    readonly Transducer<A, B> sink;
    readonly Channel<B> channel;
    readonly Transducer<B, C> source;

    internal Conduit(Transducer<A, B> sink, Channel<B> channel, Transducer<B, C> source)
    {
        this.sink = sink;
        this.channel = channel;
        this.source = source;
        Source = LanguageExt.Source.lift(channel).Transform(source);
    }

    /// <summary>
    /// Post a value to the `Sink`
    /// </summary>
    /// <remarks>
    /// Raises `Errors.SinkFull` if the `Sink` is full or closed.
    /// </remarks>
    /// <param name="value">Value to post</param>
    /// <returns>IO computation that represents the posting</returns>
    public override IO<Unit> Post(A value) =>
        sink.Reduce<Unit>(
            (_, x) => 
                IO.liftVAsync(async e =>
                          {
                              if (await channel.Writer.WaitToWriteAsync(e.Token))
                              {
                                  await channel.Writer.WriteAsync(x);
                                  return Reduced.Unit;
                              }
                              else
                              {
                                  throw Errors.SinkFull;
                              }
                          }))
                    (unit, value)
            .Map(r => r.Value);

    /// <summary>
    /// Complete and close the Sink
    /// </summary>
    public override IO<Unit> Complete() =>
        IO.lift(_ => channel.Writer.TryComplete()).Map(unit);

    /// <summary>
    /// Complete and close the Sink with an `Error`
    /// </summary>
    public override IO<Unit> Fail(Error Error) =>
        IO.lift(_ => channel.Writer.TryComplete(Error)).Map(unit);

    /// <summary>
    /// Iterate the stream, flowing values downstream to the reducer, which aggregates a
    /// result value.   
    /// </summary>
    /// <param name="state">State to reduce</param>
    /// <param name="reducer">Reducer</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Reduced state</returns>
    public override IO<S> Reduce<S>(S state, ReducerIO<C, S> reducer) =>
        IO.liftVAsync(async e =>
                      {
                          while (await channel.Reader.WaitToReadAsync(e.Token))
                          {
                              var value = await channel.Reader.ReadAsync();
                              switch (await source.Reduce(reducer)(state, value).RunAsync(e))
                              {
                                  case { Continue: true, Value: var nstate }:
                                    state = nstate;
                                    break;      
                                  
                                  case { Value: var nstate }:
                                      return nstate;
                              }
                          }
                          return state;
                      });
    
    /// <summary>
    /// Iterate the stream, flowing values downstream to the reducer, which aggregates a
    /// result value.   
    /// </summary>
    /// <param name="state">State to reduce</param>
    /// <param name="reducer">Reducer</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Reduced state</returns>
    public override K<M, S> Reduce<M, S>(S state, ReducerIO<C, S> reducer) =>
        M.LiftIOMaybe(IO.liftVAsync(async e =>
                               {
                                   while (await channel.Reader.WaitToReadAsync(e.Token))
                                   {
                                       var value = await channel.Reader.ReadAsync();
                                       switch (await source.Reduce(reducer)(state, value).RunAsync(e))
                                       {
                                           case { Continue: true, Value: var nstate }:
                                               state = nstate;
                                               break;

                                           case { Value: var nstate }:
                                               return nstate;
                                       }
                                   }
                                   return state;
                               }));
    
    /// <summary>
    /// Functor map
    /// </summary>
    public override Conduit<A, D> Map<D>(Func<C, D> f) =>
        With(source.Map(f));
    
    /// <summary>
    /// Contravariant functor map
    /// </summary>
    public override Conduit<X, C> Comap<X>(Func<X, A> f) =>
        With (sink.Comap(f));

    /// <summary>
    /// Access the underlying `Sink` for more direct manipulation.  
    /// </summary>
    /// <returns></returns>
    public override Sink<A> Sink => 
        LanguageExt.Sink.lift(channel).Comap(sink);

    /// <summary>
    /// Access the underlying `Source` for more direct manipulation.  
    /// </summary>
    /// <returns></returns>
    public override Source<C> Source { get; }

    /// <summary>
    /// Convert the `Sink` to a `ConsumerT` pipe component
    /// </summary>
    /// <typeparam name="M">Monad to lift (must support `IO`)</typeparam>
    /// <returns>`ConsumerT`</returns>
    public override ConsumerT<A, M, Unit> ToConsumerT<M>() =>
        Sink.ToConsumerT<M>();

    /// <summary>
    /// Convert the `Sink` to a `Consumer` pipe component
    /// </summary>
    /// <returns>`Consumer`</returns>
    public override Consumer<RT, A, Unit> ToConsumer<RT>() =>
        Sink.ToConsumer<RT>();

    /// <summary>
    /// Convert `Source` to a `ProducerT` pipe component
    /// </summary>
    /// <typeparam name="M">Monad to lift (must support `IO`)</typeparam>
    /// <returns>`ProducerT`</returns>
    public override ProducerT<C, M, Unit> ToProducerT<M>() =>
        Source.ToProducerT<M>();

    /// <summary>
    /// Convert `Source` to a `Producer` pipe component
    /// </summary>
    /// <returns>`Producer`</returns>
    public override Producer<RT, C, Unit> ToProducer<RT>() =>
        Source.ToProducer<RT>();
        
    /// <summary>
    /// Transform with a transducer
    /// </summary>
    /// <param name="transducer">Transducer to use to transform</param>
    /// <returns>Transformed source</returns>
    public override Conduit<A, D> Transform<D>(Transducer<C, D> transducer) =>
        With(source.Compose(transducer)); 
        
    /// <summary>
    /// Co-transform with a transducer
    /// </summary>
    /// <param name="transducer">Transducer to use to transform</param>
    /// <returns>Transformed source</returns>
    public override Conduit<X, C> CoTransform<X>(Transducer<X, A> transducer) =>
        With(transducer.Compose(sink)); 
    
    /// <summary>
    /// Filter values.  Yielding downstream when `true`
    /// </summary>
    /// <param name="f">Filter function</param>
    /// <returns>SourceT where the only values yield are those that pass the predicate</returns>
    public override Conduit<A, C> Filter(Func<C, bool> f) =>
        With(source.Filter(f)); 

    /// <summary>
    /// Skip items in the source
    /// </summary>
    /// <param name="amount">Amount to skip</param>
    /// <returns>Transformed source</returns>
    public override Conduit<A, C> Skip(int amount) =>
        With(source.Skip(amount)); 

    /// <summary>
    /// Limit the number of items processed 
    /// </summary>
    /// <param name="amount">Number to take</param>
    /// <returns>Transformed source</returns>
    public override Conduit<A, C> Take(int amount) =>
        With(source.Take(amount)); 

    /// <summary>
    /// Fold the values flowing through.  Values are yielded downstream when either the predicate returns
    /// `false`, or the source completes. 
    /// </summary>
    /// <param name="Fold">Binary operator</param>
    /// <param name="Pred">Predicate</param>
    /// <param name="Init">Initial state</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Stream of aggregate states</returns>
    public override Conduit<A, S> FoldWhile<S>(Func<S, C, S> Fold, Func<S, C, bool> Pred, S Init) =>
        With(source.FoldWhile(Fold, Pred, Init));

    /// <summary>
    /// Fold the values flowing through.  Values are yielded downstream when either the predicate returns
    /// `true`, or the source completes. 
    /// </summary>
    /// <param name="Fold">Binary operator</param>
    /// <param name="Pred">Predicate</param>
    /// <param name="Init">Initial state</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Stream of aggregate states</returns>
    public override Conduit<A, S> FoldUntil<S>(Func<S, C, S> Fold, Func<S, C, bool> Pred, S Init) =>
        With(source.FoldUntil(Fold, Pred, Init));

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
    public override Conduit<A, S> FoldWhile<S>(
        Schedule Time,
        Func<S, C, S> Fold,
        Func<S, C, bool> Pred,
        S Init) =>
        With(source.FoldWhile(Time, Fold, Pred, Init));

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
    public override Conduit<A, S> FoldUntil<S>(
        Schedule Time,
        Func<S, C, S> Fold,
        Func<S, C, bool> Pred,
        S Init) =>
        With(source.FoldUntil(Time, Fold, Pred, Init));
    

    /// <summary>
    /// New conduit with all the same properties except the Source, which is provided as the argument.
    /// </summary>
    /// <param name="src">Source to use</param>
    /// <typeparam name="Src">Source bound-value type</typeparam>
    /// <returns>Transformed conduit</returns>
    internal Conduit<A, B, Src> With<Src>(Transducer<B, Src> src) =>
        new (sink, channel, src);
    
    /// <summary>
    /// New conduit with all the same properties except the Sink, which is provided as the argument.
    /// </summary>
    /// <param name="snk">Sink to use</param>
    /// <typeparam name="Src">Source bound-value type</typeparam>
    /// <returns>Transformed conduit</returns>
    internal Conduit<Snk, B, C> With<Snk>(Transducer<Snk, B> snk) =>
        new (snk, channel, source);
}

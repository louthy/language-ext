using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Pipes;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

class ConduitT<M, A, B, C> : ConduitT<M, A, C>
    where M : MonadIO<M>, Monad<M>, Alternative<M>
{
    readonly TransducerM<M, A, B> sink;
    readonly Channel<K<M, B>> channel;
    readonly TransducerM<M, B, C> source;
    
    internal ConduitT(TransducerM<M, A, B> sink, Channel<K<M, B>> channel, TransducerM<M, B, C> source)
    {
        this.sink = sink;
        this.channel = channel;
        this.source = source;
    }

    /// <summary>
    /// Post a value to the `Sink`
    /// </summary>
    /// <remarks>
    /// Raises `Errors.SinkFull` if the `Sink` is full or closed.
    /// </remarks>
    /// <param name="value">Value to post</param>
    /// <returns>IO computation that represents the posting</returns>
    public override K<M, Unit> Post(A value) =>
        sink.Reduce<Unit>((_, b) => Write(M.Pure(b)))(unit, value);

    /// <summary>
    /// Post a value to the `Sink`
    /// </summary>
    /// <remarks>
    /// Raises `Errors.SinkFull` if the `Sink` is full or closed.
    /// </remarks>
    /// <param name="ma">Value to post</param>
    /// <returns>IO computation that represents the posting</returns>
    public override K<M, Unit> PostM(K<M, A> ma) =>
        ma.Bind(Post);
    
    /// <summary>
    /// Complete and close the Sink
    /// </summary>
    public override K<M, Unit> Complete() =>
        M.LiftIO(IO.lift(_ => channel.Writer.TryComplete()).Map(unit));

    /// <summary>
    /// Complete and close the Sink with an `Error`
    /// </summary>
    public override K<M, Unit> Fail(Error Error) =>
        M.LiftIO(IO.lift(_ => channel.Writer.TryComplete(Error)).Map(unit));

    /// <summary>
    /// Iterate the stream, flowing values downstream to the reducer, which aggregates a
    /// result value.   
    /// </summary>
    /// <param name="state">State to reduce</param>
    /// <param name="reducer">Reducer</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Reduced state</returns>
    public override K<M, S> Reduce<S>(S state, ReducerM<M, C, S> reducer)
    {
        return M.LiftIO(IO.lift(e => go(state, e.Token))).Flatten();
        
        K<M, S> go(S s0, CancellationToken token)
        {
            if(token.IsCancellationRequested) return M.Pure(s0);
            if (channel.Reader.WaitToReadAsync(token).GetAwaiter().GetResult())
            {
                var mb = channel.Reader.ReadAsync(token).GetAwaiter().GetResult();
                return mb.Bind(b => source.Reduce<S>((s1, c) => reducer(s1, c)
                                                               .Bind(s2 => go(s2, token))
                                                               .Choose(M.Pure(s0)))(s0, b));
            }
            else
            {
                return M.Pure(s0);
            }
        }
    }

    /// <summary>
    /// Functor map
    /// </summary>
    public override ConduitT<M, A, D> Map<D>(Func<C, D> f) =>
        With(source.Compose(TransducerM.map<M, C, D>(f)));
    
    /// <summary>
    /// Contravariant functor map
    /// </summary>
    public override ConduitT<M, X, C> Comap<X>(Func<X, A> f) =>
        With (TransducerM.map<M, X, A>(f).Compose(sink));

    /// <summary>
    /// Access the underlying `Sink` for more direct manipulation.  
    /// </summary>
    /// <returns></returns>
    public override SinkT<M, A> Sink => 
        SinkT.lift(channel).Comap(sink);

    /// <summary>
    /// Access the underlying `Source` for more direct manipulation.  
    /// </summary>
    /// <returns></returns>
    public override SourceT<M, C> Source => 
        SourceT.liftM(channel).Transform(source);

    /// <summary>
    /// Convert the `Sink` to a `ConsumerT` pipe component
    /// </summary>
    /// <typeparam name="M">Monad to lift (must support `IO`)</typeparam>
    /// <returns>`ConsumerT`</returns>
    public override ConsumerT<A, M, Unit> ToConsumerT() =>
        Sink.ToConsumerT();

    /// <summary>
    /// Convert `Source` to a `ProducerT` pipe component
    /// </summary>
    /// <typeparam name="M">Monad to lift (must support `IO`)</typeparam>
    /// <returns>`ProducerT`</returns>
    public override ProducerT<C, M, Unit> ToProducerT() =>
        Source.ToProducerT();
        
    /// <summary>
    /// Transform with a transducer
    /// </summary>
    /// <param name="transducer">Transducer to use to transform</param>
    /// <returns>Transformed source</returns>
    public override ConduitT<M, A, D> Transform<D>(TransducerM<M, C, D> transducer) =>
        With(source.Compose(transducer)); 
        
    /// <summary>
    /// Co-transform with a transducer
    /// </summary>
    /// <param name="transducer">Transducer to use to transform</param>
    /// <returns>Transformed source</returns>
    public override ConduitT<M, X, C> CoTransform<X>(TransducerM<M, X, A> transducer) =>
        With(transducer.Compose(sink)); 
    
    /// <summary>
    /// Filter values.  Yielding downstream when `true`
    /// </summary>
    /// <param name="f">Filter function</param>
    /// <returns>SourceT where the only values yield are those that pass the predicate</returns>
    public override ConduitT<M, A, C> Filter(Func<C, bool> f) =>
        With(source.Filter(f)); 

    /// <summary>
    /// Skip items in the source
    /// </summary>
    /// <param name="amount">Amount to skip</param>
    /// <returns>Transformed source</returns>
    public override ConduitT<M, A, C> Skip(int amount) =>
        With(source.Skip(amount)); 

    /// <summary>
    /// Limit the number of items processed 
    /// </summary>
    /// <param name="amount">Number to take</param>
    /// <returns>Transformed source</returns>
    public override ConduitT<M, A, C> Take(int amount) =>
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
    public override ConduitT<M, A, S> FoldWhile<S>(Func<S, C, S> Fold, Func<S, C, bool> Pred, S Init) =>
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
    public override ConduitT<M, A, S> FoldUntil<S>(Func<S, C, S> Fold, Func<S, C, bool> Pred, S Init) =>
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
    public override ConduitT<M, A, S> FoldWhile<S>(
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
    public override ConduitT<M, A, S> FoldUntil<S>(
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
    ConduitT<M, A, B, Src> With<Src>(TransducerM<M, B, Src> src) =>
        new (sink, channel, src);
    
    /// <summary>
    /// New conduit with all the same properties except the Sink, which is provided as the argument.
    /// </summary>
    /// <param name="sink">Sink to use</param>
    /// <typeparam name="Src">Source bound-value type</typeparam>
    /// <returns>Transformed conduit</returns>
    ConduitT<M, Snk, B, C> With<Snk>(TransducerM<M, Snk, B> sink) =>
        new (sink, channel, source);    

    K<M, Unit> Write(K<M, B> value)
    {
        return M.LiftIO(IO.liftVAsync(e => go(e, value, channel)));
        
        static async ValueTask<Unit> go(EnvIO e, K<M, B> mb, Channel<K<M, B>> ch)
        {
            if (await ch.Writer.WaitToWriteAsync(e.Token))
            {
                await ch.Writer.WriteAsync(mb);
                return unit;
            }
            else
            {
                throw Errors.SinkFull;
            }
        }
    }
}

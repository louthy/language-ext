using System;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.Pipes;

/// <summary>
/// `PipeT` monad-transformer base-type for Pipes streaming components:
///
///   * `ProducerT` is a `PipeT` with the `IN` 'closed off' with `Unit`
///   * `ConsumerT` is a `PipeT` with the `OUT` 'closed off' with `Void`
///   * `EffectT` is a `PipeT` with the `IN` and `OUT` 'closed off' with `Unit` upstream and `Void` downstream
/// 
/// </summary>
/// <typeparam name="IN">Type of values to be consumed</typeparam>
/// <typeparam name="OUT">Type of values to be produced</typeparam>
/// <typeparam name="M">Lifted monad type</typeparam>
/// <typeparam name="A">Bound value type</typeparam>
public abstract record PipeT<IN, OUT, M, A> : K<PipeT<IN, OUT, M>, A>
    where M : MonadIO<M>
{
    [Pure]
    public PipeT<IN, OUT1, M, A> Compose<OUT1>(PipeT<OUT, OUT1, M, A> rhs) =>
        rhs.PairEachAwaitWithYield(_ => this);

    [Pure]
    public ConsumerT<IN, M, A> Compose(ConsumerT<OUT, M, A> rhs) =>
        rhs.Proxy.PairEachAwaitWithYield(_ => this);

    [Pure]
    public static PipeT<IN, OUT, M, A> operator | (PipeT<IN, OUT, M, A> lhs, PipeT<OUT, OUT, M, A> rhs) =>
        lhs.Compose(rhs);

    [Pure]
    public static ProducerT<OUT, M, A> operator | (ProducerT<IN, M, A> lhs, PipeT<IN, OUT, M, A> rhs) =>
        lhs.Compose(rhs);

    [Pure]
    public static ConsumerT<IN, M, A> operator | (PipeT<IN, OUT, M, A> lhs, ConsumerT<OUT, M, A> rhs) =>
        lhs.Compose(rhs.Proxy);
    
    [Pure]
    public static implicit operator PipeT<IN, OUT, M, A>(Pure<A> rhs) =>
        PipeT.pure<IN, OUT, M, A>(rhs.Value);
    
    [Pure]
    public abstract PipeT<IN, OUT, M, B> Map<B>(Func<A, B> f);
    
    [Pure]
    public abstract PipeT<IN, OUT, M, B> MapM<B>(Func<K<M, A>, K<M, B>> f);
    
    [Pure]
    public abstract PipeT<IN, OUT, M, B> ApplyBack<B>(PipeT<IN, OUT, M, Func<A, B>> ff);
    
    [Pure]
    public abstract PipeT<IN, OUT, M, B> Action<B>(PipeT<IN, OUT, M, B> fb);
    
    [Pure]
    public abstract PipeT<IN, OUT, M, B> Bind<B>(Func<A, PipeT<IN, OUT, M, B>> f);
    
    [Pure]
    public PipeT<IN, OUT, M, B> Bind<B>(Func<A, IO<B>> f) =>
        Bind(x => PipeT.liftIO<IN, OUT, M, B>(f(x)));
    
    [Pure]
    public PipeT<IN, OUT, M, B> Bind<B>(Func<A, K<M, B>> f) =>
        Bind(x => PipeT.liftM<IN, OUT, M, B>(f(x)));
    
    [Pure]
    public PipeT<IN, OUT, M, B> Bind<B>(Func<A, Pure<B>> f) =>
        Map(x => f(x).Value);
    
    [Pure]
    public PipeT<IN, OUT, M, B> Bind<B>(Func<A, Lift<B>> f) =>
        Map(x => f(x).Function());
    
    [Pure]
    public PipeT<IN, OUT, M, B> BindAsync<B>(Func<A, ValueTask<PipeT<IN, OUT, M, B>>> f) =>
        Bind(x => PipeT.liftT(f(x)));

    [Pure]
    public PipeT<IN, OUT, M, B> Select<B>(Func<A, B> f) =>
        Map(f);
   
    [Pure]
    public PipeT<IN, OUT, M, C> SelectMany<B, C>(Func<A, PipeT<IN, OUT, M, B>> f, Func<A, B, C> g) =>
        Bind(x => f(x).Map(y => g(x, y)));
   
    [Pure]
    public PipeT<IN, OUT, M, C> SelectMany<B, C>(Func<A, K<M, B>> f, Func<A, B, C> g) =>
        Bind(x => f(x).Map(y => g(x, y)));
   
    [Pure]
    public PipeT<IN, OUT, M, C> SelectMany<B, C>(Func<A, IO<B>> f, Func<A, B, C> g) =>
        Bind(x => f(x).Map(y => g(x, y)));
   
    [Pure]
    public PipeT<IN, OUT, M, C> SelectMany<B, C>(Func<A, Pure<B>> f, Func<A, B, C> g) =>
        Map(x => g(x, f(x).Value));
   
    [Pure]
    public PipeT<IN, OUT, M, C> SelectMany<B, C>(Func<A, Lift<B>> f, Func<A, B, C> g) =>
        Map(x => g(x, f(x).Function()));
    
    [Pure]
    internal abstract PipeT<IN1, OUT, M, A> ReplaceAwait<IN1>(Func<PipeT<IN1, OUT, M, IN>> producer);
    
    [Pure]
    internal abstract PipeT<IN, OUT1, M, A> ReplaceYield<OUT1>(Func<OUT, PipeT<IN, OUT1, M, Unit>> consumer);
    
    [Pure]
    internal abstract PipeT<IN1, OUT, M, A> PairEachAwaitWithYield<IN1>(Func<Unit, PipeT<IN1, IN, M, A>> producer);
    
    [Pure]
    internal abstract PipeT<IN, OUT1, M, A> PairEachYieldWithAwait<OUT1>(Func<OUT, PipeT<OUT, OUT1, M, A>> consumer);
                
    /// <summary>
    /// Fold the given pipe until the `Schedule` completes.
    /// Once complete, the pipe yields the aggregated value downstream.
    /// </summary>
    /// <param name="Time">Schedule to run each item</param>
    /// <param name="Fold">Fold function</param>
    /// <param name="Init">Initial state</param>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public PipeT<IN, OUT, M, Unit> Fold(
        Schedule Time,
        Func<OUT, A, OUT> Fold, 
        OUT Init) =>
        PipeT.fold(Time, Fold, Init, this);

    /// <summary>
    /// Fold the given pipe until the predicate is `true`.  Once `true` the pipe yields the
    /// aggregated value downstream.
    /// </summary>
    /// <param name="Fold">Fold function</param>
    /// <param name="Pred">Until predicate</param>
    /// <param name="Init">Initial state</param>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public PipeT<IN, OUT, M, Unit> FoldUntil(
        Func<OUT, A, OUT> Fold, 
        Func<(OUT State, A Value), bool> Pred, 
        OUT Init) =>
        PipeT.foldUntil(Fold, Pred, Init, this);
        
    /// <summary>
    /// Fold the given pipe until the predicate is `true` or the `Schedule` completes.
    /// Once `true`, or completed, the pipe yields the aggregated value downstream.
    /// </summary>
    /// <param name="Time">Schedule to run each item</param>
    /// <param name="Fold">Fold function</param>
    /// <param name="Pred">Until predicate</param>
    /// <param name="Init">Initial state</param>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public PipeT<IN, OUT, M, Unit> FoldUntil(
        Schedule Time,
        Func<OUT, A, OUT> Fold, 
        Func<(OUT State, A Value), bool> Pred, 
        OUT Init) =>
        PipeT.foldUntil(Time, Fold, Pred, Init, this);
        
    /// <summary>
    /// Fold the given pipe while the predicate is `true`.  Once `false` the pipe yields the
    /// aggregated value downstream.
    /// </summary>
    /// <param name="Fold">Fold function</param>
    /// <param name="Pred">Until predicate</param>
    /// <param name="Init">Initial state</param>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public PipeT<IN, OUT, M, Unit> FoldWhile(
        Func<OUT, A, OUT> Fold, 
        Func<(OUT State, A Value), bool> Pred, 
        OUT Init) =>
        PipeT.foldWhile(Fold, Pred, Init, this);
        
    /// <summary>
    /// Fold the given pipe while the predicate is `true` or the `Schedule` completes.
    /// Once `false`, or completed, the pipe yields the aggregated value downstream.
    /// </summary>
    /// <param name="Time">Schedule to run each item</param>
    /// <param name="Fold">Fold function</param>
    /// <param name="Pred">Until predicate</param>
    /// <param name="Init">Initial state</param>
    /// <typeparam name="IN">Stream value to consume</typeparam>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public PipeT<IN, OUT, M, Unit> FoldWhile(
        Schedule Time,
        Func<OUT, A, OUT> Fold, 
        Func<(OUT State, A Value), bool> Pred, 
        OUT Init) =>
        PipeT.foldWhile(Time, Fold, Pred, Init, this);
    
    [Pure]
    internal virtual K<M, A> Run()
    {
        var t = RunAsync(CancellationToken.None);
        if(t.IsCompleted) return t.Result;
        return t.GetAwaiter().GetResult();
    }

    [Pure]
    internal abstract ValueTask<K<M, A>> RunAsync(CancellationToken token);
}

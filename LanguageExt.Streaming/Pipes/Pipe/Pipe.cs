using System;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.Pipes;

/// <summary>
/// `Pipe` monad-transformer base-type for `Eff`-based streaming components:
///
///   * `Producer` is a `Pipe` with the `IN` 'closed off' with `Unit`
///   * `Consumer` is a `Pipe` with the `OUT` 'closed off' with `Void`
///   * `Effect` is a `Pipe` with the `IN` and `OUT` 'closed off' with `Unit` upstream and `Void` downstream
/// 
/// </summary>
/// <remarks>
/// Unlike the general purpose `PipeT`, which will lift any monad, this type only lifts the `Eff` monad.
/// </remarks>
/// <typeparam name="IN">Type of values to be consumed</typeparam>
/// <typeparam name="OUT">Type of values to be produced</typeparam>
/// <typeparam name="M">Lifted monad type</typeparam>
/// <typeparam name="A">Bound value type</typeparam>
public record Pipe<RT, IN, OUT, A>(PipeT<IN, OUT, Eff<RT>, A> Proxy) : K<Pipe<RT, IN, OUT>, A>
{
    [Pure]
    public Pipe<RT, IN, OUT1, A> Compose<OUT1>(Pipe<RT, OUT, OUT1, A> rhs) =>
        Proxy.Compose(rhs.Proxy);
    [Pure]
    public Pipe<RT, IN, OUT1, A> Compose<OUT1>(PipeT<OUT, OUT1, Eff<RT>, A> rhs) =>
        Proxy.Compose(rhs);

    [Pure]
    public Consumer<RT, IN, A> Compose(Consumer<RT, OUT, A> rhs) =>
        Proxy.Compose(rhs.Proxy);

    [Pure]
    public Consumer<RT, IN, A> Compose(ConsumerT<OUT, Eff<RT>, A> rhs) =>
        Proxy.Compose(rhs.Proxy);
    
    [Pure]
    public static Pipe<RT, IN, OUT, A> operator | (Pipe<RT, IN, OUT, A> lhs, Pipe<RT, OUT, OUT, A> rhs) =>
        lhs.Compose(rhs);

    [Pure]
    public static Pipe<RT, IN, OUT, A> operator | (Pipe<RT, IN, OUT, A> lhs, PipeT<OUT, OUT, Eff<RT>, A> rhs) =>
        lhs.Compose(rhs);

    [Pure]
    public static Producer<RT, OUT, A> operator | (Producer<RT, IN, A> lhs, Pipe<RT, IN, OUT, A> rhs) =>
        lhs.Compose(rhs);

    [Pure]
    public static Producer<RT, OUT, A> operator | (ProducerT<IN, Eff<RT>, A> lhs, Pipe<RT, IN, OUT, A> rhs) =>
        lhs.Compose(rhs.Proxy).Proxy;

    [Pure]
    public static Consumer<RT, IN, A> operator | (Pipe<RT, IN, OUT, A> lhs, Consumer<RT, OUT, A> rhs) =>
        lhs.Compose(rhs);

    [Pure]
    public static Consumer<RT, IN, A> operator | (Pipe<RT, IN, OUT, A> lhs, ConsumerT<OUT, Eff<RT>, A> rhs) =>
        lhs.Compose(rhs);
    
    [Pure]
    public static implicit operator Pipe<RT, IN, OUT, A>(Pure<A> rhs) =>
        Pipe.pure<RT, IN, OUT, A>(rhs.Value);

    [Pure]
    public static implicit operator Pipe<RT, IN, OUT, A>(PipeT<IN, OUT, Eff<RT>, A> rhs) =>
        new(rhs);

    [Pure]
    public Pipe<RT, IN, OUT, B> Map<B>(Func<A, B> f) =>
        Proxy.Map(f);

    [Pure]
    public Pipe<RT, IN, OUT, B> MapM<B>(Func<Eff<RT, A>, Eff<RT, B>> f) =>
        Proxy.MapM(mx => f(mx.As()));
    
    [Pure]
    public Pipe<RT, IN, OUT, B> ApplyBack<B>(Pipe<RT, IN, OUT, Func<A, B>> ff) =>
        Proxy.ApplyBack(ff.Proxy);
    
    [Pure]
    public Pipe<RT, IN, OUT, B> Action<B>(Pipe<RT, IN, OUT, B> fb) =>
        Proxy.Action(fb.Proxy);
    
    [Pure]
    public Pipe<RT, IN, OUT, B> Bind<B>(Func<A, Pipe<RT, IN, OUT, B>> f) =>
        Proxy.Bind(x => f(x).Proxy);
    
    [Pure]
    public Pipe<RT, IN, OUT, B> Bind<B>(Func<A, IO<B>> f) =>
        Proxy.Bind(f);
    
    [Pure]
    public Pipe<RT, IN, OUT, B> Bind<B>(Func<A, K<Eff<RT>, B>> f) =>
        Proxy.Bind(f);
    
    [Pure]
    public Pipe<RT, IN, OUT, B> Bind<B>(Func<A, Pure<B>> f) =>
        Proxy.Bind(f);

    [Pure]
    public Pipe<RT, IN, OUT, B> Bind<B>(Func<A, Lift<B>> f) =>
        Proxy.Bind(f);
    
    [Pure]
    public Pipe<RT, IN, OUT, B> BindAsync<B>(Func<A, ValueTask<Pipe<RT, IN, OUT, B>>> f) =>
        Proxy.BindAsync(async x => (await f(x)).Proxy);
    
    [Pure]
    public Pipe<RT, IN, OUT, B> MapIO<B>(Func<IO<A>, IO<B>> f) =>
        Proxy.MapIO(f);

    [Pure]
    public Pipe<RT, IN, OUT, B> Select<B>(Func<A, B> f) =>
        Map(f);
   
    [Pure]
    public Pipe<RT, IN, OUT, C> SelectMany<B, C>(Func<A, Pipe<RT, IN, OUT, B>> f, Func<A, B, C> g) =>
        Proxy.SelectMany(x => f(x).Proxy, g);
   
    [Pure]
    public Pipe<RT, IN, OUT, C> SelectMany<B, C>(Func<A, K<Eff<RT>, B>> f, Func<A, B, C> g) =>
        Proxy.SelectMany(f, g);
   
    [Pure]
    public Pipe<RT, IN, OUT, C> SelectMany<B, C>(Func<A, IO<B>> f, Func<A, B, C> g) =>
        Proxy.SelectMany(f, g);
   
    [Pure]
    public Pipe<RT, IN, OUT, C> SelectMany<B, C>(Func<A, Pure<B>> f, Func<A, B, C> g) =>
        Proxy.SelectMany(f, g);
   
    [Pure]
    public Pipe<RT, IN, OUT, C> SelectMany<B, C>(Func<A, Lift<B>> f, Func<A, B, C> g) =>
        Proxy.SelectMany(f, g);
                
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
    public Pipe<RT, IN, OUT, Unit> Fold(
        Schedule Time,
        Func<OUT, A, OUT> Fold, 
        OUT Init) =>
        Proxy.Fold(Time, Fold, Init);

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
    public Pipe<RT, IN, OUT, Unit> FoldUntil(
        Func<OUT, A, OUT> Fold, 
        Func<(OUT State, A Value), bool> Pred, 
        OUT Init) =>
        Proxy.FoldUntil(Fold, Pred, Init);
        
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
    public Pipe<RT, IN, OUT, Unit> FoldUntil(
        Schedule Time,
        Func<OUT, A, OUT> Fold, 
        Func<(OUT State, A Value), bool> Pred, 
        OUT Init) =>
        Proxy.FoldUntil(Time, Fold, Pred, Init);
        
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
    public Pipe<RT, IN, OUT, Unit> FoldWhile(
        Func<OUT, A, OUT> Fold, 
        Func<(OUT State, A Value), bool> Pred, 
        OUT Init) =>
        Proxy.FoldWhile(Fold, Pred, Init);
        
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
    public Pipe<RT, IN, OUT, Unit> FoldWhile(
        Schedule Time,
        Func<OUT, A, OUT> Fold, 
        Func<(OUT State, A Value), bool> Pred, 
        OUT Init) =>
        Proxy.FoldWhile(Time, Fold, Pred, Init);
    
    [Pure]
    internal virtual Eff<RT, A> Run() =>
        Proxy.Run().As();
}

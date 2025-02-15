using System;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;

namespace LanguageExt.Pipes;

/// <summary>
/// `Producer` streaming producer monad-transformer instance
/// </summary>
public readonly record struct Producer<RT, OUT, A>(PipeT<Unit, OUT, Eff<RT>, A> Proxy) : K<Producer<RT, OUT>, A>
{
    [Pure]
    public Producer<RT, OUT1, A> Compose<OUT1>(PipeT<OUT, OUT1, Eff<RT>, A> rhs) =>
        Proxy.Compose(rhs).ToProducer();

    [Pure]
    public Producer<RT, OUT1, A> Compose<OUT1>(Pipe<RT, OUT, OUT1, A> rhs) =>
        Proxy.Compose(rhs.Proxy);

    [Pure]
    public Effect<RT, A> Compose(ConsumerT<OUT, Eff<RT>, A> rhs) =>
        Proxy.Compose(rhs.Proxy).ToEffect();

    [Pure]
    public Effect<RT, A> Compose(Consumer<RT, OUT, A> rhs) =>
        Proxy.Compose(rhs.Proxy).ToEffect();

    [Pure]
    public static Effect<RT, A> operator | (Producer<RT, OUT, A> lhs, ConsumerT<OUT, Eff<RT>, A> rhs) =>
        lhs.Proxy.Compose(rhs.Proxy).ToEffect();

    [Pure]
    public static Effect<RT, A> operator | (Producer<RT, OUT, A> lhs, Consumer<RT, OUT, A> rhs) =>
        lhs.Proxy.Compose(rhs.Proxy).ToEffect();
    
    [Pure]
    public Producer<RT, OUT, B> Map<B>(Func<A, B> f) =>
        Proxy.Map(f);
    
    [Pure]
    public Producer<RT, OUT, B> MapM<B>(Func<Eff<RT, A>, Eff<RT, B>> f) =>
        Proxy.MapM(mx => f(mx.As()));

    [Pure]
    public Producer<RT, OUT, B> MapIO<B>(Func<IO<A>, IO<B>> f) =>
        Proxy.MapIO(f);

    [Pure]
    public Producer<RT, OUT, B> ApplyBack<B>(Producer<RT, OUT, Func<A, B>> ff) =>
        Proxy.ApplyBack(ff.Proxy);
    
    [Pure]
    public Producer<RT, OUT, B> Action<B>(Producer<RT, OUT, B> fb) =>
        Proxy.Action(fb.Proxy);
    
    [Pure]
    public Producer<RT, OUT, B> Bind<B>(Func<A, Producer<RT, OUT, B>> f) =>
        Proxy.Bind(x => f(x).Proxy);
    
    [Pure]
    public Producer<RT, OUT, B> Bind<B>(Func<A, K<Eff<RT>, B>> f) => 
        Proxy.Bind(f); 
    
    [Pure]
    public Producer<RT, OUT, B> Bind<B>(Func<A, IO<B>> f) => 
        Proxy.Bind(f); 
       
    [Pure]
    public Producer<RT, OUT, B> Bind<B>(Func<A, Pure<B>> f) =>
        Proxy.Bind(f);
   
    [Pure]
    public Producer<RT, OUT, B> Bind<B>(Func<A, Lift<B>> f) =>
        Proxy.Bind(f);

    [Pure]
    public Producer<RT, OUT, B> Select<B>(Func<A, B> f) =>
        Map(f);
   
    [Pure]
    public Producer<RT, OUT, C> SelectMany<B, C>(Func<A, Producer<RT, OUT, B>> f, Func<A, B, C> g) =>
        Proxy.SelectMany(x => f(x).Proxy, g);
   
    [Pure]
    public Producer<RT, OUT, C> SelectMany<B, C>(Func<A, K<Eff<RT>, B>> f, Func<A, B, C> g) =>
        Proxy.SelectMany(f, g);
   
    [Pure]
    public Producer<RT, OUT, C> SelectMany<B, C>(Func<A, IO<B>> f, Func<A, B, C> g) =>
        Proxy.SelectMany(f, g);
   
    [Pure]
    public Producer<RT, OUT, C> SelectMany<B, C>(Func<A, Pure<B>> f, Func<A, B, C> g) =>
        Proxy.SelectMany(f, g);
   
    [Pure]
    public Producer<RT, OUT, C> SelectMany<B, C>(Func<A, Lift<B>> f, Func<A, B, C> g) =>
        Proxy.SelectMany(f, g);
                
    /// <summary>
    /// Fold the given pipe until the `Schedule` completes.
    /// Once complete, the pipe yields the aggregated value downstream.
    /// </summary>
    /// <param name="Time">Schedule to run each item</param>
    /// <param name="Fold">Fold function</param>
    /// <param name="Init">Initial state</param>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public Producer<RT, OUT, Unit> Fold(
        Schedule Time,
        Func<OUT, A, OUT> Fold, 
        OUT Init) =>
        PipeT.fold(Time, Fold, Init, Proxy);

    /// <summary>
    /// Fold the given pipe until the predicate is `true`.  Once `true` the pipe yields the
    /// aggregated value downstream.
    /// </summary>
    /// <param name="Fold">Fold function</param>
    /// <param name="Pred">Until predicate</param>
    /// <param name="Init">Initial state</param>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public Producer<RT, OUT, Unit> FoldUntil(
        Func<OUT, A, OUT> Fold, 
        Func<(OUT State, A Value), bool> Pred, 
        OUT Init) =>
        PipeT.foldUntil(Fold, Pred, Init, Proxy);
        
    /// <summary>
    /// Fold the given pipe until the predicate is `true` or the `Schedule` completes.
    /// Once `true`, or completed, the pipe yields the aggregated value downstream.
    /// </summary>
    /// <param name="Time">Schedule to run each item</param>
    /// <param name="Fold">Fold function</param>
    /// <param name="Pred">Until predicate</param>
    /// <param name="Init">Initial state</param>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public Producer<RT, OUT, Unit> FoldUntil(
        Schedule Time,
        Func<OUT, A, OUT> Fold, 
        Func<(OUT State, A Value), bool> Pred, 
        OUT Init) =>
        PipeT.foldUntil(Time, Fold, Pred, Init, Proxy);
        
    /// <summary>
    /// Fold the given pipe while the predicate is `true`.  Once `false` the pipe yields the
    /// aggregated value downstream.
    /// </summary>
    /// <param name="Fold">Fold function</param>
    /// <param name="Pred">Until predicate</param>
    /// <param name="Init">Initial state</param>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public Producer<RT, OUT, Unit> FoldWhile(
        Func<OUT, A, OUT> Fold, 
        Func<(OUT State, A Value), bool> Pred, 
        OUT Init) =>
        PipeT.foldWhile(Fold, Pred, Init, Proxy);
        
    /// <summary>
    /// Fold the given pipe while the predicate is `true` or the `Schedule` completes.
    /// Once `false`, or completed, the pipe yields the aggregated value downstream.
    /// </summary>
    /// <param name="Time">Schedule to run each item</param>
    /// <param name="Fold">Fold function</param>
    /// <param name="Pred">Until predicate</param>
    /// <param name="Init">Initial state</param>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public Producer<RT, OUT, Unit> FoldWhile(
        Schedule Time,
        Func<OUT, A, OUT> Fold, 
        Func<(OUT State, A Value), bool> Pred, 
        OUT Init) =>
        PipeT.foldWhile(Time, Fold, Pred, Init, Proxy);   
    
    [Pure]
    public static implicit operator Producer<RT, OUT, A>(PipeT<Unit, OUT, Eff<RT>, A> pipe) =>
        pipe.ToProducer();
    
    [Pure]
    public static implicit operator Producer<RT, OUT, A>(Pipe<RT, Unit, OUT, A> pipe) =>
        pipe.ToProducer();
    
    [Pure]
    public static implicit operator Producer<RT, OUT, A>(ProducerT<OUT, Eff<RT>, A> pipe) =>
        pipe.Proxy.ToProducer();
    
    [Pure]
    public static implicit operator Producer<RT, OUT, A>(Pure<A> rhs) =>
        Producer.pure<RT, OUT, A>(rhs.Value);

    [Pure]
    internal Eff<RT, A> Run() =>
        Proxy.Run().As();
}

using System.Diagnostics.Contracts;
using LanguageExt.Traits;

namespace LanguageExt.Pipes2;

/// <summary>
/// `QueueT` streaming producer monad-transformer instance
/// </summary>
public readonly struct QueueT<OUT, M, A>
    where M : Monad<M>
{
    readonly Channel<OUT> Channel;
    readonly ProducerT<OUT, M, A> Proxy;
    
    internal QueueT(Channel<OUT> channel, ProducerT<OUT, M, A> proxy)
    {
        Channel = channel;
        Proxy = proxy;
    }

    /// <summary>
    /// Enqueue an item in the queue
    /// </summary>
    /// <param name="value">Value to enqueue</param>
    /// <returns>Unit</returns>
    public Unit Enqueue(OUT value) =>
        Channel.Post(value);

    /// <summary>
    /// Stop the queue channel
    /// </summary>
    public Unit Stop() =>
        Channel.Stop();

    /// <summary>
    /// Expose the queue as a producer
    /// </summary>
    public ProducerT<OUT, M, A> ToProducer() =>
        Proxy;
    
    [Pure]
    public ProducerT<OUT1, M, A> Compose<OUT1>(PipeT<OUT, OUT1, M, A> rhs) =>
        Proxy.Compose(rhs);

    [Pure]
    public EffectT<M, A> Compose(ConsumerT<OUT, M, A> rhs) =>
        Proxy.Compose(rhs);

    [Pure]
    public static EffectT<M, A> operator | (QueueT<OUT, M, A> lhs, ConsumerT<OUT, M, A> rhs) =>
        lhs.Proxy.Compose(rhs);
    
    [Pure]
    public QueueT<OUT, M, B> Map<B>(Func<A, B> f) =>
        new(Channel, Proxy.Map(f));
    
    [Pure]
    public QueueT<OUT, M, B> MapM<B>(Func<K<M, A>, K<M, B>> f) =>
        new(Channel, Proxy.MapM(f));

    [Pure]
    public QueueT<OUT, M, B> MapIO<B>(Func<IO<A>, IO<B>> f) =>
        new(Channel, Proxy.MapIO(f));

    [Pure]
    public QueueT<OUT, M, B> ApplyBack<B>(QueueT<OUT, M, Func<A, B>> ff) =>
        new(Channel, Proxy.ApplyBack(ff.Proxy));
    
    [Pure]
    public QueueT<OUT, M, B> Action<B>(QueueT<OUT, M, B> fb) =>
        new(Channel, Proxy.Action(fb.Proxy));
    
    [Pure]
    public QueueT<OUT, M, B> Bind<B>(Func<A, QueueT<OUT, M, B>> f) =>
        new(Channel, Proxy.Bind(x => f(x).Proxy));
    
    [Pure]
    public QueueT<OUT, M, B> Bind<B>(Func<A, K<M, B>> f) => 
        new(Channel, Proxy.Bind(f)); 
    
    [Pure]
    public QueueT<OUT, M, B> Bind<B>(Func<A, IO<B>> f) => 
        new(Channel, Proxy.Bind(f)); 
       
    [Pure]
    public QueueT<OUT, M, B> Bind<B>(Func<A, Pure<B>> f) =>
        new(Channel, Proxy.Bind(f));
   
    [Pure]
    public QueueT<OUT, M, B> Bind<B>(Func<A, Lift<B>> f) =>
        new(Channel, Proxy.Bind(f));

    [Pure]
    public QueueT<OUT, M, B> Select<B>(Func<A, B> f) =>
        Map(f);
   
    [Pure]
    public QueueT<OUT, M, C> SelectMany<B, C>(Func<A, QueueT<OUT, M, B>> f, Func<A, B, C> g) =>
        new(Channel, Proxy.SelectMany(x => f(x).Proxy, g));
   
    [Pure]
    public QueueT<OUT, M, C> SelectMany<B, C>(Func<A, K<M, B>> f, Func<A, B, C> g) =>
        new(Channel, Proxy.SelectMany(f, g));
   
    [Pure]
    public QueueT<OUT, M, C> SelectMany<B, C>(Func<A, IO<B>> f, Func<A, B, C> g) =>
        new(Channel, Proxy.SelectMany(f, g));
   
    [Pure]
    public QueueT<OUT, M, C> SelectMany<B, C>(Func<A, Pure<B>> f, Func<A, B, C> g) =>
        new(Channel, Proxy.SelectMany(f, g));
   
    [Pure]
    public QueueT<OUT, M, C> SelectMany<B, C>(Func<A, Lift<B>> f, Func<A, B, C> g) =>
        new(Channel, Proxy.SelectMany(f, g));

    [Pure]
    internal K<M, A> Run() =>
        Proxy.Run();
}

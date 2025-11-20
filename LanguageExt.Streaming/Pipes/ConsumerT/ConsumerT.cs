using System;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;

namespace LanguageExt.Pipes;

/// <summary>
/// `ConsumerT` streaming consumer monad-transformer instance
/// </summary>
public readonly record struct ConsumerT<IN, M, A>(PipeT<IN, Void, M, A> Proxy) : K<ConsumerT<IN, M>, A>
    where M : MonadIO<M>
{
    [Pure]
    public ConsumerT<IN, M, B> Map<B>(Func<A, B> f) =>
        Proxy.Map(f);
    
    [Pure]
    public ConsumerT<IN, M, B> MapM<B>(Func<K<M, A>, K<M, B>> f) =>
        Proxy.MapM(f);

    [Pure]
    public ConsumerT<IN, M, B> ApplyBack<B>(ConsumerT<IN, M, Func<A, B>> ff) =>
        Proxy.ApplyBack(ff.Proxy);
    
    [Pure]
    public ConsumerT<IN, M, B> Action<B>(ConsumerT<IN, M, B> fb) =>
        Proxy.Action(fb.Proxy);
    
    [Pure]
    public ConsumerT<IN, M, B> Bind<B>(Func<A, ConsumerT<IN, M, B>> f) =>
        Proxy.Bind(x => f(x).Proxy);
    
    [Pure]
    public ConsumerT<IN, M, B> Bind<B>(Func<A, IO<B>> f) =>
        Proxy.Bind(f);

    [Pure]
    public ConsumerT<IN, M, B> Bind<B>(Func<A, K<M, B>> f) =>
        Proxy.Bind(f);
   
    [Pure]
    public ConsumerT<IN, M, B> Bind<B>(Func<A, Pure<B>> f) =>
        Proxy.Bind(f);
   
    [Pure]
    public ConsumerT<IN, M, B> Bind<B>(Func<A, Lift<B>> f) =>
        Proxy.Bind(f);

    [Pure]
    internal K<M, A> Run() =>
        Proxy.Run();
    
    [Pure]
    public ConsumerT<IN, M, B> Select<B>(Func<A, B> f) =>
        Proxy.Map(f);
   
    [Pure]
    public ConsumerT<IN, M, C> SelectMany<B, C>(Func<A, ConsumerT<IN, M, B>> f, Func<A, B, C> g) =>
        Proxy.SelectMany(x => f(x).Proxy, g);
   
    [Pure]
    public ConsumerT<IN, M, C> SelectMany<B, C>(Func<A, K<M, B>> f, Func<A, B, C> g) =>
        Proxy.SelectMany(f, g);
   
    [Pure]
    public ConsumerT<IN, M, C> SelectMany<B, C>(Func<A, IO<B>> f, Func<A, B, C> g) =>
        Proxy.SelectMany(f, g);
   
    [Pure]
    public ConsumerT<IN, M, C> SelectMany<B, C>(Func<A, Pure<B>> f, Func<A, B, C> g) =>
        Proxy.SelectMany(f, g);
   
    [Pure]
    public ConsumerT<IN, M, C> SelectMany<B, C>(Func<A, Lift<B>> f, Func<A, B, C> g) =>
        Proxy.SelectMany(f, g);

    [Pure]
    public static implicit operator ConsumerT<IN, M, A>(PipeT<IN, Void, M, A> pipe) =>
        pipe.ToConsumer();
    
    [Pure]
    public static implicit operator ConsumerT<IN, M, A>(Pure<A> rhs) =>
        ConsumerT.pure<IN, M, A>(rhs.Value);
    
}

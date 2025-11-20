using System;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;

namespace LanguageExt.Pipes;

/// <summary>
/// `Consumer` streaming consumer monad-transformer instance
/// </summary>
/// <remarks>
/// Unlike the general purpose `ConsumerT`, which will lift any monad, this type only lifts the `Eff` monad.
/// </remarks>
public readonly record struct Consumer<RT, IN, A>(PipeT<IN, Void, Eff<RT>, A> Proxy) : K<Consumer<RT, IN>, A>
{
    [Pure]
    public Consumer<RT, IN, B> Map<B>(Func<A, B> f) =>
        Proxy.Map(f);

    [Pure]
    public Consumer<RT, IN, B> MapM<B>(Func<Eff<RT, A>, Eff<RT, B>> f) =>
        Proxy.MapM(ma => f(ma.As()));

    [Pure]
    public Consumer<RT, IN, B> MapIO<B>(Func<IO<A>, IO<B>> f) =>
        Proxy.MapIO(f);

    [Pure]
    public Consumer<RT, IN, B> ApplyBack<B>(Consumer<RT, IN, Func<A, B>> ff) =>
        Proxy.ApplyBack(ff.Proxy);
    
    [Pure]
    public Consumer<RT, IN, B> Action<B>(Consumer<RT, IN, B> fb) =>
        Proxy.Action(fb.Proxy);
    
    [Pure]
    public Consumer<RT, IN, B> Bind<B>(Func<A, Consumer<RT, IN, B>> f) =>
        Proxy.Bind(x => f(x).Proxy);
    
    [Pure]
    public Consumer<RT, IN, B> Bind<B>(Func<A, IO<B>> f) =>
        Proxy.Bind(f);

    [Pure]
    public Consumer<RT, IN, B> Bind<B>(Func<A, Eff<RT, B>> f) =>
        Proxy.Bind(f).ToConsumer();
   
    [Pure]
    public Consumer<RT, IN, B> Bind<B>(Func<A, Pure<B>> f) =>
        Proxy.Bind(f);
   
    [Pure]
    public Consumer<RT, IN, B> Bind<B>(Func<A, Lift<B>> f) =>
        Proxy.Bind(f);

    [Pure]
    internal Eff<RT, A> Run() =>
        Proxy.Run().As();
    
    [Pure]
    public Consumer<RT, IN, B> Select<B>(Func<A, B> f) =>
        Proxy.Map(f);
   
    [Pure]
    public Consumer<RT, IN, C> SelectMany<B, C>(Func<A, Consumer<RT, IN, B>> f, Func<A, B, C> g) =>
        Proxy.SelectMany(x => f(x).Proxy, g);
   
    [Pure]
    public Consumer<RT, IN, C> SelectMany<B, C>(Func<A, Eff<RT, B>> f, Func<A, B, C> g) =>
        Proxy.SelectMany(f, g);
   
    [Pure]
    public Consumer<RT, IN, C> SelectMany<B, C>(Func<A, IO<B>> f, Func<A, B, C> g) =>
        Proxy.SelectMany(f, g);
   
    [Pure]
    public Consumer<RT, IN, C> SelectMany<B, C>(Func<A, Pure<B>> f, Func<A, B, C> g) =>
        Proxy.SelectMany(f, g);
   
    [Pure]
    public Consumer<RT, IN, C> SelectMany<B, C>(Func<A, Lift<B>> f, Func<A, B, C> g) =>
        Proxy.SelectMany(f, g);

    [Pure]
    public static implicit operator Consumer<RT, IN, A>(ConsumerT<IN, Eff<RT>, A> consumer) =>
        consumer.Proxy;

    [Pure]
    public static implicit operator Consumer<RT, IN, A>(PipeT<IN, Void, Eff<RT>, A> pipe) =>
        pipe.ToConsumer();

    [Pure]
    public static implicit operator Consumer<RT, IN, A>(Pipe<RT, IN, Void, A> pipe) =>
        pipe.ToConsumer();

    [Pure]
    public static implicit operator Consumer<RT, IN, A>(Pure<A> rhs) =>
        Consumer.pure<RT, IN, A>(rhs.Value);
}

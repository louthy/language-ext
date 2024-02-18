using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Pipes;
using LanguageExt.Traits;
using Void = LanguageExt.Pipes.Void;

namespace LanguageExt;

public static class ProxyExtensions
{
    /// <summary>
    /// Monad bind (specialised)
    /// </summary>
    [Pure, MethodImpl(Proxy.mops)]
    public static Producer<OUT, M, B> Bind<OUT, A, M, B>(
        this Proxy<Void, Unit, Unit, OUT, M, A> ma, 
        Func<A, Producer<OUT, M, B>> f) 
        where M : Monad<M> =>
        ma.Bind(f).ToProducer();
        
    /// <summary>
    /// Monad bind (specialised)
    /// </summary>
    [Pure, MethodImpl(Proxy.mops)]
    public static Consumer<IN, M, B> Bind<IN, A, M, B>(
        this Proxy<Unit, IN, Unit, Void, M, A> ma, 
        Func<A, Consumer<IN, M, B>> f) 
        where M : Monad<M> =>
        ma.Bind(f).ToConsumer();
        
    /// <summary>
    /// Monad bind (specialised)
    /// </summary>
    [Pure, MethodImpl(Proxy.mops)]
    public static Pipe<IN, OUT, M, B> Bind<IN, OUT, A, M, B>(
        this Proxy<Unit, IN, Unit, OUT, M, A> ma, 
        Func<A, Pipe<IN, OUT, M, B>> f) 
        where M : Monad<M> =>
        ma.Bind(f).ToPipe();
        
    /// <summary>
    /// Monad bind (specialised)
    /// </summary>
    [Pure, MethodImpl(Proxy.mops)]
    public static Client<REQ, RES, M, B> Bind<REQ, RES, A, M, B>(
        this Proxy<REQ, RES, Unit, Void, M, A> ma, 
        Func<A, Client<REQ, RES, M, B>> f) 
        where M : Monad<M> =>
        ma.Bind(f).ToClient();
        
    /// <summary>
    /// Monad bind (specialised)
    /// </summary>
    [Pure, MethodImpl(Proxy.mops)]
    public static Server<REQ, RES, M, B> Bind<REQ, RES, A, M, B>(
        this Proxy<Void, Unit, REQ, RES, M, A> ma, 
        Func<A, Server<REQ, RES, M, B>> f) 
        where M : Monad<M> =>
        ma.Bind(f).ToServer();

        
    /// <summary>
    /// Monad bind (specialised)
    /// </summary>
    [Pure, MethodImpl(Proxy.mops)]
    public static Producer<OUT, M, B> SelectMany<OUT, A, M, B>(
        this Proxy<Void, Unit, Unit, OUT, M, A> ma, 
        Func<A, Producer<OUT, M, B>> f) 
        where M : Monad<M> =>
        ma.Bind(f).ToProducer();
        
    /// <summary>
    /// Monad bind (specialised)
    /// </summary>
    [Pure, MethodImpl(Proxy.mops)]
    public static Consumer<IN, M, B> SelectMany<IN, A, M, B>(
        this Proxy<Unit, IN, Unit, Void, M, A> ma, 
        Func<A, Consumer<IN, M, B>> f) 
        where M : Monad<M> =>
        ma.Bind(f).ToConsumer();
        
    /// <summary>
    /// Monad bind (specialised)
    /// </summary>
    [Pure, MethodImpl(Proxy.mops)]
    public static Pipe<IN, OUT, M, B> SelectMany<IN, OUT, A, M, B>(
        this Proxy<Unit, IN, Unit, OUT, M, A> ma, 
        Func<A, Pipe<IN, OUT, M, B>> f) 
        where M : Monad<M> =>
        ma.Bind(f).ToPipe();
        
    /// <summary>
    /// Monad bind (specialised)
    /// </summary>
    [Pure, MethodImpl(Proxy.mops)]
    public static Client<REQ, RES, M, B> SelectMany<REQ, RES, A, M, B>(
        this Proxy<REQ, RES, Unit, Void, M, A> ma, 
        Func<A, Client<REQ, RES, M, B>> f) 
        where M : Monad<M> =>
        ma.Bind(f).ToClient();
        
    /// <summary>
    /// Monad bind (specialised)
    /// </summary>
    [Pure, MethodImpl(Proxy.mops)]
    public static Server<REQ, RES, M, B> SelectMany<REQ, RES, A, M, B>(
        this Proxy<Void, Unit, REQ, RES, M, A> ma, Func<A, Server<REQ, RES, M, B>> f) 
        where M : Monad<M> =>
        ma.Bind(f).ToServer();

        
    /// <summary>
    /// Monad bind (specialised)
    /// </summary>
    [Pure, MethodImpl(Proxy.mops)]
    public static Producer<OUT, M, C> SelectMany<OUT, A, B, M, C>(
        this Proxy<Void, Unit, Unit, OUT, M, A> ma, Func<A, Producer<OUT, M, B>> f, 
        Func<A, B, C> project) 
        where M : Monad<M> =>
        ma.Bind(a => f(a).Map(b => project(a, b))).ToProducer();
        
    /// <summary>
    /// Monad bind (specialised)
    /// </summary>
    [Pure, MethodImpl(Proxy.mops)]
    public static Consumer<IN, M, C> SelectMany<IN, A, B, M, C>(
        this Proxy<Unit, IN, Unit, Void, M, A> ma, 
        Func<A, Consumer<IN, M, B>> f, Func<A, B, C> project) 
        where M : Monad<M> =>
        ma.Bind(a => f(a).Map(b => project(a, b))).ToConsumer();
        
    /// <summary>
    /// Monad bind (specialised)
    /// </summary>
    [Pure, MethodImpl(Proxy.mops)]
    public static Pipe<IN, OUT, M, C> SelectMany<IN, OUT, A, B, M, C>(
        this Proxy<Unit, IN, Unit, OUT, M, A> ma, 
        Func<A, Pipe<IN, OUT, M, B>> f, 
        Func<A, B, C> project) 
        where M : Monad<M> =>
        ma.Bind(a => f(a).Map(b => project(a, b))).ToPipe();
        
    /// <summary>
    /// Monad bind (specialised)
    /// </summary>
    [Pure, MethodImpl(Proxy.mops)]
    public static Client<REQ, RES, M, C> SelectMany<REQ, RES, A, B, M, C>(
        this Proxy<REQ, RES, Unit, Void, M, A> ma, 
        Func<A, Client<REQ, RES, M, B>> f, 
        Func<A, B, C> project) 
        where M : Monad<M> =>
        ma.Bind(a => f(a).Map(b => project(a, b))).ToClient();
        
    /// <summary>
    /// Monad bind (specialised)
    /// </summary>
    [Pure, MethodImpl(Proxy.mops)]
    public static Server<REQ, RES, M, C> SelectMany<REQ, RES, A, B, M, C>(
        this Proxy<Void, Unit, REQ, RES, M, A> ma, 
        Func<A, Server<REQ, RES, M, B>> f, Func<A, B, C> project) 
        where M : Monad<M> =>
        ma.Bind(a => f(a).Map(b => project(a, b))).ToServer();
}

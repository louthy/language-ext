using System;
using LanguageExt.Pipes;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class ProxyExtensions
{
    public static Proxy<UOut, UIn, DIn, DOut, M, A> As<UOut, UIn, DIn, DOut, M, A>(
        this K<Proxy<UOut, UIn, DIn, DOut, M>, A> ma)
        where M : Monad<M> =>
        (Proxy<UOut, UIn, DIn, DOut, M, A>)ma;

    [Pure]
    public static Effect<M, A> As<M, A>(
        this K<Proxy<Void, Unit, Unit, Void, M>, A> ma)
        where M : Monad<M> =>
        ma switch
        {
            Effect<M, A> e                        => e,
            Proxy<Void, Unit, Unit, Void, M, A> p => new Effect<M, A>(p),
            _                                     => throw new InvalidCastException($"Expected an Effect or Proxy<Void, Unit, Unit, Void, M, A>, but got: {ma.GetType()}")
        };
    
    public static Producer<OUT, M, A> As<OUT, M, A>(
        this K<Proxy<Void, Unit, Unit, OUT, M>, A> ma)
        where M : Monad<M> =>
        ma switch
        {
            Producer<OUT, M, A> p                => p,
            Proxy<Void, Unit, Unit, OUT, M, A> p => new Producer<OUT, M, A>(p),
            _                                    => throw new InvalidCastException($"Expected a Producer or Proxy<Void, Unit, Unit, OUT, M, A>, but got: {ma.GetType()}")
        };
    
    public static Consumer<IN, M, A> As<IN, M, A>(
        this K<Proxy<Unit, IN, Unit, Void, M>, A> ma)
        where M : Monad<M> =>
        ma switch
        {
            Consumer<IN, M, A> c                => c,
            Proxy<Unit, IN, Unit, Void, M, A> p => new Consumer<IN, M, A>(p),
            _                                   => throw new InvalidCastException($"Expected a Consumer or Proxy<Unit, IN, Unit, Void, M, A>, but got: {ma.GetType()}")
        };
    
    public static Pipe<IN, OUT, M, A> As<IN, OUT, M, A>(
        this K<Proxy<Unit, IN, Unit, OUT, M>, A> ma)
        where M : Monad<M> =>
        ma switch
        {
            Pipe<IN, OUT, M, A> p              => p,
            Proxy<Unit, IN, Unit, OUT, M, A> p => new Pipe<IN, OUT, M, A>(p),
            _                                  => throw new InvalidCastException($"Expected a Pipe or Proxy<Unit, IN, Unit, OUT, M, A>, but got: {ma.GetType()}")
        };
    
    public static Client<REQ, RES, M, A> As<REQ, RES, M, A>(
        this K<Proxy<REQ, RES,  Unit, Void, M>, A> ma)
        where M : Monad<M> =>
        ma switch
        {
            Client<REQ, RES, M, A> c            => c,
            Proxy<REQ, RES, Unit, Void, M, A> p => new Client<REQ, RES, M, A>(p),
            _                                   => throw new InvalidCastException($"Expected a Client or Proxy<REQ, RES, Unit, Void, M, A>, but got: {ma.GetType()}")
        };
    
    public static Server<REQ, RES, M, A> As<REQ, RES, M, A>(
        this K<Proxy<Void, Unit, REQ, RES, M>, A> ma)
        where M : Monad<M> =>
        ma switch
        {
            Server<REQ, RES, M, A> s            => s,
            Proxy<Void, Unit, REQ, RES, M, A> p => new Server<REQ, RES, M, A>(p),
            _                                   => throw new InvalidCastException($"Expected a Server Proxy<Void, Unit, REQ, RES, M, A>, but got: {ma.GetType()}")
        };
    
    /// <summary>
    /// Converts a `Proxy` with the correct _shape_ into an `Effect`
    /// </summary>
    [Pure]
    public static Effect<M, R> ToEffect<M, R>(this Proxy<Void, Unit, Unit, Void, M, R> ma) 
        where M : Monad<M> =>
        ma as Effect<M, R> ?? new Effect<M, R>(ma);
        
    /// <summary>
    /// Converts a `Proxy` with the correct _shape_ into a `Producer`
    /// </summary>
    [Pure]
    public static Producer<A, M, R> ToProducer<A, M, R>(this Proxy<Void, Unit, Unit, A, M, R> ma) 
        where M : Monad<M> =>
        ma as Producer<A, M, R> ?? new Producer<A, M, R>(ma);
        
    /// <summary>
    /// Converts a `Proxy` with the correct _shape_ into a `Consumer`
    /// </summary>
    [Pure]
    public static Consumer<A, M, R> ToConsumer<A, M, R>(this Proxy<Unit, A, Unit, Void, M, R> ma) 
        where M : Monad<M> =>
        ma as Consumer<A, M, R> ?? new Consumer<A, M, R>(ma);

    /// <summary>
    /// Converts a `Proxy` with the correct _shape_ into n `Pipe`
    /// </summary>
    [Pure]
    public static Pipe<A, B, M, R> ToPipe<A, B, M, R>(this Proxy<Unit, A, Unit, B, M, R> ma) 
        where M : Monad<M> =>
        ma as Pipe<A, B, M, R> ?? new Pipe<A, B, M, R>(ma);
        
    /// <summary>
    /// Converts a `Proxy` with the correct _shape_ into a `Client`
    /// </summary>
    [Pure]
    public static Client<A, B, M, R> ToClient<A, B, M, R>(this Proxy<A, B, Unit, Void, M, R> ma) 
        where M : Monad<M> =>
        ma as Client<A, B, M, R> ?? new Client<A, B, M, R>(ma);
        
    /// <summary>
    /// Converts a `Proxy` with the correct _shape_ into a `Server`
    /// </summary>
    [Pure]
    public static Server<A, B, M, R> ToServer<A, B, M, R>(this Proxy<Void, Unit, A, B, M, R> ma) 
        where M : Monad<M> =>
        ma as Server<A, B, M, R> ?? new Server<A, B, M, R>(ma);
}

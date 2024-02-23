using LanguageExt.Pipes;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Traits;

namespace LanguageExt;

public static class Extensions
{
    public static Proxy<UOut, UIn, DIn, DOut, M, A> As<UOut, UIn, DIn, DOut, M, A>(
        this K<Proxy<UOut, UIn, DIn, DOut, M>, A> ma)
        where M : Monad<M> =>
        (Proxy<UOut, UIn, DIn, DOut, M, A>)ma;
    
    public static Effect<M, A> As<M, A>(
        this K<Proxy<Void, Unit, Unit, Void, M>, A> ma)
        where M : Monad<M> =>
        (Effect<M, A>)ma;
    
    public static Producer<OUT, M, A> As<OUT, M, A>(
        this K<Proxy<Void, Unit, Unit, OUT, M>, A> ma)
        where M : Monad<M> =>
        (Producer<OUT, M, A>)ma;
    
    public static Consumer<IN, M, A> As<IN, M, A>(
        this K<Proxy<Unit, IN, Unit, Void, M>, A> ma)
        where M : Monad<M> =>
        (Consumer<IN, M, A>)ma;
    
    public static Pipe<IN, OUT, M, A> As<IN, OUT, M, A>(
        this K<Proxy<Unit, IN, Unit, OUT, M>, A> ma)
        where M : Monad<M> =>
        (Pipe<IN, OUT, M, A>)ma;
    
    public static Client<REQ, RES, M, A> As<REQ, RES, M, A>(
        this K<Proxy<REQ, RES,  Unit, Void, M>, A> ma)
        where M : Monad<M> =>
        (Client<REQ, RES, M, A>)ma;
    
    public static Server<REQ, RES, M, A> As<REQ, RES, M, A>(
        this K<Proxy<Void, Unit, REQ, RES, M>, A> ma)
        where M : Monad<M> =>
        (Server<REQ, RES, M, A>)ma;
    
    /// <summary>
    /// Converts a `Proxy` with the correct _shape_ into an `Effect`
    /// </summary>
    [Pure, MethodImpl(Proxy.mops)]
    public static Effect<M, R> ToEffect<M, R>(this Proxy<Void, Unit, Unit, Void, M, R> ma) 
        where M : Monad<M> =>
        ma as Effect<M, R> ?? new Effect<M, R>(ma);
        
    /// <summary>
    /// Converts a `Proxy` with the correct _shape_ into a `Producer`
    /// </summary>
    [Pure, MethodImpl(Proxy.mops)]
    public static Producer<A, M, R> ToProducer<A, M, R>(this Proxy<Void, Unit, Unit, A, M, R> ma) 
        where M : Monad<M> =>
        ma as Producer<A, M, R> ?? new Producer<A, M, R>(ma);
        
    /// <summary>
    /// Converts a `Proxy` with the correct _shape_ into a `Consumer`
    /// </summary>
    [Pure, MethodImpl(Proxy.mops)]
    public static Consumer<A, M, R> ToConsumer<A, M, R>(this Proxy<Unit, A, Unit, Void, M, R> ma) 
        where M : Monad<M> =>
        ma as Consumer<A, M, R> ?? new Consumer<A, M, R>(ma);

    /// <summary>
    /// Converts a `Proxy` with the correct _shape_ into n `Pipe`
    /// </summary>
    [Pure, MethodImpl(Proxy.mops)]
    public static Pipe<A, B, M, R> ToPipe<A, B, M, R>(this Proxy<Unit, A, Unit, B, M, R> ma) 
        where M : Monad<M> =>
        ma as Pipe<A, B, M, R> ?? new Pipe<A, B, M, R>(ma);
        
    /// <summary>
    /// Converts a `Proxy` with the correct _shape_ into a `Client`
    /// </summary>
    [Pure, MethodImpl(Proxy.mops)]
    public static Client<A, B, M, R> ToClient<A, B, M, R>(this Proxy<A, B, Unit, Void, M, R> ma) 
        where M : Monad<M> =>
        ma as Client<A, B, M, R> ?? new Client<A, B, M, R>(ma);
        
    /// <summary>
    /// Converts a `Proxy` with the correct _shape_ into a `Server`
    /// </summary>
    [Pure, MethodImpl(Proxy.mops)]
    public static Server<A, B, M, R> ToServer<A, B, M, R>(this Proxy<Void, Unit, A, B, M, R> ma) 
        where M : Monad<M> =>
        ma as Server<A, B, M, R> ?? new Server<A, B, M, R>(ma);
}

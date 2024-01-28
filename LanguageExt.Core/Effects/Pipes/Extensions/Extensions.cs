using LanguageExt.Pipes;
using LanguageExt.Effects.Traits;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Common;

namespace LanguageExt;

public static class Extensions
{
    /// <summary>
    /// Converts a `Proxy` with the correct _shape_ into an `Effect`
    /// </summary>
    [Pure, MethodImpl(Proxy.mops)]
    public static Effect<RT, R> ToEffect<RT, R>(this Proxy<RT, Void, Unit, Unit, Void, R> ma) where RT : HasIO<RT, Error> =>
        ma as Effect<RT, R> ?? new Effect<RT, R>(ma);
        
    /// <summary>
    /// Converts a `Proxy` with the correct _shape_ into a `Producer`
    /// </summary>
    [Pure, MethodImpl(Proxy.mops)]
    public static Producer<RT, A, R> ToProducer<RT, A, R>(this Proxy<RT, Void, Unit, Unit, A, R> ma) where RT : HasIO<RT, Error> =>
        ma as Producer<RT, A, R> ?? new Producer<RT, A, R>(ma);
        
    /// <summary>
    /// Converts a `Proxy` with the correct _shape_ into a `Consumer`
    /// </summary>
    [Pure, MethodImpl(Proxy.mops)]
    public static Consumer<RT, A, R> ToConsumer<RT, A, R>(this Proxy<RT, Unit, A, Unit, Void, R> ma) where RT : HasIO<RT, Error> =>
        ma as Consumer<RT, A, R> ?? new Consumer<RT, A, R>(ma);

    /// <summary>
    /// Converts a `Proxy` with the correct _shape_ into n `Pipe`
    /// </summary>
    [Pure, MethodImpl(Proxy.mops)]
    public static Pipe<RT, A, B, R> ToPipe<RT, A, B, R>(this Proxy<RT, Unit, A, Unit, B, R> ma) where RT : HasIO<RT, Error> =>
        ma as Pipe<RT, A, B, R> ?? new Pipe<RT, A, B, R>(ma);
        
    /// <summary>
    /// Converts a `Proxy` with the correct _shape_ into a `Client`
    /// </summary>
    [Pure, MethodImpl(Proxy.mops)]
    public static Client<RT, A, B, R> ToClient<RT, A, B, R>(this Proxy<RT, A, B, Unit, Void, R> ma) where RT : HasIO<RT, Error> =>
        ma as Client<RT, A, B, R> ?? new Client<RT, A, B, R>(ma);
        
    /// <summary>
    /// Converts a `Proxy` with the correct _shape_ into a `Server`
    /// </summary>
    [Pure, MethodImpl(Proxy.mops)]
    public static Server<RT, A, B, R> ToServer<RT, A, B, R>(this Proxy<RT, Void, Unit, A, B, R> ma) where RT : HasIO<RT, Error> =>
        ma as Server<RT, A, B, R> ?? new Server<RT, A, B, R>(ma);
}

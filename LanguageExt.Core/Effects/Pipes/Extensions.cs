using LanguageExt.Pipes;
using LanguageExt.Effects.Traits;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt
{
    public static class Extensions
    {
        /// <summary>
        /// Converts a `Proxy` with the correct _shape_ into an `Effect`
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Effect<RT, R> ToEffect<RT, R>(this Proxy<RT, Void, Unit, Unit, Void, R> ma) where RT : struct, HasCancel<RT> =>
            ma is Effect<RT, R> me 
                ? me
                : new Effect<RT, R>(ma);
        
        /// <summary>
        /// Converts a `Proxy` with the correct _shape_ into a `Producer`
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<RT, A, R> ToProducer<RT, A, R>(this Proxy<RT, Void, Unit, Unit, A, R> ma) where RT : struct, HasCancel<RT> =>
            ma is Producer<RT, A, R> mp 
                ? mp
                : new Producer<RT, A, R>(ma);
        
        /// <summary>
        /// Converts a `Proxy` with the correct _shape_ into a `Consumer`
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, A, R> ToConsumer<RT, A, R>(this Proxy<RT, Unit, A, Unit, Void, R> ma) where RT : struct, HasCancel<RT> =>
            ma is Consumer<RT, A, R> mc 
                ? mc
                : new Consumer<RT, A, R>(ma);

        /// <summary>
        /// Converts a `Proxy` with the correct _shape_ into n `Pipe`
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, A, B, R> ToPipe<RT, A, B, R>(this Proxy<RT, Unit, A, Unit, B, R> ma) where RT : struct, HasCancel<RT> =>
            ma is Pipe<RT, A, B, R> mp 
                ? mp 
                : new Pipe<RT, A, B, R>(ma);
        
        /// <summary>
        /// Converts a `Proxy` with the correct _shape_ into a `Client`
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Client<RT, A, B, R> ToClient<RT, A, B, R>(this Proxy<RT, A, B, Unit, Void, R> ma) where RT : struct, HasCancel<RT> =>
            ma is Client<RT, A, B, R> mc 
                ? mc
                : new Client<RT, A, B, R>(ma);
        
        /// <summary>
        /// Converts a `Proxy` with the correct _shape_ into a `Server`
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Server<RT, A, B, R> ToServer<RT, A, B, R>(this Proxy<RT, Void, Unit, A, B, R> ma) where RT : struct, HasCancel<RT> =>
            ma is Server<RT, A, B, R> ms 
                ? ms
                : new Server<RT, A, B, R>(ma);
    }
}

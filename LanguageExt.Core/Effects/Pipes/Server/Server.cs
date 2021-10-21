using System;
using System.Collections.Generic;
using LanguageExt.Effects.Traits;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt.Pipes
{
    /// <summary>
    /// `Server` receives requests of type `REQ` and sends responses of type `RES`.
    ///
    /// `Servers` only `respond` and never `request`.
    /// </summary>
    /// <remarks> 
    ///       Upstream | Downstream
    ///           +---------+
    ///           |         |
    ///     Void <==       <== RES
    ///           |         |
    ///     Unit ==>       ==> REQ
    ///           |    |    |
    ///           +----|----+
    ///                |
    ///                A
    /// </remarks>
    public class Server
    {
        /// <summary>
        /// Monad return / pure
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Server<RT, REQ, RES, R> Pure<RT, REQ, RES, R>(R value) where RT : struct, HasCancel<RT> =>
            new Pure<RT, Void, Unit, REQ, RES, R>(value).ToServer();
 
        /// <summary>
        /// Send a value of type `RES` downstream and block waiting for a reply of type `REQ`
        /// </summary>
        /// <remarks>
        /// `respond` is the identity of the respond category.
        /// </remarks>
        [Pure, MethodImpl(Proxy.mops)]
        public static Server<RT, REQ, RES, REQ> respond<RT, REQ, RES>(RES value) where RT : struct, HasCancel<RT> =>
            new Respond<RT, Void, Unit, REQ, RES, REQ>(value, r => new Pure<RT, Void, Unit, REQ, RES, REQ>(r)).ToServer();
        
        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Server<RT, REQ, RES, R> lift<RT, REQ, RES, R>(Aff<R> ma) where RT : struct, HasCancel<RT> =>
            new M<RT, Void, Unit, REQ, RES, R>(ma.Map(Proxy.Pure<RT, Void, Unit, REQ, RES, R>).WithRuntime<RT>()).ToServer();

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Server<RT, REQ, RES, R> lift<RT, REQ, RES, R>(Eff<R> ma) where RT : struct, HasCancel<RT> =>
            new M<RT, Void, Unit, REQ, RES, R>(ma.Map(Proxy.Pure<RT, Void, Unit, REQ, RES, R>).ToAffWithRuntime<RT>()).ToServer();

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Server<RT, REQ, RES, R> lift<RT, REQ, RES, R>(Aff<RT, R> ma) where RT : struct, HasCancel<RT> =>
            new M<RT, Void, Unit, REQ, RES, R>(ma.Map(Proxy.Pure<RT, Void, Unit, REQ, RES, R>)).ToServer();

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Server<RT, REQ, RES, R> lift<RT, REQ, RES, R>(Eff<RT, R> ma) where RT : struct, HasCancel<RT> =>
            new M<RT, Void, Unit, REQ, RES, R>(ma.Map(Proxy.Pure<RT, Void, Unit, REQ, RES, R>).ToAff()).ToServer();    

        [Pure, MethodImpl(Proxy.mops)]
        public static Server<RT, REQ, RES, X> enumerate<RT, REQ, RES, X>(IEnumerable<X> xs)
            where RT : struct, HasCancel<RT> =>
            new Enumerate<RT, Void, Unit, REQ, RES, X, X>(xs, Pure<RT, REQ, RES, X>).ToServer();

        [Pure, MethodImpl(Proxy.mops)]
        public static Server<RT, REQ, RES, X> enumerate<RT, REQ, RES, X>(IAsyncEnumerable<X> xs)
            where RT : struct, HasCancel<RT> =>
            new Enumerate<RT, Void, Unit, REQ, RES, X, X>(xs, Pure<RT, REQ, RES, X>).ToServer();

        [Pure, MethodImpl(Proxy.mops)]
        public static Server<RT, REQ, RES, X> observe<RT, REQ, RES, X>(IObservable<X> xs)
            where RT : struct, HasCancel<RT> =>
            new Enumerate<RT, Void, Unit, REQ, RES, X, X>(xs, Pure<RT, REQ, RES, X>).ToServer();
        
        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Server<RT, REQ, RES, R> use<RT, REQ, RES, R>(Aff<R> ma) 
            where RT : struct, HasCancel<RT>
            where R : IDisposable =>
            Proxy.use<RT, Void, Unit, REQ, RES, R>(ma).ToServer();

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Server<RT, REQ, RES, R> use<RT, REQ, RES, R>(Eff<R> ma) 
            where RT : struct, HasCancel<RT>
            where R : IDisposable =>
            Proxy.use<RT, Void, Unit, REQ, RES, R>(ma).ToServer();

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Server<RT, REQ, RES, R> use<RT, REQ, RES, R>(Aff<RT, R> ma) 
            where RT : struct, HasCancel<RT> 
            where R : IDisposable =>
            Proxy.use<RT, Void, Unit, REQ, RES, R>(ma).ToServer();

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Server<RT, REQ, RES, R> use<RT, REQ, RES, R>(Eff<RT, R> ma) 
            where RT : struct, HasCancel<RT> 
            where R : IDisposable =>
            Proxy.use<RT, Void, Unit, REQ, RES, R>(ma).ToServer();
        
        
        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Server<RT, REQ, RES, R> use<RT, REQ, RES, R>(Aff<R> ma, Func<R, Unit> dispose) 
            where RT : struct, HasCancel<RT> =>
            Proxy.use<RT, Void, Unit, REQ, RES, R>(ma, dispose).ToServer();

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Server<RT, REQ, RES, R> use<RT, REQ, RES, R>(Eff<R> ma, Func<R, Unit> dispose) 
            where RT : struct, HasCancel<RT> =>
            Proxy.use<RT, Void, Unit, REQ, RES, R>(ma, dispose).ToServer();

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Server<RT, REQ, RES, R> use<RT, REQ, RES, R>(Aff<RT, R> ma, Func<R, Unit> dispose) 
            where RT : struct, HasCancel<RT> =>
            Proxy.use<RT, Void, Unit, REQ, RES, R>(ma, dispose).ToServer();

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Server<RT, REQ, RES, R> use<RT, REQ, RES, R>(Eff<RT, R> ma, Func<R, Unit> dispose) 
            where RT : struct, HasCancel<RT> =>
            Proxy.use<RT, Void, Unit, REQ, RES, R>(ma, dispose).ToServer();       
        
        /// <summary>
        /// Release a previously used resource
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Server<RT, REQ, RES, Unit> release<RT, REQ, RES, R>(R dispose) 
            where RT : struct, HasCancel<RT> =>
            Proxy.release<RT, Void,Unit, REQ, RES, R>(dispose).ToServer();
    }
}

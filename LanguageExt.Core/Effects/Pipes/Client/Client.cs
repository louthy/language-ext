using System;
using System.Collections.Generic;
using LanguageExt.Effects.Traits;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt.Pipes
{
    /// <summary>
    /// `Client` sends requests of type `REQ` and receives responses of type `RES`.
    /// 
    /// Clients only `request` and never `respond`.
    /// </summary>
    /// <remarks>
    /// 
    ///       Upstream | Downstream
    ///           +---------+
    ///           |         |
    ///     REQ  <==       <== Unit
    ///           |         |
    ///     RES  ==>       ==> Void
    ///           |    |    |
    ///           +----|----+
    ///                |
    ///                A
    /// </remarks>
    public class Client
    {
        /// <summary>
        /// Monad return / pure
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Client<RT, REQ, RES, R> Pure<RT, REQ, RES, R>(R value) where RT : struct, HasCancel<RT> =>
            new Pure<RT, REQ, RES, Unit, Void, R>(value).ToClient();

        /// <summary>
        /// Send a value of type `RES` downstream and block waiting for a reply of type `REQ`
        /// </summary>
        /// <remarks>
        /// `respond` is the identity of the respond category.
        /// </remarks>
        [Pure, MethodImpl(Proxy.mops)]
        public static Client<RT, REQ, RES, RES> request<RT, REQ, RES>(REQ value) where RT : struct, HasCancel<RT> =>
            new Request<RT, REQ, RES, Unit, Void, RES>(value, r => new Pure<RT, REQ, RES, Unit, Void, RES>(r)).ToClient();

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Client<RT, REQ, RES, R> lift<RT, REQ, RES, R>(Aff<R> ma) where RT : struct, HasCancel<RT> =>
            new M<RT, REQ, RES, Unit, Void, R>(ma.Map(Proxy.Pure<RT, REQ, RES, Unit, Void, R>).WithRuntime<RT>()).ToClient();

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Client<RT, REQ, RES, R> lift<RT, REQ, RES, R>(Eff<R> ma) where RT : struct, HasCancel<RT> =>
            new M<RT, REQ, RES, Unit, Void, R>(ma.Map(Proxy.Pure<RT, REQ, RES, Unit, Void, R>).ToAffWithRuntime<RT>()).ToClient();

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Client<RT, REQ, RES, R> lift<RT, REQ, RES, R>(Aff<RT, R> ma) where RT : struct, HasCancel<RT> =>
            new M<RT, REQ, RES, Unit, Void, R>(ma.Map(Proxy.Pure<RT, REQ, RES, Unit, Void, R>)).ToClient();

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Client<RT, REQ, RES, R> lift<RT, REQ, RES, R>(Eff<RT, R> ma) where RT : struct, HasCancel<RT> =>
            new M<RT, REQ, RES, Unit, Void, R>(ma.Map(Proxy.Pure<RT, REQ, RES, Unit, Void, R>).ToAff()).ToClient();

        [Pure, MethodImpl(Proxy.mops)]
        internal static Client<RT, REQ, RES, X> enumerate<RT, REQ, RES, X>(EnumerateData<X> xs)
            where RT : struct, HasCancel<RT> =>
            new Enumerate<RT, REQ, RES, Unit, Void, X, X>(xs, Pure<RT, REQ, RES, X>).ToClient();

        [Pure, MethodImpl(Proxy.mops)]
        public static Client<RT, REQ, RES, X> enumerate<RT, REQ, RES, X>(IEnumerable<X> xs)
            where RT : struct, HasCancel<RT> =>
            new Enumerate<RT, REQ, RES, Unit, Void, X, X>(xs, Pure<RT, REQ, RES, X>).ToClient();

        [Pure, MethodImpl(Proxy.mops)]
        public static Client<RT, REQ, RES, X> enumerate<RT, REQ, RES, X>(IAsyncEnumerable<X> xs)
            where RT : struct, HasCancel<RT> =>
            new Enumerate<RT, REQ, RES, Unit, Void, X, X>(xs, Pure<RT, REQ, RES, X>).ToClient();

        [Pure, MethodImpl(Proxy.mops)]
        public static Client<RT, REQ, RES, X> observe<RT, REQ, RES, X>(IObservable<X> xs)
            where RT : struct, HasCancel<RT> =>
            new Enumerate<RT, REQ, RES, Unit, Void, X, X>(xs, Pure<RT, REQ, RES, X>).ToClient();
        
        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Client<RT, REQ, RES, R> use<RT, REQ, RES, R>(Aff<R> ma) 
            where RT : struct, HasCancel<RT>
            where R : IDisposable =>
            Proxy.use<RT, REQ, RES, Unit, Void, R>(ma).ToClient();

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Client<RT, REQ, RES, R> use<RT, REQ, RES, R>(Eff<R> ma) 
            where RT : struct, HasCancel<RT>
            where R : IDisposable =>
            Proxy.use<RT, REQ, RES, Unit, Void, R>(ma).ToClient();

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Client<RT, REQ, RES, R> use<RT, REQ, RES, R>(Aff<RT, R> ma) 
            where RT : struct, HasCancel<RT> 
            where R : IDisposable =>
            Proxy.use<RT, REQ, RES, Unit, Void, R>(ma).ToClient();

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Client<RT, REQ, RES, R> use<RT, REQ, RES, R>(Eff<RT, R> ma) 
            where RT : struct, HasCancel<RT> 
            where R : IDisposable =>
            Proxy.use<RT, REQ, RES, Unit, Void, R>(ma).ToClient();
        
        
        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Client<RT, REQ, RES, R> use<RT, REQ, RES, R>(Aff<R> ma, Func<R, Unit> dispose) 
            where RT : struct, HasCancel<RT> =>
            Proxy.use<RT, REQ, RES, Unit, Void, R>(ma, dispose).ToClient();

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Client<RT, REQ, RES, R> use<RT, REQ, RES, R>(Eff<R> ma, Func<R, Unit> dispose) 
            where RT : struct, HasCancel<RT> =>
            Proxy.use<RT, REQ, RES, Unit, Void, R>(ma, dispose).ToClient();

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Client<RT, REQ, RES, R> use<RT, REQ, RES, R>(Aff<RT, R> ma, Func<R, Unit> dispose) 
            where RT : struct, HasCancel<RT> =>
            Proxy.use<RT, REQ, RES, Unit, Void, R>(ma, dispose).ToClient();

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Client<RT, REQ, RES, R> use<RT, REQ, RES, R>(Eff<RT, R> ma, Func<R, Unit> dispose) 
            where RT : struct, HasCancel<RT> =>
            Proxy.use<RT, REQ, RES, Unit, Void, R>(ma, dispose).ToClient();      
        
        /// <summary>
        /// Release a previously used resource
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Client<RT, REQ, RES, Unit> release<RT, REQ, RES, R>(R dispose) 
            where RT : struct, HasCancel<RT> =>
            Proxy.release<RT, REQ, RES, Unit, Void, R>(dispose).ToClient();
    }
}

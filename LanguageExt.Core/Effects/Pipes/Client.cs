using System;
using System.Collections.Generic;
using LanguageExt.Effects.Traits;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt.Pipes
{
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
        public static Client<RT, REQ, RES, R> liftIO<RT, REQ, RES, R>(Aff<R> ma) where RT : struct, HasCancel<RT> =>
            new M<RT, REQ, RES, Unit, Void, R>(ma.Map(Proxy.Pure<RT, REQ, RES, Unit, Void, R>).WithRuntime<RT>()).ToClient();

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Client<RT, REQ, RES, R> liftIO<RT, REQ, RES, R>(Eff<R> ma) where RT : struct, HasCancel<RT> =>
            new M<RT, REQ, RES, Unit, Void, R>(ma.Map(Proxy.Pure<RT, REQ, RES, Unit, Void, R>).ToAffWithRuntime<RT>()).ToClient();

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Client<RT, REQ, RES, R> liftIO<RT, REQ, RES, R>(Aff<RT, R> ma) where RT : struct, HasCancel<RT> =>
            new M<RT, REQ, RES, Unit, Void, R>(ma.Map(Proxy.Pure<RT, REQ, RES, Unit, Void, R>)).ToClient();

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Client<RT, REQ, RES, R> liftIO<RT, REQ, RES, R>(Eff<RT, R> ma) where RT : struct, HasCancel<RT> =>
            new M<RT, REQ, RES, Unit, Void, R>(ma.Map(Proxy.Pure<RT, REQ, RES, Unit, Void, R>).ToAff()).ToClient();

        [Pure, MethodImpl(Proxy.mops)]
        public static Client<RT, REQ, RES, X> enumerate<RT, REQ, RES, X>(IEnumerable<X> xs)
            where RT : struct, HasCancel<RT> =>
            new Enumerate<RT, REQ, RES, Unit, Void, X, X>(xs, Pure<RT, REQ, RES, X>).ToClient();

        [Pure, MethodImpl(Proxy.mops)]
        public static Client<RT, REQ, RES, X> enumerate<RT, REQ, RES, X>(IAsyncEnumerable<X> xs)
            where RT : struct, HasCancel<RT> =>
            new Enumerate<RT, REQ, RES, Unit, Void, X, X>(xs, Pure<RT, REQ, RES, X>).ToClient();

        [Pure, MethodImpl(Proxy.mops)]
        public static Client<RT, REQ, RES, X> enumerate<RT, REQ, RES, X>(IObservable<X> xs)
            where RT : struct, HasCancel<RT> =>
            new Observer<RT, REQ, RES, Unit, Void, X, X>(xs, Pure<RT, REQ, RES, X>).ToClient();
    }
}

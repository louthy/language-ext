using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Interfaces;
using static LanguageExt.Pipes.Proxy;

namespace LanguageExt.Pipes
{
    public static class Effect
    {
        [Pure, MethodImpl(Proxy.mops)]
        public static IO<Env, R> RunEffect<Env, R>(this Proxy<Env, Void, Unit, Unit, Void, R> ma) where Env : Cancellable {
            return Go(ma);
            IO<Env, R> Go(Proxy<Env, Void, Unit, Unit, Void, R> p) =>
                p.ToProxy() switch
                {
                    Request<Env, Void, Unit, Unit, Void, R> (var v, var _) => Proxy.closed<IO<Env, R>>(v),
                    Respond<Env, Void, Unit, Unit, Void, R> (var v, var _) => Proxy.closed<IO<Env, R>>(v),
                    M<Env, Void, Unit, Unit, Void, R> (var m)              => m.Clear().Bind(Go),
                    Pure<Env, Void, Unit, Unit, Void, R> (var r)           => IO<Env, R>.Success(r),                                                                                
                    _                                                      => throw new NotSupportedException()
                };
        }
        
        [Pure, MethodImpl(Proxy.mops)]
        public static Effect<Env, R> liftIO<Env, R>(IO<R> ma) where Env : Cancellable =>
            liftIO<Env, Void, Unit, Unit, Void, R>(ma).ToEffect();

        [Pure, MethodImpl(Proxy.mops)]
        public static Effect<Env, R> liftIO<Env, R>(SIO<R> ma) where Env : Cancellable =>
            liftIO<Env, Void, Unit, Unit, Void, R>(ma).ToEffect();

        [Pure, MethodImpl(Proxy.mops)]
        public static Effect<Env, R> liftIO<Env, R>(IO<Env, R> ma) where Env : Cancellable =>
            liftIO<Env, Void, Unit, Unit, Void, R>(ma).ToEffect();

        [Pure, MethodImpl(Proxy.mops)]
        public static Effect<Env, R> liftIO<Env, R>(SIO<Env, R> ma) where Env : Cancellable =>
            liftIO<Env, Void, Unit, Unit, Void, R>(ma).ToEffect();

        
        [Pure, MethodImpl(Proxy.mops)]
        public static Effect<Runtime, R> liftIO<R>(IO<R> ma) =>
            liftIO<Runtime, Void, Unit, Unit, Void, R>(ma).ToEffect();

        [Pure, MethodImpl(Proxy.mops)]
        public static Effect<Runtime, R> liftIO<R>(SIO<R> ma) =>
            liftIO<Runtime, Void, Unit, Unit, Void, R>(ma).ToEffect();

        [Pure, MethodImpl(Proxy.mops)]
        public static Effect<Runtime, R> liftIO<R>(IO<Runtime, R> ma) =>
            liftIO<Runtime, Void, Unit, Unit, Void, R>(ma).ToEffect();

        [Pure, MethodImpl(Proxy.mops)]
        public static Effect<Runtime, R> liftIO<R>(SIO<Runtime, R> ma) =>
            liftIO<Runtime, Void, Unit, Unit, Void, R>(ma).ToEffect();

        
        [Pure, MethodImpl(Proxy.mops)]
        public static Effect<Runtime, Unit> liftIO(IO<Unit> ma) =>
            liftIO<Runtime, Void, Unit, Unit, Void, Unit>(ma).ToEffect();

        [Pure, MethodImpl(Proxy.mops)]
        public static Effect<Runtime, Unit> liftIO(SIO<Unit> ma) =>
            liftIO<Runtime, Void, Unit, Unit, Void, Unit>(ma).ToEffect();

        [Pure, MethodImpl(Proxy.mops)]
        public static Effect<Runtime, Unit> liftIO(IO<Runtime, Unit> ma) =>
            liftIO<Runtime, Void, Unit, Unit, Void, Unit>(ma).ToEffect();

        [Pure, MethodImpl(Proxy.mops)]
        public static Effect<Runtime, Unit> liftIO(SIO<Runtime, Unit> ma) =>
            liftIO<Runtime, Void, Unit, Unit, Void, Unit>(ma).ToEffect();
    }
}

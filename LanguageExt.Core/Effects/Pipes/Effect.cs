using System;
using LanguageExt.Effects.Traits;
using System.Diagnostics.Contracts;
using static LanguageExt.Pipes.Proxy;
using System.Runtime.CompilerServices;
using static LanguageExt.Prelude;

namespace LanguageExt.Pipes
{
    public static class Effect
    {
        [Pure, MethodImpl(Proxy.mops)]
        public static Aff<Env, R> RunEffect<Env, R>(this Proxy<Env, Void, Unit, Unit, Void, R> ma) where Env : struct, HasCancel<Env> {
            return Go(ma);
            Aff<Env, R> Go(Proxy<Env, Void, Unit, Unit, Void, R> p) =>
                p.ToProxy() switch
                {
                    Request<Env, Void, Unit, Unit, Void, R> (var v, var _) => Proxy.closed<Aff<Env, R>>(v),
                    Respond<Env, Void, Unit, Unit, Void, R> (var v, var _) => Proxy.closed<Aff<Env, R>>(v),
                    M<Env, Void, Unit, Unit, Void, R> (var m)              => m.Clone().Bind(Go),
                    Pure<Env, Void, Unit, Unit, Void, R> (var r)           => Aff<Env, R>.Success(r),                                                                                
                    _                                                      => throw new NotSupportedException()
                };
        }

        [Pure, MethodImpl(Proxy.mops)]
        public static Effect<Env, R> liftIO<Env, R>(Aff<R> ma) where Env : struct, HasCancel<Env> =>
            liftIO<Env, Void, Unit, Unit, Void, R>(ma).ToEffect();

        [Pure, MethodImpl(Proxy.mops)]
        public static Effect<Env, R> liftIO<Env, R>(Eff<R> ma) where Env : struct, HasCancel<Env> =>
            liftIO<Env, Void, Unit, Unit, Void, R>(ma).ToEffect();

        [Pure, MethodImpl(Proxy.mops)]
        public static Effect<Env, R> liftIO<Env, R>(Aff<Env, R> ma) where Env : struct, HasCancel<Env> =>
            liftIO<Env, Void, Unit, Unit, Void, R>(ma).ToEffect();

        [Pure, MethodImpl(Proxy.mops)]
        public static Effect<Env, R> liftIO<Env, R>(Eff<Env, R> ma) where Env : struct, HasCancel<Env> =>
            liftIO<Env, Void, Unit, Unit, Void, R>(ma).ToEffect();
    }
}

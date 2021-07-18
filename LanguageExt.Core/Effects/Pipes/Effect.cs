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
        public static Aff<RT, R> RunEffect<RT, R>(this Proxy<RT, Void, Unit, Unit, Void, R> ma) where RT : struct, HasCancel<RT> {
            return Go(ma);
            static Aff<RT, R> Go(Proxy<RT, Void, Unit, Unit, Void, R> p) =>
                p.ToProxy() switch
                {
                    Request<RT, Void, Unit, Unit, Void, R> (var v, var _) => Proxy.closed<Aff<RT, R>>(v),
                    Respond<RT, Void, Unit, Unit, Void, R> (var v, var _) => Proxy.closed<Aff<RT, R>>(v),
                    M<RT, Void, Unit, Unit, Void, R> (var m)              => m.Clone().Bind(Go),
                    Pure<RT, Void, Unit, Unit, Void, R> (var r)           => Aff<RT, R>.Success(r),                                                                                
                    _                                                      => throw new NotSupportedException()
                };
        }

        [Pure, MethodImpl(Proxy.mops)]
        public static Effect<RT, R> liftIO<RT, R>(Aff<R> ma) where RT : struct, HasCancel<RT> =>
            liftIO<RT, Void, Unit, Unit, Void, R>(ma).ToEffect();

        [Pure, MethodImpl(Proxy.mops)]
        public static Effect<RT, R> liftIO<RT, R>(Eff<R> ma) where RT : struct, HasCancel<RT> =>
            liftIO<RT, Void, Unit, Unit, Void, R>(ma).ToEffect();

        [Pure, MethodImpl(Proxy.mops)]
        public static Effect<RT, R> liftIO<RT, R>(Aff<RT, R> ma) where RT : struct, HasCancel<RT> =>
            liftIO<RT, Void, Unit, Unit, Void, R>(ma).ToEffect();

        [Pure, MethodImpl(Proxy.mops)]
        public static Effect<RT, R> liftIO<RT, R>(Eff<RT, R> ma) where RT : struct, HasCancel<RT> =>
            liftIO<RT, Void, Unit, Unit, Void, R>(ma).ToEffect();
    }
}

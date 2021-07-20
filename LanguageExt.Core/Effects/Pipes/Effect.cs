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
        public static Aff<RT, R> RunEffect_Original<RT, R>(this Proxy<RT, Void, Unit, Unit, Void, R> ma) where RT : struct, HasCancel<RT> {
            return Go(ma);
            static Aff<RT, R> Go(Proxy<RT, Void, Unit, Unit, Void, R> p) =>
                p.ToProxy() switch
                {
                    Request<RT, Void, Unit, Unit, Void, R> (var v, var _) => Proxy.closed<Aff<RT, R>>(v),
                    Respond<RT, Void, Unit, Unit, Void, R> (var v, var _) => Proxy.closed<Aff<RT, R>>(v),
                    M<RT, Void, Unit, Unit, Void, R> (var m)              => m.Clone().Bind(Go),
                    Pure<RT, Void, Unit, Unit, Void, R> (var r)           => Aff<RT, R>.Success(r),                                                                                
                    _                                                     => throw new NotSupportedException()
                };
        }

        [Pure, MethodImpl(Proxy.mops)]
        public static Aff<RT, R> RunEffect<RT, R>(this Proxy<RT, Void, Unit, Unit, Void, R> ma) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, R>(async env =>
                            {
                                var p = ma;
                                while (true)
                                {
                                    switch (p.ToProxy())
                                    {
                                        case Request<RT, Void, Unit, Unit, Void, R> (var v, var _):
                                            return await Proxy.closed<Aff<RT, R>>(v).Run(env);

                                        case Respond<RT, Void, Unit, Unit, Void, R> (var v, var _):
                                            return await Proxy.closed<Aff<RT, R>>(v).Run(env);
                                        
                                        case Repeat<RT, Void, Unit, Unit, Void, R> (var inner):
                                            while (true)
                                            {
                                                var fi = await inner.RunEffect<RT, R>().Run(env);
                                                if (fi.IsFail) return fi.Error;
                                            }

                                        case Pure<RT, Void, Unit, Unit, Void, R> (var r):
                                            return FinSucc<R>(r);
                                        
                                        case M<RT, Void, Unit, Unit, Void, R> (var m):
                                            var fp = await m.Clone().Run(env);
                                            if (fp.IsFail) return fp.Error;
                                            p = fp.Value.ToProxy();
                                            break;
                                    }
                                }
                            });

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

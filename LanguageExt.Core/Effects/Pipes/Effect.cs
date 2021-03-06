using System;
using LanguageExt.Effects.Traits;
using System.Diagnostics.Contracts;
using static LanguageExt.Pipes.Proxy;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Pipes
{
    public static class Effect
    {
        [Pure]
        internal static Aff<RT, R> RunEffect_Original<RT, R>(this Proxy<RT, Void, Unit, Unit, Void, R> ma) where RT : struct, HasCancel<RT> {
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

        [Pure]
        public static Aff<RT, R> RunEffect<RT, R>(this Proxy<RT, Void, Unit, Unit, Void, R> ma) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, R>(async env =>
                            {
                                var p = ma;
                                while (!env.CancellationToken.IsCancellationRequested)
                                {
                                    switch (p.ToProxy())
                                    {
                                        case Pure<RT, Void, Unit, Unit, Void, R> (var r):
                                            return FinSucc<R>(r);
                                        
                                        case M<RT, Void, Unit, Unit, Void, R> (var m):
                                            var fp = await m.ReRun(env).ConfigureAwait(false);
                                            if (fp.IsFail) return fp.Error;
                                            p = fp.Value.ToProxy();
                                            break;
                                        
                                        case Repeat<RT, Void, Unit, Unit, Void, R> (var inner):
                                            var effect = inner.RunEffect<RT, R>();
                                            while (true)
                                            {
                                                var fi = await effect.ReRun(env).ConfigureAwait(false);
                                                if (fi.IsFail) return fi.Error;
                                                if (fi.Value is IDisposable d)
                                                {
                                                    d.Dispose();
                                                }
                                            }
                                            
                                        case Enumerate<RT, Void, Unit, Unit, Void, R> me:
                                            Fin<R> lastResult = Errors.SequenceEmpty;

                                            if (me.IsAsync)
                                            {
                                                await foreach (var f in me.MakeEffectsAsync().ConfigureAwait(false))
                                                {
                                                    lastResult = await f.RunEffect<RT, R>().Run(env).ConfigureAwait(false);
                                                    if (lastResult.IsFail) return lastResult.Error;
                                                }
                                            }
                                            else
                                            {
                                                foreach (var f in me.MakeEffects())
                                                {
                                                    lastResult = await f.RunEffect<RT, R>().Run(env).ConfigureAwait(false);
                                                    if (lastResult.IsFail) return lastResult.Error;
                                                }
                                            }

                                            return lastResult;
                                            
                                        case Observer<RT, Void, Unit, Unit, Void, R> obs:
                                            var    wait     = new AutoResetEvent(false);
                                            var    lastTask = unit.AsValueTask();
                                            Fin<R> last     = Errors.Cancelled;

                                            // Not sure if launching the effects without an await makes sense here
                                            using (var sub = obs.Subscribe(onNext: fx => lastTask = fx.RunEffect<RT, R>().Run(env).Iter(r => last = r),
                                                                           onError: err =>
                                                                                    {
                                                                                        last = err;
                                                                                        wait.Set();
                                                                                    },
                                                                           onCompleted: () => wait.Set()))
                                            {
                                                await wait.WaitOneAsync(env.CancellationToken).ConfigureAwait(false);
                                                await lastTask.ConfigureAwait(false);
                                                return last;
                                            }
                                            
                                        case Request<RT, Void, Unit, Unit, Void, R> (var v, var _):
                                            return Errors.Closed;

                                        case Respond<RT, Void, Unit, Unit, Void, R> (var v, var _):
                                            return Errors.Closed;
                                    }
                                }

                                return Errors.Cancelled;
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

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
    /// <summary>
    /// Effects represent a 'fused' set of producer, pipes, and consumer into one type.
    /// 
    /// It neither can neither `yield` nor be `awaiting`, it represents an entirely closed effect system.
    /// </summary>
    /// <remarks>
    ///       Upstream | Downstream
    ///           +---------+
    ///           |         |
    ///     Void <==       <== Unit
    ///           |         |
    ///     Unit ==>       ==> Void
    ///           |    |    |
    ///           +----|----+
    ///                |
    ///                A
    /// </remarks>
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
                    M<RT, Void, Unit, Unit, Void, R> (var m)              => m.Bind(Go),
                    Pure<RT, Void, Unit, Unit, Void, R> (var r)           => Aff<RT, R>.Success(r),                                                                                
                    _                                                     => throw new NotSupportedException()
                };
        }

        [Pure]
        public static Aff<RT, Unit> RunEffectUnit<RT>(this Proxy<RT, Void, Unit, Unit, Void, Unit> ma) where RT : struct, HasCancel<RT> =>
            ma.RunEffect<RT, Unit>() | @catch(Errors.SequenceEmpty, unitEff);

        [Pure]
        public static Aff<RT, R> RunEffect<RT, R>(this Proxy<RT, Void, Unit, Unit, Void, R> ma) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, R>(async env =>
                            {
                                var disps = new ConcurrentDictionary<object, IDisposable>(new ReferenceEqualityComparer<object>());
                                try
                                {
                                    return await RunEffect(ma, disps).Run(env);
                                }
                                finally
                                {
                                    foreach (var disp in disps)
                                    {
                                        disp.Value?.Dispose();
                                    }
                                }
                            });

        [Pure]
        static Aff<RT, R> RunEffect<RT, R>(this Proxy<RT, Void, Unit, Unit, Void, R> ma, ConcurrentDictionary<object, IDisposable> disps) where RT : struct, HasCancel<RT> =>
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
                                            var fp = await m.Run(env).ConfigureAwait(false);
                                            if (fp.IsFail) return fp.Error;
                                            p = fp.Value.ToProxy();
                                            break;

                                        case Repeat<RT, Void, Unit, Unit, Void, R> (var inner):
                                            var effect = inner.RunEffect<RT, R>(disps);
                                            while (!env.CancellationToken.IsCancellationRequested)
                                            {
                                                var fi = await effect.Run(env).ConfigureAwait(false);
                                                if (fi.IsFail) return fi.Error;
                                                if (fi.Value is IDisposable d)
                                                {
                                                    d.Dispose();
                                                }
                                            }

                                            return Errors.Cancelled;

                                        case Enumerate<RT, Void, Unit, Unit, Void, R> me:
                                            Fin<R> lastResult = Errors.SequenceEmpty;

                                            switch (me.Type)
                                            {
                                                case EnumerateDataType.AsyncEnumerable:
                                                    await foreach (var f in me.MakeEffectsAsync().ConfigureAwait(false))
                                                    {
                                                        if (env.CancellationToken.IsCancellationRequested) return Errors.Cancelled;
                                                        lastResult = await f.RunEffect(disps).Run(env).ConfigureAwait(false);
                                                        if (lastResult.IsFail) return lastResult.Error;
                                                    }

                                                    return lastResult;

                                                case EnumerateDataType.Enumerable:
                                                    foreach (var f in me.MakeEffects())
                                                    {
                                                        if (env.CancellationToken.IsCancellationRequested) return Errors.Cancelled;
                                                        lastResult = await f.RunEffect(disps).Run(env).ConfigureAwait(false);
                                                        if (lastResult.IsFail) return lastResult.Error;
                                                    }

                                                    return lastResult;

                                                case EnumerateDataType.Observable:
                                                    var    wait     = new AutoResetEvent(false);
                                                    var    lastTask = unit.AsValueTask();
                                                    Fin<R> last     = Errors.Cancelled;

                                                    using (var sub = me.Subscribe(onNext: fx => lastTask = fx.RunEffect(disps).Run(env).Iter(r => last = r),
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
                                                    
                                                default:
                                                    throw new NotSupportedException();
                                            }

                                        case Use<RT, Void, Unit, Unit, Void, R> mu:
                                            p = mu.Run(disps);
                                            break;

                                        case Release<RT, Void, Unit, Unit, Void, R> mu:
                                            p = mu.Run(disps);
                                            break;
          
                                        case Request<RT, Void, Unit, Unit, Void, R> (var v, var _):
                                            return Errors.Closed;

                                        case Respond<RT, Void, Unit, Unit, Void, R> (var v, var _):
                                            return Errors.Closed;
                                    }
                                }

                                return Errors.Cancelled;
                            });

        [Pure, MethodImpl(Proxy.mops)]
        public static Effect<RT, R> lift<RT, R>(Aff<R> ma) where RT : struct, HasCancel<RT> =>
            lift<RT, Void, Unit, Unit, Void, R>(ma).ToEffect();

        [Pure, MethodImpl(Proxy.mops)]
        public static Effect<RT, R> lift<RT, R>(Eff<R> ma) where RT : struct, HasCancel<RT> =>
            lift<RT, Void, Unit, Unit, Void, R>(ma).ToEffect();

        [Pure, MethodImpl(Proxy.mops)]
        public static Effect<RT, R> lift<RT, R>(Aff<RT, R> ma) where RT : struct, HasCancel<RT> =>
            lift<RT, Void, Unit, Unit, Void, R>(ma).ToEffect();

        [Pure, MethodImpl(Proxy.mops)]
        public static Effect<RT, R> lift<RT, R>(Eff<RT, R> ma) where RT : struct, HasCancel<RT> =>
            lift<RT, Void, Unit, Unit, Void, R>(ma).ToEffect();
    }
}

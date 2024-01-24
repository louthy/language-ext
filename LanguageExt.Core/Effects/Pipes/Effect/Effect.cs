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
        /*
         
         REFERENCE IMPLEMENTATION
         
         */
         
        [Pure]
        internal static Transducer<RT, Sum<Error, R>> RunEffect_REFERENCE<RT, R>(this Proxy<RT, Void, Unit, Unit, Void, R> ma) 
            where RT : HasIO<RT, Error> 
        {
            return Go(ma);
            
            Transducer<RT, Sum<Error, R>> Go(Proxy<RT, Void, Unit, Unit, Void, R> p) =>
                p.ToProxy() switch
                {
                    M<RT, Void, Unit, Unit, Void, R> (var mmx)        => mmx.Map(mx => mx.Map(Go)).Flatten(),
                    Pure<RT, Void, Unit, Unit, Void, R> (var r)       => Transducer.constant<RT, Sum<Error, R>>(Sum<Error, R>.Right(r)),                                                                                
                    Enumerate<RT, Void, Unit, Unit, Void, R> me       => EnumerateCase(me, disps),
                    Request<RT, Void, Unit, Unit, Void, R> (var v, _) => closed<Transducer<RT, Sum<Error, R>>>(v),
                    Respond<RT, Void, Unit, Unit, Void, R> (var v, _) => closed<Transducer<RT, Sum<Error, R>>>(v),
                    _                                                 => throw new NotSupportedException()
                };
        }        

        [Pure]
        public static Eff<RT, Unit> RunEffectUnit<RT>(this Proxy<RT, Void, Unit, Unit, Void, Unit> ma) where RT : HasIO<RT, Error> =>
            ma.RunEffect() | @catch(Errors.SequenceEmpty, unitEff);

        [Pure]
        public static Transducer<RT, Sum<Error, R>> RunEffect<RT, R>(this Proxy<RT, Void, Unit, Unit, Void, R> ma) where RT : HasIO<RT, Error> =>
            Eff<RT, R>.LiftIO(async env =>
            {
                var disps = new ConcurrentDictionary<object, IDisposable>(new ReferenceEqualityComparer<object>());
                try
                {
                    return await RunEffect(ma, disps).RunAsync(env);
                }
                finally
                {
                    foreach (var disp in disps)
                    {
                        disp.Value?.Dispose();
                    }
                }
            }).Morphism;

        /*static Transducer<RT, Sum<Error, R>> EnumerateCase<RT, R>(
            Enumerate<RT, Void, Unit, Unit, Void, R> me,
            ConcurrentDictionary<object, IDisposable> disps)
            where RT : HasIO<RT, Error> =>
            Eff<RT, R>.LiftIO(async (RT env) =>
            {
                Fin<Unit> lastResult = Errors.SequenceEmpty;

                switch (me.Type)
                {
                    case EnumerateDataType.AsyncEnumerable:
                        await foreach (var f in me.MakeEffectsAsync().ConfigureAwait(false))
                        {
                            if (env.CancellationToken.IsCancellationRequested) return Errors.Cancelled;
                            lastResult = await f.RunEffect(disps).RunAsync(env).ConfigureAwait(false);
                            if (lastResult.IsFail) return lastResult.Error;
                        }

                        return await me.Next(default).RunEffect(disps).RunAsync(env).ConfigureAwait(false);

                    case EnumerateDataType.Enumerable:
                        foreach (var f in me.MakeEffects())
                        {
                            if (env.CancellationToken.IsCancellationRequested) return Errors.Cancelled;
                            lastResult = await f.RunEffect(disps).RunAsync(env).ConfigureAwait(false);
                            if (lastResult.IsFail) return lastResult.Error;
                        }

                        return await me.Next(default).RunEffect(disps).RunAsync(env).ConfigureAwait(false);

                    case EnumerateDataType.Observable:
                    {
                        using var wait = new AutoResetEvent(false);
                        var lastTask = unit.AsTask();
                        Fin<Unit> last = Errors.Cancelled;

                        using (var sub = me.Subscribe(
                                   onNext: fx =>
                                       lastTask = fx.RunEffect(disps).RunAsync(env).Iter(r => last = r),
                                   onError: err =>
                                   {
                                       last = err;
                                       wait.Set();
                                   },
                                   onCompleted: () => wait.Set()))
                        {
                            await wait.WaitOneAsync(env.CancellationToken).ConfigureAwait(false);
                            await lastTask.ConfigureAwait(false);
                            return await me.Next(default).RunEffect(disps).RunAsync(env).ConfigureAwait(false);
                        }
                    }

                    default:
                        throw new NotSupportedException();
                }
            });*/

        [Pure]
        static Transducer<RT, Sum<Error, R>> RunEffect<RT, R>(
            this Proxy<RT, Void, Unit, Unit, Void, R> ma,
            ConcurrentDictionary<object, IDisposable> disps) 
            where RT : HasIO<RT, Error> =>
            Eff<RT, R>.LiftIO(
                async env =>
                {
                    var p = ma;

                    while (!env.CancellationToken.IsCancellationRequested)
                    {
                        switch (p.ToProxy())
                        {
                            case Pure<RT, Void, Unit, Unit, Void, R> (var r):
                                return FinSucc(r);

                            case M<RT, Void, Unit, Unit, Void, R> (var m):
                                var fp = await m.Run(env).ConfigureAwait(false);
                                if (fp.IsFail) return fp.Error;
                                p = fp.Value.ToProxy();
                                break;

                            case Enumerate<RT, Void, Unit, Unit, Void, R> me:
                            {
                                Fin<Unit> lastResult = Errors.SequenceEmpty;

                                switch (me.Type)
                                {
                                    case EnumerateDataType.AsyncEnumerable:
                                        await foreach (var f in me.MakeEffectsAsync().ConfigureAwait(false))
                                        {
                                            if (env.CancellationToken.IsCancellationRequested) return Errors.Cancelled;
                                            lastResult = await f.RunEffect(disps).RunAsync(env).ConfigureAwait(false);
                                            if (lastResult.IsFail) return lastResult.Error;
                                        }

                                        p = me.Next(default);
                                        break;

                                    case EnumerateDataType.Enumerable:
                                        foreach (var f in me.MakeEffects())
                                        {
                                            if (env.CancellationToken.IsCancellationRequested) return Errors.Cancelled;
                                            lastResult = await f.RunEffect(disps).RunAsync(env).ConfigureAwait(false);
                                            if (lastResult.IsFail) return lastResult.Error;
                                        }
                                        p = me.Next(default);
                                        break;

                                    case EnumerateDataType.Observable:
                                    {
                                        using var wait = new AutoResetEvent(false);
                                        var lastTask = unit.AsTask();
                                        Fin<Unit> last = Errors.Cancelled;

                                        using (var sub = me.Subscribe(
                                                   onNext: fx =>
                                                       lastTask = fx.RunEffect(disps).RunAsync(env).Iter(r => last = r),
                                                   onError: err =>
                                                   {
                                                       last = err;
                                                       wait.Set();
                                                   },
                                                   onCompleted: () => wait.Set()))
                                        {
                                            await wait.WaitOneAsync(env.CancellationToken).ConfigureAwait(false);
                                            await lastTask.ConfigureAwait(false);
                                            p = me.Next(default);
                                            break;
                                        }
                                    }

                                    default:
                                        throw new NotSupportedException();
                                }
                            } 
                            break;

                            case Request<RT, Void, Unit, Unit, Void, R>:
                                return Errors.Closed;

                            case Respond<RT, Void, Unit, Unit, Void, R>:
                                return Errors.Closed;
                        }
                    }

                    return Errors.Cancelled;
                }).Morphism;

        
        [Pure, MethodImpl(mops)]
        public static Effect<RT, R> lift<RT, R>(Aff<R> ma) where RT : HasIO<RT, Error> =>
            lift<RT, Void, Unit, Unit, Void, R>(ma).ToEffect();

        [Pure, MethodImpl(mops)]
        public static Effect<RT, R> lift<RT, R>(Eff<R> ma) where RT : HasIO<RT, Error> =>
            lift<RT, Void, Unit, Unit, Void, R>(ma).ToEffect();

        [Pure, MethodImpl(mops)]
        public static Effect<RT, R> lift<RT, R>(Aff<RT, R> ma) where RT : HasIO<RT, Error> =>
            lift<RT, Void, Unit, Unit, Void, R>(ma).ToEffect();

        [Pure, MethodImpl(mops)]
        public static Effect<RT, R> lift<RT, R>(Eff<RT, R> ma) where RT : HasIO<RT, Error> =>
            lift<RT, Void, Unit, Unit, Void, R>(ma).ToEffect();
    }
}

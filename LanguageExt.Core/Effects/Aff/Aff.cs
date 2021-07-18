using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using LanguageExt.Common;
using System.Threading.Tasks;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;
using LanguageExt.Effects.Traits;
using LanguageExt.Thunks;

namespace LanguageExt
{
    /// <summary>
    /// Asynchronous effect monad
    /// </summary>
    public struct Aff<Env, A> 
        where Env : struct, HasCancel<Env>
    {
        internal ThunkAsync<Env, A> Thunk => thunk ?? ThunkAsync<Env, A>.Fail(Errors.Bottom);
        ThunkAsync<Env, A> thunk;

        /// <summary>
        /// Constructor
        /// </summary>
        [MethodImpl(AffOpt.mops)]
        internal Aff(ThunkAsync<Env, A> thunk) =>
            this.thunk = thunk ?? throw new ArgumentNullException(nameof(thunk));

        /// <summary>
        /// Invoke the effect
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public ValueTask<Fin<A>> Run(in Env env) =>
            Thunk.Value(env);

        /// <summary>
        /// Invoke the effect
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public ValueTask<Fin<A>> ReRun(Env env) =>
            Thunk.ReValue(env);

        /// <summary>
        /// Clone the effect
        /// </summary>
        /// <remarks>
        /// If the effect had already run, then this state will be wiped in the clone, meaning it can be re-run
        /// </remarks>
        [Pure, MethodImpl(AffOpt.mops)]
        public Aff<Env, A> Clone() =>
            new Aff<Env, A>(Thunk.Clone());        

        /// <summary>
        /// Invoke the effect
        /// </summary>
        [MethodImpl(AffOpt.mops)]
        public async ValueTask<Unit> RunUnit(Env env) =>
            ignore(await Thunk.Value(env).ConfigureAwait(false));

        /// <summary>
        /// Launch the async computation without awaiting the result
        /// </summary>
        /// <remarks>
        /// If the parent expression has `cancel` called on it, then it will also cancel the forked child
        /// expression.
        ///
        /// `Fork` returns an `Eff<Unit>` as its bound result value.  If you run it, it will cancel the
        /// forked child expression.
        /// </remarks>
        /// <returns>Returns an `Eff<Unit>` as its bound value.  If it runs, it will cancel the
        /// forked child expression</returns>
        [MethodImpl(AffOpt.mops)]
        public Eff<Env, Eff<Unit>> Fork()
        {
            var t = Thunk;
            return Eff<Env, Eff<Unit>>(
                env =>
                {
                    // Create a new local runtime with its own cancellation token
                    var lenv = env.LocalCancel;
                    
                    // If the parent cancels, we should too
                    env.CancellationToken.Register(() => lenv.CancellationTokenSource.Cancel());
                    
                    // Run
                    ignore(t.Value(lenv));
                    
                    // Return an effect that cancels the fire-and-forget expression
                    return Eff<Unit>(() =>
                                     {
                                         lenv.CancellationTokenSource.Cancel();
                                         return unit;
                                     });
                });
        }

        /// <summary>
        /// Lift an asynchronous effect into the Aff monad
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> Effect(Func<Env, ValueTask<A>> f) =>
            new Aff<Env, A>(ThunkAsync<Env, A>.Lazy(f));

        /// <summary>
        /// Lift an asynchronous effect into the Aff monad
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> EffectMaybe(Func<Env, ValueTask<Fin<A>>> f) =>
            new Aff<Env, A>(ThunkAsync<Env, A>.Lazy(f));

        /// <summary>
        /// Lift an asynchronous effect into the Aff monad
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, Unit> Effect(Func<Env, ValueTask> f) =>
            new Aff<Env, Unit>(ThunkAsync<Env, Unit>.Lazy(async e =>
            {
                await f(e).ConfigureAwait(false);
                return Fin<Unit>.Succ(default);
            }));

        /// <summary>
        /// Lift a value into the Aff monad 
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> Success(A value) =>
            new Aff<Env, A>(ThunkAsync<Env, A>.Success(value));

        /// <summary>
        /// Lift a failure into the Aff monad 
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> Fail(Error error) =>
            new Aff<Env, A>(ThunkAsync<Env, A>.Fail(error));

        /// <summary>
        /// Force the operation to end after a time out delay
        /// </summary>
        /// <param name="timeoutDelay">Delay for the time out</param>
        /// <returns>Either success if the operation completed before the timeout, or Errors.TimedOut</returns>
        [Pure, MethodImpl(AffOpt.mops)]
        public Aff<Env, A> Timeout(TimeSpan timeoutDelay)
        {
            var t = Thunk;
            return AffMaybe<Env, A>(
                async env =>
                {
                    var delay = Task.Delay(timeoutDelay);
                    var task  = t.Value(env).AsTask();
                    await Task.WhenAny( new Task[] { delay, task }).ConfigureAwait(false);
                    if (delay.IsCompleted)
                    {
                        env.CancellationTokenSource.Cancel();
                        return FinFail<A>(Errors.TimedOut);
                    }
                    else
                    {
                        return await task;
                    }
                });
        }

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> operator |(Aff<Env, A> ma, Aff<Env, A> mb) =>
            new Aff<Env, A>(ThunkAsync<Env, A>.Lazy(
                                async env =>
                                {
                                    var ra = await ma.Run(env).ConfigureAwait(false);
                                    return ra.IsSucc
                                               ? ra
                                               : await mb.Run(env).ConfigureAwait(false);
                                }));

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> operator |(Aff<Env, A> ma, Aff<A> mb) =>
            new Aff<Env, A>(ThunkAsync<Env, A>.Lazy(
                                async env =>
                                {
                                    var ra = await ma.Run(env).ConfigureAwait(false);
                                    return ra.IsSucc
                                               ? ra
                                               : await mb.Run().ConfigureAwait(false);
                                }));

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> operator |(Aff<A> ma, Aff<Env, A> mb) =>
            new Aff<Env, A>(ThunkAsync<Env, A>.Lazy(
                                async env =>
                                {
                                    var ra = await ma.Run().ConfigureAwait(false);
                                    return ra.IsSucc
                                               ? ra
                                               : await mb.Run(env).ConfigureAwait(false);
                                }));

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> operator |(Aff<Env, A> ma, Eff<Env, A> mb) =>
            new Aff<Env, A>(ThunkAsync<Env, A>.Lazy(
                                async env =>
                                {
                                    var ra = await ma.Run(env).ConfigureAwait(false);
                                    return ra.IsSucc
                                               ? ra
                                               : mb.Run(env);
                                }));

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> operator |(Eff<Env, A> ma, Aff<Env, A> mb) =>
            new Aff<Env, A>(ThunkAsync<Env, A>.Lazy(
                                async env =>
                                {
                                    var ra = ma.Run(env);
                                    return ra.IsSucc
                                               ? ra
                                               : await mb.Run(env).ConfigureAwait(false);
                                }));

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> operator |(Eff<A> ma, Aff<Env, A> mb) =>
            new Aff<Env, A>(ThunkAsync<Env, A>.Lazy(
                                async env =>
                                {
                                    var ra = ma.Run();
                                    return ra.IsSucc
                                               ? ra
                                               : await mb.Run(env).ConfigureAwait(false);
                                }));

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> operator |(Aff<Env, A> ma, Eff<A> mb) =>
            new Aff<Env, A>(ThunkAsync<Env, A>.Lazy(
                                async env =>
                                {
                                    var ra = await ma.Run(env).ConfigureAwait(false);
                                    return ra.IsSucc
                                               ? ra
                                               : mb.Run();
                                }));

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> operator |(Aff<Env, A> ma, EffCatch<A> mb) =>
            new Aff<Env, A>(ThunkAsync<Env, A>.Lazy(
                                async env =>
                                {
                                    var ra = await ma.Run(env).ConfigureAwait(false);
                                    return ra.IsSucc
                                               ? ra
                                               : mb.Run(ra.Error);
                                }));

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> operator |(Aff<Env, A> ma, AffCatch<A> mb) =>
            new Aff<Env, A>(ThunkAsync<Env, A>.Lazy(
                                async env =>
                                {
                                    var ra = await ma.Run(env).ConfigureAwait(false);
                                    return ra.IsSucc
                                               ? ra
                                               : await mb.Run(ra.Error).ConfigureAwait(false);
                                }));

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> operator |(Aff<Env, A> ma, EffCatch<Env, A> mb) =>
            new Aff<Env, A>(ThunkAsync<Env, A>.Lazy(
                                async env =>
                                {
                                    var ra = await ma.Run(env).ConfigureAwait(false);
                                    return ra.IsSucc
                                               ? ra
                                               : mb.Run(env, ra.Error);
                                }));

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> operator |(Aff<Env, A> ma, AffCatch<Env, A> mb) =>
            new Aff<Env, A>(ThunkAsync<Env, A>.Lazy(
                                async env =>
                                {
                                    var ra = await ma.Run(env).ConfigureAwait(false);
                                    return ra.IsSucc
                                               ? ra
                                               : await mb.Run(env, ra.Error).ConfigureAwait(false);
                                }));

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> operator |(Aff<Env, A> ma, CatchValue<A> value) =>
            new Aff<Env, A>(ThunkAsync<Env, A>.Lazy(
                                async env =>
                                {
                                    var ra = await ma.Run(env).ConfigureAwait(false);
                                    return ra.IsSucc
                                               ? ra
                                               : value.Match(ra.Error)
                                                   ? FinSucc(value.Value(ra.Error))
                                                   : ra;
                                }));

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> operator |(Aff<Env, A> ma, CatchError value) =>
            new Aff<Env, A>(ThunkAsync<Env, A>.Lazy(
                                async env =>
                                {
                                    var ra = await ma.Run(env).ConfigureAwait(false);
                                    return ra.IsSucc
                                               ? ra
                                               : value.Match(ra.Error)
                                                   ? FinFail<A>(value.Value(ra.Error))
                                                   : ra;
                                }));

        /// <summary>
        /// Implicit conversion from pure Aff
        /// </summary>
        public static implicit operator Aff<Env, A>(Aff<A> ma) =>
            EffectMaybe(env => ma.Run());

        /// <summary>
        /// Implicit conversion from pure Eff
        /// </summary>
        public static implicit operator Aff<Env, A>(Eff<A> ma) =>
            EffectMaybe(env => ma.Run().AsValueTask());

        /// <summary>
        /// Implicit conversion from Eff
        /// </summary>
        public static implicit operator Aff<Env, A>(Eff<Env, A> ma) =>
            EffectMaybe(env => ma.Run(env).AsValueTask());
    }

    public static partial class AffExtensions
    {
        //
        // Sequence (with tuples)
        //
        
        /// <summary>
        /// Run the two effects in the tuple in parallel, wait for them all to finish, then return a tuple of the results
        /// </summary>
        public static Aff<Env, (A, B)> Sequence<Env, A, B>(this (Aff<Env, A>, Aff<Env, B>) ms) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env,(A, B)>(async env =>
            {
                var t1 = ms.Item1.Run(env).AsTask();
                var t2 = ms.Item2.Run(env).AsTask();
                
                var tasks = new Task[] {t1, t2};
                await Task.WhenAll(tasks);
                return from r1 in t1.Result
                       from r2 in t2.Result
                       select (r1, r2);
            });

        /// <summary>
        /// Run the three effects in the tuple in parallel, wait for them all to finish, then return a tuple of the results
        /// </summary>
        public static Aff<Env, (A, B, C)> Sequence<Env, A, B, C>(this (Aff<Env, A>, Aff<Env, B>, Aff<Env, C>) ms) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env,(A, B, C)>(async env =>
            {
                var t1 = ms.Item1.Run(env).AsTask();
                var t2 = ms.Item2.Run(env).AsTask();
                var t3 = ms.Item3.Run(env).AsTask();
                
                var tasks = new Task[] {t1, t2, t3};
                await Task.WhenAll(tasks);
                return from r1 in t1.Result
                       from r2 in t2.Result
                       from r3 in t3.Result
                       select (r1, r2, r3);
            });

        /// <summary>
        /// Run the four effects in the tuple in parallel, wait for them all to finish, then return a tuple of the results
        /// </summary>
        public static Aff<Env, (A, B, C, D)> Sequence<Env, A, B, C, D>(this (Aff<Env, A>, Aff<Env, B>, Aff<Env, C>, Aff<Env, D>) ms) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env,(A, B, C, D)>(async env =>
            {
                var t1 = ms.Item1.Run(env).AsTask();
                var t2 = ms.Item2.Run(env).AsTask();
                var t3 = ms.Item3.Run(env).AsTask();
                var t4 = ms.Item4.Run(env).AsTask();
                
                var tasks = new Task[] {t1, t2, t3, t4};
                await Task.WhenAll(tasks);
                return from r1 in t1.Result
                       from r2 in t2.Result
                       from r3 in t3.Result
                       from r4 in t4.Result
                       select (r1, r2, r3, r4);
            });

        /// <summary>
        /// Run the five effects in the tuple in parallel, wait for them all to finish, then return a tuple of the results
        /// </summary>
        public static Aff<Env, (A, B, C, D, E)> Sequence<Env, A, B, C, D, E>(this (Aff<Env, A>, Aff<Env, B>, Aff<Env, C>, Aff<Env, D>, Aff<Env, E>) ms) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env,(A, B, C, D, E)>(async env =>
            {
                var t1 = ms.Item1.Run(env).AsTask();
                var t2 = ms.Item2.Run(env).AsTask();
                var t3 = ms.Item3.Run(env).AsTask();
                var t4 = ms.Item4.Run(env).AsTask();
                var t5 = ms.Item5.Run(env).AsTask();
                
                var tasks = new Task[] {t1, t2, t3, t4, t5};
                await Task.WhenAll(tasks);
                return from r1 in t1.Result
                       from r2 in t2.Result
                       from r3 in t3.Result
                       from r4 in t4.Result
                       from r5 in t5.Result
                       select (r1, r2, r3, r4, r5);
            });

        //
        // Map
        //

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> Map<Env, A, B>(this Aff<Env, A> ma, Func<A, B> f) where Env : struct, HasCancel<Env> =>
            new Aff<Env, B>(ma.Thunk.Map(f));

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> MapAsync<Env, A, B>(this Aff<Env, A> ma, Func<A, ValueTask<B>> f) where Env : struct, HasCancel<Env> =>
            new Aff<Env, B>(ma.Thunk.MapAsync(f));

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> MapFail<Env, A>(this Aff<Env, A> ma, Func<Error, Error> f) where Env : struct, HasCancel<Env> =>
            ma.BiMap(identity, f);

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> MapFailAsync<Env, A>(this Aff<Env, A> ma, Func<Error, ValueTask<Error>> f) where Env : struct, HasCancel<Env> =>
            ma.BiMapAsync(x => x.AsValueTask(), f);

        //
        // Bi-map
        //

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> BiMap<Env, A, B>(this Aff<Env, A> ma, Func<A, B> Succ, Func<Error, Error> Fail) where Env : struct, HasCancel<Env> =>
            new Aff<Env, B>(ma.Thunk.BiMap(Succ, Fail));

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> BiMapAsync<Env, A, B>(this Aff<Env, A> ma, Func<A, ValueTask<B>> Succ, Func<Error, ValueTask<Error>> Fail) where Env : struct, HasCancel<Env> =>
            new Aff<Env, B>(ma.Thunk.BiMapAsync(Succ, Fail));

        //
        // Match
        //

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> Match<Env, A, B>(this Aff<Env, A> ma, Func<A, B> Succ, Func<Error, B> Fail) where Env : struct, HasCancel<Env> =>
            Aff<Env, B>(async env =>
            {
                var r = await ma.Run(env).ConfigureAwait(false);
                return r.IsSucc
                    ? Succ(r.Value)
                    : Fail(r.Error);
            });

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> MatchAff<Env, A, B>(this Aff<Env, A> ma, Func<A, B> Succ, Aff<Env, B> Fail) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, B>(async env =>
                             {
                                 var r = await ma.Run(env).ConfigureAwait(false);
                                 return r.IsSucc
                                            ? Succ(r.Value)
                                            : await Fail.Run(env).ConfigureAwait(false);
                             });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> MatchAff<Env, A, B>(this Aff<Env, A> ma, Func<A, B> Succ, Func<Error, Aff<Env, B>> Fail) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, B>(async env =>
                             {
                                 var r = await ma.Run(env).ConfigureAwait(false);
                                 return r.IsSucc
                                            ? Succ(r.Value)
                                            : await Fail(r.Error).Run(env).ConfigureAwait(false);
                             });

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> MatchAff<Env, A, B>(this Aff<Env, A> ma, Aff<Env, B> Succ, Func<Error, B> Fail) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, B>(async env =>
            {
                var r = await ma.Run(env).ConfigureAwait(false);
                return r.IsSucc
                    ? await Succ.Run(env).ConfigureAwait(false)
                    : Fail(r.Error);
            });

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> MatchAff<Env, A, B>(this Aff<Env, A> ma, Func<A, Aff<Env, B>> Succ, Func<Error, B> Fail) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, B>(async env =>
                             {
                                 var r = await ma.Run(env).ConfigureAwait(false);
                                 return r.IsSucc
                                            ? await Succ(r.Value).Run(env).ConfigureAwait(false)
                                            : Fail(r.Error);
                             });

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> MatchAff<Env, A, B>(this Aff<Env, A> ma, Aff<Env, B> Succ, Aff<Env, B> Fail) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, B>(async env =>
            {
                var r = await ma.Run(env).ConfigureAwait(false);
                return r.IsSucc
                    ? await Succ.Run(env).ConfigureAwait(false)
                    : await Fail.Run(env).ConfigureAwait(false);
            });

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> MatchAff<Env, A, B>(this Aff<Env, A> ma, Func<A, Aff<Env, B>> Succ, Func<Error, Aff<Env, B>> Fail) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, B>(async env =>
                             {
                                 var r = await ma.Run(env).ConfigureAwait(false);
                                 return r.IsSucc
                                            ? await Succ(r.Value).Run(env).ConfigureAwait(false)
                                            : await Fail(r.Error).Run(env).ConfigureAwait(false);
                             });

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> Match<Env, A, B>(this Aff<Env, A> ma, B Succ, Func<Error, B> Fail) where Env : struct, HasCancel<Env> =>
            Aff<Env, B>(async env =>
            {
                var r = await ma.Run(env).ConfigureAwait(false);
                return r.IsSucc
                    ? Succ
                    : Fail(r.Error);
            });

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> Match<Env, A, B>(this Aff<Env, A> ma, Func<A, B> Succ, B Fail) where Env : struct, HasCancel<Env> =>
            Aff<Env, B>(async env =>
                        {
                            var r = await ma.Run(env).ConfigureAwait(false);
                            return r.IsSucc
                                       ? Succ(r.Value)
                                       : Fail;
                        });

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> Match<Env, A, B>(this Aff<Env, A> ma, B Succ, B Fail) where Env : struct, HasCancel<Env> =>
            Aff<Env, B>(async env =>
                        {
                            var r = await ma.Run(env).ConfigureAwait(false);
                            return r.IsSucc
                                       ? Succ
                                       : Fail;
                        });
        
        //
        // IfNone
        //
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> IfFail<Env, A>(this Aff<Env, A> ma, Func<Error, A> f) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, A>(
                async env =>
                {
                    var res = await ma.Run(env).ConfigureAwait(false);
                    return res.IsSucc
                               ? res
                               : Fin<A>.Succ(f(res.Error));
                });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> IfFail<Env, A>(this Aff<Env, A> ma, A alternative) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, A>(
                async env =>
                {
                    var res = await ma.Run(env).ConfigureAwait(false);
                    return res.IsSucc
                               ? res
                               : Fin<A>.Succ(alternative);
                });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> IfFailAff<Env, A>(this Aff<Env, A> ma, Aff<Env, A> alternative) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, A>(
                async env =>
                {
                    var res = await ma.Run(env).ConfigureAwait(false);
                    return res.IsSucc
                               ? res
                               : await alternative.Run(env).ConfigureAwait(false);
                });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> IfFailAff<Env, A>(this Aff<Env, A> ma, Func<Error, Aff<Env, A>> alternative) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, A>(
                async env =>
                {
                    var res = await ma.Run(env).ConfigureAwait(false);
                    return res.IsSucc
                               ? res
                               : await alternative(res.Error).Run(env).ConfigureAwait(false);
                });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> IfFailAff<Env, A>(this Aff<Env, A> ma, Aff<A> alternative) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, A>(
                async env =>
                {
                    var res = await ma.Run(env).ConfigureAwait(false);
                    return res.IsSucc
                               ? res
                               : await alternative.Run().ConfigureAwait(false);
                });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> IfFailAff<Env, A>(this Aff<Env, A> ma, Func<Error, Aff<A>> alternative) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, A>(
                async env =>
                {
                    var res = await ma.Run(env).ConfigureAwait(false);
                    return res.IsSucc
                               ? res
                               : await alternative(res.Error).Run().ConfigureAwait(false);
                });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> IfFailAff<Env, A>(this Aff<Env, A> ma, Eff<Env, A> alternative) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, A>(
                async env =>
                {
                    var res = await ma.Run(env).ConfigureAwait(false);
                    return res.IsSucc
                               ? res
                               : alternative.Run(env);
                });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> IfFailAff<Env, A>(this Aff<Env, A> ma, Func<Error, Eff<Env, A>> alternative) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, A>(
                async env =>
                {
                    var res = await ma.Run(env).ConfigureAwait(false);
                    return res.IsSucc
                               ? res
                               : alternative(res.Error).Run(env);
                });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> IfFailAff<Env, A>(this Aff<Env, A> ma, Eff<A> alternative) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, A>(
                async env =>
                {
                    var res = await ma.Run(env).ConfigureAwait(false);
                    return res.IsSucc
                               ? res
                               : alternative.Run();
                });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> IfFailAff<Env, A>(this Aff<Env, A> ma, Func<Error, Eff<A>> alternative) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, A>(
                async env =>
                {
                    var res = await ma.Run(env).ConfigureAwait(false);
                    return res.IsSucc
                               ? res
                               : alternative(res.Error).Run();
                });
        
        //
        // Iter / Do
        //
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, Unit> Iter<Env, A>(this Aff<Env, A> ma, Func<A, Unit> f) where Env : struct, HasCancel<Env> =>
            Aff<Env, Unit>(
                async env =>
                {
                    var res = await ma.Run(env).ConfigureAwait(false);
                    if (res.IsSucc)
                    {
                        f(res.Value);
                    }
                    return unit;
                });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, Unit> Iter<Env, A>(this Aff<Env, A> ma, Func<A, Aff<Env, Unit>> f) where Env : struct, HasCancel<Env> =>
            Aff<Env, Unit>(
                async env =>
                {
                    var res = await ma.Run(env).ConfigureAwait(false);
                    if (res.IsSucc)
                    {
                        ignore(await f(res.Value).Run(env).ConfigureAwait(false));
                    }
                    return unit;
                });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, Unit> Iter<Env, A>(this Aff<Env, A> ma, Func<A, Aff<Unit>> f) where Env : struct, HasCancel<Env> =>
            Aff<Env, Unit>(
                async env =>
                {
                    var res = await ma.Run(env).ConfigureAwait(false);
                    if (res.IsSucc)
                    {
                        ignore(await f(res.Value).Run().ConfigureAwait(false));
                    }
                    return unit;
                });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, Unit> Iter<Env, A>(this Aff<Env, A> ma, Func<A, Eff<Env, Unit>> f) where Env : struct, HasCancel<Env> =>
            Aff<Env, Unit>(
                async env =>
                {
                    var res = await ma.Run(env).ConfigureAwait(false);
                    if (res.IsSucc)
                    {
                        ignore(f(res.Value).Run(env));
                    }
                    return unit;
                });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, Unit> Iter<Env, A>(this Aff<Env, A> ma, Func<A, Eff<Unit>> f) where Env : struct, HasCancel<Env> =>
            Aff<Env, Unit>(
                async env =>
                {
                    var res = await ma.Run(env).ConfigureAwait(false);
                    if (res.IsSucc)
                    {
                        ignore(f(res.Value).Run());
                    }
                    return unit;
                });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> Do<Env, A>(this Aff<Env, A> ma, Func<A, Unit> f) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, A>(
                async env =>
                {
                    var res = await ma.Run(env).ConfigureAwait(false);
                    if (res.IsSucc)
                    {
                        f(res.Value);
                    }
                    return res;
                });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> Do<Env, A>(this Aff<Env, A> ma, Func<A, Aff<Env, Unit>> f) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, A>(
                async env =>
                {
                    var res = await ma.Run(env).ConfigureAwait(false);
                    if (res.IsSucc)
                    {
                        var ures = await f(res.Value).Run(env).ConfigureAwait(false);
                        return ures.IsFail
                                   ? Fin<A>.Fail(ures.Error)
                                   : res;
                    }
                    return res;
                });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> Do<Env, A>(this Aff<Env, A> ma, Func<A, Eff<Env, Unit>> f) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, A>(
                async env =>
                {
                    var res = await ma.Run(env).ConfigureAwait(false);
                    if (res.IsSucc)
                    {
                        var ures = f(res.Value).Run(env);
                        return ures.IsFail
                                   ? Fin<A>.Fail(ures.Error)
                                   : res;
                    }
                    return res;
                });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> Do<Env, A>(this Aff<Env, A> ma, Func<A, Aff<Unit>> f) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, A>(
                async env =>
                {
                    var res = await ma.Run(env).ConfigureAwait(false);
                    if (res.IsSucc)
                    {
                        var ures = await f(res.Value).Run().ConfigureAwait(false);
                        return ures.IsFail
                                   ? Fin<A>.Fail(ures.Error)
                                   : res;
                    }
                    return res;
                });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> Do<Env, A>(this Aff<Env, A> ma, Func<A, Eff<Unit>> f) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, A>(
                async env =>
                {
                    var res = await ma.Run(env).ConfigureAwait(false);
                    if (res.IsSucc)
                    {
                        var ures = f(res.Value).Run();
                        return ures.IsFail
                                   ? Fin<A>.Fail(ures.Error)
                                   : res;
                    }
                    return res;
                });
        
        //
        // Filter / Where
        //
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> Filter<Env, A>(this Aff<Env, A> ma, Func<A, bool> f) where Env : struct, HasCancel<Env> =>
            ma.Bind(x => f(x) ? SuccessEff<A>(x) : FailEff<A>(Errors.Cancelled));


        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> Where<Env, A>(this Aff<Env, A> ma, Func<A, bool> f) where Env : struct, HasCancel<Env> =>
            Filter(ma, f);

        //
        // Bind
        //

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> Bind<Env, A, B>(this Aff<Env, A> ma, Func<A, Aff<Env, B>> f) where Env : struct, HasCancel<Env> =>
            new Aff<Env, B>(ma.Thunk.Map(x => ThunkFromIO(f(x))).Flatten());

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> Bind<Env, A, B>(this Aff<Env, A> ma, Func<A, Aff<B>> f) where Env : struct, HasCancel<Env> =>
            new Aff<Env, B>(ma.Thunk.Map(x => ThunkFromIO(f(x))).Flatten());

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> Bind<Env, A, B>(this Aff<Env, A> ma, Func<A, Eff<Env, B>> f) where Env : struct, HasCancel<Env> =>
            new Aff<Env, B>(ma.Thunk.Map(x => ThunkFromIO(f(x))).Flatten());

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> Bind<Env, A, B>(this Aff<Env, A> ma, Func<A, LanguageExt.Eff<B>> f) where Env : struct, HasCancel<Env> =>
            new Aff<Env, B>(ma.Thunk.Map(x => ThunkFromIO(f(x))).Flatten());

        //
        // Bi-Bind
        //

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> BiBind<Env, A, B>(this Aff<Env, A> ma, Func<A, Aff<Env, B>> Succ, Func<Error, Aff<Env, B>> Fail) where Env : struct, HasCancel<Env> =>
            ma.Match(Succ, Fail)
                .Flatten();

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> BiBind<Env, A, B>(this Aff<Env, A> ma, Func<A, Aff<B>> Succ, Func<Error, Aff<B>> Fail) where Env : struct, HasCancel<Env> =>
            ma.Match(Succ, Fail)
                .Flatten();

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> BiBind<Env, A, B>(this Aff<Env, A> ma, Func<A, Eff<Env, B>> Succ, Func<Error, Eff<Env, B>> Fail) where Env : struct, HasCancel<Env> =>
            ma.Match(Succ, Fail)
                .Flatten();

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> BiBind<Env, A, B>(this Aff<Env, A> ma, Func<A, LanguageExt.Eff<B>> Succ, Func<Error, LanguageExt.Eff<B>> Fail) where Env : struct, HasCancel<Env> =>
            ma.Match(Succ, Fail)
                .Flatten();

        //
        // Flatten (monadic join)
        //

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> Flatten<Env, A>(this Aff<Env, Aff<Env, A>> ma) where Env : struct, HasCancel<Env> =>
            new Aff<Env, A>(ma.Thunk.Map(ThunkFromIO).Flatten());

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> Flatten<Env, A>(this Aff<Env, Aff<A>> ma) where Env : struct, HasCancel<Env> =>
            new Aff<Env, A>(ma.Thunk.Map(ThunkFromIO).Flatten());

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> Flatten<Env, A>(this Aff<Env, Eff<Env, A>> ma) where Env : struct, HasCancel<Env> =>
            new Aff<Env, A>(ma.Thunk.Map(ThunkFromIO).Flatten());

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> Flatten<Env, A>(this Aff<Env, LanguageExt.Eff<A>> ma) where Env : struct, HasCancel<Env> =>
            new Aff<Env, A>(ma.Thunk.Map(ThunkFromIO).Flatten());

        //
        // Select
        //

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> Select<Env, A, B>(this Aff<Env, A> ma, Func<A, B> f) where Env : struct, HasCancel<Env> =>
            Map(ma, f);

        //
        // SelectMany
        //

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> SelectMany<Env, A, B>(this Aff<Env, A> ma, Func<A, Aff<Env, B>> f) where Env : struct, HasCancel<Env> =>
            Bind(ma, f);

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> SelectMany<Env, A, B>(this Aff<Env, A> ma, Func<A, Aff<B>> f) where Env : struct, HasCancel<Env> =>
            Bind(ma, f);

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> SelectMany<Env, A, B>(this Aff<Env, A> ma, Func<A, Eff<Env, B>> f) where Env : struct, HasCancel<Env> =>
            Bind(ma, f);

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> SelectMany<Env, A, B>(this Aff<Env, A> ma, Func<A, LanguageExt.Eff<B>> f) where Env : struct, HasCancel<Env> =>
            Bind(ma, f);

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, C> SelectMany<Env, A, B, C>(this Aff<Env, A> ma, Func<A, Aff<Env, B>> bind, Func<A, B, C> project) where Env : struct, HasCancel<Env> =>
            Bind(ma, x => Map(bind(x), y => project(x, y)));

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, C> SelectMany<Env, A, B, C>(this Aff<Env, A> ma, Func<A, Aff<B>> bind, Func<A, B, C> project) where Env : struct, HasCancel<Env> =>
            Bind(ma, x => Map(bind(x), y => project(x, y)));

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, C> SelectMany<Env, A, B, C>(this Aff<Env, A> ma, Func<A, Eff<Env, B>> bind, Func<A, B, C> project) where Env : struct, HasCancel<Env> =>
            Bind(ma, x => Map(bind(x), y => project(x, y)));

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, C> SelectMany<Env, A, B, C>(this Aff<Env, A> ma, Func<A, LanguageExt.Eff<B>> bind, Func<A, B, C> project) where Env : struct, HasCancel<Env> =>
            Bind(ma, x => Map(bind(x), y => project(x, y)));

        //
        // Zip
        //

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, (A, B)> Zip<Env, A, B>(this Aff<Env, A> ma, Aff<Env, B> mb) where Env : struct, HasCancel<Env> =>
            new Aff<Env, (A, B)>(ThunkAsync<Env, (A, B)>.Lazy(async e =>
            {
                var ta = ma.Run(e).AsTask();
                var tb = mb.Run(e).AsTask();
                await System.Threading.Tasks.Task.WhenAll(ta, tb).ConfigureAwait(false);
                if (ta.CompletedSuccessfully() && tb.CompletedSuccessfully())
                {
                    return ta.Result.IsSucc && tb.Result.IsSucc
                        ? Fin<(A, B)>.Succ((ta.Result.Value, tb.Result.Value))
                        : ta.Result.IsFail
                            ? Fin<(A, B)>.Fail(ta.Result.Error)
                            : Fin<(A, B)>.Fail(tb.Result.Error);
                }
                else
                {
                    return (ta.IsCanceled || tb.IsCanceled)
                        ? Fin<(A, B)>.Fail(Errors.Cancelled)
                        : ta.IsFaulted
                            ? Fin<(A, B)>.Fail(Error.New(ta.Exception))
                            : Fin<(A, B)>.Fail(Error.New(tb.Exception));
                }
            }));

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, (A, B)> Zip<Env, A, B>(this Aff<Env, A> ma, Aff<B> mb) where Env : struct, HasCancel<Env> =>
            new Aff<Env, (A, B)>(ThunkAsync<Env, (A, B)>.Lazy(async e =>
            {
                var ta = ma.Run(e).AsTask();
                var tb = mb.Run().AsTask();
                await System.Threading.Tasks.Task.WhenAll(ta, tb).ConfigureAwait(false);
                if (ta.CompletedSuccessfully() && tb.CompletedSuccessfully())
                {
                    return ta.Result.IsSucc && tb.Result.IsSucc
                        ? Fin<(A, B)>.Succ((ta.Result.Value, tb.Result.Value))
                        : ta.Result.IsFail
                            ? Fin<(A, B)>.Fail(ta.Result.Error)
                            : Fin<(A, B)>.Fail(tb.Result.Error);
                }
                else
                {
                    return (ta.IsCanceled || tb.IsCanceled)
                        ? Fin<(A, B)>.Fail(Errors.Cancelled)
                        : ta.IsFaulted
                            ? Fin<(A, B)>.Fail(Error.New(ta.Exception))
                            : Fin<(A, B)>.Fail(Error.New(tb.Exception));
                }
            }));


        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, (A, B)> Zip<Env, A, B>(this Aff<A> ma, Aff<Env, B> mb) where Env : struct, HasCancel<Env> =>
            new Aff<Env, (A, B)>(ThunkAsync<Env, (A, B)>.Lazy(async e =>
            {
                var ta = ma.Run().AsTask();
                var tb = mb.Run(e).AsTask();
                await System.Threading.Tasks.Task.WhenAll(ta, tb).ConfigureAwait(false);
                if (ta.CompletedSuccessfully() && tb.CompletedSuccessfully())
                {
                    return ta.Result.IsSucc && tb.Result.IsSucc
                        ? Fin<(A, B)>.Succ((ta.Result.Value, tb.Result.Value))
                        : ta.Result.IsFail
                            ? Fin<(A, B)>.Fail(ta.Result.Error)
                            : Fin<(A, B)>.Fail(tb.Result.Error);
                }
                else
                {
                    return (ta.IsCanceled || tb.IsCanceled)
                        ? Fin<(A, B)>.Fail(Errors.Cancelled)
                        : ta.IsFaulted
                            ? Fin<(A, B)>.Fail(Error.New(ta.Exception))
                            : Fin<(A, B)>.Fail(Error.New(tb.Exception));
                }
            }));

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, (A, B)> Zip<Env, A, B>(this Aff<Env, A> ma, LanguageExt.Eff<B> mb) where Env : struct, HasCancel<Env> =>
            new Aff<Env, (A, B)>(ThunkAsync<Env, (A, B)>.Lazy(async e =>
            {
                var ta = ma.Run(e).AsTask();
                var ra = await ta.ConfigureAwait(false);
                if (!ta.CompletedSuccessfully())
                {
                    return Fin<(A, B)>.Fail(ra.Error);
                }

                var rb = mb.Run();
                if (rb.IsFail) return Fin<(A, B)>.Fail(rb.Error);

                return Fin<(A, B)>.Succ((ra.Value, rb.Value));
            }));

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, (A, B)> Zip<Env, A, B>(this LanguageExt.Eff<A> ma, Aff<Env, B> mb) where Env : struct, HasCancel<Env> =>
            new Aff<Env, (A, B)>(ThunkAsync<Env, (A, B)>.Lazy(async e =>
            {
                var ra = ma.Run();
                if (ra.IsFail) return Fin<(A, B)>.Fail(ra.Error);

                var tb = mb.Run(e).AsTask();
                var rb = await tb.ConfigureAwait(false);
                if (!tb.CompletedSuccessfully())
                {
                    return Fin<(A, B)>.Fail(rb.Error);
                }

                return Fin<(A, B)>.Succ((ra.Value, rb.Value));
            }));


        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, (A, B)> Zip<Env, A, B>(this Aff<Env, A> ma, Eff<Env, B> mb) where Env : struct, HasCancel<Env> =>
            new Aff<Env, (A, B)>(ThunkAsync<Env, (A, B)>.Lazy(async e =>
            {
                var ta = ma.Run(e).AsTask();
                var ra = await ta.ConfigureAwait(false);
                if (!ta.CompletedSuccessfully())
                {
                    return Fin<(A, B)>.Fail(ra.Error);
                }

                var rb = mb.Run(e);
                if (rb.IsFail) return Fin<(A, B)>.Fail(rb.Error);

                return Fin<(A, B)>.Succ((ra.Value, rb.Value));
            }));

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, (A, B)> Zip<Env, A, B>(this Eff<Env, A> ma, Aff<Env, B> mb) where Env : struct, HasCancel<Env> =>
            new Aff<Env, (A, B)>(ThunkAsync<Env, (A, B)>.Lazy(async e =>
            {
                var ra = ma.Run(e);
                if (ra.IsFail) return Fin<(A, B)>.Fail(ra.Error);

                var tb = mb.Run(e).AsTask();
                var rb = await tb.ConfigureAwait(false);
                if (!tb.CompletedSuccessfully())
                {
                    return Fin<(A, B)>.Fail(rb.Error);
                }

                return Fin<(A, B)>.Succ((ra.Value, rb.Value));
            }));

        [Pure, MethodImpl(AffOpt.mops)]
        static ThunkAsync<Env, A> ThunkFromIO<Env, A>(Aff<Env, A> ma) where Env : struct, HasCancel<Env> =>
            ma.Thunk;
    }
}

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
using LanguageExt.Pipes;
using LanguageExt.Thunks;

namespace LanguageExt
{
    /// <summary>
    /// Asynchronous effect monad
    /// </summary>
    public readonly struct Aff<RT, A> 
        where RT : struct, HasCancel<RT>
    {
        internal ThunkAsync<RT, A> Thunk => thunk ?? ThunkAsync<RT, A>.Fail(Errors.Bottom);
        readonly ThunkAsync<RT, A> thunk;

        /// <summary>
        /// Constructor
        /// </summary>
        [MethodImpl(Opt.Default)]
        internal Aff(ThunkAsync<RT, A> thunk) =>
            this.thunk = thunk ?? throw new ArgumentNullException(nameof(thunk));

        /// <summary>
        /// Invoke the effect
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public ValueTask<Fin<A>> Run(in RT env) =>
            Thunk.Value(env);

        /// <summary>
        /// Invoke the effect
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public ValueTask<Fin<A>> ReRun(RT env) =>
            Thunk.ReValue(env);

        /// <summary>
        /// Clone the effect
        /// </summary>
        /// <remarks>
        /// If the effect had already run, then this state will be wiped in the clone, meaning it can be re-run
        /// </remarks>
        [Pure, MethodImpl(Opt.Default)]
        public Aff<RT, A> Clone() =>
            new Aff<RT, A>(Thunk.Clone());        

        /// <summary>
        /// Invoke the effect
        /// </summary>
        /// <remarks>
        /// Throws on error
        /// </remarks>
        [MethodImpl(Opt.Default)]
        public async ValueTask<Unit> RunUnit(RT env) =>
            (await Thunk.Value(env).ConfigureAwait(false)).Case switch
            {
                A _     => unit,
                Error e => e.Throw(),
                _       => throw new NotSupportedException()
            };

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
        [MethodImpl(Opt.Default)]
        public Eff<RT, Eff<Unit>> Fork()
        {
            var t = Thunk;
            return Eff<RT, Eff<Unit>>(
                env =>
                {
                    // Create a new local runtime with its own cancellation token
                    var lenv = env.LocalCancel;
                    
                    // If the parent cancels, we should too
                    var reg = env.CancellationToken.Register(() => lenv.CancellationTokenSource.Cancel());
                    
                    // Run
                    ignore(t.Value(lenv).Iter(_ => Dispose()));
                    
                    // Return an effect that cancels the fire-and-forget expression
                    return Eff<Unit>(() =>
                                     {
                                         lenv.CancellationTokenSource.Cancel();
                                         Dispose();
                                         return unit;
                                     });

                    void Dispose()
                    {
                        try
                        {
                            reg.Dispose();
                        }
                        catch
                        {
                        }
                    }
                });
        }

        /// <summary>
        /// Lift an asynchronous effect into the Aff monad
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> Effect(Func<RT, ValueTask<A>> f) =>
            new Aff<RT, A>(ThunkAsync<RT, A>.Lazy(f));

        /// <summary>
        /// Lift an asynchronous effect into the Aff monad
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> EffectMaybe(Func<RT, ValueTask<Fin<A>>> f) =>
            new Aff<RT, A>(ThunkAsync<RT, A>.Lazy(f));

        /// <summary>
        /// Lift an asynchronous effect into the Aff monad
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, Unit> Effect(Func<RT, ValueTask> f) =>
            new Aff<RT, Unit>(ThunkAsync<RT, Unit>.Lazy(async e =>
            {
                await f(e).ConfigureAwait(false);
                return Fin<Unit>.Succ(default);
            }));

        /// <summary>
        /// Lift a value into the Aff monad 
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> Success(A value) =>
            new Aff<RT, A>(ThunkAsync<RT, A>.Success(value));

        /// <summary>
        /// Lift a failure into the Aff monad 
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> Fail(Error error) =>
            new Aff<RT, A>(ThunkAsync<RT, A>.Fail(error));

        /// <summary>
        /// Force the operation to end after a time out delay
        /// </summary>
        /// <param name="timeoutDelay">Delay for the time out</param>
        /// <returns>Either success if the operation completed before the timeout, or Errors.TimedOut</returns>
        [Pure, MethodImpl(Opt.Default)]
        public Aff<RT, A> Timeout(TimeSpan timeoutDelay)
        {
            var t = Thunk;
            return AffMaybe<RT, A>(
                async env =>
                {
                    using var delayTokSrc = new CancellationTokenSource();
                    var lenv       = env.LocalCancel;
                    var delay      = Task.Delay(timeoutDelay, delayTokSrc.Token);
                    var task       = t.Value(lenv).AsTask();
                    var completed  = await Task.WhenAny(new Task[] {delay, task}).ConfigureAwait(false);

                    if (completed == delay)
                    {
                        lenv.CancellationTokenSource.Cancel();
                        return FinFail<A>(Errors.TimedOut);
                    }
                    else
                    {
                        delayTokSrc.Cancel();
                        return await task;
                    }
                });
        }

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> operator |(Aff<RT, A> ma, Aff<RT, A> mb) =>
            new Aff<RT, A>(ThunkAsync<RT, A>.Lazy(
                                async env =>
                                {
                                    var ra = await ma.ReRun(env).ConfigureAwait(false);
                                    return ra.IsSucc
                                               ? ra
                                               : await mb.ReRun(env).ConfigureAwait(false);
                                }));

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> operator |(Aff<RT, A> ma, Aff<A> mb) =>
            new Aff<RT, A>(ThunkAsync<RT, A>.Lazy(
                                async env =>
                                {
                                    var ra = await ma.ReRun(env).ConfigureAwait(false);
                                    return ra.IsSucc
                                               ? ra
                                               : await mb.ReRun().ConfigureAwait(false);
                                }));

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> operator |(Aff<A> ma, Aff<RT, A> mb) =>
            new Aff<RT, A>(ThunkAsync<RT, A>.Lazy(
                                async env =>
                                {
                                    var ra = await ma.ReRun().ConfigureAwait(false);
                                    return ra.IsSucc
                                               ? ra
                                               : await mb.ReRun(env).ConfigureAwait(false);
                                }));

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> operator |(Aff<RT, A> ma, Eff<RT, A> mb) =>
            new Aff<RT, A>(ThunkAsync<RT, A>.Lazy(
                                async env =>
                                {
                                    var ra = await ma.ReRun(env).ConfigureAwait(false);
                                    return ra.IsSucc
                                               ? ra
                                               : mb.ReRun(env);
                                }));

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> operator |(Eff<RT, A> ma, Aff<RT, A> mb) =>
            new Aff<RT, A>(ThunkAsync<RT, A>.Lazy(
                                async env =>
                                {
                                    var ra = ma.ReRun(env);
                                    return ra.IsSucc
                                               ? ra
                                               : await mb.ReRun(env).ConfigureAwait(false);
                                }));

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> operator |(Eff<A> ma, Aff<RT, A> mb) =>
            new Aff<RT, A>(ThunkAsync<RT, A>.Lazy(
                                async env =>
                                {
                                    var ra = ma.ReRun();
                                    return ra.IsSucc
                                               ? ra
                                               : await mb.ReRun(env).ConfigureAwait(false);
                                }));

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> operator |(Aff<RT, A> ma, Eff<A> mb) =>
            new Aff<RT, A>(ThunkAsync<RT, A>.Lazy(
                                async env =>
                                {
                                    var ra = await ma.ReRun(env).ConfigureAwait(false);
                                    return ra.IsSucc
                                               ? ra
                                               : mb.ReRun();
                                }));

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> operator |(Aff<RT, A> ma, EffCatch<A> mb) =>
            new Aff<RT, A>(ThunkAsync<RT, A>.Lazy(
                                async env =>
                                {
                                    var ra = await ma.ReRun(env).ConfigureAwait(false);
                                    return ra.IsSucc
                                               ? ra
                                               : mb.Run(ra.Error);
                                }));

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> operator |(Aff<RT, A> ma, AffCatch<A> mb) =>
            new Aff<RT, A>(ThunkAsync<RT, A>.Lazy(
                                async env =>
                                {
                                    var ra = await ma.ReRun(env).ConfigureAwait(false);
                                    return ra.IsSucc
                                               ? ra
                                               : await mb.Run(ra.Error).ConfigureAwait(false);
                                }));

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> operator |(Aff<RT, A> ma, EffCatch<RT, A> mb) =>
            new Aff<RT, A>(ThunkAsync<RT, A>.Lazy(
                                async env =>
                                {
                                    var ra = await ma.ReRun(env).ConfigureAwait(false);
                                    return ra.IsSucc
                                               ? ra
                                               : mb.Run(env, ra.Error);
                                }));

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> operator |(Aff<RT, A> ma, AffCatch<RT, A> mb) =>
            new Aff<RT, A>(ThunkAsync<RT, A>.Lazy(
                                async env =>
                                {
                                    var ra = await ma.ReRun(env).ConfigureAwait(false);
                                    return ra.IsSucc
                                               ? ra
                                               : await mb.Run(env, ra.Error).ConfigureAwait(false);
                                }));

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> operator |(Aff<RT, A> ma, CatchValue<A> value) =>
            new Aff<RT, A>(ThunkAsync<RT, A>.Lazy(
                                async env =>
                                {
                                    var ra = await ma.ReRun(env).ConfigureAwait(false);
                                    return ra.IsSucc
                                               ? ra
                                               : value.Match(ra.Error)
                                                   ? FinSucc(value.Value(ra.Error))
                                                   : ra;
                                }));

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> operator |(Aff<RT, A> ma, CatchError value) =>
            new Aff<RT, A>(ThunkAsync<RT, A>.Lazy(
                                async env =>
                                {
                                    var ra = await ma.ReRun(env).ConfigureAwait(false);
                                    return ra.IsSucc
                                               ? ra
                                               : value.Match(ra.Error)
                                                   ? FinFail<A>(value.Value(ra.Error))
                                                   : ra;
                                }));

        /// <summary>
        /// Implicit conversion from pure Aff
        /// </summary>
        public static implicit operator Aff<RT, A>(Aff<A> ma) =>
            EffectMaybe(env => ma.ReRun());

        /// <summary>
        /// Implicit conversion from pure Eff
        /// </summary>
        public static implicit operator Aff<RT, A>(Eff<A> ma) =>
            EffectMaybe(env => ma.ReRun().AsValueTask());

        /// <summary>
        /// Implicit conversion from Eff
        /// </summary>
        public static implicit operator Aff<RT, A>(Eff<RT, A> ma) =>
            EffectMaybe(env => ma.ReRun(env).AsValueTask());
    }

    public static partial class AffExtensions
    {
        //
        // Sequence (with tuples)
        //
        
        /// <summary>
        /// Run the two effects in the tuple in parallel, wait for them all to finish, then return a tuple of the results
        /// </summary>
        public static Aff<RT, (A, B)> Sequence<RT, A, B>(this (Aff<RT, A>, Aff<RT, B>) ms) where RT : struct, HasCancel<RT> => 
            AffMaybe<RT,(A, B)>(async env =>
            {
                var t1 = ms.Item1.ReRun(env).AsTask();
                var t2 = ms.Item2.ReRun(env).AsTask();
                
                var tasks = new Task[] {t1, t2};
                await Task.WhenAll(tasks);
                return from r1 in t1.Result
                       from r2 in t2.Result
                       select (r1, r2);
            });

        /// <summary>
        /// Run the three effects in the tuple in parallel, wait for them all to finish, then return a tuple of the results
        /// </summary>
        public static Aff<RT, (A, B, C)> Sequence<RT, A, B, C>(this (Aff<RT, A>, Aff<RT, B>, Aff<RT, C>) ms) where RT : struct, HasCancel<RT> => 
            AffMaybe<RT,(A, B, C)>(async env =>
            {
                var t1 = ms.Item1.ReRun(env).AsTask();
                var t2 = ms.Item2.ReRun(env).AsTask();
                var t3 = ms.Item3.ReRun(env).AsTask();
                
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
        public static Aff<RT, (A, B, C, D)> Sequence<RT, A, B, C, D>(this (Aff<RT, A>, Aff<RT, B>, Aff<RT, C>, Aff<RT, D>) ms) where RT : struct, HasCancel<RT> => 
            AffMaybe<RT,(A, B, C, D)>(async env =>
            {
                var t1 = ms.Item1.ReRun(env).AsTask();
                var t2 = ms.Item2.ReRun(env).AsTask();
                var t3 = ms.Item3.ReRun(env).AsTask();
                var t4 = ms.Item4.ReRun(env).AsTask();
                
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
        public static Aff<RT, (A, B, C, D, E)> Sequence<RT, A, B, C, D, E>(this (Aff<RT, A>, Aff<RT, B>, Aff<RT, C>, Aff<RT, D>, Aff<RT, E>) ms) where RT : struct, HasCancel<RT> => 
            AffMaybe<RT,(A, B, C, D, E)>(async env =>
            {
                var t1 = ms.Item1.ReRun(env).AsTask();
                var t2 = ms.Item2.ReRun(env).AsTask();
                var t3 = ms.Item3.ReRun(env).AsTask();
                var t4 = ms.Item4.ReRun(env).AsTask();
                var t5 = ms.Item5.ReRun(env).AsTask();
                
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

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> Map<RT, A, B>(this Aff<RT, A> ma, Func<A, B> f) where RT : struct, HasCancel<RT> =>
            new Aff<RT, B>(ma.Thunk.Map(f));

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> MapAsync<RT, A, B>(this Aff<RT, A> ma, Func<A, ValueTask<B>> f) where RT : struct, HasCancel<RT> =>
            new Aff<RT, B>(ma.Thunk.MapAsync(f));

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> MapFail<RT, A>(this Aff<RT, A> ma, Func<Error, Error> f) where RT : struct, HasCancel<RT> =>
            ma.BiMap(identity, f);

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> MapFailAsync<RT, A>(this Aff<RT, A> ma, Func<Error, ValueTask<Error>> f) where RT : struct, HasCancel<RT> =>
            ma.BiMapAsync(x => x.AsValueTask(), f);

        //
        // Bi-map
        //

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> BiMap<RT, A, B>(this Aff<RT, A> ma, Func<A, B> Succ, Func<Error, Error> Fail) where RT : struct, HasCancel<RT> =>
            new Aff<RT, B>(ma.Thunk.BiMap(Succ, Fail));

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> BiMapAsync<RT, A, B>(this Aff<RT, A> ma, Func<A, ValueTask<B>> Succ, Func<Error, ValueTask<Error>> Fail) where RT : struct, HasCancel<RT> =>
            new Aff<RT, B>(ma.Thunk.BiMapAsync(Succ, Fail));

        //
        // Match
        //

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> Match<RT, A, B>(this Aff<RT, A> ma, Func<A, B> Succ, Func<Error, B> Fail) where RT : struct, HasCancel<RT> =>
            Aff<RT, B>(async env =>
            {
                var r = await ma.ReRun(env).ConfigureAwait(false);
                return r.IsSucc
                    ? Succ(r.Value)
                    : Fail(r.Error);
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> MatchAff<RT, A, B>(this Aff<RT, A> ma, Func<A, B> Succ, Aff<RT, B> Fail) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, B>(async env =>
                            {
                                var r = await ma.ReRun(env).ConfigureAwait(false);
                                return r.IsSucc
                                           ? Succ(r.Value)
                                           : await Fail.ReRun(env).ConfigureAwait(false);
                            });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> MatchAff<RT, A, B>(this Aff<RT, A> ma, Func<A, B> Succ, Func<Error, Aff<RT, B>> Fail) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, B>(async env =>
                             {
                                 var r = await ma.ReRun(env).ConfigureAwait(false);
                                 return r.IsSucc
                                            ? Succ(r.Value)
                                            : await Fail(r.Error).Run(env).ConfigureAwait(false);
                             });

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> MatchAff<RT, A, B>(this Aff<RT, A> ma, Aff<RT, B> Succ, Func<Error, B> Fail) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, B>(async env =>
            {
                var r = await ma.ReRun(env).ConfigureAwait(false);
                return r.IsSucc
                    ? await Succ.ReRun(env).ConfigureAwait(false)
                    : Fail(r.Error);
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> MatchAff<RT, A, B>(this Aff<RT, A> ma, Func<A, Aff<RT, B>> Succ, Func<Error, B> Fail) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, B>(async env =>
                             {
                                 var r = await ma.ReRun(env).ConfigureAwait(false);
                                 return r.IsSucc
                                            ? await Succ(r.Value).Run(env).ConfigureAwait(false)
                                            : Fail(r.Error);
                             });

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> MatchAff<RT, A, B>(this Aff<RT, A> ma, Aff<RT, B> Succ, Aff<RT, B> Fail) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, B>(async env =>
            {
                var r = await ma.ReRun(env).ConfigureAwait(false);
                return r.IsSucc
                    ? await Succ.ReRun(env).ConfigureAwait(false)
                    : await Fail.ReRun(env).ConfigureAwait(false);
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> MatchAff<RT, A, B>(this Aff<RT, A> ma, Func<A, Aff<RT, B>> Succ, Func<Error, Aff<RT, B>> Fail) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, B>(async env =>
                             {
                                 var r = await ma.ReRun(env).ConfigureAwait(false);
                                 return r.IsSucc
                                            ? await Succ(r.Value).Run(env).ConfigureAwait(false)
                                            : await Fail(r.Error).Run(env).ConfigureAwait(false);
                             });

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> Match<RT, A, B>(this Aff<RT, A> ma, B Succ, Func<Error, B> Fail) where RT : struct, HasCancel<RT> =>
            Aff<RT, B>(async env =>
            {
                var r = await ma.ReRun(env).ConfigureAwait(false);
                return r.IsSucc
                    ? Succ
                    : Fail(r.Error);
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> Match<RT, A, B>(this Aff<RT, A> ma, Func<A, B> Succ, B Fail) where RT : struct, HasCancel<RT> =>
            Aff<RT, B>(async env =>
                        {
                            var r = await ma.ReRun(env).ConfigureAwait(false);
                            return r.IsSucc
                                       ? Succ(r.Value)
                                       : Fail;
                        });

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> Match<RT, A, B>(this Aff<RT, A> ma, B Succ, B Fail) where RT : struct, HasCancel<RT> =>
            Aff<RT, B>(async env =>
                        {
                            var r = await ma.ReRun(env).ConfigureAwait(false);
                            return r.IsSucc
                                       ? Succ
                                       : Fail;
                        });
        
        //
        // IfNone
        //
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> IfFail<RT, A>(this Aff<RT, A> ma, Func<Error, A> f) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, A>(
                async env =>
                {
                    var res = await ma.ReRun(env).ConfigureAwait(false);
                    return res.IsSucc
                               ? res
                               : Fin<A>.Succ(f(res.Error));
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> IfFail<RT, A>(this Aff<RT, A> ma, A alternative) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, A>(
                async env =>
                {
                    var res = await ma.ReRun(env).ConfigureAwait(false);
                    return res.IsSucc
                               ? res
                               : Fin<A>.Succ(alternative);
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> IfFailAff<RT, A>(this Aff<RT, A> ma, Aff<RT, A> alternative) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, A>(
                async env =>
                {
                    var res = await ma.ReRun(env).ConfigureAwait(false);
                    return res.IsSucc
                               ? res
                               : await alternative.ReRun(env).ConfigureAwait(false);
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> IfFailAff<RT, A>(this Aff<RT, A> ma, Func<Error, Aff<RT, A>> alternative) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, A>(
                async env =>
                {
                    var res = await ma.ReRun(env).ConfigureAwait(false);
                    return res.IsSucc
                               ? res
                               : await alternative(res.Error).Run(env).ConfigureAwait(false);
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> IfFailAff<RT, A>(this Aff<RT, A> ma, Aff<A> alternative) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, A>(
                async env =>
                {
                    var res = await ma.ReRun(env).ConfigureAwait(false);
                    return res.IsSucc
                               ? res
                               : await alternative.ReRun().ConfigureAwait(false);
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> IfFailAff<RT, A>(this Aff<RT, A> ma, Func<Error, Aff<A>> alternative) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, A>(
                async env =>
                {
                    var res = await ma.ReRun(env).ConfigureAwait(false);
                    return res.IsSucc
                               ? res
                               : await alternative(res.Error).Run().ConfigureAwait(false);
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> IfFailAff<RT, A>(this Aff<RT, A> ma, Eff<RT, A> alternative) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, A>(
                async env =>
                {
                    var res = await ma.ReRun(env).ConfigureAwait(false);
                    return res.IsSucc
                               ? res
                               : alternative.ReRun(env);
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> IfFailAff<RT, A>(this Aff<RT, A> ma, Func<Error, Eff<RT, A>> alternative) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, A>(
                async env =>
                {
                    var res = await ma.ReRun(env).ConfigureAwait(false);
                    return res.IsSucc
                               ? res
                               : alternative(res.Error).ReRun(env);
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> IfFailAff<RT, A>(this Aff<RT, A> ma, Eff<A> alternative) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, A>(
                async env =>
                {
                    var res = await ma.ReRun(env).ConfigureAwait(false);
                    return res.IsSucc
                               ? res
                               : alternative.ReRun();
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> IfFailAff<RT, A>(this Aff<RT, A> ma, Func<Error, Eff<A>> alternative) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, A>(
                async env =>
                {
                    var res = await ma.ReRun(env).ConfigureAwait(false);
                    return res.IsSucc
                               ? res
                               : alternative(res.Error).Run();
                });
        
        //
        // Iter / Do
        //
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, Unit> Iter<RT, A>(this Aff<RT, A> ma, Func<A, Unit> f) where RT : struct, HasCancel<RT> =>
            Aff<RT, Unit>(
                async env =>
                {
                    var res = await ma.ReRun(env).ConfigureAwait(false);
                    if (res.IsSucc)
                    {
                        f(res.Value);
                    }
                    return unit;
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, Unit> Iter<RT, A>(this Aff<RT, A> ma, Func<A, Aff<RT, Unit>> f) where RT : struct, HasCancel<RT> =>
            Aff<RT, Unit>(
                async env =>
                {
                    var res = await ma.ReRun(env).ConfigureAwait(false);
                    if (res.IsSucc)
                    {
                        ignore(await f(res.Value).Run(env).ConfigureAwait(false));
                    }
                    return unit;
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, Unit> Iter<RT, A>(this Aff<RT, A> ma, Func<A, Aff<Unit>> f) where RT : struct, HasCancel<RT> =>
            Aff<RT, Unit>(
                async env =>
                {
                    var res = await ma.ReRun(env).ConfigureAwait(false);
                    if (res.IsSucc)
                    {
                        ignore(await f(res.Value).Run().ConfigureAwait(false));
                    }
                    return unit;
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, Unit> Iter<RT, A>(this Aff<RT, A> ma, Func<A, Eff<RT, Unit>> f) where RT : struct, HasCancel<RT> =>
            Aff<RT, Unit>(
                async env =>
                {
                    var res = await ma.ReRun(env).ConfigureAwait(false);
                    if (res.IsSucc)
                    {
                        ignore(f(res.Value).Run(env));
                    }
                    return unit;
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, Unit> Iter<RT, A>(this Aff<RT, A> ma, Func<A, Eff<Unit>> f) where RT : struct, HasCancel<RT> =>
            Aff<RT, Unit>(
                async env =>
                {
                    var res = await ma.ReRun(env).ConfigureAwait(false);
                    if (res.IsSucc)
                    {
                        ignore(f(res.Value).Run());
                    }
                    return unit;
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> Do<RT, A>(this Aff<RT, A> ma, Func<A, Unit> f) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, A>(
                async env =>
                {
                    var res = await ma.ReRun(env).ConfigureAwait(false);
                    if (res.IsSucc)
                    {
                        f(res.Value);
                    }
                    return res;
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> Do<RT, A>(this Aff<RT, A> ma, Func<A, Aff<RT, Unit>> f) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, A>(
                async env =>
                {
                    var res = await ma.ReRun(env).ConfigureAwait(false);
                    if (res.IsSucc)
                    {
                        var ures = await f(res.Value).Run(env).ConfigureAwait(false);
                        return ures.IsFail
                                   ? Fin<A>.Fail(ures.Error)
                                   : res;
                    }
                    return res;
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> Do<RT, A>(this Aff<RT, A> ma, Func<A, Eff<RT, Unit>> f) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, A>(
                async env =>
                {
                    var res = await ma.ReRun(env).ConfigureAwait(false);
                    if (res.IsSucc)
                    {
                        var ures = f(res.Value).Run(env);
                        return ures.IsFail
                                   ? Fin<A>.Fail(ures.Error)
                                   : res;
                    }
                    return res;
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> Do<RT, A>(this Aff<RT, A> ma, Func<A, Aff<Unit>> f) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, A>(
                async env =>
                {
                    var res = await ma.ReRun(env).ConfigureAwait(false);
                    if (res.IsSucc)
                    {
                        var ures = await f(res.Value).Run().ConfigureAwait(false);
                        return ures.IsFail
                                   ? Fin<A>.Fail(ures.Error)
                                   : res;
                    }
                    return res;
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> Do<RT, A>(this Aff<RT, A> ma, Func<A, Eff<Unit>> f) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, A>(
                async env =>
                {
                    var res = await ma.ReRun(env).ConfigureAwait(false);
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
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> Filter<RT, A>(this Aff<RT, A> ma, Func<A, bool> f) where RT : struct, HasCancel<RT> =>
            ma.Bind(x => f(x) ? SuccessEff<A>(x) : FailEff<A>(Errors.Cancelled));


        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> Where<RT, A>(this Aff<RT, A> ma, Func<A, bool> f) where RT : struct, HasCancel<RT> =>
            Filter(ma, f);

        //
        // Bind
        //
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> Bind<RT, A, B>(this Aff<RT, A> ma, Func<A, Eff<B>> f) where RT : struct, HasCancel<RT> =>
            new Aff<RT, B>(ThunkAsync<RT, B>.Lazy(
                               async env =>
                               {
                                   var fa = await ma.ReRun(env);
                                   if (fa.IsFail) return FinFail<B>(fa.Error);
                                   var mb = f(fa.Value);
                                   return mb.Run();
                               }));

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> Bind<RT, A, B>(this Aff<RT, A> ma, Func<A, Eff<RT, B>> f) where RT : struct, HasCancel<RT> =>
            new Aff<RT, B>(ThunkAsync<RT, B>.Lazy(
                               async env =>
                               {
                                   var fa = await ma.ReRun(env);
                                   if (fa.IsFail) return FinFail<B>(fa.Error);
                                   var mb = f(fa.Value);
                                   return mb.Run(env);
                               }));
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> Bind<RT, A, B>(this Aff<RT, A> ma, Func<A, Aff<B>> f) where RT : struct, HasCancel<RT> =>
            new Aff<RT, B>(ThunkAsync<RT, B>.Lazy(
                               async env =>
                               {
                                   var fa = await ma.ReRun(env);
                                   if (fa.IsFail) return FinFail<B>(fa.Error);
                                   var mb = f(fa.Value);
                                   return await mb.Run().ConfigureAwait(false);
                               }));

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> Bind<RT, A, B>(this Aff<RT, A> ma, Func<A, Aff<RT, B>> f) where RT : struct, HasCancel<RT> =>
            new Aff<RT, B>(ThunkAsync<RT, B>.Lazy(
                               async env =>
                               {
                                   var fa = await ma.ReRun(env);
                                   if (fa.IsFail) return FinFail<B>(fa.Error);
                                   var mb = f(fa.Value);
                                   return await mb.Run(env).ConfigureAwait(false);
                               }));    

        //
        // Bi-Bind
        //

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> BiBind<RT, A, B>(this Aff<RT, A> ma, Func<A, Aff<RT, B>> Succ, Func<Error, Aff<RT, B>> Fail) where RT : struct, HasCancel<RT> =>
            ma.Match(Succ, Fail)
                .Flatten();

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> BiBind<RT, A, B>(this Aff<RT, A> ma, Func<A, Aff<B>> Succ, Func<Error, Aff<B>> Fail) where RT : struct, HasCancel<RT> =>
            ma.Match(Succ, Fail)
                .Flatten();

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> BiBind<RT, A, B>(this Aff<RT, A> ma, Func<A, Eff<RT, B>> Succ, Func<Error, Eff<RT, B>> Fail) where RT : struct, HasCancel<RT> =>
            ma.Match(Succ, Fail)
                .Flatten();

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> BiBind<RT, A, B>(this Aff<RT, A> ma, Func<A, LanguageExt.Eff<B>> Succ, Func<Error, LanguageExt.Eff<B>> Fail) where RT : struct, HasCancel<RT> =>
            ma.Match(Succ, Fail)
                .Flatten();

        //
        // Flatten (monadic join)
        //

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> Flatten<RT, A>(this Aff<RT, Aff<RT, A>> ma) where RT : struct, HasCancel<RT> =>
            new Aff<RT, A>(ma.Thunk.Map(ThunkFromIO).Flatten());

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> Flatten<RT, A>(this Aff<RT, Aff<A>> ma) where RT : struct, HasCancel<RT> =>
            new Aff<RT, A>(ma.Thunk.Map(ThunkFromIO).Flatten());

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> Flatten<RT, A>(this Aff<RT, Eff<RT, A>> ma) where RT : struct, HasCancel<RT> =>
            new Aff<RT, A>(ma.Thunk.Map(ThunkFromIO).Flatten());

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> Flatten<RT, A>(this Aff<RT, LanguageExt.Eff<A>> ma) where RT : struct, HasCancel<RT> =>
            new Aff<RT, A>(ma.Thunk.Map(ThunkFromIO).Flatten());

        //
        // Select
        //

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> Select<RT, A, B>(this Aff<RT, A> ma, Func<A, B> f) where RT : struct, HasCancel<RT> =>
            Map(ma, f);

        //
        // SelectMany
        //

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> SelectMany<RT, A, B>(this Aff<RT, A> ma, Func<A, Aff<RT, B>> f) where RT : struct, HasCancel<RT> =>
            Bind(ma, f);

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> SelectMany<RT, A, B>(this Aff<RT, A> ma, Func<A, Aff<B>> f) where RT : struct, HasCancel<RT> =>
            Bind(ma, f);

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> SelectMany<RT, A, B>(this Aff<RT, A> ma, Func<A, Eff<RT, B>> f) where RT : struct, HasCancel<RT> =>
            Bind(ma, f);

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> SelectMany<RT, A, B>(this Aff<RT, A> ma, Func<A, LanguageExt.Eff<B>> f) where RT : struct, HasCancel<RT> =>
            Bind(ma, f);

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, C> SelectMany<RT, A, B, C>(this Aff<RT, A> ma, Func<A, Aff<RT, B>> bind, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            Bind(ma, x => Map(bind(x), y => project(x, y)));

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, C> SelectMany<RT, A, B, C>(this Aff<RT, A> ma, Func<A, Aff<B>> bind, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            Bind(ma, x => Map(bind(x), y => project(x, y)));

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, C> SelectMany<RT, A, B, C>(this Aff<RT, A> ma, Func<A, Eff<RT, B>> bind, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            Bind(ma, x => Map(bind(x), y => project(x, y)));

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, C> SelectMany<RT, A, B, C>(this Aff<RT, A> ma, Func<A, LanguageExt.Eff<B>> bind, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            Bind(ma, x => Map(bind(x), y => project(x, y)));

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, C> SelectMany<RT, A, B, C>(this Aff<RT, A> ma, Func<A, Effect<RT, B>> bind, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            Bind(ma, x => Map(bind(x).RunEffect(), y => project(x, y)));

        //
        // Zip
        //

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, (A, B)> Zip<RT, A, B>(this Aff<RT, A> ma, Aff<RT, B> mb) where RT : struct, HasCancel<RT> =>
            new Aff<RT, (A, B)>(ThunkAsync<RT, (A, B)>.Lazy(async e =>
            {
                var ta = ma.ReRun(e).AsTask();
                var tb = mb.ReRun(e).AsTask();
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

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, (A, B)> Zip<RT, A, B>(this Aff<RT, A> ma, Aff<B> mb) where RT : struct, HasCancel<RT> =>
            new Aff<RT, (A, B)>(ThunkAsync<RT, (A, B)>.Lazy(async e =>
            {
                var ta = ma.ReRun(e).AsTask();
                var tb = mb.ReRun().AsTask();
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


        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, (A, B)> Zip<RT, A, B>(this Aff<A> ma, Aff<RT, B> mb) where RT : struct, HasCancel<RT> =>
            new Aff<RT, (A, B)>(ThunkAsync<RT, (A, B)>.Lazy(async e =>
            {
                var ta = ma.ReRun().AsTask();
                var tb = mb.ReRun(e).AsTask();
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

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, (A, B)> Zip<RT, A, B>(this Aff<RT, A> ma, LanguageExt.Eff<B> mb) where RT : struct, HasCancel<RT> =>
            new Aff<RT, (A, B)>(ThunkAsync<RT, (A, B)>.Lazy(async e =>
            {
                var ta = ma.ReRun(e).AsTask();
                var ra = await ta.ConfigureAwait(false);
                if (!ta.CompletedSuccessfully())
                {
                    return Fin<(A, B)>.Fail(ra.Error);
                }

                var rb = mb.ReRun();
                if (rb.IsFail) return Fin<(A, B)>.Fail(rb.Error);

                return Fin<(A, B)>.Succ((ra.Value, rb.Value));
            }));

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, (A, B)> Zip<RT, A, B>(this LanguageExt.Eff<A> ma, Aff<RT, B> mb) where RT : struct, HasCancel<RT> =>
            new Aff<RT, (A, B)>(ThunkAsync<RT, (A, B)>.Lazy(async e =>
            {
                var ra = ma.ReRun();
                if (ra.IsFail) return Fin<(A, B)>.Fail(ra.Error);

                var tb = mb.ReRun(e).AsTask();
                var rb = await tb.ConfigureAwait(false);
                if (!tb.CompletedSuccessfully())
                {
                    return Fin<(A, B)>.Fail(rb.Error);
                }

                return Fin<(A, B)>.Succ((ra.Value, rb.Value));
            }));


        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, (A, B)> Zip<RT, A, B>(this Aff<RT, A> ma, Eff<RT, B> mb) where RT : struct, HasCancel<RT> =>
            new Aff<RT, (A, B)>(ThunkAsync<RT, (A, B)>.Lazy(async e =>
            {
                var ta = ma.ReRun(e).AsTask();
                var ra = await ta.ConfigureAwait(false);
                if (!ta.CompletedSuccessfully())
                {
                    return Fin<(A, B)>.Fail(ra.Error);
                }

                var rb = mb.ReRun(e);
                if (rb.IsFail) return Fin<(A, B)>.Fail(rb.Error);

                return Fin<(A, B)>.Succ((ra.Value, rb.Value));
            }));

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, (A, B)> Zip<RT, A, B>(this Eff<RT, A> ma, Aff<RT, B> mb) where RT : struct, HasCancel<RT> =>
            new Aff<RT, (A, B)>(ThunkAsync<RT, (A, B)>.Lazy(async e =>
            {
                var ra = ma.ReRun(e);
                if (ra.IsFail) return Fin<(A, B)>.Fail(ra.Error);

                var tb = mb.ReRun(e).AsTask();
                var rb = await tb.ConfigureAwait(false);
                if (!tb.CompletedSuccessfully())
                {
                    return Fin<(A, B)>.Fail(rb.Error);
                }

                return Fin<(A, B)>.Succ((ra.Value, rb.Value));
            }));

        [Pure, MethodImpl(Opt.Default)]
        static ThunkAsync<RT, A> ThunkFromIO<RT, A>(Aff<RT, A> ma) where RT : struct, HasCancel<RT> =>
            ma.Thunk;
    }
}

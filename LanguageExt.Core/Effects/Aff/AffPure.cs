using System;
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
    public readonly struct Aff<A>
    {
        internal const MethodImplOptions mops = MethodImplOptions.AggressiveInlining;

        internal ThunkAsync<A> Thunk => thunk ?? ThunkAsync<A>.Fail(Errors.Bottom);
        readonly ThunkAsync<A> thunk;

        /// <summary>
        /// Constructor
        /// </summary>
        [MethodImpl(Opt.Default)]
        internal Aff(ThunkAsync<A> thunk) =>
            this.thunk = thunk ?? throw new ArgumentNullException(nameof(thunk));

        /// <summary>
        /// Invoke the effect
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public ValueTask<Fin<A>> Run() =>
            Thunk.Value();

        /// <summary>
        /// Invoke the effect
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public ValueTask<Fin<A>> ReRun() =>
            Thunk.ReValue();
        
        /// <summary>
        /// Clone the effect
        /// </summary>
        /// <remarks>
        /// If the effect had already run, then this state will be wiped in the clone, meaning it can be re-run
        /// </remarks>
        [Pure, MethodImpl(Opt.Default)]
        public Aff<A> Clone() =>
            new Aff<A>(Thunk.Clone());        

        /// <summary>
        /// Invoke the effect
        /// </summary>
        /// <remarks>
        /// Throws on error
        /// </remarks>
        [Pure, MethodImpl(Opt.Default)]
        public async ValueTask<Unit> RunUnit() =>
            (await Thunk.Value().ConfigureAwait(false)).Case switch
            {
                A _     => unit,
                Error e => e.Throw(),
                _       => throw new NotSupportedException()
            };

        /// <summary>
        /// Launch the async computation without awaiting the result
        /// </summary>
        /// <returns></returns>
        [MethodImpl(Opt.Default)]
        public Eff<Unit> Fork()
        {
            var t = Thunk;
            return Eff<Unit>(() => ignore(t.Value()));
        }

        /// <summary>
        /// Custom awaiter so Aff can be used with async/await 
        /// </summary>
        ///
        ///     PL: Doesn't seem to play nice with the async/await machinery, so commenting out until I can spend
        ///         some time on it.  Will need to re-add: [AsyncMethodBuilder(typeof(AffMethodBuilder<>))] to the Aff
        ///         struct declaration
        /// 
        //public AffAwaiter<A> GetAwaiter() =>
        //    new AffAwaiter<A>(this);
        
        /// <summary>
        /// Lift an asynchronous effect into the Aff monad
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<A> Effect(Func<ValueTask<A>> f) =>
            new Aff<A>(ThunkAsync<A>.Lazy(f));

        /// <summary>
        /// Lift an asynchronous effect into the Aff monad
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<A> EffectMaybe(Func<ValueTask<Fin<A>>> f) =>
            new Aff<A>(ThunkAsync<A>.Lazy(f));
        
        /// <summary>
        /// Lift an asynchronous effect into the Aff monad
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<Unit> Effect(Func<ValueTask> f) =>
            new Aff<Unit>(ThunkAsync<Unit>.Lazy(async () =>
            {
                await f().ConfigureAwait(false);
                return Fin<Unit>.Succ(default);
            }));

        /// <summary>
        /// Lift a value into the Aff monad 
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<A> Success(A value) =>
            new Aff<A>(ThunkAsync<A>.Success(value));

        /// <summary>
        /// Lift a failure into the Aff monad 
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<A> Fail(Error error) =>
            new Aff<A>(ThunkAsync<A>.Fail(error));

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<A> operator |(Aff<A> ma, Aff<A> mb) => new Aff<A>(ThunkAsync<A>.Lazy(
            async () =>
            {
                var ra = await ma.ReRun().ConfigureAwait(false);
                return ra.IsSucc
                    ? ra
                    : await mb.ReRun().ConfigureAwait(false);
            }));

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<A> operator |(Aff<A> ma, Eff<A> mb) => new Aff<A>(ThunkAsync<A>.Lazy(
            async () =>
            {
                var ra = await ma.ReRun().ConfigureAwait(false);
                return ra.IsSucc
                    ? ra
                    : mb.ReRun();
            }));

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<A> operator |(Eff<A> ma, Aff<A> mb) => new Aff<A>(ThunkAsync<A>.Lazy(
            async () =>
            {
                var ra = ma.ReRun();
                return ra.IsSucc
                    ? ra
                    : await mb.ReRun().ConfigureAwait(false);
            }));

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<A> operator |(Aff<A> ma, EffCatch<A> mb) =>
            new Aff<A>(ThunkAsync<A>.Lazy(
                async () =>
                {
                    var ra = await ma.ReRun().ConfigureAwait(false);
                    return ra.IsSucc
                        ? ra
                        : mb.Run(ra.Error);
                }));

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<A> operator |(Aff<A> ma, AffCatch<A> mb) =>
            new Aff<A>(ThunkAsync<A>.Lazy(
                async () =>
                {
                    var ra = await ma.ReRun().ConfigureAwait(false);
                    return ra.IsSucc
                        ? ra
                        : await mb.Run(ra.Error).ConfigureAwait(false);
                }));
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<A> operator |(Aff<A> ma, CatchValue<A> value) =>
            new Aff<A>(ThunkAsync<A>.Lazy(
                                async () =>
                                {
                                    var ra = await ma.ReRun().ConfigureAwait(false);
                                    return ra.IsSucc
                                               ? ra
                                               : value.Match(ra.Error)
                                                   ? FinSucc(value.Value(ra.Error))
                                                   : ra;
                                }));
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<A> operator |(Aff<A> ma, CatchError value) =>
            new Aff<A>(ThunkAsync<A>.Lazy(
                           async () =>
                           {
                               var ra = await ma.ReRun().ConfigureAwait(false);
                               return ra.IsSucc
                                          ? ra
                                          : value.Match(ra.Error)
                                              ? FinFail<A>(value.Value(ra.Error))
                                              : ra;
                           }));

        /// <summary>
        /// Force the operation to end after a time out delay
        /// </summary>
        /// <remarks>Note, the original operation continues even after this returns.  To cancel the original operation
        /// at the same time as the timeout triggers, use Aff<RT, A> instead of Aff<A> - as it supports cancellation
        /// tokens, and so can automatically cancel the long-running operation</remarks>
        /// <param name="timeoutDelay">Delay for the time out</param>
        /// <returns>Either success if the operation completed before the timeout, or Errors.TimedOut</returns>
        [Pure, MethodImpl(Opt.Default)]
        public Aff<A> Timeout(TimeSpan timeoutDelay)
        {
            var t = Thunk;
            return AffMaybe<A>(
                async () =>
                {
                    using var toksrc    = new CancellationTokenSource();
                    var  delay     = Task.Delay(timeoutDelay, toksrc.Token);
                    var  task      = t.Value().AsTask();
                    var  completed = await Task.WhenAny(new Task[] {delay, task}).ConfigureAwait(false);
                    
                    if (completed == delay)
                    {
                        return FinFail<A>(Errors.TimedOut);
                    }
                    else
                    {
                        toksrc.Cancel();
                        return await task;
                    }
                });
        }        

        [Pure, MethodImpl(Opt.Default)]
        public Aff<RT, A> WithRuntime<RT>() where RT : struct, HasCancel<RT>
        {
            var self = this;
            return Aff<RT, A>.EffectMaybe(e => self.ReRun());
        }

        [Pure, MethodImpl(Opt.Default)]
        public async Task<S> Fold<S>(S state, Func<S, A, S> f)
        {
            var r = await Run().ConfigureAwait(false);
            return r.IsSucc
                ? f(state, r.Value)
                : state;
        }

        [Pure, MethodImpl(Opt.Default)]
        public async Task<bool> Exists(Func<A, bool> f)
        {
            var r = await Run().ConfigureAwait(false);
            return r.IsSucc && f(r.Value);
        }

        [Pure, MethodImpl(Opt.Default)]
        public async Task<bool> ForAll(Func<A, bool> f)
        {
            var r = await Run().ConfigureAwait(false);
            return r.IsFail || (r.IsSucc && f(r.Value));
        }

        /// <summary>
        /// Implicit conversion from pure Eff
        /// </summary>
        public static implicit operator Aff<A>(Eff<A> ma) =>
            EffectMaybe(() => ma.ReRun().AsValueTask());
    }
    
    public static partial class AffExtensions
    {
        //
        // Sequence (with tuples)
        //
        
        /// <summary>
        /// Run the two effects in the tuple in parallel, wait for them all to finish, then return a tuple of the results
        /// </summary>
        public static Aff<(A, B)> Sequence<A, B>(this (Aff<A>, Aff< B>) ms) => 
            AffMaybe<(A, B)>(async () =>
            {
                var t1 = ms.Item1.ReRun().AsTask();
                var t2 = ms.Item2.ReRun().AsTask();
                
                var tasks = new Task[] {t1, t2};
                await Task.WhenAll(tasks);
                return from r1 in t1.Result
                       from r2 in t2.Result
                       select (r1, r2);
            });

        /// <summary>
        /// Run the three effects in the tuple in parallel, wait for them all to finish, then return a tuple of the results
        /// </summary>
        public static Aff<(A, B, C)> Sequence<A, B, C>(this (Aff<A>, Aff<B>, Aff<C>) ms) => 
            AffMaybe<(A, B, C)>(async () =>
            {
                var t1 = ms.Item1.ReRun().AsTask();
                var t2 = ms.Item2.ReRun().AsTask();
                var t3 = ms.Item3.ReRun().AsTask();
                
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
        public static Aff<(A, B, C, D)> Sequence<A, B, C, D>(this (Aff<A>, Aff<B>, Aff<C>, Aff< D>) ms) => 
            AffMaybe<(A, B, C, D)>(async () =>
            {
                var t1 = ms.Item1.ReRun().AsTask();
                var t2 = ms.Item2.ReRun().AsTask();
                var t3 = ms.Item3.ReRun().AsTask();
                var t4 = ms.Item4.ReRun().AsTask();
                
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
        public static Aff<(A, B, C, D, E)> Sequence<A, B, C, D, E>(this (Aff<A>, Aff<B>, Aff<C>, Aff<D>, Aff<E>) ms) => 
            AffMaybe<(A, B, C, D, E)>(async () =>
            {
                var t1 = ms.Item1.ReRun().AsTask();
                var t2 = ms.Item2.ReRun().AsTask();
                var t3 = ms.Item3.ReRun().AsTask();
                var t4 = ms.Item4.ReRun().AsTask();
                var t5 = ms.Item5.ReRun().AsTask();
                
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
        public static Aff<B> Map<A, B>(this Aff<A> ma, Func<A, B> f) =>
            new Aff<B>(ma.Thunk.Map(f));

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<B> MapAsync<A, B>(this Aff<A> ma, Func<A, ValueTask<B>> f) =>
            new Aff<B>(ma.Thunk.MapAsync(f));

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<A> MapFail<A>(this Aff<A> ma, Func<Error, Error> f) =>
            ma.BiMap(identity, f);

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<A> MapFailAsync<A>(this Aff<A> ma, Func<Error, ValueTask<Error>> f) =>
            ma.BiMapAsync(x => x.AsValueTask(), f);

        //
        // Bi-map
        //
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<B> BiMap<A, B>(this Aff<A> ma, Func<A, B> Succ, Func<Error, Error> Fail) =>
            new Aff<B>(ma.Thunk.BiMap(Succ, Fail));

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<B> BiMapAsync<A, B>(this Aff<A> ma, Func<A, ValueTask<B>> Succ, Func<Error, ValueTask<Error>> Fail) =>
            new Aff<B>(ma.Thunk.BiMapAsync(Succ, Fail));

        //
        // Match
        //

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<B> Match<A, B>(this Aff<A> ma, Func<A, B> Succ, Func<Error, B> Fail) =>
            Aff(async () => { 
                var r = await ma.ReRun().ConfigureAwait(false);
                return r.IsSucc
                    ? Succ(r.Value)
                    : Fail(r.Error);
            });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> MatchAff<RT, A, B>(this Aff<A> ma, Func<A, B> Succ, Aff<RT, B> Fail) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, B>(async env => {
                var r = await ma.ReRun().ConfigureAwait(false);
                return r.IsSucc
                    ? Succ(r.Value)
                    : await Fail.ReRun(env).ConfigureAwait(false);
            });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> MatchAff<RT, A, B>(this Aff<A> ma, Func<A, B> Succ, Func<Error, Aff<RT, B>> Fail) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, B>(async env => {
                                 var r = await ma.ReRun().ConfigureAwait(false);
                                 return r.IsSucc
                                            ? Succ(r.Value)
                                            : await Fail(r.Error).Run(env).ConfigureAwait(false);
                             });

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> MatchAff<RT, A, B>(this Aff<A> ma, Aff<RT, B> Succ, Func<Error, B> Fail) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, B>(async env => {
                var r = await ma.ReRun().ConfigureAwait(false);
                return r.IsSucc
                    ? await Succ.ReRun(env).ConfigureAwait(false)
                    : Fail(r.Error);
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> MatchAff<RT, A, B>(this Aff<A> ma, Func<A, Aff<RT, B>> Succ, Func<Error, B> Fail) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, B>(async env => {
                                 var r = await ma.ReRun().ConfigureAwait(false);
                                 return r.IsSucc
                                            ? await Succ(r.Value).Run(env).ConfigureAwait(false)
                                            : Fail(r.Error);
                             });

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<B> MatchAff<A, B>(this Aff<A> ma, Aff<B> Succ, Func<Error, B> Fail) =>
            AffMaybe<B>(async () =>
            {
                var r = await ma.ReRun().ConfigureAwait(false);
                return r.IsSucc
                           ? await Succ.ReRun().ConfigureAwait(false)
                           : Fail(r.Error);
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<B> MatchAff<A, B>(this Aff<A> ma, Func<A, Aff<B>> Succ, Func<Error, B> Fail) =>
            AffMaybe<B>(async () =>
                        {
                            var r = await ma.ReRun().ConfigureAwait(false);
                            return r.IsSucc
                                       ? await Succ(r.Value).Run().ConfigureAwait(false)
                                       : Fail(r.Error);
                        });

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> MatchAff<RT, A, B>(this Aff<A> ma, Aff<RT, B> Succ, Aff<RT, B> Fail) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, B>(async env =>
            {
                var r = await ma.ReRun().ConfigureAwait(false);
                return r.IsSucc
                    ? await Succ.ReRun(env).ConfigureAwait(false)
                    : await Fail.ReRun(env).ConfigureAwait(false);
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> MatchAff<RT, A, B>(this Aff<A> ma, Func<A, Aff<RT, B>> Succ, Func<Error, Aff<RT, B>> Fail) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, B>(async env =>
                             {
                                 var r = await ma.ReRun().ConfigureAwait(false);
                                 return r.IsSucc
                                            ? await Succ(r.Value).Run(env).ConfigureAwait(false)
                                            : await Fail(r.Error).Run(env).ConfigureAwait(false);
                             });

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<B> MatchAff<A, B>(this Aff<A> ma, Aff<B> Succ, Aff<B> Fail) =>
            AffMaybe<B>(async () =>
            {
                var r = await ma.ReRun().ConfigureAwait(false);
                return r.IsSucc
                    ? await Succ.ReRun().ConfigureAwait(false)
                    : await Fail.ReRun().ConfigureAwait(false);
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<B> MatchAff<A, B>(this Aff<A> ma, Func<A, Aff<B>> Succ, Func<Error, Aff<B>> Fail) =>
            AffMaybe<B>(async () =>
                        {
                            var r = await ma.ReRun().ConfigureAwait(false);
                            return r.IsSucc
                                       ? await Succ(r.Value).Run().ConfigureAwait(false)
                                       : await Fail(r.Error).Run().ConfigureAwait(false);
                        });

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<B> Match<A, B>(this Aff<A> ma, B Succ, Func<Error, B> Fail) =>
            Aff<B>(async () =>
            {
                var r = await ma.ReRun().ConfigureAwait(false);
                return r.IsSucc
                    ? Succ
                    : Fail(r.Error);
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<B> Match<A, B>(this Aff<A> ma, Func<A, B> Succ, B Fail) =>
            Aff<B>(async () =>
            {
                var r = await ma.ReRun().ConfigureAwait(false);
                return r.IsSucc
                           ? Succ(r.Value)
                           : Fail;
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<B> Match<A, B>(this Aff<A> ma, B Succ, B Fail) =>
            Aff<B>(async () =>
            {
                var r = await ma.ReRun().ConfigureAwait(false);
                return r.IsSucc
                           ? Succ
                           : Fail;
            });
        
        //
        // IfNone
        //
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<A> IfFail<A>(this Aff<A> ma, Func<Error, A> f) =>
            AffMaybe<A>(
                async () =>
                {
                    var res = await ma.ReRun().ConfigureAwait(false);
                    return res.IsSucc
                               ? res
                               : Fin<A>.Succ(f(res.Error));
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<A> IfFail<A>(this Aff<A> ma, A alternative) =>
            AffMaybe<A>(
                async () =>
                {
                    var res = await ma.ReRun().ConfigureAwait(false);
                    return res.IsSucc
                               ? res
                               : Fin<A>.Succ(alternative);
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> IfFailAff<RT, A>(this Aff<A> ma, Aff<RT, A> alternative) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, A>(
                async env =>
                {
                    var res = await ma.ReRun().ConfigureAwait(false);
                    return res.IsSucc
                               ? res
                               : await alternative.ReRun(env).ConfigureAwait(false);
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> IfFailAff<RT, A>(this Aff<A> ma, Func<Error, Aff<RT, A>> alternative) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, A>(
                async env =>
                {
                    var res = await ma.ReRun().ConfigureAwait(false);
                    return res.IsSucc
                               ? res
                               : await alternative(res.Error).Run(env).ConfigureAwait(false);
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<A> IfFailAff<A>(this Aff<A> ma, Aff<A> alternative) =>
            AffMaybe<A>(
                async () =>
                {
                    var res = await ma.ReRun().ConfigureAwait(false);
                    return res.IsSucc
                               ? res
                               : await alternative.ReRun().ConfigureAwait(false);
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<A> IfFailAff<A>(this Aff<A> ma, Func<Error, Aff<A>> alternative) =>
            AffMaybe<A>(
                async () =>
                {
                    var res = await ma.ReRun().ConfigureAwait(false);
                    return res.IsSucc
                               ? res
                               : await alternative(res.Error).Run().ConfigureAwait(false);
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> IfFailAff<RT, A>(this Aff<A> ma, Eff<RT, A> alternative) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, A>(
                async env =>
                {
                    var res = await ma.ReRun().ConfigureAwait(false);
                    return res.IsSucc
                               ? res
                               : alternative.ReRun(env);
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> IfFailAff<RT, A>(this Aff<A> ma, Func<Error, Eff<RT, A>> alternative) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, A>(
                async env =>
                {
                    var res = await ma.ReRun().ConfigureAwait(false);
                    return res.IsSucc
                               ? res
                               : alternative(res.Error).ReRun(env);
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<A> IfFailAff<A>(this Aff<A> ma, Eff<A> alternative) =>
            AffMaybe<A>(
                async () =>
                {
                    var res = await ma.ReRun().ConfigureAwait(false);
                    return res.IsSucc
                               ? res
                               : alternative.ReRun();
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<A> IfFailAff<A>(this Aff<A> ma, Func<Error, Eff<A>> alternative) =>
            AffMaybe<A>(
                async () =>
                {
                    var res = await ma.ReRun().ConfigureAwait(false);
                    return res.IsSucc
                               ? res
                               : alternative(res.Error).ReRun();
                });
        
        //
        // Iter / Do
        //
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<Unit> Iter<A>(this Aff<A> ma, Func<A, Unit> f) =>
            Aff<Unit>(
                async () =>
                {
                    var res = await ma.ReRun().ConfigureAwait(false);
                    if (res.IsSucc)
                    {
                        f(res.Value);
                    }
                    return unit;
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, Unit> Iter<RT, A>(this Aff<A> ma, Func<A, Aff<RT, Unit>> f) where RT : struct, HasCancel<RT> =>
            Aff<RT, Unit>(
                async env =>
                {
                    var res = await ma.ReRun().ConfigureAwait(false);
                    if (res.IsSucc)
                    {
                        ignore(await f(res.Value).Run(env).ConfigureAwait(false));
                    }
                    return unit;
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<Unit> Iter<A>(this Aff<A> ma, Func<A, Aff<Unit>> f) =>
            Aff<Unit>(
                async () =>
                {
                    var res = await ma.ReRun().ConfigureAwait(false);
                    if (res.IsSucc)
                    {
                        ignore(await f(res.Value).Run().ConfigureAwait(false));
                    }
                    return unit;
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, Unit> Iter<RT, A>(this Aff<A> ma, Func<A, Eff<RT, Unit>> f) where RT : struct, HasCancel<RT> =>
            Aff<RT, Unit>(
                async env =>
                {
                    var res = await ma.ReRun().ConfigureAwait(false);
                    if (res.IsSucc)
                    {
                        ignore(f(res.Value).Run(env));
                    }
                    return unit;
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<Unit> Iter<A>(this Aff<A> ma, Func<A, Eff<Unit>> f) =>
            Aff<Unit>(
                async () =>
                {
                    var res = await ma.ReRun().ConfigureAwait(false);
                    if (res.IsSucc)
                    {
                        ignore(f(res.Value).Run());
                    }
                    return unit;
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<A> Do<A>(this Aff<A> ma, Func<A, Unit> f) =>
            AffMaybe<A>(
                async () =>
                {
                    var res = await ma.ReRun().ConfigureAwait(false);
                    if (res.IsSucc)
                    {
                        f(res.Value);
                    }
                    return res;
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> Do<RT, A>(this Aff<A> ma, Func<A, Aff<RT, Unit>> f) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, A>(
                async env =>
                {
                    var res = await ma.ReRun().ConfigureAwait(false);
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
        public static Aff<RT, A> Do<RT, A>(this Aff<A> ma, Func<A, Eff<RT, Unit>> f) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, A>(
                async env =>
                {
                    var res = await ma.ReRun().ConfigureAwait(false);
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
        public static Aff<A> Do<A>(this Aff<A> ma, Func<A, Aff<Unit>> f) =>
            AffMaybe<A>(
                async () =>
                {
                    var res = await ma.ReRun().ConfigureAwait(false);
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
        public static Aff<A> Do<A>(this Aff<A> ma, Func<A, Eff<Unit>> f) =>
            AffMaybe<A>(
                async () =>
                {
                    var res = await ma.ReRun().ConfigureAwait(false);
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
        // Filter
        //

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<A> Filter<A>(this Aff<A> ma, Func<A, bool> f) =>
            ma.Bind(x => f(x) ? SuccessEff<A>(x) : FailEff<A>(Errors.Cancelled));        

        //
        // Bind
        //

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<B> Bind<A, B>(this Aff<A> ma, Func<A, Eff<B>> f) =>
            new Aff<B>(ThunkAsync<B>.Lazy(
                           async () =>
                           {
                               var fa = await ma.ReRun();
                               if (fa.IsFail) return FinFail<B>(fa.Error);
                               var mb = f(fa.Value);
                               return mb.Run();
                           }));

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> Bind<RT, A, B>(this Aff<A> ma, Func<A, Eff<RT, B>> f) where RT : struct, HasCancel<RT> =>
            new Aff<RT, B>(ThunkAsync<RT, B>.Lazy(
                               async env =>
                               {
                                   var fa = await ma.ReRun();
                                   if (fa.IsFail) return FinFail<B>(fa.Error);
                                   var mb = f(fa.Value);
                                   return mb.Run(env);
                               }));
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<B> Bind<A, B>(this Aff<A> ma, Func<A, Aff<B>> f) =>
            new Aff<B>(ThunkAsync<B>.Lazy(
                               async () =>
                               {
                                   var fa = await ma.ReRun();
                                   if (fa.IsFail) return FinFail<B>(fa.Error);
                                   var mb = f(fa.Value);
                                   return await mb.Run().ConfigureAwait(false);
                               }));

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> Bind<RT, A, B>(this Aff<A> ma, Func<A, Aff<RT, B>> f) where RT : struct, HasCancel<RT> =>
            new Aff<RT, B>(ThunkAsync<RT, B>.Lazy(
                               async env =>
                               {
                                   var fa = await ma.ReRun();
                                   if (fa.IsFail) return FinFail<B>(fa.Error);
                                   var mb = f(fa.Value);
                                   return await mb.Run(env).ConfigureAwait(false);
                               }));

        //
        // Bi-bind
        //

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> BiBind<RT, A, B>(this Aff<A> ma, Func<A, Aff<RT, B>> Succ, Func<Error, Aff<RT, B>> Fail) where RT : struct, HasCancel<RT> =>
            ma.Match(Succ, Fail)
              .Flatten();

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<B> BiBind<A, B>(this Aff<A> ma, Func<A, Aff<B>> Succ, Func<Error, Aff<B>> Fail) =>
            ma.Match(Succ, Fail)
              .Flatten();

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> BiBind<RT, A, B>(this Aff<A> ma, Func<A, Eff<RT, B>> Succ, Func<Error, Eff<RT, B>> Fail) where RT : struct, HasCancel<RT> =>
            ma.Match(Succ, Fail)
              .Flatten();

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<B> BiBind<A, B>(this Aff<A> ma, Func<A, Eff<B>> Succ, Func<Error, Eff<B>> Fail) =>
            ma.Match(Succ, Fail)
              .Flatten();

        //
        // Flatten
        //
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<A> Flatten<A>(this Aff<Aff<A>> ma) =>
            new Aff<A>(ma.Thunk.Map(ThunkFromIO).Flatten());
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> Flatten<RT, A>(this Aff<Aff<RT, A>> ma) where RT : struct, HasCancel<RT> =>
            new Aff<RT, A>(ma.Thunk.Map(ThunkFromIO).Flatten());
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<A> Flatten<A>(this Aff<Eff<A>> ma) =>
            new Aff<A>(ma.Thunk.Map(ThunkFromIO).Flatten());
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> Flatten<RT, A>(this Aff<Eff<RT, A>> ma) where RT : struct, HasCancel<RT> =>
            new Aff<RT, A>(ma.Thunk.Map(ThunkFromIO).Flatten());

        //
        // Select
        //
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<B> Select<A, B>(this Aff<A> ma, Func<A, B> f) =>
            Map(ma, f);

        //
        // SelectMany
        //
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> SelectMany<RT, A, B>(this Aff<A> ma, Func<A, Aff<RT, B>> f) where RT : struct, HasCancel<RT> =>
            Bind(ma, f);
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<B> SelectMany<A, B>(this Aff<A> ma, Func<A, Aff<B>> f) =>
            Bind(ma, f);
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> SelectMany<RT, A, B>(this Aff<A> ma, Func<A, Eff<RT, B>> f) where RT : struct, HasCancel<RT> =>
            Bind(ma, f);
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<B> SelectMany<A, B>(this Aff<A> ma, Func<A, Eff<B>> f) =>
            Bind(ma, f);
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, C> SelectMany<RT, A, B, C>(this Aff<A> ma, Func<A, Aff<RT, B>> bind, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            Bind(ma, x => Map(bind(x), y => project(x, y)));
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<C> SelectMany<A, B, C>(this Aff<A> ma, Func<A, Aff<B>> bind, Func<A, B, C> project) =>
            Bind(ma, x => Map(bind(x), y => project(x, y)));
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, C> SelectMany<RT, A, B, C>(this Aff<A> ma, Func<A, Eff<RT, B>> bind, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            Bind(ma, x => Map(bind(x), y => project(x, y)));
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<C> SelectMany<A, B, C>(this Aff<A> ma, Func<A, Eff<B>> bind, Func<A, B, C> project) =>
            Bind(ma, x => Map(bind(x), y => project(x, y)));
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, C> SelectMany<RT, A, B, C>(this Aff<A> ma, Func<A, Effect<RT, B>> bind, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            Bind(ma, x => Map(bind(x).RunEffect(), y => project(x, y)));
        
        //
        // Where
        //
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<A> Where<A>(this Aff<A> ma, Func<A, bool> f) =>
            Filter(ma, f);
        
        //
        // Zip
        //
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<(A, B)> Zip<A, B>(Aff<A> ma, Aff<B> mb) =>
            new Aff<(A, B)>(ThunkAsync<(A, B)>.Lazy(async () =>
            {
                var ta = ma.ReRun().AsTask();
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
        public static Aff<(A, B)> Zip<A, B>(Aff<A> ma, Eff<B> mb) =>
            new Aff<(A, B)>(ThunkAsync<(A, B)>.Lazy(async () =>
            {
                var ta = ma.ReRun().AsTask();
                var ra = await ta.ConfigureAwait(false);
                if (!ta.CompletedSuccessfully())
                {
                    return Fin<(A, B)>.Fail(ra.Error);
                }

                var rb = mb.ReRun();
                if(rb.IsFail) return Fin<(A, B)>.Fail(rb.Error);

                return Fin<(A, B)>.Succ((ra.Value, rb.Value));
            }));  
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<(A, B)> Zip<A, B>(Eff<A> ma, Aff<B> mb) =>
            new Aff<(A, B)>(ThunkAsync<(A, B)>.Lazy(async () =>
            {
                var ra = ma.ReRun();
                if(ra.IsFail) return Fin<(A, B)>.Fail(ra.Error);
                
                var tb = mb.ReRun().AsTask();
                var rb = await tb.ConfigureAwait(false);
                if (!tb.CompletedSuccessfully())
                {
                    return Fin<(A, B)>.Fail(rb.Error);
                }

                return Fin<(A, B)>.Succ((ra.Value, rb.Value));
            }));         
        
        [Pure, MethodImpl(Opt.Default)]
        static ThunkAsync<A> ThunkFromIO<A>(Aff<A> ma) =>
            ma.Thunk;
        
    }    
}

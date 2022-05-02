#nullable enable
using System;
using LanguageExt.Common;
using System.Threading.Tasks;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;
using LanguageExt.Effects.Traits;

namespace LanguageExt
{
    /// <summary>
    /// Asynchronous effect monad
    /// </summary>
    public readonly struct Aff<RT, A> 
        where RT : struct, HasCancel<RT>
    {
        internal Func<RT, ValueTask<Fin<A>>> Thunk => thunk ?? (_ => FinFail<A>(Errors.Bottom).AsValueTask());
        readonly Func<RT, ValueTask<Fin<A>>> thunk;

        /// <summary>
        /// Constructor
        /// </summary>
        [MethodImpl(Opt.Default)]
        internal Aff(Func<RT, ValueTask<Fin<A>>> thunk) =>
            this.thunk = thunk ?? throw new ArgumentNullException(nameof(thunk));

        /// <summary>
        /// Invoke the effect
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public async ValueTask<Fin<A>> Run(RT runtime)
        {
            try
            {
                return await Thunk(runtime).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                return FinFail<A>(e);
            }
        }

        /// <summary>
        /// Invoke the effect
        /// </summary>
        /// <remarks>
        /// Throws on error
        /// </remarks>
        [MethodImpl(Opt.Default)]
        public async ValueTask<Unit> RunUnit(RT runtime) =>
            (await Thunk(runtime).ConfigureAwait(false)).Case switch
            {
                A       => unit,
                Error e => e.Throw(),
                _       => throw new NotSupportedException()
            };

        /// <summary>
        /// Memoise the result, so subsequent calls don't invoke the side-effect
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public Aff<RT, A> Memo()
        {
            var thnk = Thunk;
            Fin<A> mr = default;

            return new Aff<RT, A>(async rt =>
            {
                if (mr.IsSucc) return mr;
                mr = await thnk(rt).ConfigureAwait(false);
                return mr;
            });
        }

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
                    ignore(t(lenv).Iter(_ => Dispose()));
                    
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
            new (async rt => FinSucc(await f(rt).ConfigureAwait(false)));

        /// <summary>
        /// Lift an asynchronous effect into the Aff monad
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> EffectMaybe(Func<RT, ValueTask<Fin<A>>> f) =>
            new (f);

        /// <summary>
        /// Lift an asynchronous effect into the Aff monad
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, Unit> Effect(Func<RT, ValueTask> f) =>
            new (async rt =>
            {
                await f(rt).ConfigureAwait(false);
                return FinSucc<Unit>(default);
            });

        /// <summary>
        /// Lift a value into the Aff monad 
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> Success(A value) =>
            new (_ => FinSucc(value).AsValueTask());

        /// <summary>
        /// Lift a failure into the Aff monad 
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> Fail(Error error) =>
            new (_ => FinFail<A>(error).AsValueTask());

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
                    var task       = t(lenv).AsTask();
                    var completed  = await Task.WhenAny(new Task[] {delay, task}).ConfigureAwait(false);

                    if (completed == delay)
                    {
                        lenv.CancellationTokenSource.Cancel();
                        return FinFail<A>(Errors.TimedOut);
                    }
                    else
                    {
                        delayTokSrc.Cancel();
                        return await task.ConfigureAwait(false);
                    }
                });
        }

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> operator |(Aff<RT, A> ma, Aff<RT, A> mb) =>
            new(async env =>
            {
                var ra = await ma.Run(env).ConfigureAwait(false);
                return ra.IsSucc
                    ? ra
                    : await mb.Run(env).ConfigureAwait(false);
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> operator |(Aff<RT, A> ma, Aff<A> mb) =>
            new(async env =>
            {
                var ra = await ma.Run(env).ConfigureAwait(false);
                return ra.IsSucc
                    ? ra
                    : await mb.Run().ConfigureAwait(false);
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> operator |(Aff<A> ma, Aff<RT, A> mb) =>
            new(async env =>
            {
                var ra = await ma.Run().ConfigureAwait(false);
                return ra.IsSucc
                    ? ra
                    : await mb.Run(env).ConfigureAwait(false);
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> operator |(Aff<RT, A> ma, Eff<RT, A> mb) =>
            new(async env =>
            {
                var ra = await ma.Run(env).ConfigureAwait(false);
                return ra.IsSucc
                    ? ra
                    : mb.Run(env);
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> operator |(Eff<RT, A> ma, Aff<RT, A> mb) =>
            new(async env =>
            {
                var ra = ma.Run(env);
                return ra.IsSucc
                    ? ra
                    : await mb.Run(env).ConfigureAwait(false);
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> operator |(Eff<A> ma, Aff<RT, A> mb) =>
            new(async env =>
            {
                var ra = ma.Run();
                return ra.IsSucc
                    ? ra
                    : await mb.Run(env).ConfigureAwait(false);
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> operator |(Aff<RT, A> ma, Eff<A> mb) =>
            new(async env =>
            {
                var ra = await ma.Run(env).ConfigureAwait(false);
                return ra.IsSucc
                    ? ra
                    : mb.Run();
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> operator |(Aff<RT, A> ma, EffCatch<A> mb) =>
            new(async env =>
            {
                var ra = await ma.Run(env).ConfigureAwait(false);
                return ra.IsSucc
                    ? ra
                    : mb.Run(ra.Error);
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> operator |(Aff<RT, A> ma, AffCatch<A> mb) =>
            new(async env =>
            {
                var ra = await ma.Run(env).ConfigureAwait(false);
                return ra.IsSucc
                    ? ra
                    : await mb.Run(ra.Error).ConfigureAwait(false);
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> operator |(Aff<RT, A> ma, EffCatch<RT, A> mb) =>
            new(async env =>
            {
                var ra = await ma.Run(env).ConfigureAwait(false);
                return ra.IsSucc
                    ? ra
                    : mb.Run(env, ra.Error);
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> operator |(Aff<RT, A> ma, AffCatch<RT, A> mb) =>
            new(async env =>
            {
                var ra = await ma.Run(env).ConfigureAwait(false);
                return ra.IsSucc
                    ? ra
                    : await mb.Run(env, ra.Error).ConfigureAwait(false);
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> operator |(Aff<RT, A> ma, CatchValue<A> value) =>
            new(async env =>
            {
                var ra = await ma.Run(env).ConfigureAwait(false);
                return ra.IsSucc
                    ? ra
                    : value.Match(ra.Error)
                        ? FinSucc(value.Value(ra.Error))
                        : ra;
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> operator |(Aff<RT, A> ma, CatchError value) =>
            new(async env =>
            {
                var ra = await ma.Run(env).ConfigureAwait(false);
                return ra.IsSucc
                    ? ra
                    : value.Match(ra.Error)
                        ? FinFail<A>(value.Value(ra.Error))
                        : ra;
            });

        /// <summary>
        /// Implicit conversion from pure Aff
        /// </summary>
        public static implicit operator Aff<RT, A>(Aff<A> ma) =>
            EffectMaybe(_ => ma.Run());

        /// <summary>
        /// Implicit conversion from pure Eff
        /// </summary>
        public static implicit operator Aff<RT, A>(Eff<A> ma) =>
            EffectMaybe(_ => ma.Run().AsValueTask());

        /// <summary>
        /// Implicit conversion from Eff
        /// </summary>
        public static implicit operator Aff<RT, A>(Eff<RT, A> ma) =>
            EffectMaybe(env => ma.Run(env).AsValueTask());
    }
}

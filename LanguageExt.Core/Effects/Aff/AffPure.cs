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
    public readonly struct Aff<A>
    {
        internal const MethodImplOptions mops = MethodImplOptions.AggressiveInlining;

        internal Func<CancellationToken, ValueTask<Fin<A>>> Thunk => thunk ?? (_ => FinFail<A>(Errors.Bottom).AsValueTask());
        readonly Func<CancellationToken, ValueTask<Fin<A>>> thunk;

        /// <summary>
        /// Constructor
        /// </summary>
        [MethodImpl(Opt.Default)]
        internal Aff(Func<ValueTask<Fin<A>>> thunk) :
            this(_ => thunk?.Invoke() ?? throw new ArgumentNullException(nameof(thunk)))
        {
        }
        
        /// <summary>
        /// Constructor
        /// </summary>
        [MethodImpl(Opt.Default)]
        internal Aff(Func<CancellationToken, ValueTask<Fin<A>>> thunk) =>
            this.thunk = thunk ?? throw new ArgumentNullException(nameof(thunk));

        /// <summary>
        /// Invoke the effect
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public async ValueTask<Fin<A>> Run(CancellationToken token = default)
        {
            try
            {
                return await Thunk(token).ConfigureAwait(false);
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
        [Pure, MethodImpl(Opt.Default)]
        public async ValueTask<Unit> RunUnit(CancellationToken token = default) =>
            (await Thunk(token).ConfigureAwait(false)).Case switch
            {
                A       => unit,
                Error e => e.Throw(),
                _       => throw new NotSupportedException()
            };

        /// <summary>
        /// Memoise the result, so subsequent calls don't invoke the side-effect
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public Aff<A> Memo()
        {
            var thnk = Thunk;
            Fin<A> mr = default;

            return new Aff<A>(async token =>
            {
                if (mr.IsSucc) return mr;
                mr = await thnk(token).ConfigureAwait(false);
                return mr;
            });
        }

        /// <summary>
        /// Launch the async computation without awaiting the result
        /// </summary>
        /// <returns></returns>
        [MethodImpl(Opt.Default)]
        public Eff<Eff<Unit>> Fork()
        {
            var t = Thunk;
            return Eff(() =>
            {
                var cts = new CancellationTokenSource();
                var task = Task.Run(async () => await t(cts.Token), cts.Token);
                return Eff(() =>
                {
                     cts.Cancel();
                     task.Result.ThrowIfFail();
                     return unit;
                });
            });
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
            new (async () => FinSucc(await f().ConfigureAwait(false)));
        
        /// <summary>
        /// Lift an asynchronous effect into the Aff monad
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<A> Effect(Func<CancellationToken, ValueTask<A>> f) =>
            new (async token => FinSucc(await f(token).ConfigureAwait(false)));

        /// <summary>
        /// Lift an asynchronous effect into the Aff monad
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<A> EffectMaybe(Func<ValueTask<Fin<A>>> f) =>
            new (f);
        
        /// <summary>
        /// Lift an asynchronous effect into the Aff monad
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<A> EffectMaybe(Func<CancellationToken, ValueTask<Fin<A>>> f) =>
            new (f);

        /// <summary>
        /// Lift an asynchronous effect into the Aff monad
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<Unit> Effect(Func<ValueTask> f) =>
            new(async () =>
            {
                await f().ConfigureAwait(false);
                return Fin<Unit>.Succ(default);
            });

        /// <summary>
        /// Lift a value into the Aff monad 
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<A> Success(A value) =>
            new(() => FinSucc(value).AsValueTask());

        /// <summary>
        /// Lift a failure into the Aff monad 
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<A> Fail(Error error) =>
            new(() => FinFail<A>(error).AsValueTask());

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<A> operator |(Aff<A> ma, Aff<A> mb) =>
            new(async token =>
            {
                var ra = await ma.Run(token).ConfigureAwait(false);
                return ra.IsSucc
                    ? ra
                    : await mb.Run(token).ConfigureAwait(false);
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<A> operator |(Aff<A> ma, Eff<A> mb) =>
            new(async token =>
            {
                var ra = await ma.Run(token).ConfigureAwait(false);
                return ra.IsSucc
                    ? ra
                    : mb.Run();
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<A> operator |(Eff<A> ma, Aff<A> mb) =>
            new(async token =>
            {
                var ra = ma.Run();
                return ra.IsSucc
                    ? ra
                    : await mb.Run(token).ConfigureAwait(false);
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<A> operator |(Aff<A> ma, EffCatch<A> mb) =>
            new(async token =>
            {
                var ra = await ma.Run(token).ConfigureAwait(false);
                return ra.IsSucc
                    ? ra
                    : mb.Run(ra.Error);
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<A> operator |(Aff<A> ma, AffCatch<A> mb) =>
            new(async token =>
            {
                var ra = await ma.Run(token).ConfigureAwait(false);
                return ra.IsSucc
                    ? ra
                    : await mb.Run(ra.Error).ConfigureAwait(false);
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<A> operator |(Aff<A> ma, CatchValue<A> value) =>
            new(async token =>
            {
                var ra = await ma.Run(token).ConfigureAwait(false);
                return ra.IsSucc
                    ? ra
                    : value.Match(ra.Error)
                        ? FinSucc(value.Value(ra.Error))
                        : ra;
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<A> operator |(Aff<A> ma, CatchError value) =>
            new(async token =>
            {
                var ra = await ma.Run(token).ConfigureAwait(false);
                return ra.IsSucc
                    ? ra
                    : value.Match(ra.Error)
                        ? FinFail<A>(value.Value(ra.Error))
                        : ra;
            });

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
            return AffMaybe(
                async token =>
                {
                    using var toksrc    = new CancellationTokenSource();
                    var  delay     = Task.Delay(timeoutDelay, toksrc.Token);
                    var  task      = t(token).AsTask();
                    var  completed = await Task.WhenAny(delay, task).ConfigureAwait(false);
                    
                    if (completed == delay)
                    {
                        return FinFail<A>(Errors.TimedOut);
                    }
                    else
                    {
                        toksrc.Cancel();
                        return await task.ConfigureAwait(false);
                    }
                });
        }        

        [Pure, MethodImpl(Opt.Default)]
        public Aff<RT, A> WithRuntime<RT>() where RT : struct, HasCancel<RT>
        {
            var self = this;
            return Aff<RT, A>.EffectMaybe(rt => self.Run(rt.CancellationToken));
        }

        [Pure, MethodImpl(Opt.Default)]
        public async ValueTask<S> Fold<S>(S state, Func<S, A, S> f, CancellationToken token = default)
        {
            var r = await Run(token).ConfigureAwait(false);
            return r.IsSucc
                ? f(state, r.Value)
                : state;
        }

        [Pure, MethodImpl(Opt.Default)]
        public async ValueTask<bool> Exists(Func<A, bool> f, CancellationToken token = default)
        {
            var r = await Run(token).ConfigureAwait(false);
            return r.IsSucc && f(r.Value);
        }

        [Pure, MethodImpl(Opt.Default)]
        public async ValueTask<bool> ForAll(Func<A, bool> f, CancellationToken token = default)
        {
            var r = await Run(token).ConfigureAwait(false);
            return r.IsFail || (r.IsSucc && f(r.Value));
        }

        /// <summary>
        /// Implicit conversion from pure Eff
        /// </summary>
        public static implicit operator Aff<A>(Eff<A> ma) =>
            EffectMaybe(() => ma.Run().AsValueTask());
    }
}

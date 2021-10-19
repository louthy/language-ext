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
}

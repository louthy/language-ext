using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;

namespace LanguageExt.Thunks
{
    /// <summary>
    /// Lazily evaluates an asynchronous function and then memoizes the value
    /// Runs at most once
    /// </summary>
    public class ThunkAsync<A>
    {
        internal readonly Func<ValueTask<Fin<A>>> fun;
        internal volatile int state;
        internal Error error;
        internal A value;

        /// <summary>
        /// Construct a lazy thunk
        /// </summary>
        [Pure, MethodImpl(Thunk.mops)]
        public static ThunkAsync<A> Lazy(Func<ValueTask<Fin<A>>> fun) =>
            new ThunkAsync<A>(fun);

        /// <summary>
        /// Construct a lazy ThunkAsync
        /// </summary>
        [Pure, MethodImpl(Thunk.mops)]
        public static ThunkAsync<A> Lazy(Func<ValueTask<A>> fun) =>
            new ThunkAsync<A>(async () => Fin<A>.Succ(await fun().ConfigureAwait(false)));

        /// <summary>
        /// Construct an error ThunkAsync
        /// </summary>
        [Pure, MethodImpl(Thunk.mops)]
        public static ThunkAsync<A> Fail(Error error) =>
            new ThunkAsync<A>(Thunk.IsFailed, error);

        /// <summary>
        /// Construct a success thunk
        /// </summary>
        [Pure, MethodImpl(Thunk.mops)]
        public static ThunkAsync<A> Success(A value) =>
            new ThunkAsync<A>(value);

        /// <summary>
        /// Construct a cancelled thunk
        /// </summary>
        [Pure, MethodImpl(Thunk.mops)]
        public static ThunkAsync<A> Cancelled() =>
            new ThunkAsync<A>(Thunk.IsCancelled, Errors.Cancelled);

        /// <summary>
        /// Success ctor
        /// </summary>
        [MethodImpl(Thunk.mops)]
        ThunkAsync(A value) =>
            (this.state, this.value) = (Thunk.IsSuccess, value);

        /// <summary>
        /// Failed / Cancelled constructor
        /// </summary>
        [MethodImpl(Thunk.mops)]
        ThunkAsync(int state, Error error) =>
            (this.state, this.error) = (state, error);

        /// <summary>
        /// Lazy constructor
        /// </summary>
        [MethodImpl(Thunk.mops)]
        ThunkAsync(Func<ValueTask<Fin<A>>> fun) =>
            this.fun = fun ?? throw new ArgumentNullException(nameof(value));
        
        /// <summary>
        /// Clone the thunk
        /// </summary>
        /// <remarks>For thunks that were created as pre-failed/pre-cancelled values (i.e. no delegate to run, just
        /// in a pure error state), then the clone will copy that state exactly.  For thunks that have been evaluated
        /// then a cloned thunk will reset the thunk to a non-evaluated state.  This also means any thunk that has been
        /// evaluated and failed would lose the failed status</remarks>
        /// <returns></returns>
        [Pure, MethodImpl(Thunk.mops)]
        public ThunkAsync<A> Clone() =>
            fun == null
                ? new ThunkAsync<A>(state, error)
                : new ThunkAsync<A>(fun);        

        /// <summary>
        /// Value accessor
        /// </summary>
        [Pure, MethodImpl(Thunk.mops)]
        public ValueTask<Fin<A>> Value() =>
            Eval();

        /// <summary>
        /// Value accessor (clearing the memoised value)
        /// </summary>
        [Pure, MethodImpl(Thunk.mops)]
        public ValueTask<Fin<A>> ReValue()
        {
            if (this.fun != null && (state & Thunk.HasEvaluated) != 0)
            {
                state = Thunk.NotEvaluated;
            }
            return Eval();
        }
        
        /// <summary>
        /// Functor map
        /// </summary>
        [Pure, MethodImpl(Thunk.mops)]
        public ThunkAsync<B> Map<B>(Func<A, B> f)
        {
            try
            {
                SpinWait sw = default;
                while (true)
                {
                    switch (state)
                    {
                        case Thunk.IsSuccess:
                            return ThunkAsync<B>.Success(f(value));

                        case Thunk.NotEvaluated:
                            return ThunkAsync<B>.Lazy(async () =>
                            {
                                var ev = await Eval().ConfigureAwait(false);
                                if (ev.IsSucc)
                                {
                                    return Fin<B>.Succ(f(ev.value));
                                }
                                else
                                {
                                    return ev.Cast<B>();
                                }
                            });

                        case Thunk.IsCancelled:
                            return ThunkAsync<B>.Cancelled();

                        case Thunk.IsFailed:
                            return ThunkAsync<B>.Fail(error);
                    }

                    sw.SpinOnce();
                }
            }
            catch (Exception e)
            {
                return ThunkAsync<B>.Fail(e);
            }
        }
        
        /// <summary>
        /// Functor map
        /// </summary>
        [Pure, MethodImpl(Thunk.mops)]
        public ThunkAsync<B> BiMap<B>(Func<A, B> Succ, Func<Error, Error> Fail)
        {
            try
            {
                SpinWait sw = default;
                while (true)
                {
                    switch (state)
                    {
                        case Thunk.IsSuccess:
                            return ThunkAsync<B>.Success(Succ(value));

                        case Thunk.NotEvaluated:
                            return ThunkAsync<B>.Lazy(async () =>
                            {
                                var ev = await Eval().ConfigureAwait(false);
                                if (ev.IsSucc)
                                {
                                    return Fin<B>.Succ(Succ(ev.value));
                                }
                                else
                                {
                                    return Fail(ev.Error);
                                }
                            });

                        case Thunk.IsCancelled:
                            return ThunkAsync<B>.Fail(Errors.Cancelled);

                        case Thunk.IsFailed:
                            return ThunkAsync<B>.Fail(Fail(error));
                    }

                    sw.SpinOnce();
                }
            }
            catch (Exception e)
            {
                return ThunkAsync<B>.Fail(Fail(e));
            }
        }
        
        /// <summary>
        /// Functor map
        /// </summary>
        [Pure]
        public ThunkAsync<B> MapAsync<B>(Func<A, ValueTask<B>> f)
        {
            try
            {
                SpinWait sw = default;
                while (true)
                {
                    switch (state)
                    {
                        case Thunk.IsSuccess:
                            return ThunkAsync<B>.Lazy(async () => await f(value).ConfigureAwait(false));

                        case Thunk.NotEvaluated:
                            return ThunkAsync<B>.Lazy(async () =>
                            {
                                var ev = await Eval().ConfigureAwait(false);
                                if (ev.IsSucc)
                                {
                                    return Fin<B>.Succ(await f(ev.value).ConfigureAwait(false));
                                }
                                else
                                {
                                    return ev.Cast<B>();
                                }
                            });

                        case Thunk.IsCancelled:
                            return ThunkAsync<B>.Cancelled();

                        case Thunk.IsFailed:
                            return ThunkAsync<B>.Fail(error);
                    }

                    sw.SpinOnce();
                }
            }
            catch (Exception e)
            {
                return ThunkAsync<B>.Fail(e);
            }
        }
                
        /// <summary>
        /// Functor map
        /// </summary>
        [Pure]
        public ThunkAsync<B> BiMapAsync<B>(Func<A, ValueTask<B>> Succ, Func<Error, ValueTask<Error>> Fail)
        {
            SpinWait sw = default;
            while (true)
            {
                switch (state)
                {
                    case Thunk.IsSuccess:
                        return ThunkAsync<B>.Lazy(async () => await Succ(value).ConfigureAwait(false));

                    case Thunk.NotEvaluated:
                        return ThunkAsync<B>.Lazy(async () =>
                        {
                            var ev = await Eval().ConfigureAwait(false);
                            if (ev.IsSucc)
                            {
                                return Fin<B>.Succ(await Succ(ev.value).ConfigureAwait(false));
                            }
                            else
                            {
                                return Fin<B>.Fail(await Fail(ev.Error).ConfigureAwait(false));
                            }
                        });

                    case Thunk.IsCancelled:
                        return ThunkAsync<B>.Lazy(async () => await Fail(Errors.Cancelled).ConfigureAwait(false));

                    case Thunk.IsFailed:
                        return ThunkAsync<B>.Lazy(async () => await Fail(error).ConfigureAwait(false));
                }

                sw.SpinOnce();
            }
        }

        /// <summary>
        /// Evaluates the lazy function if necessary, returns the result if it's previously been evaluated
        /// The thread goes into a spin-loop if more than one thread tries to access the lazy value at once.
        /// This is to protect against multiple evaluations.  This technique allows for a lock free access
        /// the vast majority of the time.
        /// </summary>
        [Pure, MethodImpl(Thunk.mops)]
        async ValueTask<Fin<A>> Eval()
        {
            SpinWait sw = default;
            while (true)
            {
                if (Interlocked.CompareExchange(ref state, Thunk.Evaluating, Thunk.NotEvaluated) == Thunk.NotEvaluated)
                {
                    try
                    {
                        var vt = fun();
                        var fa = await vt.ConfigureAwait(false);
                        if (fa.IsFail)
                        {
                            error = fa.Error;
                            state = Thunk.IsFailed; // state update must be last thing before return
                            return fa;
                        }
                        else
                        {
                            value = fa.Value;
                            state = Thunk.IsSuccess; // state update must be last thing before return
                            return fa;
                        }
                    }
                    catch (OperationCanceledException e)
                    {
                        state = Thunk.IsCancelled; // state update must be last thing before return
                        return Fin<A>.Fail(Error.New(e));
                    }
                    catch (Exception e)
                    {
                        error = e;
                        state = e.Message == Errors.CancelledText // state update must be last thing before return
                                    ? Thunk.IsCancelled
                                    : Thunk.IsFailed;
                        return Fin<A>.Fail(Error.New(e));
                    }
                }
                else
                {
                    sw.SpinOnce();

                    // Once we're here we should have a result from the eval thread and
                    // so we can use `value` to return
                    switch (state)
                    {
                        case Thunk.NotEvaluated: 
                        case Thunk.Evaluating: 
                            continue;
                        case Thunk.IsSuccess: 
                            return Fin<A>.Succ(value);
                        case Thunk.IsCancelled: 
                            return Fin<A>.Fail(Errors.Cancelled);
                        case Thunk.IsFailed:
                            return Fin<A>.Fail(error);
                        default:
                            throw new InvalidOperationException("should never happen");
                    }
                }
            }
        }
 
        [Pure, MethodImpl(Thunk.mops)]
        public override string ToString() =>
            state switch
            {
                Thunk.NotEvaluated => "Not evaluated",
                Thunk.Evaluating => "Evaluating",
                Thunk.IsSuccess => $"Success({value})",
                Thunk.IsCancelled => $"Cancelled",
                Thunk.IsFailed => $"Failed({error})",
                _ => ""
            };
    }
}

using System;
using System.Threading;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static class AffRxExtensions
    {
        /// <summary>
        /// Consume an observable stream
        /// </summary>
        /// <remarks>Each item is passed to the `next` function.  When the stream completes the Aff returns
        /// which allows subsequent operations to be composed.</remarks>
        /// <param name="ma">Observable to consume</param>
        /// <param name="next">Next function to call</param>
        /// <param name="token">Cancellation token</param>
        /// <typeparam name="A">Bound type</typeparam>
        /// <returns>Aff of unit</returns>
        public static Aff<Unit> Consume<A>(this IObservable<A> ma, Func<A, Aff<Unit>> next, CancellationToken token = default) =>
            AffMaybe<Unit>(
                async () =>
                {
                    A   nextValue = default;
                    var wait      = new AutoResetEvent(false);

                    using var disp = ma.Subscribe(
                        (A x) =>
                        {
                            nextValue = x;
                            wait.Set();
                        });

                    while (token == default || !token.IsCancellationRequested)
                    {
                        wait.WaitOne();
                        var res = await next(nextValue).Run();
                        if (res.IsFail) return FinFail<Unit>((Error)res);
                    }

                    return unit;
                });

        /// <summary>
        /// Consume an observable stream
        /// </summary>
        /// <remarks>Each item is passed to the `next` function.  When the stream completes the Aff returns
        /// which allows subsequent operations to be composed.</remarks>
        /// <param name="ma">Observable to consume</param>
        /// <param name="next">Next function to call</param>
        /// <typeparam name="A">Bound type</typeparam>
        /// <returns>Aff of unit</returns>
        public static Aff<RT, Unit> Consume<RT, A>(this IObservable<A> ma, Func<A, Aff<RT, Unit>> next)
            where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, Unit>(
                async env =>
                {
                    A   nextValue = default;
                    var wait      = new AutoResetEvent(false);

                    using var disp = ma.Subscribe(
                        (A x) =>
                        {
                            nextValue = x;
                            wait.Set();
                        });

                    while (env.CancellationToken == default || !env.CancellationToken.IsCancellationRequested)
                    {
                        wait.WaitOne();
                        var res = await next(nextValue).Run(env);
                        if (res.IsFail) return FinFail<Unit>((Error)res);
                    }

                    return unit;
                });
        
        /// <summary>
        /// Fold an observable stream
        /// </summary>
        /// <remarks>Each item is passed to the `next` function, with a state value.  The state is aggregated over the
        /// stream by passing the return value from `next` to the subsequent calls.  When the stream completes, the
        /// aggregate state is returned.
        /// </remarks>
        /// <param name="ma">Observable to fold</param>
        /// <param name="next">Next function to call</param>
        /// <typeparam name="A">Bound type</typeparam>
        /// <param name="token">Cancellation token</param>
        /// <returns>Aff of S</returns>
        public static Aff<S> Fold<S, A>(this IObservable<A> ma, S state, Func<S, A, Aff<S>> next, CancellationToken token = default) =>
            AffMaybe<S>(
                async () =>
                {
                    A   nextValue = default;
                    var wait      = new AutoResetEvent(false);

                    using var disp = ma.Subscribe(
                        (A x) =>
                        {
                            nextValue = x;
                            wait.Set();
                        });

                    while (token == default || !token.IsCancellationRequested)
                    {
                        wait.WaitOne();
                        var res = await next(state, nextValue).Run();
                        if (res.IsFail) return FinFail<S>((Error)res);
                        state = (S)res;
                    }

                    return state;
                });

        /// <summary>
        /// Fold an observable stream
        /// </summary>
        /// <remarks>Each item is passed to the `next` function, with a state value.  The state is aggregated over the
        /// stream by passing the return value from `next` to the subsequent calls.  When the stream completes, the
        /// aggregate state is returned.
        /// </remarks>
        /// <param name="ma">Observable to fold</param>
        /// <param name="next">Next function to call</param>
        /// <typeparam name="A">Bound type</typeparam>
        /// <returns>Aff of S</returns>
        public static Aff<RT, S> Fold<RT, S, A>(this IObservable<A> ma, S state, Func<S, A, Aff<RT, S>> next)
            where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, S>(
                async env =>
                {
                    A   nextValue = default;
                    var wait      = new AutoResetEvent(false);

                    using var disp = ma.Subscribe(
                        (A x) =>
                        {
                            nextValue = x;
                            wait.Set();
                        });

                    while (env.CancellationToken == default || !env.CancellationToken.IsCancellationRequested)
                    {
                        wait.WaitOne();
                        var res = await next(state, nextValue).Run(env);
                        if (res.IsFail) return FinFail<S>((Error)res);
                        state = (S)res;
                    }

                    return (S)state;
                });                
    }
}

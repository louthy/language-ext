using System;
using System.Collections.Concurrent;
using System.Threading;
using LanguageExt.Common;
using LanguageExt.Effects;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static class EffRxExtensions
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
    public static Eff<Unit> Consume<A>(
        this IObservable<A> ma,
        Func<A, Eff<Unit>> next) =>
        from t in cancelTokenEff
        from _ in Eff<Unit>.Lift(
            () =>
            {
                using var  wait      = new AutoResetEvent(false);
                var        items     = new ConcurrentQueue<A>();
                var        done      = false;
                Exception? exception = default;

                using var disp = ma.Subscribe(
                    onNext: x =>
                            {
                                items.Enqueue(x);
                                wait.Set();
                            },
                    onError: e =>
                             {
                                 exception = e;
                                 done = true;
                                 wait.Set();
                             },
                    onCompleted: () =>
                                 {
                                     done = true;
                                     wait.Set();
                                 });

                while (true)
                {
                    wait.WaitOne();
                    if (done)
                    {
                        return unit;
                    }

                    if (t.IsCancellationRequested)
                    {
                        return Errors.Cancelled;
                    }

                    if (exception != null)
                    {
                        return Error.New(exception);
                    }

                    while (items.TryDequeue(out var item))
                    {
                        var res = next(item).Run();
                        if (res.IsFail) return Fin.Fail<Unit>((Error)res);
                    }
                }
            })
        select unit;

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
    public static Eff<S> Fold<S, A>(this IObservable<A> ma, S initialState, Func<S, A, Eff<S>> next)
    {
        var state = Atom(initialState);
        return ma.Consume(a => next(state, a).Map(x => ignore(state.Swap(_ => x))))
                 .Map(_ => state.Value);
    }
}

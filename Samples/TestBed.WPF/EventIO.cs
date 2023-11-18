using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using LanguageExt;
using LanguageExt.Common;
using LanguageExt.Effects;
using LanguageExt.Effects.Traits;
using static LanguageExt.Prelude;

namespace TestBed.WPF;

public static class EventIO
{
    public static Func<RT, Unit> Listen<RT, E, A>(
        this IO<RT, E, A> @event, 
        Func<A, IO<RT, E, Unit>> listener)
        where RT : struct, HasIO<RT, E> =>
        rt => ignore(@event.Bind(listener).RunAsync(rt));
    
    /// <summary>
    /// Idea for how event handlers could be consumed
    ///
    /// It's pretty ugly, but it does wrap up the complexity so the consumer never has to worry.
    /// </summary>
    public static IO<MinimalRT, Error, MouseEventArgs> MouseMoveIO(this Window window)
    {
        return
            from rt    in runtime<MinimalRT, Error>()
            from queue in Pure(new ConcurrentQueue<MouseEventArgs>())
            from wait  in use(() => new AutoResetEvent(false))
            from hndlr in use(acquire: () => new MouseEventHandler((_, e) =>
                                        {
                                           queue.Enqueue(e);
                                           wait.Set();
                                        }), 
                              release: h => window.RemoveHandler(Mouse.MouseMoveEvent, h))
            from _     in lift(() => window.AddHandler(Mouse.MouseMoveEvent, hndlr, false)) 
            from xs    in many(Go(rt, queue, wait))
            select xs;

        IEnumerable<MouseEventArgs> Go(MinimalRT rt, ConcurrentQueue<MouseEventArgs> queue, AutoResetEvent wait)
        {
            while (!rt.CancellationToken.IsCancellationRequested)
            {
                wait.WaitOne();
                while (!queue.IsEmpty)
                {
                    if (queue.TryDequeue(out var e))
                    {
                        yield return e;
                    }
                }
            }
        }
    }
}

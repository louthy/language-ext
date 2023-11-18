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

public record WindowIO<RT, E>(RT Runtime)
    where RT : struct, HasIO<RT, E>
{
    /// <summary>
    /// Idea for how event handlers could be consumed
    ///
    /// It's pretty ugly, but it does wrap up the complexity so the consumer never has to worry.
    /// </summary>
    public IO<RT, E, MouseEventArgs> OnMouseMove(Window window) => 
        from rtime in runtime<RT, E>()
        from queue in Pure(new ConcurrentQueue<MouseEventArgs>())
        from waite in use(() => new AutoResetEvent(false))
        from hndlr in use(
            acquire: () => new MouseEventHandler((_, e) =>
            {
                queue.Enqueue(e);
                waite.Set();
            }),
            release: h => window.RemoveHandler(Mouse.MouseMoveEvent, h))
        from _ in lift(() => window.AddHandler(Mouse.MouseMoveEvent, hndlr, false))
        from xs in many(Stream(rtime, queue, waite))
        select xs;
    
    static IEnumerable<EVENT> Stream<EVENT>(RT rt, ConcurrentQueue<EVENT> queue, AutoResetEvent wait)
    {
        while (!rt.CancellationToken.IsCancellationRequested)
        {
            wait.WaitOne(100);
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

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LanguageExt;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;
using static LanguageExt.Prelude;


namespace TestBed.WPF;

public class WindowIO<RT, E> : Window
    where RT : struct, HasIO<RT, E>
{
    readonly RT Runtime;

    /// <summary>
    /// Constructor
    /// </summary>
    protected WindowIO(RT runtime) =>
        Runtime = runtime;
    
    /// <summary>
    /// Startup launch
    /// </summary>
    protected Unit onStart(IO<RT, E, Unit> operation) =>
        operation.Run(Runtime)
                 .IfLeft(e => Error.New(e?.ToString() ?? "there was an error").Throw());
    
    protected async Task<Either<E, A>> handle<A>(IO<RT, E, A> operation) =>
        await operation.RunAsync(Runtime);
        
    /// <summary>
    /// Helper IO for setting control text 
    /// </summary>
    protected static IO<RT, E, Unit> setContent(ContentControl control, string text) =>
        lift(action0: () => control.Content = text);
        
    /// <summary>
    /// Helper IO for setting control text 
    /// </summary>
    protected static IO<RT, E, Unit> setContent(TextBlock control, string text) =>
        lift(action0: () => control.Text = text);

    /// <summary>
    /// Get mouse position
    /// </summary>
    protected IO<RT, E, Point> getPosition(MouseEventArgs @event) =>
        lift(() => @event.GetPosition(this));
    
    /// <summary>
    /// Idea for how event handlers could be consumed
    ///
    /// It's pretty ugly, but it does wrap up the complexity so the consumer never has to worry.
    /// </summary>
    protected IO<RT, E, MouseEventArgs> onMouseMove => 
        from rtime in runtime<RT, E>()
        from queue in Pure(new ConcurrentQueue<MouseEventArgs>())
        from waite in use(() => new AutoResetEvent(false))
        from hndlr in use(
            acquire: () => new MouseEventHandler((_, e) =>
            {
                queue.Enqueue(e);
                waite.Set();
            }),
            release: h => RemoveHandler(Mouse.MouseMoveEvent, h))
        from _ in lift(() => AddHandler(Mouse.MouseMoveEvent, hndlr, false))
        from xs in many(stream(rtime, queue, waite))
        select xs;
    
    /// <summary>
    /// Async delay
    /// </summary>
    protected static IO<RT, E, Unit> waitFor(double ms) =>
        liftIO(async token => await Task.Delay(TimeSpan.FromMilliseconds(ms), token));

    
    static IEnumerable<EVENT> stream<EVENT>(RT rt, ConcurrentQueue<EVENT> queue, AutoResetEvent wait)
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

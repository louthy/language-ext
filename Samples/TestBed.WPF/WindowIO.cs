using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LanguageExt;
using LanguageExt.Pipes;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace TestBed.WPF;

/// <summary>
/// A window base-type that supports some common IO behaviours for use in
/// derived window-types. 
/// </summary>
public class WindowIO<RT> : Window
{
    readonly RT Runtime;
    readonly EnvIO EnvIO;

    /// <summary>
    /// Constructor
    /// </summary>
    protected WindowIO(RT runtime)
    {
        Runtime = runtime;
        EnvIO   = EnvIO.New();
    }
    
    /// <summary>
    /// Startup launch
    /// </summary>
    protected Unit onStart(Eff<RT, Unit> operation) =>
        operation.Run(Runtime, EnvIO)
                 .IfFail(e => e.Throw());

    /// <summary>
    /// Closes any forks created by the window
    /// </summary>
    protected override void OnClosed(EventArgs e)
    {
        EnvIO.Source.Cancel();
        base.OnClosed(e);
    }

    /// <summary>
    /// Turns an IO operation into a task that is run
    /// </summary>
    /// <remarks>
    /// Useful for wrapping IO event-handlers into a Task base event-handler
    /// </remarks>
    protected void handle<A>(Eff<RT, A> operation) =>
        operation.ForkIO().Run(Runtime, EnvIO).ThrowIfFail();
        
    /// <summary>
    /// Helper IO for setting control text 
    /// </summary>
    protected static Eff<RT, Unit> setContent(ContentControl control, string text) =>
        lift(action: () => control.Content = text);
        
    /// <summary>
    /// Helper IO for setting control text 
    /// </summary>
    protected static Eff<RT, Unit> setContent(TextBlock control, string text) =>
        lift(action: () => control.Text = text);

    /// <summary>
    /// Get mouse position
    /// </summary>
    protected Eff<RT, Point> getPosition(MouseEventArgs @event) =>
        lift(() => @event.GetPosition(this));

    protected Producer<MouseEventArgs, Eff<RT>, Unit> onMouseMove =>
        from rtime  in runtime<RT>()
        let queue = Proxy.Queue<Eff<RT>, MouseEventArgs>()
        from hndlr  in use<MouseEventHandler>(
                         acquire: () => (_, e) => queue.Enqueue(e),
                         release: h => RemoveHandler(Mouse.MouseMoveEvent, h))
        from _      in liftEff(() => AddHandler(Mouse.MouseMoveEvent, hndlr, false))
        from result in queue
        select result;
    
    /// <summary>
    /// Async delay
    /// </summary>
    protected static Eff<RT, Unit> waitFor(double ms) =>
        yieldIO(ms);
}

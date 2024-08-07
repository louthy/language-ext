using System;
using System.Windows;
using System.Windows.Input;
using LanguageExt;
using LanguageExt.Effects;
using LanguageExt.Pipes;
using static LanguageExt.Prelude;

namespace TestBed.WPF;

/// <summary>
/// Main application window
/// </summary>
public partial class MainWindow : WindowRT
{
    /// <summary>
    /// Mutable counter
    /// </summary>
    readonly Atom<int> count = Atom(0);
        
    /// <summary>
    /// Construct the window and register the events
    /// </summary>
    public MainWindow() : base(App.Runtime)
    {
        InitializeComponent();
        onStart(startup);
    }

    /// <summary>
    /// Register the window events
    /// </summary>
    Eff<MinRT, Unit> startup =>
        from _1 in tickIO.ForkIO().As()
        from _2 in (onMouseMove | Proxy.repeat(showMousePos)).RunEffect().ForkIO()
        select unit;
        
    /// <summary>
    /// Infinite loop that ticks every second
    /// </summary>
    Eff<MinRT, Unit> tickIO =>
        from _1 in modifyCount(x => x + 1)
        from _2 in postIO(setContent(CounterButton, $"{count}"))
        from _3 in waitFor(1)
        from _4 in tickIO           // tail(tickIO)
        select unit;

    Consumer<MouseEventArgs, Eff<MinRT>, Unit> showMousePos =>
        from e in Proxy.awaiting<MouseEventArgs>()
        from _ in postIO(from p in getPosition(e)
                         from x in setContent(CursorTextBoxX, $"X: {p.X:F0}")
                         from y in setContent(CursorTextBoxY, $"Y: {p.Y:F0}")
                         select unit)
        select unit;

    /// <summary>
    /// Standard button click-handler
    /// </summary>
    void ButtonOnClick(object? sender, RoutedEventArgs e) =>
        handle(buttonOnClickIO);

    /// <summary>
    /// Button handler in the IO monad
    /// </summary>
    Eff<MinRT, Unit> buttonOnClickIO =>
        from _1 in resetCount
        from _2 in postIO(setContent(CounterButton, $"{count}"))
        select unit;
        
    /// <summary>
    /// Set the count value
    /// </summary>
    Eff<MinRT, int> setCount(int value) =>
        modifyCount(_ => value);

    /// <summary>
    /// Reset the count value
    /// </summary>
    Eff<MinRT, int> resetCount =>
        modifyCount(_ => 0);

    /// <summary>
    /// Set the count value
    /// </summary>
    Eff<MinRT, int> modifyCount(Func<int, int> f) =>
        count.SwapEff(x => SuccessEff(f(x)));
}

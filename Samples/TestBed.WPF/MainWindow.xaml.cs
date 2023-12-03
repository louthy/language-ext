using System;
using System.Windows;
using System.Windows.Input;
using LanguageExt;
using LanguageExt.Common;
using LanguageExt.Effects;
using static LanguageExt.Prelude;

namespace TestBed.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : WindowRT
    {
        /// <summary>
        /// Mutable counter
        /// </summary>
        readonly AtomIO<MinRT, Error, int> count = new (0);
        
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
        IO<MinRT, Error, Unit> startup =>
            from _1 in fork(tickIO)
            from _2 in fork(onMouseMove.Bind(showMousePos))
            select unit;
        
        /// <summary>
        /// Infinite loop that ticks every second
        /// </summary>
        IO<MinRT, Error, Unit> tickIO =>
            from _1 in modifyCount(x => x + 1)
            from _2 in post(setContent(CounterButton, $"{count}"))
            from _3 in waitFor(1)
            from _4 in tail(tickIO)
            select unit;

        /// <summary>
        /// Update the mouse-pos on the view
        /// </summary>
        IO<MinRT, Error, Unit> showMousePos(MouseEventArgs e) =>
            post(from p in getPosition(e)
                 from x in setContent(CursorTextBoxX, $"X: {p.X:F0}")
                 from y in setContent(CursorTextBoxY, $"Y: {p.Y:F0}")
                 select unit);

        /// <summary>
        /// Standard button click-handler
        /// </summary>
        async void ButtonOnClick(object? sender, RoutedEventArgs e) =>
            await handle(buttonOnClickIO);

        /// <summary>
        /// Button handler in the IO monad
        /// </summary>
        IO<MinRT, Error, Unit> buttonOnClickIO =>
            from _1 in resetCount
            from _2 in post(setContent(CounterButton, $"{count}"))
            select unit;
        
        /// <summary>
        /// Set the count value
        /// </summary>
        IO<MinRT, Error, Unit> setCount(int value) =>
            modifyCount(_ => value);

        /// <summary>
        /// Reset the count value
        /// </summary>
        IO<MinRT, Error, Unit> resetCount =>
            modifyCount(_ => 0);

        /// <summary>
        /// Set the count value
        /// </summary>
        IO<MinRT, Error, Unit> modifyCount(Func<int, int> f) =>
            count.Swap(f);
    }
}

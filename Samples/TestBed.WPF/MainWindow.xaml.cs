using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LanguageExt;
using LanguageExt.Common;
using LanguageExt.Effects;
using static LanguageExt.Prelude;

namespace TestBed.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MinimalRT runtime;
        AtomIO<MinimalRT, Error, int> count = new (0);
        
        public MainWindow()
        {
            runtime = new MinimalRT();
            InitializeComponent();

            // Start the ticking...
            ignore(tickIO.RunAsync(runtime));
        }

        async void ButtonOnClick(object? sender, RoutedEventArgs e) =>
            ignore(await buttonOnClickIO.RunAsync(runtime));
        
        /// <summary>
        /// Infinite loop that ticks every second
        /// </summary>
        IO<MinimalRT, Error, Unit> tickIO =>
            from _1 in modifyCount(x => x + 1)
            from _2 in post(setButtonText(CounterButton, $"{count}"))
            from _3 in waitFor(1)
            from _4 in tail(tickIO)
            select unit;

        /// <summary>
        /// Button handler in the IO monad
        /// </summary>
        IO<MinimalRT, Error, Unit> buttonOnClickIO =>
            from _1 in resetCount
            from _2 in setButtonText(CounterButton, $"{count}")
            select unit;
        
        /// <summary>
        /// Helper IO for setting button text 
        /// </summary>
        static IO<MinimalRT, Error, Unit> setButtonText(Button button, string text) =>
            lift(action: () => button.Content = text);

        /// <summary>
        /// Set the count value
        /// </summary>
        IO<MinimalRT, Error, Unit> setCount(int value) =>
            modifyCount(_ => value);

        /// <summary>
        /// Reset the count value
        /// </summary>
        IO<MinimalRT, Error, Unit> resetCount =>
            modifyCount(_ => 0);

        /// <summary>
        /// Set the count value
        /// </summary>
        IO<MinimalRT, Error, Unit> modifyCount(Func<int, int> f) =>
            count.Swap(f);

        /// <summary>
        /// Async delay
        /// </summary>
        IO<MinimalRT, Error, Unit> waitFor(double ms) =>
            liftIO(async token => await Task.Delay(TimeSpan.FromMilliseconds(ms), token));
    }
}

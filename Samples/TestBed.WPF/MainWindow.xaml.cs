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
            
            var dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += DispatcherTick;
            dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            dispatcherTimer.Start();
        }

        async void DispatcherTick(object? sender, EventArgs e) =>
            await tickIO.RunAsync(runtime);
        
        async void ButtonOnClick(object? sender, RoutedEventArgs e) =>
            await buttonOnClickIO.RunAsync(runtime);
        
        /// <summary>
        /// Button handler in the IO monad
        /// </summary>
        IO<MinimalRT, Error, Unit> tickIO =>
            from _1 in modifyCount(x => x + 1)
            from _2 in setButtonText(CounterButton, $"{count}")
            select unit;

        /// <summary>
        /// Button handler in the IO monad
        /// </summary>
        IO<MinimalRT, Error, Unit> buttonOnClickIO =>
            from _1 in resetCount
            from _2 in setButtonText(CounterButton, $"{count}")
            select unit;
        
        /// <summary>
        /// Helper IO for setting button text safely
        /// </summary>
        static IO<MinimalRT, Error, Unit> setButtonText(Button button, string text) =>
            post<MinimalRT, Error, Unit>(lift(() =>
            {
                button.Content = text;
                return unit;
            }));

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
    }
}

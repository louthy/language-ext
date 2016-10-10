using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.Process;
using System.Reactive.Linq;
using System.Reactive.Concurrency;

namespace UnitsOfMeasureSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ProcessId update;
        ProcessId resolver;
        int processes = 0;
        const int max = 20;

        public MainWindow()
        {
            InitializeComponent();

            ProcessConfig.initialise();

            SpawnUpdate();
            SpawnResolver();
        }

        void SpawnResolver()
        {
            resolver = spawn<Map<ProcessId, BallState>, Tuple<ProcessId, BallState>>(
                "resolver",
                () => Map<ProcessId, BallState>(),
                Resolver.Inbox
            );
        }

        /// <summary>
        /// Spawn a ball process and observe its state
        /// </summary>
        void SpawnBall(int id)
        {
            var element = CreateBallUI();

            // Setup function that creates a random initial state for the
            // ball and passes on the bounds of the window
            Func<BallState> setup = () =>
                BallState.CreateRandom()
                         .With(
                            BoxWidth: box.ActualWidth,
                            BoxHeight: box.ActualHeight
                         );

            // Spawn the ball process
            kill(User()["ball-" + id]);
            var pid = spawn<BallState, BallMsg>("ball-" + id, setup, BallProcess.Inbox);

            // Subscribe the ball process to the update process's state
            var sub = observeState<DateTime>(update).Subscribe(par(TimeMsg.Tell, pid));

            // Subscribe to the state changes of the ball so we can update the view
            observeState<BallState>(pid)
                .ObserveOn(DispatcherScheduler.Current)
                .Subscribe(state =>
                {
                    element.Visibility = Visibility.Visible;
                    Canvas.SetLeft(element, state.X);
                    Canvas.SetTop(element, box.ActualHeight - state.Y);
                },
                () =>
                {
                    box.Children.Remove(element);
                    sub.Dispose();
                }
            );

            // The resolver subscribes to the state changes of the ball
            observeState<BallState>(pid).Subscribe(state => tell(resolver, Tuple(pid, state)));
        }

        Ellipse CreateBallUI()
        {
            var element = new Ellipse();
            Canvas.SetTop(element, 0);
            Canvas.SetLeft(element, 0);
            element.Width = 20;
            element.Height = 20;
            element.StrokeThickness = 1;
            element.Stroke = Brushes.Black;
            element.Fill = Brushes.Red;
            element.Visibility = Visibility.Hidden;

            box.Children.Add(element);
            return element;
        }

        /// <summary>
        /// Spawn an update process that sends a time message to the ball
        /// process (via state subscription) every 60th of a second
        /// </summary>
        void SpawnUpdate()
        {
            update = spawn<DateTime, DateTime>("update", 
                () => DateTime.Now, 
                (state,time) => {
                    tellSelf(DateTime.Now, 1 * second / 60);
                    return time;
                });

            // Go!
            tell(update, DateTime.Now);
        }

        void ResetScene(object sender, MouseButtonEventArgs e)
        {
            processes = (processes + 1) % max;
            SpawnBall(processes);
        }
    }
}

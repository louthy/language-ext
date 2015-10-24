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

namespace UnitsOfMeasureSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer dispatcherTimer = new DispatcherTimer();

        double x = 10;
        double y = 300;
        double ballSize = 20.0;
        double surfaceImpulse = 0.6;

        DateTime lastTime = DateTime.Now;
        Velocity velX     = -70*m/s;
        Velocity velY     = 0*m/s;
        Accel g           = -9.8*m/s/s;
        Accel wind        = 0.1*m/s/s;

        public MainWindow()
        {
            InitializeComponent();

            dispatcherTimer.Tick += UpdateScene;
            dispatcherTimer.Interval = TimeSpan.FromSeconds(1.0 / 60.0);
            dispatcherTimer.Start();
        }

        private void ResetScene(object sender, MouseButtonEventArgs e)
        {
            x = random(300);
            y = random(300);
            velX = random(70)*m/s;
            velY = random(70)*m/s;
        }

        private void UpdateScene(object sender, EventArgs e)
        {
            var now = DateTime.Now;

            // Delta time
            Time dt = now - lastTime;
            lastTime = now;

            // Velocity = Velocity + Accel * Time
            velY = velY + g * dt;
            velX = velX + wind * NegativeScalar(velX * s/m) * dt;

            // Turn the velocities into deltas
            var dy = velY * s/m;
            var dx = velX * s/m;

            // Move
            y += dy;
            x += dx;

            // Bounce off the walls
            CollisionDetection();

            Canvas.SetLeft(circle, x);
            Canvas.SetTop(circle, box.ActualHeight - y);
        }

        private void CollisionDetection()
        {
            // Ground collision
            if (y < ballSize)
            {
                y = ballSize;
                velY = velY * -surfaceImpulse;
                velX = velX * surfaceImpulse;
            }

            // Ceiling collision
            if (y > box.ActualHeight)
            {
                y = box.ActualHeight;
                velY = velY * -surfaceImpulse;
                velX = velX * surfaceImpulse;
            }

            // Left wall collision
            if (x < 0)
            {
                x = 0;
                velX = velX * -surfaceImpulse;
            }

            // Right wall collision
            if (x > (box.ActualWidth - ballSize))
            {
                x = box.ActualWidth - ballSize;
                velX = velX * -surfaceImpulse;
            }
        }

        private static double NegativeScalar(double x) =>
            x < 0.0
                ? 1.0
                : -1.0;
    }
}

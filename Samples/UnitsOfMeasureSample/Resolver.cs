using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LanguageExt;
using LanguageExt.UnitsOfMeasure;
using static LanguageExt.Prelude;

namespace UnitsOfMeasureSample
{
    /// <summary>
    /// Resolver - makes the objects bounce off each other
    /// Not super efficient or particularly effective, but OK for a demo
    /// TODO: This effectively ignores the units-of-measure stuff, ideally need Position2, Velocity2, etc.
    /// </summary>
    public static class Resolver
    {
        public static Map<ProcessId, BallState> Inbox(Map<ProcessId, BallState> state, Tuple<ProcessId, BallState> msg)
        {
            var last = match(state, msg.Item1,
                Some: x => x,
                None: () => msg.Item2
                );

            var pidB = msg.Item1;
            var stateB = msg.Item2;

            foreach (var item in state)
            {
                stateB = Detect(item.Key, item.Value, pidB, stateB, last);
            }
            return state.AddOrUpdate(pidB, stateB);
        }

        static BallState Detect(ProcessId pidA, BallState stateA, ProcessId pidB, BallState stateB, BallState lastStateB)
        {
            if (pidA == pidB) return stateB;

            var a = new Point2(stateA.X, stateA.Y);
            var b = new Point2(stateB.X, stateB.Y);

            var avel = new Vector2(stateA.VelX.MetresPerSecond, stateA.VelY.MetresPerSecond);
            var bvel = new Vector2(stateB.VelX.MetresPerSecond, stateB.VelY.MetresPerSecond);

            Vector2 collision = a - b;
            double distance = collision.Length;
            if (distance == 0.0)
            {              
                // hack to avoid div by zero
                collision = new Vector2(1.0, 0.0);
                distance = 1.0;
            }
            if (distance > stateA.BallSize && distance > stateB.BallSize)
            {
                return stateB;
            }

            // Get the components of the velocity vectors which are parallel to the collision.
            // The perpendicular component remains the same for both fish
            collision = collision / distance;
            double aci = Vector2.Dot(avel, collision);
            double bci = Vector2.Dot(bvel, collision);

            // Solve for the new velocities using the 1-dimensional elastic collision equations.
            // Turns out it's really simple when the masses are the same.
            double acf = bci;
            double bcf = aci;

            // Replace the collision velocity components with the new ones
            var navel = avel + collision * (acf - aci);
            var nbvel = bvel + collision * (bcf - bci);

            SetPosAndVelMsg.Tell(pidA, a.X, a.Y, navel.DX*m/s, navel.DY*m/s);
            SetPosAndVelMsg.Tell(pidB, lastStateB.X, lastStateB.Y, nbvel.DX*m/s, nbvel.DY*m/s);

            return lastStateB;
        }

        public class Point2
        {
            public readonly double X;
            public readonly double Y;

            public Point2(double x, double y)
            {
                X = x;
                Y = y;
            }

            public static Vector2 operator -(Point2 a, Point2 b) =>
                new Vector2(b.X - a.X, b.Y - a.Y);
        }

        public class Vector2
        {
            public readonly double DX;
            public readonly double DY;

            public Vector2(double dx, double dy)
            {
                DX = dx;
                DY = dy;
            }

            public double Length => 
                Math.Sqrt(DX * DX + DY * DY);

            public static Vector2 operator /(Vector2 a, double b) =>
                new Vector2(a.DX/b, a.DY/b);

            public static Vector2 operator *(Vector2 a, double b) =>
                new Vector2(a.DX*b, a.DY*b);

            public static Vector2 operator +(Vector2 a, Vector2 b) =>
                new Vector2(a.DX + b.DX, a.DY + b.DY);

            public static double Dot(Vector2 a, Vector2 b) =>
                a.DX * b.DX + a.DY * b.DY;
        }
    }
}

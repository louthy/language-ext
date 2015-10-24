using System;

using LanguageExt;
using static LanguageExt.Prelude;

namespace UnitsOfMeasureSample
{
    public class BallState
    {
        public readonly double X;
        public readonly double Y;
        public readonly DateTime Time;
        public readonly Velocity VelX;
        public readonly Velocity VelY;
        public readonly double BallSize;
        public readonly double BoxWidth;
        public readonly double BoxHeight;
        public readonly double SurfaceImpulse;

        public BallState(
            double x,
            double y,
            DateTime time,
            Velocity velX,
            Velocity velY,
            double ballSize,
            double boxWidth,
            double boxHeight,
            double surfaceImpulse
        )
        {
            X = x;
            Y = y;
            Time = time;
            VelX = velX;
            VelY = velY;
            BallSize = ballSize;
            BoxWidth = boxWidth;
            BoxHeight = boxHeight;
            SurfaceImpulse = surfaceImpulse;
        }

        /// <summary>
        /// Named argument partial update
        /// </summary>
        public BallState With(
            double? X = null,
            double? Y = null,
            DateTime? Time = null,
            Velocity? VelX = null,
            Velocity? VelY = null,
            double? BallSize = null,
            double? BoxWidth = null,
            double? BoxHeight = null,
            double? SurfaceImpulse = null
        )
        {
            return new BallState(
                X              ?? this.X,
                Y              ?? this.Y,
                Time           ?? this.Time,
                VelX           ?? this.VelX,
                VelY           ?? this.VelY,
                BallSize       ?? this.BallSize,
                BoxWidth       ?? this.BoxWidth,
                BoxHeight      ?? this.BoxHeight,
                SurfaceImpulse ?? this.SurfaceImpulse
            );
        }

        public static BallState Create() =>
            new BallState(
                x:              10,
                y:              300,
                time:           DateTime.Now,
                velX:           -70*m/s,
                velY:           0*m/s,
                ballSize:       20.0,
                boxWidth:       300.0,
                boxHeight:      300.0,
                surfaceImpulse: 0.6
            );

        public static BallState CreateRandom() =>
            Create().With(
                X:    random(300),
                Y:    random(300),
                VelX: random(100)*m/s,
                VelY: random(30)*m/s
            );
    }
}

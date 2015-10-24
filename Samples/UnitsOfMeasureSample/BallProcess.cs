using System;

using LanguageExt;
using static LanguageExt.Prelude;

namespace UnitsOfMeasureSample
{
    public static class BallProcess
    {
        static readonly Accel gravity = -9.8*m/s/s;
        static readonly Accel wind = 0.1*m/s/s;

        /// <summary>
        /// Ball inbox
        /// Receives a DateTime stamp which is used to work out the amount
        /// to move the ball.  The state contains the current position and
        /// velocity of the ball, the last time is was processed and various
        /// ball constants.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="now"></param>
        /// <returns></returns>
        public static BallState Inbox(BallState state, DateTime now) =>
            compose(
                par(Accelerate, now), 
                Move,
                FloorCeilingDetection,
                WallsDetection,
                par(SetTime, now)
            )(state);

        /// <summary>
        /// Calculate the new velocity based on the time-step from the last frame
        /// and the acceleration factors of wind and gravity.
        /// 
        ///     Unit of measure types involved:
        /// 
        ///         Accel: wind and gravity
        ///         Velocity: VelX and VelY
        ///         Time: DeltaTime
        ///         double: NegativeScalar
        /// 
        /// </summary>
        static readonly Func<DateTime,BallState,BallState> Accelerate = (now, state) => 
            state.With(
                VelX: state.VelX + wind * NegativeScalar(state.VelX * s/m) * DeltaTime(state, now),
                VelY: state.VelY + gravity * DeltaTime(state, now));

        /// <summary>
        /// Finds the new position based on the current velocity.  The velocity
        /// is converted from Velocity to double by multiplying by second/metre
        /// </summary>
        static readonly Func<BallState, BallState> Move = state =>
            state.With(
                X: state.X + state.VelX * s/m,
                Y: state.Y + state.VelY * s/m);

        /// <summary>
        /// Find the relative time-step
        /// </summary>
        static Time DeltaTime(BallState state, DateTime now) =>
            now - state.Time;

        /// <summary>
        /// Set the new absolute time
        /// </summary>
        static readonly Func<DateTime, BallState, BallState> SetTime = (now, state) =>
            state.With(Time: now);

        /// <summary>
        /// Tests if the ball has passed one of the walls, and if so provides
        /// a negative horizontal impulse on the ball
        /// </summary>
        static Func<BallState, BallState> WallsDetection => state =>
            HasCollidedWithLeftWall(state)
                ? ImpulseLeft(state)
                : HasCollidedWithRightWall(state)
                    ? ImpulseRight(state)
                    : state;

        /// <summary>
        /// Tests if the ball has passed the floor or ceiling, and if so provides
        /// a negative vertical impulse on the ball
        /// </summary>
        static Func<BallState, BallState> FloorCeilingDetection => state =>
            HasCollidedWithGround(state)
                ? ImpulseGround(state)
                : HasCollidedWithCeiling(state)
                    ? ImpulseCeiling(state)
                    : state;

        static bool HasCollidedWithGround(BallState state) =>
            state.Y < state.BallSize;

        static bool HasCollidedWithCeiling(BallState state) =>
            state.Y > state.BoxHeight;

        static bool HasCollidedWithLeftWall(BallState state) =>
            state.X < 0;

        static bool HasCollidedWithRightWall(BallState state) =>
            state.X > (state.BoxWidth - state.BallSize);

        static BallState ImpulseLeft(BallState state) => 
            state.With(
                X:    0, 
                VelX: state.VelX * -state.SurfaceImpulse);

        static BallState ImpulseRight(BallState state) => 
            state.With(
                X:    state.BoxWidth - state.BallSize, 
                VelX: state.VelX * -state.SurfaceImpulse);

        static BallState ImpulseGround(BallState state) => 
            state.With(
                Y:    state.BallSize,
                VelX: state.VelX * state.SurfaceImpulse,
                VelY: state.VelY * -state.SurfaceImpulse);

        static BallState ImpulseCeiling(BallState state) =>
            state.With(
                Y:    state.BoxHeight,
                VelX: state.VelX * state.SurfaceImpulse,
                VelY: state.VelY * -state.SurfaceImpulse);

        private static double NegativeScalar(double x) =>
            x < 0.0
                ? 1.0
                : -1.0;
    }
}

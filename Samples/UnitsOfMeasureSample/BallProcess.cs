using System;
using LanguageExt;
using LanguageExt.UnitsOfMeasure;
using static LanguageExt.Prelude;

namespace UnitsOfMeasureSample
{
    /// <summary>
    /// Ball process.  Handles the state and time-updates of a ball element.
    /// 
    /// ** Bit of fun/overkill attempt at making this Process class as functional as possible! ** 
    /// 
    /// The messageMap provides a mapping from the message-type to an inbox function.  The inbox
    /// functions take a BallMsg and then return a new function that takes a BallState.  This
    /// allows for the composition of the state manipulation functions you see in TimeInbox.
    /// 
    /// The result is a Process that is a single pure expression.  
    /// </summary>
    public static class BallProcess
    {
        static Accel gravity => -9.8*m/s/s;
        static Accel wind    => 0.1*m/s/s;

        /// <summary>
        /// Message map
        /// </summary>
        static Map<BallMsgType, Func<BallMsg, Func<BallState, BallState>>> messageMap => Map(
            Tuple(BallMsgType.Time,         TimeInbox),
            Tuple(BallMsgType.SetPosAndVel, SetPosAndVelInbox)
        );

        /// <summary>
        /// Ball inbox
        /// This is the entry point for messages sent to a ball.  It looks for the message-type
        /// in the message-map; if found it invokes the message-inbox with the message and state.
        /// </summary>
        public static BallState Inbox(BallState state, BallMsg msg) =>
            match(messageMap, msg.Tag, 
                Some: fn => fn(msg)(state), 
                None: () => state
                );

        /// <summary>
        /// Receives a DateTime stamp which is used to work out the amount
        /// to move the ball.  The state contains the current position and
        /// velocity of the ball, the last time is was processed and various
        /// ball constants.
        /// </summary>
        static Func<BallMsg, Func<BallState, BallState>> TimeInbox => msg =>
            compose(
                Accelerate(msg.Time),
                Move,
                FloorCeilingDetection,
                WallsDetection,
                SetTime(msg.Time));

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
        static Func<DateTime,Func<BallState,BallState>> Accelerate => now => state => 
            state.With(
                VelX: state.VelX + wind * NegativeScalar(state.VelX * s/m) * DeltaTime(now, state),
                VelY: state.VelY + gravity * DeltaTime(now, state));

        /// <summary>
        /// Finds the new position based on the current velocity.  The velocity
        /// is converted from Velocity to double by multiplying by second/metre
        /// </summary>
        static Func<BallState, BallState> Move => state =>
            state.With(
                X: state.X + state.VelX * s/m,
                Y: state.Y + state.VelY * s/m);

        /// <summary>
        /// Receives a new position and velocity and updates the state with 
        /// those values
        /// </summary>
        static Func<BallMsg, Func<BallState, BallState>> SetPosAndVelInbox => msg =>
            map(msg as SetPosAndVelMsg, 
                vp => 
                    compose(
                        SetPosAndVel(vp.X, vp.Y, vp.VelX, vp.VelY),
                        TimeInbox(msg)));

        /// <summary>
        /// Updates the state with new position and velocity
        /// </summary>
        static Func<BallState, BallState> SetPosAndVel(double x, double y, Velocity vx, Velocity vy) => state =>
            state.With(X: x, Y: y, VelX: vx, VelY: vy);

        /// <summary>
        /// Find the relative time-step
        /// </summary>
        static Time DeltaTime(DateTime now, BallState state) =>
            now - state.Time;

        /// <summary>
        /// Set the new absolute time
        /// </summary>
        static Func<DateTime, Func<BallState, BallState>> SetTime => now => state =>
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

        static double NegativeScalar(double x) =>
            x < 0.0
                ? 1.0
                : -1.0;
    }
}

using System;
using LanguageExt;
using LanguageExt.UnitsOfMeasure;
using static LanguageExt.Prelude;
using static LanguageExt.Process;

namespace UnitsOfMeasureSample
{
    /// <summary>
    /// Ball process message type
    /// </summary>
    public enum BallMsgType
    {
        Time,
        SetPosAndVel
    }

    /// <summary>
    /// Ball process message
    /// </summary>
    public abstract class BallMsg
    {
        public readonly DateTime Time;
        public abstract BallMsgType Tag { get; }

        public BallMsg(DateTime time)
        {
            Time = time;
        }
    }

    /// <summary>
    /// Time update message
    /// Indicates to a ball process the new time (so it can update
    /// its position based on its velocity and acceleration)
    /// </summary>
    public class TimeMsg : BallMsg
    {
        public override BallMsgType Tag => BallMsgType.Time;

        public TimeMsg(DateTime time) : base(time)
        {
        }

        /// <summary>
        /// Tell function
        /// </summary>
        /// <remarks>
        /// Declared as an action so partial-application works without specifying
        /// type arguments
        /// </remarks>
        public static Action<ProcessId,DateTime> Tell => 
            (pid, time) => 
                tell(pid, new TimeMsg(time));
    }

    /// <summary>
    /// Set position and velocity
    /// Allows an external actor to update the ball position (useful if any
    /// future ball-to-ball collision detection and impulse behaviour is 
    /// added).
    /// </summary>
    public class SetPosAndVelMsg : BallMsg
    {
        public override BallMsgType Tag => BallMsgType.SetPosAndVel;

        public readonly double X;
        public readonly double Y;
        public readonly Velocity VelX;
        public readonly Velocity VelY;

        public SetPosAndVelMsg(double x, double y, Velocity vx, Velocity vy) : base(DateTime.Now)
        {
            X = x;
            Y = y;
            VelX = vx;
            VelY = vy;
        }

        /// <summary>
        /// Tell function
        /// </summary>
        /// <remarks>
        /// Declared as an action so partial-application works without specifying
        /// type arguments
        /// </remarks>
        public static Action<ProcessId, double, double, Velocity, Velocity> Tell => 
            (pid, x, y, vx, vy) =>
                tell(pid, new SetPosAndVelMsg(x,y,vx,vy));
    }
}

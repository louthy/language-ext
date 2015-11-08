using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static LanguageExt.Prelude;
using LanguageExt.UnitsOfMeasure;

namespace LanguageExt
{
    /// <summary>
    /// Directive type
    /// 'Discriminated union' identifier for the Directive sub-types.
    /// </summary>
    public enum DirectiveType
    {
        Resume,
        Restart,
        Stop,
        Escalate
    }

    /// <summary>
    /// Instruction to the Process system of what action to take when
    /// a Process crashes.  The directive is assigned to a supervisor
    /// who apply these actions to the failed Process and optionally
    /// other Processes defined by the Strategy.
    /// </summary>
    public abstract class Directive : IEquatable<Directive>
    {
        public readonly DirectiveType Type;

        protected Directive(DirectiveType type)
        {
            Type = type;
        }

        /// <summary>
        /// Re-call the setup function to reset the state after
        /// a crash.
        /// </summary>
        public static Directive Restart =>
            new Restart();

        /// <summary>
        /// Resume with state in tact after a crash
        /// </summary>
        public static readonly Directive Resume = 
            new Resume();

        /// <summary>
        /// Shutdown after a crash
        /// </summary>
        public static readonly Directive Stop =
            new Stop();

        /// <summary>
        /// Escalate the exception to the parent of the crashing
        /// process' supervisor after a crash
        /// </summary>
        public static readonly Directive Escalate =
            new Escalate();

        /// <summary>
        /// Equality test
        /// </summary>
        public virtual bool Equals(Directive other) =>
            Type == other.Type;

        /// <summary>
        /// Equality test
        /// </summary>
        public override bool Equals(object obj) =>
            obj is Directive
                ? Equals((Directive)obj)
                : false;

        /// <summary>
        /// Returns a unique hash-code for this Directive
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() =>
            Type.GetHashCode();

        /// <summary>
        /// Equality operator
        /// </summary>
        public static bool operator ==(Directive lhs, Directive rhs) =>
            lhs.Equals(rhs);

        /// <summary>
        /// Non-equality operator
        /// </summary>
        public static bool operator !=(Directive lhs, Directive rhs) =>
            !(lhs == rhs);
    }


    /// <summary>
    /// Directive: Resume with state in tact after a crash
    /// </summary>
    public class Resume : Directive
    {
        public Resume()
            :
            base(DirectiveType.Resume)
        {
        }
    }

    /// <summary>
    /// Directive: Shutdown after a crash
    /// </summary>
    public class Stop : Directive
    {
        public Stop()
            :
            base(DirectiveType.Stop)
        {
        }
    }

    /// <summary>
    /// Directive: Re-call the setup function to reset the state after
    /// a crash.
    /// </summary>
    public class Restart : Directive
    {
        public Restart()
            :
            base(DirectiveType.Restart)
        {
        }
    }

    /// <summary>
    /// Directive: Escalate the exception to the parent of the crashing
    /// process' supervisor after a crash
    /// </summary>
    public class Escalate : Directive
    {
        public Escalate()
            :
            base(DirectiveType.Escalate)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static LanguageExt.Prelude;
using LanguageExt.UnitsOfMeasure;

namespace LanguageExt
{
    public enum DirectiveType
    {
        Resume,
        Restart,
        Stop,
        Escalate
    }

    public abstract class Directive : IEquatable<Directive>
    {
        public readonly DirectiveType Type;

        protected Directive(DirectiveType type)
        {
            Type = type;
        }

        public static Directive Restart =>
            new Restart();

        public static readonly Directive Resume = 
            new Resume();

        public static readonly Directive Stop =
            new Stop();

        public static readonly Directive Escalate =
            new Escalate();

        public virtual bool Equals(Directive other) =>
            Type == other.Type;

        public override bool Equals(object obj) =>
            obj is Directive
                ? Equals((Directive)obj)
                : false;

        public override int GetHashCode() =>
            Type.GetHashCode();

        public static bool operator ==(Directive lhs, Directive rhs) =>
            lhs.Equals(rhs);

        public static bool operator !=(Directive lhs, Directive rhs) =>
            !(lhs == rhs);
    }

    public class Resume : Directive
    {
        public Resume()
            :
            base(DirectiveType.Resume)
        {
        }
    }

    public class Stop : Directive
    {
        public Stop()
            :
            base(DirectiveType.Stop)
        {
        }
    }

    public class Restart : Directive
    {
        public Restart()
            :
            base(DirectiveType.Restart)
        {
        }
    }

    public class Escalate : Directive
    {
        public Escalate()
            :
            base(DirectiveType.Escalate)
        {
        }
    }
}

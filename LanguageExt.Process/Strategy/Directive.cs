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

    public abstract class Directive
    {
        public readonly DirectiveType Type;

        public Directive(DirectiveType type)
        {
            Type = type;
        }

        public static Directive RestartNow =>
            new Restart(0*seconds);

        public static Directive Restart(Time when) =>
            new Restart(when);

        public static readonly Directive Resume = 
            new Resume();

        public static readonly Directive Stop =
            new Stop();

        public static readonly Directive Escalate =
            new Escalate();
    }

    internal class Resume : Directive
    {
        public Resume()
            :
            base(DirectiveType.Resume)
        {
        }
    }

    internal class Stop : Directive
    {
        public Stop()
            :
            base(DirectiveType.Stop)
        {
        }
    }

    internal class Restart : Directive
    {
        public readonly Time When;

        public Restart(Time when)
            :
            base(DirectiveType.Restart)
        {
            When = when;
        }
    }

    internal class Escalate : Directive
    {
        public Escalate()
            :
            base(DirectiveType.Escalate)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static LanguageExt.Prelude;
using LanguageExt.UnitsOfMeasure;
using LanguageExt.Trans;

namespace LanguageExt
{
    public abstract class XForOneStrategy : ProcessStrategy
    {
        public readonly int MaxRetries;
        public readonly Time Duration;

        public int Failures;
        public DateTime LastFailure;

        protected XForOneStrategy(int maxRetries, Time duration)
        {
            MaxRetries = maxRetries;
            Duration = duration;
        }

        private void Reset()
        {
            Failures = 0;
            LastFailure = DateTime.MinValue;
        }

        public override Directive HandleFailure(Exception ex, ProcessId pid)
        {
            var now = DateTime.Now;

            if ((now - LastFailure).TotalSeconds * seconds > Duration)
            {
                Failures = 0;
            }
            LastFailure = now;
            Failures++;

            if (MaxRetries != -1 && Failures > MaxRetries)
            {
                Reset();
                return Directive.Stop;
            }
            foreach (var match in Pipeline.Directives)
            {
                var res = match(ex);
                if (res.IsSome)
                {
                    return res.LiftUnsafe();
                }
            }
            return Directive.Stop;
        }
    }
}

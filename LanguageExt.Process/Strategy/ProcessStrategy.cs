using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static LanguageExt.Prelude;
using static LanguageExt.Process;
using LanguageExt.UnitsOfMeasure;

namespace LanguageExt
{
    /// <summary>
    /// Abstract class that handles process failure
    /// </summary>
    public abstract class ProcessStrategy
    {
        public readonly ProcessStrategyPipeline Pipeline;

        /// <summary>
        /// Default ctor
        /// </summary>
        public ProcessStrategy()
        {
            Pipeline = new ProcessStrategyPipeline(this);
        }

        /// <summary>
        /// Handler function for a Process thrown exception
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <returns>Directive to invoke to handle the failure</returns>
        public abstract Directive HandleFailure(Exception ex, ProcessId pid);

        /// <summary>
        /// Apply to children of parent
        /// True if the supervisor should apply a directive that was returned from a child
        /// failure handler to all children (All-for-one)
        /// </summary>
        public abstract bool ApplyToAllChildrenOfSupervisor
        {
            get;
        }

        /// <summary>
        /// Default strategy
        /// Restarts the process immediately
        /// </summary>
        public static ProcessStrategy Default =>
            new OneForOneStrategy(-1, 0*seconds).Pipeline.Match(otherwise(_ => Directive.RestartNow));

        /// <summary>
        /// One for one
        /// </summary>
        /// <param name="MaxRetries"></param>
        /// <param name="Duration"></param>
        /// <returns>ProcessStrategyPipeline - call With to provide exception handlers</returns>
        public static ProcessStrategyPipeline OneForOne(int MaxRetries = -1, Time Duration = default(Time)) =>
            new OneForOneStrategy(MaxRetries, Duration).Pipeline;

        /// <summary>
        /// All for one
        /// </summary>
        /// <param name="MaxRetries"></param>
        /// <param name="Duration"></param>
        /// <returns>ProcessStrategyPipeline - call With to provide exception handlers</returns>
        public static ProcessStrategyPipeline AllForOne(int MaxRetries = -1, Time Duration = default(Time)) =>
            new AllForOneStrategy(MaxRetries, Duration).Pipeline;
    }

    public class ProcessStrategyPipeline
    {
        public readonly ProcessStrategy Strategy;
        Stck<Func<Exception, Option<Directive>>> directives;

        internal ProcessStrategyPipeline(ProcessStrategy strategy)
        {
            directives = Stck<Func<Exception, Option<Directive>>>.Empty.Push(ex => Some(Directive.Stop));
        }

        public IEnumerable<Func<Exception, Option<Directive>>> Directives
        {
            get
            {
                return directives;
            }
        }

        /// <summary>
        /// Always perform the specified directive when a Process fails
        /// </summary>
        /// <param name="directive">Directive</param>
        /// <returns>Process strategy</returns>
        public ProcessStrategy Always(Directive directive)
        {
            directives = directives.Push(_ => directive);
            return Strategy;
        }

        /// <summary>
        /// Provide a set of exception matching behaviours that return a Directive if the
        /// exception type matches
        /// </summary>
        /// <param name="directives">Directives</param>
        /// <returns>Process strategy</returns>
        public ProcessStrategy Match(params Func<Exception, Option<Directive>>[] directives)
        {
            foreach (var dir in directives.Rev())
            {
                this.directives = this.directives.Push(dir);
            }
            return Strategy;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LanguageExt.Prelude;
using LanguageExt.UnitsOfMeasure;

namespace LanguageExt
{
    public partial class Process
    {
        /// <summary>
        /// Default Process strategy if one isn't provided on spawn
        /// </summary>
        public static readonly IProcessStrategy DefaultStrategy =
            OneForOne().Always(Directive.RestartNow);

        /// <summary>
        /// One for one - generates a process failure strategy that only affects the process that
        /// has failed.
        /// </summary>
        /// <param name="MaxRetries">Maximum number of retries in the period Duration before the 
        /// Process is stopped.  -1 means there is no maximum</param>
        /// <param name="Duration">The Duration that affects the MaxRetries parameter</param>
        /// <returns>ProcessStrategyPipeline - call With to provide exception handlers</returns>
        public static IProcessStrategyPipeline OneForOne(int MaxRetries = -1, Time Duration = default(Time)) =>
            new OneForOneStrategy(MaxRetries,Duration).Pipeline;

        /// <summary>
        /// One for one - generates a process failure strategy that affects all processes that are
        /// a child of the supervisor of the failed process (including the failed process itself).
        /// </summary>
        /// <param name="MaxRetries">Maximum number of retries in the period Duration before the 
        /// Process is stopped.  -1 means there is no maximum</param>
        /// <param name="Duration">The Duration that affects the MaxRetries parameter</param>
        /// <returns>ProcessStrategyPipeline - call With to provide exception handlers</returns>
        public static IProcessStrategyPipeline AllForOne(int MaxRetries = -1, Time Duration = default(Time)) =>
            new AllForOneStrategy(MaxRetries, Duration).Pipeline;
    }
}

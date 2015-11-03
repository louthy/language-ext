using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static LanguageExt.Prelude;
using LanguageExt.UnitsOfMeasure;

namespace LanguageExt
{
    public class AllForOneStrategy : FailuresWithinDurationStrategy
    {
        public AllForOneStrategy(int maxRetries, Time duration)
            : 
            base(maxRetries, duration)
        {
        }

        /// <summary>
        /// Returns the processes that are to be affected by the failure of
        /// the 'failedProcess' process.
        /// </summary>
        /// <param name="supervisor">Supervisor of the failed process</param>
        /// <param name="failedProcess">The process that has failed</param>
        /// <returns>Enumerable of processes to apply the directive to upon failure</returns>
        public override IEnumerable<ProcessId> Affects(ProcessId supervisor, ProcessId failedProcess, IEnumerable<ProcessId> supervisorChildren) =>
            supervisorChildren;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    public interface IProcessStrategy
    {
        /// <summary>
        /// Handler function for a Process thrown exception
        /// </summary>
        /// <param name="pid">Failed process ID </param>
        /// <param name="state">Failed process strategy state</param>
        /// <param name="ex">Exception</param>
        /// <returns>Directive to invoke to handle the failure and the possibly modified state</returns>
        Tuple<IProcessStrategyState, Directive> HandleFailure(ProcessId pid, IProcessStrategyState state, Exception ex);

        /// <summary>
        /// Generate a new strategy state-object for a Process
        /// </summary>
        /// <param name="pid">Process ID</param>
        /// <returns>IProcessStrategyState</returns>
        IProcessStrategyState NewState(ProcessId pid);

        /// <summary>
        /// Returns the processes that are to be affected by the failure of
        /// the 'failedProcess' process.  
        /// </summary>
        /// <remarks>
        /// 'failedProcess' itself will always be included regardless of what Affects returns
        /// </remarks>
        /// <param name="supervisor">Supervisor of the failed process</param>
        /// <param name="failedProcess">The process that has failed</param>
        /// <returns>Enumerable of processes to apply the directive to upon failure</returns>
        IEnumerable<ProcessId> Affects(ProcessId supervisor, ProcessId failedProcess, IEnumerable<ProcessId> supervisorChildren);
    }

    public interface IProcessStrategy<TState> : IProcessStrategy
        where TState : IProcessStrategyState
    {
    }
}

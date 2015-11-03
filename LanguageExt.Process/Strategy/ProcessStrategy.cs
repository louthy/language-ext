using System;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using LanguageExt.UnitsOfMeasure;

namespace LanguageExt
{
    /// <summary>
    /// Abstract class that handles process failure
    /// </summary>
    public abstract class ProcessStrategy : IProcessStrategy
    {
        internal readonly ProcessStrategyPipeline Pipeline;

        /// <summary>
        /// Default ctor
        /// </summary>
        protected ProcessStrategy()
        {
            Pipeline = new ProcessStrategyPipeline(this);
        }

        /// <summary>
        /// Handler function for a Process thrown exception
        /// </summary>
        /// <param name="pid">Failed process ID </param>
        /// <param name="state">Failed process strategy state</param>
        /// <param name="ex">Exception</param>
        /// <returns>Directive to invoke to handle the failure and the possibly modified state</returns>
        public abstract Tuple<object, Directive> HandleFailure(ProcessId pid, object state, Exception ex);

        /// <summary>
        /// Generate a new state object that will be used by the strategy
        /// This is usually used to keep track of retries, timeouts, etc.
        /// </summary>
        /// <param name="pid">ID of the Process that the state will be keeping 
        /// track of</param>
        /// <returns>IProcessStrategyState</returns>
        public abstract object NewState(ProcessId pid);

        /// <summary>
        /// Returns the processes that are to be affected by the failure of
        /// the 'failedProcess' process.
        /// </summary>
        /// <param name="supervisor">Supervisor of the failed process</param>
        /// <param name="failedProcess">The process that has failed</param>
        /// <returns>Enumerable of processes to apply the directive to upon failure</returns>
        public abstract IEnumerable<ProcessId> Affects(ProcessId supervisor, ProcessId failedProcess, IEnumerable<ProcessId> supervisorChildren);
    }
}

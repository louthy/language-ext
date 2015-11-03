using System;
using static LanguageExt.Prelude;
using LanguageExt.UnitsOfMeasure;

namespace LanguageExt
{
    public abstract class FailuresWithinDurationStrategy : ProcessStrategy
    {
        public readonly int MaxRetries;
        public readonly Time Duration;

        /// <summary>
        /// Protected ctor
        /// </summary>
        protected FailuresWithinDurationStrategy(int maxRetries, Time duration)
        {
            MaxRetries = maxRetries;
            Duration = duration;
        }

        /// <summary>
        /// Generate a new strategy state-object for a Process
        /// </summary>
        /// <param name="pid">Process ID</param>
        /// <returns>IProcessStrategyState</returns>
        public override object NewState(ProcessId pid) =>
            FailuresWithinDurationState.Empty;

        /// <summary>
        /// Handler function for a Process thrown exception
        /// </summary>
        /// <remarks>Maps the IProcessStrategyState to FailuresWithinDurationState and back</remarks>
        /// <param name="pid">Failed process ID </param>
        /// <param name="state">Failed process strategy state</param>
        /// <param name="ex">Exception</param>
        /// <returns>Directive to invoke to handle the failure and the possibly modified state</returns>
        public override Tuple<object, Directive> HandleFailure(ProcessId pid, object state, Exception ex) =>
            map(HandleFailure(pid, state as FailuresWithinDurationState, ex),(s,d) => 
                Tuple(s as object, d));

        /// <summary>
        /// Handler function for a Process thrown exception
        /// </summary>
        /// <param name="pid">Failed process ID </param>
        /// <param name="state">Failed process strategy state</param>
        /// <param name="ex">Exception</param>
        /// <returns>Directive to invoke to handle the failure and the possibly modified state</returns>
        Tuple<FailuresWithinDurationState, Directive> HandleFailure(ProcessId pid, FailuresWithinDurationState state, Exception ex) =>
            state.CheckExpired(Duration)
                 .CheckRetriesNotExceded(MaxRetries)
                 .MapSecond(dir => Pipeline.Directives
                    .FoldUntil(dir, (_, match) => match(ex), isSome)
                    .IfNone(Directive.Stop));
    }
}

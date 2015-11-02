using System;
using System.Collections.Generic;
using static LanguageExt.Prelude;
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
        protected ProcessStrategy()
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
            OneForOne().Always(Directive.RestartNow);

        /// <summary>
        /// One for one
        /// </summary>
        /// <param name="MaxRetries">Maximum number of retries in the period Duration before the 
        /// Process is stopped.  -1 means there is no maximum</param>
        /// <param name="Duration">The Duration that affects the MaxRetries parameter</param>
        /// <returns>ProcessStrategyPipeline - call With to provide exception handlers</returns>
        public static ProcessStrategyPipeline OneForOne(int MaxRetries = -1, Time Duration = default(Time)) =>
            new OneForOneStrategy(MaxRetries, Duration).Pipeline;

        /// <summary>
        /// All for one
        /// </summary>
        /// <param name="MaxRetries">Maximum number of retries in the period Duration before the 
        /// Process is stopped.  -1 means there is no maximum</param>
        /// <param name="Duration">The Duration that affects the MaxRetries parameter</param>
        /// <returns>ProcessStrategyPipeline - call With to provide exception handlers</returns>
        public static ProcessStrategyPipeline AllForOne(int MaxRetries = -1, Time Duration = default(Time)) =>
            new AllForOneStrategy(MaxRetries, Duration).Pipeline;
    }

    public class ProcessStrategyPipeline
    {
        readonly ProcessStrategy strategy;
        Lst<Func<Exception, Option<Directive>>> directives;

        internal ProcessStrategyPipeline(ProcessStrategy strategy)
        {
            this.strategy = strategy;
            directives = List.empty<Func<Exception, Option<Directive>>>();
        }

        public IEnumerable<Func<Exception, Option<Directive>>> Directives
        {
            get
            {
                return directives;
            }
        }

        internal void SetDirectives(Lst<Func<Exception, Option<Directive>>> directives)
        {
            this.directives = directives;
        }

        /// <summary>
        /// Always perform the specified directive when a Process fails
        /// </summary>
        /// <param name="directive">Directive</param>
        /// <returns>Process strategy</returns>
        public ProcessStrategy Always(Directive directive) => 
            Match().Otherwise(directive);

        /// <summary>
        /// Provide a set of exception matching behaviours that return a Directive if the
        /// exception type matches.  Use the With function to match against an exception of a 
        /// specific type and finish with Otherwise to get the PipelineStrategy object that
        /// can be assigned to an actor.
        /// </summary>
        public ProcessStrategyPipelineCtx Match() =>
            new ProcessStrategyPipelineCtx(strategy, this);
    }

    public class ProcessStrategyPipelineCtx
    {
        readonly ProcessStrategy strategy;
        readonly ProcessStrategyPipeline pipeline;
        readonly List<Func<Exception, Option<Directive>>> directives;

        internal ProcessStrategyPipelineCtx(ProcessStrategy strategy, ProcessStrategyPipeline pipeline)
        {
            this.strategy = strategy;
            this.pipeline = pipeline;
            this.directives = new List<Func<Exception, Option<Directive>>>();
        }

        /// <summary>
        /// Provides a mapping from TException to a Directive.  This mapping is
        /// invoked when a Process fails to get instruction to the supervisor
        /// of what to do to handle the failure.
        /// </summary>
        /// <typeparam name="TException">Exception to catch and map</typeparam>
        /// <param name="map">Mapping function</param>
        /// <returns>Type that allows for fluent construction of Exception->Directive matches.  Complete
        /// the match sequence and get a ProcessStrategy by calling Otherwise</returns>
        public ProcessStrategyPipelineCtx With<TException>(Func<TException, Directive> map) where TException : Exception
        {
            Func<Exception, Option<Directive>> func = input =>
                typeof(TException).IsAssignableFrom(input.GetType())
                    ? Some(map((TException)input))
                    : None;
            directives.Add(func);
            return this;
        }

        /// <summary>
        /// Provides a mapping from TException to a Directive.  This mapping is
        /// invoked when a Process fails to get instruction to the supervisor
        /// of what to do to handle the failure.
        /// </summary>
        /// <typeparam name="TException">Exception to catch and map</typeparam>
        /// <param name="directive">Directive to return when an exception of type TException is thrown by a Process</param>
        /// <returns>Type that allows for fluent construction of Exception->Directive matches.  Complete
        /// the match sequence and get a ProcessStrategy by calling Otherwise</returns>
        public ProcessStrategyPipelineCtx With<TException>(Directive directive) where TException : Exception =>
            With<TException>(_ => directive);

        /// <summary>
        /// Catch-all for any unmatched exceptions thrown by a Process
        /// </summary>
        /// <param name="map">Mapping function from Exception to a Directive</param>
        /// <returns>A ProcessStrategy that can be applied to a Process</returns>
        public ProcessStrategy Otherwise(Func<Exception, Directive> map)
        {
            // These defaults will be at the end of the match list
            // which allows users to override the beheviour (even if it's not
            // entirely wise to do so).
            With<ProcessKillException>(Directive.Stop);
            With<ProcessSetupException>(Directive.Stop);

            Func<Exception, Option<Directive>> func = input => Some(map(input));
            directives.Add(func);
            pipeline.SetDirectives(List.createRange(directives));
            return strategy;
        }

        /// <summary>
        /// Catch-all for any unmatched exceptions thrown by a Process
        /// </summary>
        /// <param name="directive">A directive to always apply when no others match</param>
        /// <returns>A ProcessStrategy that can be applied to a Process</returns>
        public ProcessStrategy Otherwise(Directive directive) =>
            Otherwise(_ => directive);
    }
}

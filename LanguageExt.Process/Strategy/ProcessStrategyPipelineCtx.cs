using System;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using LanguageExt.UnitsOfMeasure;

namespace LanguageExt
{
    public interface IProcessStrategyPipelineCtx
    {
        /// <summary>
        /// Provides a mapping from TException to a Directive.  This mapping is
        /// invoked when a Process fails to get instruction to the supervisor
        /// of what to do to handle the failure.
        /// </summary>
        /// <typeparam name="TException">Exception to catch and map</typeparam>
        /// <param name="map">Mapping function</param>
        /// <returns>Type that allows for fluent construction of Exception->Directive matches.  Complete
        /// the match sequence and get a ProcessStrategy by calling Otherwise</returns>
        IProcessStrategyPipelineCtx With<TException>(Func<TException, Directive> map) where TException : Exception;

        /// <summary>
        /// Provides a mapping from TException to a Directive.  This mapping is
        /// invoked when a Process fails to get instruction to the supervisor
        /// of what to do to handle the failure.
        /// </summary>
        /// <typeparam name="TException">Exception to catch and map</typeparam>
        /// <param name="directive">Directive to return when an exception of type TException is thrown by a Process</param>
        /// <returns>Type that allows for fluent construction of Exception->Directive matches.  Complete
        /// the match sequence and get a ProcessStrategy by calling Otherwise</returns>
        IProcessStrategyPipelineCtx With<TException>(Directive directive) where TException : Exception;

        /// <summary>
        /// Catch-all for any unmatched exceptions thrown by a Process
        /// </summary>
        /// <param name="map">Mapping function from Exception to a Directive</param>
        /// <returns>A ProcessStrategy that can be applied to a Process</returns>
        IProcessStrategy Otherwise(Func<Exception, Directive> map);

        /// <summary>
        /// Catch-all for any unmatched exceptions thrown by a Process
        /// </summary>
        /// <param name="directive">A directive to always apply when no others match</param>
        /// <returns>A ProcessStrategy that can be applied to a Process</returns>
        IProcessStrategy Otherwise(Directive directive);
    }

    class ProcessStrategyPipelineCtx : IProcessStrategyPipelineCtx
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
        public IProcessStrategyPipelineCtx With<TException>(Func<TException, Directive> map) where TException : Exception
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
        public IProcessStrategyPipelineCtx With<TException>(Directive directive) where TException : Exception =>
            With<TException>(_ => directive);

        /// <summary>
        /// Catch-all for any unmatched exceptions thrown by a Process
        /// </summary>
        /// <param name="map">Mapping function from Exception to a Directive</param>
        /// <returns>A ProcessStrategy that can be applied to a Process</returns>
        public IProcessStrategy Otherwise(Func<Exception, Directive> map)
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
        public IProcessStrategy Otherwise(Directive directive) =>
            Otherwise(_ => directive);
    }
}

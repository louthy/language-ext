using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    public interface IProcessStrategyPipeline
    {
        /// <summary>
        /// Always perform the specified directive when a Process fails
        /// </summary>
        /// <param name="directive">Directive</param>
        /// <returns>Process strategy</returns>
        IProcessStrategy Always(Directive directive);

        /// <summary>
        /// Provide a set of exception matching behaviours that return a Directive if the
        /// exception type matches.  Use the With function to match against an exception of a 
        /// specific type and finish with Otherwise to get the PipelineStrategy object that
        /// can be assigned to an actor.
        /// </summary>
        IProcessStrategyPipelineCtx Match();
    }

    class ProcessStrategyPipeline : IProcessStrategyPipeline
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
        public IProcessStrategy Always(Directive directive) =>
            Match().Otherwise(directive);

        /// <summary>
        /// Provide a set of exception matching behaviours that return a Directive if the
        /// exception type matches.  Use the With function to match against an exception of a 
        /// specific type and finish with Otherwise to get the PipelineStrategy object that
        /// can be assigned to an actor.
        /// </summary>
        public IProcessStrategyPipelineCtx Match() =>
            new ProcessStrategyPipelineCtx(strategy, this);
    }
}

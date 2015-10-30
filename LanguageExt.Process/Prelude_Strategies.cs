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
        /// One for one
        /// </summary>
        /// <param name="MaxRetries"></param>
        /// <param name="Duration"></param>
        /// <returns>ProcessStrategyPipeline - call With to provide exception handlers</returns>
        public static ProcessStrategyPipeline OneForOne(int MaxRetries = -1, Time Duration = default(Time)) =>
            ProcessStrategy.OneForOne(MaxRetries, Duration);

        /// <summary>
        /// All for one
        /// </summary>
        /// <param name="MaxRetries"></param>
        /// <param name="Duration"></param>
        /// <returns>ProcessStrategyPipeline - call With to provide exception handlers</returns>
        public static ProcessStrategyPipeline AllForOne(int MaxRetries = -1, Time Duration = default(Time)) =>
            ProcessStrategy.AllForOne(MaxRetries, Duration);

        /// <summary>
        /// Use with the declarative strategy functions to match to an exception and provide a 
        /// directive mapping
        /// </summary>
        public static Func<Exception, Option<Directive>> exception<T>(Func<T, Directive> map)
            where T : Exception =>
            input =>
                input.GetType() == typeof(T)
                    ? Some(map((T)input))
                    : None;

        /// <summary>
        /// Use with the declarative strategy functions to match to an exception and provide a 
        /// directive
        /// </summary>
        public static Func<Exception, Option<Directive>> exception<T>(Directive directive)
            where T : Exception =>
            exception<T>(_ => directive);

        /// <summary>
        /// Use with the declarative strategy functions to provide a default value
        /// </summary>
        public static Func<Exception, Option<Directive>> otherwise(Func<Exception, Directive> directive) => ex =>
            Some(directive(ex));

        /// <summary>
        /// Use with the declarative strategy functions to provide a default value
        /// </summary>
        public static Func<Exception, Option<Directive>> otherwise(Func<Directive> directive) => _ =>
            Some(directive());

        /// <summary>
        /// Use with the declarative strategy functions to provide a default value
        /// </summary>
        public static Func<Exception, Option<Directive>> otherwise<T>(Directive directive) => _ =>
            Some(directive);
    }
}

using LanguageExt.UnitsOfMeasure;
using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// 
    ///     Process
    /// 
    /// </summary>
    public static partial class Process
    {
#if !COREFX
        /// <summary>
        /// Configure the process system
        /// Ideally call this before spawning any processes, but it can be updated
        /// live if necessary.
        /// </summary>
        /// <param name="path">Path of the configuration file to load, if no path
        /// is provided then the system will look for 'process.conf' in the folder of 
        /// the entry assembly; if no file is found then it will check the parent
        /// folder.</param>
        /// <param name="additionalStrategyFunctions">
        /// Plugin extra strategy behaviours by passing in a list of FuncSpecs.
        /// 
        ///     i.e.
        ///     
        ///     FuncSpec.Attr("always", settings => Strategy.Always((Directive)settings["value"].Value), FieldSpec.Directive("value")),
        ///     
        ///     The first argument is the name of the function, the second takes 
        ///     a Map<string, ValueToken> and provides named arguments for your behaviour,
        ///     the remaining params are to specify the type and name of the arguments.
        ///     
        ///     A single argument function (like above) can be used in the config file thus:
        ///     
        ///         always: stop
        ///         
        ///     A multi-argument function requires you to specify the name of the arguments:
        ///     
        ///         retries: count = 2, duration = 10 seconds
        /// 
        ///     Multiple variants can be registered under the same name:
        ///    
        ///         FuncSpec.Attr(
        ///             "retries",
        ///             ArgumentsSpec.Variant(
        ///                 settings => Strategy.Retries((int)settings["count"].Value,(Time)settings["duration"].Value),
        ///                 FieldSpec.Int("count"),
        ///                 FieldSpec.Time("duration")),
        ///             
        ///             ArgumentsSpec.Variant(
        ///                 settings => Strategy.Retries((int)settings["count"].Value),
        ///                 FieldSpec.Int("count"))
        ///             ))
        /// </param>
        public static Unit readConfigFromFile(string path, IEnumerable<FuncSpec> additionalStrategyFunctions)
        {
            var mgr = new ProcessSystemConfig(additionalStrategyFunctions);
            mgr.LoadFromFile(path);
            ActorContext.SetConfig(mgr);
            return unit;
        }
#endif
        /// <summary>
        /// Configure the process system
        /// Ideally call this before spawning any processes, but it can be updated
        /// live if necessary.
        /// </summary>
        /// <param name="text">Configuration script</param>
        /// <param name="additionalStrategyFunctions">
        /// Plugin extra strategy behaviours by passing in a list of FuncSpecs.
        /// 
        ///     i.e.
        ///     
        ///     FuncSpec.Attr("always", settings => Strategy.Always((Directive)settings["value"].Value), FieldSpec.Directive("value")),
        ///     
        ///     The first argument is the name of the function, the second takes 
        ///     a Map<string, ValueToken> and provides named arguments for your behaviour,
        ///     the remaining params are to specify the type and name of the arguments.
        ///     
        ///     A single argument function (like above) can be used in the config file thus:
        ///     
        ///         always: stop
        ///         
        ///     A multi-argument function requires you to specify the name of the arguments:
        ///     
        ///         retries: count = 2, duration = 10 seconds
        /// 
        ///     Multiple variants can be registered under the same name:
        ///    
        ///         FuncSpec.Attr(
        ///             "retries",
        ///             ArgumentsSpec.Variant(
        ///                 settings => Strategy.Retries((int)settings["count"].Value,(Time)settings["duration"].Value),
        ///                 FieldSpec.Int("count"),
        ///                 FieldSpec.Time("duration")),
        ///             
        ///             ArgumentsSpec.Variant(
        ///                 settings => Strategy.Retries((int)settings["count"].Value),
        ///                 FieldSpec.Int("count"))
        ///             ))
        /// </param>
        public static Unit readConfigFromText(string text, IEnumerable<FuncSpec> additionalStrategyFunctions)
        {
            var mgr = new ProcessSystemConfig(additionalStrategyFunctions);
            mgr.LoadFromText(text);
            ActorContext.SetConfig(mgr);
            return unit;
        }

        /// <summary>
        /// Configure the process system from the cluster
        /// Ideally call this before spawning any processes, but it can be updated
        /// live if necessary.  ** Must be called after Cluster.connect() **
        /// </summary>
        /// <param name="additionalStrategyFunctions">
        /// Plugin extra strategy behaviours by passing in a list of FuncSpecs.
        /// 
        ///     i.e.
        ///     
        ///     FuncSpec.Attr("always", settings => Strategy.Always((Directive)settings["value"].Value), FieldSpec.Directive("value")),
        ///     
        ///     The first argument is the name of the function, the second takes 
        ///     a Map<string, ValueToken> and provides named arguments for your behaviour,
        ///     the remaining params are to specify the type and name of the arguments.
        ///     
        ///     A single argument function (like above) can be used in the config file thus:
        ///     
        ///         always: stop
        ///         
        ///     A multi-argument function requires you to specify the name of the arguments:
        ///     
        ///         retries: count = 2, duration = 10 seconds
        /// 
        ///     Multiple variants can be registered under the same name:
        ///    
        ///         FuncSpec.Attr(
        ///             "retries",
        ///             ArgumentsSpec.Variant(
        ///                 settings => Strategy.Retries((int)settings["count"].Value,(Time)settings["duration"].Value),
        ///                 FieldSpec.Int("count"),
        ///                 FieldSpec.Time("duration")),
        ///             
        ///             ArgumentsSpec.Variant(
        ///                 settings => Strategy.Retries((int)settings["count"].Value),
        ///                 FieldSpec.Int("count"))
        ///             ))
        /// </param>
        public static Unit readConfigFromCluster(IEnumerable<FuncSpec> additionalStrategyFunctions = null)
        {
            var mgr = new ProcessSystemConfig(additionalStrategyFunctions);
            mgr.LoadFromCluster();
            ActorContext.SetConfig(mgr);
            return unit;
        }

        /// <summary>
        /// Saves the current state of the settings to the cluster
        /// 
        /// NOTE: This will just save the original text that was 
        /// either loaded or passed to this class when it was initialised 
        /// (via configureFromCluster/configureFromText/configureFromFile).  
        /// 
        /// NOTE: The settings are saved for the role, and will be shared 
        /// between multiple nodes in the role.
        /// </summary>
        /// <returns></returns>
        public static Unit writeConfigToCluster() =>
            ActorContext.Config.SaveToCluser();

#if !COREFX
        /// <summary>
        /// Configure the process system
        /// Ideally call this before spawning any processes, but it can be updated
        /// live if necessary.
        /// </summary>
        /// <param name="path">Path of the configuration file to load, if no path
        /// is provided then the system will look for 'process.conf' in the folder of 
        /// the entry assembly; if no file is found then it will check the parent
        /// folder.</param>
        public static Unit readConfigFromFile(string path = null)
        {
            var mgr = new ProcessSystemConfig(null);
            mgr.LoadFromFile(path);
            ActorContext.SetConfig(mgr);
            return unit;
        }
#endif

        /// <summary>
        /// Configure the process system
        /// Ideally call this before spawning any processes, but it can be updated
        /// live if necessary.
        /// </summary>
        /// <param name="path">Path of the configuration file to load, if no path
        /// is provided then the system will look for 'process.conf' in the folder of 
        /// the entry assembly; if no file is found then it will check the parent
        /// folder.</param>
        public static Unit readConfigFromText(string text)
        {
            var mgr = new ProcessSystemConfig(null);
            mgr.LoadFromText(text);
            ActorContext.SetConfig(mgr);
            return unit;
        }
    }
}

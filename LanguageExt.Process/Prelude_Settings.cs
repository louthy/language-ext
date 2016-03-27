using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// 
    ///     Process: Settings
    /// 
    /// </summary>
    public static partial class Process
    {
        /// <summary>
        /// Access a setting 
        /// If in a Process message loop, then this accesses the configuration settings
        /// for the Process from the the configuration file, or stored in the cluster.
        /// If not in a Process message loop, then this accesses 'global' configuration
        /// settings.  
        /// </summary>
        /// <param name="name">Name of the setting</param>
        /// <param name="defaultValue">Default value to use if it doesn't exist</param>
        /// <param name="prop">If the setting is a complex value (like a map or record), then 
        /// this selects the property of the setting to access</param>
        /// <returns>Configuration setting value</returns>
        public static T setting<T>(string name, T defaultValue, string prop = "value") =>
            setting<T>(name, prop).IfNone(defaultValue);

        /// <summary>
        /// Access a setting 
        /// If in a Process message loop, then this accesses the configuration settings
        /// for the Process from the the configuration file, or stored in the cluster.
        /// If not in a Process message loop, then this accesses 'global' configuration
        /// settings.  
        /// </summary>
        /// <param name="name">Name of the setting</param>
        /// <param name="prop">If the setting is a complex value (like a map or record), then 
        /// this selects the property of the setting to access</param>
        /// <returns>Optional configuration setting value</returns>
        public static Option<T> setting<T>(string name, string prop = "value") =>
            InMessageLoop
                ? settingLocal<T>(name, prop)
                : settingGlobal<T>(name, prop);

        /// <summary>
        /// Access a setting's type
        /// If in a Process message loop, then this accesses the configuration settings
        /// for the Process from the the configuration file, or stored in the cluster.
        /// If not in a Process message loop, then this accesses 'global' configuration
        /// settings.  
        /// </summary>
        /// <param name="name">Name of the setting</param>
        /// <param name="defaultValue">Default value to use if it doesn't exist</param>
        /// <param name="prop">If the setting is a complex value (like a map or record), then 
        /// this selects the property of the setting to access</param>
        /// <returns>Type descriptor for the configuration setting</returns>
        public static Option<ArgumentType> settingType(string name, string prop = "value") =>
            InMessageLoop
                ? settingTypeLocal(name,prop)
                : settingTypeGlobal(name,prop);

        static Option<ArgumentType> settingTypeLocal(string name, string prop = "value") =>
            from token in ActorContext.Config.ProcessSettings.Find(ActorContext.Self)
            from setting in token.Settings.Find(name)
            from valtok in setting.Values.Find(prop)
            select valtok.Type;

        static Option<ArgumentType> settingTypeGlobal(string name, string prop = "value") =>
            from setting in ActorContext.Config.GlobalSettings.Find(name)
            from valtok in setting.Values.Find(prop)
            select valtok.Type;

        static Option<T> settingLocal<T>(string name, string prop = "value") =>
            from token in ActorContext.Config.ProcessSettings.Find(ActorContext.Self)
            from setting in token.Settings.Find(name)
            from valtok in setting.Values.Find(prop)
            select (T)valtok.Value;

        static Option<T> settingGlobal<T>(string name, string prop = "value") =>
            from setting in ActorContext.Config.GlobalSettings.Find(name)
            from valtok in setting.Values.Find(prop)
            select (T)valtok.Value;
    }
}

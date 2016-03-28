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
                ? settingProcess<T>(name, prop)
                : settingRole<T>(name, prop);

        /// <summary>
        /// Access a list setting 
        /// If in a Process message loop, then this accesses the configuration settings
        /// for the Process from the the configuration file, or stored in the cluster.
        /// If not in a Process message loop, then this accesses 'global' configuration
        /// settings.  
        /// </summary>
        /// <param name="name">Name of the setting</param>
        /// <param name="prop">If the setting is a complex value (like a map or record), then 
        /// this selects the property of the setting to access</param>
        /// <returns>Optional configuration setting value</returns>
        public static Lst<T> settingList<T>(string name, string prop = "value") =>
            setting(name, List.empty<T>(), prop);

        /// <summary>
        /// Access a map setting 
        /// If in a Process message loop, then this accesses the configuration settings
        /// for the Process from the the configuration file, or stored in the cluster.
        /// If not in a Process message loop, then this accesses 'global' configuration
        /// settings.  
        /// </summary>
        /// <param name="name">Name of the setting</param>
        /// <param name="prop">If the setting is a complex value (like a map or record), then 
        /// this selects the property of the setting to access</param>
        /// <returns>Optional configuration setting value</returns>
        public static Map<string, T> settingMap<T>(string name, string prop = "value") =>
            setting(name, Map.empty<string, T>(), prop);

        static Option<T> settingProcess<T>(string name, string prop = "value") =>
            ActorContext.Config.GetProcessSetting<T>(Self, name, prop);

        static Option<T> settingRole<T>(string name, string prop = "value") =>
            ActorContext.Config.GetRoleSetting<T>(name, prop);
    }
}

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
        /// <param name="prop">If the setting is a complex value (like a map or record), then 
        /// this selects the property of the setting to access</param>
        /// <returns>Optional configuration setting value</returns>
        public static T readSetting<T>(string name, string prop, T defaultValue) =>
            InMessageLoop
                ? settingProcess(name, prop, defaultValue)
                : settingRole(name, prop, defaultValue);

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
        public static T readSetting<T>(string name, T defaultValue) =>
            InMessageLoop
                ? settingProcess(name, "value", defaultValue)
                : settingRole(name, "value", defaultValue);

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
        public static Lst<T> readListSetting<T>(string name, string prop = "value") =>
            readSetting(name, prop, List.empty<T>());

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
        public static Map<string, T> readMapSetting<T>(string name, string prop = "value") =>
            readSetting(name, prop, Map.empty<string, T>());

        public static Unit writeSetting(string name, object value, string prop = "value") =>
            InMessageLoop
                ? ActorContext.Config.WriteSettingOverride(ActorInboxCommon.ClusterSettingsKey(Self), value, name, prop)
                : ActorContext.Config.WriteSettingOverride($"role-{Role.Current.Value}@settings", value, name, prop);

        static T settingProcess<T>(string name, string prop, T defaultValue) =>
            ActorContext.Config.GetProcessSetting(Self, name, prop, defaultValue);

        static T settingRole<T>(string name, string prop, T defaultValue) =>
            ActorContext.Config.GetRoleSetting(name, prop, defaultValue);
    }
}

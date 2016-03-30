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
        ///// <summary>
        ///// Access a setting 
        ///// If in a Process message loop, then this accesses the configuration settings
        ///// for the Process from the the configuration file, or stored in the cluster.
        ///// If not in a Process message loop, then this accesses 'global' configuration
        ///// settings.  
        ///// </summary>
        ///// <param name="name">Name of the setting</param>
        ///// <param name="prop">If the setting is a complex value (like a map or record), then 
        ///// this selects the property of the setting to access</param>
        ///// <returns>Optional configuration setting value</returns>
        //public static T readSetting<T>(string name, string prop, T defaultValue) =>
        //    InMessageLoop
        //        ? settingProcess(name, prop, defaultValue)
        //        : settingRole(name, prop, defaultValue);

        ///// <summary>
        ///// Access a setting 
        ///// If in a Process message loop, then this accesses the configuration settings
        ///// for the Process from the the configuration file, or stored in the cluster.
        ///// If not in a Process message loop, then this accesses 'global' configuration
        ///// settings.  
        ///// </summary>
        ///// <param name="name">Name of the setting</param>
        ///// <param name="prop">If the setting is a complex value (like a map or record), then 
        ///// this selects the property of the setting to access</param>
        ///// <returns>Optional configuration setting value</returns>
        //public static T readSetting<T>(string name, T defaultValue) =>
        //    InMessageLoop
        //        ? settingProcess(name, "value", defaultValue)
        //        : settingRole(name, "value", defaultValue);

        ///// <summary>
        ///// Access a list setting 
        ///// If in a Process message loop, then this accesses the configuration settings
        ///// for the Process from the the configuration file, or stored in the cluster.
        ///// If not in a Process message loop, then this accesses 'global' configuration
        ///// settings.  
        ///// </summary>
        ///// <param name="name">Name of the setting</param>
        ///// <param name="prop">If the setting is a complex value (like a map or record), then 
        ///// this selects the property of the setting to access</param>
        ///// <returns>Optional configuration setting value</returns>
        //public static Lst<T> readListSetting<T>(string name, string prop = "value") =>
        //    readSetting(name, prop, List.empty<T>());

        ///// <summary>
        ///// Access a map setting 
        ///// If in a Process message loop, then this accesses the configuration settings
        ///// for the Process from the the configuration file, or stored in the cluster.
        ///// If not in a Process message loop, then this accesses 'global' configuration
        ///// settings.  
        ///// </summary>
        ///// <param name="name">Name of the setting</param>
        ///// <param name="prop">If the setting is a complex value (like a map or record), then 
        ///// this selects the property of the setting to access</param>
        ///// <returns>Optional configuration setting value</returns>
        //public static Map<string, T> readMapSetting<T>(string name, string prop = "value") =>
        //    readSetting(name, prop, Map.empty<string, T>());

        ///// <summary>
        ///// Write a setting
        ///// </summary>
        ///// <param name="name">Name of the setting</param>
        ///// <param name="value">Value to set</param>
        //public static Unit writeSetting(string name, object value) =>
        //    writeSetting(name, "value", value);

        ///// <summary>
        ///// Write a setting
        ///// </summary>
        ///// <param name="name">Name of the setting</param>
        ///// <param name="prop">If the setting is a complex value (like a map or record), then 
        ///// this selects the property of the setting to access</param>
        ///// <param name="value">Value to set</param>
        //public static Unit writeSetting(string name, string prop, object value) =>
        //    InMessageLoop
        //        ? ProcessConfig.Settings.WriteSettingOverride(ActorInboxCommon.ClusterSettingsKey(Self), value, name, prop, ActorContext.ProcessFlags)
        //        : ProcessConfig.Settings.WriteSettingOverride($"role-{Role.Current.Value}@settings", value, name, prop, ProcessFlags.PersistState);

        ///// <summary>
        ///// Clear a setting
        ///// </summary>
        ///// <param name="name">Name of the setting</param>
        ///// <param name="prop">If the setting is a complex value (like a map or record), then 
        ///// this selects the property of the setting to access</param>
        //public static Unit clearSetting(string name, string prop) =>
        //    InMessageLoop
        //        ? ProcessConfig.Settings.ClearSettingOverride(ActorInboxCommon.ClusterSettingsKey(Self), name, prop, ActorContext.ProcessFlags)
        //        : ProcessConfig.Settings.ClearSettingOverride($"role-{Role.Current.Value}@settings", name, prop, ProcessFlags.PersistState);

        ///// <summary>
        ///// Clear all settings for the process (or role if outside of the message-loop of a Process)
        ///// </summary>
        //public static Unit clearSettings() =>
        //    InMessageLoop
        //        ? ProcessConfig.Settings.ClearSettingsOverride(ActorInboxCommon.ClusterSettingsKey(Self), ActorContext.ProcessFlags)
        //        : ProcessConfig.Settings.ClearSettingsOverride($"role-{Role.Current.Value}@settings", ProcessFlags.PersistState);

        //static T settingProcess<T>(string name, string prop, T defaultValue) =>
        //    ProcessConfig.Settings.GetProcessSetting(Self, name, prop, defaultValue, ActorContext.ProcessFlags);

        //static T settingRole<T>(string name, string prop, T defaultValue) =>
        //    ProcessConfig.Settings.GetRoleSetting(name, prop, defaultValue);
    }
}

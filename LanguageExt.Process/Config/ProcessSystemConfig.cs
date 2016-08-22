using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using LanguageExt;
using LanguageExt.Parsec;
using static LanguageExt.Prelude;
using static LanguageExt.Parsec.Prim;
using static LanguageExt.Parsec.Char;
using static LanguageExt.Parsec.Expr;
using static LanguageExt.Parsec.Token;
using LanguageExt.UnitsOfMeasure;
using Newtonsoft.Json;

namespace LanguageExt.Config
{
    /// <summary>
    /// Parses and provides access to configuration settings relating
    /// to the role and individual processes.
    /// </summary>
    public class ProcessSystemConfig
    {
        public readonly SystemName SystemName;
        public readonly Option<ClusterToken> Cluster;
        public readonly string NodeName = "";
        readonly Map<string, ValueToken> roleSettings;
        readonly Map<ProcessId, ProcessToken> processSettings;
        readonly Map<string, State<StrategyContext, Unit>> stratSettings;
        readonly object sync = new object();
        readonly Types types;

        Map<string, Map<string, object>> settingOverrides;
        Time timeout = 30 * seconds;
        Time sessionTimeoutCheck = 60 * seconds;
        int maxMailboxSize = 100000;
        bool transactionalIO = false;

        public readonly static ProcessSystemConfig Empty =
            new ProcessSystemConfig(
                default(SystemName),
                "root",
                Map.empty<string, ValueToken>(),
                Map.empty<ProcessId, ProcessToken>(),
                Map.empty<string, State<StrategyContext, Unit>>(),
                null,
                new Types()
            );

        public ProcessSystemConfig(
            SystemName systemName,
            string nodeName,
            Map<string, ValueToken> roleSettings,
            Map<ProcessId, ProcessToken> processSettings,
            Map<string, State<StrategyContext, Unit>> stratSettings,
            ClusterToken cluster,
            Types types
            )
        {
            SystemName = systemName;
            NodeName = nodeName;
            this.settingOverrides = Map.empty<string, Map<string, object>>();
            this.roleSettings = roleSettings;
            this.processSettings = processSettings;
            this.stratSettings = stratSettings;
            this.Cluster = cluster;
            this.types = types;
        }

        /// <summary>
        /// Returns a named strategy
        /// </summary>
        internal Option<State<StrategyContext, Unit>> GetStrategy(string name) =>
            stratSettings.Find(name);

        internal Option<Map<string, object>> GetProcessSettingsOverrides(ProcessId pid) =>
            settingOverrides.Find(pid.Path);

        /// <summary>
        /// Make a process ID into a /role/... ID
        /// </summary>
        static ProcessId RolePid(ProcessId pid) =>
            ProcessId.Top["role"].Append(pid.Skip(1));

        /// <summary>
        /// Write a single override setting
        /// </summary>
        internal Unit WriteSettingOverride(string key, object value, string name, string prop, ProcessFlags flags)
        {
            if (value == null) failwith<Unit>("Settings can't be null");

            var propKey = $"{name}@{prop}";

            if (flags != ProcessFlags.Default)
            {
                ActorContext.System(SystemName).Cluster.Iter(c => c.HashFieldAddOrUpdate(key, propKey, value));
            }
            settingOverrides = settingOverrides.AddOrUpdate(key, propKey, value);
            return unit;
        }

        /// <summary>
        /// Clear a single override setting
        /// </summary>
        internal Unit ClearSettingOverride(string key, string name, string prop, ProcessFlags flags)
        {
            var propKey = $"{name}@{prop}";
            if (flags != ProcessFlags.Default)
            {
                ActorContext.System(SystemName).Cluster.Iter(c => c.DeleteHashField(key, propKey));
            }
            settingOverrides = settingOverrides.Remove(key, propKey);
            return unit;
        }

        /// <summary>
        /// Clear all override settings for either the process or role
        /// </summary>
        internal Unit ClearSettingsOverride(string key, ProcessFlags flags)
        {
            if (flags != ProcessFlags.Default)
            {
                ActorContext.System(SystemName).Cluster.Iter(c => c.Delete(key));
            }
            return ClearInMemorySettingsOverride(key);
        }

        /// <summary>
        /// Clear all override settings for either the process or role
        /// </summary>
        internal Unit ClearInMemorySettingsOverride(string key)
        {
            settingOverrides = settingOverrides.Remove(key);
            return unit;
        }

        /// <summary>
        /// Get a named process setting
        /// </summary>
        /// <param name="pid">Process</param>
        /// <param name="name">Name of setting</param>
        /// <param name="prop">Name of property within the setting (for complex 
        /// types, not value types)</param>
        /// <returns></returns>
        internal T GetProcessSetting<T>(ProcessId pid, string name, string prop, T defaultValue, ProcessFlags flags)
        {
            var empty = Map.empty<string, ValueToken>();

            var settingsMaps = new[] {
                    processSettings.Find(pid).Map(token => token.Settings).IfNone(empty),
                    processSettings.Find(RolePid(pid)).Map(token => token.Settings).IfNone(empty),
                    roleSettings,
                    Cluster.Map(c => c.Settings).IfNone(empty)
                };

            return GetSettingGeneral(settingsMaps, ActorInboxCommon.ClusterSettingsKey(pid), name, prop, defaultValue, flags);
        }

        /// <summary>
        /// Get a named process setting
        /// </summary>
        /// <param name="pid">Process</param>
        /// <param name="name">Name of setting</param>
        /// <param name="prop">Name of property within the setting (for complex 
        /// types, not value types)</param>
        /// <returns></returns>
        internal Option<T> GetProcessSetting<T>(ProcessId pid, string name, string prop, ProcessFlags flags)
        {
            var empty = Map.empty<string, ValueToken>();

            var settingsMaps = new[] {
                    processSettings.Find(pid).Map(token => token.Settings).IfNone(empty),
                    processSettings.Find(RolePid(pid)).Map(token => token.Settings).IfNone(empty),
                    roleSettings,
                    Cluster.Map(c => c.Settings).IfNone(empty)
                };

            return GetSettingGeneral<T>(settingsMaps, ActorInboxCommon.ClusterSettingsKey(pid), name, prop, flags);
        }

        /// <summary>
        /// Get a named role setting
        /// </summary>
        /// <param name="name">Name of setting</param>
        /// <param name="prop">Name of property within the setting (for complex 
        /// types, not value types)</param>
        internal T GetRoleSetting<T>(string name, string prop, T defaultValue)
        {
            var empty = Map.empty<string, ValueToken>();
            var key = $"role-{Role.Current}@settings";
            var flags = ActorContext.System(SystemName).Cluster.Map(_ => ProcessFlags.PersistState).IfNone(ProcessFlags.Default);
            var settingsMaps = new[] { roleSettings, Cluster.Map(c => c.Settings).IfNone(empty) };
            return GetSettingGeneral(settingsMaps, key, name, prop, defaultValue, flags);
        }

        /// <summary>
        /// Get a named cluster setting
        /// </summary>
        /// <param name="name">Name of setting</param>
        /// <param name="prop">Name of property within the setting (for complex 
        /// types, not value types)</param>
        internal T GetClusterSetting<T>(string name, string prop, T defaultValue) =>
            GetClusterSetting(name, prop, _ => defaultValue);

        /// <summary>
        /// Get a named cluster setting
        /// </summary>
        /// <param name="name">Name of setting</param>
        /// <param name="prop">Name of property within the setting (for complex 
        /// types, not value types)</param>
        internal T GetClusterSetting<T>(string name, string prop, Func<string, T> defaultValue) =>
            GetClusterSetting<T>(name, prop).IfNone(() => defaultValue(name));

        /// <summary>
        /// Get a named cluster setting
        /// </summary>
        /// <param name="name">Name of setting</param>
        /// <param name="prop">Name of property within the setting (for complex 
        /// types, not value types)</param>
        internal Option<T> GetClusterSetting<T>(string name, string prop)
        {
            var key = "cluster@settings";
            var empty = Map.empty<string, ValueToken>();
            var settingsMaps = new[] { Cluster.Map(c => c.Settings).IfNone(empty) };
            return GetSettingGeneral<T>(settingsMaps, key, name, prop, ProcessFlags.Default);
        }

        internal T GetSettingGeneral<T>(IEnumerable<Map<string, ValueToken>> settingsMaps, string key, string name, string prop, T defaultValue, ProcessFlags flags)
        {
            var res = GetSettingGeneral<T>(settingsMaps, key, name, prop, flags);

            if (res.IsNone)
            {
                // No config, no override; so cache the default value so we don't
                // go through all of this again.
                var propKey = $"{name}@{prop}";
                AddOrUpdateProcessOverride(key, propKey, Optional(defaultValue));
            }
            return res.IfNone(defaultValue);
        }

        internal Option<T> GetSettingGeneral<T>(IEnumerable<Map<string, ValueToken>> settingsMaps, string key, string name, string prop, ProcessFlags flags)
        {
            var propKey = $"{name}@{prop}";

            // First see if we have the value cached
            var over = settingOverrides.Find(key, propKey);
            if (over.IsSome) return over.Map(x => (T)x);

            // Next check the cluster data store (Redis usually)
            Option<T> tover = None;
            if (flags != ProcessFlags.Default && SystemName.IsValid)
            {
                tover = from x in ActorContext.System(SystemName).Cluster.Map(c => c.GetHashField<T>(key, propKey))
                        from y in x
                        select y;

                if (tover.IsSome)
                {
                    // It's in the data-store, so cache it locally and return
                    AddOrUpdateProcessOverride(key, propKey, tover);
                    return tover;
                }
            }

            foreach (var settings in settingsMaps)
            {
                tover = from opt1 in prop == "value"
                            ? from tok in settings.Find(name)  
                              from map in MapTokenType<T>(tok).Map(v => (T)v.Value)
                              select map
                            : settings.Find(name).Map(v => v.GetItem<T>(prop))
                        from opt2 in opt1
                        select opt2;

                if (tover.IsSome)
                {
                    // There is a config setting, so cache it and return
                    AddOrUpdateProcessOverride(key, propKey, tover);
                    return tover.IfNoneUnsafe(default(T));
                }
            }
            return None;
        }

        Option<ValueToken> MapTokenType<T>(ValueToken token)
        {
            var type = typeof(T);
            if (type.GetTypeInfo().IsAssignableFrom(token.Value.GetType().GetTypeInfo()))
            {
                return new ValueToken(token.Type, Convert.ChangeType(token.Value, type));
            }
            return from def in types.TypeMap.Find(type.FullName)
                   from map in def.Convert(token)
                   select map;
        }

        void AddOrUpdateProcessOverride<T>(string key, string propKey, Option<T> tover)
        {
            tover.Iter(v =>
            {
                lock (sync)
                {
                    // Update our cache
                    settingOverrides = settingOverrides.AddOrUpdate(key, propKey, v);
                }
            });
        }

        /// <summary>
        /// Get the name to use to register the Process
        /// </summary>
        internal Option<ProcessName> GetProcessRegisteredName(ProcessId pid) =>
            GetProcessSetting<ProcessName>(pid, "register-as", "value", ProcessFlags.Default);

        /// <summary>
        /// Get the dispatch method
        /// This is used by the registration system, it registers the Process 
        /// using a Role[dispatch][...relative path to process...]
        /// </summary>
        internal Option<string> GetProcessDispatch(ProcessId pid) =>
            GetProcessSetting<string>(pid, "dispatch", "value", ProcessFlags.Default);

        /// <summary>
        /// Get the router dispatch method
        /// This is used by routers to specify the type of routing
        /// </summary>
        internal Option<string> GetRouterDispatch(ProcessId pid) =>
            GetProcessSetting<string>(pid, "route", "value", ProcessFlags.Default);

        /// <summary>
        /// Get the router workers list
        /// </summary>
        internal Lst<ProcessToken> GetRouterWorkers(ProcessId pid) =>
            GetProcessSetting<Lst<ProcessToken>>(pid, "workers", "value", ProcessFlags.Default)
           .IfNone(List.empty<ProcessToken>());

        /// <summary>
        /// Get the router worker count
        /// </summary>
        internal Option<int> GetRouterWorkerCount(ProcessId pid) =>
            GetProcessSetting<int>(pid, "worker-count", "value", ProcessFlags.Default);

        /// <summary>
        /// Get the router worker name
        /// </summary>
        internal string GetRouterWorkerName(ProcessId pid) =>
            GetProcessSetting<string>(pid, "worker-name", "value", ProcessFlags.Default)
           .IfNone("worker");

        /// <summary>
        /// Get the mailbox size for a Process.  Returns a default size if one
        /// hasn't been set in the config.
        /// </summary>
        internal int GetProcessMailboxSize(ProcessId pid) =>
            GetProcessSetting<int>(pid, "mailbox-size", "value", ProcessFlags.Default)
           .IfNone(maxMailboxSize);

        /// <summary>
        /// Get the flags for a Process.  Returns ProcessFlags.Default if none
        /// have been set in the config.
        /// </summary>
        internal ProcessFlags GetProcessFlags(ProcessId pid) =>
            GetProcessSetting<ProcessFlags>(pid, "flags", "value", ProcessFlags.Default)
           .IfNone(ProcessFlags.Default);

        /// <summary>
        /// Get the strategy for a Process.  Returns Process.DefaultStrategy if one
        /// hasn't been set in the config.
        /// </summary>
        internal State<StrategyContext, Unit> GetProcessStrategy(ProcessId pid) =>
            GetProcessSetting<State<StrategyContext, Unit>>(pid, "strategy", "value", ProcessFlags.Default)
           .IfNone(Process.DefaultStrategy);

        /// <summary>
        /// Get the role wide timeout setting.  This specifies how long the timeout
        /// is for 'ask' operations.
        /// </summary>
        internal Time Timeout =>
            timeout;

        /// <summary>
        /// This is the setting for how often sessions are checked for expiry, *not*
        /// the expiry time itself.  That is set on each sessionStart()
        /// </summary>
        internal Time SessionTimeoutCheckFrequency =>
            sessionTimeoutCheck;

        internal bool TransactionalIO =>
            transactionalIO;

        internal void PostConnect()
        {
            // Cache the frequently accessed
            maxMailboxSize = GetRoleSetting("mailbox-size", "value", 100000);
            timeout = GetRoleSetting("timeout", "value", 30 * seconds);
            sessionTimeoutCheck = GetRoleSetting("session-timeout-check", "value", 60 * seconds);
            transactionalIO = GetRoleSetting("transactional-io", "value", true);
        }
    }
}

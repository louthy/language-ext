using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using LanguageExt;
using LanguageExt.Trans;
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
    /// <remarks>
    /// TODO: Tidy this class up, it's a bit icky.
    /// </remarks>
    public class ProcessSystemConfig
    {
        public readonly string NodeName = "";
        public readonly Parser<Map<string, LocalsToken>> Parser;
        readonly object sync = new object();

        Map<string, LocalsToken> roleSettings;
        Map<ProcessId, ProcessToken> processSettings;
        Map<string, StrategyToken> stratSettings;
        Map<string, ClusterToken> clusterSettings;
        Map<string, Map<string, object>> settingOverrides;
        Map<string, LocalsToken> cluster;

        Time timeout = 30 * seconds;
        Time sessionTimeoutCheck = 60 * seconds;
        int maxMailboxSize = 100000;
        bool transactionalIO = true;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="strategyFuncs">Allows bespoke strategies to be plugged into the parser</param>
        public ProcessSystemConfig(string nodeName, IEnumerable<FuncSpec> strategyFuncs = null)
        {
            NodeName = nodeName ?? "";
            ClearSettings();
            Parser = InitialiseParser(nodeName, strategyFuncs);
        }

        /// <summary>
        /// Returns a named strategy
        /// </summary>
        internal Option<State<StrategyContext, Unit>> GetStrategy(string name) =>
            stratSettings.Find(name).Map(x => x.Value);

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
                ActorContext.Cluster.Iter(c => c.HashFieldAddOrUpdate(key, propKey, value));
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
                ActorContext.Cluster.Iter(c => c.DeleteHashField(key, propKey));
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
                ActorContext.Cluster.Iter(c => c.Delete(key));
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
            var empty = Map.empty<string, LocalsToken>();

            var settingsMaps = new[] {
                    processSettings.Find(pid).Map(token => token.Settings).IfNone(empty),
                    processSettings.Find(RolePid(pid)).Map(token => token.Settings).IfNone(empty),
                    roleSettings,
                    cluster
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
            var empty = Map.empty<string, LocalsToken>();

            var settingsMaps = new[] {
                    processSettings.Find(pid).Map(token => token.Settings).IfNone(empty),
                    processSettings.Find(RolePid(pid)).Map(token => token.Settings).IfNone(empty),
                    roleSettings,
                    cluster
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
            var empty = Map.empty<string, LocalsToken>();
            var key = $"role-{Role.Current}@settings";
            var flags = ActorContext.Cluster.Map(_ => ProcessFlags.PersistState).IfNone(ProcessFlags.Default);
            var settingsMaps = new[] { roleSettings, cluster };
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
            var empty = Map.empty<string, LocalsToken>();
            var key = "cluster@settings";
            var settingsMaps = new[] { cluster };
            return GetSettingGeneral<T>(settingsMaps, key, name, prop, ProcessFlags.Default);
        }

        internal T GetSettingGeneral<T>(IEnumerable<Map<string, LocalsToken>> settingsMaps, string key, string name, string prop, T defaultValue, ProcessFlags flags)
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

        internal Option<T> GetSettingGeneral<T>(IEnumerable<Map<string,LocalsToken>> settingsMaps, string key, string name, string prop, ProcessFlags flags)
        {
            var propKey = $"{name}@{prop}";

            // First see if we have the value cached
            var over = settingOverrides.Find(key, propKey);
            if (over.IsSome) return over.Map(x => (T)x);

            // Next check the custer data store (Redis usually)
            Option<T> tover = None;
            if (flags != ProcessFlags.Default)
            {
                tover = from x in ActorContext.Cluster.Map(c => c.GetHashField<T>(key, propKey))
                        from y in x
                        select y;

                if (tover.IsSome)
                {
                    // It's in the data-store, so cache it locally and return
                    AddOrUpdateProcessOverride(key, propKey, tover);
                    return tover;
                }
            }

            foreach(var settings in settingsMaps)
            {
                tover = from s in settings.Find(name)
                        from v in s.Values.Find(prop)
                      // TODO: Type check
                        select (T)v.Value;

                if (tover.IsSome)
                {
                    // There is a config setting, so cache it and return
                    AddOrUpdateProcessOverride(key, propKey, tover);
                    return tover.IfNoneUnsafe(default(T));
                }
            }
            return None;
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
            GetProcessSetting<StrategyToken>(pid, "strategy", "value", ProcessFlags.Default)
           .Map(tok => tok.Value)
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

        internal void ParseConfigText(string text)
        {
            if(String.IsNullOrWhiteSpace(text))
            {
                ClearSettings();
                return;
            }

            // Parse the config text
            var res = parse(Parser, text);
            if (res.IsFaulted || res.Reply.State.ToString().Length > 0)
            {
                throw new ProcessConfigException(res.ToString());
            }

            // Extract the process settings
            processSettings = List.fold(
                from val in res.Reply.Result.Values
                where val.Spec.Args.Length > 0 && val.Spec.Args[0].Type.Tag == ArgumentTypeTag.Process
                let p = (ProcessToken)val.Values.Values.First().Value
                where p.ProcessId.IsSome
                let id = p.ProcessId.IfNone(ProcessId.None)
                select Tuple(id, p.RegisteredName.IsNone ? p.SetRegisteredName(val.Name) : p),
                Map.empty<ProcessId, ProcessToken>(),
                (s, x) => Map.tryAdd(s, x.Item1, x.Item2, (_, p) => failwith<Map<ProcessId, ProcessToken>>("Process declared twice: " + p.RegisteredName.IfNone("not defined"))));
                

            // Extract the strategy settings
            stratSettings = List.fold(
                from val in res.Reply.Result.Values
                where val.Spec.Args.Length > 0 && val.Spec.Args[0].Type.Tag == ArgumentTypeTag.Strategy
                let s = (StrategyToken)val.Values.Values.First().Value
                select Tuple(val.Name, s),
                Map.empty<string, StrategyToken>(),
                (s, x) => Map.tryAdd(s, x.Item1, x.Item2, (_, __) => failwith<Map<string, StrategyToken>>("Strategy declared twice: " + x.Item1)));

            // Extract the cluster settings
            clusterSettings = List.fold(
                from val in res.Reply.Result.Values
                where val.Spec.Args.Length > 0 && val.Spec.Args[0].Type.Tag == ArgumentTypeTag.Cluster
                let cluster = (ClusterToken)val.Values.Values.First().Value
                select Tuple(cluster.NodeName.IfNone(""), cluster.Env.IsNone ? cluster.SetEnvironment(val.Name) : cluster),
                Map.empty<string, ClusterToken>(),
                (s, x) => Map.tryAdd(s, x.Item1, x.Item2, (_, c) => failwith<Map<string, ClusterToken>>("Cluster declared twice: " + c.Env.IfNone(""))));

            roleSettings = res.Reply.Result;

            if (!String.IsNullOrEmpty(NodeName))
            {
                var clusterOpt = clusterSettings.Find(NodeName);
                if (clusterOpt.IsNone)
                {
                    throw new Exception($"Cluster defintion missing that has a node-name attribute and a value of: '{NodeName}'");
                }
                cluster = clusterOpt.Map(token => token.Settings).IfNone(Map.empty<string, LocalsToken>());
            }
        }

        internal void PostConnect()
        {
            // Cache the frequently accessed
            maxMailboxSize = GetRoleSetting("mailbox-size", "value", 100000);
            timeout = GetRoleSetting("timeout", "value", 30 * seconds);
            sessionTimeoutCheck = GetRoleSetting("session-timeout-check", "value", 60 * seconds);
            transactionalIO = GetRoleSetting("transactional-io", "value", true);
        }

        void ClearSettings()
        {
            roleSettings = Map<string, LocalsToken>.Empty;
            processSettings = Map<ProcessId, ProcessToken>.Empty;
            stratSettings = Map<string, StrategyToken>.Empty;
            clusterSettings = Map<string, ClusterToken>.Empty;
            settingOverrides = Map<string, Map<string, object>>.Empty;
            cluster = Map.empty<string, LocalsToken>();
            timeout = 30 * seconds;
            sessionTimeoutCheck = 60 * seconds;
            maxMailboxSize = 100000;
            transactionalIO = true;
        }

        Parser<Map<string,LocalsToken>> InitialiseParser(string nodeName, IEnumerable<FuncSpec> strategyFuncs)
        {
            var process = new[] {
                FuncSpec.Attr("pid", FieldSpec.ProcessId("value")),
                FuncSpec.Attr("flags", FieldSpec.ProcessFlags("value")),
                FuncSpec.Attr("mailbox-size", FieldSpec.Int("value")),

                FuncSpec.Attr("dispatch", FieldSpec.DispatcherType("value")),
                FuncSpec.Attr("route", FieldSpec.DispatcherType("value")),
                FuncSpec.Attr("workers", FieldSpec.Array("value", ArgumentType.Process)),
                FuncSpec.Attr("worker-count", FieldSpec.Int("value")),
                FuncSpec.Attr("worker-name", FieldSpec.String("value")),
                FuncSpec.Attr("strategy", FieldSpec.Strategy("value"))
            };

            var cluster = new[] {
                FuncSpec.Attr("node-name", FieldSpec.String("value")),
                FuncSpec.Attr("role", FieldSpec.String("value")),
                FuncSpec.Attr("provider", FieldSpec.String("value")),
                FuncSpec.Attr("connection", FieldSpec.String("value")),
                FuncSpec.Attr("database", FieldSpec.String("value")),
                FuncSpec.Attr("env", FieldSpec.String("value")),
                FuncSpec.Attr("user-env", FieldSpec.String("value"))
            };

            var sys = new ProcessSystemConfigParser(
                nodeName,
                process,
                BuildStrategySpec(strategyFuncs),
                cluster
            );

            return sys.Settings;
        }

        FuncSpec[] BuildStrategySpec(IEnumerable<FuncSpec> strategyFuncs)
        {
            strategyFuncs = strategyFuncs ?? new FuncSpec[0];
            var funcsToAdd = Set.createRange(strategyFuncs.Map(x => x.Name));
            return GetPredefinedStrategyFuncs().Filter(x => !funcsToAdd.Contains(x.Name))
                                                   .Append(strategyFuncs)
                                                   .ToArray();
        }

        FuncSpec[] GetPredefinedStrategyFuncs() =>
            new[] {

                // always: directive
                FuncSpec.Attr("always", settings => Strategy.Always((Directive)settings["value"].Value),  FieldSpec.Directive("value")),

                // always: pause: duration
                FuncSpec.Attr("pause", settings => Strategy.Pause((Time)settings["duration"].Value), FieldSpec.Time("duration")),

                // retries: count = int
                // retries: count = int, duration = time
                FuncSpec.Attr(
                    "retries",
                    ArgumentsSpec.Variant(
                        settings => Strategy.Retries((int)settings["count"].Value,(Time)settings["duration"].Value),
                        FieldSpec.Int("count"),
                        FieldSpec.Time("duration")),

                    ArgumentsSpec.Variant(
                        settings => Strategy.Retries((int)settings["count"].Value),
                        FieldSpec.Int("count"))
                ),

                // retries: backoff: duration
                // retries: backoff: min = time, max = time, step = time
                FuncSpec.Attr("backoff",
                    ArgumentsSpec.Variant(
                        settings => Strategy.Backoff((Time)settings["min"].Value,(Time)settings["max"].Value,(Time)settings["step"].Value),
                        FieldSpec.Time("min"), FieldSpec.Time("max"), FieldSpec.Time("step")
                        ),

                    ArgumentsSpec.Variant(
                        settings => Strategy.Backoff((Time)settings["duration"].Value),
                        FieldSpec.Time("duration")
                        )),

                // match
                // | exception -> directive
                // | _         -> directive
                FuncSpec.AttrNoArgs("match"),

                // redirect when
                // | directive -> message-directive
                // | _         -> message-directive
                FuncSpec.AttrNoArgs("redirect")
            };
    }
}

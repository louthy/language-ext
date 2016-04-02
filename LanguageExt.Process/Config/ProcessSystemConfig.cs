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
        public readonly Parser<Lst<NamedValueToken>> Parser;
        readonly object sync = new object();

        Map<string, ValueToken> roleSettings;
        Map<ProcessId, ProcessToken> processSettings;
        Map<string, State<StrategyContext, Unit>> stratSettings;
        Map<string, ClusterToken> clusterSettings;
        Map<string, Map<string, object>> settingOverrides;
        ClusterToken cluster;

        Time timeout = 30 * seconds;
        Time sessionTimeoutCheck = 60 * seconds;
        int maxMailboxSize = 100000;
        bool transactionalIO = true;
        Types types;
        TypeDef processType;
        TypeDef strategyType;
        TypeDef clusterType;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="strategyFuncs">Allows bespoke strategies to be plugged into the parser</param>
        public ProcessSystemConfig(string nodeName, IEnumerable<FuncSpec> strategyFuncs = null)
        {
            NodeName = nodeName ?? "";
            ClearSettings();
            types = new Types();
            Parser = InitialiseParser(nodeName, strategyFuncs);
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
            var empty = Map.empty<string, ValueToken>();

            var settingsMaps = new[] {
                    processSettings.Find(pid).Map(token => token.Settings).IfNone(empty),
                    processSettings.Find(RolePid(pid)).Map(token => token.Settings).IfNone(empty),
                    roleSettings,
                    cluster.Settings
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
                    cluster.Settings
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
            var key = $"role-{Role.Current}@settings";
            var flags = ActorContext.Cluster.Map(_ => ProcessFlags.PersistState).IfNone(ProcessFlags.Default);
            var settingsMaps = new[] { roleSettings, cluster.Settings };
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
            var settingsMaps = new[] { cluster.Settings };
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
            var type = typeof(T).GetTypeInfo();
            if (type.IsAssignableFrom(token.Value.GetType().GetTypeInfo()))
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

        internal void ParseConfigText(string text)
        {
            if (String.IsNullOrWhiteSpace(text))
            {
                ClearSettings();
                return;
            }

            // Parse the config text
            var res = parse(Parser, text);
            if (res.IsFaulted || res.Reply.State.ToString().Length > 0)
            {
                if (res.IsFaulted)
                {
                    throw new ProcessConfigException(res.ToString());
                }
                else
                {
                    var clipped = res.Reply.State.ToString();
                    clipped = clipped.Substring(0, Math.Min(40, clipped.Length));
                    throw new ProcessConfigException($"Configuration parse error at {res.Reply.State.Pos}, near: {clipped}");
                }
            }

            // Extract the process settings
            processSettings = List.fold(
                from nv in res.Reply.Result
                where nv.Value.Type == processType
                let process = nv.Value.Cast<ProcessToken>()
                let pid = process.ProcessId
                where pid.IsSome
                let reg = process.RegisteredName
                let final = process.SetRegisteredName(new ValueToken(types.ProcessName, reg.IfNone(new ProcessName(nv.Name))))
                select Tuple(pid.IfNone(ProcessId.None), final),
                Map.empty<ProcessId, ProcessToken>(),
                (s, x) => Map.tryAdd(s, x.Item1, x.Item2, (_, p) => failwith<Map<ProcessId, ProcessToken>>("Process declared twice: " + p.RegisteredName.IfNone("not defined"))));


            // Extract the cluster settings
            stratSettings = List.fold(
                from nv in res.Reply.Result
                where nv.Value.Type == strategyType
                let strategy = nv.Value.Cast<State<StrategyContext, Unit>>()
                select Tuple(nv.Name, strategy),
                Map.empty<string, State<StrategyContext, Unit>>(),
                (s, x) => Map.tryAdd(s, x.Item1, x.Item2, (_, __) => failwith<Map<string, State<StrategyContext, Unit>>>("Strategy declared twice: " + x.Item1)));

            // Extract the strategy settings
            clusterSettings = List.fold(
                from nv in res.Reply.Result
                where nv.Value.Type == clusterType
                let cluster = nv.Value.Cast<ClusterToken>()
                let env = cluster.Env
                let nodeName = cluster.NodeName
                where nodeName.IsSome
                let final = cluster.SetEnvironment(new ValueToken(types.String, env.IfNone(nv.Name)))
                select Tuple(nodeName.IfNone(""), final),
                Map.empty<string, ClusterToken>(),
                (s, x) => Map.tryAdd(s, x.Item1, x.Item2, (_, c) => failwith<Map<string, ClusterToken>>("Cluster declared twice: " + c.Env.IfNone(""))));

            roleSettings = List.fold(
                res.Reply.Result,
                Map.empty<string, ValueToken>(),
                (s, x) => Map.addOrUpdate(s, x.Name, x.Value)
            );

            if (!String.IsNullOrEmpty(NodeName))
            {
                cluster = clusterSettings
                                .Find(NodeName)
                                .IfNone(() => failwith<ClusterToken>($"Cluster defintion missing that has a node-name attribute and a value of: '{NodeName}'"));
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
            roleSettings = Map<string, ValueToken>.Empty;
            processSettings = Map<ProcessId, ProcessToken>.Empty;
            stratSettings = Map<string, State<StrategyContext, Unit>>.Empty;
            clusterSettings = Map<string, ClusterToken>.Empty;
            settingOverrides = Map<string, Map<string, object>>.Empty;
            cluster = ClusterToken.Empty;
            timeout = 30 * seconds;
            sessionTimeoutCheck = 60 * seconds;
            maxMailboxSize = 100000;
            transactionalIO = true;
        }



        Parser<Lst<NamedValueToken>> InitialiseParser(string nodeName, IEnumerable<FuncSpec> strategyFuncs)
        {
            strategyFuncs = strategyFuncs ?? new FuncSpec[0];

            clusterType = new TypeDef(
                "cluster",
                nvs => new ClusterToken(nvs),
                20,
                FuncSpec.Property("node-name", () => types.String),
                FuncSpec.Property("role", () => types.String),
                FuncSpec.Property("provider", () => types.String),
                FuncSpec.Property("connection", () => types.String),
                FuncSpec.Property("database", () => types.String),
                FuncSpec.Property("env", () => types.String),
                FuncSpec.Property("user-env", () => types.String));

            types.Register(clusterType);

            processType = new TypeDef(
                "process",
                nvs => new ProcessToken(nvs),
                20,
                FuncSpec.Property("pid", () => types.ProcessId),
                FuncSpec.Property("flags", () => types.ProcessFlags),
                FuncSpec.Property("mailbox-size", () => types.Int),
                FuncSpec.Property("dispatch", () => types.DispatcherType),
                FuncSpec.Property("route", () => types.DispatcherType),
                FuncSpec.Property("workers", () => TypeDef.Array(() => processType)),
                FuncSpec.Property("worker-count", () => types.Int),
                FuncSpec.Property("worker-name", () => types.String),
                FuncSpec.Property("strategy", () => strategyType));

            types.Register(processType);

            strategyType = BuildStrategySpec(types, strategyFuncs);

            types.Register(strategyType);

            var sys = new ProcessSystemConfigParser(
                nodeName,
                types
            );

            return sys.Settings;
        }

        TypeDef BuildStrategySpec(Types types, IEnumerable<FuncSpec> strategyFuncs)
        {
            TypeDef strategy = null;

            Func<Lst<NamedValueToken>, State<StrategyContext, Unit>[]> compose = items => items.Map(x => (State<StrategyContext, Unit>)x.Value.Value).ToArray();


            var oneForOne = FuncSpec.Property("one-for-one", () => strategy, () => strategy, value => Strategy.OneForOne((State<StrategyContext, Unit>)value));
            var allForOne = FuncSpec.Property("all-for-one", () => strategy, () => strategy, value => Strategy.AllForOne((State<StrategyContext, Unit>)value));

            var always = FuncSpec.Property("always", () => strategy, () => types.Directive, value => Strategy.Always((Directive)value));
            var pause = FuncSpec.Property("pause", () => strategy, () => types.Time, value => Strategy.Pause((Time)value));
            var retries1 = FuncSpec.Property("retries", () => strategy, () => types.Int, value => Strategy.Retries((int)value));

            var retries2 = FuncSpec.Attrs(
                "retries",
                () => strategy,
                locals => Strategy.Retries((int)locals["count"], (Time)locals["duration"]),
                new FieldSpec("count", () => types.Int),
                new FieldSpec("duration", () => types.Time)
            );

            var backoff1 = FuncSpec.Attrs(
                "backoff",
                () => strategy,
                locals => Strategy.Backoff((Time)locals["min"], (Time)locals["max"], (Time)locals["step"]),
                new FieldSpec("min", () => types.Time),
                new FieldSpec("max", () => types.Time),
                new FieldSpec("step", () => types.Time)
            );

            var backoff2 = FuncSpec.Property("backoff", () => strategy, () => types.Time, value => Strategy.Backoff((Time)value));

            // match
            // | exception -> directive
            // | _         -> directive
            var match = FuncSpec.Special("match", () => strategy);

            // redirect when
            // | directive -> message-directive
            // | _         -> message-directive
            var redirect = FuncSpec.Special("redirect", () => strategy);

            strategy = new TypeDef(
                "strategy",
                s => Strategy.Compose(compose(s)),
                20,
                new[] { oneForOne, allForOne, always, pause, retries1, retries2, backoff1, backoff2, match, redirect }.Append(strategyFuncs).ToArray()
            );

            return strategy;
        }
    }
}

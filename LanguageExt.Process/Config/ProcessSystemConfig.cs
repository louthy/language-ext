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

namespace LanguageExt
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
        Map<string, LocalsToken> roleSettings;
        Map<ProcessId, ProcessToken> processSettings;
        Map<string, StrategyToken> stratSettings;
        public readonly Parser<Map<string, LocalsToken>> Parser;
        Time timeout = 30*seconds;
        int maxMailboxSize = 100000;
        readonly object sync = new object();
        string configText = "";

        Map<string, Map<string, object>> settingOverrides;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="strategyFuncs">Allows bespoke strategies to be plugged 
        /// into the parser</param>
        public ProcessSystemConfig(IEnumerable<FuncSpec> strategyFuncs = null)
        {
            Parser = InitialiseParser(strategyFuncs);
            roleSettings = Map.empty<string, LocalsToken>();
            processSettings = Map.empty<ProcessId, ProcessToken>();
            settingOverrides = Map.empty<string, Map<string, object>>();
            stratSettings = Map.empty<string, StrategyToken>();
        }

        /// <summary>
        /// Setup using the configuration settings text
        /// </summary>
        public void LoadFromText(Option<string> configText) =>
            configText.Iter(ParseConfigText);

        /// <summary>
        /// Setup using the configuration settings in the file specified
        /// </summary>
        public void LoadFromFile(Option<string> configFilename)
        {
            (from p in configFilename.Map(Some: x => File.Exists(x) ? x : "", None: () => FindLocalConfig())
             where p != ""
             select p).Iter(LoadConfigFile);
        }

        /// <summary>
        /// Setup using the configuration settings from pre-saved spec in the cluster
        /// If you haven't set one up, 
        /// </summary>
        public Unit LoadFromCluster()
        {
            var key = $"role-{Role.Current}@spec";

            ActorContext.Cluster.Match(
                Some: c => {
                    if (c.Exists(key)) LoadFromText(c.GetValue<string>(key)); else LoadFromFile(None);
                },
                None: () => LoadFromFile(None)
                );
            return unit;
        }

        /// <summary>
        /// Saves the current state of the settings to the cluster
        /// 
        /// NOTE: This will just save the original text that was 
        /// either loaded or passed to this class when it was initialised 
        /// (via LoadFromCluster/LoadFromFile/LoadFromText).  
        /// 
        /// NOTE: The settings are saved for the role, and will be shared 
        /// between multiple nodes in the role.
        /// </summary>
        public Unit SaveToCluser()
        {
            var key = $"role-{Role.Current}@spec";

            var res = parse(Parser, configText);
            if (res.IsFaulted || res.Reply.State.ToString().Length > 0)
            {
                throw new ProcessConfigException(res.ToString());
            }

            return ActorContext.Cluster.Iter(c => c.SetValue(key, configText));
        }

        /// <summary>
        /// Returns a named strategy
        /// </summary>
        internal Option<State<StrategyContext, Unit>> GetStrategy(string name) =>
            stratSettings.Find(name).Map(x => x.Value);

        static ProcessId RolePid(ProcessId pid) =>
            ProcessId.Top["role"].Append(pid.Skip(1));

        internal Unit WriteSettingOverride(string key, object value, string name, string prop = "value")
        {
            if (value == null) failwith<Unit>("Settings can't be null");

            var propKey = $"{name}@{prop}";
            ActorContext.Cluster.Iter( c =>
                {
                    c.HashFieldAddOrUpdate(key, propKey, value);
                });

            settingOverrides = settingOverrides.AddOrUpdate(key, propKey, value);
            return unit;
        }

        /// <summary>
        /// Returns the token that represents all the settings for a Process
        /// </summary>
        Option<ProcessToken> GetProcessSettings(ProcessId pid)
        {
            if (pid.IsValid && pid.Count() > 1)
            {
                var exact = processSettings.Find(pid);
                if( exact.IsNone )
                {
                    return processSettings.Find(RolePid(pid));
                }
                else
                {
                    return exact;
                }
            }
            else
            { 
                return None;
            }
        }

        /// <summary>
        /// Get a named process setting
        /// </summary>
        /// <param name="pid">Process</param>
        /// <param name="name">Name of setting</param>
        /// <param name="prop">Name of property within the setting (for complex 
        /// types, not value types)</param>
        /// <returns></returns>
        internal T GetProcessSetting<T>(ProcessId pid, string name, string prop, T defaultValue)
        {
            // First see if we have the value cached
            var key     = ActorInboxCommon.ClusterSettingsKey(pid);
            var propKey = $"{name}@{prop}";
            var over    = settingOverrides.Find(key, propKey);
            if (over.IsSome) return over.Map(x => (T)x).IfNoneUnsafe(defaultValue);

            // Next check the data store
            var tover = from x in ActorContext.Cluster.Map(c => c.GetHashField<T>(key, propKey))
                        from y in x
                        select y;

            if (tover.IsSome)
            {
                // It's in the data-store, so cache it locally and return
                AddOrUpdateProcessOverride(key, propKey, tover);
                return tover.IfNoneUnsafe(default(T));
            }

            // Check the config settings
            tover = from t in GetProcessSettings(pid)
                    from s in t.Settings.Find(name)
                    from v in s.Values.Find(prop)
                    // TODO: Type check
                    select (T)v.Value;

            if( tover.IsSome )
            {
                // There is a config setting, so cache it and return
                AddOrUpdateProcessOverride(key, propKey, tover);
                return tover.IfNoneUnsafe(default(T));
            }
            else
            {
                // No config, no override; so cache the default value so we don't
                // go through all of this again.
                AddOrUpdateProcessOverride(key, propKey, Optional(defaultValue));
                return defaultValue;
            }
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
            from x in GetProcessSettings(pid)
            from y in x.RegisteredName
            select y;

        /// <summary>
        /// Get the dispatch method
        /// This is used by the registration system, it registers the Process 
        /// using a Role[dispatch][...relative path to process...]
        /// </summary>
        internal Option<string> GetProcessDispatch(ProcessId pid) =>
            from x in GetProcessSettings(pid)
            from y in x.Dispatch
            select y;

        /// <summary>
        /// Get the router dispatch method
        /// This is used by routers to specify the type of routing
        /// </summary>
        internal Option<string> GetRouterDispatch(ProcessId pid) =>
            from x in GetProcessSettings(pid)
            from y in x.Route
            select y;

        /// <summary>
        /// Get the router workers list
        /// </summary>
        internal Lst<ProcessToken> GetRouterWorkers(ProcessId pid) =>
            (from x in GetProcessSettings(pid)
             from y in x.Workers
             select y)
            .IfNone(List.empty<ProcessToken>());

        /// <summary>
        /// Get the router worker count
        /// </summary>
        internal Option<int> GetRouterWorkerCount(ProcessId pid) =>
            from x in GetProcessSettings(pid)
            from y in x.WorkerCount
            select y;

        /// <summary>
        /// Get the router worker name
        /// </summary>
        internal Option<string> GetRouterWorkerName(ProcessId pid) =>
            from x in GetProcessSettings(pid)
            from y in x.WorkerName
            select y;

        /// <summary>
        /// Get the mailbox size for a Process.  Returns a default size if one
        /// hasn't been set in the config.
        /// </summary>
        internal int GetProcessMailboxSize(ProcessId pid) =>
            (from x in GetProcessSettings(pid)
             from y in x.MailboxSize
             select y)
            .IfNone(maxMailboxSize);

        /// <summary>
        /// Get the flags for a Process.  Returns ProcessFlags.Default if none
        /// have been set in the config.
        /// </summary>
        internal ProcessFlags GetProcessFlags(ProcessId pid) =>
            (from x in GetProcessSettings(pid)
             from y in x.Flags
             select y)
            .IfNone(ProcessFlags.Default);

        /// <summary>
        /// Get the strategy for a Process.  Returns Process.DefaultStrategy if one
        /// hasn't been set in the config.
        /// </summary>
        internal State<StrategyContext, Unit> GetProcessStrategy(ProcessId pid) =>
            (from x in GetProcessSettings(pid)
             from y in x.Strategy
             select y)
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
            GetRoleSetting<Time>("session-timeout-check", "value", 60 * seconds);

        /// <summary>
        /// Get a named role setting
        /// </summary>
        /// <remarks>
        /// TODO: This and GetProcessSetting are almost exactly the same.  Refactor.
        /// </remarks>
        internal T GetRoleSetting<T>(string name, string prop, T defaultValue)
        {
            // First see if we have the value cached
            var key = $"role-{Role.Current}@settings";
            var propKey = $"{name}@{prop}";
            var over = settingOverrides.Find(key, propKey);
            if (over.IsSome) return over.Map(x => (T)x).IfNoneUnsafe(defaultValue);

            // Next check the data store
            var tover = from x in ActorContext.Cluster.Map(c => c.GetHashField<T>(key, propKey))
                        from y in x
                        select y;

            if (tover.IsSome)
            {
                // It's in the data-store, so cache it locally and return
                AddOrUpdateProcessOverride(key, propKey, tover);
                return tover.IfNoneUnsafe(default(T));
            }

            // Check the config settings
            tover = from setting in roleSettings.Find(name)
                    from valtok in setting.Values.Find(prop)
                    // TODO: Type check
                    select(T)valtok.Value;

            if (tover.IsSome)
            {
                // There is a config setting, so cache it and return
                AddOrUpdateProcessOverride(key, propKey, tover);
                return tover.IfNoneUnsafe(default(T));
            }
            else
            {
                // No config, no override; so cache the default value so we don't
                // go through all of this again.
                AddOrUpdateProcessOverride(key, propKey, Optional(defaultValue));
                return defaultValue;
            }
        }

        Option<ArgumentType> GetProcessSettingType(ProcessId pid, string name, string prop = "value") =>
            from t in GetProcessSettings(pid)
            from s in t.Settings.Find(name)
            from v in s.Values.Find(prop)
            select v.Type;

        Option<LocalsToken> GetRoleSettings(string name) =>
            roleSettings.Find(name);

        Option<ArgumentType> GetRoleSettingType(string name, string prop = "value") =>
            from setting in roleSettings.Find(name)
            from valtok in setting.Values.Find(prop)
            select valtok.Type;


#if !COREFX
        string FindLocalConfig() =>
            map(Path.Combine(Directory.GetParent(Assembly.GetEntryAssembly().Location).FullName, "process.conf"),
                (string path1) => File.Exists(path1)
                    ? path1
                    : map(Path.Combine(Directory.GetParent(Directory.GetParent(Assembly.GetEntryAssembly().Location).FullName).FullName, "process.conf"),
                        (string path2) => File.Exists(path2)
                            ? path2
                            : ""));
#endif

#if COREFX
        public void InitialiseFromText(Option<string> configText) =>
            configText.Iter(ParseConfigText);

#else
        void LoadConfigFile(string path)
        {
            ParseConfigText(File.ReadAllText(path));
        }
#endif

        void ParseConfigText(string text)
        {
            if(String.IsNullOrWhiteSpace(text))
            {
                ClearSettings();
                return;
            }

            // Parse the config text
            configText = text;
            var res = parse(Parser, text);
            if (res.IsFaulted || res.Reply.State.ToString().Length > 0)
            {
                throw new ProcessConfigException(res.ToString());
            }

            // Extract the process settings
            processSettings = Map.createRange(from val in res.Reply.Result.Values
                                              where val.Spec.Args.Length > 0 && val.Spec.Args[0].Type.Tag == ArgumentTypeTag.Process
                                              let p = (ProcessToken)val.Values.Values.First().Value
                                              where p.ProcessId.IsSome
                                              let id = p.ProcessId.IfNone(ProcessId.None)
                                              select Tuple(id, p.RegisteredName.IsNone ? p.SetRegisteredName(val.Name) : p));

            // Extract the strategy settings
            stratSettings = Map.createRange(from val in res.Reply.Result.Values
                                            where val.Spec.Args.Length > 0 && val.Spec.Args[0].Type.Tag == ArgumentTypeTag.Strategy
                                            let s = (StrategyToken)val.Values.Values.First().Value
                                            select Tuple(val.Name, s));

            roleSettings = res.Reply.Result;

            // Cache the frequently accessed
            maxMailboxSize = GetRoleSetting("mailbox-size", "value", 100000);
            timeout = GetRoleSetting("timeout", "value", 30 * seconds);


        }

        private void ClearSettings()
        {
            roleSettings = Map<string, LocalsToken>.Empty;
            processSettings = Map<ProcessId, ProcessToken>.Empty;
            stratSettings = Map<string, StrategyToken>.Empty;
            timeout = 30 * seconds;
            maxMailboxSize = 100000;
            configText = "";
        }

        Parser<Map<string,LocalsToken>> InitialiseParser(IEnumerable<FuncSpec> strategyFuncs)
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

            var sys = new ProcessSystemConfigParser(
                process,
                BuildStrategySpec(strategyFuncs)
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

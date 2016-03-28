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

namespace LanguageExt
{
    /// <summary>
    /// Parses and provides access to configuration settings relating
    /// to the role and individual processes.
    /// </summary>
    public class ProcessSystemConfig
    {
        Map<string, LocalsToken> roleSettings;
        Map<ProcessId, ProcessToken> processSettings;
        Map<string, StrategyToken> stratSettings;
        public readonly Parser<Map<string, LocalsToken>> Parser;
        Time timeout = 30*seconds;
        int maxMailboxSize = 100000;

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
            stratSettings = Map.empty<string, StrategyToken>();
        }

        /// <summary>
        /// Returns a named strategy
        /// </summary>
        public Option<State<StrategyContext, Unit>> GetStrategy(string name) =>
            stratSettings.Find(name).Map(x => x.Value);

        private static ProcessId RolePid(ProcessId pid) =>
            ProcessId.Top["role"].Append(pid.Skip(1));

        /// <summary>
        /// Returns the token that represents all the settings for a Process
        /// </summary>
        public Option<ProcessToken> GetProcessSettings(ProcessId pid)
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
        public Option<T> GetProcessSetting<T>(ProcessId pid, string name, string prop = "value") =>
            from t in GetProcessSettings(pid)
            from s in t.Settings.Find(name)
            from v in s.Values.Find(prop)
            // TODO: Type check
            select (T)v.Value;


        /// <summary>
        /// </summary>
        public Option<ProcessName> GetProcessRegisteredName(ProcessId pid) =>
            from x in GetProcessSettings(pid)
            from y in x.RegisteredName
            select y;

        /// <summary>
        /// Get the mailbox size for a Process.  Returns a default size if one
        /// hasn't been set in the config.
        /// </summary>
        public int GetProcessMailboxSize(ProcessId pid) =>
            (from x in GetProcessSettings(pid)
             from y in x.MailboxSize
             select y)
            .IfNone(maxMailboxSize);

        /// <summary>
        /// Get the flags for a Process.  Returns ProcessFlags.Default if none
        /// have been set in the config.
        /// </summary>
        public ProcessFlags GetProcessFlags(ProcessId pid) =>
            (from x in GetProcessSettings(pid)
             from y in x.Flags
             select y)
            .IfNone(ProcessFlags.Default);

        /// <summary>
        /// Get the strategy for a Process.  Returns Process.DefaultStrategy if one
        /// hasn't been set in the config.
        /// </summary>
        public State<StrategyContext, Unit> GetProcessStrategy(ProcessId pid) =>
            (from x in GetProcessSettings(pid)
             from y in x.Strategy
             select y)
            .IfNone(Process.DefaultStrategy);

        /// <summary>
        /// Get the role wide timeout setting.  This specifies how long the timeout
        /// is for 'ask' operations.
        /// </summary>
        public Time Timeout => 
            timeout;

        /// <summary>
        /// This is the setting for how often sessions are checked for expiry, *not*
        /// the expiry time itself.  That is set on each sessionStart()
        /// </summary>
        public Time SessionTimeoutCheckFrequency =>
            GetRoleSetting<Time>("session-timeout-check").IfNone(60 * seconds);

        /// <summary>
        /// Get a named role setting
        /// </summary>
        public Option<T> GetRoleSetting<T>(string name, string prop = "value") =>
            from setting in roleSettings.Find(name)
            from valtok in setting.Values.Find(prop)
                // TODO: Type check
            select (T)valtok.Value;

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
        /// <summary>
        /// Setup using the configuration settings text
        /// </summary>
        public void InitialiseFromText(Option<string> configText) =>
            configText.Iter(ParseConfigText);

        /// <summary>
        /// Setup using the configuration settings in the file specified
        /// </summary>
        public void InitialiseFromFile(Option<string> configFilename)
        {
            (from p in configFilename.Map(Some: x => File.Exists(x) ? x : "", None: () => FindLocalConfig())
             where p != ""
             select p).Iter(LoadConfigFile);
        }

        void LoadConfigFile(string path)
        {
            ParseConfigText(File.ReadAllText(path));
        }
#endif

        void ParseConfigText(string text)
        {
            // Parse the config text
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
                                              select Tuple(id, p.SetRegisteredName(val.Name)));

            // Extract the strategy settings
            stratSettings = Map.createRange(from val in res.Reply.Result.Values
                                            where val.Spec.Args.Length > 0 && val.Spec.Args[0].Type.Tag == ArgumentTypeTag.Strategy
                                            let s = (StrategyToken)val.Values.Values.First().Value
                                            select Tuple(val.Name, s));

            roleSettings = res.Reply.Result;

            // Cache the frequently accessed
            maxMailboxSize = GetRoleSetting<int>("mailbox-size").IfNone(100000);
            timeout = GetRoleSetting<Time>("timeout").IfNone(30 * seconds);


        }
        
        Parser<Map<string,LocalsToken>> InitialiseParser(IEnumerable<FuncSpec> strategyFuncs)
        {
            var process = new[] {
                FuncSpec.Attr("pid", FieldSpec.ProcessId("value")),
                FuncSpec.Attr("flags", FieldSpec.ProcessFlags("value")),
                FuncSpec.Attr("mailbox-size", FieldSpec.Int("value")),

                FuncSpec.Attr("router-type", FieldSpec.DispatcherType("value")),
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

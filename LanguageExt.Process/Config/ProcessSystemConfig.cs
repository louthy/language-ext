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
    public class ProcessSystemConfig
    {
        Map<string, LocalsToken> globalSettings;
        Map<ProcessId, ProcessToken> processSettings;
        Map<string, StrategyToken> stratSettings;
        public readonly Parser<Map<string, LocalsToken>> Parser;

        public ProcessSystemConfig(IEnumerable<FuncSpec> strategyFuncs = null)
        {
            Parser = InitialiseParser(strategyFuncs);
            globalSettings = Map.empty<string, LocalsToken>();
            processSettings = Map.empty<ProcessId, ProcessToken>();
            stratSettings = Map.empty<string, StrategyToken>();
        }

        public Map<string, LocalsToken> GlobalSettings =>
            globalSettings;

        public Map<ProcessId, ProcessToken> ProcessSettings => 
            processSettings;

        public Map<string, StrategyToken> StratSettings =>
            stratSettings;

        string FindLocalConfig() =>
            map(Path.Combine(Directory.GetParent(Assembly.GetEntryAssembly().Location).FullName, "process.conf"),
                (string path1) => File.Exists(path1)
                    ? path1
                    : map(Path.Combine(Directory.GetParent(Directory.GetParent(Assembly.GetEntryAssembly().Location).FullName).FullName, "process.conf"),
                        (string path2) => File.Exists(path2)
                            ? path2
                            : ""));

        public void Initialise(Option<string> configFilename)
        {
            (from p in configFilename.Map(Some: x => File.Exists(x) ? x : "", None: () => FindLocalConfig())
             where p != ""
             select p).Iter(LoadConfigFile);
        }

        void LoadConfigFile(string path)
        {
            var res = parse(Parser, File.ReadAllText(path));
            if( res.IsFaulted || res.Reply.State.ToString().Length > 0)
            {
                throw new ProcessConfigException(res.ToString());
            }

            var settings = res.Reply.Result;

            // Some global settings first
            // TODO - remove the static ActorSystemConfig class and 
            //        bring all settings in here
            ActorSystemConfig.Default.Timeout                      = GetSingleArg<Time>(settings, "timeout", "value").IfNone(ActorSystemConfig.Default.Timeout);
            ActorSystemConfig.Default.SessionTimeoutCheckFrequency = GetSingleArg<Time>(settings, "session-timeout-check", "value").IfNone(ActorSystemConfig.Default.SessionTimeoutCheckFrequency);
            ActorSystemConfig.Default.MaxMailboxSize               = GetSingleArg<int>(settings, "mailbox-size", "value").IfNone(ActorSystemConfig.Default.MaxMailboxSize);

            // Load process settings
            processSettings = Map.createRange(from val in settings.Values
                                              where val.Spec.Args.Length > 0 && val.Spec.Args[0].Type.Tag == ArgumentTypeTag.Process
                                              let p = (ProcessToken)val.Values.Values.First().Value
                                              where p.ProcessId.IsSome
                                              let id = p.ProcessId.IfNone(ProcessId.None)
                                              select Tuple(id, p));

            stratSettings = Map.createRange(from val in settings.Values
                                            where val.Spec.Args.Length > 0 && val.Spec.Args[0].Type.Tag == ArgumentTypeTag.Strategy
                                            let s = (StrategyToken)val.Values.Values.First().Value
                                            select Tuple(val.Name, s));

            globalSettings = settings;

        }

        static Option<T> GetSingleArg<T>(Map<string,LocalsToken> settings, string name, string argName) =>
            from y in settings.Find(name).Map(tok => tok.Values.Find(argName).Map(x => (T)x.Value))
            from z in y
            select z;

        Parser<Map<string,LocalsToken>> InitialiseParser(IEnumerable<FuncSpec> strategyFuncs)
        {
            var process = new[] {
                FuncSpec.Attr("pid", FieldSpec.ProcessId("value")),
                FuncSpec.Attr("flags", FieldSpec.ProcessFlags("value")),
                FuncSpec.Attr("mailbox-size", FieldSpec.Int("value")),
                FuncSpec.Attr("strategy", FieldSpec.Strategy("value"))
            };

            var sys = new ProcessSystemConfigParser(
                process,
                BuildStrategySpec(strategyFuncs)
                //FuncSpec.Attr("timeout", FieldSpec.Time("value")),
                //FuncSpec.Attr("session-timeout-check", FieldSpec.Time("value")),
                //FuncSpec.Attr("mailbox-size", FieldSpec.Int("value")),
                //FuncSpec.Attr("settings", FieldSpec.Int("value"))
                //FuncSpec.Attr("strategies", FieldSpec.Map("value", ArgumentType.Strategy)),
                //FuncSpec.Attr("processes", FieldSpec.Array("value", ArgumentType.Process))
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

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
    public class ProcessSystemConfigManager
    {
        Map<ProcessId, ProcessSettings> processSettings;
        Map<string, StrategySettings> stratSettings;
        readonly Parser<Map<string, SettingToken>> parser;

        public ProcessSystemConfigManager()
        {
            parser = InitialiseParser();
            processSettings = Map.empty<ProcessId, ProcessSettings>();
            stratSettings = Map.empty<string, StrategySettings>();
        }

        public Map<ProcessId, ProcessSettings> ProcessSettings => 
            processSettings;

        public Map<string, StrategySettings> StratSettings =>
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
            var res = parse(parser, File.ReadAllText(path));
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
            processSettings = GetSingleArg<Lst<ProcessSettings>>(settings, "processes", "value")
                .MapT((ProcessSettings x) =>
                    x.ProcessId.Match(
                        pid  => Tuple(pid, x),
                        () => raise<Tuple<ProcessId,ProcessSettings>>(new ProcessConfigException("A process in the configuration file is missing the 'pid' attribute"))
                    ))
                .Map(x => Map.createRange(x))
                .IfNone(Map.empty<ProcessId, ProcessSettings>());

            // Load strategy settings
            stratSettings = GetSingleArg<Map<string, StrategySettings>>(settings, "strategies", "value")
                .IfNone(Map.empty<string, StrategySettings>());

        }

        static Option<T> GetSingleArg<T>(Map<string,SettingToken> settings, string name, string argName) =>
            from y in settings.Find(name).Map(tok => tok.Values.Find(argName).Map(x => (T)x.Value))
            from z in y
            select z;

        Parser<Map<string,SettingToken>> InitialiseParser()
        {
            var strategy = new[] {
                SettingSpec.Attr("always", settings => Strategy.Always((Directive)settings["value"].Value),  ArgumentSpec.Directive("value")),

                SettingSpec.Attr("pause", settings => Strategy.Pause((Time)settings["duration"].Value), ArgumentSpec.Time("duration")),

                SettingSpec.Attr("retries-in",
                    settings => Strategy.Retries((int)settings["count"].Value,(Time)settings["duration"].Value),
                        ArgumentSpec.Int("count"), ArgumentSpec.Time("duration")),

                SettingSpec.Attr("retries", settings => Strategy.Retries((int)settings["count"].Value), ArgumentSpec.Int("count")),

                SettingSpec.Attr("back-off",
                    new ArgumentsSpec(
                        settings => Strategy.Backoff((Time)settings["min"].Value,(Time)settings["max"].Value,(Time)settings["step"].Value),
                        ArgumentSpec.Time("min"), ArgumentSpec.Time("max"), ArgumentSpec.Time("step")
                        ),

                    new ArgumentsSpec(
                        settings => Strategy.Backoff((Time)settings["duration"].Value),
                        ArgumentSpec.Time("duration")
                        )),

                SettingSpec.AttrNoArgs("match"),

                SettingSpec.AttrNoArgs("redirect")
            };

            var process = ArgumentType.Process(
                SettingSpec.Attr("pid", ArgumentSpec.ProcessId("value")),
                SettingSpec.Attr("flags", ArgumentSpec.ProcessFlags("value")),
                SettingSpec.Attr("mailbox-size", ArgumentSpec.Int("value")),
                SettingSpec.Attr("strategy", ArgumentSpec.Strategy("value", strategy)));

            var sys = new ProcessSystemConfigParser(
                SettingSpec.Attr("timeout", ArgumentSpec.Time("value")),
                SettingSpec.Attr("session-timeout-check", ArgumentSpec.Time("value")),
                SettingSpec.Attr("mailbox-size", ArgumentSpec.Int("value")),
                SettingSpec.Attr("settings", ArgumentSpec.Int("value")),
                SettingSpec.Attr("strategies", ArgumentSpec.Map("value", ArgumentType.Strategy(strategy))),
                SettingSpec.Attr("processes", ArgumentSpec.Array("value", process)));

            return sys.Settings;
        }
    }
}

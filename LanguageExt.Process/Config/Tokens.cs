using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LanguageExt.UnitsOfMeasure;
using LanguageExt.Trans;

namespace LanguageExt
{
    public class ProcessConfigToken
    { }

    public class SettingValue
    {
        public readonly string Name;
        public readonly ArgumentType Type;
        public readonly object Value;

        public SettingValue(string name, ArgumentType type, object value)
        {
            Name = name;
            Type = type;
            Value = value;
        }

        public static SettingValue Int(string name, int value) =>
            new SettingValue(name, ArgumentType.Int, value);

        public static SettingValue Double(string name, double value) =>
            new SettingValue(name, ArgumentType.Double, value);

        public static SettingValue String(string name, string value) =>
            new SettingValue(name, ArgumentType.String, value);

        public static SettingValue Time(string name, Time value) =>
            new SettingValue(name, ArgumentType.Time, value);

        public static SettingValue ProcessId(string name, ProcessId value) =>
            new SettingValue(name, ArgumentType.ProcessId, value);

        public static SettingValue ProcessName(string name, ProcessName value) =>
            new SettingValue(name, ArgumentType.ProcessName, value);

        public static SettingValue Array(string name, object value, ArgumentType genericType) =>
            new SettingValue(name, ArgumentType.Array(genericType), value);

        public static SettingValue ProcessFlags(string name, ProcessFlags value) =>
            new SettingValue(name, ArgumentType.ProcessFlags, value);

        public static SettingValue Directive(string name, Directive value) =>
            new SettingValue(name, ArgumentType.Directive, value);

        public static SettingValue Process(string name, SettingSpec[] spec,  Lst<SettingToken> values) =>
            new SettingValue(name, ArgumentType.Process(spec), new ProcessSettings(values));

        public static SettingValue Strategy(string name, SettingSpec[] spec, string type, Lst<SettingToken> values) =>
            new SettingValue(name, ArgumentType.Strategy(spec), new StrategySettings(type, values));

        public static SettingValue StrategyMatch(string name, State<StrategyContext, Unit> value) =>
            new SettingValue(name, ArgumentType.StrategyMatch, value);

        public static SettingValue StrategyRedirect(string name, State<StrategyContext, Unit> value) =>
            new SettingValue(name, ArgumentType.StrategyRedirect, value);
    }

    public class StrategySettings
    {
        public readonly string Type;
        public readonly State<StrategyContext, Unit> Value;

        public StrategySettings(string type, Lst<SettingToken> values)
        {
            Type = type;

            var instrs = values.Map(x =>
                (State<StrategyContext, Unit>)
                    (x.Spec == null
                        ? x.Values.Values.Head().Value  // TODO: hacky
                        : x.Spec.Build(x.Values))
                ).ToArray();

            Value = type == "one-for-one"
                ? Strategy.OneForOne(instrs)
                : Strategy.AllForOne(instrs);
        }
    }

    public class ProcessSettings
    {
        public readonly Option<ProcessId> ProcessId;
        public readonly Option<ProcessFlags> Flags;
        public readonly Option<int> MailboxSize;
        public readonly Option<State<StrategyContext, Unit>> Strategy;
        public readonly Map<string, SettingToken> Settings;

        public ProcessSettings(Lst<SettingToken> values)
        {
            Settings = Map.createRange(values.Map(x => Tuple.Create(x.Name, x)));

            ProcessId   = GetValue<ProcessId>("pid");
            Flags       = GetValue<ProcessFlags>("flags");
            MailboxSize = GetValue<int>("mailbox-size");
            Strategy    = GetValue<StrategySettings>("strategy").Map(x => x.Value);
        }

        Option<T> GetValue<T>(string name) =>
            from y in Settings.Find(name).Map(tok => tok.Values.Find("value").Map(x => (T)x.Value))
            from z in y
            select z;
    }

    public class SettingToken : ProcessConfigToken
    {
        public readonly string Name;
        public readonly ArgumentsSpec Spec;
        public readonly Map<string, SettingValue> Values;

        public SettingToken(string name, ArgumentsSpec spec, params SettingValue[] values)
        {
            Name = name;
            Spec = spec;
            Values = Map.createRange(values.Map(x => Tuple.Create(x.Name, x)));
        }
    }


    public class PidToken : ProcessConfigToken
    {
        public readonly ProcessId Pid;
        public PidToken(ProcessId pid)
        {
            Pid = pid;
        }
    }

    public class FlagsToken : ProcessConfigToken
    {
        public readonly ProcessFlags Flags;
        public FlagsToken(ProcessFlags flags)
        {
            Flags = flags;
        }
    }

    public class MailboxSizeToken : ProcessConfigToken
    {
        public readonly int Size;
        public MailboxSizeToken(int size)
        {
            Size = size;
        }
    }

    public class StrategyToken : ProcessConfigToken
    {
        public readonly State<StrategyContext, Unit> Strategy;
        public StrategyToken(State<StrategyContext, Unit> strategy)
        {
            Strategy = strategy;
        }
    }

    public class SettingsToken : ProcessConfigToken
    {
        public readonly Map<string, string> Settings;

        public SettingsToken(Map<string, string> settings)
        {
            Settings = settings;
        }
    }
}

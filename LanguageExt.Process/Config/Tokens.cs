using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LanguageExt.UnitsOfMeasure;
using LanguageExt.Trans;

namespace LanguageExt
{
    public class ValueToken
    {
        public readonly string Name;
        public readonly ArgumentType Type;
        public readonly object Value;

        public ValueToken(string name, ArgumentType type, object value)
        {
            Name = name;
            Type = type;
            Value = value;
        }

        public static ValueToken Int(string name, int value) =>
            new ValueToken(name, ArgumentType.Int, value);

        public static ValueToken Double(string name, double value) =>
            new ValueToken(name, ArgumentType.Double, value);

        public static ValueToken String(string name, string value) =>
            new ValueToken(name, ArgumentType.String, value);

        public static ValueToken Time(string name, Time value) =>
            new ValueToken(name, ArgumentType.Time, value);

        public static ValueToken ProcessId(string name, ProcessId value) =>
            new ValueToken(name, ArgumentType.ProcessId, value);

        public static ValueToken ProcessName(string name, ProcessName value) =>
            new ValueToken(name, ArgumentType.ProcessName, value);

        public static ValueToken Array(string name, object value, ArgumentType genericType) =>
            new ValueToken(name, ArgumentType.Array(genericType), value);

        public static ValueToken Map(string name, object value, ArgumentType genericType) =>
            new ValueToken(name, ArgumentType.Map(genericType), value);

        public static ValueToken ProcessFlags(string name, ProcessFlags value) =>
            new ValueToken(name, ArgumentType.ProcessFlags, value);

        public static ValueToken Directive(string name, Directive value) =>
            new ValueToken(name, ArgumentType.Directive, value);

        public static ValueToken Process(string name, Lst<LocalsToken> values) =>
            new ValueToken(name, ArgumentType.Process, new ProcessToken(values));

        public static ValueToken Strategy(string name, string type, Lst<LocalsToken> values) =>
            new ValueToken(name, ArgumentType.Strategy, new StrategyToken(type, values));

        public static ValueToken Strategy(string name, string named) =>
            new ValueToken(name, ArgumentType.Strategy, new StrategyToken(named));

        public static ValueToken StrategyMatch(string name, State<StrategyContext, Unit> value) =>
            new ValueToken(name, ArgumentType.StrategyMatch, value);

        public static ValueToken StrategyRedirect(string name, State<StrategyContext, Unit> value) =>
            new ValueToken(name, ArgumentType.StrategyRedirect, value);

        public ValueToken SetName(string name) =>
            new ValueToken(name, Type, Value);

        public static ValueToken Negate(ValueToken tok)
        {
            switch(tok.Type.Tag)
            {
                case ArgumentTypeTag.Array: throw new InvalidOperationException("Can't negate an array");
                case ArgumentTypeTag.Directive: throw new InvalidOperationException("Can't negate a directive");
                case ArgumentTypeTag.Double: return new ValueToken(tok.Name, tok.Type, -((double)tok.Value));
                case ArgumentTypeTag.Int: return new ValueToken(tok.Name, tok.Type, -((int)tok.Value));
                case ArgumentTypeTag.Map: throw new InvalidOperationException("Can't negate a map");
                case ArgumentTypeTag.Process: throw new InvalidOperationException("Can't negate a process");
                case ArgumentTypeTag.ProcessFlags: throw new InvalidOperationException("Can't negate process-flags");
                case ArgumentTypeTag.ProcessId: throw new InvalidOperationException("Can't negate a process-id");
                case ArgumentTypeTag.ProcessName: throw new InvalidOperationException("Can't negate a process-name");
                case ArgumentTypeTag.Strategy: throw new InvalidOperationException("Can't negate a strategy");
                case ArgumentTypeTag.StrategyMatch: throw new InvalidOperationException("Can't negate a strategy match");
                case ArgumentTypeTag.StrategyRedirect: throw new InvalidOperationException("Can't negate a strategy redirect");
                case ArgumentTypeTag.String: throw new InvalidOperationException("Can't negate a string");
                case ArgumentTypeTag.Time: throw new InvalidOperationException("You can't turn back time");
                default: throw new InvalidOperationException("Can't negate an unknown type");
            }
        }

        public static ValueToken Incr(ValueToken tok)
        {
            switch (tok.Type.Tag)
            {
                case ArgumentTypeTag.Array: throw new InvalidOperationException("Can't increment an array");
                case ArgumentTypeTag.Directive: throw new InvalidOperationException("Can't increment a directive");
                case ArgumentTypeTag.Double: return new ValueToken(tok.Name, tok.Type, -((double)tok.Value));
                case ArgumentTypeTag.Int: return new ValueToken(tok.Name, tok.Type, -((int)tok.Value));
                case ArgumentTypeTag.Map: throw new InvalidOperationException("Can't increment a map");
                case ArgumentTypeTag.Process: throw new InvalidOperationException("Can't increment a process");
                case ArgumentTypeTag.ProcessFlags: throw new InvalidOperationException("Can't increment process-flags");
                case ArgumentTypeTag.ProcessId: throw new InvalidOperationException("Can't increment a process-id");
                case ArgumentTypeTag.ProcessName: throw new InvalidOperationException("Can't increment a process-name");
                case ArgumentTypeTag.Strategy: throw new InvalidOperationException("Can't increment a strategy");
                case ArgumentTypeTag.StrategyMatch: throw new InvalidOperationException("Can't increment a strategy match");
                case ArgumentTypeTag.StrategyRedirect: throw new InvalidOperationException("Can't increment a strategy redirect");
                case ArgumentTypeTag.String: throw new InvalidOperationException("Can't increment a string");
                case ArgumentTypeTag.Time: throw new InvalidOperationException("Can't increment a time");
                default: throw new InvalidOperationException("Can't increment an unknown type");
            }
        }

        public static ValueToken Mul(ValueToken lhs, ValueToken rhs)
        {
            if( lhs.Type.Tag != rhs.Type.Tag )
            {
                throw new InvalidOperationException("Can't multiply a " + lhs.Type.Tag + " and a " + rhs.Type.Tag);
            }

            switch (lhs.Type.Tag)
            {
                case ArgumentTypeTag.Array: throw new InvalidOperationException("Can't multiply an array");
                case ArgumentTypeTag.Directive: throw new InvalidOperationException("Can't multiply a directive");
                case ArgumentTypeTag.Map: throw new InvalidOperationException("Can't multiply a map");
                case ArgumentTypeTag.Process: throw new InvalidOperationException("Can't multiply a process");
                case ArgumentTypeTag.ProcessFlags: throw new InvalidOperationException("Can't multiply process-flags");
                case ArgumentTypeTag.ProcessId: throw new InvalidOperationException("Can't multiply a process-id");
                case ArgumentTypeTag.ProcessName: throw new InvalidOperationException("Can't multiply a process-name");
                case ArgumentTypeTag.Strategy: throw new InvalidOperationException("Can't multiply a strategy");
                case ArgumentTypeTag.StrategyMatch: throw new InvalidOperationException("Can't multiply a strategy match");
                case ArgumentTypeTag.StrategyRedirect: throw new InvalidOperationException("Can't multiply a strategy redirect");
                case ArgumentTypeTag.String: throw new InvalidOperationException("Can't multiply a string");
                case ArgumentTypeTag.Time: throw new InvalidOperationException("Can't multiply a time");
            }

            switch (rhs.Type.Tag)
            {
                case ArgumentTypeTag.Array: throw new InvalidOperationException("Can't multiply an array");
                case ArgumentTypeTag.Directive: throw new InvalidOperationException("Can't multiply a directive");
                case ArgumentTypeTag.Map: throw new InvalidOperationException("Can't multiply a map");
                case ArgumentTypeTag.Process: throw new InvalidOperationException("Can't multiply a process");
                case ArgumentTypeTag.ProcessFlags: throw new InvalidOperationException("Can't multiply process-flags");
                case ArgumentTypeTag.ProcessId: throw new InvalidOperationException("Can't multiply a process-id");
                case ArgumentTypeTag.ProcessName: throw new InvalidOperationException("Can't multiply a process-name");
                case ArgumentTypeTag.Strategy: throw new InvalidOperationException("Can't multiply a strategy");
                case ArgumentTypeTag.StrategyMatch: throw new InvalidOperationException("Can't multiply a strategy match");
                case ArgumentTypeTag.StrategyRedirect: throw new InvalidOperationException("Can't multiply a strategy redirect");
                case ArgumentTypeTag.String: throw new InvalidOperationException("Can't multiply a string");
                case ArgumentTypeTag.Time: throw new InvalidOperationException("Can't multiply a time");

                case ArgumentTypeTag.Double: return new ValueToken(lhs.Name, lhs.Type, ((double)lhs.Value) * ((double)rhs.Value));
                case ArgumentTypeTag.Int: return new ValueToken(lhs.Name, lhs.Type, ((int)lhs.Value) * ((int)rhs.Value));

                default: throw new InvalidOperationException("Can't multiply an unknown type");
            }
        }

        public static ValueToken Div(ValueToken lhs, ValueToken rhs)
        {
            if (lhs.Type.Tag != rhs.Type.Tag)
            {
                throw new InvalidOperationException("Can't divide a " + lhs.Type.Tag + " and a " + rhs.Type.Tag);
            }

            switch (lhs.Type.Tag)
            {
                case ArgumentTypeTag.Array: throw new InvalidOperationException("Can't divide an array");
                case ArgumentTypeTag.Directive: throw new InvalidOperationException("Can't divide a directive");
                case ArgumentTypeTag.Map: throw new InvalidOperationException("Can't divide a map");
                case ArgumentTypeTag.Process: throw new InvalidOperationException("Can't divide a process");
                case ArgumentTypeTag.ProcessFlags: throw new InvalidOperationException("Can't divide process-flags");
                case ArgumentTypeTag.ProcessId: throw new InvalidOperationException("Can't divide a process-id");
                case ArgumentTypeTag.ProcessName: throw new InvalidOperationException("Can't divide a process-name");
                case ArgumentTypeTag.Strategy: throw new InvalidOperationException("Can't divide a strategy");
                case ArgumentTypeTag.StrategyMatch: throw new InvalidOperationException("Can't divide a strategy match");
                case ArgumentTypeTag.StrategyRedirect: throw new InvalidOperationException("Can't divide a strategy redirect");
                case ArgumentTypeTag.String: throw new InvalidOperationException("Can't divide a string");
                case ArgumentTypeTag.Time: throw new InvalidOperationException("Can't divide a time");
            }

            switch (rhs.Type.Tag)
            {
                case ArgumentTypeTag.Array: throw new InvalidOperationException("Can't divide an array");
                case ArgumentTypeTag.Directive: throw new InvalidOperationException("Can't divide a directive");
                case ArgumentTypeTag.Map: throw new InvalidOperationException("Can't divide a map");
                case ArgumentTypeTag.Process: throw new InvalidOperationException("Can't divide a process");
                case ArgumentTypeTag.ProcessFlags: throw new InvalidOperationException("Can't divide process-flags");
                case ArgumentTypeTag.ProcessId: throw new InvalidOperationException("Can't divide a process-id");
                case ArgumentTypeTag.ProcessName: throw new InvalidOperationException("Can't divide a process-name");
                case ArgumentTypeTag.Strategy: throw new InvalidOperationException("Can't divide a strategy");
                case ArgumentTypeTag.StrategyMatch: throw new InvalidOperationException("Can't divide a strategy match");
                case ArgumentTypeTag.StrategyRedirect: throw new InvalidOperationException("Can't divide a strategy redirect");
                case ArgumentTypeTag.String: throw new InvalidOperationException("Can't divide a string");
                case ArgumentTypeTag.Time: throw new InvalidOperationException("Can't divide a time");

                case ArgumentTypeTag.Double: return new ValueToken(lhs.Name, lhs.Type, ((double)lhs.Value) / ((double)rhs.Value));
                case ArgumentTypeTag.Int: return new ValueToken(lhs.Name, lhs.Type, ((int)lhs.Value) / ((int)rhs.Value));

                default: throw new InvalidOperationException("Can't divide an unknown type");
            }
        }

        public static ValueToken Sub(ValueToken lhs, ValueToken rhs)
        {
            if (lhs.Type.Tag != rhs.Type.Tag)
            {
                throw new InvalidOperationException("Can't subtract a " + lhs.Type.Tag + " and a " + rhs.Type.Tag);
            }

            switch (lhs.Type.Tag)
            {
                case ArgumentTypeTag.Array: throw new InvalidOperationException("Can't subtract an array");
                case ArgumentTypeTag.Directive: throw new InvalidOperationException("Can't subtract a directive");
                case ArgumentTypeTag.Map: throw new InvalidOperationException("Can't subtract a map");
                case ArgumentTypeTag.Process: throw new InvalidOperationException("Can't subtract a process");
                case ArgumentTypeTag.ProcessFlags: throw new InvalidOperationException("Can't subtract process-flags");
                case ArgumentTypeTag.ProcessId: throw new InvalidOperationException("Can't subtract a process-id");
                case ArgumentTypeTag.ProcessName: throw new InvalidOperationException("Can't subtract a process-name");
                case ArgumentTypeTag.Strategy: throw new InvalidOperationException("Can't subtract a strategy");
                case ArgumentTypeTag.StrategyMatch: throw new InvalidOperationException("Can't subtract a strategy match");
                case ArgumentTypeTag.StrategyRedirect: throw new InvalidOperationException("Can't subtract a strategy redirect");
                case ArgumentTypeTag.String: throw new InvalidOperationException("Can't subtract a string");
            }

            switch (rhs.Type.Tag)
            {
                case ArgumentTypeTag.Array: throw new InvalidOperationException("Can't subtract an array");
                case ArgumentTypeTag.Directive: throw new InvalidOperationException("Can't subtract a directive");
                case ArgumentTypeTag.Map: throw new InvalidOperationException("Can't subtract a map");
                case ArgumentTypeTag.Process: throw new InvalidOperationException("Can't subtract a process");
                case ArgumentTypeTag.ProcessFlags: throw new InvalidOperationException("Can't subtract process-flags");
                case ArgumentTypeTag.ProcessId: throw new InvalidOperationException("Can't subtract a process-id");
                case ArgumentTypeTag.ProcessName: throw new InvalidOperationException("Can't subtract a process-name");
                case ArgumentTypeTag.Strategy: throw new InvalidOperationException("Can't subtract a strategy");
                case ArgumentTypeTag.StrategyMatch: throw new InvalidOperationException("Can't subtract a strategy match");
                case ArgumentTypeTag.StrategyRedirect: throw new InvalidOperationException("Can't subtract a strategy redirect");
                case ArgumentTypeTag.String: throw new InvalidOperationException("Can't subtract a string");

                case ArgumentTypeTag.Time: return new ValueToken(lhs.Name, lhs.Type, ((Time)lhs.Value) - ((Time)rhs.Value));
                case ArgumentTypeTag.Double: return new ValueToken(lhs.Name, lhs.Type, ((double)lhs.Value) - ((double)rhs.Value));
                case ArgumentTypeTag.Int: return new ValueToken(lhs.Name, lhs.Type, ((int)lhs.Value) - ((int)rhs.Value));

                default: throw new InvalidOperationException("Can't subtract an unknown type");
            }
        }

        public static ValueToken Add(ValueToken lhs, ValueToken rhs)
        {
            if (lhs.Type.Tag != rhs.Type.Tag)
            {
                throw new InvalidOperationException("Can't add a " + lhs.Type.Tag + " and a " + rhs.Type.Tag);
            }

            switch (lhs.Type.Tag)
            {
                case ArgumentTypeTag.Array: throw new InvalidOperationException("Can't add an array");
                case ArgumentTypeTag.Directive: throw new InvalidOperationException("Can't add a directive");
                case ArgumentTypeTag.Map: throw new InvalidOperationException("Can't add a map");
                case ArgumentTypeTag.Process: throw new InvalidOperationException("Can't add a process");
                case ArgumentTypeTag.ProcessFlags: throw new InvalidOperationException("Can't add process-flags");
                case ArgumentTypeTag.ProcessId: throw new InvalidOperationException("Can't add a process-id");
                case ArgumentTypeTag.ProcessName: throw new InvalidOperationException("Can't add a process-name");
                case ArgumentTypeTag.Strategy: throw new InvalidOperationException("Can't add a strategy");
                case ArgumentTypeTag.StrategyMatch: throw new InvalidOperationException("Can't add a strategy match");
                case ArgumentTypeTag.StrategyRedirect: throw new InvalidOperationException("Can't add a strategy redirect");
            }

            switch (rhs.Type.Tag)
            {
                case ArgumentTypeTag.Array: throw new InvalidOperationException("Can't add an array");
                case ArgumentTypeTag.Directive: throw new InvalidOperationException("Can't add a directive");
                case ArgumentTypeTag.Map: throw new InvalidOperationException("Can't add a map");
                case ArgumentTypeTag.Process: throw new InvalidOperationException("Can't add a process");
                case ArgumentTypeTag.ProcessFlags: throw new InvalidOperationException("Can't add process-flags");
                case ArgumentTypeTag.ProcessId: throw new InvalidOperationException("Can't add a process-id");
                case ArgumentTypeTag.ProcessName: throw new InvalidOperationException("Can't add a process-name");
                case ArgumentTypeTag.Strategy: throw new InvalidOperationException("Can't add a strategy");
                case ArgumentTypeTag.StrategyMatch: throw new InvalidOperationException("Can't add a strategy match");
                case ArgumentTypeTag.StrategyRedirect: throw new InvalidOperationException("Can't add a strategy redirect");

                case ArgumentTypeTag.String: return new ValueToken(lhs.Name, lhs.Type, ((string)lhs.Value) + ((string)rhs.Value));
                case ArgumentTypeTag.Time: return new ValueToken(lhs.Name, lhs.Type, ((Time)lhs.Value) + ((Time)rhs.Value));
                case ArgumentTypeTag.Double: return new ValueToken(lhs.Name, lhs.Type, ((double)lhs.Value) + ((double)rhs.Value));
                case ArgumentTypeTag.Int: return new ValueToken(lhs.Name, lhs.Type, ((int)lhs.Value) + ((int)rhs.Value));

                default: throw new InvalidOperationException("Can't add an unknown type");
            }
        }
    }

    public class StrategyToken
    {
        public readonly string Type;
        public readonly string NamedStrategy;
        public readonly State<StrategyContext, Unit> Value;

        public StrategyToken(string named)
        {
            Type = "named";
            NamedStrategy = named;
        }

        public StrategyToken(string type, Lst<LocalsToken> values)
        {
            Type = type;

            var instrs = values.Map(x =>
                (State<StrategyContext, Unit>)
                    (x.Spec == null
                        ? x.Values.Values.Head().Value  // TODO: hacky
                        : x.Spec.Ctor(x.Values))
                ).ToArray();

            Value = type == "one-for-one"
                ? Strategy.OneForOne(instrs)
                : Strategy.AllForOne(instrs);
        }
    }

    public class ProcessToken
    {
        public readonly Option<ProcessId> ProcessId;
        public readonly Option<ProcessFlags> Flags;
        public readonly Option<int> MailboxSize;
        public readonly Option<State<StrategyContext, Unit>> Strategy;
        public readonly Option<string> NamedStrategy;
        public readonly Map<string, LocalsToken> Settings;

        public ProcessToken(Lst<LocalsToken> values)
        {
            Settings = Map.createRange(values.Map(x => Tuple.Create(x.Name, x)));

            ProcessId     = GetValue<ProcessId>("pid");
            Flags         = GetValue<ProcessFlags>("flags");
            MailboxSize   = GetValue<int>("mailbox-size");
            Strategy      = GetValue<StrategyToken>("strategy").Map(x => x.Value);
            NamedStrategy = GetValue<StrategyToken>("strategy").Map(x => x.NamedStrategy);
        }

        Option<T> GetValue<T>(string name) =>
            from y in Settings.Find(name).Map(tok => tok.Values.Find("value").Map(x => (T)x.Value))
            from z in y
            select z;
    }

    public class LocalsToken
    {
        public readonly string Name;
        public readonly ArgumentsSpec Spec;
        public readonly Map<string, ValueToken> Values;

        public LocalsToken(string name, ArgumentsSpec spec, params ValueToken[] values)
        {
            Name = name;
            Spec = spec;
            Values = Map.createRange(values.Map(x => Tuple.Create(x.Name, x)));
        }
    }
}

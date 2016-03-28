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

        public static ValueToken StrategyMatch(string name, State<StrategyContext, Unit> value) =>
            new ValueToken(name, ArgumentType.StrategyMatch, value);

        public static ValueToken StrategyRedirect(string name, State<StrategyContext, Unit> value) =>
            new ValueToken(name, ArgumentType.StrategyRedirect, value);

        public static ValueToken DispatcherType(string name, string type) =>
            new ValueToken(name, ArgumentType.DispatcherType, type);

        public static ValueToken Dispatcher(string name, string type, Lst<ProcessToken> processes) =>
            new ValueToken(
                name, 
                ArgumentType.Dispatcher, 
                LanguageExt.ProcessId.Top["disp"][type][processes.Map(x => x.ProcessId.IfNone(LanguageExt.ProcessId.None))]
            );

        public static ValueToken Role(string name, string type, ProcessToken process) =>
            new ValueToken(
                name,
                ArgumentType.Dispatcher,
                LanguageExt.ProcessId.Top["disp"]["role"][type]["user"].Append(process.ProcessId.IfNone(LanguageExt.ProcessId.None))
            );

        public ValueToken SetName(string name) =>
            new ValueToken(name, Type, Value);

        public static ValueToken Negate(ValueToken tok)
        {
            switch(tok.Type.Tag)
            {
                case ArgumentTypeTag.Double: return new ValueToken(tok.Name, tok.Type, -((double)tok.Value));
                case ArgumentTypeTag.Int: return new ValueToken(tok.Name, tok.Type, -((int)tok.Value));
                default: throw new InvalidOperationException($"Can't negate {ArgumentTypeTag.Array} type");
            }
        }

        public static ValueToken Incr(ValueToken tok)
        {
            switch (tok.Type.Tag)
            {
                case ArgumentTypeTag.Double: return new ValueToken(tok.Name, tok.Type, ((double)tok.Value)+1);
                case ArgumentTypeTag.Int: return new ValueToken(tok.Name, tok.Type, ((int)tok.Value) + 1);
                default: throw new InvalidOperationException($"Can't increment {ArgumentTypeTag.Array} type");
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
                case ArgumentTypeTag.Double: return new ValueToken(lhs.Name, lhs.Type, ((double)lhs.Value) * ((double)rhs.Value));
                case ArgumentTypeTag.Int: return new ValueToken(lhs.Name, lhs.Type, ((int)lhs.Value) * ((int)rhs.Value));

                default: throw new InvalidOperationException($"Can't multiply {ArgumentTypeTag.Array} type");
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
                case ArgumentTypeTag.Double: return new ValueToken(lhs.Name, lhs.Type, ((double)lhs.Value) / ((double)rhs.Value));
                case ArgumentTypeTag.Int: return new ValueToken(lhs.Name, lhs.Type, ((int)lhs.Value) / ((int)rhs.Value));

                default: throw new InvalidOperationException($"Can't divide {ArgumentTypeTag.Array} type");
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
                case ArgumentTypeTag.Time: return new ValueToken(lhs.Name, lhs.Type, ((Time)lhs.Value) - ((Time)rhs.Value));
                case ArgumentTypeTag.Double: return new ValueToken(lhs.Name, lhs.Type, ((double)lhs.Value) - ((double)rhs.Value));
                case ArgumentTypeTag.Int: return new ValueToken(lhs.Name, lhs.Type, ((int)lhs.Value) - ((int)rhs.Value));

                default: throw new InvalidOperationException($"Can't subtract {ArgumentTypeTag.Array} type");
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
                case ArgumentTypeTag.String: return new ValueToken(lhs.Name, lhs.Type, ((string)lhs.Value) + ((string)rhs.Value));
                case ArgumentTypeTag.Time: return new ValueToken(lhs.Name, lhs.Type, ((Time)lhs.Value) + ((Time)rhs.Value));
                case ArgumentTypeTag.Double: return new ValueToken(lhs.Name, lhs.Type, ((double)lhs.Value) + ((double)rhs.Value));
                case ArgumentTypeTag.Int: return new ValueToken(lhs.Name, lhs.Type, ((int)lhs.Value) + ((int)rhs.Value));

                default: throw new InvalidOperationException($"Can't add {ArgumentTypeTag.Array} type");
            }
        }
    }

    public class StrategyToken
    {
        public readonly string Type;
        public readonly State<StrategyContext, Unit> Value;

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
        public readonly Map<string, LocalsToken> Settings;
        public readonly Option<ProcessName> RegisteredName;
        public readonly Option<string> Dispatch;
        public readonly Option<string> Route;
        public readonly Option<Lst<ProcessToken>> Workers;
        public readonly Option<int> WorkerCount;
        public readonly Option<string> WorkerName;

        public ProcessToken(Lst<LocalsToken> values)
        {
            Settings        = Map.createRange(values.Map(x => Tuple.Create(x.Name, x)));
            ProcessId       = GetValue<ProcessId>("pid");
            Flags           = GetValue<ProcessFlags>("flags");
            MailboxSize     = GetValue<int>("mailbox-size");
            Dispatch        = GetValue<string>("dispatch");
            Route           = GetValue<string>("route");
            RegisteredName  = GetValue<ProcessName>("register-as");
            Strategy        = GetValue<StrategyToken>("strategy").Map(x => x.Value);
            Workers         = GetValue<Lst<ProcessToken>>("workers");
            WorkerCount     = GetValue<int>("worker-count");
            WorkerName      = GetValue<string>("worker-name");
        }

        ProcessToken(
            Option<ProcessId> processId,
            Option<ProcessFlags> flags,
            Option<int> mailboxSize,
            Option<State<StrategyContext, Unit>> strategy,
            Map<string, LocalsToken> settings,
            Option<ProcessName> registeredName,
            Option<string> dispatch,
            Option<string> route,
            Option<Lst<ProcessToken>> workers,
            Option<int> workerCount,
            Option<string> workerName
            )
        {
            Settings = settings;
            ProcessId = processId;
            Flags = flags;
            MailboxSize = mailboxSize;
            Strategy = strategy;
            RegisteredName = registeredName;
            Dispatch = dispatch;
            Route = route;
            Workers = workers;
            WorkerCount = workerCount;
            WorkerName = workerName;
        }

        public ProcessToken SetRegisteredName(ProcessName registeredName) =>
            new ProcessToken(ProcessId, Flags, MailboxSize, Strategy, Settings, registeredName, Dispatch, Route, Workers, WorkerCount, WorkerName);

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

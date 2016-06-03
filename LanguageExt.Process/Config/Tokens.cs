using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LanguageExt.UnitsOfMeasure;
using LanguageExt.Trans;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExt.Config
{
    public class ValueToken
    {
        public readonly TypeDef Type;
        public readonly object Value;

        public ValueToken(TypeDef type, object value)
        {
            Type = type;
            Value = value;
        }

        public static ValueToken Array(object value, TypeDef genericType) =>
            new ValueToken(TypeDef.Array(() => genericType), value);

        public static ValueToken Map(object value, TypeDef genericType) =>
            new ValueToken(TypeDef.Map(() => genericType), value);

        public static Func<string, Func<ValueToken, ValueToken, ValueToken>> BinaryOp =
            (string op) =>
                (ValueToken lhs, ValueToken rhs) =>
                    lhs.Type.BinaryOperator(op, lhs, rhs);

        public static Func<string, Func<ValueToken, ValueToken>> PrefixOp =
            (string op) =>
                (ValueToken rhs) =>
                    rhs.Type.PrefixOperator(op, rhs);

        public static Func<string, Func<ValueToken, ValueToken>> PostfixOp =
            (string op) =>
                (ValueToken lhs) =>
                    lhs.Type.PostfixOperator(op, lhs);

        public Lst<NamedValueToken> ToList() =>
            (Lst<NamedValueToken>)Value;

        public Lst<T> ToList<T>() =>
            ToList().Map(nv => (T)nv.Value.Value);

        Map<string, ValueToken> mapCache = null;
        public Map<string, ValueToken> ToMap()
        {
            if (mapCache == null)
            {
                mapCache = LanguageExt.Map.createRange(
                    ToList().Map(nv => Tuple(nv.Name, nv.Value))
                );
            }
            return mapCache;
        }

        public Option<ValueToken> GetItem(string name) =>
            ToList().Filter(nv => nv.Name == name).Map(nv => nv.Value).HeadOrNone();

        public Option<T> GetItem<T>(string name) =>
            ToList().Filter(nv => nv.Name == name).Map(nv => (T)nv.Value.Value).HeadOrNone();

        public ValueToken AddItem(string name, ValueToken val) =>
            new ValueToken(Type, ToList().Add(new NamedValueToken(name, val)));

        public T Cast<T>() => (T)Value;
             
    }

    public class NamedValueToken
    {
        public readonly string Name;
        public readonly ValueToken Value;

        public NamedValueToken(string name, ValueToken value)
        {
            Name = name;
            Value = value;
        }

        public NamedValueToken AddItem(string name, ValueToken val) =>
            new NamedValueToken(Name, Value.AddItem(name,val));
    }

    public class ProcessToken
    {
        public readonly Option<ProcessId> ProcessId;
        public readonly Option<ProcessFlags> Flags;
        public readonly Option<int> MailboxSize;
        public readonly Option<State<StrategyContext, Unit>> Strategy;
        public readonly Map<string, ValueToken> Settings;
        public readonly Option<ProcessName> RegisteredName;
        public readonly Option<string> Dispatch;
        public readonly Option<string> Route;
        public readonly Option<Lst<ProcessToken>> Workers;
        public readonly Option<int> WorkerCount;
        public readonly Option<string> WorkerName;

        public ProcessToken(Lst<NamedValueToken> values)
        {
            Settings        = Map.createRange(values.Map(x => Tuple(x.Name, x.Value)));
            ProcessId       = GetValue<ProcessId>("pid");
            Flags           = GetValue<ProcessFlags>("flags");
            MailboxSize     = GetValue<int>("mailbox-size");
            Dispatch        = GetValue<string>("dispatch");
            Route           = GetValue<string>("route");
            RegisteredName  = GetValue<ProcessName>("register-as");
            Strategy        = GetValue<State<StrategyContext, Unit>>("strategy");
            Workers         = GetValue<Lst<ProcessToken>>("workers");
            WorkerCount     = GetValue<int>("worker-count");
            WorkerName      = GetValue<string>("worker-name");
        }

        ProcessToken(
            Option<ProcessId> processId,
            Option<ProcessFlags> flags,
            Option<int> mailboxSize,
            Option<State<StrategyContext, Unit>> strategy,
            Map<string, ValueToken> settings,
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

        public ProcessToken SetRegisteredName(ValueToken registeredName) =>
            new ProcessToken(
                ProcessId, 
                Flags, 
                MailboxSize, 
                Strategy,
                Settings.AddOrUpdate("register-as", registeredName),
                (ProcessName)registeredName.Value, 
                Dispatch, 
                Route, 
                Workers, 
                WorkerCount, 
                WorkerName
            );

        Option<T> GetValue<T>(string name) =>
           Settings.Find(name).Map(tok => tok.Cast<T>());
    }

    public class ClusterToken
    {
        public readonly Map<string, ValueToken> Settings;
        public readonly Option<string> NodeName;
        public readonly Option<string> Role;
        public readonly Option<string> Connection;
        public readonly Option<string> Database;
        public readonly Option<string> Env;
        public readonly Option<string> UserEnv;
        public readonly bool Default;

        public ClusterToken(Lst<NamedValueToken> values)
        {
            Settings = Map.createRange(values.Map(x => Tuple(x.Name, x.Value)));
            NodeName = GetValue<string>("node-name");
            Role = GetValue<string>("role");
            Connection = GetValue<string>("connection");
            Database = GetValue<string>("database");
            Env = GetValue<string>("env");
            UserEnv = GetValue<string>("user-env");
            Default = GetValue<bool>("default").IfNone(false);

            if (NodeName.IsNone) throw new Exception("cluster requires a 'node-name' attribute");
            if (Role.IsNone) throw new Exception("cluster requires a 'role' attribute");
            if (Connection.IsNone) throw new Exception("cluster requires a 'connection' attribute");
            if (Database.IsNone) throw new Exception("cluster requires a 'database' attribute");
        }

        ClusterToken(
            Map<string, ValueToken> settings,
            Option<string> nodeName,
            Option<string> role,
            Option<string> connection,
            Option<string> database,
            Option<string> env,
            Option<string> userEnv
            )
        {
            Settings = settings;
            NodeName = nodeName;
            Role = role;
            Connection = connection;
            Database = database;
            Env = env;
            UserEnv = userEnv;
        }

        public ClusterToken SetEnvironment(ValueToken env) =>
            new ClusterToken(
                Settings.AddOrUpdate("env", env),
                NodeName,
                Role,
                Connection,
                Database,
                (string)env.Value,
                UserEnv
            );

        Option<T> GetValue<T>(string name) =>
           Settings.Find(name).Map(tok => tok.Cast<T>());

        public readonly static ClusterToken Empty =
            new ClusterToken(Map.empty<string, ValueToken>(), None, None, None, None, None, None);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LanguageExt.Parsec;
using static LanguageExt.Prelude;
using static LanguageExt.Parsec.Char;
using static LanguageExt.Parsec.Expr;
using static LanguageExt.Parsec.Prim;
using static LanguageExt.Parsec.Token;
using static LanguageExt.Parsec.Indent;
using LanguageExt.UnitsOfMeasure;

namespace LanguageExt.Config
{
    public class ProcessSystemConfigParser
    {
        // TODO: Create a general type system, with per-type rules for parsing,
        //       usage in maps, lists, etc.  Lots of switches below would be 
        //       avoided and would make it much easier to add new types.  Perhaps
        //       even find a way of using .NET types with a wrapper that parses
        //       and instantiates.

        GenLanguageDef definition;
        GenTokenParser tokenParser;
        public Parser<Map<string, LocalsToken>> Settings;

        void InitialiseParser()
        {
            // Process config definition
            definition = GenLanguageDef.Empty.With(
                CommentStart: "(-",
                CommentEnd: "-)",
                CommentLine: "//",
                NestedComments: true,
                IdentStart: letter,
                IdentLetter: either(alphaNum, oneOf("-_")),
                ReservedNames: List(
                    "pid", "strategy", "flags", "one-for-one", "all-for-one",
                    "retries", "backoff", "always", "redirect", "when",
                    "restart", "stop", "resume", "escalate",
                    "default", "listen-remote-and-local", "persist-all", "persist-inbox", "persist-state", "remote-publish", "remote-state-publish",
                    "forward-to-self", "forward-to-dead-letters", "forward-to-parent", "forward-to-process", "stay-in-queue",
                    "string", "int", "float", "time", "process-flags", "process-id", "process-name", "directive", 
                    "process", "router", "dispatcher", "cluster", "true", "false", "bool"
                    ));

            // Token parser
            tokenParser = makeTokenParser(definition);
        }

        Parser<T> token<T>(Parser<T> p) =>
            tokenParser.Lexeme(p);

        Parser<T> brackets<T>(Parser<T> p) =>
            tokenParser.Brackets(p);

        Parser<T> parens<T>(Parser<T> p) =>
            tokenParser.Parens(p);

        Parser<Lst<T>> commaSep<T>(Parser<T> p) =>
            tokenParser.CommaSep(p);

        Parser<Lst<T>> commaSep1<T>(Parser<T> p) =>
            tokenParser.CommaSep1(p);

        public ProcessSystemConfigParser(string nodeName, FuncSpec[] processSpecs, FuncSpec[] strategySpecs, FuncSpec[] clusterSpecs, params FuncSpec[] settingSpecs)
        {
            InitialiseParser();

            // Elements of the token parser to use below
            Parser<string> identifier = tokenParser.Identifier;
            Parser<string> stringLiteral = tokenParser.StringLiteral;
            Parser<int> integer = tokenParser.Integer;
            Parser<double> floating = tokenParser.Float;
            Parser<int> natural = tokenParser.Natural;
            Parser<Unit> whiteSpace = tokenParser.WhiteSpace;
            Func<string, Parser<string>> symbol = tokenParser.Symbol;
            Func<string, Parser<string>> reserved = tokenParser.Reserved;
            Func<string, Parser<Unit>> reservedOp = tokenParser.ReservedOp;

            Func<FuncSpec[], Parser<Lst<LocalsToken>>> settings = null;
            Func<ArgumentType, Func<string, Parser<ValueToken>>> arrayAttr = null;
            Func<ArgumentType, Func<string, Parser<ValueToken>>> mapAttr = null;
            Func<ArgumentType, string, Parser<ValueToken>> valueInst = null;
            Func<ArgumentType, Parser<ValueToken>> expr = null;
            Parser <ArgumentType> typeDef = null;

            Parser <ProcessId> processId =
                token(
                    from xs in many1(choice(lower, digit, oneOf("@/[,-_]{}: ")))
                    let r = (new string(xs.ToArray())).Trim()
                    let pid = ProcessId.TryParse(r)
                    from res in pid.Match(
                        Right: x => result(x),
                        Left: ex => failure<ProcessId>($"{ex.Message} '({r})'"))
                    select res);

            Parser<ProcessName> processName =
                token(
                    from o in symbol("\"")
                    from xs in many1(choice(lower, digit, oneOf("@/[,-_]{.}: ")))
                    from c in symbol("\"")
                    let r = (new string(xs.ToArray())).Trim()
                    let n = ProcessName.TryParse(r)
                    from res in n.Match(
                        Right: x => result(x),
                        Left: ex => failure<ProcessName>(ex.Message))
                    select res);

            Func<string, Parser<ValueToken>> boolAttr =
                name =>
                    from v in choice(
                        reserved("true"),
                        reserved("false"))
                    select ValueToken.Bool(name, v == "true");

            Func<string, Parser<ValueToken>> doubleAttr =
                name =>
                    from v in floating
                    select ValueToken.Double(name, v);

            Func<string, Parser<ValueToken>> integerAttr =
                name =>
                    from v in integer
                    select ValueToken.Int(name, v);

            Func<string, Parser<ValueToken>> stringAttr =
                name =>
                    from s in stringLiteral
                    select ValueToken.String(name, s);

            Func<string, Parser<ValueToken>> processIdAttr =
                name =>
                    from pid in processId
                    select ValueToken.ProcessId(name, pid);

            Func<string, Parser<ValueToken>> processNameAttr =
                name =>
                    from n in processName
                    select ValueToken.ProcessName(name, n);

            Func<string, ProcessFlags, Parser<ProcessFlags>> flagMap =
                (name, flags) =>
                    attempt(
                    from x in reserved(name)
                    select flags);

            Parser<ProcessFlags> flag =
                choice(
                    flagMap("default", ProcessFlags.Default),
                    flagMap("listen-remote-and-local", ProcessFlags.ListenRemoteAndLocal),
                    flagMap("persist-all", ProcessFlags.PersistAll),
                    flagMap("persist-inbox", ProcessFlags.PersistInbox),
                    flagMap("persist-state", ProcessFlags.PersistState),
                    flagMap("remote-publish", ProcessFlags.RemotePublish),
                    flagMap("remote-state-publish", ProcessFlags.RemoteStatePublish));

            Func<string, Parser<ValueToken>> flagsAttr =
                name =>
                    from fs in brackets(commaSep(flag))
                    select ValueToken.ProcessFlags(name, List.fold(fs, ProcessFlags.Default, (s, x) => s | x));

            Parser<string> timeUnit =
                choice(
                    attempt(reserved("seconds")),
                    attempt(reserved("second")),
                    attempt(reserved("secs")),
                    attempt(reserved("sec")),
                    attempt(reserved("s")),
                    attempt(reserved("minutes")),
                    attempt(reserved("minute")),
                    attempt(reserved("mins")),
                    attempt(reserved("min")),
                    attempt(reserved("milliseconds")),
                    attempt(reserved("millisecond")),
                    attempt(reserved("ms")),
                    attempt(reserved("hours")),
                    attempt(reserved("hour")),
                    reserved("hr"))
                   .label("Unit of time (e.g. seconds, mins, hours, hr, sec, min...)");

            Func<string, Parser<ValueToken>> timeAttr =
                name =>
                    from v in floating
                    from u in timeUnit
                    from r in TimeAttr.TryParse(v, u).Match(
                        Some: result,
                        None: () => failure<Time>("Invalid unit of time"))
                    select ValueToken.Time(name, r);

            Func<string, Parser<ValueToken>, Parser<ValueToken>> attr =
                (name, p) =>
                    from x in reserved(name)
                    from _ in symbol("=")
                    from v in p
                    select v.SetName(name);

            Parser<Type> type =
                from x in letter
                from xs in many1(choice(letter, ch('.'), ch('_')))
                select Type.GetType(new string(x.Cons(xs).ToArray()));

            Parser<MessageDirective> fwdToSelf =
                from _ in reserved("forward-to-self")
                select new ForwardToSelf() as MessageDirective;

            Parser<MessageDirective> fwdToParent =
                from _ in reserved("forward-to-parent")
                select new ForwardToParent() as MessageDirective;

            Parser<MessageDirective> fwdToDeadLetters =
                from _ in reserved("forward-to-dead-letters")
                select new ForwardToDeadLetters() as MessageDirective;

            Parser<MessageDirective> stayInQueue =
                from _ in reserved("stay-in-queue")
                select new StayInQueue() as MessageDirective;

            Parser<MessageDirective> fwdToProcess =
                from _ in reserved("forward-to-process")
                from pid in attempt(expr(ArgumentType.ProcessId)).label("'forward-to-process <ProcessId>'")
                select new ForwardToProcess((ProcessId)pid.Value) as MessageDirective;

            Parser<MessageDirective> msgDirective =
                choice(
                    fwdToDeadLetters,
                    fwdToSelf,
                    fwdToParent,
                    fwdToProcess,
                    stayInQueue);

            Parser<string> dispType =
                choice(
                    reserved("broadcast"),
                    attempt(reserved("least-busy")),
                    attempt(reserved("round-robin")),
                    reserved("random"),
                    reserved("hash"),
                    reserved("first"),
                    reserved("second"),
                    reserved("third"),
                    reserved("last")
                );

            Func<string, Parser<ValueToken>> dispTypeAttr =
                name =>
                    from d in dispType
                    select ValueToken.DispatcherType(name, d);


            Parser<Directive> directive =
                choice(
                    reserved("resume").Map(_ => Directive.Resume),
                    reserved("restart").Map(_ => Directive.Restart),
                    reserved("stop").Map(_ => Directive.Stop),
                    reserved("escalate").Map(_ => Directive.Escalate));

            Func<string, Parser<ValueToken>> directiveAttr =
                name =>
                    from d in directive
                    select ValueToken.Directive(name, d);

            Parser<State<Exception, Option<Directive>>> exceptionDirective =
                from b in symbol("|")
                from t in token(type)
                from a in symbol("->")
                from d in token(directive)
                select Strategy.With(d, t);

            Parser<State<Exception, Option<Directive>>> otherwiseDirective =
                from b in symbol("|")
                from t in symbol("_")
                from a in symbol("->")
                from d in token(directive)
                select Strategy.Otherwise(d);

            Parser<State<Directive, Option<MessageDirective>>> matchMessageDirective =
                from b in symbol("|")
                from d in token(directive)
                from a in symbol("->")
                from m in token(msgDirective)
                select Strategy.When(m, d);

            Parser<State<Directive, Option<MessageDirective>>> otherwiseMsgDirective =
                from b in symbol("|")
                from t in symbol("_")
                from a in symbol("->")
                from d in token(msgDirective)
                select Strategy.Otherwise(d);

            Parser<ValueToken> match =
                from _ in attempt(reserved("match"))
                from direx in many(attempt(exceptionDirective))
                from other in optional(otherwiseDirective)
                let dirs = direx.Append(other.AsEnumerable()).ToArray()
                from ok in dirs.Length > 0
                    ? result(dirs)
                    : failure<State<Exception, Option<Directive>>[]>("'match' must be followed by at least one clause")
                select ValueToken.StrategyMatch("match", Strategy.Match(dirs));

            Parser<State<StrategyContext, Unit>> redirectMatch =
                from direx in many(attempt(matchMessageDirective))
                from other in optional(otherwiseMsgDirective)
                let dirs = direx.Append(other.AsEnumerable()).ToArray()
                from ok in dirs.Length > 0
                    ? result(dirs)
                    : failure<State<Directive, Option<MessageDirective>>[]>("'redirect when' must be followed by at least one clause")
                select Strategy.Redirect(dirs);

            Parser<ValueToken> redirect =
                from n in attempt(reserved("redirect"))
                from t in either(attempt(symbol(":")), reserved("when"))
                from r in t == ":"
                   ? from d in token(msgDirective)
                     select Strategy.Redirect(d)
                   : redirectMatch
                select ValueToken.StrategyRedirect("redirect", r);
        
            Func<FuncSpec[], Func<string, Parser<ValueToken>>> buildProcessAttr = ps => name =>
                from ss in settings(ps)
                select ValueToken.Process(name, ss);

            Func<string, Parser<ValueToken>> processAttr = buildProcessAttr(processSpecs);

            Func<FuncSpec[], Func<string, Parser<ValueToken>>> buildClusterAttr = ps => name =>
                from ss in settings(ps)
                let token = new ClusterToken(ss)
                from __ in token.NodeName.Map(x => x == nodeName).IfNone(false)
                    ? from state in getState<ParserState>()
                      from newst in setState(state.SetCluster(token.Settings))
                      select newst
                    : result(unit)
                select ValueToken.Cluster(name, token);

            Func<string, Parser<ValueToken>> clusterAttr = buildClusterAttr(clusterSpecs);

            Func<FuncSpec[], Func<string, Parser<ValueToken>>> buildStrategyAttr = stratSet => name =>
                from t in attempt(either(reserved("all-for-one"), reserved("one-for-one")))
                from _ in symbol(":")
                from ss in settings(stratSet)
                select ValueToken.Strategy(name, t, ss);

            Func<string, Parser<ValueToken>> strategyAttr = buildStrategyAttr(strategySpecs);


            arrayAttr = t => name =>
                brackets(
                    from xs in commaSep(expr(t))
                    select ValueToken.Array(
                        name,
                            t.Tag == ArgumentTypeTag.Time ? (object)xs.Map(x => (Time)x.Value).Freeze()
                          : t.Tag == ArgumentTypeTag.Bool ? (object)xs.Map(x => (bool)x.Value).Freeze()
                          : t.Tag == ArgumentTypeTag.Int ? (object)xs.Map(x => (int)x.Value).Freeze()
                          : t.Tag == ArgumentTypeTag.String ? (object)xs.Map(x => (string)x.Value).Freeze()
                          : t.Tag == ArgumentTypeTag.Double ? (object)xs.Map(x => (double)x.Value).Freeze()
                          : t.Tag == ArgumentTypeTag.ProcessId ? (object)xs.Map(x => (ProcessId)x.Value).Freeze()
                          : t.Tag == ArgumentTypeTag.ProcessName ? (object)xs.Map(x => (ProcessName)x.Value).Freeze()
                          : t.Tag == ArgumentTypeTag.ProcessFlags ? (object)xs.Map(x => (ProcessFlags)x.Value).Freeze()
                          : t.Tag == ArgumentTypeTag.Directive ? (object)xs.Map(x => (Directive)x.Value).Freeze()
                          : t.Tag == ArgumentTypeTag.Process ? (object)xs.Map(x => (ProcessToken)x.Value).Freeze()
                          : t.Tag == ArgumentTypeTag.Cluster ? (object)xs.Map(x => (ClusterToken)x.Value).Freeze()
                          : t.Tag == ArgumentTypeTag.Strategy ? (object)xs.Map(x => (StrategyToken)x.Value).Freeze()
                          : t.Tag == ArgumentTypeTag.DispatcherType ? (object)xs.Map(x => (string)x.Value).Freeze()
                          : t.Tag == ArgumentTypeTag.Array ? (object)xs.Map(x => x.Value).Freeze()
                          : t.Tag == ArgumentTypeTag.Map ? (object)xs.Map(x => x.Value).Freeze()
                          : (object)null, t))
                .label("array");

            mapAttr = t => name =>
                brackets(
                    from xs in commaSep(
                        from x in identifier
                        from _ in symbol(":")
                        from v in expr(t)
                        select Tuple(x, v.Value))
                    select ValueToken.Map(
                        name,
                        t.Tag == ArgumentTypeTag.Time ? (object)Map.createRange(xs.Map(x => x.MapSecond(y => (Time)y)))
                        : t.Tag == ArgumentTypeTag.Bool ? (object)Map.createRange(xs.Map(x => x.MapSecond(y => (bool)y)))
                        : t.Tag == ArgumentTypeTag.Int ? (object)Map.createRange(xs.Map(x => x.MapSecond(y => (int)y)))
                        : t.Tag == ArgumentTypeTag.String ? (object)Map.createRange(xs.Map(x => x.MapSecond(y => (string)y)))
                        : t.Tag == ArgumentTypeTag.Double ? (object)Map.createRange(xs.Map(x => x.MapSecond(y => (double)y)))
                        : t.Tag == ArgumentTypeTag.ProcessId ? (object)Map.createRange(xs.Map(x => x.MapSecond(y => (ProcessId)y)))
                        : t.Tag == ArgumentTypeTag.ProcessName ? (object)Map.createRange(xs.Map(x => x.MapSecond(y => (ProcessName)y)))
                        : t.Tag == ArgumentTypeTag.ProcessFlags ? (object)Map.createRange(xs.Map(x => x.MapSecond(y => (ProcessFlags)y)))
                        : t.Tag == ArgumentTypeTag.Directive ? (object)Map.createRange(xs.Map(x => x.MapSecond(y => (Directive)y)))
                        : t.Tag == ArgumentTypeTag.Process ? (object)Map.createRange(xs.Map(x => x.MapSecond(y => (ProcessToken)y)))
                        : t.Tag == ArgumentTypeTag.Cluster ? (object)Map.createRange(xs.Map(x => x.MapSecond(y => (ClusterToken)y)))
                        : t.Tag == ArgumentTypeTag.Strategy ? (object)Map.createRange(xs.Map(x => x.MapSecond(y => (StrategyToken)y)))
                        : t.Tag == ArgumentTypeTag.DispatcherType ? (object)Map.createRange(xs.Map(x => x.MapSecond(y => (string)y)))
                        : t.Tag == ArgumentTypeTag.Array ? (object)Map.createRange(xs.Map(x => x.MapSecond(y => y)))
                        : t.Tag == ArgumentTypeTag.Map ? (object)Map.createRange(xs.Map(x => x.MapSecond(y => y)))
                        : (object)null, t ))
                .label("map");

            typeDef =
                choice(
                    reserved("bool").Map(_ => ArgumentType.Bool),
                    reserved("string").Map(_ => ArgumentType.String),
                    reserved("time").Map(_ => ArgumentType.Time),
                    reserved("float").Map(_ => ArgumentType.Double),
                    reserved("int").Map(_ => ArgumentType.Int),
                    reserved("process-flags").Map(_ => ArgumentType.ProcessFlags),
                    reserved("process-id").Map(_ => ArgumentType.ProcessId),
                    reserved("process-name").Map(_ => ArgumentType.ProcessName),
                    reserved("directive").Map(_ => ArgumentType.Directive),
                    reserved("strategy").Map(_ => ArgumentType.Strategy),
                    reserved("process").Map( _ => ArgumentType.Process),
                    reserved("cluster").Map( _ => ArgumentType.Cluster),
                    reserved("router").Map(_ => ArgumentType.Process),
                    reserved("disp").Map(_ => ArgumentType.DispatcherType)
                );

            Parser<ValueToken> clusterVar =
                attempt(
                        from _ in reserved("cluster")
                        from d in symbol(".")
                        from id in identifier
                        from sub in optional(from d2 in symbol(".")
                                             from id2 in identifier
                                             select id2).Map(x => x.IfNone("value"))
                        from state in getState<ParserState>()   // TODO: This can be generalised into an object walking system
                        from v in state.Cluster.Match(          //       where an object (in this case the cluster), is in 'scope'
                            Some: cluster =>                    //       and recursive walking of the dot operator will find the
                                cluster.Find(id).Match(         //       value.
                                    Some: locals =>
                                        locals.Values.Find(sub)
                                                     .Match(
                                                          Some: v => result(v),
                                                          None: () => failure<ValueToken>($"unknown identifier 'cluster.{id}'")),
                                    None: () => failure<ValueToken>($"unknown identifier 'cluster.{id}'")),
                            None: () => failure<ValueToken>($"cluster.{id} used when a cluster with a node-name attribute set to '{nodeName}' hasn't been defined"))
                        select v
                    );

            Parser<ValueToken> variable =
                either(
                    clusterVar,
                    attempt(
                        from id in identifier
                        from state in getState<ParserState>()
                        from v in state.Local(id).Match(
                            Some: v => result(v),
                            None: () => failure<ValueToken>($"unknown identifier '{id}' "))
                        select v
                    )
                );

            Parser <ValueToken> valueDef =
                from typ in either(
                    attempt(reserved("let")).Map(x => Option<ArgumentType>.None),
                    typeDef.Map(Option<ArgumentType>.Some)
                )
                from arr in optional(symbol("[]"))
                from _   in arr.IsSome && typ.IsNone
                    ? failure<Unit>("when declaring an array you must specify the type, you can't use 'let'")
                    : result<Unit>(unit)
                from id in identifier.label("identifier")
                from __ in symbol(":")
                from v in arr.IsSome
                    ? either(
                        attempt(valueInst(ArgumentType.Map(typ.IfNone(ArgumentType.Unknown)), id)),
                        valueInst(ArgumentType.Array(typ.IfNone(ArgumentType.Unknown)), id))
                    : typ.Map( t => expr(t).Map(x => x.SetName(id) ))
                         .IfNone(() => 
                              choice(
                                  variable.Map(x => x.SetName(id)),
                                  boolAttr(id),
                                  stringAttr(id),
                                  attempt(timeAttr(id)),
                                  integerAttr(id),
                                  doubleAttr(id),
                                  attempt(flagsAttr(id)),
                                  directiveAttr(id),
                                  dispTypeAttr(id),
                                  strategyAttr(id),
                                  processAttr(id),
                                  clusterAttr(id)
                         ))
                from state in getState<ParserState>()
                from res in state.LocalExists(id)
                    ? failure<ParserState>($"A value with the name '{id}' already declared")
                    : result(state.AddLocal(id, v))
                from ___ in setState(res)
                select v;

            valueInst =
                (t, name) =>
                    either(
                        variable,
                          t.Tag == ArgumentTypeTag.String ? stringAttr(name)
                        : t.Tag == ArgumentTypeTag.Time ? timeAttr(name)
                        : t.Tag == ArgumentTypeTag.Bool ? boolAttr(name)
                        : t.Tag == ArgumentTypeTag.Int ? integerAttr(name)
                        : t.Tag == ArgumentTypeTag.Double ? doubleAttr(name)
                        : t.Tag == ArgumentTypeTag.ProcessFlags ? flagsAttr(name)
                        : t.Tag == ArgumentTypeTag.ProcessId ? processIdAttr(name)
                        : t.Tag == ArgumentTypeTag.ProcessName ? processNameAttr(name)
                        : t.Tag == ArgumentTypeTag.Map ? mapAttr(t.GenericType)(name)
                        : t.Tag == ArgumentTypeTag.Array ? arrayAttr(t.GenericType)(name)
                        : t.Tag == ArgumentTypeTag.Directive ? directiveAttr(name)
                        : t.Tag == ArgumentTypeTag.Process ? processAttr(name)
                        : t.Tag == ArgumentTypeTag.Cluster ? clusterAttr(name)
                        : t.Tag == ArgumentTypeTag.Strategy ? strategyAttr(name)
                        : t.Tag == ArgumentTypeTag.DispatcherType ? dispTypeAttr(name)
                        : failure<ValueToken>($"value-type for '{t.Tag}'"));


            Func<string, Func<ValueToken, ValueToken, ValueToken>, Assoc, Operator<ValueToken>> binary =
                (name, f, assoc) =>
                     Operator.Infix(assoc,
                        from x in reservedOp(name)
                        select f);

            Func<string, Func<ValueToken, ValueToken>, Operator<ValueToken>> prefix =
                (name, f) =>
                    Operator.Prefix(
                        from x in reservedOp(name)
                        select f);

            Func<string, Func<ValueToken, ValueToken>, Operator<ValueToken>> postfix =
                (name, f) =>
                    Operator.Postfix(
                        from x in reservedOp(name)
                        select f);

            Operator<ValueToken>[][] table = {
                new [] { prefix("-",ValueToken.Negate), prefix("+",t=>t) },
                new [] { postfix("++", ValueToken.Incr) },
                new [] { binary("*", ValueToken.Mul, Assoc.Left), binary("/", ValueToken.Div, Assoc.Left) },
                new [] { binary("+", ValueToken.Add, Assoc.Left), binary("-", ValueToken.Sub, Assoc.Left) }
            };

            Func<ArgumentType, Parser<ValueToken>> term =
                expected =>
                    choice(
                        parens(lazyp(() => expr(expected))),
                        valueInst(expected, ""));

            expr =
                expected =>
                    buildExpressionParser(table, term(expected));

            /// <summary>
            /// Parses a named argument: name = value
            /// </summary>
            Func<string, FieldSpec, Parser<ValueToken>> namedArgument =
                (settingName, spec) =>
                    attempt(token(attr(spec.Name, expr(spec.Type))));

            /// <summary>
            /// Parses a single non-named argument
            /// </summary>
            Func<string, FieldSpec, Parser<ValueToken>> argument =
                (settingName, spec) =>
                    attempt(token(expr(spec.Type).Map(x => x.SetName(spec.Name))));

            /// <summary>
            ///  Parses many arguments, wrapped in ( )
            /// </summary>
            Func<string, ArgumentsSpec, Parser<Lst<ValueToken>>> argumentMany =
                (settingName, spec) =>
                    from a in commaSep1(choice(spec.Args.Map(arg => namedArgument(settingName, arg))))
                    from r in a.Count == spec.Args.Length
                        ? result(a)
                        : failure<Lst<ValueToken>>("Invalid arguments for " + settingName)
                    select r;

            /// <summary>
            /// Parses the arguments for a setting
            /// </summary>
            Func<string, ArgumentsSpec, Parser<Lst<ValueToken>>> arguments =
                (settingName, spec) =>
                    spec.Args.Length == 0
                        ? failure<Lst<ValueToken>>("Invalid arguments spec, has zero arguments")
                        : spec.Args.Length == 1
                            ? from a in argument(settingName, spec.Args.Head())
                              select List.create(a)
                            : argumentMany(settingName, spec);

            // Top level settings
            settings = (specs) =>
                from state in getState<ParserState>()
                from sets in many(
                    indented1(
                        either(
                            // Funcs
                            choice(
                                specs.Map(setting =>
                                      setting.Name == "match" ? from m in match
                                                                select new LocalsToken(setting.Name, null, m)
                                    : setting.Name == "redirect" ? from r in redirect
                                                                   select new LocalsToken(setting.Name, null, r)
                                    : from nam in attempt(reserved(setting.Name))
                                    from _ in symbol(":")
                                    from tok in choice(setting.Variants.Map(variant =>
                                        from vals in arguments(nam, variant)
                                        select new LocalsToken(setting.Name, variant, vals.ToArray())))
                                    select tok)),
                            // Var defs
                            from x in valueDef
                            select new LocalsToken(x.Name, ArgumentsSpec.Variant(new FieldSpec("value", x.Type)), x.SetName("value"))
                            )))
                from newState in getState<ParserState>()
                from _ in setState(state.SetCluster(newState.Cluster))
                select List.createRange(sets);

            Settings = from ws in whiteSpace
                       from __ in setState(ParserState.Empty)
                       from ss in settings(settingSpecs)
                       select Map.createRange(ss.Map(x => Tuple(x.Name, x)));
        }
    }
}

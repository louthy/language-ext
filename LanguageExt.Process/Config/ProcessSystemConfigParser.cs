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
    /// <summary>
    /// Parses the Process system conf file format
    /// This is a bit more advanced than your usual configuration format, in that it's
    /// essentially a statically typed programming language.  
    /// </summary>
    public class ProcessSystemConfigParser
    {
        public readonly GenLanguageDef Definition;
        public readonly GenTokenParser TokenParser;
        public Parser<Lst<NamedValueToken>> parser;
        public readonly Types types;
        public readonly TypeDef processType;
        public readonly TypeDef routerType;
        public readonly TypeDef strategyType;
        public readonly TypeDef clusterType;
        public readonly string nodeName;

        public Parser<T> token<T>(Parser<T> p) =>
            TokenParser.Lexeme(p);

        public Parser<T> brackets<T>(Parser<T> p) =>
            TokenParser.Brackets(p);

        public Parser<T> parens<T>(Parser<T> p) =>
            TokenParser.Parens(p);

        public Parser<Lst<T>> commaSep<T>(Parser<T> p) =>
            TokenParser.CommaSep(p);

        public Parser<Lst<T>> commaSep1<T>(Parser<T> p) =>
            TokenParser.CommaSep1(p);

        public readonly Parser<string> identifier;
        public readonly Parser<string> stringLiteral;
        public readonly Parser<int> integer;
        public readonly Parser<double> floating;
        public readonly Parser<int> natural;
        public readonly Parser<Unit> whiteSpace;
        public readonly Func<string, Parser<string>> symbol;
        public readonly Func<string, Parser<string>> reserved;
        public readonly Func<string, Parser<Unit>> reservedOp;
        public readonly Parser<ProcessId> processId;
        public readonly Parser<ProcessName> processName;
        public readonly Parser<ValueToken> match;
        public readonly Parser<ValueToken> redirect;
        public readonly Func<string, FieldSpec[], Parser<Lst<NamedValueToken>>> arguments;
        public readonly Func<string, FieldSpec[], Parser<Lst<NamedValueToken>>> argumentMany;
        public readonly Func<string, FieldSpec, Parser<NamedValueToken>> argument;
        public readonly Func<string, FieldSpec, Parser<NamedValueToken>> namedArgument;
        public readonly Parser<NamedValueToken> valueDef;
        public readonly Func<TypeDef, Parser<ValueToken>> term;
        public readonly Func<TypeDef, Parser<ValueToken>> expr;
        public readonly Parser<ValueToken> exprUnknownType;
        public readonly Parser<ValueToken> valueUntyped;

        public ProcessSystemConfigParser(string nodeName, Types typeDefs, IEnumerable<FuncSpec> strategyFuncs)
        {
            strategyFuncs        = strategyFuncs ?? new FuncSpec[0];
            this.nodeName        = nodeName;
            this.types           = typeDefs;
            this.clusterType     = types.Register(BuildClusterType());
            this.processType     = types.Register(BuildProcessType());
            this.routerType      = types.Register(BuildRouterType());
            this.strategyType    = types.Register(BuildStrategySpec(types, strategyFuncs));

            var opChars = ":!%&*+.<=>\\^|-~";

            // Process config definition
            Definition = GenLanguageDef.Empty.With(
                CommentStart: "/*",
                CommentEnd: "*/",
                CommentLine: "//",
                NestedComments: true,
                OpStart: oneOf(opChars),
                OpLetter: oneOf(opChars),
                IdentStart: letter,
                IdentLetter: either(alphaNum, oneOf("-_")),
                ReservedNames: List("if", "then", "else").AddRange(types.AllInOrder.Map(t => t.Name)),
                ReservedOpNames: List("-", "+", "/", "*", "==", "!=", ">", "<", "<=", ">=", "||", "&&", "|", "&", "%", "!", "~", "^")
            );

            // Token parser
            // This builds the standard token parser from the definition above
            TokenParser   = makeTokenParser(Definition);
            identifier    = TokenParser.Identifier;
            stringLiteral = TokenParser.StringLiteral;
            integer       = TokenParser.Integer;
            floating      = TokenParser.Float;
            natural       = TokenParser.Natural;
            whiteSpace    = TokenParser.WhiteSpace;
            symbol        = TokenParser.Symbol;
            reserved      = TokenParser.Reserved;
            reservedOp    = TokenParser.ReservedOp;

            // Binary operator parser
            Func<string, Assoc, Operator<ValueToken>> binary =
                (name, assoc) =>
                     Operator.Infix(assoc,
                        from x in reservedOp(name)
                        select ValueToken.BinaryOp(name));

            // Prefix operator parser
            Func<string, Operator<ValueToken>> prefix =
                (name) =>
                    Operator.Prefix(
                        from x in reservedOp(name)
                        select ValueToken.PrefixOp(name));

            // Postfix operator parser
            Func<string, Operator<ValueToken>> postfix =
                (name) =>
                    Operator.Postfix(
                        from x in reservedOp(name)
                        select ValueToken.PostfixOp(name));

            // Operator table
            Operator<ValueToken>[][] table = {
                new [] { prefix("-"), prefix("+"), prefix("!") },
                new [] { binary("*", Assoc.Left), binary("/", Assoc.Left), binary("%", Assoc.Left) },
                new [] { binary("+", Assoc.Left), binary("-", Assoc.Left) },
                new [] { binary("<", Assoc.Left), binary(">", Assoc.Left), binary(">=", Assoc.Left) , binary("<=", Assoc.Left) },
                new [] { binary("==", Assoc.Left), binary("!=", Assoc.Left) },
                new [] { binary("&", Assoc.Left) },
                new [] { binary("^", Assoc.Left) },
                new [] { binary("|", Assoc.Left) },
                new [] { binary("&&", Assoc.Left) },
                new [] { binary("||", Assoc.Left) },
            };


            Func<TypeDef, Parser<ValueToken>> valueInst = null;
            Parser<TypeDef> typeName = null;

            // ProcessId parser
            processId =
                token(
                    from xs in many1(choice(lower, digit, oneOf("@/[,-_]{}: ")))
                    let r = (new string(xs.ToArray())).Trim()
                    let pid = ProcessId.TryParse(r)
                    from res in pid.Match(
                        Right: x => result(x),
                        Left: ex => failure<ProcessId>($"{ex.Message} '({r})'"))
                    select res);

            // ProcessName parser
            processName =
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

            // Attribute parser
            Func<string, Parser<ValueToken>, Parser<NamedValueToken>> attr =
                (name, p) =>
                    from x in reserved(name)
                    from _ in symbol("=")
                    from v in p
                    select new NamedValueToken(name,v);

            // Type name parser
            Parser<Type> type =
                from x in letter
                from xs in many1(choice(letter, ch('.'), ch('_')))
                select Type.GetType(new string(x.Cons(xs).ToArray()));

            var directive = types.Directive.ValueParser(this).Map(x => (Directive)x);
            var msgDirective = types.MessageDirective.ValueParser(this).Map(x => (MessageDirective)x);

            Parser<State<Exception, Option<Directive>>> exceptionDirective =
                from b in symbol("|")
                from t in token(type)
                from a in symbol("->")
                from d in directive
                select Strategy.With(d, t);

            Parser<State<Exception, Option<Directive>>> otherwiseDirective =
                from b in symbol("|")
                from t in symbol("_")
                from a in symbol("->")
                from d in directive
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

            // Strategy exception -> directive parser
            match =
                from _ in attempt(reserved("match"))
                from direx in many(attempt(exceptionDirective))
                from other in optional(otherwiseDirective)
                let dirs = direx.Append(other.AsEnumerable()).ToArray()
                from ok in dirs.Length > 0
                    ? result(dirs)
                    : failure<State<Exception, Option<Directive>>[]>("'match' must be followed by at least one clause")
                select new ValueToken(types.Get("strategy"), Strategy.Match(dirs));

            Parser<State<StrategyContext, Unit>> redirectMatch =
                from direx in many(attempt(matchMessageDirective))
                from other in optional(otherwiseMsgDirective)
                let dirs = direx.Append(other.AsEnumerable()).ToArray()
                from ok in dirs.Length > 0
                    ? result(dirs)
                    : failure<State<Directive, Option<MessageDirective>>[]>("'redirect when' must be followed by at least one clause")
                select Strategy.Redirect(dirs);

            // Strategy directive -> message-directive matching parser
            redirect =
                from n in attempt(reserved("redirect"))
                from t in either(attempt(symbol(":")), reserved("when"))
                from r in t == ":"
                   ? from d in token(msgDirective)
                     select Strategy.Redirect(d)
                   : redirectMatch
                select new ValueToken(types.Get("strategy"), r);

            // Type name parser
            typeName = choice(types.AllInOrder.Map(t => reserved(t.Name).Map(_ => t)).ToArray());

            // cluster.<property> parser -- TODO: generalise
            Parser<ValueToken> clusterVar =
                attempt(
                    from _ in reserved("cluster")
                    from d in symbol(".")
                    from id in identifier
                    from sub in optional(from d2 in symbol(".")
                                         from id2 in identifier
                                         select id2).Map(x => x.IfNone("value"))
                    from state in getState<ParserState>()                               // TODO: This can be generalised into an object walking system
                    from v in state.Cluster.Match(                                      //       where an object (in this case the cluster), is in 'scope'
                        Some: cluster =>                                                //       and recursive walking of the dot operator will find the
                            cluster.Settings.Find(id).Match(                            //       value.
                                Some: local => result(local),
                                None: () => failure<ValueToken>($"unknown identifier 'cluster.{id}'")),
                        None: () => failure<ValueToken>($"cluster.{id} used when a cluster with a node-name attribute set to '{nodeName}' hasn't been defined"))
                    select v
                );

            // Variable of unknown type parser
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

            // Variable of known type parser
            Func<TypeDef, Parser<ValueToken>> variableOfType =
                expect =>
                    from v in variable
                    from r in v.Type == expect
                        ? result(v)
                        : expect.Convert(v).Match(
                                Some: x => result(x),
                                None: () => failure<ValueToken>($"type mismatch {v.Type} found, expected {expect}")
                            )
                    select r;

            Parser<ValueToken> ternary =
                token(
                    from __ in attempt(reserved("if"))
                    from eb in exprUnknownType
                    from ex in eb.Type == types.Bool
                        ? result(eb)
                        : failure<ValueToken>("ternary expressions must evaluate to a boolean value")
                    from th in reserved("then")
                    from te in exprUnknownType
                    from el in reserved("else")
                    from ee in exprUnknownType
                    select ((bool)ex.Value) ? te : ee
                );

            valueUntyped = choice(
                variable,
                choice(types.AllInOrder.Map(typ => attempt(typ.ValueParser(this).Map(val => new ValueToken(typ, typ.Ctor(val))))).ToArray())
            );

            // Expression term parser
            Parser<ValueToken> termUnknownType =
                choice(
                    parens(lazyp(() => exprUnknownType)),
                    ternary,
                    valueUntyped);

            // Expression parser
            exprUnknownType =
                buildExpressionParser(table, termUnknownType);

            // Variable declaration parser
            valueDef =
                from typ in either(
                    attempt(reserved("let")).Map(x => Option<TypeDef>.None),
                    typeName.Map(Option<TypeDef>.Some)
                )
                from arr in optional(symbol("[]"))
                from _ in arr.IsSome && typ.IsNone
                    ? failure<Unit>("when declaring an array you must specify the type, you can't use 'let'")
                    : result<Unit>(unit)
                from id in identifier.label("identifier")
                from __ in symbol(":")
                from v in arr.IsSome
                    ? either(
                        attempt(valueInst(TypeDef.Map(() => typ.IfNone(TypeDef.Unknown)))),
                        valueInst(TypeDef.Array(() => typ.IfNone(TypeDef.Unknown))))
                    : typ.Map(t => expr(t))
                         .IfNone(() => exprUnknownType)
                from nv in result(new NamedValueToken(id, v))
                from state in getState<ParserState>()
                from res in state.LocalExists(id)
                    ? failure<ParserState>($"A value with the name '{id}' already declared")
                    : result(state.AddLocal(id, v))
                from ___ in setState(res)
                select nv;

            // Value or variable parser
            valueInst = typ => either(variableOfType(typ), typ.ValueParser(this).Map(value => new ValueToken(typ, typ.Ctor(value))));

            // Expression term parser
            term =
                expected =>
                    choice(
                        parens(lazyp(() => expr(expected))),
                        valueInst(expected),
                        from val in exprUnknownType
                        from res in val.Type == expected
                            ? result(val)
                            : failure<ValueToken>($"expression must evaluate to {expected}, it actually evaluates to {val.Type}")
                        select res
                    );

            // Expression parser
            expr =
                expected =>
                    buildExpressionParser(table, term(expected));

            // Parses a named argument: name = value
            namedArgument =
                (settingName, spec) =>
                    attempt(token(attr(spec.Name, expr(spec.Type()))));

            // Parses a single non-named argument
            argument =
                (settingName, spec) =>
                    attempt(token(expr(spec.Type()).Map(x => new NamedValueToken(spec.Name,x))));

            // Parses many arguments, wrapped in ( )
            argumentMany =
                (settingName, spec) =>
                    from a in commaSep1(choice(spec.Map(arg => namedArgument(settingName, arg))))
                    from r in a.Count == spec.Length
                        ? result(a)
                        : failure<Lst<NamedValueToken>>("Invalid arguments for " + settingName)
                    select r;

            // Parses the arguments for a setting
            arguments =
                (settingName, spec) =>
                    spec.Length == 0
                        ? failure<Lst<NamedValueToken>>("Invalid arguments spec, has zero arguments")
                        : spec.Length == 1
                            ? from a in argument(settingName, spec.Head())
                              select List.create(a)
                            : argumentMany(settingName, spec);

            // Declare the global type
            var globalType = new TypeDef("global", x=>x, typeof(Lst<NamedValueToken>), nodeName, 0);

            // Global namespace
            parser = from ws in whiteSpace
                       from __ in setState(ParserState.Empty)
                       from ss in globalType.ValueParser(this)
                       select (Lst<NamedValueToken>)ss;
        }

        private TypeDef BuildProcessType() =>
            new TypeDef(
                "process",
                typeof(ProcessToken),
                nvs => new ProcessToken((Lst<NamedValueToken>)nvs),
                20,
                FuncSpec.Property("pid", () => types.ProcessId),
                FuncSpec.Property("flags", () => types.ProcessFlags),
                FuncSpec.Property("mailbox-size", () => types.Int),
                FuncSpec.Property("dispatch", () => types.DispatcherType),
                FuncSpec.Property("strategy", () => strategyType));

        private TypeDef BuildRouterType() =>
            new TypeDef(
                "router",
                typeof(ProcessToken),
                nvs => new ProcessToken((Lst<NamedValueToken>)nvs),
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

        private TypeDef BuildClusterType() =>
            new TypeDef(
                "cluster",
                typeof(ClusterToken),
                nvs => new ClusterToken((Lst<NamedValueToken>)nvs),
                20,
                FuncSpec.Property("node-name", () => types.String),
                FuncSpec.Property("role", () => types.String),
                FuncSpec.Property("provider", () => types.String),
                FuncSpec.Property("connection", () => types.String),
                FuncSpec.Property("database", () => types.String),
                FuncSpec.Property("env", () => types.String),
                FuncSpec.Property("user-env", () => types.String));

        public Map<SystemName, ProcessSystemConfig> ParseConfigText(string text)
        {
            // Parse the config text
            var res = parse(parser, text);
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
            var processSettings = List.fold(
                from nv in res.Reply.Result
                where nv.Value.Type == processType || nv.Value.Type == routerType
                let process = nv.Value.Cast<ProcessToken>()
                let pid = process.ProcessId
                where pid.IsSome
                let reg = process.RegisteredName
                let final = process.SetRegisteredName(new ValueToken(types.ProcessName, reg.IfNone(new ProcessName(nv.Name))))
                select Tuple(pid.IfNone(ProcessId.None), final),
                Map.empty<ProcessId, ProcessToken>(),
                (s, x) => Map.tryAdd(s, x.Item1, x.Item2, (_, p) => failwith<Map<ProcessId, ProcessToken>>("Process declared twice: " + p.RegisteredName.IfNone("not defined"))));


            // Extract the cluster settings
            var stratSettings = List.fold(
                from nv in res.Reply.Result
                where nv.Value.Type == strategyType
                let strategy = nv.Value.Cast<State<StrategyContext, Unit>>()
                select Tuple(nv.Name, strategy),
                Map.empty<string, State<StrategyContext, Unit>>(),
                (s, x) => Map.tryAdd(s, x.Item1, x.Item2, (_, __) => failwith<Map<string, State<StrategyContext, Unit>>>("Strategy declared twice: " + x.Item1)));

            // Extract the strategy settings
            var clusterSettings = List.fold(
                from nv in res.Reply.Result
                where nv.Value.Type == clusterType
                let cluster = nv.Value.Cast<ClusterToken>()
                let env = cluster.Env
                let clusterNodeName = cluster.NodeName
                where clusterNodeName.IsSome
                let final = cluster.SetEnvironment(new ValueToken(types.String, env.IfNone(nv.Name)))
                select Tuple(final.Env.IfNone(""), final),
                Map.empty<SystemName, ClusterToken>(),
                (s, x) => Map.tryAdd(s, new SystemName(x.Item1), x.Item2, (_, c) => failwith<Map<SystemName, ClusterToken>>("Cluster declared twice: " + c.Env.IfNone(""))));

            var roleSettings = List.fold(
                res.Reply.Result,
                Map.empty<string, ValueToken>(),
                (s, x) => Map.addOrUpdate(s, x.Name, x.Value)
            );

            return String.IsNullOrEmpty(nodeName)
                ? Map.create(Tuple(default(SystemName), new ProcessSystemConfig(default(SystemName), "root", roleSettings, processSettings, stratSettings, null, types)))
                : clusterSettings.Map(cluster => 
                      new ProcessSystemConfig(
                          cluster.Env.Map(e => new SystemName(e)).IfNone(default(SystemName)),
                          cluster.NodeName.IfNone(""),
                          roleSettings, 
                          processSettings, 
                          stratSettings, 
                          cluster, 
                          types));
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
                typeof(State<StrategyContext,Unit>),
                s => Strategy.Compose(compose((Lst<NamedValueToken>)s)),
                20,
                new[] { oneForOne, allForOne, always, pause, retries1, retries2, backoff1, backoff2, match, redirect }.Append(strategyFuncs).ToArray()
            );

            return strategy;
        }
    }
}

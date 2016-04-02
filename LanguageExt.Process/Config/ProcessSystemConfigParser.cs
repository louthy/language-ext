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
        public Parser<Lst<NamedValueToken>> Settings;

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

        public ProcessSystemConfigParser(string nodeName, Types types)
        {
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
                choice(types.AllInOrder.Map(x => attempt(x.ValueParser(this).Map(vt => new ValueToken(x, vt)))).ToArray())
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

            // Value parser
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
            valueInst = typ => either(variableOfType(typ), typ.ValueParser(this).Map(value => new ValueToken(typ, value)));

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
            var globalType = new TypeDef("global", x=>x, nodeName, 0);

            // Global namespace
            Settings = from ws in whiteSpace
                       from __ in setState(ParserState.Empty)
                       from ss in globalType.ValueParser(this)
                       select (Lst<NamedValueToken>)ss;
        }
    }
}

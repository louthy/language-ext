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
using LanguageExt.UnitsOfMeasure;

namespace LanguageExt
{
    public class SystemConfigParser
    {
        // Process config definition
        readonly static GenLanguageDef definition = GenLanguageDef.Empty.With(
            CommentStart: "/*",
            CommentEnd: "*/",
            CommentLine: "//",
            NestedComments: true,
            IdentStart: letter,
            IdentLetter: either(alphaNum, oneOf("-_")),
            ReservedNames: List("pid", "strategy", "flags", "mailbox-size", "one-for-one", "all-for-one", "settings"));

        // Token parser
        readonly static GenTokenParser tokenParser =
            Token.makeTokenParser(definition);

        // Elements of the token parser to use below
        readonly static Parser<string> identifier = tokenParser.Identifier;
        readonly static Parser<string> stringLiteral = tokenParser.StringLiteral;
        readonly static Parser<int> integer = tokenParser.Integer;
        readonly static Parser<double> floating = tokenParser.Float;
        readonly static Parser<int> natural = tokenParser.Natural;
        readonly static Parser<Unit> whiteSpace = tokenParser.WhiteSpace;
        readonly static Func<string, Parser<string>> symbol = tokenParser.Symbol;
        readonly static Func<string, Parser<string>> reserved = tokenParser.Reserved;
        static Parser<T> token<T>(Parser<T> p) => tokenParser.Lexeme(p);
        static Parser<T> brackets<T>(Parser<T> p) => tokenParser.Brackets(p);
        static Parser<Lst<T>> commaSep<T>(Parser<T> p) => tokenParser.CommaSep(p);
        static Parser<Lst<T>> commaSep1<T>(Parser<T> p) => tokenParser.CommaSep1(p);

        readonly static Parser<ProcessId> processId =
            token(
                from o in symbol("\"")
                from xs in many1(choice(lower, digit, oneOf("@/[,-_]{}(): ")))
                from c in symbol("\"")
                let r = (new string(xs.ToArray())).Trim()
                let pid = ProcessId.TryParse(r)
                from res in pid.Match(
                    Right: x => result(x),
                    Left: ex => failure<ProcessId>(ex.Message))
                select res);

        readonly static Parser<ProcessName> processName =
            token(
                from o in symbol("\"")
                from xs in many1(choice(lower, digit, oneOf("@/[,-_]{}(): ")))
                from c in symbol("\"")
                let r = (new string(xs.ToArray())).Trim()
                let n = ProcessName.TryParse(r)
                from res in n.Match(
                    Right: x => result(x),
                    Left: ex => failure<ProcessName>(ex.Message))
                select res);

        static Parser<SettingValue> doubleAttr(string name) =>
            from v in floating
            select SettingValue.Double(name, v);

        static Parser<SettingValue> integerAttr(string name) =>
            from v in integer
            select SettingValue.Int(name, v);

        static Parser<SettingValue> stringAttr(string name) =>
            from s in stringLiteral
            select SettingValue.String(name, s);

        static Parser<SettingValue> processIdAttr(string name) =>
            from pid in processId
            select SettingValue.ProcessId(name, pid);

        static Parser<SettingValue> processNameAttr(string name) =>
            from n in processName
            select SettingValue.ProcessName(name, n);

        static Parser<ProcessFlags> flagMap(string name, ProcessFlags flag) =>
            attempt(
             from x in symbol(name)
             select flag);

        readonly static Parser<ProcessFlags> flag =
            choice(
                flagMap("default", ProcessFlags.Default),
                flagMap("listen-remote-and-local", ProcessFlags.ListenRemoteAndLocal),
                flagMap("persist-all", ProcessFlags.PersistAll),
                flagMap("persist-inbox", ProcessFlags.PersistInbox),
                flagMap("persist-state", ProcessFlags.PersistState),
                flagMap("remote-publish", ProcessFlags.RemotePublish),
                flagMap("remote-state-publish", ProcessFlags.RemoteStatePublish));

        static Parser<SettingValue> flagsAttr(string name) =>
            from fs in brackets(commaSep(flag))
            select SettingValue.ProcessFlags(name, List.fold(fs, ProcessFlags.Default, (s, x) => s | x));

        static readonly Parser<string> timeUnit =
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

        static Parser<SettingValue> timeAttr(string name) =>
            from v in integer
            from u in timeUnit
            from r in TimeAttr.TryParse(v, u).Match(
                Some: result,
                None: () => failure<Time>("Invalid unit of time"))
            select SettingValue.Time(name, r);

        static Parser<SettingValue> attr(string name, Func<string, Parser<SettingValue>> p) =>
            from x in reserved(name)
            from _ in symbol("=")
            from v in p(name)
            select v;

        static Func<SettingSpec[], Func<string, Parser<SettingValue>>> processAttr =>
            processSettings =>
                name =>
                    from ss in settings(processSettings)
                    select SettingValue.Process(name, processSettings, ss);

        readonly static Parser<Type> type =
            from x in letter
            from xs in many1(choice(letter, ch('.'), ch('_')))
            select Type.GetType(new string(x.Cons(xs).ToArray()));

        static readonly Parser<MessageDirective> fwdToSelf =
            from _ in reserved("forward-to-self")
            select new ForwardToSelf() as MessageDirective;

        static readonly Parser<MessageDirective> fwdToParent =
            from _ in reserved("forward-to-parent")
            select new ForwardToParent() as MessageDirective;

        static readonly Parser<MessageDirective> fwdToDeadLetters =
            from _ in reserved("forward-to-dead-letters")
            select new ForwardToDeadLetters() as MessageDirective;

        static readonly Parser<MessageDirective> stayInQueue =
            from _ in reserved("stay-in-queue")
            select new StayInQueue() as MessageDirective;

        static readonly Parser<MessageDirective> fwdToProcess =
            from _ in reserved("forward-to-process")
            from pid in processId
            select new ForwardToProcess(pid) as MessageDirective;

        static Parser<MessageDirective> msgDirective =>
            choice(
                fwdToDeadLetters,
                fwdToSelf,
                fwdToParent,
                fwdToProcess,
                stayInQueue);

        static Parser<Directive> directive =>
            choice(
                reserved("resume").Map(_ => Directive.Resume),
                reserved("restart").Map(_ => Directive.Restart),
                reserved("stop").Map(_ => Directive.Stop),
                reserved("escalate").Map(_ => Directive.Escalate));

        
        static Parser<SettingValue> directiveAttr(string name) =>
            from d in directive
            select SettingValue.Directive(name, d);

        readonly static Parser<State<Exception, Option<Directive>>> exceptionDirective =
            from b in symbol("|")
            from t in token(type)
            from a in symbol("->")
            from d in token(directive)
            select Strategy.With(d, t);

        readonly static Parser<State<Exception, Option<Directive>>> otherwiseDirective =
            from b in symbol("|")
            from t in symbol("_")
            from a in symbol("->")
            from d in token(directive)
            select Strategy.Otherwise(d);

        readonly static Parser<State<Directive, Option<MessageDirective>>> matchMessageDirective =
            from b in symbol("|")
            from d in token(directive)
            from a in symbol("->")
            from m in token(msgDirective)
            select Strategy.When(m, d);

        readonly static Parser<State<Directive, Option<MessageDirective>>> otherwiseMsgDirective =
            from b in symbol("|")
            from t in symbol("_")
            from a in symbol("->")
            from d in token(msgDirective)
            select Strategy.Otherwise(d);

        readonly static Parser<SettingValue> match =
            from _     in attempt(reserved("match"))
            from direx in many(attempt(exceptionDirective))
            from other in optional(otherwiseDirective)
            let dirs = direx.Append(other.AsEnumerable()).ToArray()
            from ok    in dirs.Length > 0
                ? result(dirs)
                : failure<State<Exception, Option<Directive>>[]>("'match' must be followed by at least one clause")
            select SettingValue.StrategyMatch("match", Strategy.Match(dirs));

        readonly static Parser<State<StrategyContext, Unit>> redirectMatch =
            from direx in many(attempt(matchMessageDirective))
            from other in optional(otherwiseMsgDirective)
            let dirs = direx.Append(other.AsEnumerable()).ToArray()
            from ok in dirs.Length > 0
                ? result(dirs)
                : failure<State<Directive, Option<MessageDirective>>[]>("'redirect when' must be followed by at least one clause")
            select Strategy.Redirect(dirs);

        readonly static Parser<SettingValue> redirect =
            from n  in attempt(reserved("redirect"))
            from t  in either(attempt(symbol(":")), reserved("when"))
            from r  in t == ":"
               ? from d in token(msgDirective)
                 select Strategy.Redirect(d)
               : redirectMatch
            select SettingValue.StrategyRedirect("redirect", r);

        static Func<SettingSpec[], Func<string, Parser<SettingValue>>> strategyAttr =>
            strategySettings =>
                name =>
                    from type in attempt(either(reserved("all-for-one"), reserved("one-for-one")))
                    from _    in symbol(":")
                    from ss   in settings(strategySettings)
                    select SettingValue.Strategy(name, strategySettings, type, ss);

        static Func<ArgumentType, Func<string, Parser<SettingValue>>> arrayAttr =>
            type =>
                name => 
                    brackets(
                        from xs in commaSep(
                            type.Tag == ArgumentTypeTag.Time ? timeAttr(name)
                          : type.Tag == ArgumentTypeTag.Int ? integerAttr(name)
                          : type.Tag == ArgumentTypeTag.String ? stringAttr(name)
                          : type.Tag == ArgumentTypeTag.Double ? doubleAttr(name)
                          : type.Tag == ArgumentTypeTag.ProcessId ? processIdAttr(name)
                          : type.Tag == ArgumentTypeTag.ProcessName ? processNameAttr(name)
                          : type.Tag == ArgumentTypeTag.ProcessFlags ? flagsAttr(name)
                          : type.Tag == ArgumentTypeTag.Directive ? directiveAttr(name)
                          : type.Tag == ArgumentTypeTag.Process ? processAttr(type.Spec)(name)
                          : type.Tag == ArgumentTypeTag.Strategy ? strategyAttr(type.Spec)(name)
                          : type.Tag == ArgumentTypeTag.Array ? failure<SettingValue>("Nested arrays not allowed")
                          : failure<SettingValue>("Not supported argument type: " + type))
                        select SettingValue.Array(
                            name,
                              type.Tag == ArgumentTypeTag.Time ? (object)xs.Map(x => (Time)x.Value).Freeze()
                            : type.Tag == ArgumentTypeTag.Int ? (object)xs.Map(x => (int)x.Value).Freeze()
                            : type.Tag == ArgumentTypeTag.String ? (object)xs.Map(x => (string)x.Value).Freeze()
                            : type.Tag == ArgumentTypeTag.Double ? (object)xs.Map(x => (double)x.Value).Freeze()
                            : type.Tag == ArgumentTypeTag.ProcessId ? (object)xs.Map(x => (ProcessId)x.Value).Freeze()
                            : type.Tag == ArgumentTypeTag.ProcessName ? (object)xs.Map(x => (ProcessName)x.Value).Freeze()
                            : type.Tag == ArgumentTypeTag.ProcessFlags ? (object)xs.Map(x => (ProcessFlags)x.Value).Freeze()
                            : type.Tag == ArgumentTypeTag.Directive ? (object)xs.Map(x => (Directive)x.Value).Freeze()
                            : type.Tag == ArgumentTypeTag.Process ? (object)xs.Map(x => (ProcessSettings)x.Value).Freeze()
                            : type.Tag == ArgumentTypeTag.Strategy ? (object)xs.Map(x => (StrategySettings)x.Value).Freeze()
                            : (object)null,
                            type));


        /// <summary>
        /// Parses a named argument: name = value
        /// </summary>
        static Parser<SettingValue> namedArgument(string settingName, ArgumentSpec spec) =>
            attempt(
                token(
                    spec.Type.Tag == ArgumentTypeTag.Time ? attr(spec.Name, timeAttr)
                    : spec.Type.Tag == ArgumentTypeTag.Int ? attr(spec.Name, integerAttr)
                    : spec.Type.Tag == ArgumentTypeTag.String ? attr(spec.Name, stringAttr)
                    : spec.Type.Tag == ArgumentTypeTag.Double ? attr(spec.Name, doubleAttr)
                    : spec.Type.Tag == ArgumentTypeTag.ProcessId ? attr(spec.Name, processIdAttr)
                    : spec.Type.Tag == ArgumentTypeTag.ProcessName ? attr(spec.Name, processNameAttr)
                    : spec.Type.Tag == ArgumentTypeTag.ProcessFlags ? attr(spec.Name, flagsAttr)
                    : spec.Type.Tag == ArgumentTypeTag.Directive ? attr(spec.Name, directiveAttr)
                    : spec.Type.Tag == ArgumentTypeTag.Array ? attr(spec.Name, arrayAttr(spec.Type.GenericType))
                    : spec.Type.Tag == ArgumentTypeTag.Process ? attr(spec.Name, processAttr(spec.Type.Spec))
                    : spec.Type.Tag == ArgumentTypeTag.Strategy ? attr(spec.Name, strategyAttr(spec.Type.Spec))
                    : failure<SettingValue>("Unknown argument type: " + spec.Type.Tag)));


        /// <summary>
        /// Parses a single non-named argument
        /// </summary>
        static Parser<SettingValue> argument(string settingName, ArgumentSpec spec) =>
            attempt(
                token(
                    spec.Type.Tag == ArgumentTypeTag.Time ? timeAttr(spec.Name)
                    : spec.Type.Tag == ArgumentTypeTag.Int ? integerAttr(spec.Name)
                    : spec.Type.Tag == ArgumentTypeTag.String ? stringAttr(spec.Name)
                    : spec.Type.Tag == ArgumentTypeTag.Double ? doubleAttr(spec.Name)
                    : spec.Type.Tag == ArgumentTypeTag.ProcessId ? processIdAttr(spec.Name)
                    : spec.Type.Tag == ArgumentTypeTag.ProcessName ? processNameAttr(spec.Name)
                    : spec.Type.Tag == ArgumentTypeTag.ProcessFlags ? flagsAttr(spec.Name)
                    : spec.Type.Tag == ArgumentTypeTag.Directive ? directiveAttr(spec.Name)
                    : spec.Type.Tag == ArgumentTypeTag.Array ? arrayAttr(spec.Type.GenericType)(spec.Name)
                    : spec.Type.Tag == ArgumentTypeTag.Process ? processAttr(spec.Type.Spec)(spec.Name)
                    : spec.Type.Tag == ArgumentTypeTag.Strategy ? strategyAttr(spec.Type.Spec)(spec.Name)
                    : failure<SettingValue>("Unknown argument type: " + spec.Type.Tag)));


        /// <summary>
        ///  Parses many arguments, wrapped in ( )
        /// </summary>
        static Parser<Lst<SettingValue>> argumentMany(string settingName, ArgumentsSpec spec) =>
            //from o in symbol("(")
            from a in commaSep1(choice(spec.Args.Map(arg => namedArgument(settingName, arg))))
            //from c in symbol(")")
            from r in a.Count == spec.Args.Length
                ? result(a)
                : failure<Lst<SettingValue>>("Invalid arguments for " + settingName)
            select r;

        /// <summary>
        /// Parses the arguments for a setting
        /// </summary>
        /// <param name="settingName"></param>
        /// <param name="spec"></param>
        /// <returns></returns>
        static Parser<Lst<SettingValue>> Arguments(string settingName, ArgumentsSpec spec) =>
            spec.Args.Length == 0
                ? failure<Lst<SettingValue>>("Invalid arguments spec, has zero arguments")
                : spec.Args.Length == 1
                    ? from a in argument(settingName, spec.Args.Head())
                      select List.create(a)
                    : argumentMany(settingName, spec);


        static Parser<Lst<SettingToken>> settings(SettingSpec[] settingSpecs) =>
            from settings in many(
                choice(
                    settingSpecs.Map(setting =>
                        setting.Name == "match"    ? from m in match
                                                     select new SettingToken(setting.Name, null, m)
                      : setting.Name == "redirect" ? from r in redirect
                                                     select new SettingToken(setting.Name, null, r)
                      : from nam in attempt(reserved(setting.Name))
                        from _ in symbol(":")
                        from tok in choice(setting.Variants.Map(variant =>
                            from vals in Arguments(nam, variant)
                            select new SettingToken(setting.Name, variant, vals.ToArray())))
                        select tok)))
            select List.createRange(settings);

        public Parser<Map<string, SettingToken>> Settings;


        public SystemConfigParser(params SettingSpec[] settingSpecs)
        {
            Settings = from ws in whiteSpace
                       from ss in settings(settingSpecs)
                       select Map.createRange(ss.Map(x => Tuple(x.Name, x)));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using static LanguageExt.Parsec;

namespace LanguageExt
{
    public static class ActorConfigParser
    {
        readonly static Parser<ActorConfigToken> pid =
            from _   in symbol("pid")
            from __  in symbol(":")
            from pid in stringLiteral
            select new PidToken(new ProcessId(pid)) as ActorConfigToken;

        static Parser<ProcessFlags> flagMap(string name, ProcessFlags flag) =>
            from x in symbol(name)
            select flag;

        readonly static Parser<ProcessFlags> flag =
            choice(
                flagMap("default", ProcessFlags.Default),
                flagMap("listen-remote-and-local", ProcessFlags.ListenRemoteAndLocal),
                flagMap("persist-all", ProcessFlags.PersistAll),
                flagMap("persist-inbox", ProcessFlags.PersistInbox),
                flagMap("persist-state", ProcessFlags.PersistState),
                flagMap("remote-publish", ProcessFlags.RemotePublish),
                flagMap("remote-state-publish", ProcessFlags.RemoteStatePublish));

        readonly static Parser<ActorConfigToken> flags =
            (from _  in symbol("flags")
             from __ in symbol(":")
             from fs in commaBrackets(flag)
             select new FlagsToken(List.fold(fs, ProcessFlags.Default, (s, x) => s | x)) as ActorConfigToken);

        readonly static Parser<ActorConfigToken> maxMailboxSize =
            from _  in symbol("mailbox-size")
            from __ in symbol(":")
            from sz in token(natural)
            select new MailboxSizeToken(sz) as ActorConfigToken;

        static Parser<Attr> numericAttr(string name) =>
            from x in symbol(name)
            from _ in symbol("=")
            from v in token(integer)
            select new NumericAttr(name, v) as Attr;

        static Parser<Attr> stringAttr(string name) =>
            from x in symbol(name)
            from _ in symbol("=")
            from s in stringLiteral
            select new StringAttr(name, s) as Attr;

        static Parser<MessageDirective> msgDirective =>
            from x in choice(
                symbol("forward-to-dead-letters"),
                symbol("forward-to-self"),
                symbol("forward-to-parent"),
                symbol("forward-to-process"),
                symbol("stay-in-queue"))
            from dir in x == "forward-to-process"     ? from pid in stringLiteral
                                                        select new ForwardToProcess(new ProcessId(pid)) as MessageDirective
                      : x == "stay-in-queue"          ? result(new ForwardToSelf() as MessageDirective)
                      : x == "forward-to-dead-parent" ? result(new ForwardToParent() as MessageDirective)
                      : x == "forward-to-dead-self"   ? result(new ForwardToSelf() as MessageDirective)
                      : result(new ForwardToDeadLetters() as MessageDirective)
            select dir;

        static Parser<Directive> directive =>
            from x in choice(
                symbol("resume"),
                symbol("restart"),
                symbol("stop"),
                symbol("escalate"))
            select x == "escalate" ? Directive.Escalate
                 : x == "restart"  ? Directive.Restart
                 : x == "resume"   ? Directive.Resume
                 : Directive.Stop;

        static Parser<Attr> strAttr(string name) =>
            from x in symbol(name)
            from _ in symbol("=")
            from s in stringLiteral
            select new StringAttr(name, s) as Attr;

        static Parser<Attr> timeAttr(string name) =>
            from x in symbol(name)
            from _ in symbol("=")
            from v in token(integer)
            from u in choice(
                symbol("seconds"),
                symbol("second"),
                symbol("secs"),
                symbol("sec"),
                symbol("s"),
                symbol("minutes"),
                symbol("minute"),
                symbol("mins"),
                symbol("min"),
                symbol("milliseconds"),
                symbol("millisecond"),
                symbol("ms"),
                symbol("hours"),
                symbol("hour"),
                symbol("hr"))
               .label("Unit of time (e.g. seconds, mins, hours, hr, sec, min...)")
            select new TimeAttr(name, v, u) as Attr;

        static Parser<List<Attr>> stratAttrs(string name, params Parser<Attr>[] attrs) =>
            from n in symbol(name)
            from _ in symbol(":")
            from a in sepBy1(choice(attrs), token(ch(',')))
            select a.ToList();

        readonly static Parser<State<StrategyContext, Unit>> backoff =
            from attrs in stratAttrs("back-off", timeAttr("min"), timeAttr("max"), timeAttr("step"), timeAttr("duration"))
            select attrs.Count == 1
                ? Strategy.Backoff(attrs.GetTimeAttr("duration"))
                : Strategy.Backoff(attrs.GetTimeAttr("min"), attrs.GetTimeAttr("max"), attrs.GetTimeAttr("step"));

        readonly static Parser<State<StrategyContext, Unit>> pause =
            from attrs in stratAttrs("pause", timeAttr("duration"))
            select Strategy.Pause(attrs.GetTimeAttr("duration"));

        readonly static Parser<State<StrategyContext, Unit>> always =
            from n in symbol("always")
            from _ in symbol(":")
            from d in token(directive)
            select Strategy.Always(d);

        readonly static Parser<State<StrategyContext, Unit>> redirect =
            from n in symbol("redirect")
            from _ in symbol(":")
            from d in token(msgDirective)
            select Strategy.Redirect(d);

        readonly static Parser<Type> type =
            from x  in letter
            from xs in many1(choice(letter, ch('.'), ch('_')))
            select Type.GetType(new string(x.Cons(xs).ToArray()));

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
            from d in directive
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

        readonly static Parser<State<StrategyContext, Unit>> match =
            from _ in symbol("match")
            from direx in many1(exceptionDirective)
            from other in optional(otherwiseDirective)
            select Strategy.Match(direx.Append(other.AsEnumerable()).ToArray());

        readonly static Parser<State<StrategyContext, Unit>> redirectMatch =
            from _ in symbol("redirect")
            from direx in many1(matchMessageDirective)
            from other in optional(otherwiseMsgDirective)
            select Strategy.Redirect(direx.Append(other.AsEnumerable()).ToArray());

        readonly static Parser<State<StrategyContext, Unit>> retries =
            from attrs in stratAttrs("retries", numericAttr("count"), timeAttr("duration"))
            select attrs.Count == 1
                ? Strategy.Retries(attrs.GetNumericAttr("count"))
                : Strategy.Retries(attrs.GetNumericAttr("count"), attrs.GetTimeAttr("duration"));

        readonly static Parser<Tuple<string, string>> setting =
            (from key in token(ident)
             from _   in symbol(":")
             from val in stringLiteral
             select Tuple.Create(key, val))
            .label("setting");

        readonly static Parser<ActorConfigToken> settings =
            from n in symbol("settings")
            from _ in symbol(":")
            from s in many1(setting)
            select new SettingsToken(Map.createRange(s)) as ActorConfigToken;

        readonly static Parser<IEnumerable<State<StrategyContext, Unit>>> strategies =
            many1(
                choice(
                    retries,
                    backoff,
                    always,
                    redirectMatch,
                    redirect,
                    match));

        readonly static Parser<State<StrategyContext, Unit>> oneForOne =
            from a in symbol("one-for-one")
            from b in symbol(":")
            from attrs in strategies
            select Strategy.OneForOne(attrs.ToArray());

        readonly static Parser<State<StrategyContext, Unit>> allForOne =
            from a in symbol("all-for-one")
            from b in symbol(":")
            from attrs in strategies
            select Strategy.AllForOne(attrs.ToArray());

        readonly static Parser<ActorConfigToken> strategy =
            from a in symbol("strategy")
            from b in symbol(":")
            from s in either(oneForOne, allForOne)
            select new StrategyToken(s) as ActorConfigToken;

        public readonly static Parser<ActorConfig> Parser =
            from _      in junk
            from tokens in many1(
                choice(
                    pid, 
                    flags, 
                    strategy, 
                    settings,
                    maxMailboxSize))
            select new ActorConfig(tokens);
    }
}

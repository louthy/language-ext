using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.Parsec.Common;
using static LanguageExt.Parsec.Internal;
using static LanguageExt.Parsec.Char;
using static LanguageExt.Parsec.ParserResult;

namespace LanguageExt.Parsec
{
    /// <summary>
    /// The primitive parser combinators
    /// </summary>
    public static class Prim
    {
        static Prim()
        {
            unitp = inp => EmptyOK(unit, inp);
            getPos = (PString inp) => ConsumedOK(inp.Pos, inp);
            getIndex = (PString inp) => ConsumedOK(inp.Index, inp);
            eof = notFollowedBy(satisfy(_ => true)).label("end of input");
        }

        /// <summary>
        /// Run the parser p with the input provided
        /// </summary>
        public static ParserResult<T> parse<T>(Parser<T> p, PString input) =>
            p.Parse(input);

        /// <summary>
        /// Run the parser p with the input provided
        /// </summary>
        public static ParserResult<T> parse<T>(Parser<T> p, string input) =>
            p.Parse(input);

        /// <summary>
        /// Lazy parser - useful in recursive scenarios.
        /// </summary>
        public static Parser<T> lazyp<T>(Func<Parser<T>> fn) =>
            inp => fn()(inp);

        /// <summary>
        /// This parser is useful to put at the top of LINQ expressions, it
        /// makes it easier to put breakpoints on the actual first parser
        /// in an expression.  It returns unit
        /// </summary>
        public static readonly Parser<Unit> unitp;

        /// <summary>
        /// Special parser for setting user-state that propagates 
        /// through the computation.
        /// </summary>
        public static Parser<Unit> setState<T>(T state) =>
            inp => ConsumedOK(unit, inp.SetUserState(state));

        /// <summary>
        /// Special parser for getting user-state that was previously
        /// set with setState
        /// </summary>
        public static Parser<T> getState<T>() =>
            inp =>
                match(inp.UserState,
                    Some: x => x is T
                        ? ConsumedOK((T)x, inp)
                        : EmptyError<T>(ParserError.Message(inp.Pos, "User state type-mismatch")),
                    None: () => EmptyError<T>(ParserError.Message(inp.Pos, "No user state set")));

        /// <summary>
        /// Get the current position of the parser in the source as a line
        /// and column index (starting at 1 for both)
        /// </summary>
        public static readonly Parser<Pos> getPos;

        /// <summary>
        /// Get the current index into the source
        /// </summary>
        public static readonly Parser<int> getIndex;

        /// <summary>
        /// The parser unexpected(msg) always fails with an Unexpect error
        /// message msg without consuming any input.
        /// </summary>
        /// <remarks>
        /// The parsers 'failure', 'label' and 'unexpected' are the three parsers
        /// used to generate error messages.  Of these, only 'label' is commonly
        /// used.  For an example of the use of unexpected, see the definition
        /// of 'Text.Parsec.Combinator.notFollowedBy'.
        /// </remarks>
        /// <param name="msg">Error message to use when parsed</param>
        public static Parser<T> unexpected<T>(string msg) =>
            inp => EmptyError<T>(ParserError.Unexpect(inp.Pos, msg));

        /// <summary>
        /// The parser failure(msg) always fails with a Message error
        /// without consuming any input.
        /// 
        /// The parsers 'failure', 'label' and 'unexpected' are the three parsers
        /// used to generate error messages.  Of these, only 'label' is commonly
        /// used.  For an example of the use of unexpected, see the definition
        /// of 'Text.Parsec.Combinator.notFollowedBy'.
        /// </summary>
        /// <param name="msg">Error message to use when parsed</param>
        public static Parser<T> failure<T>(string msg) =>
            inp => EmptyError<T>(ParserError.Message(inp.Pos, msg));

        /// <summary>
        /// Always success parser.  Returns the value provided.  
        /// This is monad return for the Parser monad
        /// </summary>
        public static Parser<T> result<T>(T value) =>
            inp => EmptyOK(value, inp);

        /// <summary>
        /// Always fails (with an Unknown error) without consuming any input
        /// </summary>
        public static Parser<T> zero<T>() =>
            inp => EmptyError<T>(ParserError.Unknown(inp.Pos));

        /// <summary>
        /// This combinator implements choice. The parser either(p,q) first
        /// applies p.  If it succeeds, the value of p is returned.  If p
        /// fails /without consuming any input/, parser q is tried.  
        /// </summary>
        /// <remarks>
        /// This combinator is the mplus behaviour of the Parser monad.
        /// 
        /// The parser is called /predictive/ since q is only tried when
        /// parser p didn't consume any input (i.e.. the look ahead is 1).
        /// 
        /// This non-backtracking behaviour allows for both an efficient
        /// implementation of the parser combinators and the generation of good
        /// error messages.
        /// </remarks>
        public static Parser<T> either<T>(Parser<T> p, Parser<T> q) =>
            inp =>
            {
                var m = p(inp);

                // meerr
                if (m.Tag == ResultTag.Empty && m.Reply.Tag == ReplyTag.Error)
                {
                    var n = q(inp);

                    // neok
                    if (n.Tag == ResultTag.Empty && n.Reply.Tag == ReplyTag.OK)
                    {
                        return EmptyOK(n.Reply.Result, n.Reply.State, mergeError(m.Reply.Error, n.Reply.Error));
                    }

                    // nerr
                    if (n.Tag == ResultTag.Empty && n.Reply.Tag == ReplyTag.Error)
                    {
                        return EmptyError<T>(mergeError(m.Reply.Error, n.Reply.Error));
                    }

                    // cerr, cok
                    return n;
                }

                // cok, cerr, eok
                return m;
            };

        /// <summary>
        /// choice(ps) tries to apply the parsers in the list ps in order, until one 
        /// of them succeeds. 
        /// </summary>
        /// <returns>
        /// The value of the succeeding parser.
        /// </returns>
        public static Parser<T> choice<T>(params Parser<T>[] ps) =>
            choicei(toSeq(ps));

        /// <summary>
        /// choice(ps) tries to apply the parsers in the list ps in order, until one 
        /// of them succeeds. 
        /// </summary>
        /// <returns>
        /// The value of the succeeding parser.
        /// </returns>
        public static Parser<T> choice<T>(Seq<Parser<T>> ps) =>
            choicei(ps);

        /// <summary>
        /// Runs a sequence of parsers, if any fail then the failure state is
        /// returned immediately and subsequence parsers are not run.  
        /// </summary>
        /// <returns>
        /// The result of each parser as an enumerable.
        /// </returns>
        public static Parser<Seq<T>> chain<T>(params Parser<T>[] ps) =>
            chaini(toSeq(ps));

        /// <summary>
        /// Runs a sequence of parsers, if any fail then the failure state is
        /// returned immediately and subsequence parsers are not run.  
        /// </summary>
        /// <returns>
        /// The result of each parser as an enumerable.
        /// </returns>
        public static Parser<Seq<T>> chain<T>(Seq<Parser<T>> ps) =>
            chaini(ps);

        /// <summary>
        /// Cons for parser results
        /// </summary>
        /// <param name="p"></param>
        /// <param name="ps"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Parser<Seq<T>> cons<T>(Parser<T> p, Parser<Seq<T>> ps) =>
            from x in p
            from xs in ps
            select x.Cons(xs);
        
        /// <summary>
        /// Flattens parser result: Seq of Seq of T => Seq of T
        /// </summary>
        /// <returns>Parser with flattened result sequence</returns>
        public static Parser<Seq<T>> flatten<T>(Parser<Seq<Seq<T>>> ssp) =>
            from xss in ssp
            select xss.Flatten();

        /// <summary>
        /// The parser attempt(p) behaves like parser p, except that it
        /// pretends that it hasn't consumed any input when an error occurs.
        /// 
        /// This combinator is used whenever arbitrary look ahead is needed.
        /// Since it pretends that it hasn't consumed any input when p fails,
        /// the either combinator will try its second alternative even when the
        /// first parser failed while consuming input.
        /// 
        /// See remarks.
        /// </summary>
        /// <remarks>
        /// The attempt combinator can for example be used to distinguish
        /// identifiers and reserved words.  Both reserved words and identifiers
        /// are a sequence of letters.  Whenever we expect a certain reserved
        /// word where we can also expect an identifier we have to use the attempt
        /// combinator.  Suppose we write:
        /// 
        ///    var expr        = either(letExpr, identifier).label("expression");
        ///  
        ///    var letExpr     = from x in str("let")
        ///                      ...
        ///                      select ...;
        ///                      
        ///    var identifier  = many1(letter);
        /// 
        ///  If the user writes "lexical", the parser fails with: unexpected
        ///  "x", expecting "t" in "let".  Indeed, since the either combinator
        ///  only tries alternatives when the first alternative hasn't consumed
        ///  input, the identifier parser is never tried  (because the prefix
        ///  "le" of the str("let") parser is already consumed). The right behaviour 
        ///  can be obtained by adding the attempt combinator:
        /// 
        ///    var expr        = either(letExpr, identifier).label("expression");
        ///  
        ///    var letExpr     = from x in attempt(str("let"))
        ///                      ...
        ///                      select ...;
        ///                      
        ///    var identifier  = many1(letter);
        ///  
        ///  </remarks>
        public static Parser<T> attempt<T>(Parser<T> p) =>
            inp =>
            {
                var res = p(inp);
                if (res.Tag == ResultTag.Consumed && res.Reply.Tag == ReplyTag.Error)
                {
                    return EmptyError<T>(res.Reply.Error);
                }
                else
                {
                    return res;
                }
            };

        /// <summary>
        /// lookAhead(p) parses p without consuming any input.
        /// 
        /// If p fails and consumes some input, so does lookAhead(p). Combine with 
        /// 'attempt' if this is undesirable.
        /// </summary>
        public static Parser<T> lookAhead<T>(Parser<T> p) =>
            inp =>
            {
                var res = p(inp);
                if (res.Reply.Tag == ReplyTag.OK)
                {
                    return EmptyOK(res.Reply.Result, inp);
                }
                else
                {
                    return res;
                }
            };

        /// <summary>
        /// many(p) applies the parser p zero or more times.
        /// </summary>
        /// <example>
        ///     var identifier  = from c in letter
        ///                       from cs in many(letterOrDigit)
        ///                       select c.Cons(cs)
        /// </example>
        /// <returns>
        /// Enumerable of the returned values of p.
        /// </returns>
        public static Parser<Seq<T>> many<T>(Parser<T> p) =>
            inp =>
            {
                var current = inp;
                var results = new List<T>();
                ParserError error = null;

                while(true)
                {
                    var t = p(current);

                    // cok
                    if (t.Tag == ResultTag.Consumed && t.Reply.Tag == ReplyTag.OK)
                    {
                        results.Add(t.Reply.Result);
                        current = t.Reply.State;
                        error = t.Reply.Error;
                        continue;
                    }

                    // eok
                    if (t.Tag == ResultTag.Empty && t.Reply.Tag == ReplyTag.OK)
                    {
                        // eok, eerr
                        return EmptyError<Seq<T>>(new ParserError(ParserErrorTag.SysUnexpect, current.Pos, "many: combinator 'many' is applied to a parser that accepts an empty string.", List.empty<string>()));
                    }

                    // cerr
                    if (t.Tag == ResultTag.Consumed && t.Reply.Tag == ReplyTag.Error)
                    {
                        return ConsumedError<Seq<T>>(mergeError(error, t.Reply.Error));
                    }

                    // eerr
                    return EmptyOK(toSeq(results), current, mergeError(error, t.Reply.Error));
                }
            };

        /// <summary>
        /// manyn(p, n) applies the parser p n times.
        /// </summary>
        /// <example>
        ///     var identifier  = from c in letter
        ///                       from cs in manyn(letterOrDigit, 4)
        ///                       select c.Cons(cs)
        /// </example>
        /// <returns>
        /// Enumerable of the returned values of p.
        /// </returns>
        public static Parser<Seq<T>> manyn<T>(Parser<T> p, int n) =>
            inp =>
            {
                var current = inp;
                var results = new List<T>();
                ParserError error = null;

                int count = 0;

                while (true)
                {
                    var t = p(current);
                    count++;

                    // cok
                    if (t.Tag == ResultTag.Consumed && t.Reply.Tag == ReplyTag.OK)
                    {
                        results.Add(t.Reply.Result);
                        current = t.Reply.State;
                        error = t.Reply.Error;
                        if (count == n)
                        {
                            return EmptyOK(toSeq(results), current, mergeError(error, t.Reply.Error));
                        }
                        continue;
                    }

                    // eok
                    if (t.Tag == ResultTag.Empty && t.Reply.Tag == ReplyTag.OK)
                    {
                        // eok, eerr
                        return EmptyError<Seq<T>>(new ParserError(ParserErrorTag.SysUnexpect, current.Pos, "many: combinator 'manyn' is applied to a parser that accepts an empty string.", List.empty<string>()));
                    }

                    // cerr
                    if (t.Tag == ResultTag.Consumed && t.Reply.Tag == ReplyTag.Error)
                    {
                        return ConsumedError<Seq<T>>(mergeError(error, t.Reply.Error));
                    }

                    // eerr
                    return EmptyError<Seq<T>>(mergeError(error, t.Reply.Error));
                }
            };

        /// <summary>
        /// manyn0(p) applies the parser p zero or up to a maximum of n times.
        /// </summary>
        /// <example>
        ///     var identifier  = from c in letter
        ///                       from cs in manyn0(letterOrDigit, 4)
        ///                       select c.Cons(cs)
        /// </example>
        /// <returns>
        /// Enumerable of the returned values of p.
        /// </returns>
        public static Parser<Seq<T>> manyn0<T>(Parser<T> p, int n) =>
            n <= 0 
                ? result(Seq<T>.Empty)
                : inp =>
                {
                    var current = inp;
                    var results = new List<T>();
                    ParserError error = null;

                    int count = 0;

                    while (true)
                    {
                        var t = p(current);
                        count++;

                        // cok
                        if (t.Tag == ResultTag.Consumed && t.Reply.Tag == ReplyTag.OK)
                        {
                            results.Add(t.Reply.Result);
                            current = t.Reply.State;
                            error = t.Reply.Error;
                            if (count == n)
                            {
                                return EmptyOK(toSeq(results), current, mergeError(error, t.Reply.Error));
                            }

                            continue;
                        }

                        // eok
                        if (t.Tag == ResultTag.Empty && t.Reply.Tag == ReplyTag.OK)
                        {
                            // eok, eerr
                            return EmptyError<Seq<T>>(new ParserError(ParserErrorTag.SysUnexpect, current.Pos, "many: combinator 'manyn0' is applied to a parser that accepts an empty string.", List.empty<string>()));
                        }

                        // cerr
                        if (t.Tag == ResultTag.Consumed && t.Reply.Tag == ReplyTag.Error)
                        {
                            return ConsumedError<Seq<T>>(mergeError(error, t.Reply.Error));
                        }

                        // eerr
                        return EmptyOK(toSeq(results), current, mergeError(error, t.Reply.Error));
                    }
                };

        /// <summary>
        /// manyn1(p) applies the parser p one or up to a maximum of n times.
        /// </summary>
        /// <returns>
        /// Enumerable of the returned values of p.
        /// </returns>
        public static Parser<Seq<T>> manyn1<T>(Parser<T> p, int n) =>
            from x in p
            from xs in manyn0(p, n - 1)
            select x.Cons(xs);

        /// <summary>
        /// many1(p) applies the parser p one or more times.
        /// </summary>
        /// <returns>
        /// Enumerable of the returned values of p.
        /// </returns>
        public static Parser<Seq<T>> many1<T>(Parser<T> p) =>
            from x in p
            from xs in many(p)
            select x.Cons(xs);

        /// <summary>
        /// skipMany(p) applies the parser p zero or more times, skipping
        /// its result.
        /// </summary>
        public static Parser<Unit> skipMany<T>(Parser<T> p) =>
            either(skipMany1(p), result(unit));

        /// <summary>
        /// skipMany(p) applies the parser p one or more times, skipping
        /// its result.
        /// </summary>
        public static Parser<Unit> skipMany1<T>(Parser<T> p) =>
            from x  in p
            from xs in many(p)
            select unit;

        /// <summary>
        /// optionOrElse(x, p) tries to apply parser p. If p fails without
        /// consuming input, it returns the value x, otherwise the value
        /// returned by p.
        /// </summary>
        public static Parser<T> optionOrElse<T>(T x, Parser<T> p) =>
            either(p, result(x));

        /// <summary>
        /// optional(p) tries to apply parser p.  If p fails without
        /// consuming input, it return 'None', otherwise it returns
        /// 'Some' the value returned by p.
        /// </summary>
        public static Parser<Option<T>> optional<T>(Parser<T> p) =>
            inp =>
            {
                var r = p.Map(x => Option<T>.Some(x))(inp);
                return r.Reply.Tag == ReplyTag.OK
                    ? r
                    : EmptyOK(Option<T>.None, inp);
            };

        /// <summary>
        /// optionalList(p) tries to apply parser p.  If p fails without
        /// consuming input, it return [], otherwise it returns a one 
        /// item Lst with the result of p.
        /// </summary>
        /// <returns>A list of 0 or 1 parsed items</returns>
        public static Parser<Lst<T>> optionalList<T>(Parser<T> p) =>
            inp =>
            {
                var r = p.Map(x => List.create(x))(inp);
                return r.Reply.Tag == ReplyTag.OK
                    ? r
                    : EmptyOK(List.empty<T>(), inp);
            };

        /// <summary>
        /// optionalSeq(p) tries to apply parser p.  If p fails without
        /// consuming input, it return an empty IEnumerable, otherwise it returns 
        /// a one item IEnumerable with the result of p.
        /// </summary>
        /// <returns>A list of 0 or 1 parsed items</returns>
        public static Parser<Seq<T>> optionalSeq<T>(Parser<T> p) =>
            inp =>
            {
                var r = p.Map(x => x.Cons())(inp);
                return r.Reply.Tag == ReplyTag.OK
                    ? r
                    : EmptyOK(Seq<T>.Empty, inp);
            };

        /// <summary>
        /// optionalArray(p) tries to apply parser p.  If p fails without
        /// consuming input, it return [], otherwise it returns a one 
        /// item array with the result of p.
        /// </summary>
        /// <returns>A list of 0 or 1 parsed items</returns>
        public static Parser<T[]> optionalArray<T>(Parser<T> p) =>
            inp =>
            {
                var r = p.Map(x => new[] { x })(inp);
                return r.Reply.Tag == ReplyTag.OK
                    ? r
                    : EmptyOK(new T [0], inp);
            };

        /// <summary>
        /// between(open,close,p) parses open, followed by p and close.
        /// </summary>
        /// <returns>
        /// The value returned by p.
        /// </returns>
        public static Parser<T> between<L, R, T>(Parser<L> open, Parser<R> close, Parser<T> inner) =>
            from l in open
            from v in inner
            from r in close
            select v;

        /// <summary>
        /// sepBy1(p,sep) parses one or more occurrences of p, separated
        /// by sep. 
        /// </summary>
        /// <returns>
        /// A list of values returned by p.
        /// </returns>
        public static Parser<Seq<T>> sepBy1<S, T>(Parser<T> p, Parser<S> sep) =>
            from x in p
            from xs in many(from _ in sep
                            from y in p
                            select y)
            select x.Cons(xs);

        /// <summary>
        /// sepBy(p,sep) parses zero or more occurrences of p, separated
        /// by sep. 
        /// </summary>
        /// <returns>
        /// A list of values returned by p.
        /// </returns>
        public static Parser<Seq<T>> sepBy<S, T>(Parser<T> p, Parser<S> sep) =>
            either(sepBy1(p, sep), result(Seq<T>.Empty));

        /// <summary>
        /// sepEndBy1(p,sep) parses one or more occurrences of p,
        /// separated and optionally ended by sep. 
        /// </summary>
        /// <returns>
        /// A list of values returned by p.
        /// </returns>
        public static Parser<Seq<T>> sepEndBy1<S, T>(Parser<T> p, Parser<S> sep) =>
            from x in p
            from xs in either(from _ in sep
                              from ys in sepEndBy(p, sep)
                              select ys,
                              result(Seq<T>.Empty))
            select x.Cons(xs);

        /// <summary>
        /// sepEndBy(p,sep) parses zero or more occurrences of p,
        /// separated and optionally ended by sep. 
        /// </summary>
        /// <returns>
        /// A list of values returned by p.
        /// </returns>
        public static Parser<Seq<T>> sepEndBy<S, T>(Parser<T> p, Parser<S> sep) =>
            either(sepEndBy1(p, sep), result(Seq<T>.Empty));

        /// <summary>
        /// endBy1(p,sep) parses one or more occurrences of p, separated
        /// and ended by sep.
        /// </summary>
        /// <returns>
        /// A list of values returned by p.
        /// </returns>
        public static Parser<Seq<T>> endBy1<S, T>(Parser<T> p, Parser<S> sep) =>
            many1(from x in p
                  from _ in sep
                  select x);

        /// <summary>
        /// endBy(p,sep) parses zero or more occurrences of p, separated
        /// and ended by sep.
        /// </summary>
        /// <returns>
        /// A list of values returned by p.
        /// </returns>
        public static Parser<Seq<T>> endBy<S, T>(Parser<T> p, Parser<S> sep) =>
            many(from x in p
                 from _ in sep
                 select x);

        /// <summary>
        /// count(n,p) parses n occurrences of p. If n is smaller or
        /// equal to zero, the parser equals to result([]). 
        /// </summary>
        /// <returns>
        /// A list of values returned by p.
        /// </returns>
        public static Parser<Seq<T>> count<T>(int n, Parser<T> p) =>
            counti(n, p);

        /// <summary>
        /// chainr(p,op,x) parses zero or more occurrences of p, separated by op 
        /// </summary>
        /// <returns>
        /// a value obtained by a right associative application of all functions 
        /// returned by op to the values returned by p. If there are no occurrences 
        /// of p, the value x is returned.</returns>
        public static Parser<T> chainr<T>(Parser<T> p, Parser<Func<T, T, T>> op, T x) =>
            either(chainr1(p, op), result(x));

        /// <summary>
        /// chainl(p,op,x) parses zero or more occurrences of p, separated by op 
        /// </summary>
        /// <returns>
        /// a value obtained by a left associative application of all functions 
        /// returned by op to the values returned by p. If there are no occurrences 
        /// of p, the value x is returned.</returns>
        public static Parser<T> chainl<T>(Parser<T> p, Parser<Func<T, T, T>> op, T x) =>
            either(chainr1(p, op), result(x));

        /// <summary>
        /// chainr1(p,op) parses one or more occurrences of p, separated by op. 
        /// </summary>
        /// <returns>
        /// A value obtained by a right associative application of all functions 
        /// returned by op to the values returned by p
        /// </returns>
        public static Parser<T> chainr1<T>(Parser<T> p, Parser<Func<T, T, T>> op)
        {
            Parser<T> scan = null;

            var rest = fun((T x) => either(from f in op
                                           from y in scan
                                           select f(x, y),
                                           result(x)));

            scan = from x in p
                   from y in rest(x)
                   select y;

            return scan;
        }

        /// <summary>
        /// chainl1(p,op) parses one or more occurrences of p, separated by op. 
        /// </summary>
        /// <returns>
        /// A value obtained by a left associative application of all functions 
        /// returned by op to the values returned by p
        /// </returns>
        public static Parser<T> chainl1<T>(Parser<T> p, Parser<Func<T, T, T>> op)
        {
            Func<T, Parser<T>> rest = null;

            rest = fun((T x) => either(from f in op
                                       from y in p
                                       from r in rest(f(x, y))
                                       select r,
                                       result(x)));

            return from x in p
                   from y in rest(x)
                   select y;
        }

        /// <summary>
        /// This parser only succeeds at the end of the input. This is not a
        /// primitive parser but it is defined using 'notFollowedBy'.
        /// </summary>
        public readonly static Parser<Unit> eof;

        /// <summary>
        /// notFollowedBy(p) only succeeds when parser p fails. This parser
        /// does not consume any input.This parser can be used to implement the
        /// 'longest match' rule. 
        /// </summary>
        /// <example>For example, when recognizing keywords (for
        /// example 'let'), we want to make sure that a keyword is not followed
        /// by a legal identifier character, in which case the keyword is
        /// actually an identifier(for example 'lets'). We can program this
        /// behaviour as follows:
        /// 
        ///     var keywordLet  = attempt (from x in str("let")
        ///                                from _ in notFollowedBy letterOrDigit
        ///                                select x);
        ///                                
        /// </example>
        public static Parser<Unit> notFollowedBy<T>(Parser<T> p) =>
            attempt(
                either(from c in attempt(p)
                       from u in unexpected<Unit>(c.ToString())
                       select u,
                       result(unit)));

        /// <summary>
        /// Parse a char list and convert into a string
        /// </summary>
        public static Parser<string> asString(Parser<Seq<char>> p) =>
            p.Select(x => new string(x.ToArray()));

        /// <summary>
        /// Parse a char list and convert into an integer
        /// </summary>
        public static Parser<Option<int>> asInteger(Parser<Seq<char>> p) =>
            p.Select(x => parseInt(new string(x.ToArray())));

        /// <summary>
        /// Parse a char list and convert into an integer
        /// </summary>
        public static Parser<Option<int>> asInteger(Parser<Seq<char>> p, int fromBase) =>
            p.Select(x => parseInt(new string(x.ToArray()), fromBase));

        /// <summary>
        /// Parse a char list and convert into an double precision floating point value
        /// </summary>
        public static Parser<Option<double>> asDouble(Parser<Seq<char>> p) =>
            p.Select(x => parseDouble(new string(x.ToArray())));

        /// <summary>
        /// Parse a char list and convert into an double precision floating point value
        /// </summary>
        public static Parser<Option<float>> asFloat(Parser<Seq<char>> p) =>
            p.Select(x => parseFloat(new string(x.ToArray())));

        public static Parser<Seq<T>> manyUntil<T, U>(Parser<T> p, Parser<U> end)
        {
            Parser<Seq<T>> scan = null;

            scan = either(
                from _ in end
                select Seq<T>.Empty,
                from x  in p
                from xs in scan
                select x.Cons(xs));

            return scan;
        }
    }
}

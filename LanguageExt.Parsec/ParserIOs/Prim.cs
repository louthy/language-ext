using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.Parsec.Common;
using static LanguageExt.Parsec.InternalIO;
using static LanguageExt.Parsec.ItemIO;
using static LanguageExt.Parsec.ParserResultIO;

namespace LanguageExt.Parsec
{
    /// <summary>
    /// The primitive parser combinators
    /// </summary>
    public static class PrimIO
    {
        /// <summary>
        /// Run the parser p with the input provided
        /// </summary>
        public static ParserResult<I, O> parse<I, O>(Parser<I, O> p, PString<I> input) =>
            p.Parse(input);

        /// <summary>
        /// Run the parser p with the input provided
        /// </summary>
        public static ParserResult<I, O> parse<I, O>(Parser<I, O> p, Seq<I> input, Func<I, Pos> tokenPos) =>
            p.Parse(input, tokenPos);

        /// <summary>
        /// Lazy parser - useful in recursive scenarios.
        /// </summary>
        public static Parser<I, O> lazyp<I, O>(Func<Parser<I, O>> fn) =>
            inp => fn()(inp);

        /// <summary>
        /// This parser is useful to put at the top of LINQ expressions, it
        /// makes it easier to put breakpoints on the actual first parser
        /// in an expression.  It returns unit
        /// </summary>
        public static Parser<I, Unit> unitp<I>() =>
            inp => EmptyOK(unit, inp);

        /// <summary>
        /// Special parser for setting user-state that propagates 
        /// through the computation.
        /// </summary>
        public static Parser<I, Unit> setState<I, S>(S state) =>
            inp => ConsumedOK(unit, inp.SetUserState(state));

        /// <summary>
        /// Special parser for getting user-state that was previously
        /// set with setState
        /// </summary>
        public static Parser<I, S> getState<I, S>() =>
            inp =>
                match(inp.UserState,
                    Some: x => x is S
                        ? ConsumedOK((S)x, inp)
                        : EmptyError<I, S>(ParserError.Message(inp.Pos, "User state type-mismatch"), inp.TokenPos),
                    None: () => EmptyError<I, S>(ParserError.Message(inp.Pos, "No user state set"), inp.TokenPos));

        /// <summary>
        /// Get the current position of the parser in the source as a line
        /// and column index (starting at 1 for both)
        /// </summary>
        public static Parser<I, Pos> getPos<I>() =>
            (PString<I> inp) => ConsumedOK(inp.Pos, inp);

        /// <summary>
        /// Get the current index into the source
        /// </summary>
        public static Parser<I, int> getIndex<I>() =>
            (PString<I> inp) => ConsumedOK(inp.Index, inp);

        /// <summary>
        /// The parser unexpected(msg) always fails with an Unexpect error
        /// message msg without consuming any input.
        /// </summary>
        /// <remarks>
        /// The parsers 'failure', 'label' and 'unexpected' are the three parsers
        /// used to generate error messages.  Of these, only 'label' is commonly
        /// used.  For an example of the use of unexpected, see the definition
        /// of 'notFollowedBy'.
        /// </remarks>
        /// <param name="msg">Error message to use when parsed</param>
        public static Parser<I, O> unexpected<I, O>(string msg) =>
            inp => EmptyError<I, O>(ParserError.Unexpect(inp.Pos, msg), inp.TokenPos);

        /// <summary>
        /// The parser failure(msg) always fails with a Message error
        /// without consuming any input.
        /// 
        /// The parsers 'failure', 'label' and 'unexpected' are the three parsers
        /// used to generate error messages.  Of these, only 'label' is commonly
        /// used.  For an example of the use of unexpected, see the definition
        /// of 'notFollowedBy'.
        /// </summary>
        /// <param name="msg">Error message to use when parsed</param>
        public static Parser<I, O> failure<I, O>(string msg) =>
            inp => EmptyError<I, O>(ParserError.Message(inp.Pos, msg), inp.TokenPos);

        /// <summary>
        /// Always success parser.  Returns the value provided.  
        /// This is monad return for the Parser monad
        /// </summary>
        public static Parser<I, O> result<I, O>(O value) =>
            inp => EmptyOK(value, inp);

        /// <summary>
        /// Always fails (with an Unknown error) without consuming any input
        /// </summary>
        public static Parser<I, O> zero<I, O>() =>
            inp => EmptyError<I, O>(ParserError.Unknown(inp.Pos), inp.TokenPos);

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
        public static Parser<I, O> either<I, O>(Parser<I, O> p, Parser<I, O> q) =>
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
                        return EmptyError<I, O>(mergeError(m.Reply.Error, n.Reply.Error), inp.TokenPos);
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
        public static Parser<I, O> choice<I, O>(params Parser<I, O>[] ps) =>
            choicei(toSeq(ps));

        /// <summary>
        /// choice(ps) tries to apply the parsers in the list ps in order, until one 
        /// of them succeeds. 
        /// </summary>
        /// <returns>
        /// The value of the succeeding parser.
        /// </returns>
        public static Parser<I, O> choice<I, O>(Seq<Parser<I, O>> ps) =>
            choicei(ps);

        /// <summary>
        /// Runs a sequence of parsers, if any fail then the failure state is
        /// returned immediately and subsequence parsers are not run.  
        /// </summary>
        /// <returns>
        /// The result of each parser as an enumerable.
        /// </returns>
        public static Parser<I, Seq<O>> chain<I, O>(params Parser<I, O>[] ps) =>
            chaini(toSeq(ps));

        /// <summary>
        /// Runs a sequence of parsers, if any fail then the failure state is
        /// returned immediately and subsequence parsers are not run.  
        /// </summary>
        /// <returns>
        /// The result of each parser as an enumerable.
        /// </returns>
        public static Parser<I, Seq<O>> chain<I, O>(Seq<Parser<I, O>> ps) =>
            chaini(ps);

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
        public static Parser<I, O> attempt<I, O>(Parser<I, O> p) =>
            inp =>
            {
                var res = p(inp);
                if (res.Tag == ResultTag.Consumed && res.Reply.Tag == ReplyTag.Error)
                {
                    return EmptyError<I, O>(res.Reply.Error, inp.TokenPos);
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
        public static Parser<I, O> lookAhead<I, O>(Parser<I, O> p) =>
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
        public static Parser<I, Seq<O>> many<I, O>(Parser<I, O> p) =>
            inp =>
            {
                var current = inp;
                var results = new List<O>();
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
                        return EmptyError<I, Seq<O>>(new ParserError(ParserErrorTag.SysUnexpect, current.Pos, "many: combinator 'many' is applied to a parser that accepts an empty string.", List.empty<string>()), inp.TokenPos);
                    }

                    // cerr
                    if (t.Tag == ResultTag.Consumed && t.Reply.Tag == ReplyTag.Error)
                    {
                        return ConsumedError<I, Seq<O>>(mergeError(error, t.Reply.Error), inp.TokenPos);
                    }

                    // eerr
                    return EmptyOK(toSeq(results), current, mergeError(error, t.Reply.Error));
                }
            };

        /// <summary>
        /// many1(p) applies the parser p one or more times.
        /// </summary>
        /// <returns>
        /// Enumerable of the returned values of p.
        /// </returns>
        public static Parser<I, Seq<O>> many1<I, O>(Parser<I, O> p) =>
            from x in p
            from xs in many(p)
            select x.Cons(xs);

        /// <summary>
        /// skipMany(p) applies the parser p zero or more times, skipping
        /// its result.
        /// </summary>
        public static Parser<I, Unit> skipMany<I, O>(Parser<I, O> p) =>
            either(skipMany1(p), result<I,Unit>(unit));

        /// <summary>
        /// skipMany(p) applies the parser p one or more times, skipping
        /// its result.
        /// </summary>
        public static Parser<I, Unit> skipMany1<I, O>(Parser<I, O> p) =>
            from x  in p
            from xs in many(p)
            select unit;

        /// <summary>
        /// optionOrElse(x, p) tries to apply parser p. If p fails without
        /// consuming input, it returns the value x, otherwise the value
        /// returned by p.
        /// </summary>
        public static Parser<I, O> optionOrElse<I, O>(O x, Parser<I, O> p) =>
            either(p, result<I, O>(x));

        /// <summary>
        /// optional(p) tries to apply parser p.  If p fails without
        /// consuming input, it return 'None', otherwise it returns
        /// 'Some' the value returned by p.
        /// </summary>
        public static Parser<I, Option<O>> optional<I, O>(Parser<I, O> p) =>
            inp =>
            {
                var r = p.Map(x => Option<O>.Some(x))(inp);
                return r.Reply.Tag == ReplyTag.OK
                    ? r
                    : EmptyOK(Option<O>.None, inp);
            };

        /// <summary>
        /// optionalSeq(p) tries to apply parser p.  If p fails without
        /// consuming input, it return an empty IEnumerable, otherwise it returns 
        /// a one item IEnumerable with the result of p.
        /// </summary>
        /// <returns>A list of 0 or 1 parsed items</returns>
        public static Parser<I, Seq<O>> optionalSeq<I, O>(Parser<I, O> p) =>
            inp =>
            {
                var r = p.Map(x => x.Cons(Seq<O>.Empty))(inp);
                return r.Reply.Tag == ReplyTag.OK
                    ? r
                    : EmptyOK(Seq<O>.Empty, inp);
            };

        /// <summary>
        /// optionalList(p) tries to apply parser p.  If p fails without
        /// consuming input, it return [], otherwise it returns a one 
        /// item Lst with the result of p.
        /// </summary>
        /// <returns>A list of 0 or 1 parsed items</returns>
        public static Parser<I, Lst<O>> optionalList<I, O>(Parser<I, O> p) =>
            inp =>
            {
                var r = p.Map(x => List.create(x))(inp);
                return r.Reply.Tag == ReplyTag.OK
                    ? r
                    : EmptyOK(List.empty<O>(), inp);
            };

        /// <summary>
        /// optionalArray(p) tries to apply parser p.  If p fails without
        /// consuming input, it return [], otherwise it returns a one 
        /// item array with the result of p.
        /// </summary>
        /// <returns>A list of 0 or 1 parsed items</returns>
        public static Parser<I, O[]> optionalArray<I, O>(Parser<I, O> p) =>
            inp =>
            {
                var r = p.Map(x => new[] { x })(inp);
                return r.Reply.Tag == ReplyTag.OK
                    ? r
                    : EmptyOK(new O [0], inp);
            };

        /// <summary>
        /// between(open,close,p) parses open, followed by p and close.
        /// </summary>
        /// <returns>
        /// The value returned by p.
        /// </returns>
        public static Parser<I, O> between<L, R, I, O>(Parser<I, L> open, Parser<I, R> close, Parser<I, O> inner) =>
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
        public static Parser<I, Seq<O>> sepBy1<S, I, O>(Parser<I, O> p, Parser<I, S> sep) =>
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
        public static Parser<I, Seq<O>> sepBy<S, I, O>(Parser<I, O> p, Parser<I, S> sep) =>
            either(sepBy1(p, sep), result<I, Seq<O>>(Seq<O>.Empty));

        /// <summary>
        /// sepEndBy1(p,sep) parses one or more occurrences of p,
        /// separated and optionally ended by sep. 
        /// </summary>
        /// <returns>
        /// A list of values returned by p.
        /// </returns>
        public static Parser<I, Seq<O>> sepEndBy1<S, I, O>(Parser<I, O> p, Parser<I, S> sep) =>
            from x in p
            from xs in either(from _ in sep
                              from ys in sepEndBy(p, sep)
                              select ys,
                              result<I, Seq<O>>(Seq<O>.Empty))
            select x.Cons(xs);

        /// <summary>
        /// sepEndBy(p,sep) parses zero or more occurrences of p,
        /// separated and optionally ended by sep. 
        /// </summary>
        /// <returns>
        /// A list of values returned by p.
        /// </returns>
        public static Parser<I, Seq<O>> sepEndBy<S, I, O>(Parser<I, O> p, Parser<I, S> sep) =>
            either(sepEndBy1(p, sep), result<I, Seq<O>>(Seq<O>.Empty));

        /// <summary>
        /// endBy1(p,sep) parses one or more occurrences of p, separated
        /// and ended by sep.
        /// </summary>
        /// <returns>
        /// A list of values returned by p.
        /// </returns>
        public static Parser<I, Seq<O>> endBy1<S, I, O>(Parser<I, O> p, Parser<I, S> sep) =>
            many1(from x in p
                  from _ in sep
                  select x);

        /// <summary>
        /// endBy(p,sep) parses zerp or more occurrences of p, separated
        /// and ended by sep.
        /// </summary>
        /// <returns>
        /// A list of values returned by p.
        /// </returns>
        public static Parser<I, Seq<O>> endBy<S, I, O>(Parser<I, O> p, Parser<I, S> sep) =>
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
        public static Parser<I, Seq<O>> count<S, I, O>(int n, Parser<I, O> p) =>
            counti(n, p);

        /// <summary>
        /// chainr(p,op,x) parses zero or more occurrences of p, separated by op 
        /// </summary>
        /// <returns>
        /// a value obtained by a right associative application of all functions 
        /// returned by op to the values returned by p. If there are no occurrences 
        /// of p, the value x is returned.</returns>
        public static Parser<I, O> chainr<I, O>(Parser<I, O> p, Parser<I, Func<O, O, O>> op, O x) =>
            either(chainr1(p, op), result<I, O>(x));

        /// <summary>
        /// chainl(p,op,x) parses zero or more occurrences of p, separated by op 
        /// </summary>
        /// <returns>
        /// a value obtained by a left associative application of all functions 
        /// returned by op to the values returned by p. If there are no occurrences 
        /// of p, the value x is returned.</returns>
        public static Parser<I, O> chainl<I, O>(Parser<I, O> p, Parser<I, Func<O, O, O>> op, O x) =>
            either(chainr1(p, op), result<I, O>(x));

        /// <summary>
        /// chainr1(p,op) parses one or more occurrences of p, separated by op. 
        /// </summary>
        /// <returns>
        /// A value obtained by a right associative application of all functions 
        /// returned by op to the values returned by p
        /// </returns>
        public static Parser<I, O> chainr1<I, O>(Parser<I, O> p, Parser<I, Func<O, O, O>> op)
        {
            Parser<I, O> scan = null;

            var rest = fun((O x) => either(from f in op
                                           from y in scan
                                           select f(x, y),
                                           result<I, O>(x)));

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
        public static Parser<I, O> chainl1<I, O>(Parser<I, O> p, Parser<I, Func<O, O, O>> op)
        {
            Func<O, Parser<I, O>> rest = null;

            rest = fun((O x) => either(from f in op
                                       from y in p
                                       from r in rest(f(x, y))
                                       select r,
                                       result<I, O>(x)));

            return from x in p
                   from y in rest(x)
                   select y;
        }

        /// <summary>
        /// This parser only succeeds at the end of the input. This is not a
        /// primitive parser but it is defined using 'notFollowedBy'.
        /// </summary>
        public static Parser<I, Unit> eof<I>() =>
            notFollowedBy(anyItem<I>()).label("end of input");

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
        public static Parser<I, Unit> notFollowedBy<I, O>(Parser<I, O> p) =>
            attempt(
                either(from c in attempt(p)
                       from u in unexpected<I, Unit>(c.ToString())
                       select u,
                       result<I, Unit>(unit)));

        /// <summary>
        /// Parse a char list and convert into a string
        /// </summary>
        public static Parser<I, string> asString<I>(Parser<I, Seq<char>> p) =>
            p.Select(x => new string(x.ToArray()));

        /// <summary>
        /// Parse a T list and convert into a string
        /// </summary>
        public static Parser<I, string> asString<I, O>(Parser<I, Seq<O>> p) =>
            p.Select(x => String.Join("",x.Select(o =>o.ToString())));

        /// <summary>
        /// Parse a T list and convert into a string
        /// </summary>
        public static Parser<I, string> asString<I, O>(Parser<I, O> p) =>
            p.Select(toString);

        /// <summary>
        /// Parse a char list and convert into an integer
        /// </summary>
        public static Parser<I, Option<int>> asInteger<I>(Parser<I, Seq<char>> p) =>
            p.Select(x => parseInt(new string(x.ToArray())));

        /// <summary>
        /// Parse a char list and convert into an integer
        /// </summary>
        public static Parser<I, Option<int>> asInteger<I>(Parser<I, string> p) =>
            p.Select(parseInt);

        /// <summary>
        /// Parse a char list and convert into an integer
        /// </summary>
        public static Parser<I, Option<int>> asInteger<I>(Parser<I, Seq<char>> p, int fromBase) =>
            p.Select(x => parseInt(new string(x.ToArray()), fromBase));

        /// <summary>
        /// Parse a char list and convert into an integer
        /// </summary>
        public static Parser<I, Option<int>> asInteger<I>(Parser<I, string> p, int fromBase) =>
            p.Select(x => parseInt(x, fromBase));

        /// <summary>
        /// Parse a char list and convert into an double precision floating point value
        /// </summary>
        public static Parser<I, Option<double>> asDouble<I>(Parser<I, Seq<char>> p) =>
            p.Select(x => parseDouble(new string(x.ToArray())));

        /// <summary>
        /// Parse a char list and convert into an double precision floating point value
        /// </summary>
        public static Parser<I, Option<double>> asDouble<I>(Parser<I, string> p) =>
            p.Select(parseDouble);

        /// <summary>
        /// Parse a char list and convert into an double precision floating point value
        /// </summary>
        public static Parser<I, Option<float>> asFloat<I>(Parser<I, Seq<char>> p) =>
            p.Select(x => parseFloat(new string(x.ToArray())));

        /// <summary>
        /// Parse a char list and convert into an double precision floating point value
        /// </summary>
        public static Parser<I, Option<float>> asFloat<I>(Parser<I, string> p) =>
            p.Select(parseFloat);

        public static Parser<I, Seq<O>> manyUntil<I, O, U>(Parser<I, O> p, Parser<I, U> end)
        {
            Parser<I, Seq<O>> scan = null;

            scan = either(
                from _ in end
                select Seq<O>.Empty,
                from x  in p
                from xs in scan
                select x.Cons(xs));

            return scan;
        }

        /// <summary>
        /// Parse child tokens
        /// </summary>
        /// <param name="children">Parser that gets the child tokens</param>
        /// <param name="p">Parser to run on the child tokens</param>
        /// <typeparam name="TOKEN">Token type</typeparam>
        /// <typeparam name="A">Type of the value to parse</typeparam>
        /// <returns>Parser that parses a set of tokens then uses them as a new stream to parse</returns>
        public static Parser<TOKEN, A> children<TOKEN, A>(Parser<TOKEN, Seq<TOKEN>> children, Parser<TOKEN, A> p) =>
            inp =>
            {
                var cres = children(inp);
                if (cres.Reply.Tag == ReplyTag.OK)
                {
                    var kids = cres.Reply.Result.ToArray();
                    var pres = p(new PString<TOKEN>(kids, 0, kids.Length, cres.Reply.State.UserState, inp.TokenPos));
                    
                    return pres.Reply.Tag == ReplyTag.OK && pres.Reply.State.Index < kids.Length
                               ? new ParserResult<TOKEN, A>(
                                   pres.Tag,
                                   new Reply<TOKEN, A>(
                                       ReplyTag.Error,
                                       pres.Reply.Result,
                                       new PString<TOKEN>(inp.Value, cres.Reply.State.Index, inp.EndIndex, pres.Reply.State.UserState, pres.Reply.State.TokenPos),
                                       ParserError.Unexpect(pres.Reply.State.Pos, "extra tokens in element that can't be parsed")))
                               : new ParserResult<TOKEN, A>(
                                   pres.Tag,
                                   new Reply<TOKEN, A>(
                                       pres.Reply.Tag,
                                       pres.Reply.Result,
                                       new PString<TOKEN>(inp.Value, cres.Reply.State.Index, inp.EndIndex, pres.Reply.State.UserState, pres.Reply.State.TokenPos),
                                       pres.Reply.Error));
                }
                else
                {
                    return new ParserResult<TOKEN, A>(
                        cres.Tag,
                        new Reply<TOKEN, A>(
                            cres.Reply.Tag,
                            default(A),
                            cres.Reply.State,
                            cres.Reply.Error));
                }
            };        
    }
}

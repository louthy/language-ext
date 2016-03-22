using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.ParserResult;
using static LanguageExt.ParsecInternal;

namespace LanguageExt
{
    public static class Parsec
    {
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
        /// Special parser for setting user state that propagates 
        /// through the computation.
        /// </summary>
        public static Parser<Unit> setState<T>(T state) =>
            (PString inp) => ConsumedOK(unit, inp.SetUserState(state));

        /// <summary>
        /// Special parser for getting user state that was previously
        /// set with setState
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Parser<T> getState<T>() =>
            (PString inp) => match(inp.UserState,
                                Some: x => x is T
                                                ? ConsumedOK((T)x, inp)
                                                : EmptyError<T>(inp.Pos, "User state type-mismatch"),
                                None: () => EmptyError<T>(inp.Pos, "No user state set"));

        /// <summary>
        /// 'empty' parser
        /// No error, no results
        /// </summary>
        public static Parser<T> zero<T>() =>
            inp => EmptyError<T>(inp.Pos, "");

        /// <summary>
        /// A parser that always fails
        /// </summary>
        /// <param name="err">Error message to use when parsed</param>
        public static Parser<T> failure<T>(string err) =>
            inp => EmptyError<T>(inp.Pos, err);

        /// <summary>
        /// Always success parser.  Returns the value provided.  
        /// This is the identity function for Parser<T>
        /// </summary>
        public static Parser<T> result<T>(T value) =>
            inp => EmptyOK(value, inp);

        /// <summary>
        /// Item parser.  
        /// Parses a single character of any kind.
        /// If the end of the stream is found then an error.
        /// </summary>
        public static readonly Parser<char> item =
            (PString inp) =>
                inp.Index >= inp.Value.Length
                    ? EmptyError<char>(inp.Pos, "end of stream")
                    : newstate(inp);

        public static ParserResult<T> mergeOk<T>(T x, PString inp, ParserError msg1, ParserError msg2)
        {
            if (msg1 != null && msg2 != null && msg1.Pos.Column != msg2.Pos.Column)
            {
                x = x;
            }

            return EmptyOK(x, inp, merge(msg1, msg2));
        }

        public static ParserResult<T> mergeError<T>(ParserError msg1, ParserError msg2)
        {
            if (msg1 != null && msg2 != null && msg1.Pos.Column != msg2.Pos.Column)
            {
                msg1 = msg1;
            }

            return EmptyError<T>(merge(msg1, msg2));
        }

        public static ParserError merge(ParserError exp1, ParserError exp2)
        {
            if (exp1 != null && exp2 != null && exp1.Pos.Column != exp2.Pos.Column)
            {
                exp1 = exp1;
            }

            return exp1 == null && exp2 == null
                ? exp1
                : exp1 == null
                    ? exp2
                    : exp2 == null                        ? exp1                        : new ParserError(exp1.Pos, exp1.Message, List.append(exp1.Expected, exp2.Expected).Freeze());
        }
        /// <summary>
        /// Logical OR for parsers
        /// Parse p, if it fails parse q
        /// </summary>
        public static Parser<T> either<T>(Parser<T> p, Parser<T> q) =>
            state =>
                p(state).Match(
                    EmptyError: msg1 =>
                        q(state).Match(
                            EmptyError: msg2           => mergeError<T>(msg1, msg2),
                            EmptyOK:    (x, inp, msg2) => mergeOk(x, inp, msg1, msg2),
                            Consumed:   consumed       => consumed
                            ),
                    EmptyOK: (x,inp,msg1) =>
                        q(state).Match(
                            EmptyError: msg2           => mergeOk(x, inp, msg1, msg2),
                            EmptyOK:    (_, __, msg2)  => mergeOk(x, inp, msg1, msg2),
                            Consumed:   consumed       => consumed
                            ),
                    Consumed: consumed => consumed);

        /// <summary>
        /// Logical OR for parsers
        /// Parse p, if it fails parse q
        /// Useful when constructing recursive parsers
        /// </summary>
        public static Parser<T> eitherLazy<T>(Func<Parser<T>> p, Func<Parser<T>> q) =>
            inp => either(p(), q())(inp);

        /// <summary>
        /// Logical OR for parsers
        /// Parse p, if it fails parse q
        /// Useful when constructing recursive parsers
        /// </summary>
        public static Parser<T> eitherLazy<T>(Func<Unit,Parser<T>> p, Func<Unit,Parser<T>> q) =>
            inp => either(p(unit), q(unit))(inp);

        /// <summary>
        /// Parse a char list and convert into a string
        /// </summary>
        public static Parser<string> asString(Parser<IEnumerable<char>> p) =>
            p.Select(x => new string(x.ToArray()));

        /// <summary>
        /// Parse a char list and convert into an integer
        /// </summary>
        public static Parser<int> asInteger(Parser<IEnumerable<char>> p) =>
            p.Select(x => Int32.Parse(new string(x.ToArray())));

        public static Parser<char> satisfy(Func<char, bool> pred) =>
            inp =>
            {
                if (inp.Index >= inp.Value.Length)
                {
                    return EmptyError<char>(inp.Pos, "end of stream");
                }
                else {
                    var ns = newstate(inp);

                    if (ns.Tag == ResultTag.Consumed)
                    {
                        if (pred(ns.Reply.Result))
                        {
                            return ns;
                        }
                        else
                        {
                            return EmptyError<char>(inp.Pos, ns.Reply.Result.ToString());
                        }
                    }
                    else
                    {
                        return EmptyError<char>(inp.Pos, "end of stream");
                    }
                }
            };
            //from x in item
            //where pred(x)
            //select x;

        public static Parser<char> ch(char c) =>
            satisfy(x => x == c).label($"'{c}'");

        public readonly static Parser<char> digit =
            satisfy(Char.IsDigit).label("digit");

        public readonly static Parser<char> lower =
            satisfy(Char.IsLower).label("lower-case character");

        public readonly static Parser<char> upper =
            satisfy(Char.IsUpper).label("upper-case character");

        public readonly static Parser<char> letter =
            either(lower,upper).label("letter");

        public readonly static Parser<char> letterOrDigit =
            either(letter, digit).label("letter or digit");

        public static Parser<IEnumerable<T>> many<T>(Parser<T> p) =>
            inp =>
            {
                var value = new List<T>();
                var current = inp;

                while (true)
                {
                    var res = p(current);

                    if (res.Tag == ResultTag.Consumed && res.Reply.Tag == ReplyTag.OK)
                    {
                        value.Add(res.Reply.Result);
                        current = res.Reply.Remaining;
                    }
                    else if (res.Tag == ResultTag.Consumed && res.Reply.Tag == ReplyTag.Error)
                    {
                        return ConsumedError<IEnumerable<T>>(res.Reply.Remaining, res.Reply.Error);
                    }
                    else if (res.Tag == ResultTag.Empty && res.Reply.Tag == ReplyTag.OK)
                    {
                        return EmptyError<IEnumerable<T>>(inp.Pos, "Combinator 'many' is applied to a parser that accepts an empty string.");
                    }
                    else 
                    {
                        // Empty Error - actually means success
                        return ConsumedOK<IEnumerable<T>>(value, current, res.Reply.Error);
                    }
                }
            };

        public static Parser<IEnumerable<T>> many1<T>(Parser<T> p) =>
            from x in p
            from xs in many(p)
            select x.Cons(xs);

        public static Parser<T> choice<T>(params Parser<T>[] ps) =>
           choice(ps.AsEnumerable());

        public static Parser<T> choice<T>(IEnumerable<Parser<T>> ps) =>
            ps.Count() == 1
                ? ps.Head()
                : either(ps.Head(), choice(ps.Tail()));

        /// <summary>
        /// Runs a sequence of parsers, if any fail then the failure state is
        /// returned immediately and subsequence parsers are not run.  The result
        /// of each parser is returned.
        /// </summary>
        public static Parser<IEnumerable<T>> chain<T>(params Parser<T>[] ps) =>
            chain(ps.AsEnumerable());

        /// <summary>
        /// Runs a sequence of parsers, if any fail then the failure state is
        /// returned immediately and subsequence parsers are not run.  The result
        /// of each parser is returned.
        /// </summary>
        public static Parser<IEnumerable<T>> chain<T>(IEnumerable<Parser<T>> ps) =>
            ps.Count() == 1
                ? ps.Head().Map(x => new[] {x}.AsEnumerable())
                : from x in ps.Head()
                  from y in chain(ps.Tail())
                  select x.Cons(y);

        /// <summary>
        /// Must match any character in a string of characters
        /// </summary>
        public static Parser<char> oneOf(string str) =>
            choice(str.Map(ch)).label($"one of: {str}");

        /// <summary>
        /// Parses many letters
        /// </summary>
        public readonly static Parser<string> word =
            asString(many1(letter).label("word"));

        /// <summary>
        /// Parse a string
        /// </summary>
        public static Parser<string> str(string s) =>
            asString(chain(s.Map(c => ch(c)))).label($"'{s}'");

        /// <summary>
        /// Parses a natural number (positive integer)
        /// </summary>
        public readonly static Parser<int> natural =
            asInteger(many1(digit)).label("natural number");

        /// <summary>
        /// Parses an integer
        /// </summary>
        public readonly static Parser<int> integer =
            either(
                from n in ch('-')
                from nat in natural
                select -nat,
                natural)
               .label("integer");

        /// <summary>
        /// Parses an octal digit (0-7)
        /// </summary>
        public readonly static Parser<char> octalDigit =
            satisfy(c => List.exists("01234567", x => x == c))
           .label("octal-digit");

        /// <summary>
        /// Parses a hex digit (0-F | 0-f)
        /// </summary>
        public readonly static Parser<char> hexDigit =
            satisfy(c => Char.IsDigit(c) || List.exists("abcdefABCDEF", x => x == c))
           .label("hexadecimal digit");

        /// <summary>
        /// Parses an octal number
        /// </summary>
        public readonly static Parser<int> octal =
            (from x in asString(from _ in either(ch('o'), ch('O'))
                                from d in many1(octalDigit)
                                select d)
             select Convert.ToInt32(x, 8))
           .label("octal number");

        /// <summary>
        /// Parses a hex number
        /// </summary>
        public readonly static Parser<int> hex =
            (from x in asString(from _ in either(ch('x'), ch('X'))
                                from d in many1(hexDigit)
                                select d)
             select Convert.ToInt32(x, 16))
            .label("hexadecimal number");

        /// <summary>
        /// Parses at least one value item using p, separated by items parsed with sep
        /// </summary>
        public static Parser<IEnumerable<T>> sepBy1<S, T>(Parser<T> p, Parser<S> sep) =>
            from x in p
            from xs in many(from _ in sep
                            from y in p
                            select y)
            select x.Cons(xs);

        /// <summary>
        /// Parses at zero or more items using p, separated by items parsed with sep
        /// </summary>
        public static Parser<IEnumerable<T>> sepBy<S, T>(Parser<T> p, Parser<S> sep) =>
            either(sepBy1(p, sep), result(new T[0].AsEnumerable()));

        /// <summary>
        /// Parses whitespace characters (including cr/lf)
        /// </summary>
        public readonly static Parser<string> spaces =
            asString(many1(satisfy(c => c == ' ' || c == '\t' || c == '\n' || c == '\r')));

        /// <summary>
        /// Parses a standard C/C++/C# comment 
        /// </summary>
        public readonly static Parser<string> comment =
            asString(from _ in str("//")
                     from c in many(satisfy(c => c != '\n'))
                     from x in item
                     select c);

        /// <summary>
        /// Parses whitespaces and comments
        /// </summary>
        public readonly static Parser<string> junk =
            from xs in many(either(spaces, comment))
            select String.Join("", xs);

        /// <summary>
        /// Parses a token - first runs parser p, followed by junk
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p"></param>
        /// <returns></returns>
        public static Parser<T> token<T>(Parser<T> p) =>
            from v in p
            from _ in junk
            select v;

        /// <summary>
        /// Parses a series of tokenised integers separated by sep
        /// </summary>
        public static Parser<IEnumerable<int>> integers<T>(Parser<T> sep) =>
            sepBy1(token(integer), token(sep));

        /// <summary>
        /// Parses a series of tokenised words separated by sep
        /// </summary>
        public static Parser<IEnumerable<string>> words<T>(Parser<T> sep) =>
            sepBy1(token(word), token(sep));

        /// <summary>
        /// parses inner, wrapped by left and right
        /// </summary>
        public static Parser<T> wrappedBy<L, R, T>(Parser<L> left, Parser<R> right, Parser<T> inner) =>
            from l in token(left)
            from v in token(inner)
            from r in token(right)
            select v;

        /// <summary>
        /// Parses inner, wrapped by [ ]
        /// </summary>
        public static Parser<T> brackets<T>(Parser<T> inner) =>
            wrappedBy(ch('['), ch(']'), inner);

        /// <summary>
        /// Parses inner, wrapped by ( )
        /// </summary>
        public static Parser<T> parens<T>(Parser<T> inner) =>
            wrappedBy(ch('('), ch(')'), inner);

        /// <summary>
        /// Parses inner, wrapped by { }
        /// </summary>
        public static Parser<T> braces<T>(Parser<T> inner) =>
            wrappedBy(ch('{'), ch('}'), inner);

        /// <summary>
        /// Parses inner, wrapped by &lt; &gt;
        /// </summary>
        public static Parser<T> angles<T>(Parser<T> inner) =>
            wrappedBy(ch('<'), ch('>'), inner);

        /// <summary>
        /// Parses a symbol: a tokenised string 
        /// </summary>
        public static Parser<string> symbol(string name) =>
            token(str(name))
           .label($"'{name}'");

        /// <summary>
        /// Parses reserved symbols
        /// </summary>
        /// <param name="names">List of reserved symbols</param>
        public static Parser<string> reserved(IEnumerable<string> names) =>
            choice(names.Map(x=>symbol(x)))
           .label("reserved symbol");

        /// <summary>
        /// Parses an identifier: a letter followed by many letters or digits
        /// </summary>
        public readonly static Parser<string> ident =
            (from x in letter
             from xs in many(letterOrDigit)
             select new string(x.Cons(xs).ToArray()))
            .label("identifier");

        /// <summary>
        /// Parses an identifier that must not match the reserved names
        /// </summary>
        /// <param name="names">Reserved names</param>
        public static Parser<string> identifier(IEnumerable<string> names) =>
            token(from id in ident
                  from res in names.Contains(id)
                        ? failure<string>("reserved identifier")
                        : result(id)
                  select res)
                 .label("indentifier");

        /// <summary>
        /// Parses sep separated p, wrapped by left and right
        /// </summary>
        public static Parser<IEnumerable<T>> wrappedAndSep<L,R,T,S>(Parser<L> left, Parser<R> right, Parser<S> sep, Parser<T> p) =>
            wrappedBy(left, right, sepBy1(token(p), sep));

        /// <summary>
        /// Parses comma separated p, wrapped by [ ]
        /// </summary>
        public static Parser<IEnumerable<T>> commaBrackets<T>(Parser<T> p) =>
            wrappedAndSep(ch('['), ch(']'), symbol(","), p);

        /// <summary>
        /// Parses comma separated p, wrapped by ( )
        /// </summary>
        public static Parser<IEnumerable<T>> commaParens<T>(Parser<T> p) =>
            wrappedAndSep(ch('('), ch(')'), symbol(","), p);

        /// <summary>
        /// Parses comma separated p, wrapped by { }
        /// </summary>
        public static Parser<IEnumerable<T>> commaBraces<T>(Parser<T> p) =>
            wrappedAndSep(ch('{'), ch('}'), symbol(","), p);

        /// <summary>
        /// Parses comma separated p, wrapped by &lt; &gt;
        /// </summary>
        public static Parser<IEnumerable<T>> commaAngles<T>(Parser<T> p) =>
            wrappedAndSep(ch('<'), ch('>'), symbol(","), p);

        /// <summary>
        /// Optionaly parses p
        /// </summary>
        public static Parser<Option<T>> optional<T>(Parser<T> p) =>
            inp =>
            {
                var r = p(inp);
                return r.Tag == ResultTag.Consumed && r.Reply.Tag == ReplyTag.OK
                    ? ConsumedOK(Option<T>.Some(r.Reply.Result), r.Reply.Remaining)
                    : EmptyOK(Option<T>.None, inp);
            };

        /// <summary>
        /// Optionally parses p
        /// </summary>
        /// <returns>A list of 0 or 1 parsed items</returns>
        public static Parser<Lst<T>> optionalList<T>(Parser<T> p) =>
            optional(p).Select(x => x.ToList());


        readonly static Parser<char> stringLetter =
            satisfy(c => (c != '"') && (c != '\\') && (c > 26));

        readonly static Lst<Tuple<char, char>> escapeMap =
            List.zip("abfnrtv\\\"\'", "\a\b\f\n\r\t\v\\\"\'").Freeze();

        static Parser<T> parseEsc<T>(char c, T code) =>
            from _ in ch(c)
            select code;

        static readonly Lst<Parser<char>> escapeMapParsers =
            escapeMap.Map(tup => parseEsc(tup.Item1, tup.Item2)).Freeze();

        static readonly Parser<char> charEsc =
            choice(escapeMapParsers);

        static readonly Parser<char> charNum =
            from code in choice(natural, hex, octal)
            select Char.ConvertFromUtf32(code)[0];

        static readonly Parser<char> escapeCode =
            either(charEsc, charNum); // || charAscii || charControl

        static readonly Parser<char> escapeEmpty =
            ch('&');

        static readonly Parser<char> escapeGap =
            from sp in many1(ch(' '))
            from ch in ch('\\')
            select ch;

        static readonly Parser<char> stringEscape =
            from _ in ch('\\')
            from e in choice(escapeGap, escapeEmpty, escapeCode)
            select e;

        /// <summary>
        /// Parses a character in a string-literal - deals with escape codes also
        /// </summary>
        public static readonly Parser<char> stringChar =
            either(stringLetter, stringEscape);

        /// <summary>
        /// Parses a multi-line string-literal as-is with no consideration of 
        /// escape-codes
        /// 
        /// e.g. """a string"""
        /// </summary>
        public readonly static Parser<string> heavyString =
            asString(wrappedBy(
                chain(ch('"'), ch('"'), ch('"')),
                chain(ch('"'), ch('"'), ch('"')),
                many(satisfy(c => c != '"'))));

        /// <summary>
        /// Parses a string-literal with full consideration of escape-codes
        /// </summary>
        public readonly static Parser<string> stringLiteral =
            token(
                choice(
                    heavyString,
                    wrappedBy(ch('"'), ch('"'), asString(many(stringChar)))))
                   .label("string");


    }
}

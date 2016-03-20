using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static class Parsec
    {
        public static ParserResult<T> Success<T>(T result, PString remaining) =>
            new ParserResult<T>(result, remaining);

        public static ParserResult<T> Successes<T>(params Tuple<T, PString>[] results) =>
            new ParserResult<T>(results);

        public static ParserResult<T> Successes<T>(IEnumerable<Tuple<T, PString>> results) =>
            new ParserResult<T>(results);

        public static ParserResult<T> Failure<T>(string message, PString location) =>
            new ParserResult<T>(new[] { new ParserError(message, location) });

        public static ParserResult<T> Failure<T>(params ParserError[] errors) =>
            new ParserResult<T>(errors);

        public static ParserResult<T> Failure<T>(IEnumerable<ParserError> errors) =>
            new ParserResult<T>(errors);

        /// <summary>
        /// Run the parser with the input provided
        /// </summary>
        public static ParserResult<T> parse<T>(Parser<T> p, PString input) =>
            p.Parse(input);

        /// <summary>
        /// Run the parser with the input provided
        /// </summary>
        public static ParserResult<T> parse<T>(Parser<T> p, string input) =>
            p.Parse(input);

        /// <summary>
        /// 'empty' parser
        /// No error, no results
        /// </summary>
        public static Parser<T> zero<T>() =>
            inp => Successes<T>();

        /// <summary>
        /// A parser that always fails
        /// </summary>
        /// <param name="err">Error message to use when parsed</param>
        public static Parser<T> failure<T>(string err) =>
            inp => Failure<T>(new ParserError(err, inp));

        /// <summary>
        /// Always success parser.  Returns the value provided.  
        /// This is the identity function for Parser<T>
        /// </summary>
        public static Parser<T> result<T>(T value) =>
            inp => Success(value,inp);

        static bool onside(Pos pos, Pos delta) =>
            pos.Column > delta.Column || pos.Line == delta.Line;

        static ParserResult<char> newstate(PString inp)
        {
            var x = inp.Value[inp.Index];

            var newpos = x == '\n' ? new Pos(inp.Pos.Line + 1, 0)
                       : x == '\t' ? new Pos(inp.Pos.Line, ((inp.Pos.Column / 4) + 1) * 4)
                       : new Pos(inp.Pos.Line, inp.Pos.Column + 1);

            return Success(x,
                new PString(
                    inp.Value,
                    inp.Index+1,
                    newpos,
                    inp.DefPos,
                    onside(newpos, inp.DefPos)
                        ? Sidedness.Onside
                        : Sidedness.Offside));
        }

        /// <summary>
        /// Item parser.  
        /// Parses a single character of any kind.
        /// If the end of the stream is found then an error.
        /// </summary>
        public static readonly Parser<char> item =
            (PString inp) =>
                inp.Index >= inp.Value.Length
                    ? Failure<char>("End of stream", inp)
                    : newstate(inp);

        static readonly Parser<Pos> getPos =
            (PString inp) => Success(inp.Pos, inp);

        static readonly Parser<Pos> getDefPos =
            (PString inp) => Success(inp.DefPos, inp);

        static Parser<T> setDefPos<T>(Pos defpos, Parser<T> p) =>
            (PString inp) => p(inp.SetDefPos(defpos));

        /// <summary>
        /// Parses the same input with p and q and returns either
        /// all errors or all results
        /// </summary>
        public static Parser<T> plus<T>(Parser<T> p, Parser<T> q)
        {
            return inp =>
            {
                var r1 = p(inp);
                var r2 = q(inp);
                return r1.IsFaulted || r2.IsFaulted
                    ? Failure<T>(List.append(r1.Errors, r2.Errors))
                    : Successes(List.append(r1.Results, r2.Results));
            };
        }

        /// <summary>
        /// Logical AND for parsers
        /// Not super useful, use a sequence of from x in p statements
        /// in LINQ for the same result
        /// </summary>
        public static Parser<T> and<T>(Parser<T> p, Parser<T> q) =>
            from x in p
            from y in q
            select y;

        /// <summary>
        /// Logical OR for parsers
        /// Parse p, if it fails parse q
        /// </summary>
        public static Parser<T> either<T>(Parser<T> p, Parser<T> q)
        {
            return inp =>
            {
                var r1 = p(inp);
                return r1.IsFaulted
                    ? q(inp)
                    : r1;
            };
        }

        /// <summary>
        /// Logical OR for parsers
        /// Parse p, if it fails parse q
        /// Useful when constructing recursive parsers
        /// </summary>
        public static Parser<T> eitherLazy<T>(Func<Parser<T>> p, Func<Parser<T>> q)
        {
            return inp =>
            {
                var r1 = p()(inp);
                return r1.IsFaulted
                    ? q()(inp)
                    : r1;
            };
        }

        /// <summary>
        /// Logical OR for parsers
        /// Parse p, if it fails parse q
        /// Useful when constructing recursive parsers
        /// </summary>
        public static Parser<T> eitherLazy<T>(Func<Unit,Parser<T>> p, Func<Unit,Parser<T>> q)
        {
            return inp =>
            {
                var r1 = p(unit)(inp);
                return r1.IsFaulted
                    ? q(unit)(inp)
                    : r1;
            };
        }

        static string charConcat(IEnumerable<char> chs) =>
            new string(chs.ToArray());

        /// <summary>
        /// Parse a char list and convert into a string
        /// </summary>
        public static Parser<string> asString(Parser<IEnumerable<char>> p) =>
            inp =>
            {
                var r = p(inp);
                return r.IsFaulted
                    ? Failure<string>(r.Errors)
                    : Success(charConcat(r.Result), r.Remaining);
            };

        /// <summary>
        /// Parse a char list and convert into an integer
        /// </summary>
        public static Parser<int> asInteger(Parser<IEnumerable<char>> p) =>
            inp =>
            {
                var r = p(inp);
                return r.IsFaulted
                    ? Failure<int>(r.Errors)
                    : Success(Int32.Parse(charConcat(r.Result)), r.Remaining);
            };

        public static Parser<char> satisfy(Func<char, bool> pred) =>
            from x in item
            where pred(x)
            select x;

        public static Parser<char> ch(char c) =>
            satisfy(x => x == c);

        public readonly static Parser<char> digit =
            satisfy(Char.IsDigit);

        public readonly static Parser<char> lower =
            satisfy(Char.IsLower);

        public readonly static Parser<char> upper =
            satisfy(Char.IsUpper);

        public readonly static Parser<char> letter =
            either(lower,upper);

        public readonly static Parser<char> letterOrDigit =
            either(letter, digit);

        public static Parser<IEnumerable<T>> many<T>(Parser<T> p) =>
            inp =>
            {
                var value = new List<T>();
                var current = inp;

                while (true)
                {
                    var res = p(current);
                    if(res.IsFaulted)
                    {
                        return Success<IEnumerable<T>>(value, current);
                    }
                    else
                    {
                        value.AddRange(res.Results.Map(x => x.Item1));
                        current = res.Remaining;
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
            inp =>
            {
                foreach (var p in ps)
                {
                    var res = p(inp);
                    if (!res.IsFaulted && res.Results.Any())
                    {
                        return res;
                    }
                }
                return Failure<T>();
            };

        public static Parser<IEnumerable<T>> chain<T>(params Parser<T>[] ps) =>
            chain(ps.AsEnumerable());

        public static Parser<IEnumerable<T>> chain<T>(IEnumerable<Parser<T>> ps) =>
            inp =>
            {
                var value = new List<T>();
                var current = inp;

                foreach(var p in ps)
                {
                    var res = p(current);
                    if(res.IsFaulted || !res.Results.Any())
                    {
                        return Failure<IEnumerable<T>>(res.Errors);
                    }
                    else
                    {
                        value.AddRange(res.Results.Map(x => x.Item1));
                        current = res.Remaining;
                    }
                }
                return Success<IEnumerable<T>>(value, current);
            };

        public static Parser<char> oneOf(string str) =>
            choice(str.Select(c => ch(c)));

        public readonly static Parser<string> word =
            asString(many1(letter));

        public static Parser<string> str(string s) =>
            inp =>
            {
                var value = new StringBuilder();
                var current = inp;

                foreach (var c in s)
                {
                    var res = ch(c)(current);
                    if (res.IsFaulted || !res.Results.Any())
                    {
                        return Failure<string>(res.Errors);
                    }
                    else
                    {
                        value.Append(res.Result);
                        current = res.Remaining;
                    }
                }
                return Success(value.ToString(), current);
            };

        public readonly static Parser<int> natural =
            asInteger(many1(digit));

        public readonly static Parser<int> integer =
            either(
                from n in ch('-')
                from nat in natural
                select -nat,
                natural);

        public readonly static Parser<char> octalDigit =
            satisfy(c => List.exists("01234567", x => x == c));

        public readonly static Parser<char> hexDigit =
            satisfy(c => Char.IsDigit(c) || List.exists("abcdefABCDEF", x => x == c));

        public readonly static Parser<int> octal =
            from x in asString(from _ in either(ch('o'), ch('O'))
                               from d in many1(octalDigit)
                               select d)
            select Convert.ToInt32(x, 8);

        public readonly static Parser<int> hex =
            from x in asString(from _ in either(ch('x'), ch('X'))
                               from d in many1(hexDigit)
                               select d)
            select Convert.ToInt32(x, 16);

        public static Parser<IEnumerable<T>> sepBy1<S, T>(Parser<T> p, Parser<S> sep) =>
            from x in p
            from xs in many(from _ in sep
                            from y in p
                            select y)
            select x.Cons(xs);

        public static Parser<IEnumerable<T>> sepBy<S, T>(Parser<T> p, Parser<S> sep) =>
            either(sepBy1(p, sep), result(new T[0].AsEnumerable()));

        public readonly static Parser<string> spaces =
            asString(many1(satisfy(c => c == ' ' || c == '\t' || c == '\n' || c == '\r')));

        public readonly static Parser<string> comment =
            asString(from _ in str("//")
                     from c in many(satisfy(c => c != '\n'))
                     from x in item
                     select c);

        public readonly static Parser<string> junk =
            from xs in many(either(spaces, comment))
            select String.Join("", xs);

        public static Parser<T> token<T>(Parser<T> p) =>
            from v in p
            from _ in junk
            select v;

        public static Parser<IEnumerable<int>> integers<T>(Parser<T> sep) =>
            sepBy1(token(integer), token(sep));

        public static Parser<IEnumerable<string>> words<T>(Parser<T> sep) =>
            sepBy1(token(word), token(sep));

        public static Parser<T> wrappedBy<L, R, T>(Parser<L> left, Parser<R> right, Parser<T> inner) =>
            from l in token(left)
            from v in token(inner)
            from r in token(right)
            select v;

        public static Parser<T> brackets<T>(Parser<T> inner) =>
            wrappedBy(ch('['), ch(']'), inner);

        public static Parser<T> parens<T>(Parser<T> inner) =>
            wrappedBy(ch('('), ch(')'), inner);

        public static Parser<T> braces<T>(Parser<T> inner) =>
            wrappedBy(ch('{'), ch('}'), inner);

        public static Parser<T> angles<T>(Parser<T> inner) =>
            wrappedBy(ch('<'), ch('>'), inner);

        public static Parser<string> symbol(string name) =>
            token(str(name));

        public static Parser<string> reserved(IEnumerable<string> names) =>
            choice(names.Map(x=>symbol(x)));

        public readonly static Parser<string> ident =
            from x in letter
            from xs in many(letterOrDigit)
            select new string(x.Cons(xs).ToArray());

        public static Parser<string> identifier(IEnumerable<string> names) =>
            token(from id in ident
                  from res in names.Contains(id)
                        ? failure<string>("reserved identifier")
                        : result(id)
                  select res);

        public static Parser<IEnumerable<T>> wrappedAndSep<L,R,T,S>(Parser<L> left, Parser<R> right, Parser<S> sep, Parser<T> p) =>
            wrappedBy(left, right, sepBy1(token(p), sep));

        public static Parser<IEnumerable<T>> commaBrackets<T>(Parser<T> p) =>
            wrappedAndSep(ch('['), ch(']'), symbol(","), p);

        public static Parser<IEnumerable<T>> commaParens<T>(Parser<T> p) =>
            wrappedAndSep(ch('('), ch(')'), symbol(","), p);

        public static Parser<IEnumerable<T>> commaBraces<T>(Parser<T> p) =>
            wrappedAndSep(ch('{'), ch('}'), symbol(","), p);

        public static Parser<IEnumerable<T>> commaAngles<T>(Parser<T> p) =>
            wrappedAndSep(ch('<'), ch('>'), symbol(","), p);

        public static Parser<Option<T>> optional<T>(Parser<T> p) =>
            inp =>
            {
                var r = p(inp);
                if (r.IsFaulted)
                {
                    return Success<Option<T>>(Option<T>.None, inp);
                }
                else
                {
                    return Success<Option<T>>(Option<T>.Some(r.Result), r.Remaining);
                }
            };

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

        static readonly Parser<char> stringChar =
            either(stringLetter, stringEscape);

        public readonly static Parser<string> heavyString =
            asString(wrappedBy(
                chain(ch('"'), ch('"'), ch('"')),
                chain(ch('"'), ch('"'), ch('"')),
                many(satisfy(c => c != '"'))));

        public readonly static Parser<string> stringLiteral =
            token(
                choice(
                    heavyString,
                    wrappedBy(ch('"'), ch('"'), asString(many(stringChar)))));


    }
}

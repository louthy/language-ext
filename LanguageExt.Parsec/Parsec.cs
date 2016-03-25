using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.Parsec.ParserResult;

namespace LanguageExt.Parsec
{
    public static class Parsec_Old
    {
        ///// <summary>
        ///// Parses many letters
        ///// </summary>
        //public readonly static Parser<string> word =
        //    asString(many1(letter).label("word"));

        ///// <summary>
        ///// Parses a natural number (positive integer)
        ///// </summary>
        //public readonly static Parser<int> natural =
        //    asInteger(many1(digit)).label("natural number");

        ///// <summary>
        ///// Parses an integer
        ///// </summary>
        //public readonly static Parser<int> integer =
        //    either(
        //        from n in ch('-')
        //        from nat in natural
        //        select -nat,
        //        natural)
        //       .label("integer");

        ///// <summary>
        ///// Parses an octal number
        ///// </summary>
        //public readonly static Parser<int> octal =
        //    (from x in asString(from _ in either(ch('o'), ch('O'))
        //                        from d in many1(octalDigit)
        //                        select d)
        //     select Convert.ToInt32(x, 8))
        //   .label("octal number");

        ///// <summary>
        ///// Parses a hex number
        ///// </summary>
        //public readonly static Parser<int> hex =
        //    (from x in asString(from _ in either(ch('x'), ch('X'))
        //                        from d in many1(hexDigit)
        //                        select d)
        //     select Convert.ToInt32(x, 16))
        //    .label("hexadecimal number");

        ///// <summary>
        ///// Parses whitespace characters (including cr/lf)
        ///// </summary>
        //public readonly static Parser<string> spaces =
        //    asString(many1(satisfy(c => c == ' ' || c == '\t' || c == '\n' || c == '\r')));

        ///// <summary>
        ///// Parses a standard C/C++/C# comment 
        ///// </summary>
        //public readonly static Parser<string> comment =
        //    asString(from _ in str("//")
        //             from c in many(satisfy(c => c != '\n'))
        //             from x in item
        //             select c);

        ///// <summary>
        ///// Parses whitespaces and comments
        ///// </summary>
        //public readonly static Parser<string> junk =
        //    either(
        //        from xs in many(either(spaces, comment))
        //        select String.Join("", xs),
        //        result(""));

        ///// <summary>
        ///// Parses a token - first runs parser p, followed by junk
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="p"></param>
        ///// <returns></returns>
        //public static Parser<T> token<T>(Parser<T> p) =>
        //    attempt(from v in p
        //            from _ in junk
        //            select v);

        ///// <summary>
        ///// Parses a series of tokenised integers separated by sep
        ///// </summary>
        //public static Parser<IEnumerable<int>> integers<T>(Parser<T> sep) =>
        //    sepBy1(token(integer), token(sep));

        ///// <summary>
        ///// Parses a series of tokenised words separated by sep
        ///// </summary>
        //public static Parser<IEnumerable<string>> words<T>(Parser<T> sep) =>
        //    sepBy1(token(word), token(sep));

        ///// <summary>
        ///// Parses inner, wrapped by [ ]
        ///// </summary>
        //public static Parser<T> brackets<T>(Parser<T> inner) =>
        //    between(ch('['), ch(']'), inner);

        ///// <summary>
        ///// Parses inner, wrapped by ( )
        ///// </summary>
        //public static Parser<T> parens<T>(Parser<T> inner) =>
        //    between(ch('('), ch(')'), inner);

        ///// <summary>
        ///// Parses inner, wrapped by { }
        ///// </summary>
        //public static Parser<T> braces<T>(Parser<T> inner) =>
        //    between(ch('{'), ch('}'), inner);

        ///// <summary>
        ///// Parses inner, wrapped by &lt; &gt;
        ///// </summary>
        //public static Parser<T> angles<T>(Parser<T> inner) =>
        //    between(ch('<'), ch('>'), inner);

        ///// <summary>
        ///// Parses a symbol: a string token
        ///// </summary>
        //public static Parser<string> symbol(string name) =>
        //    token(str(name))
        //   .label($"'{name}'");

        ///// <summary>
        ///// Parses reserved symbols
        ///// </summary>
        ///// <param name="names">List of reserved symbols</param>
        //public static Parser<string> reserved(IEnumerable<string> names) =>
        //    choice(names.Map(x=>symbol(x)))
        //   .label("reserved symbol");

        ///// <summary>
        ///// Parses an identifier: a letter followed by many letters or digits
        ///// </summary>
        //public readonly static Parser<string> ident =
        //    (from x  in letter
        //     from xs in many(letterOrDigit)
        //     select new string(x.Cons(xs).ToArray()))
        //    .label("identifier");

        ///// <summary>
        ///// Parses an identifier that must not match the reserved names
        ///// </summary>
        ///// <param name="names">Reserved names</param>
        //public static Parser<string> identifier(IEnumerable<string> names) =>
        //    token(from id  in ident
        //          from res in names.Contains(id)
        //                ? failure<string>("reserved identifier")
        //                : result(id)
        //          select res)
        //         .label("indentifier");

        ///// <summary>
        ///// Parses sep separated p, wrapped by left and right
        ///// </summary>
        //public static Parser<IEnumerable<T>> betweenAndSep<L,R,T,S>(Parser<L> left, Parser<R> right, Parser<S> sep, Parser<T> p) =>
        //    between(left, right, sepBy1(token(p), sep));

        ///// <summary>
        ///// Parses comma separated p, wrapped by [ ]
        ///// </summary>
        //public static Parser<IEnumerable<T>> commaBrackets<T>(Parser<T> p) =>
        //    betweenAndSep(ch('['), ch(']'), symbol(","), p);

        ///// <summary>
        ///// Parses comma separated p, wrapped by ( )
        ///// </summary>
        //public static Parser<IEnumerable<T>> commaParens<T>(Parser<T> p) =>
        //    betweenAndSep(ch('('), ch(')'), symbol(","), p);

        ///// <summary>
        ///// Parses comma separated p, wrapped by { }
        ///// </summary>
        //public static Parser<IEnumerable<T>> commaBraces<T>(Parser<T> p) =>
        //    betweenAndSep(ch('{'), ch('}'), symbol(","), p);

        ///// <summary>
        ///// Parses comma separated p, wrapped by &lt; &gt;
        ///// </summary>
        //public static Parser<IEnumerable<T>> commaAngles<T>(Parser<T> p) =>
        //    betweenAndSep(ch('<'), ch('>'), symbol(","), p);


        //readonly static Parser<char> stringLetter =
        //    satisfy(c => (c != '"') && (c != '\\') && (c > 26));

        //readonly static Lst<Tuple<char, char>> escapeMap =
        //    List.zip("abfnrtv\\\"\'", "\a\b\f\n\r\t\v\\\"\'").Freeze();

        //static Parser<T> parseEsc<T>(char c, T code) =>
        //    from _ in ch(c)
        //    select code;

        //static readonly Lst<Parser<char>> escapeMapParsers =
        //    escapeMap.Map(tup => parseEsc(tup.Item1, tup.Item2)).Freeze();

        //static readonly Parser<char> charEsc =
        //    choice(escapeMapParsers);

        //static readonly Parser<char> charNum =
        //    from code in choice(natural, hex, octal)
        //    select Char.ConvertFromUtf32(code)[0];

        //static readonly Parser<char> escapeCode =
        //    either(charEsc, charNum); // || charAscii || charControl

        //static readonly Parser<char> escapeEmpty =
        //    ch('&');

        //static readonly Parser<char> escapeGap =
        //    from sp in many1(ch(' '))
        //    from ch in ch('\\')
        //    select ch;

        //static readonly Parser<char> stringEscape =
        //    from _ in ch('\\')
        //    from e in choice(escapeGap, escapeEmpty, escapeCode)
        //    select e;

        ///// <summary>
        ///// Parses a character in a string-literal - deals with escape codes also
        ///// </summary>
        //public static readonly Parser<char> stringChar =
        //    either(stringLetter, stringEscape);

        ///// <summary>
        ///// Parses a multi-line string-literal as-is with no consideration of 
        ///// escape-codes
        ///// 
        ///// e.g. """a string"""
        ///// </summary>
        //public readonly static Parser<string> heavyString =
        //    asString(
        //        between(
        //            str("\"\"\""),
        //            str("\"\"\""),
        //            many(satisfy(c => c != '"'))));

        ///// <summary>
        ///// Parses a string-literal with full consideration of escape-codes
        ///// </summary>
        //public readonly static Parser<string> stringLiteral =
        //    token(
        //        either(
        //            attempt(heavyString),
        //            from a in ch('"')
        //            from s in asString(many(stringChar))
        //            from b in ch('"')
        //            select s))
        //   .label("string");

        ///// <summary>
        ///// Allows inline logging of parser results
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="p"></param>
        ///// <param name="Succ"></param>
        ///// <param name="Fail"></param>
        ///// <returns></returns>
        //public static Parser<T> log<T>(this Parser<T> p, Action<T> Succ, Action<ParserError> Fail) =>
        //    inp =>
        //    {
        //        var res = p(inp);
        //        if (res.Reply.Tag == ReplyTag.Error)
        //        {
        //            Fail(res.Reply.Error);
        //        }
        //        else
        //        {
        //            Succ(res.Reply.Result);
        //        }
        //        return res;
        //    };

    }
}

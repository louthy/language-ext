using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.Parsec.ParserResult;
using static LanguageExt.Parsec.Internal;
using static LanguageExt.Parsec.Prim;
using LanguageExt.TypeClasses;

namespace LanguageExt.Parsec
{
    /// <summary>
    /// Commonly used character parsers.
    /// </summary>
    public static class Char
    {
        static Char()
        {
            space = satisfy(System.Char.IsWhiteSpace).label("space");
            spaces = skipMany(space).label("white space");
            control = satisfy(System.Char.IsControl).label("control");
            tab = satisfy(c => c == '\t').label("tab");
#pragma warning disable CS0618 // Type or member is obsolete
            newline = satisfy(c => c == '\n').label("lf new-line");
#pragma warning restore CS0618 // Type or member is obsolete
            LF = satisfy(c => c == '\n').label("lf new-line");
            CR = satisfy(c => c == '\r').label("cr carriage-return");
            CRLF = (from cr in ch('\r')
                    from nl in ch('\n')
                    select nl)
                   .label("crlf new-line");
            endOfLine = either(LF, CRLF).label("new-line");
            digit = satisfy(System.Char.IsDigit).label("digit");
            letter = satisfy(System.Char.IsLetter).label("letter");
            alphaNum = satisfy(System.Char.IsLetterOrDigit).label("letter or digit");
            lower = satisfy(System.Char.IsLower).label("lowercase letter");
            upper = satisfy(System.Char.IsUpper).label("uppercase letter");
            punctuation = satisfy(System.Char.IsPunctuation).label("punctuation");
            separator = satisfy(System.Char.IsSeparator).label("separator");
            symbolchar = satisfy(System.Char.IsSymbol).label("symbolchar");
            octDigit = satisfy(c => "01234567".Contains(c)).label("octal digit");
            hexDigit = satisfy(c => System.Char.IsDigit(c) || "abcdefABCDEF".Contains(c)).label("hexadecimal digit");
            anyChar = satisfy(_ => true);
        }

        /// <summary>
        /// ch(c) parses a single character c
        /// </summary>
        /// <returns>The parsed character</returns>
        public static Parser<char> ch(char c) =>
            satisfy(x => x == c).label($"'{c}'");

        /// <summary>
        /// ch(c) parses a single character c
        /// </summary>
        /// <typeparam name="EQ">Eq<char> type-class</typeparam>
        /// <returns>The parsed character</returns>
        public static Parser<char> ch<EQ>(char c) where EQ : struct, Eq<char> =>
            satisfy(x => default(EQ).Equals(x, c)).label($"'{c}'");

        /// <summary>
        /// The parser satisfy(pred) succeeds for any character for which the
        /// supplied function pred returns 'True'. 
        /// </summary>
        /// <returns>
        /// The character that is actually parsed.</returns>
        public static Parser<char> satisfy(Func<char, bool> pred) =>
            inp =>
            {
                if (inp.Index >= inp.EndIndex)
                {
                    return EmptyError<char>(ParserError.SysUnexpect(inp.Pos, "end of stream"));
                }
                else
                {
                    var ns = newstate(inp);

                    if (ns.Tag == ResultTag.Consumed)
                    {
                        if (pred(ns.Reply.Result))
                        {
                            return ns;
                        }
                        else
                        {
                            return EmptyError<char>(ParserError.SysUnexpect(inp.Pos, $"\"{ns.Reply.Result}\""));
                        }
                    }
                    else
                    {
                        return EmptyError<char>(ParserError.SysUnexpect(inp.Pos, "end of stream"));
                    }
                }
            };

        /// <summary>
        /// oneOf(str) succeeds if the current character is in the supplied list of 
        /// characters str. Returns the parsed character. See also satisfy
        /// </summary>
        public static Parser<char> oneOf(string str) =>
            satisfy(c => str.Contains(c));

        /// <summary>
        /// oneOf(str) succeeds if the current character is in the supplied list of 
        /// characters str. Returns the parsed character. See also satisfy
        /// </summary>
        public static Parser<char> oneOf(params char[] str) =>
            satisfy(c => str.Contains(c));

        /// <summary>
        /// As the dual of 'oneOf', noneOf(str) succeeds if the current
        /// character not in the supplied list of characters str. 
        /// 
        ///     var consonant = noneOf("aeiou")
        /// </summary>
        /// <returns>
        /// The parsed character.</returns>
        public static Parser<char> noneOf(string str) =>
            satisfy(c => !str.Contains(c));

        /// <summary>
        /// As the dual of 'oneOf', noneOf(str) succeeds if the current
        /// character not in the supplied list of characters str. 
        /// 
        ///     var consonant = noneOf("aeiou")
        /// </summary>
        /// <returns>
        /// The parsed character.</returns>
        public static Parser<char> noneOf(params char[] str) =>
            satisfy(c => !str.Contains(c));

        /// <summary>
        /// Parses a white space character (any character which satisfies 'System.Char.IsWhiteSpace')
        /// Returns the parsed character.
        /// </summary>
        public static readonly Parser<char> space;

        /// <summary>
        /// Skips zero or more white space characters. See also 'skipMany'.
        /// </summary>
        public static readonly Parser<Unit> spaces;

        /// <summary>
        /// Parses a control character
        /// Returns the parsed character.
        /// </summary>
        public static readonly Parser<char> control;

        /// <summary>
        /// Parses a tab
        /// Returns the parsed character.
        /// </summary>
        public static readonly Parser<char> tab;

        /// <summary>
        /// Equivalent to `LF`. Parses a line-feed newline char (\n). 
        /// Returns the parsed character.
        /// </summary>
        [Obsolete("This parses only \\n (regardless of environment). Use (explicit) parser `LF` instead. Related parsers: `CRLF` and `endOfLine` (parsing CRLF or LF).")]
        public static readonly Parser<char> newline;

        /// <summary>
        /// Parses a carriage-return char (\r)
        /// Returns the parsed character.
        /// </summary>
        public static readonly Parser<char> CR;

        /// <summary>
        /// Parses a line-feed newline char (\n)
        /// Returns the parsed character.
        /// </summary>
        public static readonly Parser<char> LF;
 
        /// <summary>
        /// Parses a carriage-return then line-feed
        /// Returns the new-line.
        /// </summary>
        public static readonly Parser<char> CRLF;

        /// <summary>
        /// Parses a CRLF (see 'crlf') or LF (see 'newline') end-of-line.
        /// Returns a newline character(\'\\n\').
        /// </summary>
        public static readonly Parser<char> endOfLine;

        /// <summary>
        /// Parses a digit
        /// Returns the parsed character.
        /// </summary>
        public static readonly Parser<char> digit;

        /// <summary>
        /// Parses a letter
        /// Returns the parsed character.
        /// </summary>
        public static readonly Parser<char> letter;

        /// <summary>
        /// Parses a letter or digit
        /// Returns the parsed character.
        /// </summary>
        public static readonly Parser<char> alphaNum;

        /// <summary>
        /// Parses a lowercase letter
        /// Returns the parsed character.
        /// </summary>
        public static readonly Parser<char> lower;

        /// <summary>
        /// Parses a uppercase letter
        /// Returns the parsed character.
        /// </summary>
        public static readonly Parser<char> upper;

        /// <summary>
        /// Parses a punctuation character
        /// Returns the parsed character.
        /// </summary>
        public static readonly Parser<char> punctuation;

        /// <summary>
        /// Parses a separator character
        /// Returns the parsed character.
        /// </summary>
        public static readonly Parser<char> separator;

        /// <summary>
        /// Parses a symbol character
        /// Returns the parsed character.
        /// </summary>
        public static readonly Parser<char> symbolchar;

        /// <summary>
        /// Parses an octal digit (0-7)
        /// </summary>
        public readonly static Parser<char> octDigit;

        /// <summary>
        /// Parses a hex digit (0-F | 0-f)
        /// </summary>
        public readonly static Parser<char> hexDigit;

        /// <summary>
        /// The parser anyChar accepts any kind of character.
        /// </summary>
        public readonly static Parser<char> anyChar;

        /// <summary>
        /// Parse a string
        /// </summary>
        public static Parser<string> str(string s) =>
            asString(chain(toSeq(s.Map(ch)))).label($"'{s}'");

        /// <summary>
        /// Parse a string case insensitive (char by char)
        /// <typeparam name="EQ">Eq<char> type-class</typeparam>
        /// </summary>
        public static Parser<string> str<EQ>(string s) where EQ: struct, Eq<char>  =>
            asString(chain(toSeq(s.Map(ch<EQ>)))).label($"'{s}'");
    }
}

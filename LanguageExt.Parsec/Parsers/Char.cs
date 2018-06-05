using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.Parsec.ParserResult;
using static LanguageExt.Parsec.Internal;
using static LanguageExt.Parsec.Prim;

namespace LanguageExt.Parsec
{
    /// <summary>
    /// Commonly used character parsers.
    /// </summary>
    public static class Char
    {
        /// <summary>
        /// ch(c) parses a single character c
        /// </summary>
        /// <returns>The parsed character</returns>
        public static Parser<char> ch(char c) =>
            satisfy(x => x == c).label($"'{c}'");

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
        public static readonly Parser<char> space =
            satisfy(System.Char.IsWhiteSpace).label("space");

        /// <summary>
        /// Skips zero or more white space characters. See also 'skipMany'.
        /// </summary>
        public static readonly Parser<Unit> spaces =
            skipMany(space).label("white space");

        /// <summary>
        /// Parses a control character
        /// Returns the parsed character.
        /// </summary>
        public static readonly Parser<char> control =
            satisfy(System.Char.IsControl).label("control");

        /// <summary>
        /// Parses a tab
        /// Returns the parsed character.
        /// </summary>
        public static readonly Parser<char> tab =
            satisfy(c => c == '\t').label("tab");

        /// <summary>
        /// Parses a line-feed newline char (\n)
        /// Returns the parsed character.
        /// </summary>
        public static readonly Parser<char> newline =
            satisfy(c => c == '\n').label("lf new-line");

        /// <summary>
        /// Parses a carriage-return char (\r)
        /// Returns the parsed character.
        /// </summary>
        public static readonly Parser<char> CR =
            satisfy(c => c == '\r').label("cr carriage-return");

        /// <summary>
        /// Parses a carriage-return then line-feed
        /// Returns the new-line.
        /// </summary>
        public static readonly Parser<char> CRLF =
            (from cr in ch('\r')
             from nl in ch('\n')
             select nl)
            .label("crlf new-line");

        /// <summary>
        /// Parses a CRLF (see 'crlf') or LF (see 'newline') end-of-line.
        /// Returns a newline character(\'\\n\').
        /// </summary>
        public static readonly Parser<char> endOfLine =
             either(newline, CRLF).label("new-line");

        /// <summary>
        /// Parses a digit
        /// Returns the parsed character.
        /// </summary>
        public static readonly Parser<char> digit =
            satisfy(System.Char.IsDigit).label("digit");

        /// <summary>
        /// Parses a letter
        /// Returns the parsed character.
        /// </summary>
        public static readonly Parser<char> letter =
            satisfy(System.Char.IsLetter).label("letter");

        /// <summary>
        /// Parses a letter or digit
        /// Returns the parsed character.
        /// </summary>
        public static readonly Parser<char> alphaNum =
            satisfy(System.Char.IsLetterOrDigit).label("letter or digit");

        /// <summary>
        /// Parses a lowercase letter
        /// Returns the parsed character.
        /// </summary>
        public static readonly Parser<char> lower =
            satisfy(System.Char.IsLower).label("lowercase letter");

        /// <summary>
        /// Parses a uppercase letter
        /// Returns the parsed character.
        /// </summary>
        public static readonly Parser<char> upper =
            satisfy(System.Char.IsUpper).label("uppercase letter");

        /// <summary>
        /// Parses a punctuation character
        /// Returns the parsed character.
        /// </summary>
        public static readonly Parser<char> punctuation =
            satisfy(System.Char.IsPunctuation).label("punctuation");

        /// <summary>
        /// Parses a separator character
        /// Returns the parsed character.
        /// </summary>
        public static readonly Parser<char> separator =
            satisfy(System.Char.IsSeparator).label("separator");

        /// <summary>
        /// Parses a symbol character
        /// Returns the parsed character.
        /// </summary>
        public static readonly Parser<char> symbolchar =
            satisfy(System.Char.IsSymbol).label("symbolchar");

        /// <summary>
        /// Parses an octal digit (0-7)
        /// </summary>
        public readonly static Parser<char> octDigit =
            satisfy(c => "01234567".Contains(c))
           .label("octal digit");

        /// <summary>
        /// Parses a hex digit (0-F | 0-f)
        /// </summary>
        public readonly static Parser<char> hexDigit =
            satisfy(c => System.Char.IsDigit(c) || "abcdefABCDEF".Contains(c))
           .label("hexadecimal digit");

        /// <summary>
        /// The parser anyChar accepts any kind of character.
        /// </summary>
        public readonly static Parser<char> anyChar =
            satisfy(_ => true);

        /// <summary>
        /// Parse a string
        /// </summary>
        public static Parser<string> str(string s) =>
            asString(chain(Seq(s.Map(ch)))).label($"'{s}'");
    }
}

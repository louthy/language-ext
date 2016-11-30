using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.Parsec.ParserResultIO;
using static LanguageExt.Parsec.InternalIO;
using static LanguageExt.Parsec.PrimT;

namespace LanguageExt.Parsec
{
    /// <summary>
    /// Commonly used character parsers.
    /// </summary>
    public static class ItemT
    {
        /// <summary>
        /// item(c) parses a single I
        /// </summary>
        /// <returns>The parsed character</returns>
        public static Parser<T, T> item<T>(T c) =>
            satisfy<T>(x => EqualityComparer<T>.Default.Equals(x,c)).label($"'{c}'");

        /// <summary>
        /// The parser satisfy(pred) succeeds for any character for which the
        /// supplied function pred returns 'True'. 
        /// </summary>
        /// <returns>
        /// The character that is actually parsed.</returns>
        public static Parser<T, T> satisfy<T>(Func<T, bool> pred) =>
            inp =>
            {
                if (inp.Index >= inp.EndIndex)
                {
                    return EmptyError<T, T>(ParserError.SysUnexpect(inp.Pos, "end of stream"));
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
                            return EmptyError<T, T>(ParserError.SysUnexpect(inp.Pos, $"\"{ns.Reply.Result}\""));
                        }
                    }
                    else
                    {
                        return EmptyError<T, T>(ParserError.SysUnexpect(inp.Pos, "end of stream"));
                    }
                }
            };

        /// <summary>
        /// oneOf(str) succeeds if the current character is in the supplied list of 
        /// characters str. Returns the parsed character. See also satisfy
        /// </summary>
        public static Parser<T, T> oneOf<T>(T[] str) =>
            satisfy<T>(c => str.Contains(c));

        /// <summary>
        /// As the dual of 'oneOf', noneOf(str) succeeds if the current
        /// character not in the supplied list of characters str. 
        /// 
        ///     var consonant = noneOf("aeiou")
        /// </summary>
        /// <returns>
        /// The parsed character.</returns>
        public static Parser<T, T> noneOf<T>(T[] str) =>
            satisfy<T>(c => !str.Contains(c));

        /// <summary>
        /// The parser anyChar accepts any kind of character.
        /// </summary>
        public static Parser<T, T> anyItem<T>() =>
            satisfy<T>(_ => true);

        /// <summary>
        /// Parse a string
        /// </summary>
        public static Parser<T, T[]> str<T>(T[] s) =>
            chain(s.Map(c => item(c))).Map(x => x.ToArray());
    }
}

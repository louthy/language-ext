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
        public static Parser<A, A> item<A>(A c) =>
            satisfy<A>(x => EqualityComparer<A>.Default.Equals(x,c)).label($"'{c}'");

        /// <summary>
        /// The parser satisfy(pred) succeeds for any character for which the
        /// supplied function pred returns 'True'. 
        /// </summary>
        /// <returns>
        /// The character that is actually parsed.</returns>
        public static Parser<A, A> satisfy<A>(Func<A, bool> pred) =>
            inp =>
            {
                if (inp.Index >= inp.EndIndex)
                {
                    return EmptyError<A, A>(ParserError.SysUnexpect(inp.Pos, "end of stream"));
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
                            return EmptyError<A, A>(ParserError.SysUnexpect(inp.Pos, $"\"{ns.Reply.Result}\""));
                        }
                    }
                    else
                    {
                        return EmptyError<A, A>(ParserError.SysUnexpect(inp.Pos, "end of stream"));
                    }
                }
            };

        /// <summary>
        /// oneOf(str) succeeds if the current character is in the supplied list of 
        /// characters str. Returns the parsed character. See also satisfy
        /// </summary>
        public static Parser<A, A> oneOf<A>(A[] str) =>
            satisfy<A>(c => str.Contains(c));

        /// <summary>
        /// As the dual of 'oneOf', noneOf(str) succeeds if the current
        /// character not in the supplied list of characters str. 
        /// 
        ///     var consonant = noneOf("aeiou")
        /// </summary>
        /// <returns>
        /// The parsed character.</returns>
        public static Parser<A, A> noneOf<A>(A[] str) =>
            satisfy<A>(c => !str.Contains(c));

        /// <summary>
        /// The parser anyChar accepts any kind of character.
        /// </summary>
        public static Parser<A, A> anyItem<A>() =>
            satisfy<A>(_ => true);

        /// <summary>
        /// Parse a string
        /// </summary>
        public static Parser<A, A[]> str<A>(A[] s) =>
            chain(s.Map(c => item(c))).Map(x => x.ToArray());
    }
}

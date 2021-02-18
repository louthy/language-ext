using System;

namespace LanguageExt.Parsec
{
    public static class Indent
    {
        /// <summary>
        /// Parses only when indented past the level of the reference
        /// </summary>
        public static Parser<T> indented<T>(int offset, Parser<T> p) =>
            inp =>
            {
                var col = inp.Pos.Column + offset;
                var newpos = inp.Pos;

                for (var index = inp.Index; index < inp.EndIndex; index++)
                {
                    var x = inp.Value[index];

                    if(newpos.Column < col && newpos.Line > inp.Pos.Line && x != ' ' && x != '\t' && x != '\n' && x != '\r')
                    {
                        // first char that's not white-space and is left of the reference
                        var block = inp.SetEndIndex(index);
                        var res = p(block);
                        return res.SetEndIndex(inp.EndIndex);
                    }

                    newpos = x == '\n' ? new Pos(newpos.Line + 1, 0)
                           : x == '\t' ? new Pos(newpos.Line, ((newpos.Column / 4) + 1) * 4)
                           : new Pos(newpos.Line, newpos.Column + 1);
                }

                return p(inp);
            };

        /// <summary>
        /// Parses only when indented zero or more characters past the level of the reference
        /// </summary>
        public static Parser<T> indented<T>(Parser<T> p) =>
            indented(0, p);

        /// <summary>
        /// Parses only when indented one or more characters past the level of the reference
        /// </summary>
        public static Parser<T> indented1<T>(Parser<T> p) =>
            indented(1, p);

        /// <summary>
        /// Parses only when indented two or more characters past the level of the reference
        /// </summary>
        public static Parser<T> indented2<T>(Parser<T> p) =>
            indented(2, p);

        /// <summary>
        /// Parses only when indented four or more characters past the level of the reference
        /// </summary>
        public static Parser<T> indented4<T>(Parser<T> p) =>
            indented(4, p);

    }
}

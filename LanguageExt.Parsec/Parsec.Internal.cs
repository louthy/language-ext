using System;
using System.Collections.Generic;
using System.Linq;
using static LanguageExt.Parsec;
using static LanguageExt.ParserResult;

namespace LanguageExt
{
    static class ParsecInternal
    {
        public static bool onside(Pos pos, Pos delta) =>
            pos.Column > delta.Column || pos.Line == delta.Line;

        public static readonly Parser<Pos> getPos =
            (PString inp) => ConsumedOK(inp.Pos, inp);

        public static readonly Parser<Pos> getDefPos =
            (PString inp) => ConsumedOK(inp.DefPos, inp);

        public static Parser<T> setDefPos<T>(Pos defpos, Parser<T> p) =>
            (PString inp) => p(inp.SetDefPos(defpos));

        public static string concat(IEnumerable<char> chs) =>
            new string(chs.ToArray());

        public static ParserResult<char> newstate(PString inp)
        {
            var x = inp.Value[inp.Index];

            var newpos = x == '\n' ? new Pos(inp.Pos.Line + 1, 0)
                       : x == '\t' ? new Pos(inp.Pos.Line, ((inp.Pos.Column / 4) + 1) * 4)
                       : new Pos(inp.Pos.Line, inp.Pos.Column + 1);

            return ConsumedOK(x,
                new PString(
                    inp.Value,
                    inp.Index + 1,
                    newpos,
                    inp.DefPos,
                    onside(newpos, inp.DefPos)
                        ? Sidedness.Onside
                        : Sidedness.Offside,
                    inp.UserState));
        }
    }
}

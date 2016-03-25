using System;
using System.Collections.Generic;
using System.Linq;
using static LanguageExt.Parsec.Prim;
using static LanguageExt.Parsec.ParserResult;

namespace LanguageExt.Parsec
{
    static class Internal
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

        public static Parser<T> choicei<T>(Parser<T>[] ps, int index) =>
           index == ps.Length - 1
                ? ps[index]
                : either(ps[index], choicei(ps, index + 1));

        public static Parser<IEnumerable<T>> chaini<T>(Parser<T>[] ps, int index) =>
           index == ps.Length - 1
                ? ps[index].Map(x => new[] { x }.AsEnumerable())
                : from x in ps[index]
                  from y in chaini(ps, index + 1)
                  select x.Cons(y);

        public static Parser<IEnumerable<T>> counti<T>(int n, Parser<T> p) =>
           n <= 0
                ? result(new T [0].AsEnumerable())
                : from x in p
                  from y in counti(n-1, p)
                  select x.Cons(y);

    }
}

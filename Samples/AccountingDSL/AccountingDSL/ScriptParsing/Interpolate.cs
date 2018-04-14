using LanguageExt.Parsec;
using static LanguageExt.Prelude;
using static LanguageExt.Parsec.Char;
using static LanguageExt.Parsec.Prim;

namespace AccountingDSL.ScriptParsing
{
    public static class Interpolate
    {
        public static readonly Parser<ScriptExpr> Parser;

        static Interpolate()
        {
            var text = from s in asString(many1(attempt(satisfy(c => c != '{'))))
                       select ScriptExpr.ConstString(s);

            var format = from o in ch('{')
                         from s in asString(many1(attempt(satisfy(c => c != '}'))))
                         from c in ch('}')
                         from d in ScriptParser.Parse(s)
                                    .Match(
                                        Right: dsl => result(dsl),
                                        Left: e => failure<ScriptExpr>(e))
                         select d;

            var blocks = many(attempt(choice(attempt(text), format)));

            Parser = from b in blocks
                     from _ in eof
                     select b.IsEmpty
                        ? ScriptExpr.ConstString("")
                        : b.Tail.IsEmpty && b.Head is StringConstExpr
                            ? b.Head
                            : ScriptExpr.InterpString(b);
        }
    }
}

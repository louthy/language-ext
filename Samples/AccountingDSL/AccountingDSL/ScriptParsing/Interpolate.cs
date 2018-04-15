using LanguageExt.Parsec;
using static LanguageExt.Prelude;
using static LanguageExt.Parsec.Char;
using static LanguageExt.Parsec.Prim;

namespace AccountingDSL.ScriptParsing
{
    /// <summary>
    /// Parses a string literal that has interpolation blocks within and 
    /// returns an expression represeting the components (or a ConstString
    /// if no blocks are found).
    /// </summary>
    public static class Interpolate
    {
        public static readonly Parser<ScriptExpr> Parser;

        static Interpolate()
        {
            // Parser for a text block
            var text = from s in asString(many1(attempt(satisfy(c => c != '{'))))
                       select ScriptExpr.ConstString(s);

            // Parser for the interpolation block
            var format = from o in ch('{')
                         from s in asString(many1(attempt(satisfy(c => c != '}'))))
                         from c in ch('}')
                         from d in ScriptParser.Parse(s)
                                    .Match(
                                        Right: dsl => result(dsl),
                                        Left: e => failure<ScriptExpr>(e))
                         select d;

            // Parser for many blocks
            var blocks = many(attempt(choice(attempt(text), format)));

            // Final parser which does some post processing on the blocks
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

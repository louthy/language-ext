using System;
using System.Linq;
using LanguageExt;
using LanguageExt.Parsec;
using static LanguageExt.Prelude;
using static LanguageExt.Parsec.Char;
using static LanguageExt.Parsec.Expr;
using static LanguageExt.Parsec.Token;
using static LanguageExt.Parsec.Prim;
using static AccountingDSL.ScriptParsing.ScriptExpr;

namespace AccountingDSL.ScriptParsing
{
    public static class ScriptParser
    {
        static Parser<int> integer;
        static Parser<double> floating;
        static Parser<string> stringlit;
        static Parser<char> charlit;
        static Parser<string> ident;
        static Func<string, Parser<string>> reserved;
        static Func<string, Parser<Unit>> reservedOp;
        static Func<string, Parser<string>> symbol;
        static Parser<Unit> whiteSpace;
        static readonly GenTokenParser lexer;
        static readonly Parser<ScriptExpr> parser;

        static ScriptParser()
        {
            var opChars = ":!%&*+.<=>\\^|-~()[].";

            var definition = GenLanguageDef.Empty.With(
                CommentStart: "/*",
                CommentEnd: "*/",
                CommentLine: "//",
                NestedComments: true,
                OpStart: oneOf(opChars),
                OpLetter: oneOf(opChars),
                IdentStart: letter,
                IdentLetter: either(alphaNum, oneOf("-_")),
                ReservedNames: List("log", "fail", "Sum", "true", "false", "unit"),
                ReservedOpNames: List(";", "&&", "&", "-", "+", "/", "*", "==", "!=",
                                      "=", "<-", "<<", ">>", ">", "<", "<=", ">=",
                                      "=>", "||", "|", ",")
            );

            lexer = makeTokenParser(definition);

            charlit = lexer.CharLiteral;
            ident = either(
                        lexer.Identifier,
                        from t in token(letter)
                        from _ in notFollowedBy(letter)
                        select t.ToString());
            reserved = lexer.Reserved;
            reservedOp = lexer.ReservedOp;
            symbol = lexer.Symbol;
            whiteSpace = lexer.WhiteSpace;
            integer = lexer.Integer;
            floating = lexer.Float;
            stringlit = lexer.StringLiteral;

            Func<ScriptExpr, ScriptExpr, ScriptExpr> BinaryOp(string op) =>
                (ScriptExpr lhs, ScriptExpr rhs) =>
                      op == "-" ? Binary(lhs, rhs, "-")
                    : op == "+" ? Binary(lhs, rhs, "+")
                    : op == "/" ? Binary(lhs, rhs, "/")
                    : op == "*" ? Binary(lhs, rhs, "*")
                    : op == "==" ? BooleanBinary(lhs, rhs, "==")
                    : op == "!=" ? BooleanBinary(lhs, rhs, "!=")
                    : op == "=" ? Binary(lhs, rhs, "=")
                    : op == "<-" ? Binary(lhs, rhs, "<-")
                    : op == ">" ? BooleanBinary(lhs, rhs, ">")
                    : op == "<" ? BooleanBinary(lhs, rhs, "<")
                    : op == ">=" ? BooleanBinary(lhs, rhs, ">=")
                    : op == "<=" ? BooleanBinary(lhs, rhs, "<=")
                    : op == "&&" ? BooleanBinary(lhs, rhs, "&&")
                    : op == "||" ? BooleanBinary(lhs, rhs, "||")
                    : op == "%" ? Binary(lhs, rhs, "%")
                    : op == "??" ? Binary(lhs, rhs, "??")
                    : op == ";" ? Binary(lhs, rhs, ";")
                    : op == "," ? Args(lhs, rhs)
                    : throw new NotSupportedException();

            Func<ScriptExpr, ScriptExpr> PrefixOp(string op) =>
                (ScriptExpr rhs) =>
                      op == "!" ? BooleanPrefix(rhs, "!")
                    : op == "-" ? Prefix(rhs, "-")
                    : op == "+" ? rhs
                    : throw new NotSupportedException();

            // Binary operator parser
            Operator<ScriptExpr> binary(string name, Assoc assoc) =>
                Operator.Infix(assoc,
                    from x in reservedOp(name)
                    select BinaryOp(name));

            // Prefix operator parser
            Operator<ScriptExpr> prefix(string name) =>
                Operator.Prefix(
                    from x in reservedOp(name)
                    select PrefixOp(name));

            Operator<ScriptExpr>[][] operators = {
                new [] { prefix("-"), prefix("+"), prefix("!") },
                new [] { binary("*", Assoc.Left), binary("/", Assoc.Left), binary("%", Assoc.Left) },
                new [] { binary("+", Assoc.Left), binary("-", Assoc.Left) },
                new [] { binary("<<", Assoc.Left), binary(">>", Assoc.Left) },
                new [] { binary("<", Assoc.Left), binary(">", Assoc.Left), binary(">=", Assoc.Left) , binary("<=", Assoc.Left) },
                new [] { binary("==", Assoc.Left), binary("!=", Assoc.Left) },
                new [] { binary("&", Assoc.Left) },
                new [] { binary("|", Assoc.Left) },
                new [] { binary("&&", Assoc.Left) },
                new [] { binary("||", Assoc.Left) },
                new [] { binary("??", Assoc.Left) },
                new [] { binary(">>=", Assoc.Right) },
                new [] { binary("?", Assoc.Left) },
                new [] { binary(":", Assoc.Right) },
                new [] { binary("=", Assoc.Right), binary("<-", Assoc.Right) },
                new [] { binary(".", Assoc.Right) },
                new [] { binary(",", Assoc.Right) },
                new [] { binary("=>", Assoc.Right) },
                new [] { binary(";", Assoc.Right) },
            };

            Parser<ScriptExpr> expr = null;
            Parser<ScriptExpr> lazyExpr = lazyp(() => expr);

            var interpStr = from str in attempt(stringlit).label("string literal")
                            from res2 in (str.Contains('{') && str.Contains('}'))
                                ? from istr in result(Interpolate.Parser(str.ToPString()))
                                  from res in istr.IsFaulted
                                      ? failure<ScriptExpr>(istr.Reply.Error.Msg)
                                      : result(istr.Reply.Result)
                                  select res
                                : result(ConstString(str))
                            select res2;

            var constant = choice(
                attempt(integer.Map(Const).label("integer")),
                attempt(charlit.Map(ConstChar).label("char literal")),
                attempt(interpStr),
                attempt(floating.Map(Const).label("float")),
                attempt(symbol("true").Map(_ => Const(true)).label("true")),
                symbol("false").Map(_ => Const(false)).label("false"));

            var invoke = from id in ident
                         from ar in either(
                             attempt(symbol("()")).Map(_ => Seq<ScriptExpr>()),
                             from op in symbol("(")
                             from args in lazyExpr
                             from cl in symbol(")")
                             select FlattenTuple(args))
                         select Invoke(id, ar);

            var log = from _ in symbol("log")
                      from o in symbol("(")
                      from m in lazyExpr
                      from c in symbol(")")
                      select Log(m);

            var fail = from _ in symbol("fail")
                       from o in symbol("(")
                       from m in lazyExpr
                       from c in symbol(")")
                       select Fail(m);

            var term =
                choice(
                    attempt(log),
                    attempt(fail),
                    attempt(invoke),
                    attempt(constant),
                    attempt(ident).Map(Ident),
                    token(lexer.Parens(lazyExpr)).Map(Parens));

            expr = buildExpressionParser(operators.Reverse().ToArray(), term);

            parser = from ws in whiteSpace
                     from ex in expr
                     from __ in eof
                     select ex;
        }

        public static Either<string, ScriptExpr> Parse(string str)
        {
            var res = parser(str.TrimEnd('\n', '\r', '\t', ' ', ';').ToPString());
            return res.ToEither();
        }

        static Parser<A> token<A>(Parser<A> p) =>
            from to in p
            from ws in whiteSpace
            select to;

        public static Seq<ScriptExpr> FlattenTuple(ScriptExpr expr) =>
            expr is TupleExpr tuple
                ? Prelude.Cons(tuple.Left, FlattenTuple(tuple.Right))
                : Seq1(expr);
    }
}

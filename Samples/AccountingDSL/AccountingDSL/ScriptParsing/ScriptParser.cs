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
    /// <summary>
    /// Script parser that uses LanguageExt.Parsec to turn text into a abstract syntax tree
    /// of type: ScriptExpr.
    /// </summary>
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
            // All characters that can be operators
            var opChars = ":!%&*+.<=>\\^|-~()[].";

            // Defines the components of the scripting language
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

            // Takes the definition and builds a set of core parsers that
            // can be used to build more complex parsers.
            lexer = makeTokenParser(definition);

            // Take some local copies of the lexer parsers for ease of use later
            charlit = lexer.CharLiteral;
            ident = either(
                        lexer.Identifier,
                        from t in token(letter)
                        from _ in notFollowedBy(letter)
                        select t.ToString());
            reserved = lexer.Reserved;
            reservedOp = lexer.ReservedOp;
            symbol = lexer.Symbol;
            whiteSpace = lexer.WhiteSpace; // This incidentally will also strip comments
            integer = lexer.Integer;
            floating = lexer.Float;
            stringlit = lexer.StringLiteral;

            // Helper method that takes an operator string and returns a Func
            // that makes it easy to build ScriptExprs that represent binary
            // operators
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

            // Helper method that takes an operator string and returns a Func
            // that makes it easy to build ScriptExprs that represent unary
            // prefix operators
            Func<ScriptExpr, ScriptExpr> PrefixOp(string op) =>
                (ScriptExpr rhs) =>
                      op == "!" ? BooleanPrefix(rhs, "!")
                    : op == "-" ? Prefix(rhs, "-")
                    : op == "+" ? rhs
                    : throw new NotSupportedException();

            // Binary operator parser definition - this is used in the operators table
            // to define precendence and associativity.  
            Operator<ScriptExpr> binary(string name, Assoc assoc) =>
                Operator.Infix(assoc,
                    from x in reservedOp(name)
                    select BinaryOp(name));

            // Unary prefix operator parser definition - this is used in the operators 
            // table to define precendence and associativity.  
            Operator<ScriptExpr> prefix(string name) =>
                Operator.Prefix(
                    from x in reservedOp(name)
                    select PrefixOp(name));

            // Operator precedence and associativity table
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

            // Constant or interpolated string parser
            var interpStr = from str in attempt(stringlit).label("string literal")
                            from res2 in (str.Contains('{') && str.Contains('}'))
                                ? from istr in result(Interpolate.Parser(str.ToPString()))
                                  from res in istr.IsFaulted
                                      ? failure<ScriptExpr>(istr.Reply.Error.Msg)
                                      : result(istr.Reply.Result)
                                  select res
                                : result(ConstString(str))
                            select res2;

            // Constant parser
            var constant = choice(
                attempt(integer.Map(Const).label("integer")),
                attempt(charlit.Map(ConstChar).label("char literal")),
                attempt(interpStr),
                attempt(floating.Map(Const).label("float")),
                attempt(symbol("true").Map(_ => Const(true)).label("true")),
                symbol("false").Map(_ => Const(false)).label("false"));

            // Function invocation parser
            var invoke = from id in ident
                         from ar in either(
                             attempt(symbol("()")).Map(_ => Seq<ScriptExpr>()),
                             from op in symbol("(")
                             from args in lazyExpr
                             from cl in symbol(")")
                             select FlattenTuple(args))
                         select Invoke(id, ar);

            // log function parser
            var log = from _ in symbol("log")
                      from o in symbol("(")
                      from m in lazyExpr
                      from c in symbol(")")
                      select Log(m);

            // fail function parser
            var fail = from _ in symbol("fail")
                       from o in symbol("(")
                       from m in lazyExpr
                       from c in symbol(")")
                       select Fail(m);

            // Term parser - this is the main building block for the expression parser
            //               all language terms must be definied here.
            var term =
                choice(
                    attempt(log),
                    attempt(fail),
                    attempt(invoke),
                    attempt(constant),
                    attempt(ident).Map(Ident),
                    token(lexer.Parens(lazyExpr)).Map(Parens));

            // This takes the operator precedence and associativity table and the term
            // parser and builds a full expression parser that deals with all of the 
            // complex rules over precedence and associativity.  This is a wonderful
            // function that saves lives.
            expr = buildExpressionParser(operators.Reverse().ToArray(), term);

            // This is the final parser.  Because the tokens and the symbols all strip
            // the space *after* their content has been parsed, the final parser must
            // strip the space *before* the first token.  So, that's what this does, 
            // removes the space, parses the expression, then it expects an end-of-file
            parser = from ws in whiteSpace
                     from ex in expr
                     from __ in eof
                     select ex;
        }

        /// <summary>
        /// Helper function for parsing a script.  It strips the last semi-colon because
        /// it's a common mistake to leave it on when it's not needed.  This just saves 
        /// some unnecessary complexity in the parser itself.
        /// </summary>
        public static Either<string, ScriptExpr> Parse(string str)
        {
            var res = parser(str.TrimEnd('\n', '\r', '\t', ' ', ';').ToPString());
            return res.ToEither();
        }

        /// <summary>
        /// Token parser - this takes another parser, runs it, and then strips
        /// the trailing whiteSpace.
        /// </summary>
        static Parser<A> token<A>(Parser<A> p) =>
            from to in p
            from ws in whiteSpace
            select to;

        /// <summary>
        /// Helper function for taking the tree expression that you get with comma
        /// separated values and flattens them inta Seq of ScriptExprs.
        /// </summary>
        static Seq<ScriptExpr> FlattenTuple(ScriptExpr expr) =>
            expr is TupleExpr tuple
                ? Prelude.Cons(tuple.Left, FlattenTuple(tuple.Right))
                : Seq1(expr);
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

using LanguageExt;
using M = LanguageExt.Map;
using LanguageExt.Parsec;
using static LanguageExt.Prelude;
using static LanguageExt.Parsec.Prim;
using static LanguageExt.Parsec.Char;
using static LanguageExt.Parsec.Expr;
using static LanguageExt.Parsec.Token;
using LanguageExt.UnitsOfMeasure;
using LanguageExt.ClassInstances;
using Char = System.Char;

namespace LanguageExt.Tests
{
    public class ParsecTests
    {

        [Fact]
        public void MultiLineNestedComments()
        {
            var jstp = makeTokenParser(Language.JavaStyle.With(NestedComments: true));
            var ws = jstp.WhiteSpace;
            var test3 = parse(ws, @"
/*
*/
");
        }

        [Fact]
        public void MultiLineComments()
        {
            var jstp = makeTokenParser(Language.JavaStyle.With(NestedComments: false));
            var ws = jstp.WhiteSpace;
            var test3 = parse(ws, @"
/*
*/
");
        }

        [Fact]
        public void ResultComb()
        {
            var p = result(1234);
            var r = parse(p, "Hello");

            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == 1234);
        }

        [Fact]
        public void ZeroComb()
        {
            var p = zero<Unit>();
            var r = parse(p, "Hello");

            Assert.True(r.IsFaulted);
        }

        [Fact]
        public void ItemComb()
        {
            var p = anyChar;
            var r = parse(p, "Hello");

            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == 'H');
            Assert.True(r.Reply.State.ToString() == "ello");
        }

        [Fact]
        public void ItemFailComb()
        {
            var p = anyChar;
            var r = parse(p, "");

            Assert.True(r.IsFaulted);
        }

        [Fact]
        public void Item2Comb()
        {
            var p = anyChar;
            var r1 = parse(p, "Hello");

            Assert.False(r1.IsFaulted);
            Assert.True(r1.Reply.Result == 'H');
            Assert.True(r1.Reply.State.ToString() == "ello");

            var r2 = parse(p, r1.Reply.State);

            Assert.False(r2.IsFaulted);
            Assert.True(r2.Reply.Result == 'e');
            Assert.True(r2.Reply.State.ToString() == "llo");

        }

        [Fact]
        public void Item1LinqComb()
        {
            var p = from x in anyChar
                    select x;

            var r = parse(p, "Hello");

            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == 'H');
            Assert.True(r.Reply.State.ToString() == "ello");
        }

        [Fact]
        public void Item2LinqComb()
        {
            var p = from x in anyChar
                    from y in anyChar
                    select Tuple(x, y);

            var r = parse(p, "Hello");

            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result.Item1 == 'H');
            Assert.True(r.Reply.Result.Item2 == 'e');
            Assert.True(r.Reply.State.ToString() == "llo");
        }

        [Fact]
        public void EitherFirstComb()
        {
            var p = either(ch('a'), ch('1'));

            var r = parse(p, "a");

            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == 'a');
            Assert.True(r.Reply.State.ToString() == "");
        }

        [Fact]
        public void EitherSecondComb()
        {
            var p = either(ch('a'), ch('1'));

            var r = parse(p, "1");

            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == '1');
            Assert.True(r.Reply.State.ToString() == "");
        }

        [Fact]
        public void EitherLINQComb()
        {
            var p = from x in either(ch('a'), ch('1'))
                    from y in either(ch('a'), ch('1'))
                    select Tuple(x, y);

            var r = parse(p, "a1");

            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result.Item1 == 'a');
            Assert.True(r.Reply.Result.Item2 == '1');
            Assert.True(r.Reply.State.ToString() == "");
        }

        [Fact]
        public void UpperComb()
        {
            var p = upper;
            var r = parse(p, "Hello");

            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == 'H');
            Assert.True(r.Reply.State.ToString() == "ello");
        }

        [Fact]
        public void UpperFailComb()
        {
            var p = upper;
            var r = parse(p, "hello");

            Assert.True(r.IsFaulted);
        }

        [Fact]
        public void LowerComb()
        {
            var p = lower;
            var r = parse(p, "hello");

            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == 'h');
            Assert.True(r.Reply.State.ToString() == "ello");
        }

        [Fact]
        public void LowerFailComb()
        {
            var p = lower;
            var r = parse(p, "Hello");

            Assert.True(r.IsFaulted);
        }

        [Fact]
        public void DigitComb()
        {
            var p = digit;
            var r = parse(p, "1234");

            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == '1');
            Assert.True(r.Reply.State.ToString() == "234");
        }

        [Fact]
        public void DigitFailComb()
        {
            var p = digit;
            var r = parse(p, "Hello");

            Assert.True(r.IsFaulted);
        }

        [Fact]
        public void LetterComb()
        {
            var p = letter;
            var r = parse(p, "hello");

            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == 'h');
            Assert.True(r.Reply.State.ToString() == "ello");
        }

        [Fact]
        public void LetterFailComb()
        {
            var p = letter;
            var r = parse(p, "1ello");

            Assert.True(r.IsFaulted);
        }

        [Fact]
        public void WordComb()
        {
            var p = asString(many1(letter));
            var r = parse(p, "hello   ");

            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == "hello");
            Assert.True(r.Reply.State.ToString() == "   ");
        }

        [Fact]
        public void WordFailComb()
        {
            var p = asString(many1(letter));
            var r = parse(p, "1ello  ");

            Assert.True(r.IsFaulted);
        }

        [Fact]
        public void StringMatchComb()
        {
            var p = str("hello");
            var r = parse(p, "hello world");

            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == "hello");
            Assert.True(r.Reply.State.ToString() == " world");
        }

        [Fact]
        public void StringMatchFailComb()
        {
            var p = str("hello");
            var r = parse(p, "no match");

            Assert.True(r.IsFaulted);
        }

        [Fact]
        public void StringOrdinalIgnoreCase()
        {
            var p = str<EqCharOrdinalIgnoreCase>("Hello");

            Assert.Equal("hello", parse(p, "hello").ToOption());
            Assert.Equal("Hello", parse(p, "Hello").ToOption());
            Assert.Equal("HELLO" , parse(p, "HELLO").ToOption());
            Assert.Equal("hELLO", parse(p, "hELLO").ToOption());
            Assert.True(parse(p, "olleH").IsFaulted);
            Assert.True(parse(p, "Héllo").IsFaulted);
        }

        [Fact]
        public void NaturalNumberComb()
        {
            var tok = makeTokenParser(Language.HaskellStyle);

            var p = tok.Natural;
            var r = parse(p, "1234  ");

            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == 1234);
            Assert.True(r.Reply.State.ToString() == "");
        }

        [Fact]
        public void NaturalNumberFailComb()
        {
            var tok = makeTokenParser(Language.HaskellStyle);

            var p = tok.Natural;
            var r = parse(p, "no match");

            Assert.True(r.IsFaulted);
        }

        [Fact]
        public void IntegerNumberComb()
        {
            var tok = makeTokenParser(Language.HaskellStyle);

            var p = tok.Integer;
            var r = parse(p, "1234  ");

            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == 1234);
            Assert.True(r.Reply.State.ToString() == "");
        }

        [Fact]
        public void IntegerNegativeNumberComb()
        {
            var tok = makeTokenParser(Language.HaskellStyle);

            var p = tok.Integer;
            var r = parse(p, "-1234  ");

            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == -1234);
            Assert.True(r.Reply.State.ToString() == "");
        }

        [Fact]
        public void IntegerNumberFailComb()
        {
            var tok = makeTokenParser(Language.HaskellStyle);

            var p = tok.Integer;
            var r = parse(p, "no match");

            Assert.True(r.IsFaulted);
        }

        [Fact]
        public void BracketAndIntegerComb()
        {
            var tok = makeTokenParser(Language.HaskellStyle);

            var p = from x in tok.Brackets(tok.Integer)
                    from _ in tok.WhiteSpace
                    select x;

            var r = parse(p, "[1]  ");

            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == 1);
            Assert.True(r.Reply.State.ToString() == "");
        }

        [Fact]
        public void BracketAndIntegerFailComb()
        {
            var tok = makeTokenParser(Language.HaskellStyle);

            var p = tok.Brackets(tok.Integer);
            var r = parse(p, "[x]  ");

            Assert.True(r.IsFaulted);
        }

        [Fact]
        public void BracketAndIntegerListComb()
        {
            var tok = makeTokenParser(Language.HaskellStyle);

            var p = from x in tok.BracketsCommaSep(tok.Integer)
                    from _ in tok.WhiteSpace
                    select x;
            var r = parse(p, "[1,2,3,4]  ");

            Assert.False(r.IsFaulted);

            var arr = r.Reply.Result.ToArray();
            Assert.True(arr[0] == 1);
            Assert.True(arr[1] == 2);
            Assert.True(arr[2] == 3);
            Assert.True(arr[3] == 4);
            Assert.True(r.Reply.State.ToString() == "");
        }

        [Fact]
        public void BracketAndSpacedIntegerListComb()
        {
            var tok = makeTokenParser(Language.HaskellStyle);

            var p = from x in tok.BracketsCommaSep(tok.Integer)
                    from _ in tok.WhiteSpace
                    select x;

            var r = parse(p, "[ 1, 2 ,3,   4]  ");

            Assert.False(r.IsFaulted);

            var arr = r.Reply.Result.ToArray();
            Assert.True(arr[0] == 1);
            Assert.True(arr[1] == 2);
            Assert.True(arr[2] == 3);
            Assert.True(arr[3] == 4);
            Assert.True(r.Reply.State.ToString() == "");
        }

        [Fact]
        public void BracketAndIntegerListFailComb()
        {
            var tok = makeTokenParser(Language.HaskellStyle);

            var p = tok.BracketsCommaSep(tok.Integer);
            var r = parse(p, "[1,x,3,4]  ");

            Assert.True(r.IsFaulted);
        }

        [Fact]
        public void JunkEmptyComb()
        {
            var tok = makeTokenParser(Language.HaskellStyle);

            var p = tok.WhiteSpace;
            var r = parse(p, "");
            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == unit);
        }

        [Fact]
        public void JunkNoMatchComb()
        {
            var tok = makeTokenParser(Language.HaskellStyle);

            var p = tok.WhiteSpace;
            var r = parse(p, ",");
            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == unit);
        }

        [Fact]
        public void JunkFourSpacesComb()
        {
            var tok = makeTokenParser(Language.HaskellStyle);

            var p = tok.WhiteSpace;
            var r = parse(p, "    ,");
            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == unit);
        }

        [Fact]
        public void JunkFourSpacesThenCommentComb()
        {
            var tok = makeTokenParser(Language.JavaStyle);
            var p = tok.WhiteSpace;
            var r = parse(p, "    // A comment\nabc");
            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == unit);
            Assert.True(r.Reply.State.ToString() == "abc");
        }

        [Fact]
        public void StringLiteralComb()
        {
            var tok = makeTokenParser(Language.HaskellStyle);

            var p = tok.StringLiteral;
            var r = parse(p, "\"/abc\"");
            Assert.False(r.IsFaulted);
            Assert.True(r.Reply.Result == "/abc");
        }

        [Theory]
        [InlineData("1234")]
        [InlineData("12345")]
        [InlineData("123456")]
        [InlineData("1234567")]
        [InlineData("12345678")]
        public void ParseNTimes(string input)
        {
            var p = asString(manyn(digit, 4));

            var r = parse(p, input).ToEither();

            Assert.True(r.IfLeft("") == "1234");
        }

        [Theory]
        [InlineData("")]
        [InlineData("1")]
        [InlineData("12")]
        [InlineData("123")]
        public void ParseNTimesFail(string input)
        {
            var p = asString(manyn(digit, 4));

            var r = parse(p, input).ToEither();

            Assert.True(r.IsLeft);
        }

        [Theory]
        [InlineData("1", "1")]
        [InlineData("12", "12")]
        [InlineData("123", "123")]
        [InlineData("1234", "1234")]
        [InlineData("12345", "1234")]
        public void ParseN1Times(string input, string expected)
        {
            var p = asString(manyn1(digit, 4));

            var r = parse(p, input).ToEither();

            Assert.True(r.IfLeft("") == expected);
        }

        [Fact]
        public void ParseN1TimesFail()
        {
            var p = asString(manyn1(digit, 4));

            var r = parse(p, "").ToEither();

            Assert.True(r.IsLeft);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("1", "1")]
        [InlineData("12", "12")]
        [InlineData("123", "123")]
        [InlineData("1234", "1234")]
        [InlineData("12345", "1234")]
        public void ParseN0Times(string input, string expected)
        {
            var p = asString(manyn0(digit, 4));

            var r = parse(p, input).ToEither();

            Assert.True(r.IfLeft("") == expected);
        }
        
        [Fact]
        public void ParseN0TimesZeroNegative()
        {
            Assert.True(parse(asString(manyn0(digit, 0)), "123").ToEither().IfLeft("x") == "");
            Assert.True(parse(asString(manyn0(digit, -1)), "123").ToEither().IfLeft("x") == "");
        }

        [Fact]
        public void SepByTest()
        {
            // greedy, but works because of nice input
            Assert.Equal(Seq("this", "is", "a", "path"), parse(sepBy1(asString(many1(alphaNum)), ch('/')), "this/is/a/path").ToEither());
            Assert.Equal(Seq("this", "is", "a", "path"), parse(sepBy1(asString(many1(alphaNum)), ch('/')), "this/is/a/path.").ToEither());

            // greedy + runs into dead end
            Assert.True(parse(sepBy1(asString(many1(alphaNum)), ch('/')), "this/is/a/path/").IsFaulted);

            // consume as many items as possible without failing
            Assert.Equal(Seq("this", "is", "a", "path"), parse(from x in asString(many1(alphaNum)) from xs in many(attempt(from sep in ch('/') from word in asString(many1(alphaNum)) select word)) select x.Cons(xs), "this/is/a/path/").ToEither());
        }

        [Fact]
        public void EndByTest()
        {
            // greedy, but works because of nice input
            Assert.Equal(Seq("this", "is", "a", "folder"), parse(endBy1(asString(many1(alphaNum)), ch('/')), "this/is/a/folder//").ToEither());
            Assert.Equal(Seq("this", "is", "a", "folder"), parse(endBy1(asString(many1(alphaNum)), ch('/')), "this/is/a/folder/").ToEither());

            // greedy + runs into dead end
            Assert.True(parse(endBy1(attempt(asString(many1(alphaNum))), attempt(ch('/'))), "this/is/a/folder/filename").IsFaulted);

            // consume as many items as possible without failing
            Assert.Equal(Seq("this", "is", "a", "folder"), parse(many1(attempt(from word in asString(many1(alphaNum)) from sep in ch('/') select word)), "this/is/a/folder/filename").ToEither());
        }

        [Fact]
        public void SepEndByTest()
        {
            Assert.Equal(Seq("this", "is", "a", "path"), parse(sepEndBy1(asString(many1(alphaNum)), ch('/')), "this/is/a/path//").ToEither());
            Assert.Equal(Seq("this", "is", "a", "path"), parse(sepEndBy1(asString(many1(alphaNum)), ch('/')), "this/is/a/path/").ToEither());
            Assert.Equal(Seq("this", "is", "a", "path"), parse(sepEndBy1(asString(many1(alphaNum)), ch('/')), "this/is/a/path").ToEither());
            Assert.True(parse(sepEndBy1(asString(many1(alphaNum)), ch('/')), ".").IsFaulted);
            
            Assert.Equal(Seq("this", "is", "a", "path"), parse(sepEndBy(asString(many1(alphaNum)), ch('/')), "this/is/a/path//").ToEither());
            Assert.Equal(Seq("this", "is", "a", "path"), parse(sepEndBy(asString(many1(alphaNum)), ch('/')), "this/is/a/path/").ToEither());
            Assert.Equal(Seq("this", "is", "a", "path"), parse(sepEndBy(asString(many1(alphaNum)), ch('/')), "this/is/a/path").ToEither());
            Assert.Equal(Seq<string>(), parse(sepEndBy(asString(many1(alphaNum)), ch('/')), ".").ToEither());

        }
        
        [Fact]
        public void ParallelCheck()
        {
            // works
            Parallel.ForEach(Enumerable.Repeat("", 4), str => parse(from _ in notFollowedBy(anyChar).label("end of input") select unit, str));
            
            // sometimes crashes (net461)
            Parallel.ForEach(Enumerable.Repeat("", 4), str => parse(from _ in eof select unit, str));
        }

        [Fact]
        public void FlattenCheck()
        {
            Parser<string> HexDigitParser() =>
                from hexDigitString in asString(flatten(sepBy1(many1(hexDigit), ch('-'))))
                from end in eof
                select hexDigitString;

            Assert.Equal("17CBA779DF1E4794A4992AAB59802C19", parse(HexDigitParser(), "17CBA779-DF1E-4794-A499-2AAB59802C19").ToOption());
            Assert.Equal("17CBA779DF1E4794A4992AAB59802C19", parse(HexDigitParser(), "17CBA779DF1E4794A4992AAB59802C19").ToOption());
            Assert.Equal(Option<string>.None, parse(HexDigitParser(), "-17CBA779-DF1E-4794-A499-2AAB59802C19").ToOption());       
            Assert.Equal(Option<string>.None, parse(HexDigitParser(), "17CBA779-DF1E-4794-A499--2AAB59802C19").ToOption());
        }
        
        [Fact]
        public void ConsCheck()
        {
            Parser<string> HexDigitBlocksParser() =>
                from guidString in asString(flatten(cons(many1(hexDigit), many(cons(ch('-'), many1(hexDigit))))))
                from end in eof
                select guidString;

            Assert.Equal("17CBA779-DF1E-4794-A499-2AAB59802C19", parse(HexDigitBlocksParser(), "17CBA779-DF1E-4794-A499-2AAB59802C19").ToOption());
            Assert.Equal("17CBA779DF1E4794A4992AAB59802C19", parse(HexDigitBlocksParser(), "17CBA779DF1E4794A4992AAB59802C19").ToOption());
            Assert.Equal(Option<string>.None, parse(HexDigitBlocksParser(), "-17CBA779-DF1E-4794-A499-2AAB59802C19").ToOption());       
            Assert.Equal(Option<string>.None, parse(HexDigitBlocksParser(), "17CBA779-DF1E-4794-A499--2AAB59802C19").ToOption());
        }
        
        [Fact]
        public void EMailParserCheck()
        {
            // Note: This e-mail parser is not correct! See https://tools.ietf.org/html/rfc5322#section-3.4.1 and related RFCs
            Parser<string> QuickAndDirtyEMailParser() =>
                from localpart in asString(flatten(cons(many1(alphaNum), many(cons(oneOf('-','.'), many1(alphaNum)))))) // simple name
                from at in ch('@')
                from domain in asString(flatten(cons(many1(alphaNum), many1(cons(ch('.'), many1(alphaNum)))))) // domain with at least one dot
             select $"{localpart}{at}{domain.ToLower()}";

            Assert.Equal("john@example.org", parse(QuickAndDirtyEMailParser(), "john@EXAMPLE.org").ToOption());
            Assert.EndsWith("expecting letter or digit or '@'", parse(QuickAndDirtyEMailParser(), "john @EXAMPLE.org").ToEither().IfRight(""));
            Assert.EndsWith("expecting letter or digit", parse(QuickAndDirtyEMailParser(), ".john @EXAMPLE.org").ToEither().IfRight(""));
            Assert.EndsWith("expecting letter or digit or '.'", parse(QuickAndDirtyEMailParser(), "john.doe@EXAMPLE").ToEither().IfRight(""));
            Assert.Equal("john-doe@example.org", parse(QuickAndDirtyEMailParser(), "john-doe@EXAMPLE.org").ToOption());
            Assert.Equal("john.doe@example.org", parse(QuickAndDirtyEMailParser(), "john.doe@EXAMPLE.org").ToOption());
        }

        [Fact]
        public void Issue889()
        {
            // Code adapted from the AccountingDSL example, keeping only the basic arithmetic operators
            var opChars = "+-*/";

            var definition = GenLanguageDef.Empty.With(
                CommentStart: "/*",
                CommentEnd: "*/",
                CommentLine: "//",
                NestedComments: true,
                OpStart: oneOf(opChars),
                OpLetter: oneOf(opChars),
                IdentStart: letter,
                IdentLetter: either(alphaNum, ch('_')),
                ReservedNames: Empty,
                ReservedOpNames: List("+", "-", "*", "/")
            );

            var lexer = makeTokenParser(definition);
            var input = "3.14159";
            var result = lexer.NaturalOrFloat.Parse(input).ToEither().Map(x => x.IfLeft(Double.MaxValue));
                // > Success(Right(1418.9))

            var expected = lexer.Float.Parse(input).ToEither().IfLeft(Double.MinValue);
                // > Success(3.14159)

            Assert.True(result == expected);
        }

        [Fact]
        public void Nat_or_float_must_be_int()
        {
            var opChars = "+-*/";

            var definition = GenLanguageDef.Empty.With(
                CommentStart: "/*",
                CommentEnd: "*/",
                CommentLine: "//",
                NestedComments: true,
                OpStart: oneOf(opChars),
                OpLetter: oneOf(opChars),
                IdentStart: letter,
                IdentLetter: either(alphaNum, ch('_')),
                ReservedNames: Empty,
                ReservedOpNames: List("+", "-", "*", "/")
            );

            var lexer  = makeTokenParser(definition);
            var input  = "300";
            var result = lexer.NaturalOrFloat.Parse(input).ToEither().Map(x => x.IfRight(0));
            Assert.True(result == 300);
        }

        [Fact]
        public void ExpressionResultShouldBeSixDueToMultiplicationOperationPriority()
        {
            // Arrange
            var tokenParser = makeTokenParser(Language.JavaStyle);
            var reservedOp = tokenParser.ReservedOp;

            // Natural parser
            var natural = from n in tokenParser.Natural
                          select Expr.Natural(n);

            // Binary operator expression factory
            Func<Expr, Expr, Expr> binaryOp(string op) =>
                (Expr lhs, Expr rhs) =>
                    op == "+" ? Expr.Add(lhs, rhs)
                  : op == "-" ? Expr.Sub(lhs, rhs)
                  : op == "/" ? Expr.Div(lhs, rhs)
                  : op == "*" ? Expr.Mul(lhs, rhs)
                  : throw new NotSupportedException();

            // Binary operator parser builder
            Operator<Expr> binary(string op, Assoc assoc) =>
                Operator.Infix(assoc,
                    from x in reservedOp(op)
                    select binaryOp(op));

            // Operator table
            Operator<Expr>[][] table =
            {
                new[] { binary("+", Assoc.Left), binary("-", Assoc.Left) },
                new[] { binary("*", Assoc.Left), binary("/", Assoc.Left) }
            };

            // Null because it will be not null later and can be used by the lazyp parser
            Parser<Expr> expr = null; 

            // Build up the expression term
            var term = either(
                            attempt(natural), 
                            tokenParser.Parens(lazyp(() => expr)));

            // Build the expression parser
            expr = buildExpressionParser(table, term).label("expression");


            var expression = "2 + 2 * 2";
            var exprectedResult = 6;

            // Act
            var actualResult = parse(expr, expression)
                                  .ToOption()
                                  .Map(ex => ex.Eval())
                                  .IfNone(0);

            // Assert
            Assert.Equal(exprectedResult, actualResult);
        }

        [Fact]
        public void EofTest()
        {
            Assert.Equal(unit, parse(eof, "").ToEither());
            Assert.Equal("error at (line 1, column 1): unexpected 'x', expecting end of input", parse(eof, "x").ToEither());
        }

        [Fact]
        public void NotFollowedByEofTest()
        {
            Assert.Equal("error at (line 1, column 1): unexpected end of input", parse(notFollowedBy(eof), "").ToEither());
        }

        [Fact]
        public void NotFollowedByPositionTest()
        {
            Assert.Equal("error at (line 1, column 1): unexpected 'x'", parse(notFollowedBy(ch('x')), "x").ToEither());
            Assert.Equal("error at (line 1, column 2): unexpected 'y'", parse(from x in ch('x') from noY in notFollowedBy(ch('y')) select x, "xyz").ToEither());
            Assert.Equal("error at (line 1, column 2): unexpected \"Text\"", parse(from x in ch('x') from noText in notFollowedBy(str("Text")) select x, "xText").ToEither());
        }

        public abstract class Expr
        {
            public abstract int Eval();

            public static Expr Natural(int x) => new NaturalExpr(x);
            public static Expr Add(Expr left, Expr right) => new AddExpr(left, right);
            public static Expr Sub(Expr left, Expr right) => new SubExpr(left, right);
            public static Expr Mul(Expr left, Expr right) => new MulExpr(left, right);
            public static Expr Div(Expr left, Expr right) => new DivExpr(left, right);

            public class NaturalExpr : Expr
            {
                public int Value;
                public NaturalExpr(int value) => Value = value;
                public override int Eval() => Value;
            }

            public class AddExpr : Expr
            {
                public readonly Expr Left;
                public readonly Expr Right;
                public AddExpr(Expr left, Expr right)
                {
                    Left = left;
                    Right = right;
                }
                public override int Eval() =>
                    Left.Eval() + Right.Eval();
            }
            public class SubExpr : Expr
            {
                public readonly Expr Left;
                public readonly Expr Right;
                public SubExpr(Expr left, Expr right)
                {
                    Left = left;
                    Right = right;
                }
                public override int Eval() =>
                    Left.Eval() - Right.Eval();
            }
            public class MulExpr : Expr
            {
                public readonly Expr Left;
                public readonly Expr Right;
                public MulExpr(Expr left, Expr right)
                {
                    Left = left;
                    Right = right;
                }
                public override int Eval() =>
                    Left.Eval() * Right.Eval();
            }
            public class DivExpr : Expr
            {
                public readonly Expr Left;
                public readonly Expr Right;
                public DivExpr(Expr left, Expr right)
                {
                    Left = left;
                    Right = right;
                }
                public override int Eval() =>
                    Left.Eval() / Right.Eval();
            }
        }
    }
}

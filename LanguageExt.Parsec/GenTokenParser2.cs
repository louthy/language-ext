using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static LanguageExt.Prelude;
using static LanguageExt.Parsec.ParserResult;
using static LanguageExt.Parsec.Internal;
using static LanguageExt.Parsec.Prim;
using static LanguageExt.Parsec.Char;

namespace LanguageExt.Parsec
{
    public class GenTokenParser2
    {
        /// <summary>
        /// This lexeme parser parses a legal identifier. Returns the identifier
        /// string. This parser will fail on identifiers that are reserved
        /// words. Legal identifier (start) characters and reserved words are
        /// defined in the 'LanguageDef' that is passed to
        /// 'makeTokenParser'. An identifier is treated as
        /// a single token using 'try'.
        /// </summary>
        public readonly Parser<(string Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> Identifier;

        /// <summary>
        /// The lexeme parser reserved(name) parses a symbol 
        /// name, but it also checks that the name is not a prefix of a
        /// valid identifier. A reserved word is treated as a single token
        /// using 'try'. 
        /// </summary>
        public readonly Func<string, Parser<(string Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)>> Reserved;

        /// <summary>
        /// This lexeme parser parses a legal operator. Returns the name of the
        /// operator. This parser will fail on any operators that are reserved
        /// operators. Legal operator (start) characters and reserved operators
        /// are defined in the 'LanguageDef' that is passed to
        /// 'makeTokenParser'. An operator is treated as a
        /// single token using 'try'. 
        /// </summary>
        public readonly Parser<(string Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> Operator;

        /// <summary>
        /// The lexeme parser reservedOp name parses symbol
        /// name, but it also checks that the name is not a prefix of a
        /// valid operator. A reservedOp is treated as a single token using
        /// 'try'. 
        /// </summary>
        public readonly Func<string, Parser<(Unit Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)>> ReservedOp;

        /// <summary>
        /// This lexeme parser parses a single literal character. Returns the
        /// literal character value. This parsers deals correctly with escape
        /// sequences. The literal character is parsed according to the grammar
        /// rules defined in the Haskell report (which matches most programming
        /// languages quite closely). 
        /// </summary>
        public readonly Parser<(char Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> CharLiteral;

        /// <summary>
        /// This lexeme parser parses a literal string. Returns the literal
        /// string value. This parsers deals correctly with escape sequences and
        /// gaps. The literal string is parsed according to the grammar rules
        /// defined in the Haskell report (which matches most programming
        /// languages quite closely). 
        /// </summary>
        public readonly Parser<(string Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> StringLiteral;

        /// <summary>
        /// This lexeme parser parses a natural number (a positive whole
        /// number). Returns the value of the number. The number can be
        /// specified in 'decimal', 'hexadecimal' or
        /// 'octal'. The number is parsed according to the grammar
        /// rules in the Haskell report. 
        /// </summary>
        public readonly Parser<(int Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> Natural;

        /// <summary>
        /// This lexeme parser parses an integer (a whole number). This parser
        /// is like 'natural' except that it can be prefixed with
        /// sign (i.e. \'-\' or \'+\'). Returns the value of the number. The
        /// number can be specified in 'decimal', 'hexadecimal'
        /// or 'octal'. The number is parsed according
        /// to the grammar rules in the Haskell report. 
        /// </summary>
        public readonly Parser<(int Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> Integer;

        /// <summary>
        /// This lexeme parser parses a floating point value. Returns the value
        /// of the number. The number is parsed according to the grammar rules
        /// defined in the Haskell report. 
        /// </summary>
        public readonly Parser<(double Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> Float;

        /// <summary>
        /// This lexeme parser parses either 'natural' or a 'float'.
        /// Returns the value of the number. This parsers deals with
        /// any overlap in the grammar rules for naturals and floats. The number
        /// is parsed according to the grammar rules defined in the Haskell report. 
        /// </summary>
        public readonly Parser<(Either<int, double> Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> NaturalOrFloat;

        /// <summary>
        /// Parses a positive whole number in the decimal system. Returns the
        /// value of the number. 
        /// </summary>
        public readonly Parser<(int Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> Decimal;

        /// <summary>
        /// Parses a positive whole number in the hexadecimal system. The number
        /// should be prefixed with "0x" or "0X". Returns the value of the
        /// number.
        /// </summary>
        public readonly Parser<(int Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> Hexadecimal;

        /// <summary>
        /// Parses a positive whole number in the octal system. The number
        /// should be prefixed with "0o" or "0O". Returns the value of the
        /// number. 
        /// </summary>
        public readonly Parser<(int Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> Octal;

        /// <summary>
        /// Lexeme parser symbol(s) parses 'string' s and skips
        /// trailing white space. 
        /// </summary>
        public readonly Func<string, Parser<(string Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)>> Symbol;

        /// <summary>
        /// lexeme(p) first applies parser p and than the 'whiteSpace'
        /// parser, returning the value of p. Every lexical
        /// token (lexeme) is defined using lexeme, this way every parse
        /// starts at a point without white space. Parsers that use lexeme are
        /// called lexeme parsers in this document.
        /// 
        /// The only point where the 'whiteSpace' parser should be
        /// called explicitly is the start of the main parser in order to skip
        /// any leading white space.
        /// </summary>
        public Parser<(A Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> Lexeme<A>(Parser<A> p) =>
            Token2.lexemeDef<A>(WhiteSpace)(p);

        /// <summary>
        /// Parses any white space. White space consists of /zero/ or more
        /// occurrences of a 'space', a line comment or a block (multi
        /// line) comment. Block comments may be nested. How comments are
        /// started and ended is defined in the 'LanguageDef'
        /// that is passed to 'makeTokenParser'. 
        /// </summary>
        public readonly Parser<Unit> WhiteSpace;

        /// <summary>
        /// Lexeme parser parens(p) parses p enclosed in parenthesis,
        /// returning the value of p.
        /// </summary>
        public Parser<(A Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> Parens<A>(Parser<(A Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> p) =>
            from o in Symbol("(")
            from x in p
            from c in Symbol(")")
            select x;

        /// <summary>
        /// Lexeme parser braces(p) parses p enclosed in braces { and
        /// }, returning the value of p. 
        /// </summary>
        public Parser<(A Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> Braces<A>(Parser<(A Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> p) =>
            from o in Symbol("{")
            from x in p
            from c in Symbol("}")
            select x;

        /// <summary>
        /// Lexeme parser angles(p) parses p enclosed in angle brackets <
        /// and >, returning the value of p. 
        /// </summary>
        public Parser<(A Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> Angles<A>(Parser<(A Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> p) =>
            from o in Symbol("<")
            from x in p
            from c in Symbol(">")
            select x;

        /// <summary>
        /// Lexeme parser brackets(p) parses p enclosed in brackets [
        /// and ], returning the value of p. 
        /// </summary>
        public Parser<(A Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> Brackets<A>(Parser<(A Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> p) =>
            from o in Symbol("[")
            from x in p
            from c in Symbol("]")
            select x;

        /// <summary>
        /// Lexeme parser semi parses the character ; and skips any
        /// trailing white space. Returns the string ";". 
        /// </summary>
        public readonly Parser<(string Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> Semi;

        /// <summary>
        /// Lexeme parser comma parses the character , and skips any
        /// trailing white space. Returns the string ",". 
        /// </summary>
        public readonly Parser<(string Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> Comma;

        /// <summary>
        /// Lexeme parser colon parses the character : and skips any
        /// trailing white space. Returns the string ":". 
        /// </summary>
        public readonly Parser<(string Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> Colon;

        /// <summary>
        /// Lexeme parser dot parses the character . and skips any
        /// trailing white space. Returns the string ".". 
        /// </summary>
        public readonly Parser<(string Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> Dot;

        /// <summary>
        /// Lexeme parser semiSep(p) parses /zero/ or more occurrences of p
        /// separated by semi. Returns a list of values returned by
        /// p.
        /// </summary>
        public Parser<(Seq<A> Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> SepBy<A, S>(Parser<(A Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> p, Parser<S> sep) =>
            from bp in getPos
            from bi in getIndex
            from xs in sepBy(p, sep)
            select xs.IsEmpty
                       ? (Seq<A>(), bp, bp, bi, bi)
                       : (xs.Map(static x => x.Value), xs.Head.BeginPos, xs.Last.EndPos, xs.Head.BeginIndex, xs.Head.EndIndex);

        /// <summary>
        /// Lexeme parser semiSep1(p) parses /one/ or more occurrences of p
        /// separated by 'semi'. Returns a list of values returned by p. 
        /// </summary>
        public Parser<(Seq<A> Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> SepBy1<A, S>(Parser<(A Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> p, Parser<S> sep) =>
            sepBy1(p, sep)
               .Map(static xs => (xs.Map(static x => x.Value), xs.Head.BeginPos, xs.Last.EndPos, xs.Head.BeginIndex, xs.Head.EndIndex));        

        /// <summary>
        /// Lexeme parser semiSep(p) parses /zero/ or more occurrences of p
        /// separated by semi. Returns a list of values returned by
        /// p.
        /// </summary>
        public Parser<(Seq<A> Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> SemiSep<A>(Parser<(A Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> p) =>
            SepBy(p, Semi);

        /// <summary>
        /// Lexeme parser semiSep1(p) parses /one/ or more occurrences of p
        /// separated by 'semi'. Returns a list of values returned by p. 
        /// </summary>
        public Parser<(Seq<A> Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> SemiSep1<A>(Parser<(A Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> p) =>
            SepBy1(p, Semi);

        /// <summary>
        /// Lexeme parser commaSep(p) parses /zero/ or more occurrences of
        /// p separated by 'comma'. Returns a list of values returned
        /// by p. 
        /// </summary>
        public Parser<(Seq<A> Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> CommaSep<A>(Parser<(A Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> p) =>
            SepBy(p, Comma);
        
        /// <summary>
        /// Lexeme parser commaSep1(p) parses /one/ or more occurrences of
        /// p separated by 'comma'. Returns a list of values returned
        /// by p. 
        /// </summary>
        public Parser<(Seq<A> Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> CommaSep1<A>(Parser<(A Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> p) =>
            SepBy1(p, Comma);

        public Parser<(Seq<A> Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> BracketsCommaSep1<A>(Parser<(A Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> p) =>
            Brackets(SepBy1(p, Comma));

        public Parser<(Seq<A> Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> BracketsCommaSep<A>(Parser<(A Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> p) =>
            Brackets(SepBy(p, Comma));

        public Parser<(Seq<A> Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> ParensCommaSep1<A>(Parser<(A Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> p) =>
            Parens(SepBy1(p, Comma));

        public Parser<(Seq<A> Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> ParensCommaSep<A>(Parser<(A Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> p) =>
            Parens(SepBy(p, Comma));

        public Parser<(Seq<A> Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> AnglesCommaSep1<A>(Parser<(A Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> p) =>
            Angles(SepBy1(p, Comma));

        public Parser<(Seq<A> Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> AnglesCommaSep<A>(Parser<(A Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> p) =>
            Angles(SepBy(p, Comma));

        public Parser<(Seq<A> Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> BracesCommaSep1<A>(Parser<(A Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> p) =>
            Braces(SepBy1(p, Comma));

        public Parser<(Seq<A> Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> BracesCommaSep<A>(Parser<(A Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> p) =>
            Braces(SepBy(p, Comma));


        public Parser<(Seq<A> Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> BracketsSemiSep1<A>(Parser<(A Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> p) =>
            Brackets(SepBy1(p, Semi));

        public Parser<(Seq<A> Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> BracketsSemiSep<A>(Parser<(A Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> p) =>
            Brackets(SepBy(p, Semi));

        public Parser<(Seq<A> Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> ParensSemiSep1<A>(Parser<(A Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> p) =>
            Parens(SepBy1(p, Semi));

        public Parser<(Seq<A> Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> ParensSemiSep<A>(Parser<(A Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> p) =>
            Parens(SepBy(p, Semi));

        public Parser<(Seq<A> Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> AnglesSemiSep1<A>(Parser<(A Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> p) =>
            Angles(SepBy1(p, Semi));

        public Parser<(Seq<A> Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> AnglesSemiSep<A>(Parser<(A Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> p) =>
            Angles(SepBy(p, Semi));

        public Parser<(Seq<A> Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> BracesSemiSep1<A>(Parser<(A Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> p) =>
            Braces(SepBy1(p, Semi));

        public Parser<(Seq<A> Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> BracesSemiSep<A>(Parser<(A Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> p) =>
            Braces(SepBy(p, Semi));

        internal GenTokenParser2(
            Parser<(string Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> indentifier,
            Func<string, Parser<(string Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)>> reserved,
            Parser<(string Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> op,
            Func<string, Parser<(Unit Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)>> reservedOp,
            Parser<(char Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> charLiteral,
            Parser<(string Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> stringLiteral,
            Parser<(int Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> natural,
            Parser<(int Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> integer,
            Parser<(double Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> floating,
            Parser<(Either<int, double> Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> naturalOrFloat,
            Parser<(int Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> dec,
            Parser<(int Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> hexadecimal,
            Parser<(int Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> octal,
            Func<string, Parser<(string Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)>> symbol,
            Parser<Unit> whiteSpace,
            Parser<(string Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> semi,
            Parser<(string Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> comma,
            Parser<(string Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> colon,
            Parser<(string Value, Pos BeginPos, Pos EndPos, int BeginIndex, int EndIndex)> dot
            )
        {
            Identifier = indentifier;
            this.Reserved = reserved;
            this.Operator = op;
            this.ReservedOp = reservedOp;
            this.CharLiteral = charLiteral;
            this.StringLiteral = stringLiteral;
            this.Natural = natural;
            this.Integer = integer;
            this.Float = floating;
            this.NaturalOrFloat = naturalOrFloat;
            this.Decimal = dec;
            Hexadecimal = hexadecimal;
            Octal = octal;
            this.Symbol = symbol;
            this.WhiteSpace = whiteSpace;
            this.Semi = semi;
            this.Comma = comma;
            this.Colon = colon;
            this.Dot = dot;
        }
    }
}

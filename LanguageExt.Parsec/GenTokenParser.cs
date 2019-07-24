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
    public class GenTokenParser
    {
        /// <summary>
        /// This lexeme parser parses a legal identifier. Returns the identifier
        /// string. This parser will fail on identifiers that are reserved
        /// words. Legal identifier (start) characters and reserved words are
        /// defined in the 'LanguageDef' that is passed to
        /// 'makeTokenParser'. An identifier is treated as
        /// a single token using 'try'.
        /// </summary>
        public readonly Parser<string> Identifier;

        /// <summary>
        /// The lexeme parser reserved(name) parses a symbol 
        /// name, but it also checks that the name is not a prefix of a
        /// valid identifier. A reserved word is treated as a single token
        /// using 'try'. 
        /// </summary>
        public readonly Func<string, Parser<string>> Reserved;

        /// <summary>
        /// This lexeme parser parses a legal operator. Returns the name of the
        /// operator. This parser will fail on any operators that are reserved
        /// operators. Legal operator (start) characters and reserved operators
        /// are defined in the 'LanguageDef' that is passed to
        /// 'makeTokenParser'. An operator is treated as a
        /// single token using 'try'. 
        /// </summary>
        public readonly Parser<string> Operator;

        /// <summary>
        /// The lexeme parser reservedOp name parses symbol
        /// name, but it also checks that the name is not a prefix of a
        /// valid operator. A reservedOp is treated as a single token using
        /// 'try'. 
        /// </summary>
        public readonly Func<string, Parser<Unit>> ReservedOp;

        /// <summary>
        /// This lexeme parser parses a single literal character. Returns the
        /// literal character value. This parsers deals correctly with escape
        /// sequences. The literal character is parsed according to the grammar
        /// rules defined in the Haskell report (which matches most programming
        /// languages quite closely). 
        /// </summary>
        public readonly Parser<char> CharLiteral;

        /// <summary>
        /// This lexeme parser parses a literal string. Returns the literal
        /// string value. This parsers deals correctly with escape sequences and
        /// gaps. The literal string is parsed according to the grammar rules
        /// defined in the Haskell report (which matches most programming
        /// languages quite closely). 
        /// </summary>
        public readonly Parser<string> StringLiteral;

        /// <summary>
        /// This lexeme parser parses a natural number (a positive whole
        /// number). Returns the value of the number. The number can be
        /// specified in 'decimal', 'hexadecimal' or
        /// 'octal'. The number is parsed according to the grammar
        /// rules in the Haskell report. 
        /// </summary>
        public readonly Parser<int> Natural;

        /// <summary>
        /// This lexeme parser parses an integer (a whole number). This parser
        /// is like 'natural' except that it can be prefixed with
        /// sign (i.e. \'-\' or \'+\'). Returns the value of the number. The
        /// number can be specified in 'decimal', 'hexadecimal'
        /// or 'octal'. The number is parsed according
        /// to the grammar rules in the Haskell report. 
        /// </summary>
        public readonly Parser<int> Integer;

        /// <summary>
        /// This lexeme parser parses a floating point value. Returns the value
        /// of the number. The number is parsed according to the grammar rules
        /// defined in the Haskell report. 
        /// </summary>
        public readonly Parser<double> Float;

        /// <summary>
        /// This lexeme parser parses either 'natural' or a 'float'.
        /// Returns the value of the number. This parsers deals with
        /// any overlap in the grammar rules for naturals and floats. The number
        /// is parsed according to the grammar rules defined in the Haskell report. 
        /// </summary>
        public readonly Parser<Either<int,double>> NaturalOrFloat;

        /// <summary>
        /// Parses a positive whole number in the decimal system. Returns the
        /// value of the number. 
        /// </summary>
        public readonly Parser<int> Decimal;

        /// <summary>
        /// Parses a positive whole number in the hexadecimal system. The number
        /// should be prefixed with "0x" or "0X". Returns the value of the
        /// number.
        /// </summary>
        public readonly Parser<int> Hexadecimal;

        /// <summary>
        /// Parses a positive whole number in the octal system. The number
        /// should be prefixed with "0o" or "0O". Returns the value of the
        /// number. 
        /// </summary>
        public readonly Parser<int> Octal;

        /// <summary>
        /// Lexeme parser symbol(s) parses 'string' s and skips
        /// trailing white space. 
        /// </summary>
        public readonly Func<string, Parser<string>> Symbol;

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
        public Parser<T> Lexeme<T>(Parser<T> p) =>
            from x in p
            from _ in WhiteSpace
            select x;

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
        public Parser<T> Parens<T>(Parser<T> p) =>
            from o in Symbol("(")
            from x in p
            from c in Symbol(")")
            select x;

        /// <summary>
        /// Lexeme parser braces(p) parses p enclosed in braces { and
        /// }, returning the value of p. 
        /// </summary>
        public Parser<T> Braces<T>(Parser<T> p) =>
            from o in Symbol("{")
            from x in p
            from c in Symbol("}")
            select x;

        /// <summary>
        /// Lexeme parser angles(p) parses p enclosed in angle brackets <
        /// and >, returning the value of p. 
        /// </summary>
        public Parser<T> Angles<T>(Parser<T> p) =>
            from o in Symbol("<")
            from x in p
            from c in Symbol(">")
            select x;

        /// <summary>
        /// Lexeme parser brackets(p) parses p enclosed in brackets [
        /// and ], returning the value of p. 
        /// </summary>
        public Parser<T> Brackets<T>(Parser<T> p) =>
            from o in Symbol("[")
            from x in p
            from c in Symbol("]")
            select x;

        /// <summary>
        /// Lexeme parser semi parses the character ; and skips any
        /// trailing white space. Returns the string ";". 
        /// </summary>
        public readonly Parser<string> Semi;

        /// <summary>
        /// Lexeme parser comma parses the character , and skips any
        /// trailing white space. Returns the string ",". 
        /// </summary>
        public readonly Parser<string> Comma;

        /// <summary>
        /// Lexeme parser colon parses the character : and skips any
        /// trailing white space. Returns the string ":". 
        /// </summary>
        public readonly Parser<string> Colon;

        /// <summary>
        /// Lexeme parser dot parses the character . and skips any
        /// trailing white space. Returns the string ".". 
        /// </summary>
        public readonly Parser<string> Dot;

        /// <summary>
        /// Lexeme parser semiSep(p) parses /zero/ or more occurrences of p
        /// separated by semi. Returns a list of values returned by
        /// p.
        /// </summary>
        public Parser<Seq<T>> SemiSep<T>(Parser<T> p) =>
            sepBy(p, Semi);

        /// <summary>
        /// Lexeme parser semiSep1(p) parses /one/ or more occurrences of p
        /// separated by 'semi'. Returns a list of values returned by p. 
        /// </summary>
        public Parser<Seq<T>> SemiSep1<T>(Parser<T> p) =>
            sepBy1(p, Semi);

        /// <summary>
        /// Lexeme parser commaSep(p) parses /zero/ or more occurrences of
        /// p separated by 'comma'. Returns a list of values returned
        /// by p. 
        /// </summary>
        public Parser<Seq<T>> CommaSep<T>(Parser<T> p) =>
            sepBy(p, Comma);

        /// <summary>
        /// Lexeme parser commaSep1(p) parses /one/ or more occurrences of
        /// p separated by 'comma'. Returns a list of values returned
        /// by p. 
        /// </summary>
        public Parser<Seq<T>> CommaSep1<T>(Parser<T> p) =>
            sepBy1(p, Comma);

        public Parser<Seq<T>> BracketsCommaSep1<T>(Parser<T> p) =>
            Brackets(sepBy1(p, Comma));

        public Parser<Seq<T>> BracketsCommaSep<T>(Parser<T> p) =>
            Brackets(sepBy(p, Comma));

        public Parser<Seq<T>> ParensCommaSep1<T>(Parser<T> p) =>
            Parens(sepBy1(p, Comma));

        public Parser<Seq<T>> ParensCommaSep<T>(Parser<T> p) =>
            Parens(sepBy(p, Comma));

        public Parser<Seq<T>> AnglesCommaSep1<T>(Parser<T> p) =>
            Angles(sepBy1(p, Comma));

        public Parser<Seq<T>> AnglesCommaSep<T>(Parser<T> p) =>
            Angles(sepBy(p, Comma));

        public Parser<Seq<T>> BracesCommaSep1<T>(Parser<T> p) =>
            Braces(sepBy1(p, Comma));

        public Parser<Seq<T>> BracesCommaSep<T>(Parser<T> p) =>
            Braces(sepBy(p, Comma));


        public Parser<Seq<T>> BracketsSemiSep1<T>(Parser<T> p) =>
            Brackets(sepBy1(p, Semi));

        public Parser<Seq<T>> BracketsSemiSep<T>(Parser<T> p) =>
            Brackets(sepBy(p, Semi));

        public Parser<Seq<T>> ParensSemiSep1<T>(Parser<T> p) =>
            Parens(sepBy1(p, Semi));

        public Parser<Seq<T>> ParensSemiSep<T>(Parser<T> p) =>
            Parens(sepBy(p, Semi));

        public Parser<Seq<T>> AnglesSemiSep1<T>(Parser<T> p) =>
            Angles(sepBy1(p, Semi));

        public Parser<Seq<T>> AnglesSemiSep<T>(Parser<T> p) =>
            Angles(sepBy(p, Semi));

        public Parser<Seq<T>> BracesSemiSep1<T>(Parser<T> p) =>
            Braces(sepBy1(p, Semi));

        public Parser<Seq<T>> BracesSemiSep<T>(Parser<T> p) =>
            Braces(sepBy(p, Semi));

        internal GenTokenParser(
            Parser<string> indentifier,
            Func<string, Parser<string>> reserved,
            Parser<string> op,
            Func<string, Parser<Unit>> reservedOp,
            Parser<char> charLiteral,
            Parser<string> stringLiteral,
            Parser<int> natural,
            Parser<int> integer,
            Parser<double> floating,
            Parser<Either<int, double>> naturalOrFloat,
            Parser<int> dec,
            Parser<int> hexadecimal,
            Parser<int> octal,
            Func<string, Parser<string>> symbol,
            Parser<Unit> whiteSpace,
            Parser<string> semi,
            Parser<string> comma,
            Parser<string> colon,
            Parser<string> dot
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

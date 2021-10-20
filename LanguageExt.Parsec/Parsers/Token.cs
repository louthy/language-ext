using System;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.Parsec.Prim;
using static LanguageExt.Parsec.Char;

namespace LanguageExt.Parsec
{
    /// <summary>
    /// A helper module to parse lexical elements (tokens)
    /// </summary>
    public static class Token
    {
        /// <summary>
        ///  Given a LanguageDef, create a token parser.
        /// 
        ///  The expression makeTokenParser(language) creates a 'GenTokenParser'
        ///  record that contains lexical parsers that are defined using the 
        ///  definitions in the language record.
        /// 
        ///  The use of this function is quite stylised - one imports the
        ///  appropriate language definition and selects the lexical parsers that
        ///  are needed from the resulting 'GenTokenParser'.
        /// 
        ///    // The parser
        ///    ...
        ///  
        ///    var expr  =  either(
        ///                     parens(expr),
        ///                     identifier,
        ///                     ...
        ///                 )
        ///  
        ///    // The lexer
        ///    var lexer       = makeTokenParser(langDef)
        ///        
        ///    var parens      = lexer.Parens(p);
        ///    var braces      = lexer.Braces(p);
        ///    var identifier  = lexer.Identifier;
        ///    var reserved    = lexer.Reserved;
        ///    ...        
        /// </summary>
        public static GenTokenParser makeTokenParser(GenLanguageDef def)
        {
            var simpleSpace = skipMany1(satisfy(System.Char.IsWhiteSpace));

            Parser<Unit> multiLineComment = null;
            Parser<Unit> inCommentMulti = null;
            Parser<Unit> inCommentSingle = null;
            Func<string, bool> isReservedName = null;

            var startEnd = List.append(def.CommentEnd.ToArray(), def.CommentStart.ToArray()).Distinct().ToArray();

            inCommentMulti =
                choice(
                    from x in attempt(str(def.CommentEnd))
                    select unit,
                    from x in lazyp(() => multiLineComment)
                    from y in inCommentMulti
                    select unit,
                    from x in skipMany1(noneOf(startEnd))
                    from y in inCommentMulti
                    select unit,
                    from x in oneOf(startEnd)
                    from y in inCommentMulti
                    select unit)
                   .label("end of comment");

            inCommentSingle =
                choice(
                    from x in attempt(str(def.CommentEnd))
                    select unit,
                    from x in skipMany1(noneOf(startEnd))
                    from y in inCommentSingle
                    select unit,
                    from x in oneOf(startEnd)
                    from y in inCommentSingle
                    select unit)
                   .label("end of comment");


            var inComment = def.NestedComments
                ? inCommentMulti
                : inCommentSingle;

            multiLineComment =
                from x in attempt(str(def.CommentStart))
                from _ in inComment
                select unit;

            var oneLineComment =
                from x in attempt(str(def.CommentLine))
                from _ in skipMany(satisfy(c => c != '\n'))
                select unit;

            var whiteSpace =
                def.CommentLine == null && def.CommentStart == null ? skipMany(simpleSpace.label(""))
              : def.CommentStart == null ? skipMany(either(simpleSpace, multiLineComment).label(""))
              : def.CommentLine == null ? skipMany(either(simpleSpace, oneLineComment).label(""))
              : skipMany(choice(simpleSpace, oneLineComment, multiLineComment).label(""));

            var lexemeStr      = lexemeDef<string>(whiteSpace);
            var lexemeCh       = lexemeDef<char>(whiteSpace);
            var lexemeInt      = lexemeDef<int>(whiteSpace);
            var lexemeDbl      = lexemeDef<double>(whiteSpace);
            var lexemeUnit     = lexemeDef<Unit>(whiteSpace);
            var lexemeFnIntInt = lexemeDef<Func<int,int>>(whiteSpace);
            var lexemeEiIntDbl = lexemeDef<Either<int,double>>(whiteSpace);

            var symbol = fun((string name) => lexemeStr(str(name)));

            var semi = symbol(";");
            var comma = symbol(",");
            var dot = symbol(".");
            var colon = symbol(":");

            var dec = from x in many1(digit)
                      let v = parseInt(new string(x.ToArray()))
                      from n in v.Match(
                          Some: result,
                          None: () => failure<int>("Not a valid decimal value"))
                      select n;

            var octal = (from _ in ch('o')
                         from x in many1(octDigit)
                         let v = parseInt(new string(x.ToArray()), 8)
                         from n in v.Match(
                             Some: result,
                             None: () => failure<int>("Not a valid octal value"))
                         select n)
                        .label("octal number");

            var hexadecimal = (from _ in ch('x')
                               from x in many1(hexDigit)
                               let v = parseInt(new string(x.ToArray()), 16)
                               from n in v.Match(
                                   Some: result,
                                   None: () => failure<int>("Not a valid hexadecimal value"))
                               select n)
                              .label("hexadecimal number");

            var zeroNumber = (from _ in ch('0')
                              from r in choice(hexadecimal, octal, dec, result(0))
                              select r)
                             .label("");

            var nat = either(zeroNumber, dec);

            var sign = choice(from _ in ch('-')
                              select fun((double d) => -d),
                              from _ in ch('+')
                              select fun((double d) => d),
                              result(fun((double d) => d)));

            var signi = choice(from _ in ch('-')
                               select fun((int d) => -d),
                               from _ in ch('+')
                               select fun((int d) => d),
                               result(fun((int d) => d)));

            var int_ = from f in lexemeFnIntInt(signi)
                       from n in nat
                       select f(n);

            var charEsc = choice(escMap.Map(pair => parseEsc(pair.Item1, pair.Item2)));

            var charNum = choice(dec, hexadecimal, octal)
                            .Map(System.Char.ConvertFromUtf32)
                            .Map(x => x[0]);

            var charControl = from _ in ch('^')
                              from c in upper
                              select (char)(c - 64);

            var escapeCode = choice(charEsc, charNum, charControl).label("escape code");

            var charEscape = from _ in ch('\\')
                             from c in escapeCode
                             select c;

            var charLetter = satisfy(c => c != '\'' && c != '\\' && c > 26);

            var characterChar = either(charLetter, charEscape).label("literal character");

            var charLiteral =
                lexemeCh(
                    between(
                        ch('\''),
                        ch('\'').label("end of character"),
                        characterChar))
               .label("character");

            var stringLetter = satisfy(c => c != '"' && c != '\\' && c > 26);

            var escapeGap = (from _ in many1(space)
                             from c in ch('\\')
                             select c)
                            .label("end of string gap");

            var escapeEmpty = ch('&');

            var stringEscape = from _ in ch('\\')
                               from esc in choice(
                                    from x in escapeGap
                                    select Option<char>.None,
                                    from x in escapeEmpty
                                    select Option<char>.None,
                                    from c in escapeCode
                                    select Some(c))
                               select esc;


            var stringChar =
                 either(from c in stringLetter
                        select Some(c),
                        stringEscape)
                .label("string character");


            var stringLiteral =
                lexemeStr(
                    from s in between(
                        ch('"'),
                        ch('"'),
                        many(stringChar)
                    )
                    select new string(s.Somes().ToArray()))
               .label("literal string");

            // -1.05e+003 - optional fractional part
            var floating = from si in optionOrElse("", oneOf("+-").Select(static x => x.ToString()))
                           from nu in asString(many(digit))
                           from frac in optionOrElse("",
                               from pt in dot
                               from fr in asString(many(digit))
                               from ex in optionOrElse("",
                                   from e in oneOf("eE")
                                   from s in oneOf("+-")
                                   from n in asString(many1(digit))
                                   select $"{e}{s}{n}"
                                   )
                               select $"{pt}{fr}{ex}")
                           let all = $"{si}{nu}{frac}"
                           let opt = parseDouble(all)
                           from res in opt.Match(
                               result,
                               () => failure<double>("Invalid floating point value")
                           )
                           select res;

            // -1.05e+003 - must have fractional part
            var fracfloat = from si in optionOrElse("", from x in oneOf("+-") select x.ToString())
                            from nu in asString(many(digit))
                            from frac in from pt in dot
                                         from fr in asString(many(digit))
                                         from ex in optionOrElse("",
                                                                 from e in oneOf("eE")
                                                                 from s in oneOf("+-")
                                                                 from n in asString(many1(digit))
                                                                 select $"{e}{s}{n}"
                                         )
                                         select $"{pt}{fr}{ex}"
                            let all = $"{si}{nu}{frac}"
                            let opt = parseDouble(all)
                            from res in opt.Match(
                                result,
                                () => failure<double>("Invalid floating point value")
                            )
                            select res;
            
            var natural        = lexemeInt(nat).label("natural");
            var integer        = lexemeInt(int_).label("integer");
            var float_         = lexemeDbl(floating).label("float");
            var naturalOrFloat = either(attempt(lexemeDbl(fracfloat)).Map(Right<int, double>), 
                                        integer.Map(Left<int, double>)).label("number");

            var reservedOp = fun((string name) =>
                lexemeUnit(
                    attempt(
                        from n in str(name)
                        from _ in notFollowedBy(def.OpLetter).label("end of " + name)
                        select unit)));

            var oper = (from c in def.OpStart
                        from cs in many(def.OpLetter)
                        select new string(c.Cons(cs).ToArray()))
                       .label("operator");

            var operator_ =
                lexemeStr(
                    attempt(
                        from name in oper
                        from op in def.ReservedOpNames.Contains(name)
                            ? unexpected<string>("reserved operator " + name)
                            : result(name)
                        select op));

            var theReservedNames =
                def.CaseSensitive
                    ? def.ReservedNames
                    : def.ReservedNames.Map(x => x.ToLower()).Freeze();

            var isReserved = fun((Lst<string> names, string name) =>
                names.Contains(
                    def.CaseSensitive
                        ? name
                        : name.ToLower()));

            isReservedName = fun((string name) =>
                isReserved(theReservedNames, name));

            var ident = (from c in def.IdentStart
                         from cs in many(def.IdentLetter)
                         select new string(c.Cons(cs).ToArray()))
                        .label("identifier");

            var identifier = lexemeStr(
                attempt(
                    from name in ident
                    from r in isReservedName(name)
                        ? unexpected<string>("reserved word " + name)
                        : result(name)
                    select r));

            var caseString = fun((string name) => str(name));
            //def.CaseSensitive
            //    ? str(name)
            //    : /* TODO - fully implement case insensitive version*/);

            var reserved = fun((string name) =>
                lexemeStr(
                    attempt(
                        from x in caseString(name)
                        from _ in notFollowedBy(def.IdentLetter).label("end of " + name)
                        select x)));

            return new GenTokenParser(
                identifier,
                reserved,
                operator_,
                reservedOp,
                charLiteral,
                stringLiteral,
                natural,
                integer,
                float_,
                naturalOrFloat,
                lexemeInt(dec),
                lexemeInt(hexadecimal),
                lexemeInt(octal),
                symbol,
                whiteSpace,
                semi,
                comma,
                colon,
                dot
                );
        }

        static Func<Parser<T>, Parser<T>> lexemeDef<T>(Parser<Unit> whiteSpace)
        {
            var ws = whiteSpace;
            return (Parser<T> p) =>
                from x in p
                from _ in ws
                select x;
        }

        static Parser<T> parseEsc<T>(char c, T code) =>
            from _ in ch(c)
            select code;

        static readonly Seq<(char, char)> escMap =
            toSeq(List.zip("abfnrtv\\\"\'", "\a\b\f\n\r\t\v\\\"\'").ToArray());
    }
}

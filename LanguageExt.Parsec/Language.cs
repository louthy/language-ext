using System;
using System.Collections.Generic;
using System.Linq;
using static LanguageExt.Prelude;
using static LanguageExt.Parsec.Prim;
using static LanguageExt.Parsec.Char;
using static LanguageExt.Parsec.Expr;
using static LanguageExt.Parsec.Token;

namespace LanguageExt.Parsec
{
    public static class Language
    {
        /// <summary>
        /// This is a minimal token definition for Haskell style languages. It
        /// defines the style of comments, valid identifiers and case
        /// sensitivity.  It does not define any reserved words or operators.
        /// </summary>
        public readonly static GenLanguageDef HaskellStyle =
            GenLanguageDef.Empty.With(
                CommentStart: "{-",
                CommentEnd: "-}",
                CommentLine: "--",
                NestedComments: true,
                IdentStart: letter,
                IdentLetter: either(alphaNum, oneOf("_'")),
                OpStart: oneOf(":!#$%&*+./<=>?@\\^|-~"),
                OpLetter: oneOf(":!#$%&*+./<=>?@\\^|-~"),
                ReservedOpNames: List<string>(),
                ReservedNames: List<string>(),
                CaseSensitive: true
                );

        /// <summary>
        /// This is a minimal token definition for Java style languages. It
        /// defines the style of comments, valid identifiers and case
        /// sensitivity.  It does not define any reserved words.
        /// </summary>
        public readonly static GenLanguageDef JavaStyle =
            GenLanguageDef.Empty.With(
                CommentStart: "/*",
                CommentEnd: "*/",
                CommentLine: "//",
                NestedComments: true,
                IdentStart: letter,
                IdentLetter: either(alphaNum, oneOf("_'")),
                OpStart: oneOf(@"!%&*+.<=>?@\^|-~"),
                OpLetter: oneOf(@"!%&*+.<=>?@\^|-~"),
                ReservedOpNames: List<string>(),
                ReservedNames: List<string>(),
                CaseSensitive: true
                );

        /// <summary>
        /// The language definition for the language Haskell98.
        /// </summary>
        public readonly static GenLanguageDef Haskell98Def =
            HaskellStyle.With(
                ReservedOpNames: List.create("::", "..", "=", "\\", "|", "<-", "->", "@", "~", "=>"),
                ReservedNames: List.create(
                    "let", "in", "case", "of", "if", "then", "else",
                    "data", "type",
                    "class", "default", "deriving", "do", "import",
                    "infix", "infixl", "infixr", "instance", "module",
                    "newtype", "where",
                    "primitive"));

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static LanguageExt.Parsec.PrimT;
using static LanguageExt.Parsec.CharT;

namespace LanguageExt.Parsec
{
    /// <summary>
    /// The GenLanguageDef type is a record that contains all parameteridable
    /// features of the "Parsec.Text.Token" module.  The module "Parsec.Text.Language"
    /// contains some default definitions.
    /// </summary>
    public class GenLanguageDefIO
    {
        /// <summary>
        /// Describes the start of a block comment. Use the empty string if the
        /// language doesn't support block comments. For example "/*". 
        /// </summary>
        public readonly string CommentStart;

        /// <summary>
        /// Describes the end of a block comment. Use the empty string if the
        /// language doesn't support block comments. For example "*\". 
        /// </summary>
        public readonly string CommentEnd;

        /// <summary>
        /// Describes the start of a line comment. Use the empty string if the
        /// language doesn't support line comments. For example "//". 
        /// </summary>
        public readonly string CommentLine;

        /// <summary>
        /// Set to 'True' if the language supports nested block comments. 
        /// </summary>
        public readonly bool NestedComments;

        /// <summary>
        /// This parser should accept any start characters of identifiers. For
        /// example either(letter,char('_')). 
        /// </summary>
        public readonly Parser<char, char> IdentStart;

        /// <summary>
        /// This parser should accept any legal tail characters of identifiers.
        /// For example either(alphaNum, char('_')). 
        /// </summary>
        public readonly Parser<char, char> IdentLetter;

        /// <summary>
        /// This parser should accept any start characters of operators. For
        /// example oneOf(":!#$%&*+.\/\<=>?\@\\\\^|-~")
        /// </summary>
        public readonly Parser<char, char> OpStart;

        /// <summary>
        /// This parser should accept any legal tail characters of operators.
        /// Note that this parser should even be defined if the language doesn't
        /// support user-defined operators, or otherwise the 'reservedOp'
        /// parser won't work correctly. 
        /// </summary>
        public readonly Parser<char, char> OpLetter;

        /// <summary>
        /// The list of reserved identifiers. 
        /// </summary>
        public readonly Lst<string> ReservedNames;

        /// <summary>
        /// The list of reserved operators.
        /// </summary>
        public readonly Lst<string> ReservedOpNames;

        /// <summary>
        /// Set to 'True' if the language is case sensitive. 
        /// </summary>
        public readonly bool CaseSensitive;

        /// <summary>
        /// Empty definition, use With to build
        /// </summary>
        public static readonly GenLanguageDefIO Empty =
            new GenLanguageDefIO("", "", "", true, zero<char, char>(), zero<char, char>(), zero<char, char>(), zero<char, char>(), List.empty<string>(), List.empty<string>(), true);

        private GenLanguageDefIO(
            string commentStart,
            string commentEnd,
            string commentLine,
            bool nestedComments,
            Parser<char, char> identStart,
            Parser<char, char> identLetter,
            Parser<char, char> opStart,
            Parser<char, char> opLetter,
            Lst<string> reservedNames,
            Lst<string> reservedOpNames,
            bool caseSensitive
            )
        {
            CommentStart = commentStart;
            CommentEnd = commentEnd;
            CommentLine = commentLine;
            NestedComments = nestedComments;
            IdentStart = identStart;
            IdentLetter = identLetter;
            OpStart = opStart;
            OpLetter = opLetter;
            ReservedNames = reservedNames.OrderBy(x => x).Freeze();
            ReservedOpNames = reservedOpNames.OrderBy(x=> x).Freeze();
            CaseSensitive = caseSensitive;
        }

        public GenLanguageDefIO With(
            string CommentStart = null,
            string CommentEnd = null,
            string CommentLine = null,
            bool? NestedComments = null,
            Parser<char, char> IdentStart = null,
            Parser<char, char> IdentLetter = null,
            Parser<char, char> OpStart = null,
            Parser<char, char> OpLetter = null,
            Lst<string> ReservedNames = null,
            Lst<string> ReservedOpNames = null,
            bool? CaseSensitive = null
            ) =>
            new GenLanguageDefIO(
                CommentStart == null ? this.CommentStart : CommentStart,
                CommentEnd == null ? this.CommentEnd : CommentEnd,
                CommentLine == null ? this.CommentLine : CommentLine,
                NestedComments == null ? this.NestedComments : NestedComments.Value,
                IdentStart == null ? this.IdentStart : IdentStart,
                IdentLetter == null ? this.IdentLetter : IdentLetter,
                OpStart == null ? this.OpStart : OpStart,
                OpLetter == null ? this.OpLetter : OpLetter,
                ReservedNames == null ? this.ReservedNames : ReservedNames,
                ReservedOpNames == null ? this.ReservedOpNames : ReservedOpNames,
                CaseSensitive == null ? this.CaseSensitive : CaseSensitive.Value
            );
    }
}

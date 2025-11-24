using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.Megaparsec.Parsers;

public static class Text<E, S, M>
    where S : TokenStream<S, char>
    where M : MonadParsec<E, S, char, M> 
{
    /// <summary>
    /// Parse a string
    /// </summary>
    public static K<M, S> @string(string xs) =>
        Prim<E, S, char, M>.chunk(S.TokensToChunk(xs.AsSpan()));
    
    /// <summary>
    /// Parse a newline character.
    /// </summary>
    public static readonly K<M, char> newline =
        M.Token(static ch => ch == '\n' ? Some('\n') : None, ExpectedErrors.newline);
    
    /// <summary>
    /// Parse a newline character.
    /// </summary>
    public static readonly K<M, char> tab =
        M.Token(static ch => ch == '\t' ? Some('\t') : None, ExpectedErrors.tab);

    /// <summary>
    /// Parse a newline character.
    /// </summary>
    public static readonly K<M, S> crlf =
        Prim<E, S, char, M>.chunk(S.TokensToChunk(['\r', '\n']));
    
    /// <summary>
    /// End of line parser.
    /// </summary>
    /// <remarks>
    /// Tries a newline parser first, then tries a carriage-return followed by a newline parser.
    /// </remarks>
    public static readonly K<M, S> eol =
        charToStream * newline 
            | crlf
            | label("end of line");

    /// <summary>
    /// Parse an uppercase letter
    /// </summary>
    public static readonly K<M, char> upperChar =
        M.Token(static ch => char.IsUpper(ch) ? Some(ch) : None, ExpectedErrors.upperChar);

    /// <summary>
    /// Parse a lowercase letter
    /// </summary>
    public static readonly K<M, char> lowerChar =
        M.Token(static ch => char.IsLower(ch) ? Some(ch) : None, ExpectedErrors.lowerChar);

    /// <summary>
    /// Parse a letter
    /// </summary>
    public static readonly K<M, char> letterChar =
        M.Token(static ch => char.IsLetter(ch) ? Some(ch) : None, ExpectedErrors.letterChar);

    /// <summary>
    /// Parse an alphanumeric character (letter or digit)
    /// </summary>
    public static readonly K<M, char> aplhaNumChar =
        M.Token(static ch => char.IsLetterOrDigit(ch) ? Some(ch) : None, ExpectedErrors.alphaNumChar);

    /// <summary>
    /// Parse a digit
    /// </summary>
    public static readonly K<M, char> digitChar =
        M.Token(static ch => char.IsDigit(ch) ? Some(ch) : None, ExpectedErrors.digitChar);

    /// <summary>
    /// Parse a binary digit
    /// </summary>
    public static readonly K<M, char> binaryDigitChar =
        M.Token(static ch => ch is '0' or '1' ? Some(ch) : None, ExpectedErrors.binaryDigitChar);

    /// <summary>
    /// Parse a hexadecimal digit
    /// </summary>
    public static readonly K<M, char> hexDigitChar =
        M.Token(static ch => char.IsAsciiHexDigit(ch) ? Some(ch) : None, ExpectedErrors.hexDigitChar);

    /// <summary>
    /// Parse a number character
    /// </summary>
    public static readonly K<M, char> numberChar =
        M.Token(static ch => char.IsNumber(ch) ? Some(ch) : None, ExpectedErrors.numberChar);

    /// <summary>
    /// Parse a symbol character
    /// </summary>
    public static readonly K<M, char> symbolChar =
        M.Token(static ch => char.IsSymbol(ch) ? Some(ch) : None, ExpectedErrors.symbolChar);

    /// <summary>
    /// Parse a punctuation character
    /// </summary>
    public static readonly K<M, char> punctuationChar =
        M.Token(static ch => char.IsPunctuation(ch) ? Some(ch) : None, ExpectedErrors.punctuationChar);

    /// <summary>
    /// Parse a white-space character.
    /// </summary>
    public static readonly K<M, char> spaceChar =
        M.Token(static ch => char.IsWhiteSpace(ch) ? Some(ch) : None, ExpectedErrors.whiteSpaceChar);

    /// <summary>
    /// Parse zero or more white-space characters.
    /// </summary>
    public static readonly K<M, Unit> space =
        (_ => unit) * M.TakeWhile(ExpectedErrors.whiteSpace, char.IsWhiteSpace);

    /// <summary>
    /// Parse one or more white-space characters.
    /// </summary>
    public static readonly K<M, Unit> space1 =
        (_ => unit) * M.TakeWhile1(ExpectedErrors.whiteSpace, char.IsWhiteSpace);

    /// <summary>
    /// Parse zero or more white-space characters (ignoring newlines and carriage-returns).
    /// </summary>
    public static readonly K<M, Unit> hspace =
        (_ => unit) * M.TakeWhile(ExpectedErrors.whiteSpace, 
                                  static ch => ch != '\n' && ch != '\r' && char.IsWhiteSpace(ch));

    /// <summary>
    /// Parse zero or more white-space characters (ignoring newlines and carriage-returns).
    /// </summary>
    public static readonly K<M, Unit> hspace1 =
        (_ => unit) * M.TakeWhile1(ExpectedErrors.whiteSpace, 
                                   static ch => ch != '\n' && ch != '\r' && char.IsWhiteSpace(ch));

    /// <summary>
    /// Parse a control character
    /// </summary>
    public static readonly K<M, char> controlChar =
        M.Token(static ch => char.IsControl(ch) ? Some(ch) : None, ExpectedErrors.control);
    
    /// <summary>
    /// Make a chunk `S` from a single token `char`
    /// </summary>
    public static S charToStream(char ch) =>
        S.TokenToChunk(ch);
    
    public static K<M, char> Test =>
        Prim<E, S, char, M>.oneOf(['1', '2', '3']) | label("hello");
}

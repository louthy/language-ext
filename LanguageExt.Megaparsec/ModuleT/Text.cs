using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.Megaparsec;

public static partial class ModuleT<MP, E, S, M>
    where MP : MonadParsecT<MP, E, S, char, M>
    where S : TokenStream<S, char>
    where M : Monad<M>
{
    /// <summary>
    /// Parse a string
    /// </summary>
    public static K<MP, S> @string(string xs) =>
        ModuleT<MP, E, S, char, M>.chunk(S.TokensToChunk(xs.AsSpan()));

    /// <summary>
    /// Parse the specified character
    /// </summary>
    /// <param name="c">Character to parse</param>
    /// <returns>Parser</returns>
    public static K<MP, char> ch(char c) =>
        MP.Token(ch => ch == c ? Some(c) : None, Set.singleton(ErrorItem.Label<char>($"{c}")));
    
    /// <summary>
    /// Parse a newline character.
    /// </summary>
    public static readonly K<MP, char> newline =
        MP.Token(static ch => ch == '\n' ? Some('\n') : None, ExpectedErrors.newline);
    
    /// <summary>
    /// Parse a newline character.
    /// </summary>
    public static readonly K<MP, char> tab =
        MP.Token(static ch => ch == '\t' ? Some('\t') : None, ExpectedErrors.tab);

    /// <summary>
    /// Parse a newline character.
    /// </summary>
    public static readonly K<MP, S> crlf =
        ModuleT<MP, E, S, char, M>.chunk(S.TokensToChunk(['\r', '\n']));
    
    /// <summary>
    /// End of line parser.
    /// </summary>
    /// <remarks>
    /// Tries a newline parser first, then tries a carriage-return followed by a newline parser.
    /// </remarks>
    public static readonly K<MP, S> eol =
        charToStream * newline 
            | crlf
            | Prelude.label("end of line");

    /// <summary>
    /// Parse an uppercase letter
    /// </summary>
    public static readonly K<MP, char> upperChar =
        MP.Token(static ch => char.IsUpper(ch) ? Some(ch) : None, ExpectedErrors.upperChar);

    /// <summary>
    /// Parse a lowercase letter
    /// </summary>
    public static readonly K<MP, char> lowerChar =
        MP.Token(static ch => char.IsLower(ch) ? Some(ch) : None, ExpectedErrors.lowerChar);

    /// <summary>
    /// Parse a letter
    /// </summary>
    public static readonly K<MP, char> letterChar =
        MP.Token(static ch => char.IsLetter(ch) ? Some(ch) : None, ExpectedErrors.letterChar);

    /// <summary>
    /// Parse an alphanumeric character (letter or digit)
    /// </summary>
    public static readonly K<MP, char> aplhaNumChar =
        MP.Token(static ch => char.IsLetterOrDigit(ch) ? Some(ch) : None, ExpectedErrors.alphaNumChar);

    /// <summary>
    /// Parse a digit
    /// </summary>
    public static readonly K<MP, char> digitChar =
        MP.Token(static ch => char.IsDigit(ch) ? Some(ch) : None, ExpectedErrors.digitChar);

    /// <summary>
    /// Parse a binary digit
    /// </summary>
    public static readonly K<MP, char> binaryDigitChar =
        MP.Token(static ch => ch is '0' or '1' ? Some(ch) : None, ExpectedErrors.binaryDigitChar);

    /// <summary>
    /// Parse a hexadecimal digit
    /// </summary>
    public static readonly K<MP, char> hexDigitChar =
        MP.Token(static ch => char.IsAsciiHexDigit(ch) ? Some(ch) : None, ExpectedErrors.hexDigitChar);

    /// <summary>
    /// Parse a number character
    /// </summary>
    public static readonly K<MP, char> numberChar =
        MP.Token(static ch => char.IsNumber(ch) ? Some(ch) : None, ExpectedErrors.numberChar);

    /// <summary>
    /// Parse a symbol character
    /// </summary>
    public static readonly K<MP, char> symbolChar =
        MP.Token(static ch => char.IsSymbol(ch) ? Some(ch) : None, ExpectedErrors.symbolChar);

    /// <summary>
    /// Parse a punctuation character
    /// </summary>
    public static readonly K<MP, char> punctuationChar =
        MP.Token(static ch => char.IsPunctuation(ch) ? Some(ch) : None, ExpectedErrors.punctuationChar);

    /// <summary>
    /// Parse a white-space character.
    /// </summary>
    public static readonly K<MP, char> spaceChar =
        MP.Token(static ch => char.IsWhiteSpace(ch) ? Some(ch) : None, ExpectedErrors.whiteSpaceChar);

    /// <summary>
    /// Parse zero or more white-space characters.
    /// </summary>
    public static readonly K<MP, Unit> space =
        (static _ => unit) * MP.TakeWhile(char.IsWhiteSpace, ExpectedErrors.whiteSpace);

    /// <summary>
    /// Parse one or more white-space characters.
    /// </summary>
    public static readonly K<MP, Unit> space1 =
        (static _ => unit) * MP.TakeWhile1(char.IsWhiteSpace, ExpectedErrors.whiteSpace);

    /// <summary>
    /// Parse zero or more white-space characters (ignoring newlines and carriage-returns).
    /// </summary>
    public static readonly K<MP, Unit> hspace =
        (static _ => unit) * MP.TakeWhile(static ch => ch != '\n' && ch != '\r' && char.IsWhiteSpace(ch),
                                          ExpectedErrors.whiteSpace);

    /// <summary>
    /// Parse zero or more white-space characters (ignoring newlines and carriage-returns).
    /// </summary>
    public static readonly K<MP, Unit> hspace1 =
        (static _ => unit) * MP.TakeWhile1(static ch => ch != '\n' && ch != '\r' && char.IsWhiteSpace(ch),
                                           ExpectedErrors.whiteSpace);

    /// <summary>
    /// Parse a control character
    /// </summary>
    public static readonly K<MP, char> controlChar =
        MP.Token(static ch => char.IsControl(ch) ? Some(ch) : None, ExpectedErrors.control);

    /// <summary>
    /// Parse a separator character
    /// </summary>
    public static readonly K<MP, char> separatorChar =
        MP.Token(static ch => char.IsSeparator(ch) ? Some(ch) : None, ExpectedErrors.separator);
    
    /// <summary>
    /// Make a chunk `S` from a single token `char`
    /// </summary>
    public static S charToStream(char ch) =>
        S.TokenToChunk(ch);
    
    static K<MP, char> Test =>
        ModuleT<MP, E, S, char, M>.oneOf(['1', '2', '3']) | label("hello");
}

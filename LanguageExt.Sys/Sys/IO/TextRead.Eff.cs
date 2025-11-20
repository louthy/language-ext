using System.IO;
using LanguageExt.Pipes;
using LanguageExt.Traits;
using LanguageExt.Sys.Traits;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt.Sys.IO;

public static class TextRead<RT>
    where RT : 
        Has<Eff<RT>, TextReadIO>
{
    /// <summary>
    /// Open a text file and streams the lines through the pipe
    /// </summary>
    [Pure]
    public static Pipe<RT, TextReader, string, Unit> readLine =>
        TextRead<Eff<RT>, RT>.readLine;
        
    /// <summary>
    /// Open a text file and streams the chars through the pipe
    /// </summary>
    [Pure]
    public static Pipe<RT, TextReader, char, Unit> readChar =>
        TextRead<Eff<RT>, RT>.readChar;
        
    /// <summary>
    /// Read the rest of the text in the stream
    /// </summary>
    [Pure]
    public static Pipe<RT, TextReader, string, Unit> readToEnd =>
        TextRead<Eff<RT>, RT>.readToEnd;

    /// <summary>
    /// Repeatedly read a number of chars from the stream
    /// </summary>
    [Pure]
    public static Pipe<RT, TextReader, SeqLoan<char>, Unit> readChars(int charCount) =>
        TextRead<Eff<RT>, RT>.readChars(charCount).As();

    /// <summary>
    /// Read a number of chars from the stream
    /// </summary>
    [Pure]
    public static Pipe<RT, TextReader, string, Unit> read(int charCount) =>
        TextRead<Eff<RT>, RT>.read(charCount).As();

    /// <summary>
    /// Close the reader
    /// </summary>
    [Pure, MethodImpl(EffOpt.mops)]
    public static Eff<RT, Unit> close(TextReader reader) =>
        TextRead<Eff<RT>, RT>.close(reader).As();
}

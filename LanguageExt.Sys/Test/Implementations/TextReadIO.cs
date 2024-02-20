using System;
using System.IO;

namespace LanguageExt.Sys.Test.Implementations;

public struct TextReadIO : Sys.Traits.TextReadIO
{
    public static Sys.Traits.TextReadIO Default =
        new TextReadIO();
 
    /// <summary>
    /// Read a line of text from the stream
    /// </summary>
    public IO<Option<string>> ReadLine(TextReader reader) =>
        Live.TextReadIO.Default.ReadLine(reader);

    /// <summary>
    /// Read the rest of the text in the stream
    /// </summary>
    public IO<string> ReadToEnd(TextReader reader) =>
        Live.TextReadIO.Default.ReadToEnd(reader);

    /// <summary>
    /// Read chars from the stream into the buffer
    /// Returns the number of chars read
    /// </summary>
    public IO<int> Read(TextReader reader, Memory<char> buffer) =>
        Live.TextReadIO.Default.Read(reader, buffer);

    /// <summary>
    /// Close the reader
    /// </summary>
    public IO<Unit> Close(TextReader reader) =>
        Live.TextReadIO.Default.Close(reader);
}

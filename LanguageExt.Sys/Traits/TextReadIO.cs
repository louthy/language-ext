using System;
using System.IO;

namespace LanguageExt.Sys.Traits;

public interface TextReadIO
{
    /// <summary>
    /// Read a line of text from the stream
    /// </summary>
    IO<Option<string>> ReadLine(TextReader reader);
        
    /// <summary>
    /// Read the rest of the text in the stream
    /// </summary>
    IO<string> ReadToEnd(TextReader reader);        
        
    /// <summary>
    /// Read chars from the stream into the buffer
    /// Returns the number of chars read
    /// </summary>
    IO<int> Read(TextReader reader, Memory<char> buffer);        
        
    /// <summary>
    /// Close the reader
    /// </summary>
    IO<Unit> Close(TextReader reader);        
}

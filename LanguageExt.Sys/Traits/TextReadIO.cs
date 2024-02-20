using System;
using System.IO;
using LanguageExt.Attributes;
using LanguageExt.Effects.Traits;

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
    
/// <summary>
/// Type-class giving a struct the trait of supporting TextReader IO
/// </summary>
/// <typeparam name="RT">Runtime</typeparam>
[Trait("*")]
public interface HasTextRead<RT> : HasIO<RT>
    where RT : HasTextRead<RT>
{
    /// <summary>
    /// Access the TextReader synchronous effect environment
    /// </summary>
    /// <returns>TextReader synchronous effect environment</returns>
    IO<TextReadIO> TextReadIO { get; }
}

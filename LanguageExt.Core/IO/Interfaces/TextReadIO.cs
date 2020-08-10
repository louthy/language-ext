using System;
using System.IO;
using System.Threading.Tasks;
using LanguageExt.Attributes;

namespace LanguageExt.Interfaces
{
    public interface TextReadIO
    {
        /// <summary>
        /// Read a line of text from the stream
        /// </summary>
        ValueTask<Option<string>> ReadLine(TextReader reader);
        
        /// <summary>
        /// Read the rest of the text in the stream
        /// </summary>
        ValueTask<string> ReadToEnd(TextReader reader);        
        
        /// <summary>
        /// Read chars from the stream into the buffer
        /// Returns the number of chars read
        /// </summary>
        ValueTask<int> Read(TextReader reader, Memory<char> buffer);        
        
        /// <summary>
        /// Close the reader
        /// </summary>
        Unit Close(TextReader reader);        
    }
    
    /// <summary>
    /// Type-class giving a struct the trait of supporting TextReader IO
    /// </summary>
    /// <typeparam name="RT">Runtime</typeparam>
    [Typeclass("*")]
    public interface HasTextRead<RT> : HasCancel<RT>
        where RT : struct, HasCancel<RT>
    {
        /// <summary>
        /// Access the TextReader IO environment
        /// </summary>
        /// <returns>TextReader IO environment</returns>
        IO<RT, TextReadIO> TextReadIO { get; }

        /// <summary>
        /// Access the TextReader SIO environment
        /// </summary>
        /// <returns>TextReader SIO environment</returns>
        SIO<RT, TextReadIO> TextReadSIO { get; }
    }
}

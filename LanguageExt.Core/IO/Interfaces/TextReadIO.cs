using System;
using System.IO;
using System.Threading.Tasks;

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
}

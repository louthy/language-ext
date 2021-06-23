using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace LanguageExt.Sys.Test
{
    public struct TextReadIO : Sys.Traits.TextReadIO
    {
        public static Sys.Traits.TextReadIO Default =
            new TextReadIO();
 
        /// <summary>
        /// Read a line of text from the stream
        /// </summary>
        public ValueTask<Option<string>> ReadLine(TextReader reader) =>
            Live.TextReadIO.Default.ReadLine(reader);

        /// <summary>
        /// Read the rest of the text in the stream
        /// </summary>
        public ValueTask<string> ReadToEnd(TextReader reader) =>
            Live.TextReadIO.Default.ReadToEnd(reader);

        /// <summary>
        /// Read chars from the stream into the buffer
        /// Returns the number of chars read
        /// </summary>
        public ValueTask<int> Read(TextReader reader, Memory<char> buffer) =>
            Live.TextReadIO.Default.Read(reader, buffer);

        /// <summary>
        /// Close the reader
        /// </summary>
        public Unit Close(TextReader reader) =>
            Live.TextReadIO.Default.Close(reader);
    }
}

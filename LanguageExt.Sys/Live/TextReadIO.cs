using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace LanguageExt.Sys.Live
{
    public readonly struct TextReadIO : Sys.Traits.TextReadIO
    {
        public readonly static Sys.Traits.TextReadIO Default =
            new TextReadIO();
 
        /// <summary>
        /// Read a line of text from the stream
        /// </summary>
        public async ValueTask<Option<string>> ReadLine(TextReader reader)
        {
            var str = await reader.ReadLineAsync().ConfigureAwait(false);
            return str == null
                ? None
                : Some(str);
        }

        /// <summary>
        /// Read the rest of the text in the stream
        /// </summary>
        public async ValueTask<string> ReadToEnd(TextReader reader) =>
            await reader.ReadToEndAsync().ConfigureAwait(false);        

#if NET5PLUS
        /// <summary>
        /// Read chars from the stream into the buffer
        /// Returns the number of chars read
        /// </summary>
        public async ValueTask<int> Read(TextReader reader, Memory<char> buffer) =>
            await reader.ReadAsync(buffer).ConfigureAwait(false);
#else
        /// <summary>
        /// Read chars from the stream into the buffer
        /// Returns the number of chars read
        /// </summary>
        public async ValueTask<int> Read(TextReader reader, Memory<char> buffer) =>
            MemoryMarshal.TryGetArray(buffer, out ArraySegment<char> nbuffer)
                ? await reader.ReadAsync(nbuffer.Array, nbuffer.Offset, nbuffer.Count).ConfigureAwait(false)
                : throw new InvalidOperationException();
#endif

        /// <summary>
        /// Close the reader
        /// </summary>
        public Unit Close(TextReader reader)
        {
            reader?.Close();
            reader?.Dispose();
            return default;
        }
    }
}

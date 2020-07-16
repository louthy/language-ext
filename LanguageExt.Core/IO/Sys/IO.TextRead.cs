using System;
using System.IO;

namespace LanguageExt
{
    /// <summary>
    /// IO prelude
    /// </summary>
    public static partial class IO
    {
        public static class TextRead
        {
            /// <summary>
            /// Read a line of text from the stream
            /// </summary>
            public static IO<Runtime, Option<string>> readLine(TextReader reader) =>
                Runtime.env.MapAsync(e => e.TextRead.ReadLine(reader));

            /// <summary>
            /// Read the rest of the text in the stream
            /// </summary>
            public static IO<Runtime, string> readToEnd(TextReader reader) =>
                Runtime.env.MapAsync(e => e.TextRead.ReadToEnd(reader));

            /// <summary>
            /// Read chars from the stream into the buffer
            /// Returns the number of chars read
            /// </summary>
            public static IO<Runtime, int> read(TextReader reader, Memory<char> buffer) =>
                Runtime.env.MapAsync(e => e.TextRead.Read(reader, buffer));

            /// <summary>
            /// Close the reader
            /// </summary>
            public static SIO<Runtime, Unit> close(TextReader reader) =>
                Runtime.senv.Map(e => e.TextRead.Close(reader));
        }
    }
}

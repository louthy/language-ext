using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime.CompilerServices;
using LanguageExt.Interfaces;

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
            [Pure, MethodImpl(IO.mops)]
            public static IO<RT, Option<string>> readLine<RT>(TextReader reader) 
                where RT : struct, HasTextRead<RT> =>
                default(RT).TextReadIO.MapAsync(e => e.ReadLine(reader));

            /// <summary>
            /// Read the rest of the text in the stream
            /// </summary>
            [Pure, MethodImpl(IO.mops)]
            public static IO<RT, string> readToEnd<RT>(TextReader reader) 
                where RT : struct, HasTextRead<RT> =>
                default(RT).TextReadIO.MapAsync(e => e.ReadToEnd(reader));

            /// <summary>
            /// Read chars from the stream into the buffer
            /// Returns the number of chars read
            /// </summary>
            [Pure, MethodImpl(IO.mops)]
            public static IO<RT, int> read<RT>(TextReader reader, Memory<char> buffer)
                where RT : struct, HasTextRead<RT> =>
                default(RT).TextReadIO.MapAsync(e => e.Read(reader, buffer));

            /// <summary>
            /// Close the reader
            /// </summary>
            [Pure, MethodImpl(IO.mops)]
            public static SIO<RT, Unit> close<RT>(TextReader reader)
                where RT : struct, HasTextRead<RT> =>
                default(RT).TextReadSIO.Map(e => e.Close(reader));
        }
    }
}

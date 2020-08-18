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
            [Pure, MethodImpl(AffOpt.mops)]
            public static Aff<RT, Option<string>> readLine<RT>(TextReader reader) 
                where RT : struct, HasTextRead<RT> =>
                default(RT).TextReadAff.MapAsync(e => e.ReadLine(reader));

            /// <summary>
            /// Read the rest of the text in the stream
            /// </summary>
            [Pure, MethodImpl(AffOpt.mops)]
            public static Aff<RT, string> readToEnd<RT>(TextReader reader) 
                where RT : struct, HasTextRead<RT> =>
                default(RT).TextReadAff.MapAsync(e => e.ReadToEnd(reader));

            /// <summary>
            /// Read chars from the stream into the buffer
            /// Returns the number of chars read
            /// </summary>
            [Pure, MethodImpl(AffOpt.mops)]
            public static Aff<RT, int> read<RT>(TextReader reader, Memory<char> buffer)
                where RT : struct, HasTextRead<RT> =>
                default(RT).TextReadAff.MapAsync(e => e.Read(reader, buffer));

            /// <summary>
            /// Close the reader
            /// </summary>
            [Pure, MethodImpl(AffOpt.mops)]
            public static Eff<RT, Unit> close<RT>(TextReader reader)
                where RT : struct, HasTextRead<RT> =>
                default(RT).TextReadEff.Map(e => e.Close(reader));
        }
    }
}

using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime.CompilerServices;
using LanguageExt.Effects.Traits;
using LanguageExt.Sys.Traits;

namespace LanguageExt.Sys.IO
{
    public static class TextRead<RT>
        where RT : struct, HasTextRead<RT>
    {
        /// <summary>
        /// Read a line of text from the stream
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<RT, Option<string>> readLine(TextReader reader) => 
            default(RT).TextReadEff.MapAsync(e => e.ReadLine(reader));

        /// <summary>
        /// Read the rest of the text in the stream
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<RT, string> readToEnd(TextReader reader) =>
            default(RT).TextReadEff.MapAsync(e => e.ReadToEnd(reader));

        /// <summary>
        /// Read chars from the stream into the buffer
        /// Returns the number of chars read
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<RT, int> read(TextReader reader, Memory<char> buffer) =>
            default(RT).TextReadEff.MapAsync(e => e.Read(reader, buffer));

        /// <summary>
        /// Close the reader
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<RT, Unit> close(TextReader reader) =>
            default(RT).TextReadEff.Map(e => e.Close(reader));
    }
}

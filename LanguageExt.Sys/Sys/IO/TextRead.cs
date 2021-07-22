using System;
using System.IO;
using LanguageExt.Pipes;
using LanguageExt.Sys.Traits;
using static LanguageExt.Prelude;
using LanguageExt.Effects.Traits;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using LanguageExt.UnsafeValueAccess;
using System.Runtime.CompilerServices;
using static LanguageExt.Pipes.Proxy;

namespace LanguageExt.Sys.IO
{
    public static class TextRead<RT>
        where RT : struct, HasTextRead<RT>
    {
        /// <summary>
        /// Open a text file and streams the lines through the pipe
        /// </summary>
        [Pure]
        public static Pipe<RT, TextReader, string, Unit> readLine
        {
            get
            {
                return from tr in awaiting<TextReader>()
                       from ln in enumerate(readLine(tr))
                       from __ in yield(ln)
                       select unit;

                static async IAsyncEnumerable<string> readLine(TextReader reader)
                {
                    while (true)
                    {
                        var line = await reader.ReadLineAsync();
                        if(line == null) yield break;
                        yield return line;
                    }
                }
            }
        } 
        
        /// <summary>
        /// Read the rest of the text in the stream
        /// </summary>
        [Pure]
        public static Pipe<RT, TextReader, string, Unit> readToEnd =>
            from tr in awaiting<TextReader>()
            from tx in Aff<RT, string>(async _ => await tr.ReadToEndAsync())
            from __ in yield(tx)
            select unit;

        /// <summary>
        /// Repeatedly read a number of chars from the stream
        /// </summary>
        [Pure]
        public static Pipe<RT, TextReader, Seq<char>, Unit> readChars(int charCount)
        {
            return from tr in awaiting<TextReader>()
                   from cs in enumerate(go(tr, charCount))
                   from __ in yield(cs)
                   select unit;

            static async IAsyncEnumerable<Seq<char>> go(TextReader reader, int count)
            {
                while (true)
                {
                    var buffer = new char[count];
                    var nread  = await reader.ReadAsync(buffer, 0, count);
                    if(nread < 0) yield break;
                    yield return buffer.ToSeqUnsafe();
                }
            }
        }         

        /// <summary>
        /// Read a number of chars from the stream
        /// </summary>
        [Pure]
        public static Pipe<RT, TextReader, string, Unit> read(int charCount)
        {
            return from tr in awaiting<TextReader>()
                   from cs in enumerate(go(tr, charCount))
                   from __ in yield(cs)
                   select unit;

            static async IAsyncEnumerable<string> go(TextReader reader, int count)
            {
                while (true)
                {
                    var buffer = new char[count];
                    var nread  = await reader.ReadAsync(buffer, 0, count);
                    if(nread < 0) yield break;
                    yield return new string(buffer);
                }
            }
        }         

        /// <summary>
        /// Close the reader
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<RT, Unit> close(TextReader reader) =>
            default(RT).TextReadEff.Map(e => e.Close(reader));
    }
}

using System;
using System.Buffers;
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
                       from ln in enumerate2(go(tr))
                       from __ in yield(ln)
                       select unit;

                static async IAsyncEnumerable<string> go(TextReader reader)
                {
                    while (true)
                    {
                        var line = await reader.ReadLineAsync().ConfigureAwait(false);
                        if(line == null) yield break;
                        yield return line;
                    }
                }
            }
        } 
        
        /// <summary>
        /// Open a text file and streams the chars through the pipe
        /// </summary>
        [Pure]
        public static Pipe<RT, TextReader, char, Unit> readChar
        {
            get
            {
                return from tr in awaiting<TextReader>()
                       from ln in enumerate2(go(tr))
                       from __ in yield(ln)
                       select unit;

                static async IAsyncEnumerable<char> go(TextReader reader)
                {
                    var buffer = new char[1];
                    while (true)
                    {
                        var nread = await reader.ReadAsync(buffer, 0, 1).ConfigureAwait(false);
                        if(nread < 1) yield break;
                        yield return buffer[0];
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
        public static Pipe<RT, TextReader, SeqLoan<char>, Unit> readChars(int charCount)
        {
            return from tr in awaiting<TextReader>()
                   from cs in enumerate2(go(tr, charCount))
                   from __ in yield(cs)
                   select unit;

            static async IAsyncEnumerable<SeqLoan<char>> go(TextReader reader, int count)
            {
                var pool = ArrayPool<char>.Shared;
                while (true)
                {
                    var buffer = pool.Rent(count);
                    var nread  = await reader.ReadAsync(buffer, 0, count).ConfigureAwait(false);
                    if(nread < 0) yield break;
                    yield return buffer.ToSeqLoanUnsafe(nread, pool);
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
                   from cs in enumerate2(go(tr, charCount))
                   from __ in yield(cs)
                   select unit;

            static async IAsyncEnumerable<string> go(TextReader reader, int count)
            {
                var pool   = ArrayPool<char>.Shared;
                var buffer = pool.Rent(count);
                try
                {
                    while (true)
                    {
                        var nread = await reader.ReadAsync(buffer, 0, count).ConfigureAwait(false);
                        if (nread < 0) yield break;
                        yield return new string(buffer);
                    }
                }
                finally
                {
                    pool.Return(buffer);
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

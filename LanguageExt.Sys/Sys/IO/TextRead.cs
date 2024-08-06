using System.Buffers;
using System.IO;
using LanguageExt.Pipes;
using LanguageExt.Sys.Traits;
using static LanguageExt.Prelude;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using LanguageExt.UnsafeValueAccess;
using System.Runtime.CompilerServices;
using LanguageExt.Traits;
using static LanguageExt.Pipes.Proxy;

namespace LanguageExt.Sys.IO;

public static class TextRead<M, RT>
    where RT : 
        Has<M, TextReadIO>
    where M : 
        Monad<M>
{
    static K<M, TextReadIO> textReadIO => 
        Has<M, RT, TextReadIO>.ask;
    
    /// <summary>
    /// Open a text file and streams the lines through the pipe
    /// </summary>
    [Pure]
    public static Pipe<TextReader, string, M, Unit> readLine
    {
        get
        {
            return from tr in awaiting<TextReader>()
                   from _  in yieldAll(go(tr))
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
    public static Pipe<TextReader, char, M, Unit> readChar
    {
        get
        {
            return from tr in awaiting<TextReader>()
                   from _  in yieldAll(go(tr))
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
    public static Pipe<TextReader, string, M, Unit> readToEnd =>
        from tr in awaiting<TextReader>()
        from tx in IO<string>.LiftAsync(async e => await tr.ReadToEndAsync(e.Token))
        from __ in yield(tx)
        select unit;

    /// <summary>
    /// Repeatedly read a number of chars from the stream
    /// </summary>
    [Pure]
    public static Pipe<TextReader, SeqLoan<char>, M, Unit> readChars(int charCount)
    {
        return from tr in awaiting<TextReader>()
               from _  in yieldAll(go(tr, charCount))
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
    public static Pipe<TextReader, string, M, Unit> read(int charCount)
    {
        return from tr in awaiting<TextReader>()
               from _  in yieldAll(go(tr, charCount))
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
    [Pure, MethodImpl(EffOpt.mops)]
    public static K<M, Unit> close(TextReader reader) =>
        textReadIO.Bind(e => e.Close(reader));
}

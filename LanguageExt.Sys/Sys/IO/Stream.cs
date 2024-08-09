using System.IO;
using System.Buffers;
using LanguageExt.Pipes;
using System.Collections.Generic;
using LanguageExt.Traits;
using static LanguageExt.Prelude;
using LanguageExt.UnsafeValueAccess;

namespace LanguageExt.Sys.IO;

public class Stream<M> where M : Monad<M>
{
    /// <summary>
    /// Get a pipe of chunks from a Stream
    /// </summary>
    public static Pipe<System.IO.Stream, SeqLoan<byte>, M, Unit> read(int chunkSize)
    {
        return from fs in Proxy.awaiting<System.IO.Stream>()
               from _  in Proxy.yieldAll(chunks(fs, chunkSize))
               select unit;

        static async IAsyncEnumerable<SeqLoan<byte>> chunks(System.IO.Stream fs, int chunkSize)
        {
            var pool = ArrayPool<byte>.Shared;
            while (true)
            {
                var buffer = pool.Rent(chunkSize);
                var count  = await fs.ReadAsync(buffer, 0, chunkSize).ConfigureAwait(false);
                if (count < 1)
                {
                    pool.Return(buffer);
                    yield break;
                }
                yield return buffer.ToSeqLoanUnsafe(count, pool); 
            }
        }
    }
}

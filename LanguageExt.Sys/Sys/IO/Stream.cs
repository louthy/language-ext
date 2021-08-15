using System.IO;
using System.Buffers;
using LanguageExt.Pipes;
using System.Collections.Generic;
using LanguageExt.Effects.Traits;
using static LanguageExt.Prelude;
using LanguageExt.UnsafeValueAccess;

namespace LanguageExt.Sys.IO
{
    public static class Stream<RT> where RT : struct, HasCancel<RT>
    {
        /// <summary>
        /// Get a pipe of chunks from a Stream
        /// </summary>
        public static Pipe<RT, Stream, SeqLoan<byte>, Unit> read(int chunkSize)
        {
            return from fs in Proxy.awaiting<Stream>()
                   from bt in Proxy.enumerate2(chunks(fs, chunkSize))
                   from un in Proxy.yield(bt)
                   select unit;

            static async IAsyncEnumerable<SeqLoan<byte>> chunks(Stream fs, int chunkSize)
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
}

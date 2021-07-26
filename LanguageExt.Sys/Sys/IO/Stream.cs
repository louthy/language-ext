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
        public static Pipe<RT, Stream, Seq<byte>, Unit> read(int chunkSize)
        {
            return from fs in Proxy.awaiting<Stream>()
                   from bt in Proxy.enumerate(chunks(fs, chunkSize))
                   from un in Proxy.yield(bt)
                   select unit;

            static async IAsyncEnumerable<Seq<byte>> chunks(Stream fs, int chunkSize)
            {
                var pool = ArrayPool<byte>.Shared;
                while (true)
                {
                    var buffer = pool.Rent(chunkSize);
                    var count  = await fs.ReadAsync(buffer, 0, chunkSize);
                    if (count < 1)
                    {
                        yield break;
                    }
                    yield return buffer.ToSeqUnsafe(count, pool); 
                }
            }
        }
    }
}

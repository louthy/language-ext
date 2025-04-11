using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

internal class SourceTInternal
{
    public static async ValueTask<bool> ReadyToRead<M, A>(Seq<SourceTIterator<M, A>> sources, CancellationToken token)
        where M : Monad<M>, Alternative<M>
    {
        if (sources.Count == 0) throw Errors.SourceClosed;

        var             remaining  = sources.Count;
        using var       wait       = new CountdownEvent(remaining);
        using var       src        = new CancellationTokenSource();
        await using var reg        = token.Register(() => src.Cancel());
        var             childToken = src.Token;
        var             ready      = false;

        try
        {
            sources.Map(s => s.ReadyToRead(childToken)
                              .Map(f =>
                                   {
                                       ready = f || ready;
                                       if (ready)
                                       {
                                           // Clear all signals
                                           // ReSharper disable once AccessToDisposedClosure
                                           wait.Signal(remaining);
                                       }
                                       else
                                       {
                                           // Clear one signal
                                           // ReSharper disable once AccessToDisposedClosure
                                           wait.Signal();
                                           Interlocked.Decrement(ref remaining);
                                       }
                                       return f;
                                   }))
                   .Strict();

            wait.Wait(token);
            return ready;
        }
        finally
        {
            await src.CancelAsync();
        }
    }
    
    public static ReadResult<M, A> Read<M, A>(Seq<SourceTIterator<M, A>> sources)
        where M : Monad<M>, Alternative<M>
    {
        if (sources.Count == 0) throw Errors.SourceClosed;

        var                    remaining  = sources.Count;
        using var              wait       = new CountdownEvent(remaining);
      //using var              src        = new CancellationTokenSource();
      //using var              reg        = token.Register(() => src.Cancel());
      //var                    childToken = src.Token;
        var                    flag       = 0;
        SourceTIterator<M, A>? source     = null;

      //try
      //{
            sources.Map(s => s.ReadyToRead(default)
                              .Map(f =>
                                   {
                                       if (f && Interlocked.CompareExchange(ref flag, 1, 0) == 0)
                                       {
                                           // The source that is ready to yield a value
                                           source = s;
                                           flag = 2;

                                           // Clear all signals
                                           // ReSharper disable once AccessToDisposedClosure
                                           wait.Signal(remaining);
                                       }
                                       else
                                       {
                                           // Clear one signal
                                           // ReSharper disable once AccessToDisposedClosure
                                           wait.Signal();
                                           Interlocked.Decrement(ref remaining);
                                       }

                                       return f;
                                   }))
                   .Strict();

            wait.Wait();
            return flag == 2
                       ? source!.Read()
                       : throw Errors.SourceClosed;
      //}
      //finally
      //{
      //    src.Cancel();
      //}
    }
}

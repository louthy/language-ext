using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;

namespace LanguageExt.Pipes.Concurrent;

internal class OutboxInternal
{
    public static async ValueTask<bool> ReadyToRead<A>(Seq<Outbox<A>> sources, CancellationToken token)
    {
        if (sources.Count == 0) throw Errors.OutboxChannelClosed;

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
    
    public static async ValueTask<A> Read<A>(Seq<Outbox<A>> sources, EnvIO envIO)
    {
        if (sources.Count == 0) throw Errors.OutboxChannelClosed;

        var             remaining  = sources.Count;
        using var       wait       = new CountdownEvent(remaining);
        using var       src        = new CancellationTokenSource();
        await using var reg        = envIO.Token.Register(() => src.Cancel());
        var             childToken = src.Token;
        var             flag       = 0;
        Outbox<A>?      source     = null;

        try
        {
            sources.Map(s => s.ReadyToRead(childToken)
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

            wait.Wait(envIO.Token);
            return flag == 2
                       ? await source!.Read().RunAsync(envIO)
                       : throw Errors.OutboxChannelClosed;
        }
        finally
        {
            await src.CancelAsync();
        }
    }
}

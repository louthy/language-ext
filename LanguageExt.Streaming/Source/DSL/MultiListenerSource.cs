using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace LanguageExt;

record MultiListenerSource<A>(Channel<A> Source) : Source<A>
{
    volatile int count;
    readonly ConcurrentDictionary<Channel<A>, Unit> listeners = new();
    readonly CancellationTokenSource tokenSource = new();

    internal override async ValueTask<Reduced<S>> ReduceAsync<S>(S state, ReducerAsync<A, S> reducer, CancellationToken token)
    {
        var channel = Channel.CreateUnbounded<A>();
        listeners.TryAdd(channel, unit);
        
        if (Interlocked.Increment(ref count) == 1)
        {
            _ = Startup();
        }

        try
        {
            var rdr = channel.Reader;
            while (await rdr.WaitToReadAsync(token))
            {
                switch (await reducer(state, await rdr.ReadAsync(token)))
                {
                    case { Continue: true, Value: var nstate }:
                        state = nstate;
                        break;
                    
                    case { Value: var nstate }:
                        return Reduced.Done(nstate);
                }
            }
            return Reduced.Continue(state);
        }
        finally
        {
            listeners.TryRemove(channel, out _);
            if (Interlocked.Decrement(ref count) == 0)
            {
                await Shutdown();
            }
        }
    }

    async Task Startup()
    {
        try
        {
            var token = tokenSource.Token;
            while (await Source.Reader.WaitToReadAsync(token))
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }

                var x = await Source.Reader.ReadAsync(token);
                foreach (var listener in listeners.Keys)
                {
                    if (await listener.Writer.WaitToWriteAsync(token))
                    {
                        await listener.Writer.WriteAsync(x, token);
                    }
                }
            }
        }
        catch (Exception e)
        {
            foreach (var listener in listeners.Keys)
            {
                listener.Writer.TryComplete(e);
            }
        }
        finally
        {
            foreach (var listener in listeners.Keys)
            {
                listener.Writer.TryComplete();
            }
        }
    }

    async Task Shutdown() =>
        await tokenSource.CancelAsync();
}

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

record MultiListenerPureSourceT<M, A>(Channel<A> Source) : SourceT<M, A>
    where M : MonadIO<M>, Alternative<M>
{
    volatile int count;
    readonly ConcurrentDictionary<Channel<K<M, A>>, Unit> listeners = new();
    readonly CancellationTokenSource tokenSource = new();

    public override K<M, Reduced<S>> ReduceInternalM<S>(S state, ReducerM<M, K<M, A>, S> reducer)
    {
        return from channel in mkChannel
               let final    =  final(channel)
               from result  in body(channel, state)
                                .Bind(final.ConstMap)                          // Run final if we succeed
                                .Choose(final.ConstMap(Reduced.Done(state)))   // Run final if we fail and keep the original state
               select result;

        K<M, Reduced<S>> body(Channel<K<M, A>> channel, S state) =>
            from _  in addListener(channel) >> 
                       startup
            from st in readAll(channel, reducer, state) >>
                       final(channel)
            select st;
    }

    IO<Channel<K<M, A>>> mkChannel =>
        IO.lift(_ => Channel.CreateUnbounded<K<M, A>>());
    
    IO<Unit> addListener(Channel<K<M, A>> channel) =>
        IO.lift(_ => ignore(listeners.TryAdd(channel, unit)));
    
    IO<Unit> startup =>
        IO.lift(_ =>
                {
                    if (Interlocked.Increment(ref count) == 1)
                    {
                        // We let the Task run without awaiting, because we don't want to block
                        ignore(startupAsync());
                    }
                    return unit;
                });
    
    async Task startupAsync()
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
                        await listener.Writer.WriteAsync(M.Pure(x), token);
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

    K<M, Reduced<S>> readAll<S>(Channel<K<M, A>> channel, ReducerM<M, K<M, A>, S> reducer, S state)
    {
        return M.LiftIO(IO.liftVAsync(async e => await go(state, reducer, e.Token))).Flatten();
        
        async ValueTask<K<M, Reduced<S>>> go(S state, ReducerM<M, K<M, A>, S> reducer, CancellationToken token)
        {
            if (token.IsCancellationRequested) return M.Pure(Reduced.Done(state));
            if (!await channel.Reader.WaitToReadAsync(token)) return M.Pure(Reduced.Done(state));
            var head = await channel.Reader.ReadAsync(token);
            return reducer(state, head) >>
                   (ns => ns.Continue
                              ? go(ns.Value, reducer, token).GetAwaiter().GetResult()
                              : M.Pure(ns));
        }
    }

    K<M, Unit> final(Channel<K<M, A>> channel) =>
        M.LiftIO(
            IO.liftVAsync(async e =>
                          {
                              listeners.TryRemove(channel, out _);
                              if (Interlocked.Decrement(ref count) == 0)
                              {
                                  await shutdown();
                              }

                              return unit;
                          }));

    Task shutdown() =>
        tokenSource.CancelAsync();
}

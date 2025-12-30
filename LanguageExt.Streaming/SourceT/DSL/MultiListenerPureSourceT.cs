using System;
using System.Threading;
using LanguageExt.Common;
using LanguageExt.Traits;
using System.Threading.Tasks;
using System.Threading.Channels;
using static LanguageExt.Prelude;
using System.Collections.Concurrent;

namespace LanguageExt;

record MultiListenerPureSourceT<M, A>(Channel<A> Source) : SourceT<M, A>
    where M : MonadIO<M>, Fallible<M>
{
    volatile int count;
    readonly ConcurrentDictionary<Channel<K<M, A>>, Unit> listeners = new();
    readonly CancellationTokenSource tokenSource = new();

    public override K<M, Reduced<S>> ReduceInternalM<S>(S state, ReducerM<M, K<M, A>, S> reducer)
    {
        return from channel in mkChannel
               let final    =  final(channel)
               from result  in body(channel, state)
                                .Bind(final.ConstMap)       // Run final if we succeed
                                .Catch(error<S>(channel))   // Run final if we fail and keep the original state
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

    K<M, Reduced<S>> readAll<S>(Channel<K<M, A>> channel, ReducerM<M, K<M, A>, S> reducer, S initialState)
    {
        return M.Recur(initialState, go);
        
        K<M, Next<S, Reduced<S>>> go(S state) =>
            IO.token >> (t => t.IsCancellationRequested
                                  ? complete(state)
                                  : waitToRead() >>
                                    (available => available
                                                      ? reduce(state)
                                                      : complete(state)));               

        K<M, Next<S, Reduced<S>>> reduce(S state) =>
            read() >> (head => reducer(state, head) >> next);

        K<M, Next<S, Reduced<S>>> next(Reduced<S> reduced) =>
            reduced.Continue
                ? M.Pure(Next.Loop<S, Reduced<S>>(reduced.Value))
                : M.Pure(Next.Done<S, Reduced<S>>(reduced));
        
        IO<bool> waitToRead() =>
            IO.liftVAsync(e => channel.Reader.WaitToReadAsync(e.Token));

        IO<K<M, A>> read() =>
            IO.liftVAsync(e => channel.Reader.ReadAsync(e.Token));

        K<M, Next<S, Reduced<S>>> complete(S state) =>
            M.Pure(Next.Done<S, Reduced<S>>(Reduced.Done(state)));
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
    
    Func<Error, K<M, Reduced<S>>> error<S>(Channel<K<M, A>> channel) =>
        err =>
            final(channel) >> M.Fail<Reduced<S>>(err);
    
    Task shutdown() =>
        tokenSource.CancelAsync();
}

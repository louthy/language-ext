using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Pipes.Concurrent;

record Zip3Source<A, B, C>(Source<A> SourceA, Source<B> SourceB, Source<C> SourceC) : Source<(A First, B Second, C Third)>
{
    internal override SourceIterator<(A First, B Second, C Third)> GetIterator() =>
        new Zip3SourceIterator<A, B, C>(SourceA.GetIterator(), SourceB.GetIterator(), SourceC.GetIterator());
    
    internal override async ValueTask<Reduced<S>> ReduceAsync<S>(
        S state, 
        Reducer<(A First, B Second, C Third), S> reducer,
        CancellationToken token)
    {
        using var wait   = new AutoResetEvent(false);
        var       active = true;
        var       queueA = new ConcurrentQueue<A>();
        var       queueB = new ConcurrentQueue<B>();
        var       queueC = new ConcurrentQueue<C>();

        var ta = SourceA.ReduceAsync(unit, reduceA, token)
                        .Map(_ =>
                             {
                                 active = false;
                                 wait.Set();
                                 return unit;
                             });

        var tb = SourceB.ReduceAsync(unit, reduceB, token)
                        .Map(_ =>
                             {
                                 active = false;
                                 wait.Set();
                                 return unit;
                             });

        var tc = SourceC.ReduceAsync(unit, reduceC, token)
                        .Map(_ =>
                             {
                                 active = false;
                                 wait.Set();
                                 return unit;
                             });
        
        Task.WhenAll(ta.AsTask(), tb.AsTask(), tc.AsTask()).Ignore();

        while (active)
        {
            if(token.IsCancellationRequested) throw new TaskCanceledException();
            await wait.WaitOneAsync(token);
            while (!queueA.IsEmpty && !queueB.IsEmpty && !queueC.IsEmpty)
            {
                if (queueA.TryDequeue(out var a) && queueB.TryDequeue(out var b) && queueC.TryDequeue(out var c))
                {
                    switch (await reducer(state, (a, b, c)))
                    {
                        case { Continue: true, Value: var nstate }:
                            state = nstate;
                            break;
                    
                        case { Value: var nstate }:
                            return Reduced.Done(nstate);
                    }
                }
            }
        }
        return Reduced.Done(state);
        
        ValueTask<Reduced<Unit>> reduceA(Unit _, A x)
        {
            queueA.Enqueue(x);
            wait.Set();
            return Reduced.UnitAsync;
        }
        
        ValueTask<Reduced<Unit>> reduceB(Unit _, B x)
        {
            queueB.Enqueue(x);
            wait.Set();
            return Reduced.UnitAsync;
        }
        
        ValueTask<Reduced<Unit>> reduceC(Unit _, C x)
        {
            queueC.Enqueue(x);
            wait.Set();
            return Reduced.UnitAsync;
        }
    }
}

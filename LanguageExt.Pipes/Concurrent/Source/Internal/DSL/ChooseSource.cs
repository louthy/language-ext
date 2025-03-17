using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace LanguageExt.Pipes.Concurrent;

record ChooseSource<A>(Source<A> SourceA, Source<A> SourceB) : Source<A>
{
    internal override SourceIterator<A> GetIterator() =>
        new ChooseSourceIterator<A>(SourceA.GetIterator(), SourceB.GetIterator());
    
    internal override async ValueTask<Reduced<S>> ReduceAsync<S>(
        S state, 
        Reducer<A, S> reducer,
        CancellationToken token)
    {
        using var wait   = new AutoResetEvent(false);
        var       active = true;
        var       queueA = new ConcurrentQueue<(long Index, A Value)>();
        var       queueB = new ConcurrentQueue<(long Index, A Value)>();
        var       index  = 0;
        var       countA = -1L;
        var       countB = -1L;

        var ta = SourceA.Map(x =>
                             {
                                 var ix = Interlocked.Increment(ref countA);
                                 return (Index: ix, Value: x);
                             })
                        .ReduceAsync(unit, reduceA, token)
                        .Map(_ =>
                             {
                                 active = false;
                                 wait.Set();
                                 return unit;
                             });

        var tb = SourceB.Map(x =>
                             {
                                 var ix = Interlocked.Increment(ref countB);
                                 return (Index: ix, Value: x);
                             })
                        .ReduceAsync(unit, reduceB, token)
                        .Map(_ =>
                             {
                                 active = false;
                                 wait.Set();
                                 return unit;
                             });
        
        Task.WhenAll(ta.AsTask(), tb.AsTask()).Ignore();

        while (active)
        {
            if(token.IsCancellationRequested) throw new TaskCanceledException();
            await wait.WaitOneAsync(token);
            while (!queueA.IsEmpty || !queueB.IsEmpty)
            {
                // Remove all items that are the past
                while (queueA.TryPeek(out var qta) && qta.Index < index) queueA.TryDequeue(out _);
                while (queueB.TryPeek(out var qtb) && qtb.Index < index) queueA.TryDequeue(out _);
                
                if (queueA.TryPeek(out var a) && a.Index == index)
                {
                    index++;
                    switch (await reducer(state, a.Value))
                    {
                        case { Continue: true, Value: var nstate }:
                            state = nstate;
                            break;
                    
                        case { Value: var nstate }:
                            return Reduced.Done(nstate);
                    }
                }
                else if (queueB.TryPeek(out var b) && b.Index == index)
                {
                    index++;
                    switch (await reducer(state, b.Value))
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
        
        ValueTask<Reduced<Unit>> reduceA(Unit _, (long Index, A Value) x)
        {
            queueA.Enqueue(x);
            wait.Set();
            return Reduced.UnitAsync;
        }
        
        ValueTask<Reduced<Unit>> reduceB(Unit _, (long Index, A Value) x)
        {
            queueB.Enqueue(x);
            wait.Set();
            return Reduced.UnitAsync;
        }
    }
}

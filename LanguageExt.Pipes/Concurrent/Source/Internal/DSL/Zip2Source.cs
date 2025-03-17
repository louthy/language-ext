using System.Threading;
using System.Threading.Tasks;
using static LanguageExt.Prelude;
using System.Collections.Concurrent;

namespace LanguageExt.Pipes.Concurrent;

record Zip2Source<A, B>(Source<A> SourceA, Source<B> SourceB) : Source<(A First, B Second)>
{
    internal override SourceIterator<(A First, B Second)> GetIterator() =>
        new Zip2SourceIterator<A, B>(SourceA.GetIterator(), SourceB.GetIterator());
    
    internal override async ValueTask<Reduced<S>> ReduceAsync<S>(
        S state, 
        Reducer<(A First, B Second), S> reducer,
        CancellationToken token)
    {
        using var wait   = new AutoResetEvent(false);
        var       active = true;
        var       queueA = new ConcurrentQueue<A>();
        var       queueB = new ConcurrentQueue<B>();

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
        
        Task.WhenAll(ta.AsTask(), tb.AsTask()).Ignore();

        while (active)
        {
            if(token.IsCancellationRequested) throw new TaskCanceledException();
            await wait.WaitOneAsync(token);
            while (!queueA.IsEmpty && !queueB.IsEmpty)
            {
                if (queueA.TryDequeue(out var a) && queueB.TryDequeue(out var b))
                {
                    switch (await reducer(state, (a, b)))
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
    }
}

using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace LanguageExt.Pipes.Concurrent;

record Zip4Source<A, B, C, D>(Source<A> SourceA, Source<B> SourceB, Source<C> SourceC, Source<D> SourceD) 
    : Source<(A First, B Second, C Third, D Fourth)>
{
    internal override SourceIterator<(A First, B Second, C Third, D Fourth)> GetIterator() =>
        new Zip4SourceIterator<A, B, C, D>(
            SourceA.GetIterator(), 
            SourceB.GetIterator(), 
            SourceC.GetIterator(), 
            SourceD.GetIterator());
    
    internal override async ValueTask<Reduced<S>> ReduceAsync<S>(
        S state, 
        Reducer<(A First, B Second, C Third, D Fourth), S> reducer,
        CancellationToken token)
    {
        using var wait   = new AutoResetEvent(false);
        var       active = true;
        var       queueA = new ConcurrentQueue<A>();
        var       queueB = new ConcurrentQueue<B>();
        var       queueC = new ConcurrentQueue<C>();
        var       queueD = new ConcurrentQueue<D>();

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

        var td = SourceD.ReduceAsync(unit, reduceD, token)
                        .Map(_ =>
                             {
                                 active = false;
                                 wait.Set();
                                 return unit;
                             });
        
        Task.WhenAll(ta.AsTask(), tb.AsTask(), tc.AsTask(), td.AsTask()).Ignore();

        while (active)
        {
            if(token.IsCancellationRequested) throw new TaskCanceledException();
            await wait.WaitOneAsync(token);
            while (!queueA.IsEmpty && !queueB.IsEmpty && !queueC.IsEmpty)
            {
                if (queueA.TryDequeue(out var a) && 
                    queueB.TryDequeue(out var b) && 
                    queueC.TryDequeue(out var c) && 
                    queueD.TryDequeue(out var d))
                {
                    switch (await reducer(state, (a, b, c, d)))
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
        
        ValueTask<Reduced<Unit>> reduceD(Unit _, D x)
        {
            queueD.Enqueue(x);
            wait.Set();
            return Reduced.UnitAsync;
        }
    }
}

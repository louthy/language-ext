using System.Threading;
using System.Threading.Tasks;
using static LanguageExt.Prelude;
using System.Collections.Concurrent;

namespace LanguageExt.Pipes.Concurrent;

record CombineSource<A>(Seq<Source<A>> Sources) : Source<A>
{
    internal override SourceIterator<A> GetIterator() =>
        new CombineSourceIterator<A>(Sources.Map(x => x.GetIterator()));
    
    internal override async ValueTask<Reduced<S>> ReduceAsync<S>(S state, Reducer<A, S> reducer, CancellationToken token)
    {
        using var wait   = new AutoResetEvent(false);
        var       active = Sources.Count;
        var       queue  = new ConcurrentQueue<A>();
        
        Task.WhenAll(Sources.Map(s => s.ReduceAsync(unit, reduce, token)
                                       .Map(_ =>
                                            {
                                                Interlocked.Decrement(ref active);
                                                wait.Set();
                                                return 0;
                                            }).AsTask())
                            .Strict())
            .Ignore();

        while (true)
        {
            if(token.IsCancellationRequested) throw new TaskCanceledException();
            await wait.WaitOneAsync(token);
            while (queue.TryDequeue(out var x))
            {
                switch (await reducer(state, x))
                {
                    case { Continue: true, Value: var nstate }:
                        state = nstate;
                        break;
                    
                    case { Value: var nstate }:
                        return Reduced.Done(nstate);
                }
            }
            if(active <= 0) return Reduced.Done(state);
        }
        
        ValueTask<Reduced<Unit>> reduce(Unit _, A x)
        {
            queue.Enqueue(x);
            wait.Set();
            return Reduced.UnitAsync;
        }
    }
}

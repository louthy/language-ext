using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;

namespace LanguageExt.Pipes.Concurrent;

record IteratorSyncSource<A>(IEnumerable<A> Items) : Source<A>
{
    internal override SourceIterator<A> GetIterator() =>
        new IteratorSyncSourceIterator<A> { Src = Items.GetIterator() };
    
    internal override ValueTask<Reduced<S>> ReduceAsync<S>(S state, Reducer<A, S> reducer, CancellationToken token)
    {
        using var iter = Items.GetEnumerator();
        while(iter.MoveNext())
        {
            if(token.IsCancellationRequested) return ValueTask.FromException<Reduced<S>>(Errors.Cancelled);
            var tstate = reducer(state, iter.Current);
            if (tstate.IsCompleted)
            {
                switch (tstate.Result)
                {
                    case { Continue: true, Value: var nstate }:
                        state = nstate;
                        break;
                    
                    case { Value: var nstate }:
                        return Reduced.DoneAsync(nstate);
                }
            }
            else
            {
                return ReduceAsyncInternal(tstate, iter, reducer, token);
            }
        }        
        return Reduced.DoneAsync(state);
    }
    
    async ValueTask<Reduced<S>> ReduceAsyncInternal<S>(
        ValueTask<Reduced<S>> tstate, 
        IEnumerator<A> iter, 
        Reducer<A, S> reducer, 
        CancellationToken token)
    {
        S? state;
        switch (await tstate)
        {
            case { Continue: true, Value: var nstate }:
                state = nstate;
                break;
                    
            case { Value: var nstate }:
                return Reduced.Done(nstate);
        }
        
        while(iter.MoveNext())
        {
            if(token.IsCancellationRequested) throw new TaskCanceledException();
            switch (await reducer(state, iter.Current))
            {
                case { Continue: true, Value: var nstate }:
                    state = nstate;
                    break;
                    
                case { Value: var nstate }:
                    return Reduced.Done(nstate);
            }
        }
        return Reduced.Done(state);
    }
}

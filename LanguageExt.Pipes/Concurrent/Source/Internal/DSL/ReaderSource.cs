using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using LanguageExt.Common;

namespace LanguageExt.Pipes.Concurrent;

record ReaderSource<A>(Channel<A> Channel) : Source<A>
{
    internal override SourceIterator<A> GetIterator() =>
        new ReaderSourceIterator<A>(Channel);
    
    internal override ValueTask<Reduced<S>> ReduceAsync<S>(S state, Reducer<A, S> reducer, CancellationToken token)
    {
        var reader = Channel.Reader;
        while (true)
        {
            if(token.IsCancellationRequested) return ValueTask.FromException<Reduced<S>>(Errors.Cancelled);
            
            var tready = reader.WaitToReadAsync(token);
            if(!tready.IsCompleted) return ReduceAsyncInternal(tready, state, reducer, reader, token);
            if(!tready.Result) return Reduced.DoneAsync(state);
            
            var tvalue = reader.ReadAsync(token);
            if(!tvalue.IsCompleted) return ReduceAsyncInternal(tvalue, state, reducer, reader, token);
            var value  = tvalue.Result;

            var tstate = reducer(state, value);
            if(!tstate.IsCompleted) return ReduceAsyncInternal(tstate, reducer, reader, token);
            
            switch (tstate.Result)
            {
                case { Continue: true, Value: var nstate }:
                    state = nstate;
                    break;
                    
                case { Value: var nstate }:
                    return Reduced.DoneAsync(nstate);
            }
        }
    }

    async ValueTask<Reduced<S>> ReduceAsyncInternal<S>(
        ValueTask<bool> tready, 
        S state, 
        Reducer<A, S> reducer, 
        ChannelReader<A> reader, 
        CancellationToken token)
    {
        var ready = await tready;
        if(!ready) return Reduced.Continue(state);
        do
        {
            if (token.IsCancellationRequested) throw new TaskCanceledException();
            
            var value = await reader.ReadAsync(token);

            switch (await reducer(state, value))
            {
                case { Continue: true, Value: var nstate }:
                    state = nstate;
                    break;
                    
                case { Value: var nstate }:
                    return Reduced.Done(nstate);
            }
            
        } while(await reader.WaitToReadAsync(token));
        return Reduced.Done(state);
    }

    async ValueTask<Reduced<S>> ReduceAsyncInternal<S>(
        ValueTask<A> tvalue, 
        S state, 
        Reducer<A, S> reducer, 
        ChannelReader<A> reader, 
        CancellationToken token)
    {
        var value = await tvalue;
        
        switch (await reducer(state, value))
        {
            case { Continue: true, Value: var nstate }:
                state = nstate;
                break;
                    
            case { Value: var nstate }:
                return Reduced.Done(nstate);
        }
        
        while(await reader.WaitToReadAsync(token))
        {
            if (token.IsCancellationRequested) throw new TaskCanceledException();
            
            value = await reader.ReadAsync(token);
            
            switch (await reducer(state, value))
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

    async ValueTask<Reduced<S>> ReduceAsyncInternal<S>(
        ValueTask<Reduced<S>> tstate, 
        Reducer<A, S> reducer, 
        ChannelReader<A> reader, 
        CancellationToken token)
    {
        S? state = default;
        
        switch (await tstate)
        {
            case { Continue: true, Value: var nstate }:
                state = nstate;
                break;
                    
            case { Value: var nstate }:
                return Reduced.Done(nstate);
        }
        
        while(await reader.WaitToReadAsync(token))
        {
            if (token.IsCancellationRequested) throw new TaskCanceledException();
            
            var value = await reader.ReadAsync(token);
            switch (await reducer(state, value))
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

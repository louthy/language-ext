using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Pipes.Concurrent;

record PureSource<A>(A Value) : Source<A>
{
    internal override SourceIterator<A> GetIterator() =>
        new SingletonSourceIterator<A>(Value);
    
    internal override ValueTask<Reduced<S>> ReduceAsync<S>(S state, Reducer<A, S> reducer, CancellationToken token) =>
        reducer(state, Value);
}

record ForeverSource<A>(A Value) : Source<A>
{
    internal override SourceIterator<A> GetIterator() =>
        new ForeverSourceIterator<A>(Value);

    internal override async ValueTask<Reduced<S>> ReduceAsync<S>(S state, Reducer<A, S> reducer, CancellationToken token)
    {
        while (true)
        {
            switch (await reducer(state, Value))
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

record EmptySource<A> : Source<A>
{
    public static readonly Source<A> Default = new EmptySource<A>();
    
    internal override SourceIterator<A> GetIterator() =>
        EmptySourceIterator<A>.Default;

    public override Source<B> Map<B>(Func<A, B> f) =>
        EmptySource<B>.Default;

    public override Source<B> Bind<B>(Func<A, Source<B>> f) => 
        EmptySource<B>.Default;

    public override Source<B> ApplyBack<B>(Source<Func<A, B>> ff) =>
        EmptySource<B>.Default;

    internal override ValueTask<Reduced<S>> ReduceAsync<S>(S state, Reducer<A, S> reducer, CancellationToken token) =>
        Reduced.DoneAsync(state);
}

record ReaderSource<A>(Channel<A> Channel, string Label) : Source<A>
{
    internal override SourceIterator<A> GetIterator() =>
        new ReaderSourceIterator<A>(Channel, Label);
    
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

/*
record MapSource<A, B>(Source<A> Source, Func<A, B> F) : Source<B>
{
    public override Source<C> Map<C>(Func<B, C> f) =>
        new MapSource<A, C>(Source, x => f(F(x)));

    public override Source<C> Bind<C>(Func<B, Source<C>> f) => 
        new BindSource<A, C>(Source, x => f(F(x)));

    public override SourceIterator<B> GetIterator() => 
        new MapSourceIterator<A, B>(Source.GetIterator(), F);
}

record ApplySource<A, B>(Source<A> Source, Source<Func<A, B>> FF) : Source<B>
{
    public override SourceIterator<B> GetIterator() => 
        new ApplySourceIterator<A, B>(Source.GetIterator(), FF.GetIterator());
}
*/

record ApplySource<A, B>(Source<Func<A, B>> FF, Source<A> FA) : Source<B>
{
    internal override SourceIterator<B> GetIterator() =>
        new ApplySourceIterator<A, B>(FF.GetIterator(), FA.GetIterator());

    internal override ValueTask<Reduced<S>> ReduceAsync<S>(S state, Reducer<B, S> reducer, CancellationToken token) =>
        FF.ReduceAsync(state,
                       (s, f) => token.IsCancellationRequested
                                     ? ValueTask.FromException<Reduced<S>>(Errors.Cancelled)
                                     : FA.ReduceAsync(s, (s1, x) => token.IsCancellationRequested
                                                                        ? ValueTask.FromException<Reduced<S>>(Errors.Cancelled)
                                                                        : reducer(s1, f(x)), token),
                       token);
}

record BindSource<A, B>(Source<A> Source, Func<A, Source<B>> F) : Source<B>
{
    internal override SourceIterator<B> GetIterator() =>
        new BindSourceIterator<A, B>(Source.GetIterator(), x => F(x).GetIterator());
    
    internal override ValueTask<Reduced<S>> ReduceAsync<S>(S state, Reducer<B, S> reducer, CancellationToken token) =>
        Source.ReduceAsync(state,
                           (s, x) => token.IsCancellationRequested
                                         ? ValueTask.FromException<Reduced<S>>(Errors.Cancelled)
                                         : F(x).ReduceAsync(s, reducer, token),
                           token);
}

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

record IteratorAsyncSource<A>(IAsyncEnumerable<A> Items) : Source<A>
{
    internal override SourceIterator<A> GetIterator() =>
        new IteratorAsyncSourceIterator<A> { Src = Items.GetIteratorAsync() };
    
    internal override async ValueTask<Reduced<S>> ReduceAsync<S>(S state, Reducer<A, S> reducer, CancellationToken token)
    {
        await foreach (var item in Items)
        {
            switch (await reducer(state, item))
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

record TransformSource<A, B>(Source<A> Source, Transducer<A, B> Transducer) : Source<B>
{
    internal override SourceIterator<B> GetIterator() =>
        new TransformSourceIterator<A, B>(Source.GetIterator(), Transducer);
    
    internal override ValueTask<Reduced<S>> ReduceAsync<S>(S state, Reducer<B, S> reducer, CancellationToken token) =>
        Source.ReduceAsync(state, Transducer.Reduce(reducer), token);
}

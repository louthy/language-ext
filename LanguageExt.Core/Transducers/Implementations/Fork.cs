#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LanguageExt.Transducers;

record ForkTransducer1<A, B>(Transducer<A, B> F, Option<TimeSpan> Timeout) : Transducer<A, TFork<B>>
{
    public override Reducer<A, S> Transform<S>(Reducer<TFork<B>, S> reduce) =>
        new Reduce1<S>(F, Timeout, reduce); 
            
    public override string ToString() =>  
        "fork";

    record Reduce1<S>(Transducer<A, B> F, Option<TimeSpan> Timeout, Reducer<TFork<B>, S> Reduce) : Reducer<A, S>
    {
        public override TResult<S> Run(TState state, S stateValue, A value)
        {
            // Create a new local token-source with its own cancellation token
            var tsrc = Timeout.IsSome 
                ? new CancellationTokenSource((TimeSpan)Timeout)
                : new CancellationTokenSource();
            var token = tsrc.Token;

            // If the parent cancels, we should too
            var reg = state.Token.Register(() => tsrc.Cancel());

            // Run the transducer asynchronously
            var cleanup = new CleanUp(tsrc, reg);
            var task = F.Invoke1Async(value, () => { cleanup.Dispose(); }, token, state.SynchronizationContext);
            
            return Reduce.Run(
                state,
                stateValue,
                new TFork<B>(new CancellationTransducer(tsrc), new AwaitTransducer(task, token)));
        }
    }

    record CleanUp(CancellationTokenSource Src, CancellationTokenRegistration Reg) : IDisposable
    {
        volatile int disposed;
        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed, 1) == 0)
            {
                try{Src?.Dispose();} catch { /* not important */ } 
                try{Reg.Dispose();} catch { /* not important */ }
            }
        }
    }

    record AwaitTransducer(Task<TResult<B>> Task, CancellationToken Token) 
        : Transducer<Unit, B>
    {
        public override Reducer<Unit, S> Transform<S>(Reducer<B, S> reduce) =>
            Reducer.from<Unit, S>((st, s, _) =>
            {
                var result = TaskAsync<B>.Run(_ => Task, Token);
                    
                return result switch
                {
                    TContinue<B> v => reduce.Run(st, s, v.Value),
                    TComplete<B> v => reduce.Run(st, s, v.Value),
                    TCancelled<B> => TResult.Cancel<S>(),
                    TFail<B> f => TResult.Fail<S>(f.Error),
                    _ => TResult.None<S>()
                };
            });
    }

    record CancellationTransducer(CancellationTokenSource Source) 
        : Transducer<Unit, Unit>
    {
        public override Reducer<Unit, S> Transform<S>(Reducer<Unit, S> reduce) => 
            Reducer.from<Unit, S>((st, s, u) =>
            {
                Cancel();
                return reduce.Run(st, s, u);
            });

        void Cancel()
        {
            try
            {
                Source.Cancel();
            }
            catch
            {
                // Not important
            }
        }
    }
}

record ForkTransducer2<FState, A, B>(
        Transducer<A, B> F, 
        FState Initial, 
        Reducer<B, FState> ForkedReducer, 
        Option<TimeSpan> Timeout) 
    : Transducer<A, TFork<FState>>
{
    public override Reducer<A, S> Transform<S>(Reducer<TFork<FState>, S> reduce) =>
        new Reduce1<S>(F, Initial, ForkedReducer, Timeout, reduce); 
            
    public override string ToString() =>  
        "fork";

    record Reduce1<S>(
        Transducer<A, B> F, 
        FState Initial, 
        Reducer<B, FState> ForkedReducer, 
        Option<TimeSpan> Timeout, 
        Reducer<TFork<FState>, S> Reduce) : Reducer<A, S>
    {
        public override TResult<S> Run(TState state, S stateValue, A value)
        {
            // Create a new local token-source with its own cancellation token
            var tsrc = Timeout.IsSome 
                ? new CancellationTokenSource((TimeSpan)Timeout)
                : new CancellationTokenSource();
            var token = tsrc.Token;

            // If the parent cancels, we should too
            var reg = state.Token.Register(() => tsrc.Cancel());

            // Run the transducer asynchronously
            var cleanup = new CleanUp(tsrc, reg);
            var task = F.InvokeAsync(value, Initial, ForkedReducer, () => { cleanup.Dispose(); }, token, state.SynchronizationContext);
            
            return Reduce.Run(
                state,
                stateValue,
                new TFork<FState>(new CancellationTransducer(tsrc), new AwaitTransducer(task, token)));
        }
    }

    record CleanUp(CancellationTokenSource Src, CancellationTokenRegistration Reg) : IDisposable
    {
        volatile int disposed;
        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed, 1) == 0)
            {
                try{Src?.Dispose();} catch { /* not important */ } 
                try{Reg.Dispose();} catch { /* not important */ }
            }
        }
    }

    record AwaitTransducer(Task<TResult<FState>> Task, CancellationToken Token) 
        : Transducer<Unit, FState>
    {
        public override Reducer<Unit, S> Transform<S>(Reducer<FState, S> reduce) =>
            Reducer.from<Unit, S>((st, s, _) =>
            {
                var result = TaskAsync<FState>.Run(_ => Task, Token);
                    
                return result switch
                {
                    TContinue<FState> v => reduce.Run(st, s, v.Value),
                    TComplete<FState> v => reduce.Run(st, s, v.Value),
                    TCancelled<FState> => TResult.Cancel<S>(),
                    TFail<FState> f => TResult.Fail<S>(f.Error),
                    _ => TResult.None<S>()
                };
            });
    }

    record CancellationTransducer(CancellationTokenSource Source) 
        : Transducer<Unit, Unit>
    {
        public override Reducer<Unit, S> Transform<S>(Reducer<Unit, S> reduce) => 
            Reducer.from<Unit, S>((st, s, u) =>
            {
                Cancel();
                return reduce.Run(st, s, u);
            });

        void Cancel()
        {
            try
            {
                Source.Cancel();
            }
            catch
            {
                // Not important
            }
        }
    }
}

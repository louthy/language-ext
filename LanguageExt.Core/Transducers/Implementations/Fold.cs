#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

namespace LanguageExt.Transducers;

record FoldTransducer<ST, A>(
        Schedule Schedule, 
        ST InitialState, 
        Func<ST, A, ST> Folder, 
        Func<(ST State, A Value), TResult<Unit>> Predicate)
    : Transducer<A, ST>
{
    public Transducer<A, ST> Morphism =>
        this;

    public Reducer<A, S> Transform<S>(Reducer<ST, S> reduce) =>
        new FoldReduce<S>(Schedule, InitialState, Folder, Predicate, reduce);

    public override string ToString() => 
        "fold";

    record FoldReduce<S>(
        Schedule Schedule,
        ST InitialState,
        Func<ST, A, ST> Folder, 
        Func<(ST State, A Value), TResult<Unit>> Predicate,
        Reducer<ST, S> Reduce) : Reducer<A, S>
    {
        enum FoldState
        {
            NotStarted,
            Schedule,
            Folding
        }

        FoldState fstate = FoldState.NotStarted;
        ST cstate = InitialState;
        IEnumerator<Duration> schedule = Array.Empty<Duration>().AsEnumerable().GetEnumerator();

        void Reset()
        {
            cstate = InitialState;
            fstate = FoldState.NotStarted;
            schedule.Dispose();
        }

        public override TResult<S> Run(TState state, S stateValue, A value)
        {
            while (true)
            {
                switch (fstate)
                {
                    case FoldState.NotStarted:
                        cstate = InitialState;
                        fstate = FoldState.Folding;
                        break;

                    case FoldState.Schedule:
                        if (schedule.MoveNext())
                        {
                            if (schedule.Current != Duration.Zero)
                            {
                                TaskAsync.Wait((TimeSpan)schedule.Current, state.Token);
                                if (state.Token.IsCancellationRequested)
                                {
                                    return TResult.Cancel<S>();
                                }
                            }

                            fstate = FoldState.Folding;
                        }
                        else
                        {
                            Reset();
                            return TResult.Complete(stateValue);
                        }
                        break;                    
                    
                    case FoldState.Folding:
                        cstate = Folder(cstate, value);
                        var tr = Predicate((cstate, value));

                        while (true)
                        {
                            switch (tr)
                            {
                                case TContinue<Unit>:
                                    fstate = FoldState.Schedule;
                                    return TResult.Continue(stateValue);

                                case TComplete<Unit>:
                                    Reset();
                                    return TResult.Recursive(state, stateValue, cstate, Reduce);

                                case TFail<Unit> f:
                                    Reset();
                                    return TResult.Fail<S>(f.Error);

                                case TCancelled<Unit>:
                                    Reset();
                                    return TResult.Cancel<S>();

                                case TNone<Unit>:
                                    Reset();
                                    return TResult.None<S>();
                                
                                case TRecursive<Unit> r:
                                    tr = r;
                                    break;

                                default:
                                    Reset();
                                    return TResult.None<S>();
                            }
                        }
                }
            }
        }
    }
}

record FoldSumTransducer<ST, X, A>(
        Schedule Schedule,
        ST InitialState, 
        Func<ST, A, ST> Folder, 
        Func<(ST State, A Value), TResult<Unit>> Predicate)
    : Transducer<Sum<X, A>, Sum<X, ST>>
{
    public Transducer<Sum<X, A>, Sum<X, ST>> Morphism =>
        this;

    public Reducer<Sum<X, A>, S> Transform<S>(Reducer<Sum<X, ST>, S> reduce) =>
        new FoldReduce<S>(Schedule, InitialState, Folder, Predicate, reduce);

    public override string ToString() => 
        "fold";

    record FoldReduce<S>(
        Schedule Schedule,
        ST InitialState,
        Func<ST, A, ST> Folder, 
        Func<(ST State, A Value), TResult<Unit>> Predicate,
        Reducer<Sum<X, ST>, S> Reduce) : Reducer<Sum<X, A>, S>
    {
        enum FoldState
        {
            NotStarted,
            Schedule,
            Folding
        }

        FoldState fstate = FoldState.NotStarted;
        ST cstate = InitialState;
        IEnumerator<Duration> schedule = Array.Empty<Duration>().AsEnumerable().GetEnumerator();

        public override TResult<S> Run(TState state, S stateValue, Sum<X, A> value)
        {
            switch (value)
            {
                case SumRight<X, A> r:
                    return RunRight(state, stateValue, r.Value);

                case SumLeft<X, A> l:
                    fstate = FoldState.NotStarted;
                    return TResult.Recursive(state, stateValue, Sum<X, ST>.Left(l.Value), Reduce);

                default:
                    return TResult.Complete(stateValue);
            }
        }

        void Reset()
        {
            cstate = InitialState;
            fstate = FoldState.NotStarted;
            schedule.Dispose();
        }

        TResult<S> RunRight(TState state, S stateValue, A value)
        {
            while (true)
            {
                switch (fstate)
                {
                    case FoldState.NotStarted:
                        cstate = InitialState;
                        fstate = FoldState.Folding;
                        schedule = Schedule.Run().GetEnumerator();
                        break;

                    case FoldState.Schedule:
                        if (schedule.MoveNext())
                        {
                            if (schedule.Current != Duration.Zero)
                            {
                                TaskAsync.Wait((TimeSpan)schedule.Current, state.Token);
                                if (state.Token.IsCancellationRequested)
                                {
                                    return TResult.Cancel<S>();
                                }
                            }

                            fstate = FoldState.Folding;
                        }
                        else
                        {
                            Reset();
                            return TResult.Complete(stateValue);
                        }
                        break;
                    
                    case FoldState.Folding:
                        cstate = Folder(cstate, value);
                        var tr = Predicate((cstate, value));

                        while (true)
                        {
                            switch (tr)
                            {
                                case TContinue<Unit>:
                                    fstate = FoldState.Schedule;
                                    return TResult.Continue(stateValue);

                                case TComplete<Unit>:
                                    Reset();
                                    return TResult.Recursive(
                                        state, 
                                        stateValue, 
                                        Sum<X, ST>.Right(cstate), 
                                        Reduce);

                                case TFail<Unit> f:
                                    Reset();
                                    return TResult.Fail<S>(f.Error);

                                case TCancelled<Unit>:
                                    Reset();
                                    return TResult.Cancel<S>();

                                case TNone<Unit>:
                                    Reset();
                                    return TResult.None<S>();
                                
                                case TRecursive<Unit> r:
                                    tr = r;
                                    break;

                                default:
                                    Reset();
                                    return TResult.None<S>();
                            }
                        }
                }
            }
        }
    }
}

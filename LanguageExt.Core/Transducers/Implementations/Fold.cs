#nullable enable
using System;
using System.Collections.Generic;

namespace LanguageExt.Transducers;

record FoldTransducer<ST, A>(
        Schedule Schedule, 
        ST InitialState, 
        Func<ST, A, ST> Folder, 
        Func<(ST State, A Value), TResult<Unit>> Predicate)
    : Transducer<A, ST>
{
    CState<ST, A> cstate = new(InitialState, Schedule, Folder, Predicate);
        
    public override Reducer<A, S> Transform<S>(Reducer<ST, S> reduce) =>
        new FoldReduce<S>(cstate, reduce);

    public override string ToString() => 
        "fold";

    record FoldReduce<S>(CState<ST, A> State, Reducer<ST, S> Reduce) : Reducer<A, S>
    {
        public override TResult<S> Run(TState state, S stateValue, A value) =>
            State.RunStateMachine(state, stateValue, value, Reduce);
    }
}

enum FoldState
{
    NotStarted,
    Schedule,
    Folding
}

class CState<ST, A>
{
    readonly ST InitialState;
    readonly Schedule Schedule;
    readonly Func<ST, A, ST> Folder;
    readonly Func<(ST State, A Value), TResult<Unit>> Predicate;
    
    ST State;
    FoldState FState;
    IEnumerator<Duration> Durations;

    public CState(ST initialState, Schedule schedule, Func<ST, A, ST> folder, Func<(ST State, A Value), TResult<Unit>> predicate)
    {
        InitialState = initialState;
        Schedule     = schedule;
        Folder       = folder;
        Predicate    = predicate;
        
        State        = InitialState;
        FState       = FoldState.NotStarted;
        Durations    = Schedule.Run().GetEnumerator();
    }
    
    public void Reset()
    {
        State     = InitialState;
        FState    = FoldState.NotStarted;
        Durations = Schedule.Run().GetEnumerator();
    }

    public TResult<S> CompleteStateMachine<S>(TState state, S stateValue, Reducer<ST, S> reduce)
    {
        var lstate = State;
        Reset();
        return reduce.Run(state, stateValue, lstate);
    }
    
    public TResult<S> RunStateMachine<S>(TState state, S stateValue, A value, Reducer<ST, S> reduce)
    {
        while (true)
        {
            switch (FState)
            {
                case FoldState.NotStarted:
                    State     = InitialState;
                    FState    = FoldState.Folding;
                    Durations = Schedule.Run().GetEnumerator();
                    break;

                case FoldState.Schedule:
                    if (Durations.MoveNext())
                    {
                        if (Durations.Current != Duration.Zero)
                        {
                            TaskAsync.Wait((TimeSpan)Durations.Current, state.Token);
                            if (state.Token.IsCancellationRequested)
                            {
                                return TResult.Cancel<S>();
                            }
                        }

                        FState = FoldState.Folding;
                    }
                    else
                    {
                        var lstate = State;
                        Reset();
                        return reduce.Run(state, stateValue, lstate);
                    }
                    break;                    
                
                case FoldState.Folding:
                    State = Folder(State, value);
                    var tr = Predicate((State, value));

                    while (true)
                    {
                        switch (tr)
                        {
                            case TContinue<Unit>:
                                FState = FoldState.Schedule;
                                return TResult.Continue(stateValue);

                            case TComplete<Unit>:
                                var lstate = State;
                                Reset();
                                return reduce.Run(state, stateValue, lstate);

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

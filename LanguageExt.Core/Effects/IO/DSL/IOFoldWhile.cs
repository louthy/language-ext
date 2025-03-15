using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageExt.DSL;
using LanguageExt.Traits;

namespace LanguageExt.DSL;

record IOFoldWhile<S, A, B>(
    IO<A> Operation,
    Schedule Schedule,
    S InitialState,
    Func<S, A, S> Folder,
    Func<(S State, A Value), bool> Predicate,
    Func<S, K<IO, B>> Next) : 
    InvokeSyncIO<B>
{
    public override IO<C> Map<C>(Func<B, C> f) =>
        new IOFoldWhile<S, A, C>(Operation, Schedule, InitialState, Folder, Predicate, x => Next(x).Map(f));

    public override IO<C> Bind<C>(Func<B, K<IO, C>> f) => 
        new IOFoldWhile<S, A, C>(Operation, Schedule, InitialState, Folder, Predicate, x => Next(x).Bind(f));

    public override IO<C> BindAsync<C>(Func<B, ValueTask<K<IO, C>>> f) => 
        new IOFoldWhile<S, A, C>(Operation, Schedule, InitialState, Folder, Predicate, x => Next(x).As().BindAsync(f));

    public override IO<B> Invoke(EnvIO envIO)
    {
        var task = Operation.RunAsync(envIO);
        if (task.IsCompleted)
        {
            var value = task.Result;
            if (!Predicate((InitialState, value)))
            {
                return Next(InitialState).As();
            }
            var state = Folder(InitialState, value);
            
            return new IOFoldingWhileSync<S, A, B>(Operation, Schedule.Run().GetEnumerator(), state, Folder, Predicate, Next);
        }
        else
        {
            return new IOFoldingWhileInitialAsync<S, A, B>(
                task, 
                Operation, 
                Schedule.Run().GetEnumerator(), InitialState, Folder, Predicate, Next);
        }
    }
    
    public override string ToString() => 
        "IO fold while";
}

record IOFoldingWhileInitialAsync<S, A, B>(
    ValueTask<A> First,
    IO<A> Operation,
    IEnumerator<Duration> Schedule,
    S State,
    Func<S, A, S> Folder,
    Func<(S State, A Value), bool> Predicate,
    Func<S, K<IO, B>> Next) : 
    InvokeAsyncIO<B>
{
    public override IO<C> Map<C>(Func<B, C> f) =>
        new IOFoldingWhileInitialAsync<S, A, C>(First, Operation, Schedule, State, Folder, Predicate, x => Next(x).Map(f));

    public override IO<C> Bind<C>(Func<B, K<IO, C>> f) => 
        new IOFoldingWhileInitialAsync<S, A, C>(First, Operation, Schedule, State, Folder, Predicate, x => Next(x).Bind(f));

    public override IO<C> BindAsync<C>(Func<B, ValueTask<K<IO, C>>> f) => 
        new IOFoldingWhileInitialAsync<S, A, C>(First, Operation, Schedule, State, Folder, Predicate, x => Next(x).As().BindAsync(f));

    public override async ValueTask<IO<B>> Invoke(EnvIO envIO) 
    {
        var value = await First;
        var state = State;
        if (!Predicate((state, value)))
        {
            return Next(state).As();
        }
        state = Folder(State, value);
            
        if (Schedule.MoveNext())
        {
            await Task.Delay((TimeSpan)Schedule.Current, envIO.Token);
            value = await Operation.RunAsync(envIO);
            if (!Predicate((state, value)))
            {
                return Next(State).As();
            }
            state = Folder(State, value);
            return new IOFoldingWhileAsync<S, A, B>(Operation, Schedule, state, Folder, Predicate, Next);
        }
        else
        {
            return Next(State).As();
        }
    }
    
    public override string ToString() => 
        "IO fold while initial async";
}

record IOFoldingWhileAsync<S, A, B>(
    IO<A> Operation,
    IEnumerator<Duration> Schedule,
    S State,
    Func<S, A, S> Folder,
    Func<(S State, A Value), bool> Predicate,
    Func<S, K<IO, B>> Next)
    : InvokeAsyncIO<B>
{
    public override IO<C> Map<C>(Func<B, C> f) => 
        new IOFoldingWhileAsync<S, A, C>(Operation, Schedule, State, Folder, Predicate, x => Next(x).Map(f));

    public override IO<C> Bind<C>(Func<B, K<IO, C>> f) => 
        new IOFoldingWhileAsync<S, A, C>(Operation, Schedule, State, Folder, Predicate, x => Next(x).Bind(f));

    public override IO<C> BindAsync<C>(Func<B, ValueTask<K<IO, C>>> f) => 
        new IOFoldingWhileAsync<S, A, C>(Operation, Schedule, State, Folder, Predicate, x => Next(x).As().BindAsync(f));

    public override async ValueTask<IO<B>> Invoke(EnvIO envIO) 
    {
        if (Schedule.MoveNext())
        {
            await Task.Delay((TimeSpan)Schedule.Current, envIO.Token);
            var value = await Operation.RunAsync(envIO);
            var state = State;
            if (!Predicate((state, value)))
            {
                return Next(State).As();
            }
            state = Folder(State, value);
            return new IOFoldingWhileAsync<S, A, B>(Operation, Schedule, state, Folder, Predicate, Next);
        }
        else
        {
            return Next(State).As();
        }
    }
    
    public override string ToString() => 
        "IO fold while async";
}

record IOFoldingWhileSync<S, A, B>(
    IO<A> Operation,
    IEnumerator<Duration> Schedule,
    S State,
    Func<S, A, S> Folder,
    Func<(S State, A Value), bool> Predicate,
    Func<S, K<IO, B>> Next) : 
    InvokeSyncIO<B>
{
    public override IO<C> Map<C>(Func<B, C> f) => 
        new IOFoldingWhileSync<S, A, C>(Operation, Schedule, State, Folder, Predicate, x => Next(x).Map(f));

    public override IO<C> Bind<C>(Func<B, K<IO, C>> f) => 
        new IOFoldingWhileSync<S, A, C>(Operation, Schedule, State, Folder, Predicate, x => Next(x).Bind(f));

    public override IO<C> BindAsync<C>(Func<B, ValueTask<K<IO, C>>> f) => 
        new IOFoldingWhileSync<S, A, C>(Operation, Schedule, State, Folder, Predicate, x => Next(x).As().BindAsync(f));

    public override IO<B> Invoke(EnvIO envIO) 
    {
        if (Schedule.MoveNext())
        {
            Task.Delay((TimeSpan)Schedule.Current, envIO.Token).GetAwaiter().GetResult();
            var value = Operation.Run(envIO);
            var state = State;
            if (!Predicate((state, value)))
            {
                return Next(State).As();
            }
            state = Folder(State, value);
            return new IOFoldingWhileSync<S, A, B>(Operation, Schedule, state, Folder, Predicate, Next);
        }
        else
        {
            return Next(State).As();
        }
    }
    
    public override string ToString() => 
        "IO fold while sync";
}

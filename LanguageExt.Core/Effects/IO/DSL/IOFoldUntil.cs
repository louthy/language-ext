using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageExt.DSL;
using LanguageExt.Traits;

namespace LanguageExt.DSL;

record IOFoldUntil<S, A, B>(
    IO<A> Operation,
    Schedule Schedule,
    S InitialState,
    Func<S, A, S> Folder,
    Func<(S State, A Value), bool> Predicate,
    Func<S, K<IO, B>> Next)
    : InvokeSyncIO<B>
{
    public override IO<C> Map<C>(Func<B, C> f) =>
        new IOFoldUntil<S, A, C>(Operation, Schedule, InitialState, Folder, Predicate, x => Next(x).Map(f));

    public override IO<C> Bind<C>(Func<B, K<IO, C>> f) => 
        new IOFoldUntil<S, A, C>(Operation, Schedule, InitialState, Folder, Predicate, x => Next(x).Bind(f));

    public override IO<B> Invoke(EnvIO envIO)
    {
        var task = Operation.RunAsync(envIO);
        if (task.IsCompleted)
        {
            var value = task.Result;
            var state = Folder(InitialState, value);
            return Predicate((state, value))
                       ? Next(state).As()
                       : new IOFoldingUntilSync<S, A, B>(Operation, Schedule.Run().GetEnumerator(), state, Folder, Predicate, Next);
        }
        else
        {
            return new IOFoldingUntilInitialAsync<S, A, B>(
                task, 
                Operation, 
                Schedule.Run().GetEnumerator(), InitialState, Folder, Predicate, Next);
        }
    }
    
    public override string ToString() => 
        "IO fold until";
}

record IOFoldingUntilInitialAsync<S, A, B>(
    ValueTask<A> First,
    IO<A> Operation,
    IEnumerator<Duration> Schedule,
    S State,
    Func<S, A, S> Folder,
    Func<(S State, A Value), bool> Predicate,
    Func<S, K<IO, B>> Next)
    : InvokeAsyncIO<B>
{
    public override IO<C> Map<C>(Func<B, C> f) =>
        new IOFoldingUntilInitialAsync<S, A, C>(First, Operation, Schedule, State, Folder, Predicate, x => Next(x).Map(f));

    public override IO<C> Bind<C>(Func<B, K<IO, C>> f) => 
        new IOFoldingUntilInitialAsync<S, A, C>(First, Operation, Schedule, State, Folder, Predicate, x => Next(x).Bind(f));

    public override async ValueTask<IO<B>> Invoke(EnvIO envIO) 
    {
        var value = await First;
        var state = Folder(State, value);
        if (Predicate((state, value)))
        {
            return Next(state).As();
        }
            
        if (Schedule.MoveNext())
        {
            await Task.Delay((TimeSpan)Schedule.Current, envIO.Token);
            value = await Operation.RunAsync(envIO);
            state = Folder(State, value);
            return Predicate((state, value))
                       ? Next(state).As()
                       : new IOFoldingUntilAsync<S, A, B>(Operation, Schedule, state, Folder, Predicate, Next);
        }
        else
        {
            return Next(State).As();
        }
    }
    
    public override string ToString() => 
        "IO fold until initial async";
}

record IOFoldingUntilAsync<S, A, B>(
    IO<A> Operation,
    IEnumerator<Duration> Schedule,
    S State,
    Func<S, A, S> Folder,
    Func<(S State, A Value), bool> Predicate,
    Func<S, K<IO, B>> Next)
    : InvokeAsyncIO<B>
{
    public override IO<C> Map<C>(Func<B, C> f) => 
        new IOFoldingUntilAsync<S, A, C>(Operation, Schedule, State, Folder, Predicate, x => Next(x).Map(f));

    public override IO<C> Bind<C>(Func<B, K<IO, C>> f) => 
        new IOFoldingUntilAsync<S, A, C>(Operation, Schedule, State, Folder, Predicate, x => Next(x).Bind(f));

    public override async ValueTask<IO<B>> Invoke(EnvIO envIO) 
    {
        if (Schedule.MoveNext())
        {
            await Task.Delay((TimeSpan)Schedule.Current, envIO.Token);
            var value = await Operation.RunAsync(envIO);
            var state = Folder(State, value);
            return Predicate((state, value))
                       ? Next(state).As()
                       : new IOFoldingUntilAsync<S, A, B>(Operation, Schedule, state, Folder, Predicate, Next);
        }
        else
        {
            return Next(State).As();
        }
    }
    
    public override string ToString() => 
        "IO fold until async";
}

record IOFoldingUntilSync<S, A, B>(
    IO<A> Operation,
    IEnumerator<Duration> Schedule,
    S State,
    Func<S, A, S> Folder,
    Func<(S State, A Value), bool> Predicate,
    Func<S, K<IO, B>> Next)
    : InvokeSyncIO<B>
{
    public override IO<C> Map<C>(Func<B, C> f) => 
        new IOFoldingUntilSync<S, A, C>(Operation, Schedule, State, Folder, Predicate, x => Next(x).Map(f));

    public override IO<C> Bind<C>(Func<B, K<IO, C>> f) => 
        new IOFoldingUntilSync<S, A, C>(Operation, Schedule, State, Folder, Predicate, x => Next(x).Bind(f));

    public override IO<B> Invoke(EnvIO envIO) 
    {
        if (Schedule.MoveNext())
        {
            Task.Delay((TimeSpan)Schedule.Current, envIO.Token).GetAwaiter().GetResult();
            var value = Operation.Run(envIO);
            var state = Folder(State, value);
            return Predicate((state, value))
                       ? Next(state).As()
                       : new IOFoldingUntilSync<S, A, B>(Operation, Schedule, state, Folder, Predicate, Next);
        }
        else
        {
            return Next(State).As();
        }
    }
    
    public override string ToString() => 
        "IO fold until sync";
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageExt.DSL;
using LanguageExt.Traits;

namespace LanguageExt.DSL;

record IOFold<S, A, B>(
    IO<A> Operation,
    Schedule Schedule,
    S InitialState,
    Func<S, A, S> Folder,
    Func<S, K<IO, B>> Next)
    : InvokeSyncIO<B>
{
    public override IO<C> Map<C>(Func<B, C> f) =>
        new IOFold<S, A, C>(Operation, Schedule, InitialState, Folder, x => Next(x).Map(f));

    public override IO<C> Bind<C>(Func<B, K<IO, C>> f) => 
        new IOFold<S, A, C>(Operation, Schedule, InitialState, Folder, x => Next(x).Bind(f));

    public override IO<B> Invoke(EnvIO envIO)
    {
        var task = Operation.RunAsync(envIO);
        if (task.IsCompleted)
        {
            var value = task.Result;
            var state = Folder(InitialState, value);
            return new IOFoldingSync<S, A, B>(Operation, Schedule.Run().GetEnumerator(), state, Folder, Next);
        }
        else
        {
            return new IOFoldingInitialAsync<S, A, B>(
                task, 
                Operation, 
                Schedule.Run().GetEnumerator(), InitialState, Folder, Next);
        }
    }
    
    public override string ToString() => 
        "IO fold";
}

record IOFoldingInitialAsync<S, A, B>(
    ValueTask<A> First,
    IO<A> Operation,
    IEnumerator<Duration> Schedule,
    S State,
    Func<S, A, S> Folder,
    Func<S, K<IO, B>> Next)
    : InvokeAsyncIO<B>
{
    public override IO<C> Map<C>(Func<B, C> f) => 
        new IOFoldingInitialAsync<S, A, C>(First, Operation, Schedule, State, Folder, x => Next(x).Map(f));

    public override IO<C> Bind<C>(Func<B, K<IO, C>> f) => 
        new IOFoldingInitialAsync<S, A, C>(First, Operation, Schedule, State, Folder, x => Next(x).Bind(f));

    public override async ValueTask<IO<B>> Invoke(EnvIO envIO) 
    {
        var value = await First;
        var state = Folder(State, value);
            
        if (Schedule.MoveNext())
        {
            await Task.Delay((TimeSpan)Schedule.Current, envIO.Token);
            value = await Operation.RunAsync(envIO);
            state = Folder(State, value);
            return new IOFoldingAsync<S, A, B>(Operation, Schedule, state, Folder, Next);
        }
        else
        {
            return Next(State).As();
        }
    }
    
    public override string ToString() => 
        "IO folding initial async";
}

record IOFoldingAsync<S, A, B>(
    IO<A> Operation,
    IEnumerator<Duration> Schedule,
    S State,
    Func<S, A, S> Folder,
    Func<S, K<IO, B>> Next)
    : InvokeAsyncIO<B>
{
    public override IO<C> Map<C>(Func<B, C> f) => 
        new IOFoldingAsync<S, A, C>(Operation, Schedule, State, Folder, x => Next(x).Map(f));

    public override IO<C> Bind<C>(Func<B, K<IO, C>> f) => 
        new IOFoldingAsync<S, A, C>(Operation, Schedule, State, Folder, x => Next(x).Bind(f));

    public override async ValueTask<IO<B>> Invoke(EnvIO envIO) 
    {
        if (Schedule.MoveNext())
        {
            await Task.Delay((TimeSpan)Schedule.Current, envIO.Token);
            var value = await Operation.RunAsync(envIO);
            var state = Folder(State, value);
            return new IOFoldingAsync<S, A, B>(Operation, Schedule, state, Folder, Next);
        }
        else
        {
            return Next(State).As();
        }
    }
    
    public override string ToString() => 
        "IO folding async";
}

record IOFoldingSync<S, A, B>(
    IO<A> Operation,
    IEnumerator<Duration> Schedule,
    S State,
    Func<S, A, S> Folder,
    Func<S, K<IO, B>> Next)
    : InvokeSyncIO<B>
{
    public override IO<C> Map<C>(Func<B, C> f) => 
        new IOFoldingSync<S, A, C>(Operation, Schedule, State, Folder, x => Next(x).Map(f));

    public override IO<C> Bind<C>(Func<B, K<IO, C>> f) => 
        new IOFoldingSync<S, A, C>(Operation, Schedule, State, Folder, x => Next(x).Bind(f));

    public override IO<B> Invoke(EnvIO envIO) 
    {
        if (Schedule.MoveNext())
        {
            Task.Delay((TimeSpan)Schedule.Current, envIO.Token).GetAwaiter().GetResult();
            var value = Operation.Run(envIO);
            var state = Folder(State, value);
            return new IOFoldingSync<S, A, B>(Operation, Schedule, state, Folder, Next);
        }
        else
        {
            return Next(State).As();
        }
    }
    
    public override string ToString() => 
        "IO folding sync";
}

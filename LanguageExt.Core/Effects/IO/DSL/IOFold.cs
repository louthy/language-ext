using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageExt.DSL;
using LanguageExt.Traits;

namespace LanguageExt;

record IOFold<S, A, B>(
    IO<A> Operation,
    Schedule Schedule,
    S InitialState,
    Func<S, A, S> Folder,
    Func<(S State, A Value), bool> Predicate,
    Func<S, K<IO, B>> Next)
    : InvokeAsyncIO<B>
{
    public override IO<C> Map<C>(Func<B, C> f) =>
        new IOFold<S, A, C>(Operation, Schedule, InitialState, Folder, Predicate, x => Next(x).Map(f));

    public override IO<C> Bind<C>(Func<B, K<IO, C>> f) => 
        new IOFold<S, A, C>(Operation, Schedule, InitialState, Folder, Predicate, x => Next(x).Bind(f));

    public override async ValueTask<IO<B>> Invoke(EnvIO envIO)
    {
        var value = await Operation.RunAsync(envIO);
        var state = Folder(InitialState, value);
        return Predicate((state, value))
                   ? Next(state).As()
                   : new IOFolding<S, A, B>(Operation, Schedule.Run().GetEnumerator(), state, Folder, Predicate, Next);
    }
}

record IOFolding<S, A, B>(
    IO<A> Operation,
    IEnumerator<Duration> Schedule,
    S State,
    Func<S, A, S> Folder,
    Func<(S State, A Value), bool> Predicate,
    Func<S, K<IO, B>> Next)
    : InvokeAsyncIO<B>
{
    public override IO<C> Map<C>(Func<B, C> f) => 
        new IOFolding<S, A, C>(Operation, Schedule, State, Folder, Predicate, x => Next(x).Map(f));

    public override IO<C> Bind<C>(Func<B, K<IO, C>> f) => 
        new IOFolding<S, A, C>(Operation, Schedule, State, Folder, Predicate, x => Next(x).Bind(f));

    public override async ValueTask<IO<B>> Invoke(EnvIO envIO) 
    {
        if (Schedule.MoveNext())
        {
            await Task.Delay((TimeSpan)Schedule.Current, envIO.Token);
            var value = await Operation.RunAsync(envIO);
            var state = Folder(State, value);
            return Predicate((state, value))
                       ? Next(state).As()
                       : new IOFolding<S, A, B>(Operation, Schedule, state, Folder, Predicate, Next);
        }
        else
        {
            return Next(State).As();
        }
    }
}

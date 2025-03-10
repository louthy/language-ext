using System;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt.DSL;

record IOFail<A>(Error Value) : InvokeSync<A>
{
    public override A Invoke(EnvIO envIO) => 
        Value.Throw<A>();
    
    public override IO<B> Map<B>(Func<A, B> f) => 
        new IOFail<B>(Value);

    public override IO<B> Bind<B>(Func<A, K<IO, B>> f) => 
        new IOFail<B>(Value);
    
    public override IO<S> Fold<S>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder) =>
        new IOFail<S>(Value);

    public override IO<S> Fold<S>(
        S initialState,
        Func<S, A, S> folder) =>
        new IOFail<S>(Value);

    public override IO<S> FoldWhile<S>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<S, bool> stateIs) =>
        new IOFail<S>(Value);

    public override IO<S> FoldWhile<S>(
        S initialState,
        Func<S, A, S> folder,
        Func<S, bool> stateIs) =>
        new IOFail<S>(Value);
    
    public override IO<S> FoldWhile<S>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<A, bool> valueIs) =>
        new IOFail<S>(Value);

    public override IO<S> FoldWhile<S>(
        S initialState,
        Func<S, A, S> folder,
        Func<A, bool> valueIs) =>
        new IOFail<S>(Value);
    
    public override IO<S> FoldWhile<S>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), bool> predicate) =>
        new IOFail<S>(Value);

    public override IO<S> FoldWhile<S>(
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), bool> predicate) =>
        new IOFail<S>(Value);

    public override IO<S> FoldUntil<S>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<S, bool> stateIs) =>
        new IOFail<S>(Value);
    
    public override IO<S> FoldUntil<S>(
        S initialState,
        Func<S, A, S> folder,
        Func<S, bool> stateIs) =>
        new IOFail<S>(Value);
    
    public override IO<S> FoldUntil<S>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<A, bool> valueIs) =>
        new IOFail<S>(Value);
    
    public override IO<S> FoldUntil<S>(
        S initialState,
        Func<S, A, S> folder,
        Func<A, bool> valueIs) =>
        new IOFail<S>(Value);
    
    public override IO<S> FoldUntil<S>(
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), bool> predicate) =>
        new IOFail<S>(Value);

    public override IO<S> FoldUntil<S>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), bool> predicate) =>
        new IOFail<S>(Value);
    
    public override IO<A> Post() =>
        this;

    protected override IO<A> WithEnv(Func<EnvIO, EnvIO> f) => 
        this;

    protected override IO<A> WithEnvFail(Func<EnvIO, EnvIO> f) => 
        this;
    
    public override IO<ForkIO<A>> Fork(Option<TimeSpan> timeout = default) =>
        new IOFail<ForkIO<A>>(Value);

    public override IO<A> RepeatUntil(Schedule schedule, Func<A, bool> predicate) => 
        this;

    public override IO<A> RetryUntil(Schedule schedule, Func<Error, bool> predicate) => 
        this;

    public override IO<A> Finally<X>(K<IO, X> @finally) => 
        this;

    public override IO<A> Catch(Func<Error, bool> Predicate, Func<Error, K<IO, A>> Fail) =>
        this;
    
    public override string ToString() => 
        $"fail({Value})";
}

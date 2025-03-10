using System;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.DSL;

record IOEmpty<A> : InvokeSync<A>
{
    internal static readonly IO<A> Default = new IOEmpty<A>();

    public override A Invoke(EnvIO envIO) => 
        throw Errors.None;
    
    public override IO<B> Map<B>(Func<A, B> f) => 
        IOEmpty<B>.Default;

    public override IO<B> Bind<B>(Func<A, K<IO, B>> f) => 
        IOEmpty<B>.Default;

    public override IO<B> ApplyBack<B>(K<IO, Func<A, B>> f)=>
        new IOEmptyAsync<Func<A, B>, B>(f);
    
    public override IO<S> Fold<S>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder) =>
        IOEmpty<S>.Default;

    public override IO<S> Fold<S>(
        S initialState,
        Func<S, A, S> folder) =>
        IOEmpty<S>.Default;

    public override IO<S> FoldWhile<S>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<S, bool> stateIs) =>
        IOEmpty<S>.Default;

    public override IO<S> FoldWhile<S>(
        S initialState,
        Func<S, A, S> folder,
        Func<S, bool> stateIs) =>
        IOEmpty<S>.Default;
    
    public override IO<S> FoldWhile<S>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<A, bool> valueIs) =>
        IOEmpty<S>.Default;

    public override IO<S> FoldWhile<S>(
        S initialState,
        Func<S, A, S> folder,
        Func<A, bool> valueIs) =>
        IOEmpty<S>.Default;
    
    public override IO<S> FoldWhile<S>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), bool> predicate) =>
        IOEmpty<S>.Default;

    public override IO<S> FoldWhile<S>(
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), bool> predicate) =>
        IOEmpty<S>.Default;

    public override IO<S> FoldUntil<S>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<S, bool> stateIs) =>
        IOEmpty<S>.Default;
    
    public override IO<S> FoldUntil<S>(
        S initialState,
        Func<S, A, S> folder,
        Func<S, bool> stateIs) =>
        IOEmpty<S>.Default;
    
    public override IO<S> FoldUntil<S>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<A, bool> valueIs) =>
        IOEmpty<S>.Default;
    
    public override IO<S> FoldUntil<S>(
        S initialState,
        Func<S, A, S> folder,
        Func<A, bool> valueIs) =>
        IOEmpty<S>.Default;
    
    public override IO<S> FoldUntil<S>(
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), bool> predicate) =>
        IOEmpty<S>.Default;

    public override IO<S> FoldUntil<S>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), bool> predicate) =>
        IOEmpty<S>.Default;
    
    public override IO<A> Post() =>
        Default;

    protected override IO<A> WithEnv(Func<EnvIO, EnvIO> f) => 
        Default;

    protected override IO<A> WithEnvFail(Func<EnvIO, EnvIO> f) => 
        Default;
    
    public override IO<ForkIO<A>> Fork(Option<TimeSpan> timeout = default) =>
        IOEmpty<ForkIO<A>>.Default;

    public override IO<A> RepeatUntil(Schedule schedule, Func<A, bool> predicate) => 
        Default;

    public override IO<A> RetryUntil(Schedule schedule, Func<Error, bool> predicate) => 
        Default;

    public override IO<A> Finally<X>(K<IO, X> @finally) => 
        Default;

    public override IO<A> Catch(Func<Error, bool> Predicate, Func<Error, K<IO, A>> Fail) =>
        Default;
}

record IOEmptyAsync<X, A>(K<IO, X> RunFirst) : InvokeAsync<A>
{
    public override async ValueTask<A> Invoke(EnvIO envIO)
    {
        ignore(await RunFirst.RunAsync(envIO));
        throw Errors.None;
    }
    
    public override IO<B> Map<B>(Func<A, B> f) => 
        IOEmpty<B>.Default;

    public override IO<B> Bind<B>(Func<A, K<IO, B>> f) => 
        IOEmpty<B>.Default;

    public override IO<B> ApplyBack<B>(K<IO, Func<A, B>> f)=>
        new IOEmptyAsync<Func<A, B>, B>(f);
    
    public override IO<S> Fold<S>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder) =>
        IOEmpty<S>.Default;

    public override IO<S> Fold<S>(
        S initialState,
        Func<S, A, S> folder) =>
        IOEmpty<S>.Default;

    public override IO<S> FoldWhile<S>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<S, bool> stateIs) =>
        IOEmpty<S>.Default;

    public override IO<S> FoldWhile<S>(
        S initialState,
        Func<S, A, S> folder,
        Func<S, bool> stateIs) =>
        IOEmpty<S>.Default;
    
    public override IO<S> FoldWhile<S>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<A, bool> valueIs) =>
        IOEmpty<S>.Default;

    public override IO<S> FoldWhile<S>(
        S initialState,
        Func<S, A, S> folder,
        Func<A, bool> valueIs) =>
        IOEmpty<S>.Default;
    
    public override IO<S> FoldWhile<S>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), bool> predicate) =>
        IOEmpty<S>.Default;

    public override IO<S> FoldWhile<S>(
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), bool> predicate) =>
        IOEmpty<S>.Default;

    public override IO<S> FoldUntil<S>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<S, bool> stateIs) =>
        IOEmpty<S>.Default;
    
    public override IO<S> FoldUntil<S>(
        S initialState,
        Func<S, A, S> folder,
        Func<S, bool> stateIs) =>
        IOEmpty<S>.Default;
    
    public override IO<S> FoldUntil<S>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<A, bool> valueIs) =>
        IOEmpty<S>.Default;
    
    public override IO<S> FoldUntil<S>(
        S initialState,
        Func<S, A, S> folder,
        Func<A, bool> valueIs) =>
        IOEmpty<S>.Default;
    
    public override IO<S> FoldUntil<S>(
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), bool> predicate) =>
        IOEmpty<S>.Default;

    public override IO<S> FoldUntil<S>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), bool> predicate) =>
        IOEmpty<S>.Default;
    
    public override IO<A> Post() =>
        IOEmpty<A>.Default;

    protected override IO<A> WithEnv(Func<EnvIO, EnvIO> f) => 
        IOEmpty<A>.Default;

    protected override IO<A> WithEnvFail(Func<EnvIO, EnvIO> f) => 
        IOEmpty<A>.Default;
    
    public override IO<ForkIO<A>> Fork(Option<TimeSpan> timeout = default) =>
        IOEmpty<ForkIO<A>>.Default;

    public override IO<A> RepeatUntil(Schedule schedule, Func<A, bool> predicate) => 
        IOEmpty<A>.Default;

    public override IO<A> RetryUntil(Schedule schedule, Func<Error, bool> predicate) => 
        IOEmpty<A>.Default;

    public override IO<A> Finally<X>(K<IO, X> @finally) => 
        IOEmpty<A>.Default;

    public override IO<A> Catch(Func<Error, bool> Predicate, Func<Error, K<IO, A>> Fail) =>
        IOEmpty<A>.Default;    
}

using System;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt.DSL;

record IOActions<A, B>(IterableNE<K<IO, A>> Fas, Func<A, IO<B>> Next) : InvokeSyncIO<B>
{
    public override IO<C> Map<C>(Func<B, C> f) => 
        new IOActions<A, C>(Fas, x => Next(x).Map(f));

    public override IO<C> Bind<C>(Func<B, K<IO, C>> f) => 
        new IOActions<A, C>(Fas, x => Next(x).Bind(f));

    public override IO<C> BindAsync<C>(Func<B, ValueTask<K<IO, C>>> f) => 
        new IOActions<A, C>(Fas, x => Next(x).BindAsync(f));

    public override IO<B> Invoke(EnvIO envIO)
    {
        if (Fas.ForwardIterator() is (Exist<K<IO, A>> (var head), var tail))
        {
            var task = head.RunAsync(envIO);
            if (task.IsCompleted)
            {
                return new IOActionsSync<A, B>(task.Result, tail, Next);
            }
            else
            {
                return new IOActionsAsync<A, B>(task, tail, Next);
            }
        }
        else
        {
            throw new NotSupportedException("IterableNE can't be empty, so we will never get here");
        }
    }
}

record IOActionsSync<A, B>(A Head, Iterator<K<IO, A>> Tail, Func<A, IO<B>> Next) : InvokeSyncIO<B>
{
    public override IO<C> Map<C>(Func<B, C> f) => 
        new IOActionsSync<A, C>(Head, Tail, x => Next(x).Map(f));

    public override IO<C> Bind<C>(Func<B, K<IO, C>> f) => 
        new IOActionsSync<A, C>(Head, Tail, x => Next(x).Bind(f));

    public override IO<C> BindAsync<C>(Func<B, ValueTask<K<IO, C>>> f) => 
        new IOActionsSync<A, C>(Head, Tail, x => Next(x).BindAsync(f));

    public override IO<B> Invoke(EnvIO envIO)
    {
        if (Tail is (Exist<K<IO, A>> (var head), var tail))
        {
            var th = head.RunAsync(envIO);
            if (th.IsCompleted)
            {
                return new IOActionsSync<A, B>(th.Result, tail, Next);
            }
            else
            {
                return new IOActionsAsync<A, B>(th, tail, Next);
            }
        }
        else
        {
            return Next(Head);
        }
    }
}

record IOActionsAsync<A, B>(ValueTask<A> Head, Iterator<K<IO, A>> Tail, Func<A, IO<B>> Next) : InvokeAsyncIO<B>
{
    public override IO<C> Map<C>(Func<B, C> f) => 
        new IOActionsAsync<A, C>(Head, Tail, x => Next(x).Map(f));

    public override IO<C> Bind<C>(Func<B, K<IO, C>> f) => 
        new IOActionsAsync<A, C>(Head, Tail, x => Next(x).Bind(f));

    public override IO<C> BindAsync<C>(Func<B, ValueTask<K<IO, C>>> f) => 
        new IOActionsAsync<A, C>(Head, Tail, x => Next(x).BindAsync(f));

    public override async ValueTask<IO<B>> Invoke(EnvIO envIO)
    {
        var value = await Head;
        
        if (Tail is (Exist<K<IO, A>> (var head), var tail))
        {
            var th = head.RunAsync(envIO);
            if (th.IsCompleted)
            {
                return new IOActionsSync<A, B>(th.Result, tail, Next);
            }
            else
            {
                return new IOActionsAsync<A, B>(th, tail, Next);
            }
        }
        else
        {
            return Next(value);
        }
    }
}

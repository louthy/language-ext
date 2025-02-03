using System;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt.DSL;

record IOActions<A, B>(Iterator<K<IO, A>> Fas, Func<A, IO<B>> Next) : InvokeSyncIO<B>
{
    public override IO<C> Map<C>(Func<B, C> f) => 
        new IOActions<A, C>(Fas, x => Next(x).Map(f));

    public override IO<C> Bind<C>(Func<B, K<IO, C>> f) => 
        new IOActions<A, C>(Fas, x => Next(x).Bind(f));

    public override IO<B> Invoke(EnvIO envIO)
    {
        if (Fas.IsEmpty)
        {
            return IO.fail<B>(Error.New("Actions is empty"));
        }
        else
        {
            var head = Fas.Head;
            var task = head.RunAsync(envIO);
            if (task.IsCompleted)
            {
                return new IOActionsSync<A, B>(task.Result, Fas.Tail.Split(), Next);
            }
            else
            {
                return new IOActionsAsync<A, B>(task, Fas.Tail.Split(), Next);
            }
        }
    }
}

record IOActionsSync<A, B>(A Value, Iterator<K<IO, A>> Fas, Func<A, IO<B>> Next) : InvokeSyncIO<B>
{
    public override IO<C> Map<C>(Func<B, C> f) => 
        new IOActionsSync<A, C>(Value, Fas, x => Next(x).Map(f));

    public override IO<C> Bind<C>(Func<B, K<IO, C>> f) => 
        new IOActionsSync<A, C>(Value, Fas, x => Next(x).Bind(f));

    public override IO<B> Invoke(EnvIO envIO)
    {
        if (Fas.IsEmpty)
        {
            return Next(Value);
        }
        else
        {
            return new IOActions<A, B>(Fas.Clone(), Next);
        }
    }
}

record IOActionsAsync<A, B>(ValueTask<A> Value, Iterator<K<IO, A>> Fas, Func<A, IO<B>> Next) : InvokeAsyncIO<B>
{
    public override IO<C> Map<C>(Func<B, C> f) => 
        new IOActionsAsync<A, C>(Value, Fas, x => Next(x).Map(f));

    public override IO<C> Bind<C>(Func<B, K<IO, C>> f) => 
        new IOActionsAsync<A, C>(Value, Fas, x => Next(x).Bind(f));

    public override async ValueTask<IO<B>> Invoke(EnvIO envIO)
    {
        var value = await Value;
        
        if (Fas.IsEmpty)
        {
            return Next(value);
        }
        else
        {
            return new IOActions<A, B>(Fas.Clone(), Next);
        }
    }
}

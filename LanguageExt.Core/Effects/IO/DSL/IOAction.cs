using System;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.DSL;

record IOAction<A, B, C>(K<IO, A> Fa, K<IO, B> Fb, Func<B, IO<C>> Next) : InvokeSyncIO<C>
{
    public override IO<D> Map<D>(Func<C, D> f) =>
        new IOAction<A, B, D>(Fa, Fb, b => Next(b).Map(f));

    public override IO<D> Bind<D>(Func<C, K<IO, D>> f) => 
        new IOAction<A, B, D>(Fa, Fb, b => Next(b).Bind(f));

    public override IO<C> Invoke(EnvIO envIO)
    {
        var taskA = Fa.As().RunAsync(envIO);
        if (taskA.IsCompleted)
        {
            return Fb.Bind(Next).As();
        }
        else
        {
            return new IOActionAsync<A, B, C>(taskA, Fb, Next);
        }
    }
}

record IOActionAsync<A, B, C>(ValueTask<A> Fa, K<IO, B> Fb, Func<B, IO<C>> Next) : InvokeAsyncIO<C>
{
    public override IO<D> Map<D>(Func<C, D> f) =>
        new IOActionAsync<A, B, D>(Fa, Fb, b => Next(b).Map(f));

    public override IO<D> Bind<D>(Func<C, K<IO, D>> f) => 
        new IOActionAsync<A, B, D>(Fa, Fb, b => Next(b).Bind(f));

    public override async ValueTask<IO<C>> Invoke(EnvIO envIO)
    {
        await Fa;
        return Fb.Bind(Next).As();
    }
}

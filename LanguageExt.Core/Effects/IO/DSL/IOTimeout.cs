using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Traits;
namespace LanguageExt.DSL;

record IOTimeout<A, B>(IO<A> Fa, TimeSpan TimeLimit, Func<A, K<IO, B>> Next) : InvokeSyncIO<B>
{
    public override IO<C> Map<C>(Func<B, C> f) => 
        new IOTimeout<A, C>(Fa, TimeLimit, x => Next(x).Map(f));

    public override IO<C> Bind<C>(Func<B, K<IO, C>> f) => 
        new IOTimeout<A, C>(Fa, TimeLimit, x => Next(x).Bind(f));

    public override IO<C> BindAsync<C>(Func<B, ValueTask<K<IO, C>>> f) => 
        new IOTimeout<A, C>(Fa, TimeLimit, x => Next(x).As().BindAsync(f));

    public override IO<B> Invoke(EnvIO envIO)
    {
        using var localEnv = envIO.LocalCancelWithTimeout(TimeLimit);
        var       r        = Fa.Run(localEnv);
        return +Next(r);
    }
}

using System;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.DSL;

record IOFinal<X, A, B>(K<IO, A> Fa, K<IO, X> Final, Func<A, K<IO, B>> Next) : InvokeSyncIO<B>
{
    public override IO<C> Map<C>(Func<B, C> f) => 
        new IOFinal<X,A,C>(Fa, Final, x => Next(x).Map(f));

    public override IO<C> Bind<C>(Func<B, K<IO, C>> f) => 
        new IOFinal<X,A,C>(Fa, Final, x => Next(x).Bind(f));

    public override IO<B> Invoke(EnvIO envIO)
    {
        var finalShouldRun = true;
        try
        {
            var task = Fa.RunAsync(envIO);
            finalShouldRun = false;
            if (task.IsCompleted)
            {
                return Final.As().Bind(_ => Next(task.Result));
            }
            else
            {
                return new IOFinalAsync<X, A, B>(task, Final, Next);
            }
        }
        finally
        {
            if(finalShouldRun) Final.As().Run(envIO);
        }
    }
    
    public override string ToString() => 
        "IO final";
}

record IOFinalAsync<X, A, B>(ValueTask<A> Fa, K<IO, X> Final, Func<A, K<IO, B>> Next) : InvokeAsyncIO<B>
{
    public override IO<C> Map<C>(Func<B, C> f) => 
        new IOFinalAsync<X, A, C>(Fa, Final, x => Next(x).Map(f));

    public override IO<C> Bind<C>(Func<B, K<IO, C>> f) => 
        new IOFinalAsync<X, A, C>(Fa, Final, x => Next(x).Bind(f));

    public override async ValueTask<IO<B>> Invoke(EnvIO envIO)
    {
        try
        {
            var value = await Fa;
            return Next(value).As();
        }
        finally
        {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            await Final.RunAsync(envIO);
        }
    }
    
    public override string ToString() => 
        "IO final async";
}

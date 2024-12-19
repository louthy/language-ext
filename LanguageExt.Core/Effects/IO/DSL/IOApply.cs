using System;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.DSL;

record IOApply<A, B, C>(K<IO, Func<A, B>> Ff, K<IO, A> Fa, Func<B, K<IO, C>> Next) : InvokeAsyncIO<C>
{
    public override IO<D> Map<D>(Func<C, D> f) => 
        new IOApply<A, B, D>(Ff, Fa, x => Next(x).As().Map(f));

    public override IO<D> Bind<D>(Func<C, K<IO, D>> f) => 
        new IOApply<A, B, D>(Ff, Fa, x => Next(x).As().Bind(f));

    public override async ValueTask<IO<C>> Invoke(EnvIO envIO)
    {
        var tf = Ff.RunAsync(envIO).AsTask();
        var ta = Fa.RunAsync(envIO).AsTask();
        await Task.WhenAll(tf, ta);
        return Next(tf.Result(ta.Result)).As();
    }
}

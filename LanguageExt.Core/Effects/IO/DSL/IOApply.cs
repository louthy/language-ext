using System;
using System.Threading.Tasks;

namespace LanguageExt.DSL;

record IOApply<A, B, C>(IO<Func<A, B>> ff, IO<A> fa, Func<B, C> Next) : DslInvokeIOAsync<C>
{
    public override IODsl<D> Map<D>(Func<C, D> f) =>
        IODsl.Apply(ff, fa, x => f(Next(x)));

    public override async ValueTask<C> Invoke(EnvIO envIO)
    {
        var tf = ff.RunAsync(envIO).AsTask();
        var ta = fa.RunAsync(envIO).AsTask();
        await Task.WhenAll(tf, ta);
        return Next(tf.Result(ta.Result));
    }
}

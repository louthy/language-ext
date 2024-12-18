using System;
using System.Threading.Tasks;
using LanguageExt.DSL;
using LanguageExt.Traits;

namespace LanguageExt;

record IOPureAsync<A>(Task<A> Value) : IO<A>
{
    public override IO<B> Map<B>(Func<A, B> f) =>
        new IOPureAsync<B>(Value.Map(f));

    public override IO<B> Bind<B>(Func<A, K<IO, B>> f) =>
        new IOPureAsync<K<IO, B>>(Value.Map(f)).Bind(Prelude.identity);
    
    public override IO<B> ApplyBack<B>(K<IO, Func<A, B>> mf) =>
        mf switch
        {
            IOPure<Func<A, B>> (var f)       => new IOPureAsync<B>(Apply(f, Value)),
            IOPureAsync<Func<A, B>> (var tf) => new IOPureAsync<B>(Apply(tf, Value)), 
            IOFail<Func<A, B>> (var v)       => new IOFail<B>(v),
            IOBind<Func<A, B>> (var f)       => new IOBind<B>(f.Map(f1 => f1.Apply(this).As())),
            IOCatch<Func<A, B>> mc           => mc.Bind(f => new IOPureAsync<B>(Value.Map(f))),
            _                                => throw new InvalidOperationException()
        };

    static async Task<B> Apply<B>(Func<A, B> f, Task<A> ta)
    {
        var a = await ta;
        return f(a);
    }

    static async Task<B> Apply<B>(Task<Func<A, B>> f, Task<A> a)
    {
        await Task.WhenAll(f, a);
        return f.Result(a.Result);
    }
}

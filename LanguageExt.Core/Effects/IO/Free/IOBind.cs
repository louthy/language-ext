using System;
using LanguageExt.DSL;
using LanguageExt.Traits;

namespace LanguageExt;

record IOBind<A>(IODsl<IO<A>> Value) : IO<A>
{
    public override IO<B> Map<B>(Func<A, B> f) =>
        new IOBind<B>(Value.Map(fa => fa.Map(f)));

    public override IO<B> Bind<B>(Func<A, K<IO, B>> f) =>
        new IOBind<B>(Value.Map(mx => mx.Bind(f)));

    public override IO<B> ApplyBack<B>(K<IO, Func<A, B>> mf) =>
        mf switch
        {
            IOPure<Func<A, B>> (var f) => new IOBind<B>(Value.Map(fa => fa.Map(f))),
            IOPureAsync<Func<A, B>> f  => new IOBind<B>(Value.Map(fa => fa.ApplyBack(f))),
            IOFail<Func<A, B>> (var v) => new IOFail<B>(v),
            IOBind<Func<A, B>> (var f) => new IOBind<B>(f.Map(x => x.Apply(this).As())),
            IOCatch<Func<A, B>> mc     => new IOBind<B>(Value.Map(fa => fa.ApplyBack(mc))),
            _                          => throw new InvalidOperationException()
        };
}

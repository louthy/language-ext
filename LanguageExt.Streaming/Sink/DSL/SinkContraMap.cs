using System;
using LanguageExt.Common;

namespace LanguageExt;

record SinkContraMap<A, B>(Func<B, A> F, Sink<A> Sink) : Sink<B>
{
    public override Sink<C> Comap<C>(Func<C, B> f) =>
        new SinkContraMap<A, C>(x => F(f(x)), Sink);

    public override IO<Unit> Post(B value) => 
        Sink.Post(F(value));

    public override IO<Unit> Complete() => 
        Sink.Complete();

    public override IO<Unit> Fail(Error Error) => 
        Sink.Fail(Error);
}

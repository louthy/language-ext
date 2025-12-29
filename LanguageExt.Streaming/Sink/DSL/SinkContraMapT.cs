using System;
using System.Threading.Channels;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt;

record SinkContraMapT<A, B>(Transducer<B, A> F, Sink<A> Sink) : Sink<B>
{
    public override Sink<C> Comap<C>(Func<C, B> f) =>
        new SinkContraMapT<A, C>(Transducer.map(f).Compose(F), Sink);

    public override Sink<C> Comap<C>(Transducer<C, B> f) => 
        new SinkContraMapT<A, C>(f.Compose(F), Sink);

    public override IO<Unit> Post(B value) =>
        F.Reduce<Unit>((_, a) => Sink.Post(a).Map(Reduced.Continue))(unit, value)
         .Map(x => x.Value);

    public override IO<Unit> Complete() => 
        Sink.Complete();

    public override IO<Unit> Fail(Error Error) => 
        Sink.Fail(Error);
}

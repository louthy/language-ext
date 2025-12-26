using System;
using LanguageExt.Common;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

record SinkTContraMapT<M, A, B>(TransducerM<M, B, A> F, SinkT<M, A> Sink) : SinkT<M, B>
    where M : MonadIO<M>
{
    public override SinkT<M, C> Comap<C>(Func<C, B> f) =>
        new SinkTContraMapT<M, A, C>(TransducerM.map<M, C, B>(f).Compose(F), Sink);

    public override SinkT<M, C> Comap<C>(TransducerM<M, C, B> f) => 
        new SinkTContraMapT<M, A, C>(f.Compose(F), Sink);

    public override K<M, Unit> PostM(K<M, B> mb) =>
        mb.Bind(b => F.Reduce<Unit>((_, a) => Sink.PostM(M.Pure(a)) * Reduced.Continue)(unit, b))
          .Map(r => r.Value);

    public override K<M, Unit> Complete() => 
        Sink.Complete();

    public override K<M, Unit> Fail(Error Error) => 
        Sink.Fail(Error);
}

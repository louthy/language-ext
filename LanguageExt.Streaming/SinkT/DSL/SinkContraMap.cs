using System;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

record SinkTContraMap<M, A, B>(Func<B, A> F, SinkT<M, A> Sink) : SinkT<M, B>
    where M : MonadIO<M>
{
    public override SinkT<M, C> Comap<C>(Func<C, B> f) =>
        new SinkTContraMap<M, A, C>(x => F(f(x)), Sink);

    public override K<M, Unit> PostM(K<M, B> ma) => 
        Sink.PostM(ma.Map(F));

    public override K<M, Unit> Complete() => 
        Sink.Complete();

    public override K<M, Unit> Fail(Error Error) => 
        Sink.Fail(Error);
}

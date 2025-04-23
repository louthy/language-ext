using System;
using LanguageExt.Common;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

record SinkTVoid<M, A> : SinkT<M, A>
    where M : MonadIO<M>
{
    public static readonly SinkT<M, A> Default = new SinkTVoid<M, A>();
    
    public override SinkT<M, B> Comap<B>(Func<B, A> f) => 
        SinkTVoid<M, B>.Default;

    public override K<M, Unit> PostM(K<M, A> ma) =>
        M.Pure(unit);

    public override K<M, Unit> Complete() =>
        M.Pure(unit);

    public override K<M, Unit> Fail(Error error) =>
        M.Pure(unit);
}

using System;
using LanguageExt.Common;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

record SinkTEmpty<M, A> : SinkT<M, A>
    where M : MonadIO<M>
{
    public static readonly SinkT<M, A> Default = new SinkTEmpty<M, A>();
    
    public override SinkT<M, B> Comap<B>(Func<B, A> f) => 
        SinkTEmpty<M, B>.Default;

    public override K<M, Unit> PostM(K<M, A> ma) =>
        M.LiftIO(IO.fail<Unit>(Errors.SinkFull));

    public override K<M, Unit> Complete() =>
        M.Pure(unit);

    public override K<M, Unit> Fail(Error error) =>
        M.Pure(unit);
}

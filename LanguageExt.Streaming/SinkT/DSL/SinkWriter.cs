using System;
using System.Threading.Channels;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

record SinkTWriter<M, A>(Channel<K<M, A>> Channel) : SinkT<M, A>
    where M : MonadIO<M>
{
    public override SinkT<M, B> Comap<B>(Func<B, A> f) => 
        new SinkTContraMap<M, A, B>(f, this);

    public override K<M, Unit> PostM(K<M, A> ma) =>
        M.LiftIO(from f in IO.liftVAsync(e => Channel.Writer.WaitToWriteAsync(e.Token))
                 from r in f
                               ? IO.liftVAsync(() => Channel.Writer.WriteAsync(ma).ToUnit())
                               : IO.fail<Unit>(Errors.SinkFull)
                 select r);

    public override K<M, Unit> Complete() =>
        M.LiftIO(IO.lift(() => Channel.Writer.Complete()));    

    public override K<M, Unit> Fail(Error error) =>
        M.LiftIO(IO.lift(() => Channel.Writer.Complete(error.ToException())));    
}

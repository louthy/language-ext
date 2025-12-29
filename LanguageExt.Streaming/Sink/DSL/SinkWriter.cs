using System;
using System.Threading.Channels;
using LanguageExt.Common;

namespace LanguageExt;

record SinkWriter<A>(Channel<A> Channel) : Sink<A>
{
    public override Sink<B> Comap<B>(Func<B, A> f) => 
        new SinkContraMap<A, B>(f, this);

    public override IO<Unit> Post(A value) =>
        from f in IO.liftVAsync(e => Channel.Writer.WaitToWriteAsync(e.Token))
        from r in f ? IO.liftVAsync(() => Channel.Writer.WriteAsync(value).ToUnit())
                    : IO.fail<Unit>(Errors.SinkFull)
        select r;

    public override IO<Unit> Complete() =>
        IO.lift(() => Channel.Writer.Complete());    

    public override IO<Unit> Fail(Error error) =>
        IO.lift(() => Channel.Writer.Complete(error.ToException()));    
}

/*
using System.Threading.Channels;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.Pipes.Concurrent;

public record SourceTIterator<M, A>(SourceT<M, A> Source) 
    where M : MonadIO<M>, Alternative<M>
{
    internal static SourceTIterator<M, A> Create(SourceT<M, A> source)
    {
        var channel = Channel.CreateUnbounded<K<M, A>>();
        var ms = source.ReduceM(unit, (_, ma) => M.LiftIO(IO.liftVAsync(e => channel.Writer.WaitToWriteAsync(e.Token)))
                                                  .Map(f => f ? channel.Writer.TryWrite(ma).Ignore()
                                                              : channel.Writer.TryComplete().Ignore()));

    }

    public void Run()
    {
        var channel = Channel.CreateUnbounded<K<M, A>>();
        var ms = Source.ReduceM(unit, (_, ma) => M.LiftIO(IO.liftVAsync(e => channel.Writer.WaitToWriteAsync(e.Token)))
                                                  .Map(f => f ? channel.Writer.TryWrite(ma).Ignore()
                                                              : channel.Writer.TryComplete().Ignore()));
        
        
    }
}
*/

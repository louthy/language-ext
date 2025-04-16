using LanguageExt.Traits;
using System.Threading.Channels;
using static LanguageExt.Prelude;

namespace LanguageExt.Pipes.Concurrent;

record Zip2SourceT<M, A, B>(SourceT<M, A> SourceA, SourceT<M, B> SourceB) : SourceT<M, (A First, B Second)>
    where M : MonadIO<M>, Alternative<M>
{
    internal override SourceTIterator<M, (A First, B Second)> GetIterator()
    {
        var channelA = Channel.CreateUnbounded<K<M, A>>();
        var channelB = Channel.CreateUnbounded<K<M, B>>();
        
        var writerA = channelA.Writer;
        var writerB = channelB.Writer;

        var forks = from signal in M.Pure(unit)
                    
                    let triggerA = trigger<A>(writerA)
                    let triggerB = trigger<B>(writerB)
                    
                    from resultA in SourceA.ReduceM(unit, (_, ma) => writeAsync(writerA, ma))
                                           .Bind(_ => triggerA)
                                           .Choose(triggerA)
                                           .ForkIO()
                    
                    from resultB in SourceB.ReduceM(unit, (_, ma) => writeAsync(writerB, ma))
                                           .Bind(_ => triggerB)
                                           .Choose(triggerB)
                                           .ForkIO()

                    select unit;

        // First, create a singleton source that will run the forks
        var single = new LiftSourceT<M, Unit>(forks);

        // Then create a reader iterator that will yield the merged values 
        var reader = new Reader2SourceTIterator<M, A, B>(channelA.Reader, channelB.Reader);

        // Then bind them together so the forks launch, then the reader reads
        return new BindSourceTIterator<M, Unit, (A, B)>(single.GetIterator(), _ => reader);
    }

    static K<M, Unit> trigger<X>(ChannelWriter<K<M, X>> writer) =>
        M.LiftIO(IO.lift(() => writer.TryComplete().Ignore()));

    static K<M, Unit> writeAsync<X>(ChannelWriter<X> writer, X value) =>
        M.LiftIO(IO.liftVAsync(e => writer.WriteAsync(value, e.Token).ToUnit()));
}

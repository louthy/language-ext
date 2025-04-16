using LanguageExt.Traits;
using System.Threading.Channels;
using static LanguageExt.Prelude;

namespace LanguageExt.Pipes.Concurrent;

record ChooseSourceT<M, A>(Seq<SourceT<M, A>> Sources) : SourceT<M, A>
    where M : MonadIO<M>, Alternative<M>
{
    internal override SourceTIterator<M, A> GetIterator() =>
        ChooseSourceT.makeIterator(Sources);
}

static class ChooseSourceT
{
    public static SourceTIterator<M, A> makeIterator<M, A>(Seq<SourceT<M, A>> sources)
        where M : MonadIO<M>, Alternative<M> =>
        makeIterator(sources.Map(s => s.GetIterator()));
    
    public static SourceTIterator<M, A> makeIterator<M, A>(Seq<SourceTIterator<M, A>> sources)
        where M : MonadIO<M>, Alternative<M>
    {
        // Create a channel for the merged streams
        var channel = Channel.CreateUnbounded<K<M, A>>();
        var writer  = channel.Writer;

        // For each stream, reduce it (which writes to the merged stream), fork it, and return
        // This gives us K<M, A> that we can't run directly, so we must bind it...
        var forks = from signal in Signal.countdown<M>(sources.Count)
                    let trigger = trigger<M, A>(signal, writer)
                    from result in sources.Map(s => s.ReduceM(unit, (_, ma) => writeAsync<M, K<M, A>>(writer, ma))
                                                     .Bind(_ => trigger)
                                                     .Choose(trigger))
                                          .Traverse(ms => ms.ForkIO())
                                          .ForkIO()
                                          .Map(unit)
                    select result;

        // First, create a singleton source that will run the forks
        var single = new LiftSourceT<M, Unit>(forks);

        // Then create a reader iterator that will yield the merged values 
        var reader = new ReaderSourceTIterator<M, A>(channel.Reader);

        // Then bind them together so the forks launch, then the reader reads
        return new BindSourceTIterator<M, Unit, A>(single.GetIterator(), _ => reader);
    }

    static K<M, Unit> trigger<M, A>(CountdownSignal<M> signal, ChannelWriter<K<M, A>> writer)
        where M : MonadIO<M>, Alternative<M> =>
        from f in signal.Trigger()
        let _ = !f || writer.TryComplete()  // Mark channel completed if all sources are complete
        select unit;
    
    static K<M, Unit> writeAsync<M, X>(ChannelWriter<X> writer, X value) 
        where M : MonadIO<M>, Alternative<M> =>
        M.LiftIO(IO.liftVAsync(e => writer.WriteAsync(value, e.Token).ToUnit()));
}

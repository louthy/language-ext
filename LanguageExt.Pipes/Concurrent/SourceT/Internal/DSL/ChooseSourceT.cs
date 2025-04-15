using LanguageExt.Traits;
using System.Threading.Channels;
using static LanguageExt.Prelude;

namespace LanguageExt.Pipes.Concurrent;

record ChooseSourceT<M, A>(Seq<SourceT<M, A>> Sources) : SourceT<M, A>
    where M : MonadIO<M>, Alternative<M>
{
    internal override SourceTIterator<M, A> GetIterator() =>
        ChooseSourceT.MakeIterator(Sources);
}

static class ChooseSourceT
{
    public static SourceTIterator<M, A> MakeIterator<M, A>(Seq<SourceT<M, A>> sources)
        where M : MonadIO<M>, Alternative<M> =>
        MakeIterator(sources.Map(s => s.GetIterator()));
    
    public static SourceTIterator<M, A> MakeIterator<M, A>(Seq<SourceTIterator<M, A>> sources)
        where M : MonadIO<M>, Alternative<M>
    {
        // Create a channel for the merged streams
        var channel = Channel.CreateUnbounded<K<M, A>>();

        // For each stream, reduce it (which writes to the merged stream), fork it, and return
        // This gives us K<M, A> that we can't run directly, so we must bind it...
        var envIO = EnvIO.New();
        var forks = sources.Map(s => s.ReduceM(unit, (_, ma) => writeAsync<M, K<M, A>>(channel, ma)))
                           .Traverse(ms => ms.ForkIO())
                           .ForkIO()
                           .Map(unit);

        // First, create a singleton source that will run the forks
        var single = new LiftSourceT<M, Unit>(forks);

        // Then create a reader iterator that will yield the merged values 
        var reader = new MergedReaderSourceTIterator<M, A>(channel.Reader, envIO);

        // Then bind them together so the forks launch, then the reader reads
        return new BindSourceTIterator<M, Unit, A>(single.GetIterator(), _ => reader);
    }
    
    static K<M, Unit> writeAsync<M, X>(Channel<X> channel, X value) 
        where M : MonadIO<M>, Alternative<M> =>
        M.LiftIO(IO.liftVAsync(e => channel.Writer.WriteAsync(value, e.Token).ToUnit()));
}

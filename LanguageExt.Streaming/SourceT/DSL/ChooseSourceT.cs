using LanguageExt.Traits;
using System.Threading.Channels;
using static LanguageExt.Prelude;

namespace LanguageExt;

record ChooseSourceT<M, A>(Seq<SourceT<M, A>> Sources) : SourceT<M, A>
    where M : MonadIO<M>, Alternative<M>
{
    public override K<M, Reduced<S>> ReduceInternalM<S>(S state, ReducerM<M, K<M, A>, S> reducer) =>
        // Create a channel for the merged streams
        from channel in M.Pure(Channel.CreateUnbounded<K<M, A>>())
        let writer = channel.Writer

        // For each stream, reduce it (which writes to the merged stream), fork it, and return
        // This gives us K<M, A> that we can't run directly, so we must bind it...
        from signal in Signal.countdown<M>(Sources.Count)
        let trigger =  trigger(signal, writer)
        from forks  in Sources.Map(s => s.ReduceInternalM(unit, (_, ma) => writeAsync(writer, ma))
                                         .Bind(_ => trigger)
                                         .Choose(trigger))
                              .Traverse(ms => M.ForkIOMaybe(ms))

        // Reduce the merged stream
        from result in SourceT.liftM<M, A>(channel).ReduceInternalM(state, reducer)
        
        // Make sure the forks are shutdown
        from _      in M.LiftIOMaybe(forks.Traverse(f => f.Cancel))
        
        select result;

    static K<M, Unit> trigger(CountdownSignal<M> signal, ChannelWriter<K<M, A>> writer) =>
        from f in signal.Trigger()
        let _ = !f || writer.TryComplete()  // Mark channel completed if all sources are complete
        select unit;
    
    static K<M, Reduced<Unit>> writeAsync<X>(ChannelWriter<X> writer, X value) =>
        M.LiftIO(IO.liftVAsync(async e =>
                               {
                                    await writer.WriteAsync(value, e.Token);
                                    return Reduced.Continue(unit);
                               }));
}

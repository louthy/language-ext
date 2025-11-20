using LanguageExt.Traits;
using System.Threading.Channels;
using static LanguageExt.Prelude;

namespace LanguageExt;

record Zip2SourceT<M, A, B>(SourceT<M, A> SourceA, SourceT<M, B> SourceB) : SourceT<M, (A First, B Second)>
    where M : MonadIO<M>, Alternative<M>
{
    public override K<M, S> ReduceM<S>(S state, ReducerM<M, K<M, (A First, B Second)>, S> reducer) =>
        
        // Create channels that receive the values yielded by the two sources
        from channelA in M.Pure(Channel.CreateUnbounded<K<M, A>>())
        from channelB in M.Pure(Channel.CreateUnbounded<K<M, B>>())
        let writerA = channelA.Writer
        let writerB = channelB.Writer

        // Triggers which signal when a channel has completed
        let triggerA = trigger<A>(writerA)
        let triggerB = trigger<B>(writerB)

        // Create a forked first channel                        
        from forkA in SourceA.ReduceM(unit, (_, ma) => writeAsync(writerA, ma))
                             .Bind(_ => triggerA)
                             .Choose(triggerA)
                             .ForkIOMaybe()

        // Create a forked second channel                        
        from forkB in SourceB.ReduceM(unit, (_, ma) => writeAsync(writerB, ma))
                             .Bind(_ => triggerB)
                             .Choose(triggerB)
                             .ForkIOMaybe()

        // Then create a reader iterator that will yield the merged values 
        from result in new Reader2SourceT<M, A, B>(channelA, channelB).ReduceM(state, reducer)
        
        // Make sure the forks are shutdown
        from _      in M.LiftIOMaybe(Seq(forkA, forkB).Traverse(f => f.Cancel))

        select result;

    static K<M, Unit> trigger<X>(ChannelWriter<K<M, X>> writer) =>
        M.LiftIOMaybe(IO.lift(() => writer.TryComplete().Ignore()));

    static K<M, Unit> writeAsync<X>(ChannelWriter<X> writer, X value) =>
        M.LiftIOMaybe(IO.liftVAsync(e => writer.WriteAsync(value, e.Token).ToUnit()));
}

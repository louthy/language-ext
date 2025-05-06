using static LanguageExt.Prelude;
using System.Threading.Channels;
using LanguageExt.Traits;

namespace LanguageExt;

record Zip3SourceT<M, A, B, C>(
    SourceT<M, A> SourceA, 
    SourceT<M, B> SourceB, 
    SourceT<M, C> SourceC) : 
    SourceT<M, (A First, B Second, C Third)>
    where M : MonadUnliftIO<M>, Alternative<M>
{
    public override K<M, S> ReduceM<S>(S state, ReducerM<M, K<M, (A First, B Second, C Third)>, S> reducer) =>
        
        // Create channels that receive the values yielded by the two sources
        from channelA in M.Pure(Channel.CreateUnbounded<K<M, A>>())
        from channelB in M.Pure(Channel.CreateUnbounded<K<M, B>>())
        from channelC in M.Pure(Channel.CreateUnbounded<K<M, C>>())
        let writerA = channelA.Writer
        let writerB = channelB.Writer
        let writerC = channelC.Writer

        // Triggers which signal when a channel has completed
        let triggerA = trigger<A>(writerA)
        let triggerB = trigger<B>(writerB)
        let triggerC = trigger<C>(writerC)

        // Create a forked first channel                        
        from forkA in SourceA.ReduceM(unit, (_, ma) => writeAsync(writerA, ma))
                             .Bind(_ => triggerA)
                             .Choose(triggerA)
                             .ForkIO()

        // Create a forked second channel                        
        from forkB in SourceB.ReduceM(unit, (_, ma) => writeAsync(writerB, ma))
                             .Bind(_ => triggerB)
                             .Choose(triggerB)
                             .ForkIO()

        // Create a forked third channel                        
        from forkC in SourceC.ReduceM(unit, (_, ma) => writeAsync(writerC, ma))
                             .Bind(_ => triggerC)
                             .Choose(triggerC)
                             .ForkIO()

        // Then create a reader iterator that will yield the merged values 
        from result in new Reader3SourceT<M, A, B, C>(channelA, channelB, channelC).ReduceM(state, reducer)

        // Make sure the forks are shutdown
        from _      in M.LiftIO(Seq(forkA, forkB, forkC).Traverse(f => f.Cancel))

        select result;
    
    static K<M, Unit> trigger<X>(ChannelWriter<K<M, X>> writer) =>
        M.LiftIO(IO.lift(() => writer.TryComplete().Ignore()));

    static K<M, Unit> writeAsync<X>(ChannelWriter<X> writer, X value) =>
        M.LiftIO(IO.liftVAsync(e => writer.WriteAsync(value, e.Token).ToUnit()));    
}

using System;
using static LanguageExt.Prelude;
using System.Threading.Channels;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

record Zip3SourceT<M, A, B, C>(
    SourceT<M, A> SourceA, 
    SourceT<M, B> SourceB, 
    SourceT<M, C> SourceC) : 
    SourceT<M, (A First, B Second, C Third)>
    where M : MonadIO<M>, Fallible<Error, M>
{
    internal override K<M, Reduced<S>> ReduceInternalM<S>(S state, ReducerM<M, K<M, (A First, B Second, C Third)>, S> reducer) =>
        
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
        let error    = error(writerA, writerB, writerC)

        // Create a forked first channel                        
        from forkA in SourceA.ReduceInternalM(unit, (_, ma) => writeAsync(writerA, ma))
                             .Bind(_ => triggerA)
                             .Catch(error)
                             .ForkIOMaybe()

        // Create a forked second channel                        
        from forkB in SourceB.ReduceInternalM(unit, (_, ma) => writeAsync(writerB, ma))
                             .Bind(_ => triggerB)
                             .Catch(error)
                             .ForkIOMaybe()

        // Create a forked third channel                        
        from forkC in SourceC.ReduceInternalM(unit, (_, ma) => writeAsync(writerC, ma))
                             .Bind(_ => triggerC)
                             .Catch(error)
                             .ForkIOMaybe()

        let forks = Seq(forkA, forkB, forkC)

        // Then create a reader iterator that will yield the merged values 
        from result in new Reader3SourceT<M, A, B, C>(channelA, channelB, channelC).ReduceInternalM(state, reducer)

        // Make sure the forks are shutdown
        from _      in M.LiftIOMaybe(forks.Traverse(f => f.Cancel))

        // Await all of the values - this should not yield anything useful unless an error occurred.
        from rs     in M.LiftIO(forks.Traverse(f => f.Await))

        select result;
    
    static K<M, Unit> trigger<X>(ChannelWriter<K<M, X>> writer) =>
        M.LiftIOMaybe(IO.lift(() => writer.TryComplete().Ignore()));
    
    static Func<Error, K<M, Unit>> error(
        ChannelWriter<K<M, A>> writerA,
        ChannelWriter<K<M, B>> writerB,
        ChannelWriter<K<M, C>> writerC) =>
        err => M.LiftIO(
                     IO.lift(e => 
                             { 
                                 writerA.TryComplete(); 
                                 writerB.TryComplete(); 
                                 writerC.TryComplete(); 
                                 return unit; 
                             }))
                    .Bind(_ => M.Fail<Unit>(err));

    static K<M, Reduced<Unit>> writeAsync<X>(ChannelWriter<X> writer, X value) =>
        M.LiftIO(IO.liftVAsync(async e =>
                               {
                                   await writer.WriteAsync(value, e.Token);
                                   return Reduced.Continue(unit);
                               }));   
}

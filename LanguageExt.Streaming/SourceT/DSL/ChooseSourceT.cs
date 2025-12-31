using System;
using System.Threading;
using LanguageExt.Traits;
using System.Threading.Channels;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt;

record ChooseSourceT<M, A>(Seq<SourceT<M, A>> Sources) : SourceT<M, A>
    where M : MonadIO<M>, Fallible<M> 
{
    internal override K<M, Reduced<S>> ReduceInternalM<S>(S state, ReducerM<M, K<M, A>, S> reducer) =>
        M.BracketIOMaybe(
            
            // Get our environment
            from envIO  in IO.token

            // Create a channel for the merged streams
            from channel in M.Pure(Channel.CreateUnbounded<K<M, A>>())
            let writer = channel.Writer

            // For each stream, reduce it (which writes to the merged stream), fork it, and return
            // This gives us K<M, A> that we can't run directly, so we must bind it...
            from signal in useMaybe(Signal.countdown<M>(Sources.Count))
            let trigger = trigger(signal, writer)
            let error   = error(signal, writer)
            
            from forks  in useMaybe(Sources.Map(s => s.ReduceInternalM(unit, (_, ma) => M.Pure(writeAsync(writer, ma)))
                                                      .Bind(_ => trigger)
                                                      .Catch(error)
                                                      .ForkIOMaybe())
                                           .Sequence()
                                           .Map(ForkRelease.New))

            // Reduce the merged stream
            from result in SourceT.liftM<M, A>(channel).ReduceInternalM(state, reducer)
            
            // Make sure the forks are shutdown
            from _      in forks.Cancel()

            select result);

    static Func<Error, K<M, Unit>> error(CountdownSignal<M> signal, ChannelWriter<K<M, A>> writer) =>
        err => signal.Count
                     .Bind(signal.Trigger)
                     .Map(_ => writer.TryComplete())
                     .Bind(_ => M.Fail<Unit>(err));

    static K<M, Unit> trigger(CountdownSignal<M> signal, ChannelWriter<K<M, A>> writer) =>
        // Mark channel completed if all sources are complete
        (f => ignore(f && writer.TryComplete())) * signal.Trigger();

    static Reduced<Unit> writeAsync<X>(ChannelWriter<K<M, X>> writer, K<M, X> value) =>
        writer.TryWrite(value)
            ? Reduced.Unit
            : Reduced.Done(unit);

    class ForkRelease(K<Seq, ForkIO<Unit>> Forks) : IDisposable
    {
        int disposed;
        public static ForkRelease New(K<Seq, ForkIO<Unit>> forks) =>
            new (forks);

        public IO<Unit> Cancel() =>
            IO.lift(e =>
                    {
                        if (Interlocked.CompareExchange(ref disposed, 1, 0) == 0)
                        {
                            foreach (var fork in +Forks)
                            {
                                fork.Cancel.Run(e);
                            }
                        }
                        return unit;
                    });

        public void Dispose() =>
            Cancel().Run();
    }
}

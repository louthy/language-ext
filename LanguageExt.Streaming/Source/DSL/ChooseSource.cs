using System;
using System.Threading;
using System.Threading.Channels;
using static LanguageExt.Prelude;

namespace LanguageExt;

record ChooseSource<A>(Seq<Source<A>> Sources) : Source<A>
{
    internal override IO<Reduced<S>> ReduceInternal<S>(S state, ReducerIO<A, S> reducer) =>
        from channel in createChannel
        let writer   =  channel.Writer
        let signal   =  new CountdownEvent(Sources.Count)
        let trigger  =  triggerF(signal, writer)
        from forks   in Sources.Map(s => IO.liftAsync(async e =>
                                           {
                                               try
                                               {
                                                   await s.Reduce(unit, 
                                                                  (_, a) => e.Token.IsCancellationRequested
                                                                                ? Reduced.Done(unit) 
                                                                                : writeAsync(writer, a))
                                                          .RunAsync(e);
                                               }
                                               finally
                                               {
                                                   trigger();
                                               }

                                               return unit;
                                           }))
                            .Traverse(s => s.Fork())

        from nstate in Source.lift(channel).ReduceInternal(state, reducer)
        from _      in forks.Traverse(f => f.Cancel).Map(s => s.Strict())
        select nstate;
    
    IO<Channel<A>> createChannel =>
        IO.lift(Channel.CreateUnbounded<A>);

    static Func<Unit> triggerF(CountdownEvent signal, ChannelWriter<A> writer) =>
        () =>
        {
            if (signal.Signal())
            {
                writer.TryComplete();
            }
            return default;
        };

    static Reduced<Unit> writeAsync<X>(ChannelWriter<X> writer, X value) =>
        writer.TryWrite(value)
            ? Reduced.Unit
            : Reduced.Done(unit);
}

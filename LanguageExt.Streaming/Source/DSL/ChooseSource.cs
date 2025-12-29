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
                                                           await s.ReduceIO(unit, (_, ma) => writeAsync(writer, ma))
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

    static IO<Reduced<Unit>> writeAsync<X>(ChannelWriter<X> writer, X value) =>
        IO.liftVAsync(async e =>
                      {
                          if (e.Token.IsCancellationRequested)
                          {
                              return Reduced.Done(unit);
                          }
                          else
                          {
                              await writer.WriteAsync(value, e.Token);
                              return Reduced.Continue(unit);
                          }
                      });
}

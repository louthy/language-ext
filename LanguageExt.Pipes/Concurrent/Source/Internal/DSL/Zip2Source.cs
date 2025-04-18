using System.Threading;
using System.Threading.Tasks;
using static LanguageExt.Prelude;
using System.Threading.Channels;

namespace LanguageExt.Pipes.Concurrent;

record Zip2Source<A, B>(Source<A> SourceA, Source<B> SourceB) : Source<(A First, B Second)>
{
    internal override async ValueTask<Reduced<S>> ReduceAsync<S>(
        S state, 
        ReducerAsync<(A First, B Second), S> reducer,
        CancellationToken token)
    {
        // Create channels that receive the values yielded by the two sources
        var channelA = Channel.CreateUnbounded<A>();
        var channelB = Channel.CreateUnbounded<B>();
        var writerA  = channelA.Writer;
        var writerB  = channelB.Writer;

        // Create a forked first channel                        
        var taskA = SourceA.ReduceAsync(unit, async (_, a) =>
                                              {
                                                  if (await writerA.WaitToWriteAsync(token))
                                                  {
                                                      await writerA.WriteAsync(a, token);
                                                      return Reduced.Unit;
                                                  }
                                                  else
                                                  {
                                                      writerA.TryComplete();
                                                      return Reduced.Done(unit);
                                                  }
                                              }, token)
                           .Map(r =>
                                {
                                    writerA.TryComplete();
                                    return r;
                                });

        // Create a forked second channel                        
        var taskB = SourceB.ReduceAsync(unit, async (_, b) =>
                                              {
                                                  if (await writerB.WaitToWriteAsync(token))
                                                  {
                                                      await writerB.WriteAsync(b, token);
                                                      return Reduced.Unit;
                                                  }
                                                  else
                                                  {
                                                      writerB.TryComplete();
                                                      return Reduced.Done(unit);
                                                  }
                                              }, token)
                           .Map(r =>
                                {
                                    writerB.TryComplete();
                                    return r;
                                });

        return await new Reader2Source<A, B>(channelA, channelB).ReduceAsync(state, reducer, token);
    }
}

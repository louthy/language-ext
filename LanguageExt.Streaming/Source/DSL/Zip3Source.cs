using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace LanguageExt;

record Zip3Source<A, B, C>(Source<A> SourceA, Source<B> SourceB, Source<C> SourceC) : Source<(A First, B Second, C Third)>
{
    internal override async ValueTask<Reduced<S>> ReduceAsync<S>(
        S state, 
        ReducerAsync<(A First, B Second, C Third), S> reducer, 
        CancellationToken token) 
    {
        
        // Create channels that receive the values yielded by the two sources
        var channelA = Channel.CreateUnbounded<A>();
        var channelB = Channel.CreateUnbounded<B>();
        var channelC = Channel.CreateUnbounded<C>();
        var writerA  = channelA.Writer;
        var writerB  = channelB.Writer;
        var writerC  = channelC.Writer;

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

        // Create a forked third channel                        
        var taskC = SourceC.ReduceAsync(unit, async (_, c) =>
                                              {
                                                  if (await writerC.WaitToWriteAsync(token))
                                                  {
                                                      await writerC.WriteAsync(c, token);
                                                      return Reduced.Unit;
                                                  }
                                                  else
                                                  {
                                                      writerC.TryComplete();
                                                      return Reduced.Done(unit);
                                                  }
                                              }, token)
                           .Map(r =>
                                {
                                    writerC.TryComplete();
                                    return r;
                                });

        return await new Reader3Source<A, B, C>(channelA, channelB, channelC).ReduceAsync(state, reducer, token);
    }        
}

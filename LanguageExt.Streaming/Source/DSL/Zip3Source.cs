using System.Threading.Channels;
using static LanguageExt.Prelude;

namespace LanguageExt;

record Zip3Source<A, B, C>(Source<A> SourceA, Source<B> SourceB, Source<C> SourceC) : Source<(A First, B Second, C Third)>
{
    internal override IO<Reduced<S>> ReduceInternal<S>(S state, ReducerIO<(A First, B Second, C Third), S> reducer) =>
        IO.liftVAsync(async e =>
          {

              // Create channels that receive the values yielded by the two sources
              var channelA = Channel.CreateUnbounded<A>();
              var channelB = Channel.CreateUnbounded<B>();
              var channelC = Channel.CreateUnbounded<C>();
              var writerA  = channelA.Writer;
              var writerB  = channelB.Writer;
              var writerC  = channelC.Writer;

              // Create a forked first channel                        
              var taskA = SourceA.ReduceInternal(
                                      unit,
                                      (_, a) => IO.liftVAsync(async e1 =>
                                                              {
                                                                  if (!e1.Token.IsCancellationRequested &&
                                                                      await writerA.WaitToWriteAsync(e1.Token))
                                                                  {
                                                                      await writerA.WriteAsync(a, e1.Token);
                                                                      return Reduced.Unit;
                                                                  }
                                                                  else
                                                                  {
                                                                      writerA.TryComplete();
                                                                      return Reduced.Done(unit);
                                                                  }
                                                              }))
                                 .Map(r =>
                                      {
                                          writerA.TryComplete();
                                          return r;
                                      })
                                 .RunAsync(e);

              // Create a forked second channel                        
              var taskB = SourceB.ReduceInternal(
                                      unit,
                                      (_, b) => IO.liftVAsync(async e1 =>
                                                              {
                                                                  if (!e1.Token.IsCancellationRequested &&
                                                                      await writerB.WaitToWriteAsync(e1.Token))
                                                                  {
                                                                      await writerB.WriteAsync(b, e1.Token);
                                                                      return Reduced.Unit;
                                                                  }
                                                                  else
                                                                  {
                                                                      writerB.TryComplete();
                                                                      return Reduced.Done(unit);
                                                                  }
                                                              }))
                                 .Map(r =>
                                      {
                                          writerB.TryComplete();
                                          return r;
                                      })
                                 .RunAsync(e);

              // Create a forked third channel                        
              var taskC = SourceC.ReduceInternal(
                                      unit,
                                      (_, c) => IO.liftVAsync(async e1 =>
                                                              {
                                                                  if (!e1.Token.IsCancellationRequested &&
                                                                      await writerC.WaitToWriteAsync(e1.Token))
                                                                  {
                                                                      await writerC.WriteAsync(c, e1.Token);
                                                                      return Reduced.Unit;
                                                                  }
                                                                  else
                                                                  {
                                                                      writerC.TryComplete();
                                                                      return Reduced.Done(unit);
                                                                  }
                                                              }))
                                 .Map(r =>
                                      {
                                          writerC.TryComplete();
                                          return r;
                                      })
                                 .RunAsync(e);

              return await new Reader3Source<A, B, C>(channelA, channelB, channelC)
                          .ReduceInternal(state, reducer)
                          .RunAsync(e);
          });
}

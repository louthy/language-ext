using static LanguageExt.Prelude;
using System.Threading.Channels;

namespace LanguageExt;

record Zip2Source<A, B>(Source<A> SourceA, Source<B> SourceB) : Source<(A First, B Second)>
{
    internal override IO<Reduced<S>> ReduceInternal<S>(S state, ReducerIO<(A First, B Second), S> reducer) =>
        IO.liftVAsync(async e =>
        {
            // Create channels that receive the values yielded by the two sources
            var channelA = Channel.CreateUnbounded<A>();
            var channelB = Channel.CreateUnbounded<B>();
            var writerA  = channelA.Writer;
            var writerB  = channelB.Writer;

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

            return await new Reader2Source<A, B>(channelA, channelB)
                        .ReduceInternal(state, reducer)
                        .RunAsync(e);
        });
}

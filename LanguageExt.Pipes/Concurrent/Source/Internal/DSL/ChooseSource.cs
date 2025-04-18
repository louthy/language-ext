using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace LanguageExt.Pipes.Concurrent;

record ChooseSource<A>(Seq<Source<A>> Sources) : Source<A>
{
    internal override async ValueTask<Reduced<S>> ReduceAsync<S>(S state, ReducerAsync<A, S> reducer, CancellationToken token)
    {
        var       channel = Channel.CreateUnbounded<A>();
        using var envIO   = EnvIO.New(token: token);
        var forks = Sources.Traverse(
                                s => s.ReduceAsync(unit, async (_, x) =>
                                                    {
                                                        if (envIO.Token.IsCancellationRequested)
                                                        {
                                                            channel.Writer.TryComplete();
                                                            return Reduced.Done(unit);
                                                        }

                                                        if (!await channel.Writer.WaitToWriteAsync(envIO.Token))
                                                        {
                                                            channel.Writer.TryComplete();
                                                            return Reduced.Done(unit);
                                                        }

                                                        await channel.Writer.WriteAsync(x);
                                                        if (envIO.Token.IsCancellationRequested)
                                                        {
                                                            channel.Writer.TryComplete();
                                                            return Reduced.Done(unit);
                                                        }
                                                        else
                                                        {
                                                            return Reduced.Continue(unit);
                                                        }
                                                    })
                                      .Fork())
                           .Run(envIO)
                           .Strict();

        var nstate = await Source.lift(channel).ReduceAsync(state, reducer, envIO.Token);
        (await forks.Traverse(f => f.Cancel).RunAsync(envIO)).Strict();
        return nstate;
    }
}

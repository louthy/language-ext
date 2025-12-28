using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

record ChooseSource<A>(Seq<Source<A>> Sources) : Source<A>
{
    internal override async ValueTask<Reduced<S>> ReduceAsync<S>(
        S state, 
        ReducerAsync<A, S> reducer,
        CancellationToken token)
    {
        // Create a channel for the merged streams
        var channel = Channel.CreateUnbounded<A>();
        var writer  = channel.Writer;

        var signal  = new CountdownEvent(Sources.Count);
        var trigger = triggerF(signal, writer);

        using var env = EnvIO.New(token: token);
        var forks = await Sources.Map(s => IO.liftAsync(async e =>
                                                   {
                                                       try
                                                       {
                                                           await s.ReduceAsync(unit, (_, ma) => writeAsync(writer, ma, e.Token))
                                                                  .RunAsync(e);
                                                       }
                                                       finally
                                                       {
                                                           trigger();
                                                       }

                                                       return unit;
                                                   }))
                            .Traverse(s => s.Fork())
                            .RunAsync(env);

        var nstate = await Source.lift(channel).ReduceAsync(state, reducer, token);
        (await forks.Traverse(f => f.Cancel).RunAsync(env)).Strict();
        return nstate;
    }

    static Func<Unit> triggerF(CountdownEvent signal, ChannelWriter<A> writer) =>
        () =>
        {
            if (signal.Signal())
            {
                writer.TryComplete();
            }
            return default;
        };

    static async ValueTask<Reduced<Unit>> writeAsync<X>(ChannelWriter<X> writer, X value, CancellationToken token) 
    {
        if (token.IsCancellationRequested)
        {
            return Reduced.Done(unit);
        }
        else
        {
            await writer.WriteAsync(value, token);
            return Reduced.Continue(unit);
        }
    }
}

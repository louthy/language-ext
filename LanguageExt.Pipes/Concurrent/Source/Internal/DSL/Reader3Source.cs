using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace LanguageExt.Pipes.Concurrent;

record Reader3Source<A, B, C>(Channel<A> ChannelA, Channel<B> ChannelB, Channel<C> ChannelC) : Source<(A First, B Second, C Third)>
{
    internal override async ValueTask<Reduced<S>> ReduceAsync<S>(
        S state,
        ReducerAsync<(A First, B Second, C Third), S> reducer,
        CancellationToken token)
    {
        while (true)
        {
            if (token.IsCancellationRequested) return Reduced.Done(state);

            var fa = ChannelA.Reader.WaitToReadAsync(token).AsTask();
            var fb = ChannelB.Reader.WaitToReadAsync(token).AsTask();
            var fc = ChannelC.Reader.WaitToReadAsync(token).AsTask();
            await Task.WhenAll(fa, fb, fc);

            if (!fa.Result || !fb.Result) return Reduced.Done(state);

            var ta = ChannelA.Reader.ReadAsync(token).AsTask();
            var tb = ChannelB.Reader.ReadAsync(token).AsTask();
            var tc = ChannelC.Reader.ReadAsync(token).AsTask();
            await Task.WhenAll(ta, tb, tc);
            
            switch (await reducer(state, (ta.Result, tb.Result, tc.Result)))
            {
                case { Continue: true, Value: var value }:
                    state = value;
                    break;
                
                case { Value: var value }:
                    return Reduced.Done(value);
            }
        }
    }
}

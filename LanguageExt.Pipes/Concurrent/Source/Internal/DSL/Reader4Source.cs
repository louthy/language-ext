using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace LanguageExt.Pipes.Concurrent;

record Reader4Source<A, B, C, D>(Channel<A> ChannelA, Channel<B> ChannelB, Channel<C> ChannelC, Channel<D> ChannelD) : Source<(A First, B Second, C Third, D Fourth)>
{
    internal override async ValueTask<Reduced<S>> ReduceAsync<S>(
        S state,
        ReducerAsync<(A First, B Second, C Third, D Fourth), S> reducer,
        CancellationToken token)
    {
        while (true)
        {
            if (token.IsCancellationRequested) return Reduced.Done(state);

            var fa = ChannelA.Reader.WaitToReadAsync(token).AsTask();
            var fb = ChannelB.Reader.WaitToReadAsync(token).AsTask();
            var fc = ChannelC.Reader.WaitToReadAsync(token).AsTask();
            var fd = ChannelD.Reader.WaitToReadAsync(token).AsTask();
            await Task.WhenAll(fa, fb, fc, fd);

            if (!fa.Result || !fb.Result) return Reduced.Done(state);

            var ta = ChannelA.Reader.ReadAsync(token).AsTask();
            var tb = ChannelB.Reader.ReadAsync(token).AsTask();
            var tc = ChannelC.Reader.ReadAsync(token).AsTask();
            var td = ChannelD.Reader.ReadAsync(token).AsTask();
            await Task.WhenAll(ta, tb, tc, td);
            
            switch (await reducer(state, (ta.Result, tb.Result, tc.Result, td.Result)))
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

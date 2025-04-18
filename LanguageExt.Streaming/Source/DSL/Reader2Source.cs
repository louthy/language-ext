using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt;

record Reader2Source<A, B>(Channel<A> ChannelA, Channel<B> ChannelB) : Source<(A First, B Second)>
{
    internal override async ValueTask<Reduced<S>> ReduceAsync<S>(
        S state,
        ReducerAsync<(A First, B Second), S> reducer,
        CancellationToken token)
    {
        while (true)
        {
            if (token.IsCancellationRequested) return Reduced.Done(state);

            var fa = ChannelA.Reader.WaitToReadAsync(token).AsTask();
            var fb = ChannelB.Reader.WaitToReadAsync(token).AsTask();
            await Task.WhenAll(fa, fb);

            if (!fa.Result || !fb.Result) return Reduced.Done(state);

            var ta = ChannelA.Reader.ReadAsync(token).AsTask();
            var tb = ChannelB.Reader.ReadAsync(token).AsTask();
            await Task.WhenAll(ta, tb);
            
            switch (await reducer(state, (ta.Result, tb.Result)))
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

using System.Threading.Channels;
using System.Threading.Tasks;

namespace LanguageExt;

record Reader3Source<A, B, C>(Channel<A> ChannelA, Channel<B> ChannelB, Channel<C> ChannelC) : Source<(A First, B Second, C Third)>
{
    internal override IO<Reduced<S>> ReduceInternal<S>(S state, ReducerIO<(A First, B Second, C Third), S> reducer) =>
        IO.liftVAsync(async e =>
        {
            while (true)
            {
                if (e.Token.IsCancellationRequested) return Reduced.Done(state);

                var fa = ChannelA.Reader.WaitToReadAsync(e.Token).AsTask();
                var fb = ChannelB.Reader.WaitToReadAsync(e.Token).AsTask();
                var fc = ChannelC.Reader.WaitToReadAsync(e.Token).AsTask();
                await Task.WhenAll(fa, fb, fc);

                if (!fa.Result || !fb.Result) return Reduced.Done(state);

                var ta = ChannelA.Reader.ReadAsync(e.Token).AsTask();
                var tb = ChannelB.Reader.ReadAsync(e.Token).AsTask();
                var tc = ChannelC.Reader.ReadAsync(e.Token).AsTask();
                await Task.WhenAll(ta, tb, tc);
                
                switch (await reducer(state, (ta.Result, tb.Result, tc.Result)).RunAsync(e))
                {
                    case { Continue: true, Value: var value }:
                        state = value;
                        break;
                    
                    case { Value: var value }:
                        return Reduced.Done(value);
                }
            }
        });
}

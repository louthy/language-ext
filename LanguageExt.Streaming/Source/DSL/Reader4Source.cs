using System.Threading.Channels;
using System.Threading.Tasks;

namespace LanguageExt;

record Reader4Source<A, B, C, D>(Channel<A> ChannelA, Channel<B> ChannelB, Channel<C> ChannelC, Channel<D> ChannelD) : Source<(A First, B Second, C Third, D Fourth)>
{
    internal override IO<Reduced<S>> ReduceInternal<S>(S state, ReducerIO<(A First, B Second, C Third, D Fourth), S> reducer) =>
        IO.liftVAsync(async e =>
        {
            while (true)
            {
                if (e.Token.IsCancellationRequested) return Reduced.Done(state);

                var fa = ChannelA.Reader.WaitToReadAsync(e.Token).AsTask();
                var fb = ChannelB.Reader.WaitToReadAsync(e.Token).AsTask();
                var fc = ChannelC.Reader.WaitToReadAsync(e.Token).AsTask();
                var fd = ChannelD.Reader.WaitToReadAsync(e.Token).AsTask();
                await Task.WhenAll(fa, fb, fc, fd);

                if (!fa.Result || !fb.Result) return Reduced.Done(state);

                var ta = ChannelA.Reader.ReadAsync(e.Token).AsTask();
                var tb = ChannelB.Reader.ReadAsync(e.Token).AsTask();
                var tc = ChannelC.Reader.ReadAsync(e.Token).AsTask();
                var td = ChannelD.Reader.ReadAsync(e.Token).AsTask();
                await Task.WhenAll(ta, tb, tc, td);
                
                switch (await reducer(state, (ta.Result, tb.Result, tc.Result, td.Result)).RunAsync(e))
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

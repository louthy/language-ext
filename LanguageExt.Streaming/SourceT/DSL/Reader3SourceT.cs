using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt;

record Reader3SourceT<M, A, B, C>(
    Channel<K<M, A>> ChannelA, 
    Channel<K<M, B>> ChannelB, 
    Channel<K<M, C>> ChannelC) : SourceT<M, (A First, B Second, C Third)>
    where M : MonadIO<M>, Alternative<M>
{
    public override K<M, S> ReduceM<S>(S state, ReducerM<M, K<M, (A First, B Second, C Third)>, S> reducer)
    {
        return M.LiftIOMaybe(IO.liftVAsync(e => go(state, e.Token))).Flatten();
        async ValueTask<K<M, S>> go(S state, CancellationToken token)
        {
            if(token.IsCancellationRequested) return M.Pure(state);
            var fa = ChannelA.Reader.WaitToReadAsync(token).AsTask();
            var fb = ChannelB.Reader.WaitToReadAsync(token).AsTask();
            var fc = ChannelC.Reader.WaitToReadAsync(token).AsTask();
            await Task.WhenAll(fa, fb, fc);

            if (!fa.Result || !fb.Result || !fc.Result) return M.Pure(state);

            var ta = ChannelA.Reader.ReadAsync(token).AsTask();
            var tb = ChannelB.Reader.ReadAsync(token).AsTask();
            var tc = ChannelC.Reader.ReadAsync(token).AsTask();
            await Task.WhenAll(ta, tb, tc);
            
            return reducer(state, ta.Result.Zip(tb.Result, tc.Result))
               .Bind(s => go(s, token).GetAwaiter().GetResult());
        }
    }
}

using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt;

record Reader4SourceT<M, A, B, C, D>(
    Channel<K<M, A>> ChannelA, 
    Channel<K<M, B>> ChannelB, 
    Channel<K<M, C>> ChannelC, 
    Channel<K<M, D>> ChannelD) : SourceT<M, (A First, B Second, C Third, D Fourth)>
    where M : MonadIO<M>, Alternative<M>
{
    public override K<M, S> ReduceM<S>(S state, ReducerM<M, K<M, (A First, B Second, C Third, D Fourth)>, S> reducer)
    {
        return M.LiftIOMaybe(IO.liftVAsync(e => go(state, e.Token))).Flatten();
        async ValueTask<K<M, S>> go(S state, CancellationToken token)
        {
            if(token.IsCancellationRequested) return M.Pure(state);
            var fa = ChannelA.Reader.WaitToReadAsync(token).AsTask();
            var fb = ChannelB.Reader.WaitToReadAsync(token).AsTask();
            var fc = ChannelC.Reader.WaitToReadAsync(token).AsTask();
            var fd = ChannelD.Reader.WaitToReadAsync(token).AsTask();
            await Task.WhenAll(fa, fb, fc, fd);

            if (!fa.Result || !fb.Result || !fc.Result || !fd.Result) return M.Pure(state);

            var ta = ChannelA.Reader.ReadAsync(token).AsTask();
            var tb = ChannelB.Reader.ReadAsync(token).AsTask();
            var tc = ChannelC.Reader.ReadAsync(token).AsTask();
            var td = ChannelD.Reader.ReadAsync(token).AsTask();
            await Task.WhenAll(ta, tb, tc, td);
            
            return reducer(state, ta.Result.Zip(tb.Result, tc.Result, td.Result))
               .Bind(s => go(s, token).GetAwaiter().GetResult());
        }
    }
}

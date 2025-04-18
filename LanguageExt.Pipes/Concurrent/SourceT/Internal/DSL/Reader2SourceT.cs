using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record Reader2SourceT<M, A, B>(Channel<K<M, A>> ChannelA, Channel<K<M, B>> ChannelB) : SourceT<M, (A First, B Second)>
    where M : MonadIO<M>, Alternative<M>
{
    public override K<M, S> ReduceM<S>(S state, ReducerM<M, K<M, (A First, B Second)>, S> reducer)
    {
        return M.LiftIO(IO.liftVAsync(e => go(state, e.Token))).Flatten();
        async ValueTask<K<M, S>> go(S state, CancellationToken token)
        {
            if(token.IsCancellationRequested) return M.Pure(state);
            var fa = ChannelA.Reader.WaitToReadAsync(token).AsTask();
            var fb = ChannelB.Reader.WaitToReadAsync(token).AsTask();
            await Task.WhenAll(fa, fb);
            
            if (!fa.Result || !fb.Result) return M.Pure(state);

            var ta = ChannelA.Reader.ReadAsync(token).AsTask();
            var tb = ChannelB.Reader.ReadAsync(token).AsTask();
            await Task.WhenAll(ta, tb);
            
            return reducer(state, ta.Result.Zip(tb.Result))
               .Bind(s => go(s, token).GetAwaiter().GetResult());
        }
    }
}

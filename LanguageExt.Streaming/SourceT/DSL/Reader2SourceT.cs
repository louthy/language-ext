using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt;

record Reader2SourceT<M, A, B>(Channel<K<M, A>> ChannelA, Channel<K<M, B>> ChannelB) : SourceT<M, (A First, B Second)>
    where M : MonadIO<M>
{
    public override K<M, Reduced<S>> ReduceInternalM<S>(S state, ReducerM<M, K<M, (A First, B Second)>, S> reducer)
    {
        return M.LiftIO(IO.liftVAsync(e => go(state, e.Token))).Flatten();
        async ValueTask<K<M, Reduced<S>>> go(S state, CancellationToken token)
        {
            if(token.IsCancellationRequested) return M.Pure(Reduced.Done(state));
            var fa = ChannelA.Reader.WaitToReadAsync(token).AsTask();
            var fb = ChannelB.Reader.WaitToReadAsync(token).AsTask();
            await Task.WhenAll(fa, fb);
            
            if (!fa.Result || !fb.Result) return M.Pure(Reduced.Done(state));

            var ta = ChannelA.Reader.ReadAsync(token).AsTask();
            var tb = ChannelB.Reader.ReadAsync(token).AsTask();
            await Task.WhenAll(ta, tb);

            return reducer(state, ta.Result.Zip(tb.Result)) >>
                   (ns => ns.Continue
                              ? go(ns.Value, token).GetAwaiter().GetResult()
                              : M.Pure(ns));
        }
    }
}

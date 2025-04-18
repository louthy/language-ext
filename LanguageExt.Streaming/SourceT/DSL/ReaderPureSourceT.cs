using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt;

record ReaderPureSourceT<M, A>(Channel<A> Channel) : SourceT<M, A>
    where M : MonadIO<M>, Alternative<M>
{
    public override K<M, S> ReduceM<S>(S state, ReducerM<M, K<M, A>, S> reducer)
    {
        return M.LiftIO(IO.liftVAsync(e => go(state, e.Token))).Flatten();
        async ValueTask<K<M, S>> go(S state, CancellationToken token)
        {
            if(token.IsCancellationRequested) return M.Pure(state);
            if (!await Channel.Reader.WaitToReadAsync(token)) return M.Pure(state);
            var head = await Channel.Reader.ReadAsync(token);
            return reducer(state, M.Pure(head)).Bind(s => go(s, token).GetAwaiter().GetResult());
        }
    }
}

using System;
using System.Threading.Tasks;
using LanguageExt.Traits;
using System.Threading;

namespace LanguageExt;

record ObservableSourceT<M, A>(IObservable<K<M, A>> Items) : SourceT<M, A>
    where M : MonadIO<M>, Alternative<M>
{
    public override K<M, S> ReduceM<S>(S state, ReducerM<M, K<M, A>, S> reducer) 
    {
        return M.LiftIO(IO.liftVAsync(e => go(state, Items.ToAsyncEnumerable(e.Token).GetIteratorAsync(), e.Token))).Flatten();
        async ValueTask<K<M, S>> go(S state, IteratorAsync<K<M, A>> iter, CancellationToken token)
        {
            if(token.IsCancellationRequested) return M.Pure(state);
            if (await iter.IsEmpty) return M.Pure(state);
            var head = await iter.Head;
            var tail = (await iter.Tail).Split();
            return reducer(state, head).Bind(s => go(s, tail, token).GetAwaiter().GetResult());
        }
    }
}

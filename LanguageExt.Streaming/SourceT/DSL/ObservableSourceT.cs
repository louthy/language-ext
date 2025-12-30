using System;
using System.Threading.Tasks;
using LanguageExt.Traits;
using System.Threading;

namespace LanguageExt;

record ObservableSourceT<M, A>(IObservable<K<M, A>> Items) : SourceT<M, A>
    where M : MonadIO<M>
{
    public override K<M, Reduced<S>> ReduceInternalM<S>(S state, ReducerM<M, K<M, A>, S> reducer) 
    {
        return M.LiftIO(IO.liftVAsync(e => go(state, Items.ToAsyncEnumerable(e.Token).GetIteratorAsync(), e.Token))).Flatten();
        async ValueTask<K<M, Reduced<S>>> go(S state, IteratorAsync<K<M, A>> iter, CancellationToken token)
        {
            if(token.IsCancellationRequested) return M.Pure(Reduced.Done(state));
            if (await iter.IsEmpty) return M.Pure(Reduced.Done(state));
            var head = await iter.Head;
            var tail = (await iter.Tail).Split();
            return reducer(state, head) >> (ns => ns.Continue 
                                                      ? go(ns.Value, tail, token).GetAwaiter().GetResult()
                                                      : M.Pure(ns));
        }
    }
}

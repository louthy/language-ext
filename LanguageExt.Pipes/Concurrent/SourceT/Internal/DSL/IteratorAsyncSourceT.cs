using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageExt.Traits;
using System.Threading;

namespace LanguageExt.Pipes.Concurrent;

record IteratorAsyncSourceT<M, A>(IAsyncEnumerable<K<M, A>> Items) : SourceT<M, A>
    where M : Monad<M>, Alternative<M>
{
    internal override SourceTIterator<M, A> GetIterator() =>
        new IteratorAsyncSourceTIterator<M, A> { Src = Items.GetIteratorAsync() };

    internal override K<M, S> ReduceInternal<S>(S state, ReducerM<M, A, S> reducer)
    {
        return IO.token.BindAsync(t => go(state, Items.GetIteratorAsync(), t));

        async ValueTask<K<M, S>> go(S state, IteratorAsync<K<M, A>> iter, CancellationToken token)
        {
            if(token.IsCancellationRequested) throw new TaskCanceledException();
            if (await iter.IsEmpty) return M.Pure(state);
            var head = await iter.Head;
            var tail = await iter.Tail;
            return head.Bind(a => reducer(state, a).Bind(s => go(s, tail, token).GetAwaiter().GetResult()));
        }
    }
}

using LanguageExt.Traits;
using static LanguageExt.Prelude;
using System.Collections.Concurrent;

namespace LanguageExt.Pipes.Concurrent;

record CombineSourceT<M, A>(Seq<SourceT<M, A>> SourceTs) : SourceT<M, A>
    where M : Monad<M>, Alternative<M>
{
    internal override SourceTIterator<M, A> GetIterator() =>
        new CombineSourceTIterator<M, A>(SourceTs.Map(x => x.GetIterator()));

    internal override K<M, S> ReduceInternal<S>(S state, ReducerM<M, A, S> reducer)
    {
        return from wait   in Signal.autoReset<M>()
               let active  =  Atom(SourceTs.Count)
               let queue   =  new ConcurrentQueue<A>()
               from forks  in SourceTs.Traverse(s => s.ReduceInternal(unit, reduce(wait, queue))
                                                        .Map(_ =>
                                                             {
                                                                 active.Swap(x => x - 1);
                                                                 wait.TriggerUnsafe();
                                                                 return unit;
                                                             }))
               from nstate in merged(wait, queue, active, state)
               from _      in release(wait)
               select nstate;

        ReducerM<M, A, Unit> reduce(Signal<M> wait, ConcurrentQueue<A> queue) =>
            (_, x) =>
                M.LiftIO(
                    IO.lift(_ =>
                            {
                                queue.Enqueue(x);
                                wait.TriggerUnsafe();
                                return unit;
                            }));

        K<M, S> merged(Signal<M> wait, ConcurrentQueue<A> queue, Atom<int> active, S state) =>
            wait.Wait().Bind(_ => dequeue(wait, queue, active, state));

        K<M, S> dequeue(Signal<M> wait, ConcurrentQueue<A> queue, Atom<int> active, S state) =>
            queue.TryDequeue(out var ms)
                ? reducer(state, ms).Bind(s => dequeue(wait, queue, active, s))
                : active.Value <= 0
                    ? M.Pure(state)
                    : merged(wait, queue, active, state);
    }
}

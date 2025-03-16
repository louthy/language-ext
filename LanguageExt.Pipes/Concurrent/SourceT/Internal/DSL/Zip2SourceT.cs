using System;
using LanguageExt.Traits;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using static LanguageExt.Prelude;

namespace LanguageExt.Pipes.Concurrent;

record Zip2SourceT<M, A, B>(SourceT<M, A> SourceTA, SourceT<M, B> SourceTB) : SourceT<M, (A First, B Second)>
    where M : Monad<M>, Alternative<M>
{
    internal override SourceTIterator<M, (A First, B Second)> GetIterator() =>
        new Zip2SourceTIterator<M, A, B>(SourceTA.GetIterator(), SourceTB.GetIterator());

    internal override K<M, S> ReduceInternal<S>(S state, ReducerM<M, (A First, B Second), S> reducer)
    {
        return from wait in Signal.autoReset<M>()
               let qa = new ConcurrentQueue<A>()
               let qb = new ConcurrentQueue<B>()
               let active = Atom(true)
               let done = complete(wait, active)
               from tasks in Seq(SourceTA.ReduceInternal(unit, reduce(qa, wait)).Map(done),
                                 SourceTB.ReduceInternal(unit, reduce(qb, wait)).Map(done))
                            .Traverse(mu => mu.ForkIO())
               from newstate in listen(state, active, wait, qa, qb)
               from _ in tasks.Traverse(t => t.Cancel)
               select newstate;

        K<M, S> listen(
            S state,
            Atom<bool> active,
            Signal<M> wait,
            ConcurrentQueue<A> qa,
            ConcurrentQueue<B> qb) =>
            active.Value
                ? M.Pure(state)
                : wait.Wait().Bind(_ => read(state, active, wait, qa, qb));

        K<M, S> read(
            S state,
            Atom<bool> active,
            Signal<M> wait,
            ConcurrentQueue<A> qa,
            ConcurrentQueue<B> qb) =>
            M.LiftIO(
                  IO.lift(
                      e =>
                      {
                          var mr = M.Pure(state);
                          while (!qa.IsEmpty && !qb.IsEmpty)
                          {
                              if (e.Token.IsCancellationRequested) throw new TaskCanceledException();
                              if (qa.TryDequeue(out var a) &&
                                  qb.TryDequeue(out var b))
                              {
                                  mr = mr.Action(reducer(state, (a, b)));
                              }
                          }

                          return mr.Bind(_ => listen(state, active, wait, qa, qb));
                      }))
             .Flatten();

        ReducerM<M, X, Unit> reduce<X>(ConcurrentQueue<X> queue, Signal<M> wait) =>
            (_, x) =>
            {
                queue.Enqueue(x);
                wait.TriggerUnsafe();
                return M.Pure(unit);
            };

        Func<Unit, Unit> complete(Signal<M> wait, Atom<bool> active) =>
            _ =>
            {
                active.Swap(_ => false);
                wait.TriggerUnsafe();
                return unit;
            };
    }
}

using System;
using static LanguageExt.Prelude;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

record Zip3SourceT<M, A, B, C>(
    SourceT<M, A> SourceTA, 
    SourceT<M, B> SourceTB, 
    SourceT<M, C> SourceTC) : 
    SourceT<M, (A First, B Second, C Third)>
    where M : Monad<M>, Alternative<M>
{
    internal override SourceTIterator<M, (A First, B Second, C Third)> GetIterator() =>
        new Zip3SourceTIterator<M, A, B, C>(SourceTA.GetIterator(), SourceTB.GetIterator(), SourceTC.GetIterator());

    internal override K<M, S> ReduceInternal<S>(S state, ReducerM<M, (A First, B Second, C Third), S> reducer)
    {
        return from wait in Signal.autoReset<M>()
               let qa = new ConcurrentQueue<A>()
               let qb = new ConcurrentQueue<B>()
               let qc = new ConcurrentQueue<C>()
               let active = Atom(true)
               let done = complete(wait, active)
               from tasks in Seq(SourceTA.ReduceInternal(unit, reduce(qa, wait)).Map(done),
                                 SourceTB.ReduceInternal(unit, reduce(qb, wait)).Map(done),
                                 SourceTC.ReduceInternal(unit, reduce(qc, wait)).Map(done))
                            .Traverse(mu => mu.ForkIO())
               from newstate in listen(state, active, wait, qa, qb, qc)
               from _ in tasks.Traverse(t => t.Cancel)
               select newstate;

        K<M, S> listen(
            S state,
            Atom<bool> active,
            Signal<M> wait,
            ConcurrentQueue<A> qa,
            ConcurrentQueue<B> qb,
            ConcurrentQueue<C> qc) =>
            active.Value
                ? M.Pure(state)
                : wait.Wait().Bind(_ => read(state, active, wait, qa, qb, qc));

        K<M, S> read(
            S state,
            Atom<bool> active,
            Signal<M> wait,
            ConcurrentQueue<A> qa,
            ConcurrentQueue<B> qb,
            ConcurrentQueue<C> qc) =>
            M.LiftIO(
                  IO.lift(
                      e =>
                      {
                          var mr = M.Pure(state);
                          while (!qa.IsEmpty && !qb.IsEmpty && !qc.IsEmpty)
                          {
                              if (e.Token.IsCancellationRequested) throw new TaskCanceledException();
                              if (qa.TryDequeue(out var a) &&
                                  qb.TryDequeue(out var b) &&
                                  qc.TryDequeue(out var c))
                              {
                                  mr = mr.Action(reducer(state, (a, b, c)));
                              }
                          }

                          return mr.Bind(_ => listen(state, active, wait, qa, qb, qc));
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

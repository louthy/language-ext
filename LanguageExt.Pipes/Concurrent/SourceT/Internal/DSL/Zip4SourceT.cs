using System;
using LanguageExt.Traits;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using static LanguageExt.Prelude;

namespace LanguageExt.Pipes.Concurrent;

record Zip4SourceT<M, A, B, C, D>(SourceT<M, A> SourceTA, SourceT<M, B> SourceTB, SourceT<M, C> SourceTC, SourceT<M, D> SourceTD) 
    : SourceT<M, (A First, B Second, C Third, D Fourth)>
    where M : Monad<M>, Alternative<M>
{
    internal override SourceTIterator<M, (A First, B Second, C Third, D Fourth)> GetIterator() =>
        new Zip4SourceTIterator<M, A, B, C, D>(
            SourceTA.GetIterator(), 
            SourceTB.GetIterator(), 
            SourceTC.GetIterator(), 
            SourceTD.GetIterator());

    internal override K<M, S> ReduceInternal<S>(S state, ReducerM<M, (A First, B Second, C Third, D Fourth), S> reducer)
    {
        return from wait in Signal.autoReset<M>()
               let qa = new ConcurrentQueue<A>()
               let qb = new ConcurrentQueue<B>()
               let qc = new ConcurrentQueue<C>()
               let qd = new ConcurrentQueue<D>()
               let active = Atom(true)
               let done = complete(wait, active)
               from tasks in Seq(SourceTA.ReduceInternal(unit, reduce(qa, wait)).Map(done),
                                 SourceTB.ReduceInternal(unit, reduce(qb, wait)).Map(done),
                                 SourceTC.ReduceInternal(unit, reduce(qc, wait)).Map(done),
                                 SourceTD.ReduceInternal(unit, reduce(qd, wait)).Map(done))
                            .Traverse(mu => mu.ForkIO())
               from newstate in listen(state, active, wait, qa, qb, qc, qd)
               from _ in tasks.Traverse(t => t.Cancel)
               select newstate;

        K<M, S> listen(
            S state,
            Atom<bool> active,
            Signal<M> wait,
            ConcurrentQueue<A> qa,
            ConcurrentQueue<B> qb,
            ConcurrentQueue<C> qc,
            ConcurrentQueue<D> qd) =>
            active.Value
                ? M.Pure(state)
                : wait.Wait().Bind(_ => read(state, active, wait, qa, qb, qc, qd));

        K<M, S> read(
            S state,
            Atom<bool> active,
            Signal<M> wait,
            ConcurrentQueue<A> qa,
            ConcurrentQueue<B> qb,
            ConcurrentQueue<C> qc,
            ConcurrentQueue<D> qd) =>
            M.LiftIO(
                  IO.lift(
                      e =>
                      {
                          var mr = M.Pure(state);
                          while (!qa.IsEmpty && !qb.IsEmpty && !qc.IsEmpty && !qd.IsEmpty)
                          {
                              if (e.Token.IsCancellationRequested) throw new TaskCanceledException();
                              if (qa.TryDequeue(out var a) &&
                                  qb.TryDequeue(out var b) &&
                                  qc.TryDequeue(out var c) &&
                                  qd.TryDequeue(out var d))
                              {
                                  mr = mr.Action(reducer(state, (a, b, c, d)));
                              }
                          }

                          return mr.Bind(_ => listen(state, active, wait, qa, qb, qc, qd));
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

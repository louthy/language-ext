using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.Pipes.Concurrent;

record PureSourceT<M, A>(A Value) : SourceT<M, A>
    where M : Monad<M>, Alternative<M>
{
    internal override SourceTIterator<M, A> GetIterator() =>
        new SingletonSourceTIterator<M, A>(Value);

    public override K<M, S> Reduce<S>(S state, ReducerM<M, A, S> reducer) =>
        reducer(state, Value);
}

record LiftSourceT<M, A>(K<M, A> Value) : SourceT<M, A>
    where M : Monad<M>, Alternative<M>
{
    internal override SourceTIterator<M, A> GetIterator() =>
        new LiftSourceTIterator<M, A>(Value);

    public override K<M, S> Reduce<S>(S state, ReducerM<M, A, S> reducer) =>
        Value.Bind(x => reducer(state, x));
}

record EmptySourceT<M, A> : SourceT<M, A>
    where M : Monad<M>, Alternative<M>
{
    public static readonly SourceT<M, A> Default = new EmptySourceT<M, A>();
    
    internal override SourceTIterator<M, A> GetIterator() =>
        EmptySourceTIterator<M, A>.Default;

    public override SourceT<M, B> Map<B>(Func<A, B> f) =>
        EmptySourceT<M, B>.Default;

    public override SourceT<M, B> Bind<B>(Func<A, SourceT<M, B>> f) => 
        EmptySourceT<M, B>.Default;

    public override SourceT<M, B> ApplyBack<B>(SourceT<M, Func<A, B>> ff) =>
        EmptySourceT<M, B>.Default;

    public override K<M, S> Reduce<S>(S state, ReducerM<M, A, S> reducer) =>
        M.Pure(state);
}

record ForeverSourceT<M, A>(K<M, A> Value) : SourceT<M, A>
    where M : Monad<M>, Alternative<M>
{
    internal override SourceTIterator<M, A> GetIterator() =>
        new ForeverSourceTIterator<M, A>(Value);

    public override K<M, S> Reduce<S>(S state, ReducerM<M, A, S> reducer) =>
        Value.Bind(x => reducer(state, x).Bind(s => Reduce(s, reducer)));
}

record ReaderSourceT<M, A>(Channel<K<M, A>> Channel) : SourceT<M, A>
    where M : Monad<M>, Alternative<M>
{
    internal override SourceTIterator<M, A> GetIterator() =>
        new ReaderSourceTIterator<M, A>(Channel);

    public override K<M, S> Reduce<S>(S state, ReducerM<M, A, S> reducer)
    {
        return reduce(state);

        K<M, S> reduce(S nstate) =>
            M.LiftIO(IO.liftAsync(async e =>
                                  {
                                      if (await Channel.Reader.WaitToReadAsync(e.Token))
                                      {
                                          var value = await Channel.Reader.ReadAsync(e.Token);
                                          return value.Bind(v => reducer(nstate, v));
                                      }
                                      else
                                      {
                                          return M.Pure(nstate);
                                      }
                                  }))
             .Flatten();
    }

}

record ReaderPureSourceT<M, A>(Channel<A> Channel) : SourceT<M, A>
    where M : Monad<M>, Alternative<M>
{
    internal override SourceTIterator<M, A> GetIterator() =>
        new ReaderPureSourceTIterator<M, A>(Channel);

    public override K<M, S> Reduce<S>(S state, ReducerM<M, A, S> reducer)
    {
        return reduce(state);

        K<M, S> reduce(S nstate) =>
            M.LiftIO(IO.liftAsync(async e =>
                                  {
                                      if (await Channel.Reader.WaitToReadAsync(e.Token))
                                      {
                                          var value = await Channel.Reader.ReadAsync(e.Token);
                                          return reducer(nstate, value);
                                      }
                                      else
                                      {
                                          return M.Pure(nstate);
                                      }
                                  }))
             .Flatten();
    }

}

record ApplySourceT<M, A, B>(SourceT<M, Func<A, B>> FF, SourceT<M, A> FA) : SourceT<M, B>
    where M : Monad<M>, Alternative<M>
{
    internal override SourceTIterator<M, B> GetIterator() =>
        new ApplySourceTIterator<M, A, B>(FF.GetIterator(), FA.GetIterator());

    public override K<M, S> Reduce<S>(S state, ReducerM<M, B, S> reducer) =>
        FF.Reduce(state, (s, f) => FA.Reduce(s, (s1, x) => reducer(s1, f(x))));
}

record BindSourceT<M, A, B>(SourceT<M, A> SourceT, Func<A, SourceT<M, B>> F) : SourceT<M, B>
    where M : Monad<M>, Alternative<M>
{
    internal override SourceTIterator<M, B> GetIterator() =>
        new BindSourceTIterator<M, A, B>(SourceT.GetIterator(), x => F(x).GetIterator());

    public override K<M, S> Reduce<S>(S state, ReducerM<M, B, S> reducer) =>
        SourceT.Reduce(state, (s, x) => F(x).Reduce(s, reducer));
}

record CombineSourceT<M, A>(Seq<SourceT<M, A>> SourceTs) : SourceT<M, A>
    where M : Monad<M>, Alternative<M>
{
    internal override SourceTIterator<M, A> GetIterator() =>
        new CombineSourceTIterator<M, A>(SourceTs.Map(x => x.GetIterator()));

    public override K<M, S> Reduce<S>(S state, ReducerM<M, A, S> reducer)
    {
        return from wait   in Signal.autoReset<M>()
               let active  =  Atom(SourceTs.Count)
               let queue   =  new ConcurrentQueue<A>()
               from forks  in SourceTs.Traverse(s => s.Reduce(unit, reduce(wait, queue))
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

record ChooseSourceT<M, A>(SourceT<M, A> SourceTA, SourceT<M, A> SourceTB) : SourceT<M, A>
    where M : Monad<M>, Alternative<M>
{
    internal override SourceTIterator<M, A> GetIterator() =>
        new ChooseSourceTIterator<M, A>(SourceTA.GetIterator(), SourceTB.GetIterator());

    public override K<M, S> Reduce<S>(S state, ReducerM<M, A, S> reducer)
    {
        try
        {
            return SourceTA.Reduce(state, reducer).Choose(() => SourceTB.Reduce(state, reducer));
        }
        catch
        {
            return SourceTB.Reduce(state, reducer);
        }
    }
}

record Zip2SourceT<M, A, B>(SourceT<M, A> SourceTA, SourceT<M, B> SourceTB) : SourceT<M, (A First, B Second)>
    where M : Monad<M>, Alternative<M>
{
    internal override SourceTIterator<M, (A First, B Second)> GetIterator() =>
        new Zip2SourceTIterator<M, A, B>(SourceTA.GetIterator(), SourceTB.GetIterator());

    public override K<M, S> Reduce<S>(S state, ReducerM<M, (A First, B Second), S> reducer)
    {
        return from wait in Signal.autoReset<M>()
               let qa = new ConcurrentQueue<A>()
               let qb = new ConcurrentQueue<B>()
               let active = Atom(true)
               let done = complete(wait, active)
               from tasks in Seq(SourceTA.Reduce(unit, reduce(qa, wait)).Map(done),
                                 SourceTB.Reduce(unit, reduce(qb, wait)).Map(done))
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

record Zip3SourceT<M, A, B, C>(
    SourceT<M, A> SourceTA, 
    SourceT<M, B> SourceTB, 
    SourceT<M, C> SourceTC) : 
    SourceT<M, (A First, B Second, C Third)>
    where M : Monad<M>, Alternative<M>
{
    internal override SourceTIterator<M, (A First, B Second, C Third)> GetIterator() =>
        new Zip3SourceTIterator<M, A, B, C>(SourceTA.GetIterator(), SourceTB.GetIterator(), SourceTC.GetIterator());

    public override K<M, S> Reduce<S>(S state, ReducerM<M, (A First, B Second, C Third), S> reducer)
    {
        return from wait in Signal.autoReset<M>()
               let qa = new ConcurrentQueue<A>()
               let qb = new ConcurrentQueue<B>()
               let qc = new ConcurrentQueue<C>()
               let active = Atom(true)
               let done = complete(wait, active)
               from tasks in Seq(SourceTA.Reduce(unit, reduce(qa, wait)).Map(done),
                                 SourceTB.Reduce(unit, reduce(qb, wait)).Map(done),
                                 SourceTC.Reduce(unit, reduce(qc, wait)).Map(done))
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

    public override K<M, S> Reduce<S>(S state, ReducerM<M, (A First, B Second, C Third, D Fourth), S> reducer)
    {
        return from wait in Signal.autoReset<M>()
               let qa = new ConcurrentQueue<A>()
               let qb = new ConcurrentQueue<B>()
               let qc = new ConcurrentQueue<C>()
               let qd = new ConcurrentQueue<D>()
               let active = Atom(true)
               let done = complete(wait, active)
               from tasks in Seq(SourceTA.Reduce(unit, reduce(qa, wait)).Map(done),
                                 SourceTB.Reduce(unit, reduce(qb, wait)).Map(done),
                                 SourceTC.Reduce(unit, reduce(qc, wait)).Map(done),
                                 SourceTD.Reduce(unit, reduce(qd, wait)).Map(done))
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

record IteratorSyncSourceT<M, A>(IEnumerable<K<M, A>> Items) : SourceT<M, A>
    where M : Monad<M>, Alternative<M>
{
    internal override SourceTIterator<M, A> GetIterator() =>
        new IteratorSyncSourceTIterator<M, A> { Src = Items.GetIterator() };

    public override K<M, S> Reduce<S>(S state, ReducerM<M, A, S> reducer)
    {
        return go(state, Items.GetIterator());
        K<M, S> go(S state, Iterator<K<M, A>> iter) =>
            iter.IsEmpty
                ? M.Pure(state)
                : iter.Head.Bind(a => reducer(state, a).Bind(s => go(s, iter.Tail.Split())));    
    }
}

record IteratorAsyncSourceT<M, A>(IAsyncEnumerable<K<M, A>> Items) : SourceT<M, A>
    where M : Monad<M>, Alternative<M>
{
    internal override SourceTIterator<M, A> GetIterator() =>
        new IteratorAsyncSourceTIterator<M, A> { Src = Items.GetIteratorAsync() };

    public override K<M, S> Reduce<S>(S state, ReducerM<M, A, S> reducer)
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

record TransformSourceT<M, A, B>(SourceT<M, A> SourceT, Transducer<A, B> Transducer) : SourceT<M, B>
    where M : Monad<M>, Alternative<M>
{
    internal override SourceTIterator<M, B> GetIterator() =>
        new TransformSourceTIterator<M, A, B>(SourceT.GetIterator(), Transducer);

    public override K<M, S> Reduce<S>(S state, ReducerM<M, B, S> reducer)
    {
        return read(state, SourceT.GetIterator());

        K<M, S> read(S state, SourceTIterator<M, A> iter) =>
            IO.liftVAsync(e => iter.ReadyToRead(e.Token))
              .Bind(f => f
                             ? iter.Read()
                                   .Bind(x => Transducer.ReduceM(reducer)(state, x))
                                   .Bind(ns => read(ns, iter))
                             : M.Pure(state));
    }
}

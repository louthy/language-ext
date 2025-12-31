#pragma warning disable CS1998
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LanguageExt;

sealed class IterableEnumerable<A>(IO<IEnumerable<A>> runEnumerable) : Iterable<A>
{
    internal override bool IsAsync =>
        false;
    
    public override IO<int> CountIO() =>
        AsEnumerableIO().Map(xs => xs.Count());

    public override IO<IEnumerable<A>> AsEnumerableIO()
    {
        // This makes a regular IEnumerable cancellable.
        return IO.lift(env => go(env, runEnumerable));
        static IEnumerable<A> go(EnvIO env, IO<IEnumerable<A>> run)
        {
            var xs = run.Run(env);
            foreach (var x in xs)
            {
                if (env.Token.IsCancellationRequested) throw new OperationCanceledException();
                yield return x;
            }
        }
    }

    public override IO<IAsyncEnumerable<A>> AsAsyncEnumerableIO() =>
        AsEnumerableIO().Map(xs => xs.ToAsyncEnumerable());

    public override Iterable<A> Reverse() =>
        new IterableEnumerable<A>(AsEnumerableIO().Map(xs => xs.Reverse()));

    public override Iterable<B> Map<B>(Func<A, B> f) =>
        new IterableEnumerable<B>(AsEnumerableIO().Map(xs => xs.Select(f)));

    public override Iterable<A> Filter(Func<A, bool> f) =>
        new IterableEnumerable<A>(AsEnumerableIO().Map(xs => xs.Where(f)));

    public override IO<S> FoldWhileIO<S>(Func<A, Func<S, S>> f, Func<(S State, A Value), bool> predicate, S initialState) => 
        IO.liftVAsync(async env =>
                      {
                          var s  = initialState;
                          var xs = await runEnumerable.RunAsync(env);
                          foreach (var x in xs)
                          {
                              if(env.Token.IsCancellationRequested) throw new OperationCanceledException();
                              if (!predicate((s, x)))
                              {
                                  return s;
                              }
                              s = f(x)(s);
                          }
                          return s;
                      });

    public override IO<S> FoldWhileIO<S>(Func<S, A, S> f, Func<(S State, A Value), bool> predicate, S initialState) => 
        IO.liftVAsync(async env =>
                      {
                          var s  = initialState;
                          var xs = await runEnumerable.RunAsync(env);
                          foreach (var x in xs)
                          {
                              if(env.Token.IsCancellationRequested) throw new OperationCanceledException();
                              if (!predicate((s, x)))
                              {
                                  return s;
                              }
                              s = f(s, x);
                          }
                          return s;
                      });


    public override IO<S> FoldUntilIO<S>(Func<A, Func<S, S>> f, Func<(S State, A Value), bool> predicate, S initialState) => 
        IO.liftVAsync(async env =>
                      {
                          var s  = initialState;
                          var xs = await runEnumerable.RunAsync(env);
                          foreach (var x in xs)
                          {
                              if(env.Token.IsCancellationRequested) throw new OperationCanceledException();
                              s = f(x)(s);
                              if (predicate((s, x)))
                              {
                                  return s;
                              }
                          }
                          return s;
                      });

    public override IO<S> FoldUntilIO<S>(Func<S, A, S> f, Func<(S State, A Value), bool> predicate, S initialState) =>
        IO.liftVAsync(async env =>
                      {
                          var s  = initialState;
                          var xs = await runEnumerable.RunAsync(env);
                          foreach (var x in xs)
                          {
                              if (env.Token.IsCancellationRequested) throw new OperationCanceledException();
                              s = f(s, x);
                              if (predicate((s, x)))
                              {
                                  return s;
                              }
                          }
                          return s;
                      });

    public override Iterable<A> Choose(Iterable<A> rhs) =>
        rhs.IsAsync
            ? new IterableAsyncEnumerable<A>(
                IO.liftVAsync(async env =>
                              {
                                  var ls   = AsAsyncEnumerable(env.Token);
                                  var iter = ls.GetIteratorAsync();
                                  if (await iter.IsEmpty)
                                  {
                                      return rhs.AsAsyncEnumerable(env.Token);
                                  }
                                  else
                                  {
                                      // This has already been evaluated by `IsEmpty`
                                      var head = await iter.Head;
                                      var tail = (await iter.Tail).Split().AsEnumerable(env.Token);
                                      return tail.Prepend(head);
                                  }
                              }))
            : new IterableEnumerable<A>(
                IO.lift(env =>
                        {
                            var ls   = AsEnumerable(env.Token);
                            var iter = ls.GetIterator();
                            if (iter.IsEmpty)
                            {
                                return rhs.AsEnumerable(env.Token);
                            }
                            else
                            {
                                // This has already been evaluated by `IsEmpty`
                                var head = iter.Head;
                                var tail = iter.Tail.Split().AsEnumerable();
                                return tail.Prepend(head);
                            }
                        }));

    public override Iterable<A> Choose(Memo<Iterable, A> rhs) =>
        new IterableAsyncEnumerable<A>(
            IO.liftVAsync(async env =>
                          {
                              var ls   = AsAsyncEnumerable(env.Token);
                              var iter = ls.GetIteratorAsync();
                              if (await iter.IsEmpty)
                              {
                                  return rhs.Value.As().AsAsyncEnumerable(env.Token);
                              }
                              else
                              {
                                  // This has already been evaluated by `IsEmpty`
                                  var head = await iter.Head;
                                  var tail = (await iter.Tail).Split().AsEnumerable(env.Token);
                                  return tail.Prepend(head);
                              }
                          }));
}

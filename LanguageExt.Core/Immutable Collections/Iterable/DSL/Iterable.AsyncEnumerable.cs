using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt;

sealed class IterableAsyncEnumerable<A>(IO<IAsyncEnumerable<A>> runEnumerable) : Iterable<A>
{
    internal override bool IsAsync =>
        true;

    public override IO<int> CountIO() =>
        IO.liftVAsync(async env => await (await runEnumerable.RunAsync(env)).CountAsync(env.Token));

    public override IO<IEnumerable<A>> AsEnumerableIO()
    {
        return IO.lift(env => go(env, runEnumerable));

        static IEnumerable<A> go(EnvIO env, IO<IAsyncEnumerable<A>> run)
        {
            var xs = run.Run(env);
            foreach (var x in xs.ToBlockingEnumerable(env.Token))
            {
                if (env.Token.IsCancellationRequested) throw new OperationCanceledException();
                yield return x;
            }
        }
    }

    public override IO<IAsyncEnumerable<A>> AsAsyncEnumerableIO() 
    {
        return IO.lift(env => go(env, runEnumerable));

        static async IAsyncEnumerable<A> go(EnvIO env, IO<IAsyncEnumerable<A>> run)
        {
            var xs = await run.RunAsync(env);
            await foreach (var x in xs.WithCancellation(env.Token))
            {
                yield return x;
            }
        }
    }

    public override Iterable<A> Reverse() =>
        new IterableAsyncEnumerable<A>(AsAsyncEnumerableIO().Map(xs => xs.Reverse()));

    public override Iterable<B> Map<B>(Func<A, B> f) =>
        new IterableAsyncEnumerable<B>(AsAsyncEnumerableIO().Map(xs => xs.Select(f)));

    public override Iterable<A> Filter(Func<A, bool> f) =>
        new IterableAsyncEnumerable<A>(AsAsyncEnumerableIO().Map(xs => xs.Where(f)));

    public override IO<S> FoldWhileIO<S>(Func<S, A, S> f, Func<(S State, A Value), bool> predicate, S initialState) =>
        IO.liftVAsync(async env =>
                      {
                          var s  = initialState;
                          var xs = await runEnumerable.RunAsync(env);
                          await foreach (var x in xs.WithCancellation(env.Token))
                          {
                              if (!predicate((s, x)))
                              {
                                  return s;
                              }

                              s = f(s, x);
                          }

                          return s;
                      });

    public override IO<S> FoldUntilIO<S>(Func<S, A, S> f, Func<(S State, A Value), bool> predicate, S initialState) => 
        IO.liftVAsync(async env =>
                      {
                          var s  = initialState;
                          var xs = await runEnumerable.RunAsync(env);
                          await foreach (var x in xs.WithCancellation(env.Token))
                          {
                              s = f(s, x);
                              if (predicate((s, x)))
                              {
                                  return s;
                              }
                          }
                          return s;
                      });


    public override Iterable<A> Choose(Iterable<A> rhs) =>
        new IterableAsyncEnumerable<A>(
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
                          }));

    /// <summary>
    /// If this sequence is empty, return the other sequence, otherwise return this sequence.
    /// </summary>
    /// <param name="rhs">Right hand side of the operator</param>
    /// <returns>A choice between two sequences based</returns>
    [Pure]
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

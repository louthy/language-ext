using System;
using System.Collections.Generic;
using System.Linq;

namespace LanguageExt;

sealed class IterableZip<A, B>(Iterable<A> First, Iterable<B> Second) : Iterable<(A First, B Second)>
{
    internal override bool IsAsync =>
        First.IsAsync || Second.IsAsync;

    public override IO<int> CountIO() =>
        IsAsync
            ? IO.liftVAsync(async env => await (await AsAsyncEnumerableIO().RunAsync(env)).CountAsync(env.Token))
            : IO.lift(env => AsEnumerableIO().Run(env).Count());

    public override IO<IEnumerable<(A First, B Second)>> AsEnumerableIO()
    {
        return IO.lift(e => go(e, First, Second));

        static IEnumerable<(A, B)> go(EnvIO env, Iterable<A> fst, Iterable<B> snd)
        {
            using var iterA = fst.AsEnumerable(env.Token).GetEnumerator();
            using var iterB = snd.AsEnumerable(env.Token).GetEnumerator();

            while (iterA.MoveNext() && iterB.MoveNext())
            {
                yield return (iterA.Current, iterB.Current);
            }
        }
    }

    public override IO<IAsyncEnumerable<(A First, B Second)>> AsAsyncEnumerableIO()
    {
        return (First.IsAsync, Second.IsAsync) switch
               {
                   (true, true)   => IO.lift(e => AsyncAsync(e, First, Second)),
                   (true, false)  => IO.lift(e => AsyncSync(e, First, Second)),
                   (false, true)  => IO.lift(e => SyncAsync(e, First, Second)),
                   (false, false) => IO.lift(e => SyncSync(e, First, Second).ToAsyncEnumerable()),
               };

        static async IAsyncEnumerable<(A, B)> AsyncAsync(EnvIO env, Iterable<A> fst, Iterable<B> snd)
        {
            var iterA = fst.AsAsyncEnumerable(env.Token).GetAsyncEnumerator(env.Token);
            var iterB = snd.AsAsyncEnumerable(env.Token).GetAsyncEnumerator(env.Token);

            while (await iterA.MoveNextAsync() && await iterB.MoveNextAsync())
            {
                yield return (iterA.Current, iterB.Current);
            }
        }

        static async IAsyncEnumerable<(A, B)> AsyncSync(EnvIO env, Iterable<A> fst, Iterable<B> snd)
        {
            var       iterA = fst.AsAsyncEnumerable(env.Token).GetAsyncEnumerator(env.Token);
            using var iterB = snd.AsEnumerable(env.Token).GetEnumerator();

            while (await iterA.MoveNextAsync() && iterB.MoveNext())
            {
                yield return (iterA.Current, iterB.Current);
            }
        }        

        static async IAsyncEnumerable<(A, B)> SyncAsync(EnvIO env, Iterable<A> fst, Iterable<B> snd)
        {
            using var iterA = fst.AsEnumerable(env.Token).GetEnumerator();
            var       iterB = snd.AsAsyncEnumerable(env.Token).GetAsyncEnumerator(env.Token);

            while (iterA.MoveNext() && await iterB.MoveNextAsync())
            {
                yield return (iterA.Current, iterB.Current);
            }
        }

        static IEnumerable<(A, B)> SyncSync(EnvIO env, Iterable<A> fst, Iterable<B> snd)
        {
            using var iterA = fst.AsEnumerable(env.Token).GetEnumerator();
            using var iterB = snd.AsEnumerable(env.Token).GetEnumerator();

            while (iterA.MoveNext() && iterB.MoveNext())
            {
                yield return (iterA.Current, iterB.Current);
            }
        }
    }

    public override Iterable<(A First, B Second)> Reverse() =>
        Make().Reverse();

    public override Iterable<C> Map<C>(Func<(A First, B Second), C> f) => 
        Make().Map(f);

    public override Iterable<(A First, B Second)> Filter(Func<(A First, B Second), bool> f) => 
        Make().Filter(f);

    public override IO<S> FoldWhileIO<S>(Func<(A First, B Second), Func<S, S>> f, Func<(S State, (A First, B Second) Value), bool> predicate, S initialState) => 
        Make().FoldWhileIO(f, predicate, initialState);

    public override IO<S> FoldWhileIO<S>(Func<S, (A First, B Second), S> f, Func<(S State, (A First, B Second) Value), bool> predicate, S initialState) => 
        Make().FoldWhileIO(f, predicate, initialState);

    public override IO<S> FoldUntilIO<S>(Func<(A First, B Second), Func<S, S>> f, Func<(S State, (A First, B Second) Value), bool> predicate, S initialState) => 
        Make().FoldUntilIO(f, predicate, initialState);

    public override IO<S> FoldUntilIO<S>(Func<S, (A First, B Second), S> f, Func<(S State, (A First, B Second) Value), bool> predicate, S initialState) => 
        Make().FoldUntilIO(f, predicate, initialState);
    
    public override Iterable<(A First, B Second)> Choose(Iterable<(A First, B Second)> rhs) =>
        new IterableAsyncEnumerable<(A First, B Second)>(
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

    public override Iterable<(A First, B Second)> Choose(Memo<Iterable, (A First, B Second)> rhs) => 
        new IterableAsyncEnumerable<(A First, B Second)>(
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

    Iterable<(A First, B Second)> Make() =>
        IsAsync
            ? new IterableAsyncEnumerable<(A First, B Second)>(AsAsyncEnumerableIO())
            : new IterableEnumerable<(A First, B Second)>(AsEnumerableIO());
    
}

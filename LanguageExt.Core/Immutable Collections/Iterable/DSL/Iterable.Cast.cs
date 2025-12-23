using System;
using System.Collections.Generic;
using System.Linq;

namespace LanguageExt;

sealed class IterableCast<X, A>(Iterable<X> Items) : Iterable<A>
{
    internal override bool IsAsync =>
        Items.IsAsync;
    
    public override IO<int> CountIO() =>
        Items.CountIO();

    public override IO<IEnumerable<A>> AsEnumerableIO()
    {
        return Items.AsEnumerableIO()
                    .Map(xs => xs as IEnumerable<A> ?? go(xs));
        
        IEnumerable<A> go(IEnumerable<X> xs)
        {
            foreach (object? x in xs)
            {
                if (x is A y) yield return y;
            }
        }
    }

    public override IO<IAsyncEnumerable<A>> AsAsyncEnumerableIO()
    {
        return Items.AsAsyncEnumerableIO()
                    .Map(xs => xs as IAsyncEnumerable<A> ?? go(xs));
        
        async IAsyncEnumerable<A> go(IAsyncEnumerable<X> xs)
        {
            await foreach (object? x in xs)
            {
                if (x is A y) yield return y;
            }
        }
    }

    public override Iterable<A> Reverse() =>
        new IterableCast<X, A>(Items.Reverse());

    public override Iterable<B> Map<B>(Func<A, B> f) =>
        Make().Map(f);

    public override Iterable<A> Filter(Func<A, bool> f) =>
        Make().Filter(f);

    public override IO<S> FoldWhileIO<S>(Func<A, Func<S, S>> f, Func<(S State, A Value), bool> predicate, S initialState) => 
        Make().FoldWhileIO(f, predicate, initialState);

    public override IO<S> FoldWhileIO<S>(Func<S, A, S> f, Func<(S State, A Value), bool> predicate, S initialState) => 
        Make().FoldWhileIO(f, predicate, initialState);

    public override IO<S> FoldUntilIO<S>(Func<A, Func<S, S>> f, Func<(S State, A Value), bool> predicate, S initialState) => 
        Make().FoldUntilIO(f, predicate, initialState);

    public override IO<S> FoldUntilIO<S>(Func<S, A, S> f, Func<(S State, A Value), bool> predicate, S initialState) => 
        Make().FoldUntilIO(f, predicate, initialState);

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

    Iterable<A> Make() =>
        IsAsync
            ? new IterableEnumerable<A>(AsEnumerableIO())
            : new IterableAsyncEnumerable<A>(AsAsyncEnumerableIO());
}

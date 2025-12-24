using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.Traits;

namespace LanguageExt;

sealed class IterableConcat<A>(Seq<Iterable<A>> Items) : Iterable<A>
{
    public Seq<Iterable<A>> Items { get; } = Items;

    internal override bool IsAsync =>
        Items.Exists(x => x.IsAsync);
    
    public override IO<int> CountIO() =>
        +Items.FoldM(0, (s, iter) => iter.CountIO().Map(c => c + s));

    public override IO<IEnumerable<A>> AsEnumerableIO()
    {
        return +go(Items);
        K<IO, IEnumerable<A>> go(Seq<Iterable<A>> items) =>
            items switch
            {
                []         => IO.pure(Enumerable.Empty<A>()),
                var (h, t) => +h.AsEnumerableIO() >> (xs => xs.Concat * go(t))
            };
    }

    public override IO<IAsyncEnumerable<A>> AsAsyncEnumerableIO() 
    {
        return go(Items);
        IO<IAsyncEnumerable<A>> go(Seq<Iterable<A>> items) =>
            items switch
            {
                []         => IO.pure(AsyncEnumerable.Empty<A>()),
                var (h, t) => from xs in h.AsAsyncEnumerableIO()
                              from ys in go(t)
                              select xs.Concat<A>(ys)
            };
    }

    public override Iterable<A> Reverse() =>
        new IterableConcat<A>(Items.Map(xs => xs.Reverse()).Rev());

    public override Iterable<B> Map<B>(Func<A, B> f) =>
        new IterableConcat<B>(Items.Map(xs => xs.Map(f)));

    public override Iterable<A> Filter(Func<A, bool> f) =>
        new IterableConcat<A>(Items.Map(xs => xs.Filter(f)));

    public override IO<S> FoldWhileIO<S>(Func<A, Func<S, S>> f, Func<(S State, A Value), bool> predicate, S initialState) => 
        IO.liftVAsync(async env =>
                      {
                          var s  = initialState;
                          foreach (var xxs in Items)
                          {
                              if (xxs.IsAsync)
                              {
                                  await foreach (var x in xxs.AsAsyncEnumerable(env.Token))
                                  {
                                      if (!predicate((s, x)))
                                      {
                                          return s;
                                      }

                                      s = f(x)(s);
                                  }
                              }
                              else
                              {
                                  foreach (var x in xxs.AsEnumerable(env.Token))
                                  {
                                      if (!predicate((s, x)))
                                      {
                                          return s;
                                      }
                                      s = f(x)(s);
                                  }
                              }
                          }
                          return s;
                      });

    public override IO<S> FoldWhileIO<S>(Func<S, A, S> f, Func<(S State, A Value), bool> predicate, S initialState) => 
        IO.liftVAsync(async env =>
                      {
                          var s  = initialState;
                          foreach (var xxs in Items)
                          {
                              if (xxs.IsAsync)
                              {
                                  await foreach (var x in xxs.AsAsyncEnumerable(env.Token))
                                  {
                                      if (!predicate((s, x)))
                                      {
                                          return s;
                                      }
                                      s = f(s, x);
                                  }
                              }
                              else
                              {
                                  foreach (var x in xxs.AsEnumerable(env.Token))
                                  {
                                      if (!predicate((s, x)))
                                      {
                                          return s;
                                      }
                                      s = f(s, x);
                                  }
                              }
                          }
                          return s;
                      });
        

    public override IO<S> FoldUntilIO<S>(Func<A, Func<S, S>> f, Func<(S State, A Value), bool> predicate, S initialState) => 
        IO.liftVAsync(async env =>
                      {
                          var s  = initialState;
                          foreach (var xxs in Items)
                          {
                              if (xxs.IsAsync)
                              {
                                  await foreach (var x in xxs.AsAsyncEnumerable(env.Token))
                                  {
                                      s = f(x)(s);
                                      if (predicate((s, x)))
                                      {
                                          return s;
                                      }
                                  }
                              }
                              else
                              {
                                  foreach (var x in xxs.AsEnumerable(env.Token))
                                  {
                                      s = f(x)(s);
                                      if (predicate((s, x)))
                                      {
                                          return s;
                                      }
                                  }
                              }
                          }
                          return s;
                      });

    public override IO<S> FoldUntilIO<S>(Func<S, A, S> f, Func<(S State, A Value), bool> predicate, S initialState) => 
        IO.liftVAsync(async env =>
                      {
                          var s  = initialState;
                          foreach (var xxs in Items)
                          {
                              if (xxs.IsAsync)
                              {
                                  await foreach (var x in xxs.AsAsyncEnumerable(env.Token))
                                  {
                                      s = f(s, x);
                                      if (predicate((s, x)))
                                      {
                                          return s;
                                      }
                                  }
                              }
                              else
                              {
                                  foreach (var x in xxs.AsEnumerable(env.Token))
                                  {
                                      s = f(s, x);
                                      if (predicate((s, x)))
                                      {
                                          return s;
                                      }
                                  }
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

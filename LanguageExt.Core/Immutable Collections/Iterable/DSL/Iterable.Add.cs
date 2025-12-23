using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LanguageExt;

sealed class IterableAdd<A>(SeqStrict<A> Prefix, Iterable<A> Source, SeqStrict<A> Postfix) : Iterable<A>
{
    internal override bool IsAsync =>
        Source.IsAsync;
    
    public override IO<int> CountIO() =>
        Source.CountIO().Map(c => Prefix.Count + c + Postfix.Count);

    public override Iterable<A> Add(A item) =>
        new IterableAdd<A>(Prefix, Source, (SeqStrict<A>)Postfix.Add(item));

    public override Iterable<A> Cons(A item) =>
        new IterableAdd<A>((SeqStrict<A>)Prefix.Cons(item), Source, Postfix);

    public override IO<IEnumerable<A>> AsEnumerableIO()
    {
        return IO.lift(go);
        IEnumerable<A> go(EnvIO env)
        {
            foreach (var x in Prefix)
            {
                if (env.Token.IsCancellationRequested) throw new TaskCanceledException();
                yield return x;
            }

            foreach (var x in Source.AsEnumerable(env.Token))
            {
                if(env.Token.IsCancellationRequested) throw new TaskCanceledException();
                yield return x;
            }

            foreach (var x in Postfix)
            {
                if (env.Token.IsCancellationRequested) throw new TaskCanceledException();
                yield return x;
            }
        }
    }

    public override IO<IAsyncEnumerable<A>> AsAsyncEnumerableIO()
    {
        return IO.lift(go);
        async IAsyncEnumerable<A> go(EnvIO env)
        {
            foreach (var x in Prefix)
            {
                if (env.Token.IsCancellationRequested) throw new TaskCanceledException();
                yield return x;
            }

            await foreach (var x in Source.AsAsyncEnumerable(env.Token))
            {
                yield return x;
            }

            foreach (var x in Postfix)
            {
                if (env.Token.IsCancellationRequested) throw new TaskCanceledException();
                yield return x;
            }
        }
    }

    public override Iterable<A> Reverse() =>
        new IterableAdd<A>(Postfix.Rev(), Source.Rev(), Prefix.Rev());

    public override Iterable<B> Map<B>(Func<A, B> f) =>
        new IterableAdd<B>(Prefix.Map(f), Source.Map(f), Postfix.Map(f));

    public override Iterable<A> Filter(Func<A, bool> f) =>
        new IterableAdd<A>(Prefix.Filter(f), Source.Filter(f), Postfix.Filter(f));

    public override IO<S> FoldWhileIO<S>(
        Func<A, Func<S, S>> f,
        Func<(S State, A Value), bool> predicate,
        S initialState) =>
        Source.IsAsync
            ? IO.liftVAsync(async env =>
                            {
                                var s = initialState;

                                foreach (var x in Prefix)
                                {
                                    if (!predicate((s, x)))
                                    {
                                        return s;
                                    }

                                    s = f(x)(s);
                                }

                                await foreach (var x in Source.AsAsyncEnumerable(env.Token))
                                {
                                    if (!predicate((s, x)))
                                    {
                                        return s;
                                    }

                                    s = f(x)(s);
                                }

                                foreach (var x in Postfix)
                                {
                                    if (!predicate((s, x)))
                                    {
                                        return s;
                                    }

                                    s = f(x)(s);
                                }

                                return s;
                            })
            : IO.lift(env =>
                      {
                          var s = initialState;

                          foreach (var x in Prefix)
                          {
                              if (!predicate((s, x)))
                              {
                                  return s;
                              }

                              s = f(x)(s);
                          }

                          foreach (var x in Source.AsEnumerable(env.Token))
                          {
                              if (!predicate((s, x)))
                              {
                                  return s;
                              }

                              s = f(x)(s);
                          }

                          foreach (var x in Postfix)
                          {
                              if (!predicate((s, x)))
                              {
                                  return s;
                              }

                              s = f(x)(s);
                          }

                          return s;
                      });

    public override IO<S> FoldWhileIO<S>(Func<S, A, S> f, Func<(S State, A Value), bool> predicate, S initialState) =>
        Source.IsAsync
            ? IO.liftVAsync(async env =>
                            {
                                var s = initialState;

                                foreach (var x in Prefix)
                                {
                                    if (!predicate((s, x)))
                                    {
                                        return s;
                                    }

                                    s = f(s, x);
                                }

                                await foreach (var x in Source.AsAsyncEnumerable(env.Token))
                                {
                                    if (!predicate((s, x)))
                                    {
                                        return s;
                                    }

                                    s = f(s, x);
                                }

                                foreach (var x in Postfix)
                                {
                                    if (!predicate((s, x)))
                                    {
                                        return s;
                                    }

                                    s = f(s, x);
                                }

                                return s;
                            })
            : IO.lift(env =>
                      {
                          var s = initialState;

                          foreach (var x in Prefix)
                          {
                              if (!predicate((s, x)))
                              {
                                  return s;
                              }

                              s = f(s, x);
                          }

                          foreach (var x in Source.AsEnumerable(env.Token))
                          {
                              if (!predicate((s, x)))
                              {
                                  return s;
                              }

                              s = f(s, x);
                          }

                          foreach (var x in Postfix)
                          {
                              if (!predicate((s, x)))
                              {
                                  return s;
                              }

                              s = f(s, x);
                          }

                          return s;
                      });

    public override IO<S> FoldUntilIO<S>(
        Func<A, Func<S, S>> f, 
        Func<(S State, A Value), bool> predicate,
        S initialState) =>
        Source.IsAsync
            ? IO.liftVAsync(async env =>
                            {
                                var s = initialState;

                                foreach (var x in Prefix)
                                {
                                    s = f(x)(s);
                                    if (predicate((s, x)))
                                    {
                                        return s;
                                    }
                                }

                                await foreach (var x in Source.AsAsyncEnumerable(env.Token))
                                {
                                    s = f(x)(s);
                                    if (predicate((s, x)))
                                    {
                                        return s;
                                    }
                                }

                                foreach (var x in Postfix)
                                {
                                    s = f(x)(s);
                                    if (predicate((s, x)))
                                    {
                                        return s;
                                    }
                                }
                                return s;
                            })
            : IO.lift(env =>
                      {
                          var s = initialState;

                          foreach (var x in Prefix)
                          {
                              s = f(x)(s);
                              if (predicate((s, x)))
                              {
                                  return s;
                              }
                          }

                          foreach (var x in Source.AsEnumerable(env.Token))
                          {
                              s = f(x)(s);
                              if (predicate((s, x)))
                              {
                                  return s;
                              }
                          }

                          foreach (var x in Postfix)
                          {
                              s = f(x)(s);
                              if (predicate((s, x)))
                              {
                                  return s;
                              }
                          }

                          return s;
                      });

    public override IO<S> FoldUntilIO<S>(Func<S, A, S> f, Func<(S State, A Value), bool> predicate, S initialState) => 
        Source.IsAsync
            ? IO.liftVAsync(async env =>
                            {
                                var s = initialState;

                                foreach (var x in Prefix)
                                {
                                    s = f(s, x);
                                    if (predicate((s, x)))
                                    {
                                        return s;
                                    }
                                }

                                await foreach (var x in Source.AsAsyncEnumerable(env.Token))
                                {
                                    s = f(s, x);
                                    if (predicate((s, x)))
                                    {
                                        return s;
                                    }
                                }

                                foreach (var x in Postfix)
                                {
                                    s = f(s, x);
                                    if (predicate((s, x)))
                                    {
                                        return s;
                                    }
                                }
                                return s;
                            })
            : IO.lift(env =>
                      {
                          var s = initialState;

                          foreach (var x in Prefix)
                          {
                              s = f(s, x);
                              if (predicate((s, x)))
                              {
                                  return s;
                              }
                          }

                          foreach (var x in Source.AsEnumerable(env.Token))
                          {
                              s = f(s, x);
                              if (predicate((s, x)))
                              {
                                  return s;
                              }
                          }

                          foreach (var x in Postfix)
                          {
                              s = f(s, x);
                              if (predicate((s, x)))
                              {
                                  return s;
                              }
                          }

                          return s;
                      });

    public override Iterable<A> Choose(Iterable<A> rhs)
    {
        if (!Prefix.IsEmpty) return this;
        return new IterableAsyncEnumerable<A>(
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
    }

    public override Iterable<A> Choose(Memo<Iterable, A> rhs) 
    {
        if (!Prefix.IsEmpty) return this;
        return new IterableAsyncEnumerable<A>(
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

}

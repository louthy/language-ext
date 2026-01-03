#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LanguageExt;

sealed class IterableStrict<A>(SeqStrict<A> Items) : Iterable<A>
{
    internal override bool IsAsync =>
        false;
    
    public override IO<int> CountIO() =>
        IO.pure(Items.Count);

    public override Iterable<A> Add(A item) =>
        new IterableStrict<A>((SeqStrict<A>)Items.Add(item));

    public override Iterable<A> Cons(A item) =>
        new IterableStrict<A>((SeqStrict<A>)Items.Cons(item));

    public override IO<IEnumerable<A>> AsEnumerableIO()
    {
        return IO.lift(go);
        IEnumerable<A> go(EnvIO env)
        {
            foreach (var x in Items)
            {
                if (env.Token.IsCancellationRequested) throw new OperationCanceledException();
                yield return x;
            }
        }
    }

    public override IO<IAsyncEnumerable<A>> AsAsyncEnumerableIO()
    {
        return IO.lift(go);
        async IAsyncEnumerable<A> go(EnvIO env)
        {
            foreach (var x in Items)
            {
                if (env.Token.IsCancellationRequested) throw new OperationCanceledException();
                yield return x;
            }
        }
    }

    public override Iterable<A> Reverse() =>
        new IterableStrict<A>(Items.Rev());

    public override Iterable<B> Map<B>(Func<A, B> f) =>
        new IterableStrict<B>(Items.Map(f));

    public override Iterable<A> Filter(Func<A, bool> f) =>
        new IterableStrict<A>(Items.Filter(f));

    public override IO<S> FoldWhileIO<S>(Func<S, A, S> f, Func<(S State, A Value), bool> predicate, S initialState) => 
        IO.liftVAsync(async env =>
                      {
                          var s  = initialState;
                          foreach (var x in Items)
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

    public override IO<S> FoldUntilIO<S>(Func<S, A, S> f, Func<(S State, A Value), bool> predicate, S initialState) => 
        IO.liftVAsync(async env =>
                      {
                          var s  = initialState;
                          foreach (var x in Items)
                          {
                              if(env.Token.IsCancellationRequested) throw new OperationCanceledException();
                              s = f(s, x);
                              if (predicate((s, x)))
                              {
                                  return s;
                              }
                          }
                          return s;
                      });

    public override Iterable<A> Choose(Iterable<A> rhs) => 
        Items.IsEmpty
            ? rhs
            : this;

    public override Iterable<A> Choose(Memo<Iterable, A> rhs) => 
        Items.IsEmpty
            ? rhs.Value.As()
            : this;
}

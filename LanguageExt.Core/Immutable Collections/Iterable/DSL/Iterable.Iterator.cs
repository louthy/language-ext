#pragma warning disable CS1998
using System;
using System.Linq;
using System.Collections.Generic;

namespace LanguageExt;

sealed class IterableIterator<A>(Iterator<A> iterator) : Iterable<A>
{
    internal override bool IsAsync =>
        false;
    
    public override IO<int> CountIO() =>
        AsEnumerableIO().Map(xs => xs.Count());

    public override IO<IEnumerable<A>> AsEnumerableIO() =>
        IO.lift(iterator.AsEnumerable);

    public override IO<IAsyncEnumerable<A>> AsAsyncEnumerableIO() =>
        AsEnumerableIO().Map(xs => xs.ToAsyncEnumerable());

    public override Iterable<A> Reverse() =>
        new IterableIterator<A>(iterator.Reverse());

    public override Iterable<B> Map<B>(Func<A, B> f) =>
        new IterableIterator<B>(iterator.Map(f));

    public override Iterable<A> Filter(Func<A, bool> f) =>
        new IterableIterator<A>(iterator.Filter(f));

    public override IO<S> FoldWhileIO<S>(Func<S, A, S> f, Func<(S State, A Value), bool> predicate, S initialState) =>
        IO.lift(env =>
                {
                    var s = initialState;
                    for (var i = iterator; i is (Exist<A> (var x), var tail); i = tail)
                    {
                        if (env.Token.IsCancellationRequested) throw new OperationCanceledException();
                        if (!predicate((s, x)))
                        {
                            return s;
                        }

                        s = f(s, x);
                    }

                    return s;
                });


    public override IO<S> FoldUntilIO<S>(Func<S, A, S> f, Func<(S State, A Value), bool> predicate, S initialState) =>
        IO.lift(env =>
                {
                    var s  = initialState;
                    for (var i = iterator; i is (Exist<A> (var x), var tail); i = tail)
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

    public override Iterator<A> ForwardIterator() =>
        iterator;
}

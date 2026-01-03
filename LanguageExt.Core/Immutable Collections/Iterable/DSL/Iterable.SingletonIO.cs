#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
using System;
using System.Collections.Generic;
using System.Linq;

namespace LanguageExt;

sealed class IterableSingletonIO<A>(IO<A> Value) : Iterable<A>
{
    internal override bool IsAsync =>
        false;

    public override IO<int> CountIO() =>
        IO.pure(1);

    public override IO<IEnumerable<A>> AsEnumerableIO() =>
        Value.Map(One);

    public override IO<IAsyncEnumerable<A>> AsAsyncEnumerableIO() =>
        Value.Map(OneAsync);

    static IEnumerable<A> One(A value)
    {
        yield return value;
    }

    static async IAsyncEnumerable<A> OneAsync(A value)
    {
        yield return value;
    }

    public override Iterable<A> Reverse() =>
        this;

    public override Iterable<B> Map<B>(Func<A, B> f) =>
        new IterableSingletonIO<B>(Value.Map(f));

    public override Iterable<A> Filter(Func<A, bool> f) =>
        new IterableEnumerable<A>(AsEnumerableIO().Map(xs => xs.Where(f)));

    public override IO<S> FoldWhileIO<S>(Func<S, A, S> f, Func<(S State, A Value), bool> predicate, S initialState) =>
        Value.Map(x => predicate((initialState, x)) ? f(initialState, x) : initialState);

    public override IO<S> FoldUntilIO<S>(Func<S, A, S> f, Func<(S State, A Value), bool> predicate, S initialState) =>
        Value.Map(x => f(initialState, x));

    public override Iterable<A> Choose(Iterable<A> rhs) =>
        this;

    public override Iterable<A> Choose(Memo<Iterable, A> rhs) =>
        this;
}
     

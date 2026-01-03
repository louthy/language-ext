#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
using System;
using System.Collections.Generic;

namespace LanguageExt;

sealed class IterableSingleton<A>(A Value) : Iterable<A>
{
    internal override bool IsAsync =>
        false;

    public override IO<int> CountIO() =>
        IO.pure(1);

    public override IO<IEnumerable<A>> AsEnumerableIO() =>
        IO.pure<IEnumerable<A>>([Value]);

    public override IO<IAsyncEnumerable<A>> AsAsyncEnumerableIO() =>
        IO.pure(One(Value));

    static async IAsyncEnumerable<A> One(A value)
    {
        yield return value;
    }

    public override Iterable<A> Reverse() =>
        this;

    public override Iterable<B> Map<B>(Func<A, B> f) =>
        new IterableSingleton<B>(f(Value));

    public override Iterable<A> Filter(Func<A, bool> f) =>
        f(Value) ? this : Empty;

    public override IO<S> FoldWhileIO<S>(Func<S, A, S> f, Func<(S State, A Value), bool> predicate, S initialState) =>
        IO.lift(_ => predicate((initialState, Value)) ? f(initialState, Value) : initialState);

    public override IO<S> FoldUntilIO<S>(Func<S, A, S> f, Func<(S State, A Value), bool> predicate, S initialState) =>
        IO.lift(_ => f(initialState, Value));

    public override Iterable<A> Choose(Iterable<A> rhs) =>
        this;

    public override Iterable<A> Choose(Memo<Iterable, A> rhs) =>
        this;
}
     

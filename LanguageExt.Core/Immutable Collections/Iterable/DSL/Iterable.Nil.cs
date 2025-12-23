using System;
using System.Collections.Generic;
using System.Linq;

namespace LanguageExt;

sealed class IterableNil<A> : Iterable<A>
{
    public static readonly Iterable<A> Default = new IterableNil<A>();

    internal override bool IsAsync =>
        false;

    public override IO<int> CountIO() =>
        IO.pure(0);

    public override IO<IEnumerable<A>> AsEnumerableIO() =>
        IO.pure(Enumerable.Empty<A>());

    public override IO<IAsyncEnumerable<A>> AsAsyncEnumerableIO() => 
        IO.pure(AsyncEnumerable.Empty<A>());

    public override Iterable<A> Reverse() =>
        this;

    public override Iterable<B> Map<B>(Func<A, B> f) => 
        IterableNil<B>.Default;

    public override Iterable<A> Filter(Func<A, bool> f) =>
        this;

    public override IO<S> FoldWhileIO<S>(Func<A, Func<S, S>> f, Func<(S State, A Value), bool> predicate, S initialState) => 
        IO.pure(initialState);

    public override IO<S> FoldWhileIO<S>(Func<S, A, S> f, Func<(S State, A Value), bool> predicate, S initialState) => 
        IO.pure(initialState);

    public override IO<S> FoldUntilIO<S>(Func<A, Func<S, S>> f, Func<(S State, A Value), bool> predicate, S initialState) => 
        IO.pure(initialState);

    public override IO<S> FoldUntilIO<S>(Func<S, A, S> f, Func<(S State, A Value), bool> predicate, S initialState) => 
        IO.pure(initialState);

    public override Iterable<A> Choose(Iterable<A> rhs) =>
        rhs;

    public override Iterable<A> Choose(Memo<Iterable, A> rhs) => 
        rhs.Value.As();
}

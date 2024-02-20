using System;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

public static class ValueTuple2Extensions
{
    /// <summary>
    /// Append an extra item to the tuple
    /// </summary>
    [Pure]
    public static (A, B, C) Add<A, B, C>(this (A, B) self, C third) =>
        (self.Item1, self.Item2, third);

    /// <summary>
    /// Take the first item
    /// </summary>
    [Pure]
    public static T1 Head<T1, T2>(this ValueTuple<T1, T2> self) =>
        self.Item1;

    /// <summary>
    /// Take the last item
    /// </summary>
    [Pure]
    public static T2 Last<T1, T2>(this ValueTuple<T1, T2> self) =>
        self.Item2;

    /// <summary>
    /// Take the second item onwards and build a new tuple
    /// </summary>
    [Pure]
    public static ValueTuple<T2> Tail<T1, T2>(this ValueTuple<T1, T2> self) =>
        new (self.Item2);

    /// <summary>
    /// Map
    /// </summary>
    [Pure]
    public static R Map<A, B, R>(this ValueTuple<A, B> self, Func<ValueTuple<A, B>, R> map) =>
        map(self);

    /// <summary>
    /// Map
    /// </summary>
    [Pure]
    public static R Map<A, B, R>(this ValueTuple<A, B> self, Func<A, B, R> map) =>
        map(self.Item1, self.Item2);

    /// <summary>
    /// Bi-map to tuple
    /// </summary>
    [Pure]
    public static ValueTuple<R1, R2> BiMap<T1, T2, R1, R2>(this ValueTuple<T1, T2> self, Func<T1, R1> firstMap, Func<T2, R2> secondMap) =>
        (firstMap(self.Item1), secondMap(self.Item2));

    /// <summary>
    /// First item-map to tuple
    /// </summary>
    [Pure]
    public static ValueTuple<R1, T2> MapFirst<T1, T2, R1>(this ValueTuple<T1, T2> self, Func<T1, R1> firstMap) =>
        (firstMap(self.Item1), self.Item2);

    /// <summary>
    /// Second item-map to tuple
    /// </summary>
    [Pure]
    public static ValueTuple<T1, R2> MapSecond<T1, T2, R2>(this ValueTuple<T1, T2> self, Func<T2, R2> secondMap) =>
        (self.Item1, secondMap(self.Item2));

    /// <summary>
    /// Map to tuple
    /// </summary>
    [Pure]
    public static ValueTuple<R1, R2> Select<T1, T2, R1, R2>(this ValueTuple<T1, T2> self, Func<ValueTuple<T1, T2>, ValueTuple<R1, R2>> map) =>
        map(self);

    /// <summary>
    /// Iterate
    /// </summary>
    public static Unit Iter<T1, T2>(this ValueTuple<T1, T2> self, Action<T1, T2> func)
    {
        func(self.Item1, self.Item2);
        return Unit.Default;
    }

    /// <summary>
    /// Iterate
    /// </summary>
    public static Unit Iter<T1, T2>(this ValueTuple<T1, T2> self, Action<T1> first, Action<T2> second)
    {
        first(self.Item1);
        second(self.Item2);
        return Unit.Default;
    }

    /// <summary>
    /// Fold
    /// </summary>
    [Pure]
    public static S Fold<T1, T2, S>(this ValueTuple<T1, T2> self, S state, Func<S, T1, T2, S> fold) =>
        fold(state, self.Item1, self.Item2);

    /// <summary>
    /// Bi-fold
    /// </summary>
    [Pure]
    public static S BiFold<T1, T2, S>(this ValueTuple<T1, T2> self, S state, Func<S, T1, S> firstFold, Func<S, T2, S> secondFold) =>
        secondFold(firstFold(state, self.Item1), self.Item2);

    /// <summary>
    /// Bi-fold
    /// </summary>
    [Pure]
    public static S BiFoldBack<T1, T2, S>(this ValueTuple<T1, T2> self, S state, Func<S, T2, S> firstFold, Func<S, T1, S> secondFold) =>
        secondFold(firstFold(state, self.Item2), self.Item1);

    /// <summary>
    /// Flip the tuple monads from inside the tuple to outside and apply a transformation function
    /// </summary>
    [Pure]
    public static Arr<(C, D)> Traverse<A, B, C, D>(this (Arr<A> ma, Arr<B> mb) tuple, Func<(A a, B b), (C c, D d)> f) =>
        from a in tuple.ma
        from b in tuple.mb
        let r = f((a, b))
        select (r.Item1, r.Item2);

    /// <summary>
    /// Flip the tuple monads from inside the tuple to outside and apply a transformation function
    /// </summary>
    [Pure]
    public static Arr<(C, D)> Traverse<A, B, C, D>(this (Arr<A> ma, Arr<B> mb) tuple, Func<A, B, (C c, D d)> f) =>
        from a in tuple.ma
        from b in tuple.mb
        let r = f(a, b)
        select (r.Item1, r.Item2);

    /// <summary>
    /// Flip the tuple monads from inside the tuple to outside and apply a transformation function
    /// </summary>
    [Pure]
    public static Arr<(A, B)> Sequence<A, B>(this (Arr<A> ma, Arr<B> mb) tuple) =>
        from a in tuple.ma
        from b in tuple.mb
        select (a, b);

    /// <summary>
    /// Flip the tuple monads from inside the tuple to outside and apply a transformation function
    /// </summary>
    [Pure]
    public static Either<L, (C, D)> Traverse<L, A, B, C, D>(this (Either<L, A> ma, Either<L, B> mb) tuple, Func<(A a, B b), (C c, D d)> f) =>
        from a in tuple.ma
        from b in tuple.mb
        let r = f((a, b))
        select (r.Item1, r.Item2);

    /// <summary>
    /// Flip the tuple monads from inside the tuple to outside and apply a transformation function
    /// </summary>
    [Pure]
    public static Either<L, (C, D)> Traverse<L, A, B, C, D>(this (Either<L, A> ma, Either<L, B> mb) tuple, Func<A, B, (C c, D d)> f) =>
        from a in tuple.ma
        from b in tuple.mb
        let r = f(a, b)
        select (r.Item1, r.Item2);

    /// <summary>
    /// Flip the tuple monads from inside the tuple to outside and apply a transformation function
    /// </summary>
    [Pure]
    public static Either<L, (A, B)> Sequence<L, A, B>(this (Either<L, A> ma, Either<L, B> mb) tuple) =>
        from a in tuple.ma
        from b in tuple.mb
        select (a, b);

    /// <summary>
    /// Flip the tuple monads from inside the tuple to outside and apply a transformation function
    /// </summary>
    [Pure]
    public static HashSet<(C, D)> Traverse<A, B, C, D>(this (HashSet<A> ma, HashSet<B> mb) tuple, Func<(A a, B b), (C c, D d)> f) =>
        from a in tuple.ma
        from b in tuple.mb
        let r = f((a, b))
        select (r.Item1, r.Item2);

    /// <summary>
    /// Flip the tuple monads from inside the tuple to outside and apply a transformation function
    /// </summary>
    [Pure]
    public static HashSet<(C, D)> Traverse<A, B, C, D>(this (HashSet<A> ma, HashSet<B> mb) tuple, Func<A, B, (C c, D d)> f) =>
        from a in tuple.ma
        from b in tuple.mb
        let r = f(a, b)
        select (r.Item1, r.Item2);

    /// <summary>
    /// Flip the tuple monads from inside the tuple to outside and apply a transformation function
    /// </summary>
    [Pure]
    public static HashSet<(A, B)> Sequence<A, B>(this (HashSet<A> ma, HashSet<B> mb) tuple) =>
        from a in tuple.ma
        from b in tuple.mb
        select (a, b);

    /// <summary>
    /// Flip the tuple monads from inside the tuple to outside and apply a transformation function
    /// </summary>
    [Pure]
    public static Lst<(C, D)> Traverse<A, B, C, D>(this (Lst<A> ma, Lst<B> mb) tuple, Func<(A a, B b), (C c, D d)> f) =>
        from a in tuple.ma
        from b in tuple.mb
        let r = f((a, b))
        select (r.Item1, r.Item2);

    /// <summary>
    /// Flip the tuple monads from inside the tuple to outside and apply a transformation function
    /// </summary>
    [Pure]
    public static Lst<(C, D)> Traverse<A, B, C, D>(this (Lst<A> ma, Lst<B> mb) tuple, Func<A, B, (C c, D d)> f) =>
        from a in tuple.ma
        from b in tuple.mb
        let r = f(a, b)
        select (r.Item1, r.Item2);

    /// <summary>
    /// Flip the tuple monads from inside the tuple to outside and apply a transformation function
    /// </summary>
    [Pure]
    public static Lst<(A, B)> Sequence<A, B>(this (Lst<A> ma, Lst<B> mb) tuple) =>
        from a in tuple.ma
        from b in tuple.mb
        select (a, b);

    /// <summary>
    /// Flip the tuple monads from inside the tuple to outside and apply a transformation function
    /// </summary>
    [Pure]
    public static Option<(C, D)> Traverse<A, B, C, D>(this (Option<A> ma, Option<B> mb) tuple, Func<(A a, B b), (C c, D d)> f) =>
        from a in tuple.ma
        from b in tuple.mb
        let r = f((a, b))
        select (r.Item1, r.Item2);

    /// <summary>
    /// Flip the tuple monads from inside the tuple to outside and apply a transformation function
    /// </summary>
    [Pure]
    public static Option<(C, D)> Traverse<A, B, C, D>(this (Option<A> ma, Option<B> mb) tuple, Func<A, B, (C c, D d)> f) =>
        from a in tuple.ma
        from b in tuple.mb
        let r = f(a, b)
        select (r.Item1, r.Item2);

    /// <summary>
    /// Flip the tuple monads from inside the tuple to outside and apply a transformation function
    /// </summary>
    [Pure]
    public static Option<(A, B)> Sequence<A, B>(this (Option<A> ma, Option<B> mb) tuple) =>
        from a in tuple.ma
        from b in tuple.mb
        select (a, b);

    /// <summary>
    /// Flip the tuple monads from inside the tuple to outside and apply a transformation function
    /// </summary>
    [Pure]
    public static Set<(C, D)> Traverse<A, B, C, D>(this (Set<A> ma, Set<B> mb) tuple, Func<(A a, B b), (C c, D d)> f) =>
        from a in tuple.ma
        from b in tuple.mb
        let r = f((a, b))
        select (r.Item1, r.Item2);

    /// <summary>
    /// Flip the tuple monads from inside the tuple to outside and apply a transformation function
    /// </summary>
    [Pure]
    public static Set<(C, D)> Traverse<A, B, C, D>(this (Set<A> ma, Set<B> mb) tuple, Func<A, B, (C c, D d)> f) =>
        from a in tuple.ma
        from b in tuple.mb
        let r = f(a, b)
        select (r.Item1, r.Item2);

    /// <summary>
    /// Flip the tuple monads from inside the tuple to outside and apply a transformation function
    /// </summary>
    [Pure]
    public static Set<(A, B)> Sequence<A, B>(this (Set<A> ma, Set<B> mb) tuple) =>
        from a in tuple.ma
        from b in tuple.mb
        select (a, b);

    /// <summary>
    /// Flip the tuple monads from inside the tuple to outside and apply a transformation function
    /// </summary>
    [Pure]
    public static Task<(C, D)> Traverse<A, B, C, D>(this (Task<A> ma, Task<B> mb) tuple, Func<(A a, B b), (C c, D d)> f) =>
        apply((a, b) => f((a, b)), tuple.ma, tuple.mb);

    /// <summary>
    /// Flip the tuple monads from inside the tuple to outside and apply a transformation function
    /// </summary>
    [Pure]
    public static Task<(C, D)> Traverse<A, B, C, D>(this (Task<A> ma, Task<B> mb) tuple, Func<A, B, (C c, D d)> f) =>
        apply((a, b) => f(a, b), tuple.ma, tuple.mb);

    /// <summary>
    /// Flip the tuads from inside the tuple to outside and apply a transformation function
    /// </summary>
    [Pure]
    public static Task<(A, B)> Sequence<A, B>(this (Task<A> ma, Task<B> mb) tuple) =>
        apply((a, b) => (a, b), tuple.ma, tuple.mb);
}

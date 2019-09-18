using System;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using System.Diagnostics.Contracts;
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
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
    /// Semigroup append
    /// </summary>
    [Pure]
    public static (A, B) Append<SemiA, SemiB, A, B>(this(A, B) a, (A, B) b)
        where SemiA : struct, Semigroup<A>
        where SemiB : struct, Semigroup<B> =>
        (default(SemiA).Append(a.Item1, b.Item1),
         default(SemiB).Append(a.Item2, b.Item2));

    /// <summary>
    /// Semigroup append
    /// </summary>
    [Pure]
    public static A Append<SemiA, A>(this ValueTuple<A, A> a)
        where SemiA : struct, Semigroup<A> =>
        default(SemiA).Append(a.Item1, a.Item2);

    /// <summary>
    /// Monoid concat
    /// </summary>
    [Pure]
    public static (A, B) Concat<MonoidA, MonoidB, A, B>(this (A, B) a, (A, B) b)
        where MonoidA : struct, Monoid<A>
        where MonoidB : struct, Monoid<B> =>
        (mconcat<MonoidA, A>(a.Item1, b.Item1),
         mconcat<MonoidB, B>(a.Item2, b.Item2));

    /// <summary>
    /// Monoid concat
    /// </summary>
    [Pure]
    public static A Concat<MonoidA, A>(this ValueTuple<A, A> a)
        where MonoidA : struct, Monoid<A> =>
        mconcat<MonoidA, A>(a.Item1, a.Item2);

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
        VTuple(self.Item2);

    /// <summary>
    /// Sum of the items
    /// </summary>
    [Pure]
    public static A Sum<NUM, A>(this ValueTuple<A, A> self)
        where NUM : struct, Num<A> =>
        TypeClass.sum<NUM, FoldTuple<A>, ValueTuple<A, A>, A>(self);

    /// <summary>
    /// Product of the items
    /// </summary>
    [Pure]
    public static A Product<NUM, A>(this ValueTuple<A, A> self)
        where NUM : struct, Num<A> =>
        TypeClass.product<NUM, FoldTuple<A>, ValueTuple<A, A>, A>(self);

    /// <summary>
    /// One of the items matches the value passed
    /// </summary>
    [Pure]
    public static bool Contains<EQ, A>(this ValueTuple<A, A> self, A value)
        where EQ : struct, Eq<A> =>
        TypeClass.contains<EQ, FoldTuple<A>, ValueTuple<A, A>, A>(self, value);

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
    public static EitherUnsafe<L, (C, D)> Traverse<L, A, B, C, D>(this (EitherUnsafe<L, A> ma, EitherUnsafe<L, B> mb) tuple, Func<(A a, B b), (C c, D d)> f) =>
        from a in tuple.ma
        from b in tuple.mb
        let r = f((a, b))
        select (r.Item1, r.Item2);

    /// <summary>
    /// Flip the tuple monads from inside the tuple to outside and apply a transformation function
    /// </summary>
    [Pure]
    public static EitherUnsafe<L, (C, D)> Traverse<L, A, B, C, D>(this (EitherUnsafe<L, A> ma, EitherUnsafe<L, B> mb) tuple, Func<A, B, (C c, D d)> f) =>
        from a in tuple.ma
        from b in tuple.mb
        let r = f(a, b)
        select (r.Item1, r.Item2);

    /// <summary>
    /// Flip the tuple monads from inside the tuple to outside and apply a transformation function
    /// </summary>
    [Pure]
    public static EitherUnsafe<L, (A, B)> Sequence<L, A, B>(this (EitherUnsafe<L, A> ma, EitherUnsafe<L, B> mb) tuple) =>
        from a in tuple.ma
        from b in tuple.mb
        select (a, b);

    /// <summary>
    /// Flip the tuple monads from inside the tuple to outside and apply a transformation function
    /// </summary>
    [Pure]
    public static EitherAsync<L, (C, D)> Traverse<L, A, B, C, D>(this (EitherAsync<L, A> ma, EitherAsync<L, B> mb) tuple, Func<(A a, B b), (C c, D d)> f) =>
        apply((a, b) => f((a, b)), tuple.ma, tuple.mb);

    /// <summary>
    /// Flip the tuple monads from inside the tuple to outside and apply a transformation function
    /// </summary>
    [Pure]
    public static EitherAsync<L, (C, D)> Traverse<L, A, B, C, D>(this (EitherAsync<L, A> ma, EitherAsync<L, B> mb) tuple, Func<A, B, (C c, D d)> f) =>
        apply((a, b) => f(a, b), tuple.ma, tuple.mb);

    /// <summary>
    /// Flip the tuple monads from inside the tuple to outside and apply a transformation function
    /// </summary>
    [Pure]
    public static EitherAsync<L, (A, B)> Sequence<L, A, B>(this (EitherAsync<L, A> ma, EitherAsync<L, B> mb) tuple) =>
        apply((a, b) => (a, b), tuple.ma, tuple.mb);

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
    public static OptionUnsafe<(C, D)> Traverse<A, B, C, D>(this (OptionUnsafe<A> ma, OptionUnsafe<B> mb) tuple, Func<(A a, B b), (C c, D d)> f) =>
        from a in tuple.ma
        from b in tuple.mb
        let r = f((a, b))
        select (r.Item1, r.Item2);

    /// <summary>
    /// Flip the tuple monads from inside the tuple to outside and apply a transformation function
    /// </summary>
    [Pure]
    public static OptionUnsafe<(C, D)> Traverse<A, B, C, D>(this (OptionUnsafe<A> ma, OptionUnsafe<B> mb) tuple, Func<A, B, (C c, D d)> f) =>
        from a in tuple.ma
        from b in tuple.mb
        let r = f(a, b)
        select (r.Item1, r.Item2);

    /// <summary>
    /// Flip the tuple monads from inside the tuple to outside and apply a transformation function
    /// </summary>
    [Pure]
    public static OptionUnsafe<(A, B)> Sequence<A, B>(this (OptionUnsafe<A> ma, OptionUnsafe<B> mb) tuple) =>
        from a in tuple.ma
        from b in tuple.mb
        select (a, b);

    /// <summary>
    /// Flip the tuple monads from inside the tuple to outside and apply a transformation function
    /// </summary>
    [Pure]
    public static OptionAsync<(C, D)> Traverse<A, B, C, D>(this (OptionAsync<A> ma, OptionAsync<B> mb) tuple, Func<(A a, B b), (C c, D d)> f) =>
        apply((a, b) => f((a, b)), tuple.ma, tuple.mb);

    /// <summary>
    /// Flip the tuple monads from inside the tuple to outside and apply a transformation function
    /// </summary>
    [Pure]
    public static OptionAsync<(C, D)> Traverse<A, B, C, D>(this (OptionAsync<A> ma, OptionAsync<B> mb) tuple, Func<A, B, (C c, D d)> f) =>
        apply((a, b) => f(a, b), tuple.ma, tuple.mb);

    /// <summary>
    /// Flip the tuple monads from inside the tuple to outside and apply a transformation function
    /// </summary>
    [Pure]
    public static OptionAsync<(A, B)> Sequence<A, B>(this (OptionAsync<A> ma, OptionAsync<B> mb) tuple) =>
        apply((a, b) => (a, b), tuple.ma, tuple.mb);

    /// <summary>
    /// Flip the tuple monads from inside the tuple to outside and apply a transformation function
    /// </summary>
    [Pure]
    public static Reader<Env, (C, D)> Traverse<Env, A, B, C, D>(this (Reader<Env, A> ma, Reader<Env, B> mb) tuple, Func<(A a, B b), (C c, D d)> f) =>
        from a in tuple.ma
        from b in tuple.mb
        let r = f((a, b))
        select (r.Item1, r.Item2);

    /// <summary>
    /// Flip the tuple monads from inside the tuple to outside and apply a transformation function
    /// </summary>
    [Pure]
    public static Reader<Env, (C, D)> Traverse<Env, A, B, C, D>(this (Reader<Env, A> ma, Reader<Env, B> mb) tuple, Func<A, B, (C c, D d)> f) =>
        from a in tuple.ma
        from b in tuple.mb
        let r = f(a, b)
        select (r.Item1, r.Item2);

    /// <summary>
    /// Flip the tuple monads from inside the tuple to outside and apply a transformation function
    /// </summary>
    [Pure]
    public static Reader<Env, (A, B)> Sequence<Env, A, B>(this (Reader<Env, A> ma, Reader<Env, B> mb) tuple) =>
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
    public static State<S, (C, D)> Traverse<S, A, B, C, D>(this (State<S, A> ma, State<S, B> mb) tuple, Func<(A a, B b), (C c, D d)> f) =>
        from a in tuple.ma
        from b in tuple.mb
        let r = f((a, b))
        select (r.Item1, r.Item2);

    /// <summary>
    /// Flip the tuple monads from inside the tuple to outside and apply a transformation function
    /// </summary>
    [Pure]
    public static State<S, (C, D)> Traverse<S, A, B, C, D>(this (State<S, A> ma, State<S, B> mb) tuple, Func<A, B, (C c, D d)> f) =>
        from a in tuple.ma
        from b in tuple.mb
        let r = f(a, b)
        select (r.Item1, r.Item2);

    /// <summary>
    /// Flip the tuple monads from inside the tuple to outside and apply a transformation function
    /// </summary>
    [Pure]
    public static State<S, (A, B)> Sequence<S, A, B>(this (State<S, A> ma, State<S, B> mb) tuple) =>
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

    /// <summary>
    /// Flip the tuple monads from inside the tuple to outside and apply a transformation function
    /// </summary>
    [Pure]
    public static Try<(C, D)> Traverse<A, B, C, D>(this (Try<A> ma, Try<B> mb) tuple, Func<(A a, B b), (C c, D d)> f) =>
        from a in tuple.ma
        from b in tuple.mb
        let r = f((a, b))
        select (r.Item1, r.Item2);

    /// <summary>
    /// Flip the tuple monads from inside the tuple to outside and apply a transformation function
    /// </summary>
    [Pure]
    public static Try<(C, D)> Traverse<A, B, C, D>(this (Try<A> ma, Try<B> mb) tuple, Func<A, B, (C c, D d)> f) =>
        from a in tuple.ma
        from b in tuple.mb
        let r = f(a, b)
        select (r.Item1, r.Item2);

    /// <summary>
    /// Flip the tuple monads from inside the tuple to outside and apply a transformation function
    /// </summary>
    [Pure]
    public static Try<(A, B)> Sequence<A, B>(this (Try<A> ma, Try<B> mb) tuple) =>
        from a in tuple.ma
        from b in tuple.mb
        select (a, b);

    /// <summary>
    /// Flip the tuple monads from inside the tuple to outside and apply a transformation function
    /// </summary>
    [Pure]
    public static TryAsync<(C, D)> Traverse<A, B, C, D>(this (TryAsync<A> ma, TryAsync<B> mb) tuple, Func<(A a, B b), (C c, D d)> f) =>
        apply((a, b) => f((a, b)), tuple.ma, tuple.mb);

    /// <summary>
    /// Flip the tuple monads from inside the tuple to outside and apply a transformation function
    /// </summary>
    [Pure]
    public static TryAsync<(C, D)> Traverse<A, B, C, D>(this (TryAsync<A> ma, TryAsync<B> mb) tuple, Func<A, B, (C c, D d)> f) =>
        apply((a, b) => f(a, b), tuple.ma, tuple.mb);

    /// <summary>
    /// Flip the tuple monads from inside the tuple to outside and apply a transformation function
    /// </summary>
    [Pure]
    public static TryAsync<(A, B)> Sequence<A, B>(this (TryAsync<A> ma, TryAsync<B> mb) tuple) =>
        apply((a, b) => (a, b), tuple.ma, tuple.mb);

    /// <summary>
    /// Flip the tuple monads from inside the tuple to outside and apply a transformation function
    /// </summary>
    [Pure]
    public static TryOption<(C, D)> Traverse<A, B, C, D>(this (TryOption<A> ma, TryOption<B> mb) tuple, Func<(A a, B b), (C c, D d)> f) =>
        from a in tuple.ma
        from b in tuple.mb
        let r = f((a, b))
        select (r.Item1, r.Item2);

    /// <summary>
    /// Flip the tuple monads from inside the tuple to outside and apply a transformation function
    /// </summary>
    [Pure]
    public static TryOption<(C, D)> Traverse<A, B, C, D>(this (TryOption<A> ma, TryOption<B> mb) tuple, Func<A, B, (C c, D d)> f) =>
        from a in tuple.ma
        from b in tuple.mb
        let r = f(a, b)
        select (r.Item1, r.Item2);

    /// <summary>
    /// Flip the tuple monads from inside the tuple to outside and apply a transformation function
    /// </summary>
    [Pure]
    public static TryOption<(A, B)> Sequence<A, B>(this (TryOption<A> ma, TryOption<B> mb) tuple) =>
        from a in tuple.ma
        from b in tuple.mb
        select (a, b);

    /// <summary>
    /// Flip the tuple monads from inside the tuple to outside and apply a transformation function
    /// </summary>
    [Pure]
    public static TryOptionAsync<(C, D)> Traverse<A, B, C, D>(this (TryOptionAsync<A> ma, TryOptionAsync<B> mb) tuple, Func<(A a, B b), (C c, D d)> f) =>
        apply((a, b) => f((a, b)), tuple.ma, tuple.mb);

    /// <summary>
    /// Flip the tuple monads from inside the tuple to outside and apply a transformation function
    /// </summary>
    [Pure]
    public static TryOptionAsync<(C, D)> Traverse<A, B, C, D>(this (TryOptionAsync<A> ma, TryOptionAsync<B> mb) tuple, Func<A, B, (C c, D d)> f) =>
        apply((a, b) => f(a, b), tuple.ma, tuple.mb);

    /// <summary>
    /// Flip the tuple monads from inside the tuple to outside and apply a transformation function
    /// </summary>
    [Pure]
    public static TryOptionAsync<(A, B)> Sequence<A, B>(this (TryOptionAsync<A> ma, TryOptionAsync<B> mb) tuple) =>
        apply((a, b) => (a, b), tuple.ma, tuple.mb);

    /// <summary>
    /// Flip the tuple monads from inside the tuple to outside and apply a transformation function
    /// </summary>
    [Pure]
    public static Validation<MonoidFail, L, (C, D)> Traverse<MonoidFail, L, A, B, C, D>(this (Validation<MonoidFail, L, A> ma, Validation<MonoidFail, L, B> mb) tuple, Func<(A a, B b), (C c, D d)> f)
        where MonoidFail : struct, Monoid<L>, Eq<L> =>
        tuple.Apply<MonoidFail, L, A, B, (C, D)>((a, b) => f((a, b)));

    /// <summary>
    /// Flip the tuple monads from inside the tuple to outside and apply a transformation function
    /// </summary>
    [Pure]
    public static Validation<MonoidFail, L, (C, D)> Traverse<MonoidFail, L, A, B, C, D>(this (Validation<MonoidFail, L, A> ma, Validation<MonoidFail, L, B> mb) tuple, Func<A, B, (C c, D d)> f)
        where MonoidFail : struct, Monoid<L>, Eq<L> =>
        tuple.Apply<MonoidFail, L, A, B, (C, D)>(f);

    /// <summary>
    /// Flip the tuple monads from inside the tuple to outside and apply a transformation function
    /// </summary>
    [Pure]
    public static Validation<MonoidFail, L, (A, B)> Sequence<MonoidFail, L, A, B>(this (Validation<MonoidFail, L, A> ma, Validation<MonoidFail, L, B> mb) tuple)
        where MonoidFail : struct, Monoid<L>, Eq<L> =>
        tuple.Apply<MonoidFail, L, A, B, (A, B)>((a, b) => (a, b));

    /// <summary>
    /// Flip the tuple monads from inside the tuple to outside and apply a transformation function
    /// </summary>
    [Pure]
    public static Validation<L, (C, D)> Traverse<L, A, B, C, D>(this (Validation<L, A> ma, Validation<L, B> mb) tuple, Func<(A a, B b), (C c, D d)> f) =>
        tuple.Apply<L, A, B, (C, D)>((a, b) => f((a, b)));

    /// <summary>
    /// Flip the tuple monads from inside the tuple to outside and apply a transformation function
    /// </summary>
    [Pure]
    public static Validation<L, (C, D)> Traverse<L, A, B, C, D>(this (Validation<L, A> ma, Validation<L, B> mb) tuple, Func<A, B, (C c, D d)> f) =>
        tuple.Apply<L, A, B, (C, D)>(f);

    /// <summary>
    /// Flip the tuple monads from inside the tuple to outside and apply a transformation function
    /// </summary>
    [Pure]
    public static Validation<L, (A, B)> Sequence<L, A, B>(this (Validation<L, A> ma, Validation<L, B> mb) tuple) =>
        tuple.Apply<L, A, B, (A, B)>((a, b) => (a, b));
}

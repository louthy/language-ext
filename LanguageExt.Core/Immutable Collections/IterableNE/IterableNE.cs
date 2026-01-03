using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Numerics;
using System.Threading;
using LanguageExt.ClassInstances;
using LanguageExt.Traits;
using static LanguageExt.Prelude;
// ReSharper disable NonReadonlyMemberInGetHashCode

namespace LanguageExt;

/// <summary>
/// Non-empty lazy-sequence
/// </summary>
/// <remarks>
/// This always has a Head value and a Tail of length 0 to `n`.   
/// </remarks>
/// <typeparam name="A">Type of the values in the sequence</typeparam>
public record IterableNE<A>(A Head, Iterable<A> Tail) :
    IEnumerable<A>,
    Semigroup<IterableNE<A>>,
    IComparable<IterableNE<A>>,
    IComparisonOperators<IterableNE<A>, IterableNE<A>, bool>,
    IAdditionOperators<IterableNE<A>, IterableNE<A>, IterableNE<A>>,
    K<IterableNE, A>
{
    int? hashCode;

    public static IterableNE<A> FromSpan(ReadOnlySpan<A> ma)
    {
        if (ma.IsEmpty) throw new ArgumentException("Cannot create an IterableNE from an empty span");
        return new IterableNE<A>(ma[0], Iterable<A>.FromSpan(ma.Slice(1)));
    }
    
    [Pure]
    internal bool IsAsync =>
        Tail.IsAsync;
    
    /// <summary>
    /// Number of items in the sequence.
    /// </summary>
    /// <remarks>
    /// NOTE: This will force evaluation of the sequence
    /// </remarks>
    [Pure]
    public int Count() =>
        CountIO().Run();

    /// <summary>
    /// Number of items in the sequence.
    /// </summary>
    /// <remarks>
    /// NOTE: This will force evaluation of the sequence
    /// </remarks>
    [Pure]
    public IO<int> CountIO() =>
        Tail.CountIO().Map(c => c + 1);

    /// <summary>
    /// Stream as an enumerable
    /// </summary>
    [Pure]
    public IO<IEnumerable<A>> AsEnumerableIO() =>
        Tail.AsEnumerableIO().Map(xs => xs.Prepend(Head));

    /// <summary>
    /// Stream as an enumerable
    /// </summary>
    [Pure]
    public IEnumerable<A> AsEnumerable(CancellationToken token = default)
    {
        using var env = EnvIO.New(token: token);
        return AsEnumerableIO().Run(env);
    }

    /// <summary>
    /// Stream as an enumerable
    /// </summary>
    [Pure]
    public IO<IAsyncEnumerable<A>> AsAsyncEnumerableIO() =>
        Tail.AsAsyncEnumerableIO().Map(xs => xs.Prepend(Head));

    /// <summary>
    /// Stream as an enumerable
    /// </summary>
    [Pure]
    public IAsyncEnumerable<A> AsAsyncEnumerable(CancellationToken token = default)
    {
        using var env = EnvIO.New(token: token);
        return AsAsyncEnumerableIO().Run(env);
    }

    /// <summary>
    /// Stream as an enumerable
    /// </summary>
    [Pure]
    public Iterable<A> AsIterable() =>
        Head.Cons(Tail);

    /// <summary>
    /// Add an item to the end of the sequence
    /// </summary>
    /// <remarks>
    /// This does not force evaluation of the whole lazy sequence, nor does it cause
    /// exponential iteration issues when repeated adds occur.
    /// </remarks>
    [Pure]
    public IterableNE<A> Add(A item) =>
        new(Head, Tail.Add(item));

    /// <summary>
    /// Add an item to the beginning of the sequence
    /// </summary>
    /// <remarks>
    /// This does not force evaluation of the whole lazy sequence, nor does it cause
    /// exponential iteration issues when repeated cons occur.
    /// </remarks>
    [Pure]
    public IterableNE<A> Cons(A item) =>
        new(item, Head.Cons(Tail));

    /// <summary>
    /// Impure iteration of the bound values in the structure
    /// </summary>
    /// <returns>
    /// Returns the original unmodified structure
    /// </returns>
    public IO<Unit> IterIO(Action<A> f) =>
        IO.lift(() => f(Head)) >> Tail.IterIO(f);

    /// <summary>
    /// Impure iteration of the bound values in the structure
    /// </summary>
    /// <returns>
    /// Returns the original unmodified structure
    /// </returns>
    public Unit Iter(Action<A> f) =>
        IterIO(f).Run();

    /// <summary>
    /// Impure iteration of the bound values in the structure
    /// </summary>
    /// <returns>
    /// Returns the original unmodified structure
    /// </returns>
    public IO<Unit> IterIO(Action<A, int> f) =>
        IO.lift(() => f(Head, 0)) >> Tail.IterIO(f, 1);

    /// <summary>
    /// Impure iteration of the bound values in the structure
    /// </summary>
    /// <returns>
    /// Returns the original unmodified structure
    /// </returns>
    public Unit Iter(Action<A, int> f) =>
        IterIO(f).Run();

    /// <summary>
    /// Map the sequence using the function provided
    /// </summary>
    /// <typeparam name="B"></typeparam>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped sequence</returns>
    [Pure]
    public IterableNE<B> Map<B>(Func<A, B> f) =>
        new(f(Head), Tail.Map(f));

    /// <summary>
    /// Map the sequence using the function provided
    /// </summary>
    /// <typeparam name="B"></typeparam>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped sequence</returns>
    [Pure]
    public IterableNE<B> Map<B>(Func<A, int, B> f) =>
        new(f(Head, 0), Tail.Map(f, 1));

    /// <summary>
    /// Monadic bind (flatmap) of the sequence
    /// </summary>
    /// <typeparam name="B">Bound return value type</typeparam>
    /// <param name="f">Bind function</param>
    /// <returns>Flat-mapped sequence</returns>
    [Pure]
    public IterableNE<B> Bind<B>(Func<A, IterableNE<B>> f)
    {
        var head = f(Head);
        var tail = Tail.Bind(a => f(a).AsIterable());
        return head + tail;
    }

    /// <summary>
    /// Filter the items in the sequence
    /// </summary>
    /// <param name="f">Predicate to apply to the items</param>
    /// <returns>Filtered sequence</returns>
    [Pure]
    public Iterable<A> Filter(Func<A, bool> f) =>
        f(Head)
            ? Head.Cons(Tail.Filter(f))
            : Tail.Filter(f);

    /// <summary>
    /// Fold over the sequence from the left, accumulating state in `f`
    /// </summary>
    /// <param name="f">Fold function to apply to each item in the sequence</param>
    /// <param name="predicate">Continue while the predicate returns true for any pair of value and state.
    /// This is tested before the value is processed and the state is updated. So, use `FoldWhile*` for pre-assertions.
    /// </param>
    /// <param name="initialState">Initial state value</param>
    /// <typeparam name="S">State value type</typeparam>
    /// <returns>Resulting state</returns>
    public S FoldWhile<S>(
        Func<S, A, S> f,
        Func<(S State, A Value), bool> predicate,
        S initialState) =>
        FoldWhileIO(f, predicate, initialState).Run();

    /// <summary>
    /// Fold over the sequence from the left, accumulating state in `f`
    /// </summary>
    /// <param name="f">Fold function to apply to each item in the sequence</param>
    /// <param name="predicate">Continue while the predicate returns true for any pair of value and state.
    /// This is tested before the value is processed and the state is updated. So, use `FoldWhile*` for pre-assertions.
    /// </param>
    /// <param name="initialState">Initial state value</param>
    /// <typeparam name="S">State value type</typeparam>
    /// <returns>Resulting state</returns>
    public IO<S> FoldWhileIO<S>(
        Func<S, A, S> f,
        Func<(S State, A Value), bool> predicate,
        S initialState) =>
        IO.liftVAsync<S>(async env => predicate((initialState, Head))
                                          ? initialState
                                          : await Tail.FoldUntilIO(f, predicate, f(initialState, Head)).RunAsync(env));

    /// <summary>
    /// Fold over the sequence from the left, accumulating state in `f`
    /// </summary>
    /// <param name="f">Fold function to apply to each item in the sequence</param>
    /// <param name="predicate">Continue while the predicate returns true for any pair of value and state.
    /// This is tested before the value is processed and the state is updated. So, use `FoldWhile*` for pre-assertions.
    /// </param>
    /// <param name="initialState">Initial state value</param>
    /// <typeparam name="S">State value type</typeparam>
    /// <returns>Resulting state</returns>
    public S FoldUntil<S>(
        Func<S, A, S> f,
        Func<(S State, A Value), bool> predicate,
        S initialState) =>
        FoldUntilIO(f, predicate, initialState).Run();

    /// <summary>
    /// Fold over the sequence from the left, accumulating state in `f`
    /// </summary>
    /// <param name="f">Fold function to apply to each item in the sequence</param>
    /// <param name="predicate">Continue while the predicate returns true for any pair of value and state.
    /// This is tested before the value is processed and the state is updated. So, use `FoldWhile*` for pre-assertions.
    /// </param>
    /// <param name="initialState">Initial state value</param>
    /// <typeparam name="S">State value type</typeparam>
    /// <returns>Resulting state</returns>
    public IO<S> FoldUntilIO<S>(
        Func<S, A, S> f,
        Func<(S State, A Value), bool> predicate,
        S initialState) =>
        IO.liftVAsync<S>(async env =>
                         {
                             var s = f(initialState, Head);
                             if (predicate((s, Head))) return s;
                             return await Tail.FoldUntilIO(f, predicate, s).RunAsync(env);
                         });

    /// <summary>
    /// Fold until the `Option` returns `None`
    /// </summary>
    /// <param name="f">Fold function</param>
    /// <param name="initialState">Initial state for the fold</param>
    /// <param name="ta">Foldable structure</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Aggregated value</returns>
    public S FoldMaybe<S>(
        Func<S, A, Option<S>> f,
        S initialState) =>
        FoldMaybeIO(f, initialState).Run();

    /// <summary>
    /// Fold until the `Option` returns `None`
    /// </summary>
    /// <param name="f">Fold function</param>
    /// <param name="initialState">Initial state for the fold</param>
    /// <param name="ta">Foldable structure</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Aggregated value</returns>
    public IO<S> FoldMaybeIO<S>(
        Func<S, A, Option<S>> f,
        S initialState) =>
        FoldWhileIO<(bool IsSome, S Value)>(
                (s, a) => f(s.Value, a) switch
                          {
                              { IsSome: true, Case: S value } => (true, value),
                              _                               => (false, s.Value)
                          },
                s => s.State.IsSome,
                (true, initialState))
           .Map(s => s.Value);

    /// <summary>
    /// Same behaviour as `Fold` but the fold operation returns a monadic type and allows
    /// early exit of the operation once the predicate function becomes `false` for the
    /// state/value pair 
    /// </summary>
    public K<M, S> FoldWhileM<M, S>(
        Func<S, A, K<M, S>> f, 
        Func<A, bool> predicate, 
        S initialState) 
        where M : MonadIO<M>
    {
        return FoldWhileIO(acc, s => predicate(s.Value), Monad.pure<M, S>)
           .Bind(f1 => f1(initialState));

        Func<S, K<M, S>> acc(Func<S, K<M, S>> bind, A value) =>
            state => Monad.bind(f(state, value), bind);
    }

    /// <summary>
    /// Same behaviour as `Fold` but the fold operation returns a monadic type and allows
    /// early exit of the operation once the predicate function becomes `false` for the
    /// state/value pair 
    /// </summary>
    public K<M, S> FoldUntilM<M, S>(
        Func<S, A, K<M, S>> f, 
        Func<A, bool> predicate, 
        S initialState) 
        where M : MonadIO<M>
    {
        return FoldUntilIO(acc, s => predicate(s.Value), Monad.pure<M, S>).Bind(f1 => f1(initialState));

        Func<S, K<M, S>> acc(Func<S, K<M, S>> bind, A value) =>
            state => Monad.bind(f(state, value), bind);
    }

    /// <summary>
    /// Right-associative fold of a structure, lazy in the accumulator.
    ///
    /// In the case of lists, 'Fold', when applied to a binary operator, a
    /// starting value (typically the right-identity of the operator), and a
    /// list, reduces the list using the binary operator, from right to left.
    /// </summary>
    public S Fold<S>(Func<S, A, S> f, S initialState) =>
        FoldIO(f, initialState).Run();

    /// <summary>
    /// Right-associative fold of a structure, lazy in the accumulator.
    ///
    /// In the case of lists, 'Fold', when applied to a binary operator, a
    /// starting value (typically the right-identity of the operator), and a
    /// list, reduces the list using the binary operator, from right to left.
    /// </summary>
    public IO<S> FoldIO<S>(Func<S, A, S> f, S initialState) =>
        FoldWhileIO(f, _ => true, initialState);

    /// <summary>
    /// Right-associative fold of a structure, lazy in the accumulator.
    ///
    /// In the case of lists, 'Fold', when applied to a binary operator, a
    /// starting value (typically the right-identity of the operator), and a
    /// list, reduces the list using the binary operator, from right to left.
    /// </summary>
    public K<M, S> FoldM<M, S>(
        Func<S, A, K<M, S>> f, 
        S initialState) 
        where M : MonadIO<M>
    {
        return FoldIO(acc, Monad.pure<M, S>).Bind(f => f(initialState));

        Func<S, K<M, S>> acc(Func<S, K<M, S>> bind, A value) =>
            state => Monad.bind(f(state, value), bind);
    }

    /// <summary>
    /// Semigroup combine two iterables (concatenate)
    /// </summary>
    [Pure]
    public IterableNE<A> Combine(IterableNE<A> y) =>
        new(Head, Tail + y.Head.Cons(y.Tail));

    /// <summary>
    /// Add a range of items to the end of the sequence
    /// </summary>
    [Pure]
    public IterableNE<A> Concat(IEnumerable<A> items) =>
        new(Head, Tail.Concat(items));

    /// <summary>
    /// Add a range of items to the end of the sequence
    /// </summary>
    [Pure]
    public IterableNE<A> Concat(Iterable<A> items) =>
        new(Head, Tail + items);

    /// <summary>
    /// Add a range of items to the end of the sequence
    /// </summary>
    [Pure]
    public IterableNE<A> Concat(IterableNE<A> items) =>
        new(Head, Tail + items.Head.Cons(items.Tail));

    /// <summary>
    /// Monadic bind (flatmap) of the sequence
    /// </summary>
    /// <typeparam name="B">Bound return value type</typeparam>
    /// <param name="f">Bind function</param>
    /// <returns>Flat-mapped sequence</returns>
    [Pure]
    public IterableNE<B> Bind<B>(Func<A, K<IterableNE, B>> f) =>
        Bind(a => f(a).As());

    /// <summary>
    /// Returns true if the sequence has items in it
    /// </summary>
    /// <returns>True if the sequence has items in it</returns>
    [Pure]
    public bool Any() =>
        true;

    [Pure]
    public int CompareTo(object? obj) =>
        obj is IterableNE<A> rhs
            ? CompareTo(rhs)
            : 1;

    [Pure]
    public int CompareTo(IterableNE<A>? other) =>
        CompareTo<OrdDefault<A>>(other);

    [Pure]
    public IO<int> CompareToIO(IterableNE<A>? other) =>
        CompareToIO<OrdDefault<A>>(other);

    /// <summary>
    /// Compare to another sequence
    /// </summary>
    [Pure]
    public int CompareTo<OrdA>(IterableNE<A>? rhs)
        where OrdA : Ord<A> =>
        CompareToIO<OrdA>(rhs).Run();
    
    /// <summary>
    /// Compare to another sequence
    /// </summary>
    [Pure]
    public IO<int> CompareToIO<OrdA>(IterableNE<A>? rhs)
        where OrdA : Ord<A> =>
        IO.liftVAsync(async e =>
                      {
                          if (rhs is null) return 1;
                          var cmp = OrdA.Compare(Head, rhs.Head);
                          if (cmp != 0) return cmp;
                          return await Tail.CompareToIO(rhs.Tail).RunAsync(e);
                      });

    /// <summary>
    /// Format the collection as `[a, b, c, ...]`
    /// The ellipsis is used for collections over 50 items
    /// To get a formatted string with all the items, use `ToFullString`
    /// or `ToFullArrayString`.
    /// </summary>
    [Pure]
    public override string ToString() =>
        CollectionFormat.ToShortArrayString(AsEnumerable());

    /// <summary>
    /// Format the collection as `[a, b, c, ...]`
    /// The ellipsis is used for collections over 50 items
    /// To get a formatted string with all the items, use `ToFullString`
    /// or `ToFullArrayString`.
    /// </summary>
    [Pure]
    public IO<string> ToStringIO() =>
        AsEnumerableIO().Map(xs => CollectionFormat.ToShortArrayString(xs));

    /// <summary>
    /// Format the collection as `a, b, c, ...`
    /// </summary>
    [Pure]
    public string ToFullString(string separator = ", ") =>
        CollectionFormat.ToFullString(AsEnumerable(), separator);

    /// <summary>
    /// Format the collection as `a, b, c, ...`
    /// </summary>
    [Pure]
    public IO<string> ToFullStringIO(string separator = ", ") =>
        AsEnumerableIO().Map(xs => CollectionFormat.ToFullString(xs, separator));

    /// <summary>
    /// Format the collection as `[a, b, c, ...]`
    /// </summary>
    [Pure]
    public string ToFullArrayString(string separator = ", ") =>
        CollectionFormat.ToFullArrayString(AsEnumerable(), separator);

    /// <summary>
    /// Format the collection as `[a, b, c, ...]`
    /// </summary>
    [Pure]
    public IO<string> ToFullArrayStringIO(string separator = ", ") =>
        AsEnumerableIO().Map(xs => CollectionFormat.ToFullArrayString(xs, separator));

    /// <summary>
    /// Skip count items
    /// </summary>
    [Pure]
    public Iterable<A> Skip(int amount) =>
        amount switch
        {
            0 => AsIterable(),
            1 => Tail,
            _ => Tail.Skip(amount - 1)
        };

    /// <summary>
    /// Take count items
    /// </summary>
    [Pure]
    public Iterable<A> Take(int amount) =>
        amount switch
        {
            0 => [],
            1 => [Head],
            _ => Tail.Take(amount - 1)
        };

    /// <summary>
    /// Iterate the sequence, yielding items if they match the predicate 
    /// provided, and stopping as soon as one doesn't
    /// </summary>
    /// <returns>A new sequence with the first items that match the 
    /// predicate</returns>
    [Pure]
    public Iterable<A> TakeWhile(Func<A, bool> pred) =>
        AsIterable().TakeWhile(pred);

    /// <summary>
    /// Iterate the sequence, yielding items if they match the predicate 
    /// provided, and stopping as soon as one doesn't.  An index value is 
    /// also provided to the predicate function.
    /// </summary>
    /// <returns>A new sequence with the first items that match the 
    /// predicate</returns>
    [Pure]
    public Iterable<A> TakeWhile(Func<A, int, bool> pred) =>
        AsIterable().TakeWhile(pred);

    [Pure]
    /// <summary>
    /// Partition a list into two based on  a predicate
    /// </summary>
    /// <param name="predicate">True if the item goes in the first list, false for the second list</param>
    /// <returns>Pair of lists</returns>
    public (Iterable<A> First, Iterable<A> Second) Partition(Func<A, bool> predicate)
    {
        var (f, s) = Tail.Partition(predicate);
        return predicate(Head)
                ? (Head.Cons(f), s)
                : (f, Head.Cons(s));
    }

    /// <summary>
    /// Zip two iterables into pairs
    /// </summary>
    [Pure]
    public IterableNE<(A First, B Second)> Zip<B>(IterableNE<B> rhs) =>
        new ((Head, rhs.Head), Tail.Zip(rhs.Tail));

    /// <summary>
    /// Zip two iterables into pairs
    /// </summary>
    [Pure]
    public IterableNE<C> Zip<B, C>(IterableNE<B> rhs, Func<A, B, C> zipper) =>
        new (zipper(Head, rhs.Head), Tail.Zip(rhs.Tail, zipper));

    /// <summary>
    /// Append operator
    /// </summary>
    [Pure]
    public static IterableNE<A> operator +(IterableNE<A> x, IterableNE<A> y) =>
        x.Concat(y);

    /// <summary>
    /// Append operator
    /// </summary>
    [Pure]
    public static IterableNE<A> operator +(IterableNE<A> x, Iterable<A> y) =>
        x.Concat(y);

    /// <summary>
    /// Ordering operator
    /// </summary>
    [Pure]
    public static bool operator >(IterableNE<A> x, IterableNE<A> y) =>
        x.CompareTo(y) > 0;

    /// <summary>
    /// Ordering operator
    /// </summary>
    [Pure]
    public static bool operator >=(IterableNE<A> x, IterableNE<A> y) =>
        x.CompareTo(y) >= 0;

    /// <summary>
    /// Ordering  operator
    /// </summary>
    [Pure]
    public static bool operator <(IterableNE<A> x, IterableNE<A> y) =>
        x.CompareTo(y) < 0;

    /// <summary>
    /// Ordering  operator
    /// </summary>
    [Pure]
    public static bool operator <=(IterableNE<A> x, IterableNE<A> y) =>
        x.CompareTo(y) <= 0;

    /// <summary>
    /// Map the sequence using the function provided
    /// </summary>
    /// <typeparam name="B"></typeparam>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped sequence</returns>
    [Pure]
    public IterableNE<B> Select<B>(Func<A, B> f) =>
        Map(f);

    /// <summary>
    /// Map the sequence using the function provided
    /// </summary>
    /// <typeparam name="B"></typeparam>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped sequence</returns>
    [Pure]
    public IterableNE<B> Select<B>(Func<A, int, B> f) =>
        Map(f);

    /// <summary>
    /// Monadic bind (flatmap) of the sequence
    /// </summary>
    /// <typeparam name="B">Bound return value type</typeparam>
    /// <param name="bind">Bind function</param>
    /// <returns>Flat-mapped sequence</returns>
    [Pure]
    public IterableNE<B> SelectMany<B>(Func<A, IterableNE<B>> bind) =>
        Bind(bind);

    /// <summary>
    /// Monadic bind (flatmap) of the sequence
    /// </summary>
    /// <typeparam name="B">Bound return value type</typeparam>
    /// <param name="bind">Bind function</param>
    /// <returns>Flat-mapped sequence</returns>
    [Pure]
    public IterableNE<C> SelectMany<B, C>(Func<A, IterableNE<B>> bind, Func<A, B, C> project) =>
        Bind(x  => bind(x).Map(y => project(x, y)));

    [Pure]
    public IEnumerator<A> GetEnumerator() =>
        // ReSharper disable once NotDisposedResourceIsReturned
        AsEnumerable().GetEnumerator();

    /// <summary>
    /// Get the hash code for all the items in the sequence, or 0 if empty
    /// </summary>
    /// <returns></returns>
    [Pure]
    public override int GetHashCode() =>
        hashCode.HasValue
            ? hashCode.Value
            : (hashCode = hash(AsEnumerable())).Value;
    
    IEnumerator IEnumerable.GetEnumerator() => 
        GetEnumerator();
}

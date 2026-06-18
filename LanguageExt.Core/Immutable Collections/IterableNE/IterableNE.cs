using System;
using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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
public partial class IterableNE<A>(A Head, Iterator<A> Tail) :
    IEnumerable<A>,
    Semigroup<IterableNE<A>>,
    IComparable<IterableNE<A>>,
    IComparisonOperators<IterableNE<A>, IterableNE<A>, bool>,
    IAdditionOperators<IterableNE<A>, IterableNE<A>, IterableNE<A>>,
    K<IterableNE, A>
{
    int? hashCode;
    
    /// <summary>
    /// Create an iterable from a span
    /// </summary>
    public static IterableNE<A> FromSpan(ReadOnlySpan<A> ma) =>
        new (ma[0], Iterator.forward(ma[1..]));

    /// <summary>
    /// Convert to an Iterable
    /// </summary>
    public Iterable<A> AsIterable() =>
        new (Iterator.cons(Head, Tail));

    /// <summary>
    /// Number of items in the sequence.
    /// </summary>
    /// <remarks>
    /// NOTE: This will force evaluation of the sequence
    /// </remarks>
    [Pure]
    public long Count =>
        Tail.Count + 1;

    /// <summary>
    /// Get the first item in the sequence.
    /// </summary>
    [Pure]
    public A Head { get; } = Head;

    /// <summary>
    /// Get the first item in the sequence.
    /// </summary>
    [Pure]
    public Iterator<A> Tail { get; } = Tail;

    /// <summary>
    /// Stream as an enumerable
    /// </summary>
    [Pure]
    public IEnumerable<A> AsEnumerable() =>
        Head.Cons(Tail);

    /// <summary>
    /// Reverse the sequence
    /// </summary>
    [Pure]
    public IterableNE<A> Reverse() =>
        Tail.Reverse().Append(Head) switch
        {
            (Exist<A> (var h), var t) => new IterableNE<A>(h, t),
            _                         => throw new InvalidOperationException("Won't get here")
        };

    /// <summary>
    /// Add an item to the end of the sequence
    /// </summary>
    /// <remarks>
    /// This does not force evaluation of the whole lazy sequence, nor does it cause
    /// exponential iteration issues when repeated adds occur.
    /// </remarks>
    [Pure]
    public IterableNE<A> Add(A item) =>
        new (Head, Tail.Append(item)); 

    /// <summary>
    /// Add an item to the beginning of the sequence
    /// </summary>
    /// <remarks>
    /// This does not force evaluation of the whole lazy sequence, nor does it cause
    /// exponential iteration issues when repeated cons occur.
    /// </remarks>
    [Pure]
    public IterableNE<A> Cons(A item) =>
        new(item, Iterator.cons(item, Tail));

    /// <summary>
    /// Impure iteration of the bound values in the structure
    /// </summary>
    public Unit Iter(Action<A> f)
    {
        f(Head);
        return Tail.Iter(f);
    }

    /// <summary>
    /// Impure iteration of the bound values in the structure
    /// </summary>
    /// <returns>
    /// Returns the original unmodified structure
    /// </returns>
    public Unit Iter(Action<long, A> f)
    {
        f(0, Head);
        return Tail.Iter(f, 1);
    }

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
    public IterableNE<B> Map<B>(Func<A, long, B> f, int offset = 0) =>
        new(f(Head, 0), Tail.Map(f, offset));
    
    /// <summary>
    /// Filter the items in the sequence
    /// </summary>
    /// <param name="f">Predicate to apply to the items</param>
    /// <returns>Filtered sequence</returns>
    [Pure]
    public Iterable<A> Filter(Func<A, bool> f) =>
        f(Head)
            ? Iterator.cons(Head, Tail.Filter(f)).AsIterable()
            : Tail.Filter(f).AsIterable();

    /// <summary>
    /// Applies the given function `f` to each element of the sequence. Returns the sequence 
    /// of results for each element where the result is `Some(f(x))`.
    /// </summary>
    /// <param name="f">Selector function</param>
    /// <returns>Mapped and filtered sequence</returns>
    [Pure]
    public Iterable<B> Choose<B>(Func<A, Option<B>> f) =>
        f(Head) switch
        {
            { IsSome: true } o => Iterator.cons(o.Value!, Tail.Choose(f)).AsIterable(),
            _                  => Tail.Choose(f).AsIterable()
        };

    /// <summary>
    /// Equality test
    /// </summary>
    [Pure]
    public bool Equals(IterableNE<A>? other) =>
        Equals<OrdDefault<A>>(other);

    /// <summary>
    /// Equality test
    /// </summary>
    [Pure]
    public bool Equals<EqA>(IterableNE<A>? other)
        where EqA : Eq<A> =>
        other is not null            && 
        EqA.Equals(Head, other.Head) && 
        Tail.Equals<EqA>(other.Tail);

    [Pure]
    public override bool Equals(object? obj) =>
        obj is IterableNE<A> rhs && Equals(rhs);

    [Pure]
    public static bool operator ==(IterableNE<A>? lhs, IterableNE<A>? rhs) =>
        (lhs, rhs) switch
        {
            (null, null) => true,
            (null, _)    => false,
            (_, null)    => false,
            _            => lhs.Equals(rhs)
        };

    [Pure]
    public static bool operator !=(IterableNE<A>? lhs, IterableNE<A>? rhs) =>
        (lhs, rhs) switch
        {
            (null, null) => false,
            (null, _)    => true,
            (_, null)    => true,
            _            => !lhs.Equals(rhs)
        };

    /// <summary>
    /// Semigroup combine two iterables (concatenate)
    /// </summary>
    [Pure]
    public IterableNE<A> Combine(IterableNE<A> items) =>
        new (Head, Tail.ForwardIterator() + items.ForwardIterator());

    /// <summary>
    /// Add a range of items to the end of the sequence
    /// </summary>
    [Pure]
    public IterableNE<A> Concat(Iterable<A> items) =>
        new (Head, Tail.ForwardIterator() + items.ForwardIterator());

    /// <summary>
    /// Add a range of items to the end of the sequence
    /// </summary>
    [Pure]
    public IterableNE<A> Concat(IterableNE<A> items) =>
        new (Head, Tail.ForwardIterator() + items.ForwardIterator());

    /// <summary>
    /// Make sure no element in the sequence appears more than once
    /// </summary>
    [Pure]
    public IterableNE<A> Distinct<EqA>()
        where EqA : Eq<A> =>
        new(Head, Tail.Distinct<EqA>([Head]));

    /// <summary>
    /// Make sure no element in the sequence appears more than once
    /// </summary>
    [Pure]
    public IterableNE<A> Distinct() =>
        new(Head, Tail.Distinct<EqDefault<A>>([Head]));
    
    /// <summary>
    /// Map each element of a structure to an action, evaluate these actions from
    /// left to right, and collect the results.
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="F">Applicative functor trait</typeparam>
    /// <typeparam name="B">Bound value (output)</typeparam>
    [Pure]
    public K<F, Iterable<B>> Traverse<F, B>(Func<A, K<F, B>> f)
        where F : Applicative<F>
    {
        return this.Fold(add, F.Pure(Iterable<B>.Empty));
        K<F, Iterable<B>> add(K<F, Iterable<B>> state, A value) =>
            Applicative.lift((bs, b) => bs.Add(b), state, f(value));                                            
    }

    /// <summary>
    /// Map each element of a structure to an action, evaluate these actions from
    /// left to right, and collect the results.
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="M">Monad trait</typeparam>
    /// <typeparam name="B">Bound value (output)</typeparam>
    [Pure]
    public K<M, Iterable<B>> TraverseM<M, B>(Func<A, K<M, B>> f)
        where M : Monad<M>
    {
        return this.Fold(add, M.Pure(Iterable<B>.Empty));
        K<M, Iterable<B>> add(K<M, Iterable<B>> state, A value) =>
            state.Bind(bs => f(value).Map(bs.Add)); 
    }

    /// <summary>
    /// Monadic bind (flatmap) of the sequence
    /// </summary>
    /// <typeparam name="B">Bound return value type</typeparam>
    /// <param name="f">Bind function</param>
    /// <returns>Flat-mapped sequence</returns>
    [Pure]
    public IterableNE<B> Bind<B>(Func<A, K<IterableNE, B>> f)
    {
        var head = +f(Head);
        var tail = Tail.Bind(a => f(a).ForwardIterator());
        return new IterableNE<B>(head.Head, head.Tail + tail);
    }

    /// <summary>
    /// Monadic bind (flatmap) of the sequence
    /// </summary>
    /// <typeparam name="B">Bound return value type</typeparam>
    /// <param name="f">Bind function</param>
    /// <returns>Flat-mapped sequence</returns>
    [Pure]
    public IterableNE<B> Bind<B>(Func<A, IterableNE<B>> f)
    {
        var head = +f(Head);
        var tail = Tail.Bind(a => f(a).ForwardIterator());
        return new IterableNE<B>(head.Head, head.Tail + tail);
    }

    /// <summary>
    /// Monadic bind (flatmap) of the sequence
    /// </summary>
    /// <typeparam name="B">Bound return value type</typeparam>
    /// <param name="f">Bind function</param>
    /// <returns>Flat-mapped sequence</returns>
    [Pure]
    public Iterable<B> Bind<B>(Func<A, Iterable<B>> f)
    {
        var head = +f(Head);
        var tail = Tail.Bind(a => f(a).ForwardIterator());
        return new(head.ForwardIterator() + tail);
    }

    /// <summary>
    /// Inject a value in between each item in the sequence 
    /// </summary>
    /// <param name="sep">Item to inject</param>
    /// <typeparam name="A">Bound type</typeparam>
    /// <returns>A sequence with the values injected</returns>
    [Pure]
    public IterableNE<A> Intersperse(A sep) =>
        new(Head, Iterator.cons(sep, Tail.Intersperse(sep)));

    /// <summary>
    /// Compare to another sequence
    /// </summary>
    [Pure]
    public int CompareTo(IterableNE<A>? rhs) =>
        CompareTo<OrdDefault<A>>(rhs);

    /// <summary>
    /// Compare to another sequence
    /// </summary>
    [Pure]
    public int CompareTo<OrdA>(IterableNE<A>? rhs)
        where OrdA : Ord<A>
    {
        if (rhs is null) return 1;
        var cmp = OrdA.Compare(Head, rhs.Head);
        if (cmp != 0) return cmp;
        return Tail.CompareTo<OrdA>(rhs.Tail);
    }

    /// <summary>
    /// Skip a specified number of items from the start of the IteratorIO. 
    /// </summary>
    [Pure]
    public Iterable<A> Skip(long amount) =>
        AsIterable().Skip(amount);

    /// <summary>
    /// Skip items at the start of the sequence whilst the predicate returns true. 
    /// </summary>
    [Pure]
    public Iterable<A> SkipWhile(Func<A, bool> predicate) =>
        AsIterable().SkipWhile(predicate);

    /// <summary>
    /// Skip items at the start of the sequence until the predicate returns true. 
    /// </summary>
    [Pure]
    public Iterable<A> SkipUntil(Func<A, bool> predicate) =>
        AsIterable().SkipUntil(predicate);
    
    /// <summary>
    /// Take a specified number of items from the start of the IteratorIO. 
    /// </summary>
    [Pure]
    public Iterable<A> Take(long amount) =>
        AsIterable().Take(amount);

    /// <summary>
    /// Take items from the sequence whilst the predicate returns true.   
    /// </summary>
    [Pure]
    public Iterable<A> TakeWhile(Func<A, bool> predicate) =>
        AsIterable().TakeWhile(predicate);

    /// <summary>
    /// Take items from the sequence until the predicate returns true.   
    /// </summary>
    [Pure]
    public Iterable<A> TakeUntil(Func<A, bool> predicate) =>
        AsIterable().TakeUntil(predicate);

    /// <summary>
    /// Cast items to another type
    /// </summary>
    /// <remarks>
    /// Any item in the sequence that can't be cast to a `B` will be dropped from the result 
    /// </remarks>
    [Pure]
    public Iterable<B> Cast<B>() =>
        AsIterable().Cast<B>();

    /// <summary>
    /// Zip two iterables into pairs
    /// </summary>
    [Pure]
    public IterableNE<(A First, B Second)> Zip<B>(IterableNE<B> rhs) =>
        new((Head, rhs.Head), Tail.Zip(rhs.Tail));

    /// <summary>
    /// Zip two iterables into pairs
    /// </summary>
    [Pure]
    public IterableNE<C> Zip<B, C>(IterableNE<B> rhs, Func<A, B, C> join) =>
        new(join(Head, rhs.Head), Tail.Zip(rhs.Tail, join));

    /// <summary>
    /// Append operator
    /// </summary>
    [Pure]
    public static IterableNE<A> operator +(IterableNE<A> x, IterableNE<A> y) =>
        new (x.Head, x.Tail.ForwardIterator() + y.ForwardIterator());

    /// <summary>
    /// Append operator
    /// </summary>
    [Pure]
    public static IterableNE<A> operator +(A x, IterableNE<A> y) =>
        new (x, y.ForwardIterator());

    /// <summary>
    /// Append operator
    /// </summary>
    [Pure]
    public static IterableNE<A> operator +(IterableNE<A> x, A y) =>
        new(x.Head, x.Tail.Append(y));
    
    /// <summary>
    /// Ordering operator
    /// </summary>
    [Pure]
    public static bool operator >(IterableNE<A> x, IterableNE<A> y) =>
        x.ForwardIterator() > y.ForwardIterator();

    /// <summary>
    /// Ordering operator
    /// </summary>
    [Pure]
    public static bool operator >=(IterableNE<A> x, IterableNE<A> y) =>
        x.ForwardIterator() >= y.ForwardIterator();

    /// <summary>
    /// Ordering  operator
    /// </summary>
    [Pure]
    public static bool operator <(IterableNE<A> x, IterableNE<A> y) =>
        x.ForwardIterator() < y.ForwardIterator();

    /// <summary>
    /// Ordering  operator
    /// </summary>
    [Pure]
    public static bool operator <=(IterableNE<A> x, IterableNE<A> y) =>
        x.ForwardIterator() <= y.ForwardIterator();

    /// <summary>
    /// Map the sequence using the function provided
    /// </summary>
    /// <typeparam name="B"></typeparam>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped sequence</returns>
    [Pure]
    public IterableNE<B> Select<B>(Func<A, long, B> f) =>
        Map(f);

    /// <summary>
    /// Monadic bind (flatmap) of the sequence
    /// </summary>
    /// <typeparam name="B">Bound return value type</typeparam>
    /// <param name="f">Bind function</param>
    /// <returns>Flat-mapped sequence</returns>
    [Pure]
    public Iterable<B> SelectMany<B>(Func<A, Iterable<B>> f) =>
        Bind(f);

    /// <summary>
    /// Monadic bind (flatmap) of the sequence
    /// </summary>
    /// <typeparam name="B">Bound return value type</typeparam>
    /// <param name="bind">Bind function</param>
    /// <returns>Flat-mapped sequence</returns>
    [Pure]
    public Iterable<C> SelectMany<B, C>(Func<A, Iterable<B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

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
        hashCode is null
            ? (hashCode = hash(AsEnumerable())).Value
            : hashCode.Value;

    /// <summary>
    /// Get the additive-identity, i.e. the monoid-zero.  Which is the empty sequence/
    /// </summary>
    public static Iterable<A> AdditiveIdentity => 
        Empty;
    
    IEnumerator IEnumerable.GetEnumerator() => 
        GetEnumerator();


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
    /// Format the collection as `a, b, c, ...`
    /// </summary>
    [Pure]
    public string ToFullString(string separator = ", ") =>
        CollectionFormat.ToFullString(AsEnumerable(), separator);
    
    /// <summary>
    /// Format the collection as `[a, b, c, ...]`
    /// </summary>
    [Pure]
    public string ToFullArrayString(string separator = ", ") =>
        CollectionFormat.ToFullArrayString(AsEnumerable(), separator);

}

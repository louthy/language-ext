using System;
using System.Numerics;
using System.Collections;
using LanguageExt.Traits;
using LanguageExt.ClassInstances;
using static LanguageExt.Prelude;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt;

/// <summary>
/// Lazy sequence
/// </summary>
/// <remarks>
/// Initially designed as a lightweight wrapper around `IEnumerable`, it has since expanded to be a
/// much more powerful lazy sequence of values. 
/// </remarks>
/// <typeparam name="A">Value type</typeparam>
[CollectionBuilder(typeof(Iterable), nameof(Iterable.create))]
public sealed partial class Iterable<A> :
    IEnumerable<A>,
    Monoid<Iterable<A>>,
    IComparable<Iterable<A>>,
    IAdditiveIdentity<Iterable<A>, Iterable<A>>,
    IComparisonOperators<Iterable<A>, Iterable<A>, bool>,
    IAdditionOperators<Iterable<A>, Iterable<A>, Iterable<A>>,
    K<Iterable, A>
{
    int? hashCode;
    internal readonly Iterator<A> iterator;
    
    internal Iterable(Iterator<A> iterator) =>
        this.iterator = iterator;

    /// <summary>
    /// Create an iterable from a span
    /// </summary>
    public static Iterable<A> FromSpan(ReadOnlySpan<A> ma) =>
        new (Iterator.forward(Arr.create(ma)));

    /// <summary>
    /// Empty sequence
    /// </summary>
    [Pure]
    public static Iterable<A> Empty { get; } =
        new(Iterator.empty<A>());

    /// <summary>
    /// Number of items in the sequence.
    /// </summary>
    /// <remarks>
    /// NOTE: This will force evaluation of the sequence
    /// </remarks>
    [Pure]
    public long Count =>
        iterator.Count;
    
    /// <summary>
    /// Stream as an enumerable
    /// </summary>
    [Pure]
    public IEnumerable<A> AsEnumerable() =>
        iterator.AsEnumerable();

    /// <summary>
    /// Reverse the sequence
    /// </summary>
    [Pure]
    public Iterable<A> Reverse() =>
        new (iterator.Reverse());

    /// <summary>
    /// Add an item to the end of the sequence
    /// </summary>
    /// <remarks>
    /// This does not force evaluation of the whole lazy sequence, nor does it cause
    /// exponential iteration issues when repeated adds occur.
    /// </remarks>
    [Pure]
    public Iterable<A> Add(A item) =>
        new (iterator.Append(item)); 

    /// <summary>
    /// Add an item to the beginning of the sequence
    /// </summary>
    /// <remarks>
    /// This does not force evaluation of the whole lazy sequence, nor does it cause
    /// exponential iteration issues when repeated cons occur.
    /// </remarks>
    [Pure]
    public Iterable<A> Cons(A item) =>
        new(Iterator.cons(item, iterator));

    /// <summary>
    /// Impure iteration of the bound values in the structure
    /// </summary>
    public Unit Iter(Action<A> f) =>
        iterator.Iter(f);

    /// <summary>
    /// Impure iteration of the bound values in the structure
    /// </summary>
    /// <returns>
    /// Returns the original unmodified structure
    /// </returns>
    public Unit Iter(Action<long, A> f) =>
        iterator.Iter(f);

    /// <summary>
    /// Map the sequence using the function provided
    /// </summary>
    /// <typeparam name="B"></typeparam>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped sequence</returns>
    [Pure]
    public Iterable<B> Map<B>(Func<A, B> f) =>
        new(iterator.Map(f));

    /// <summary>
    /// Map the sequence using the function provided
    /// </summary>
    /// <typeparam name="B"></typeparam>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped sequence</returns>
    [Pure]
    public Iterable<B> Map<B>(Func<A, long, B> f, int offset = 0) =>
        new(iterator.Map(f, offset));

    /// <summary>
    /// Filter the items in the sequence
    /// </summary>
    /// <param name="f">Predicate to apply to the items</param>
    /// <returns>Filtered sequence</returns>
    [Pure]
    public Iterable<A> Filter(Func<A, bool> f) =>
        new(iterator.Filter(f));

    /// <summary>
    /// Applies the given function `f` to each element of the sequence. Returns the sequence 
    /// of results for each element where the result is `Some(f(x))`.
    /// </summary>
    /// <param name="f">Selector function</param>
    /// <returns>Mapped and filtered sequence</returns>
    [Pure]
    public Iterable<B> Choose<B>(Func<A, Option<B>> f) =>
        new(iterator.Choose(f));

    /// <summary>
    /// Equality test
    /// </summary>
    [Pure]
    public bool Equals(Iterable<A>? other) =>
        iterator.Equals(other?.iterator);

    /// <summary>
    /// Equality test
    /// </summary>
    [Pure]
    public bool Equals<EqA>(Iterable<A>? other)
        where EqA : Eq<A> =>
        iterator.Equals(other?.iterator);

    [Pure]
    public override bool Equals(object? obj) =>
        obj is Iterable<A> rhs && Equals(rhs);

    [Pure]
    public static bool operator ==(Iterable<A>? lhs, Iterable<A>? rhs) =>
        (lhs, rhs) switch
        {
            (null, null) => true,
            (null, _)    => false,
            (_, null)    => false,
            _            => lhs.Equals(rhs)
        };

    [Pure]
    public static bool operator !=(Iterable<A>? lhs, Iterable<A>? rhs) =>
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
    public Iterable<A> Combine(Iterable<A> y) =>
        new(iterator.Combine(y.iterator));

    /// <summary>
    /// Add a range of items to the end of the sequence
    /// </summary>
    [Pure]
    public Iterable<A> Concat(IEnumerable<A> items) =>
        new(iterator.Combine(Iterator.forward(items)));

    /// <summary>
    /// Add a range of items to the end of the sequence
    /// </summary>
    [Pure]
    public Iterable<A> Concat(Iterable<A> items) =>
        new(iterator.Combine(items.iterator));

    /// <summary>
    /// Make sure no element in the sequence appears more than once
    /// </summary>
    [Pure]
    public Iterable<A> Distinct<EqA>()
        where EqA : Eq<A> =>
        new(iterator.Distinct<EqA>());

    /// <summary>
    /// Make sure no element in the sequence appears more than once
    /// </summary>
    [Pure]
    public Iterable<A> Distinct() =>
        new(iterator.Distinct());

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
    public Iterable<B> Bind<B>(Func<A, K<Iterable, B>> f) =>
        new(iterator.Bind(a => f(a).As().iterator));

    /// <summary>
    /// Monadic bind (flatmap) of the sequence
    /// </summary>
    /// <typeparam name="B">Bound return value type</typeparam>
    /// <param name="f">Bind function</param>
    /// <returns>Flat-mapped sequence</returns>
    [Pure]
    public Iterable<B> Bind<B>(Func<A, Iterable<B>> f) =>
        new(iterator.Bind(a => f(a).iterator));
    
    /// <summary>
    /// Returns true if the sequence has items in it
    /// </summary>
    /// <returns>True if the sequence has items in it</returns>
    [Pure]
    public bool Any() =>
        iterator.Exists(_ => true);

    /// <summary>
    /// Inject a value in between each item in the sequence 
    /// </summary>
    /// <param name="value">Item to inject</param>
    /// <typeparam name="A">Bound type</typeparam>
    /// <returns>A sequence with the values injected</returns>
    [Pure]
    public Iterable<A> Intersperse(A value) =>
        new(iterator.Intersperse(value));

    /// <summary>
    /// Compare to another sequence
    /// </summary>
    [Pure]
    public int CompareTo(Iterable<A>? rhs) =>
        iterator.CompareTo<OrdDefault<A>>(rhs?.iterator);

    /// <summary>
    /// Compare to another sequence
    /// </summary>
    [Pure]
    public int CompareTo<OrdA>(Iterable<A>? rhs) 
        where OrdA : Ord<A> =>
        iterator.CompareTo<OrdA>(rhs?.iterator);

    /// <summary>
    /// Consume the item at the head (first) of the sequence or None if the sequence is empty
    /// </summary>
    /// <returns>Optional head item</returns>
    [Pure]
    public Option<A> Head =>
        iterator.Head;

    /// <summary>
    /// Consume the item at the head (first) of the sequence or `Alternative.Empty` if the sequence is empty
    /// </summary>
    /// <returns>Optional head item</returns>
    [Pure]
    public K<M, A> HeadM<M>()
        where M : Alternative<M> =>
        iterator.HeadM<Iterator, M, A>();

    /// <summary>
    /// Consume the first item of the sequence, returning the tail of the sequence. 
    /// </summary>
    /// <returns>The tail items</returns>
    [Pure]
    public Iterable<A> Tail =>
        iterator is (Exist<A>, var tail)
            ? new(tail)
            : Empty;

    /// <summary>
    /// Skip a specified number of items from the start of the IteratorIO. 
    /// </summary>
    [Pure]
    public Iterable<A> Skip(long amount) =>
        new(iterator.Skip(amount));

    /// <summary>
    /// Skip items at the start of the sequence whilst the predicate returns true. 
    /// </summary>
    [Pure]
    public Iterable<A> SkipWhile(Func<A, bool> predicate) =>
        new(iterator.SkipWhile(predicate));

    /// <summary>
    /// Skip items at the start of the sequence until the predicate returns true. 
    /// </summary>
    [Pure]
    public Iterable<A> SkipUntil(Func<A, bool> predicate) =>
        new(iterator.SkipUntil(predicate));
    
    /// <summary>
    /// Take a specified number of items from the start of the IteratorIO. 
    /// </summary>
    [Pure]
    public Iterable<A> Take(long amount) =>
        new(iterator.Take(amount));

    /// <summary>
    /// Take items from the sequence whilst the predicate returns true.   
    /// </summary>
    [Pure]
    public Iterable<A> TakeWhile(Func<A, bool> predicate) =>
        new(iterator.TakeWhile(predicate));

    /// <summary>
    /// Take items from the sequence until the predicate returns true.   
    /// </summary>
    [Pure]
    public Iterable<A> TakeUntil(Func<A, bool> predicate) =>
        new(iterator.TakeUntil(predicate));

    /// <summary>
    /// Cast items to another type
    /// </summary>
    /// <remarks>
    /// Any item in the sequence that can't be cast to a `B` will be dropped from the result 
    /// </remarks>
    [Pure]
    public Iterable<B> Cast<B>() =>
        new(iterator.Cast<B>());

    /// <summary>
    /// Zip two iterables into pairs
    /// </summary>
    [Pure]
    public Iterable<(A First, B Second)> Zip<B>(Iterable<B> rhs) =>
        new(iterator.Zip(rhs.iterator));

    /// <summary>
    /// Zip two iterables into pairs
    /// </summary>
    [Pure]
    public Iterable<C> Zip<B, C>(Iterable<B> rhs, Func<A, B, C> join) =>
        Zip(rhs).Map(pair => join(pair.First, pair.Second));

    /// <summary>
    /// Append operator
    /// </summary>
    [Pure]
    public static Iterable<A> operator +(Iterable<A> x, Iterable<A> y) =>
        new(x.iterator + y.iterator);

    /// <summary>
    /// Append operator
    /// </summary>
    [Pure]
    public static Iterable<A> operator +(A x, Iterable<A> y) =>
        new(y.iterator.Prepend(x));

    /// <summary>
    /// Append operator
    /// </summary>
    [Pure]
    public static Iterable<A> operator +(Iterable<A> x, A y) =>
        new(x.iterator.Append(y));
    
    /// <summary>
    /// Ordering operator
    /// </summary>
    [Pure]
    public static bool operator >(Iterable<A> x, Iterable<A> y) =>
        x.iterator > y.iterator;

    /// <summary>
    /// Ordering operator
    /// </summary>
    [Pure]
    public static bool operator >=(Iterable<A> x, Iterable<A> y) =>
        x.iterator >= y.iterator;

    /// <summary>
    /// Ordering  operator
    /// </summary>
    [Pure]
    public static bool operator <(Iterable<A> x, Iterable<A> y) =>
        x.iterator < y.iterator;

    /// <summary>
    /// Ordering  operator
    /// </summary>
    [Pure]
    public static bool operator <=(Iterable<A> x, Iterable<A> y) =>
        x.iterator <= y.iterator;
                
    /// <summary>
    /// Implicit conversion from an untyped empty list
    /// </summary>
    [Pure]
    public static implicit operator Iterable<A>(UnitCollection _) =>
        Empty;

    /// <summary>
    /// Map the sequence using the function provided
    /// </summary>
    /// <typeparam name="B"></typeparam>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped sequence</returns>
    [Pure]
    public Iterable<B> Select<B>(Func<A, long, B> f) =>
        new(iterator.Map(f));

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

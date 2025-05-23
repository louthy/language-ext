﻿using System;
using System.Collections.Generic;
using System.Collections;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Numerics;
using System.Runtime.CompilerServices;
using LanguageExt.ClassInstances;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Immutable set
/// AVL tree implementation
/// AVL tree is a self-balancing binary search tree. 
/// [wikipedia.org/wiki/AVL_tree](http://en.wikipedia.org/wiki/AVL_tree)
/// </summary>
/// <typeparam name="A">Set item type</typeparam>
[Serializable]
[CollectionBuilder(typeof(Set), nameof(Set.createRange))]
public readonly struct Set<A> : 
    IEquatable<Set<A>>,
    IComparable<Set<A>>,
    IComparable,
    IComparisonOperators<Set<A>, Set<A>, bool>,
    IAdditionOperators<Set<A>, Set<A>, Set<A>>,
    ISubtractionOperators<Set<A>, Set<A>, Set<A>>,
    IAdditiveIdentity<Set<A>, Set<A>>,
    IReadOnlyCollection<A>,
    Monoid<Set<A>>,
    K<Set, A>
{
    public static Set<A> Empty { get; } = new(SetInternal<OrdDefault<A>, A>.Empty);

    readonly SetInternal<OrdDefault<A>, A> value;

    internal SetInternal<OrdDefault<A>, A> Value => value ?? Empty.Value;

    /// <summary>
    /// Ctor from an enumerable 
    /// </summary>
    public Set(IEnumerable<A> items) : this(items, true)
    {
    }

    /// <summary>
    /// Ctor from an enumerable 
    /// </summary>
    public Set(ReadOnlySpan<A> items) : this(items, true)
    {
    }

    /// <summary>
    /// Default ctor
    /// </summary>
    internal Set(SetInternal<OrdDefault<A>, A> set) =>
        value = set;

    /// <summary>
    /// Ctor that takes a root element
    /// </summary>
    /// <param name="root"></param>
    internal Set(SetItem<A> root) =>
        value = new SetInternal<OrdDefault<A>, A>(root);

    /// <summary>
    /// Ctor that takes an initial (distinct) set of items
    /// </summary>
    /// <param name="items"></param>
    public Set(IEnumerable<A> items, bool tryAdd) =>
        value = new SetInternal<OrdDefault<A>, A>(
            items, 
            tryAdd
                ? SetModuleM.AddOpt.TryAdd
                : SetModuleM.AddOpt.ThrowOnDuplicate);

    /// <summary>
    /// Ctor that takes an initial (distinct) set of items
    /// </summary>
    /// <param name="items"></param>
    public Set(ReadOnlySpan<A> items, bool tryAdd) =>
        value = new SetInternal<OrdDefault<A>, A>(
            items, 
            tryAdd
                ? SetModuleM.AddOpt.TryAdd
                : SetModuleM.AddOpt.ThrowOnDuplicate);

    /// <summary>
    /// Item at index lens
    /// </summary>
    [Pure]
    public static Lens<Set<A>, bool> item(A key) => Lens<Set<A>, bool>.New(
        Get: la => la.Contains(key),
        Set: a => la => a ? la.AddOrUpdate(key) : la.Remove(key));

    /// <summary>
    /// Lens map
    /// </summary>
    [Pure]
    public static Lens<Set<A>, Set<A>> map(Lens<A, A> lens) => Lens<Set<A>, Set<A>>.New(
        Get: la => la.Map(lens.Get),
        Set: lb => la =>
                   {
                       foreach (var item in lb)
                       {
                           la = la.Find(item).Match(Some: x => la.AddOrUpdate(lens.Set(x, item)), None: () => la);
                       }
                       return la;
                   });

    static Set<A> Wrap(SetInternal<OrdDefault<A>, A> set) =>
        new (set);

    static Set<B> Wrap<B>(SetInternal<OrdDefault<B>, B> set) =>
        new (set);

    /// <summary>
    /// Reference version for use in pattern-matching
    /// </summary>
    /// <remarks>
    ///
    ///     Empty collection     = null
    ///     Singleton collection = A
    ///     More                 = (A, Seq〈A〉)   -- head and tail
    ///
    ///     var res = set.Case switch
    ///     {
    ///       
    ///        (var x, var xs) => ...,
    ///        A value         => ...,
    ///        _               => ...
    ///     }
    /// 
    /// </remarks>
    [Pure]
    public object? Case =>
        IsEmpty
            ? null
            : toSeq(Value).Case;

    /*
    /// <summary>
    /// Stream as an enumerable
    /// </summary>
    [Pure]
    public StreamT<M, A> AsStream<M>()
        where M : Monad<M> =>
        StreamT<M, A>.Lift(AsEnumerable());
        */

    /// <summary>
    /// Add an item to the set
    /// </summary>
    /// <param name="value">Value to add to the set</param>
    /// <returns>New set with the item added</returns>
    [Pure]
    public Set<A> Add(A value) =>
        Wrap(Value.Add(value));

    /// <summary>
    /// Attempt to add an item to the set.  If an item already
    /// exists then return the Set as-is.
    /// </summary>
    /// <param name="value">Value to add to the set</param>
    /// <returns>New set with the item maybe added</returns>
    [Pure]
    public Set<A> TryAdd(A value) =>
        Wrap(Value.TryAdd(value));

    /// <summary>
    /// Add an item to the set.  If an item already
    /// exists then replace it.
    /// </summary>
    /// <param name="value">Value to add to the set</param>
    /// <returns>New set with the item maybe added</returns>
    [Pure]
    public Set<A> AddOrUpdate(A value) =>
        Wrap(Value.AddOrUpdate(value));

    /// <summary>
    /// Atomically adds a range of items to the set.
    /// </summary>
    /// <remarks>Null is not allowed for a Key</remarks>
    /// <param name="range">Range of keys to add</param>
    /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys already exist</exception>
    /// <exception cref="ArgumentNullException">Throws ArgumentNullException if any of the keys are null</exception>
    /// <returns>New Set with the items added</returns>
    [Pure]
    public Set<A> AddRange(IEnumerable<A> range) =>
        Wrap(Value.AddRange(range));

    /// <summary>
    /// Atomically adds a range of items to the set.  If an item already exists, it's ignored.
    /// </summary>
    /// <remarks>Null is not allowed for a Key</remarks>
    /// <param name="range">Range of keys to add</param>
    /// <exception cref="ArgumentNullException">Throws ArgumentNullException if any of the keys are null</exception>
    /// <returns>New Set with the items added</returns>
    [Pure]
    public Set<A> TryAddRange(IEnumerable<A> range) =>
        Wrap(Value.TryAddRange(range));

    /// <summary>
    /// Atomically adds a range of items to the set.  If an item already exists then replace it.
    /// </summary>
    /// <remarks>Null is not allowed for a Key</remarks>
    /// <param name="range">Range of keys to add</param>
    /// <exception cref="ArgumentNullException">Throws ArgumentNullException if any of the keys are null</exception>
    /// <returns>New Set with the items added</returns>
    [Pure]
    public Set<A> AddOrUpdateRange(IEnumerable<A> range) =>
        Wrap(Value.AddOrUpdateRange(range));

    /// <summary>
    /// Attempts to find an item in the set.  
    /// </summary>
    /// <param name="value">Value to find</param>
    /// <returns>Some(T) if found, None otherwise</returns>
    [Pure]
    public Option<A> Find(A value) =>
        Value.Find(value);

    /// <summary>
    /// Retrieve a range of values 
    /// </summary>
    /// <param name="keyFrom">Range start (inclusive)</param>
    /// <param name="keyTo">Range to (inclusive)</param>
    /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keyFrom or keyTo are null</exception>
    /// <returns>Range of values</returns>
    [Pure]
    public Iterable<A> FindRange(A keyFrom, A keyTo) => Value.FindRange(keyFrom, keyTo);

    /// <summary>
    /// Retrieve the value from previous item to specified key
    /// </summary>
    /// <param name="key">Key to find</param>
    /// <returns>Found key</returns>
    [Pure]
    public Option<A> FindPredecessor(A key) => Value.FindPredecessor(key);

    /// <summary>
    /// Retrieve the value from exact key, or if not found, the previous item 
    /// </summary>
    /// <param name="key">Key to find</param>
    /// <returns>Found key</returns>
    [Pure]
    public Option<A> FindExactOrPredecessor(A key) => Value.FindOrPredecessor(key);

    /// <summary>
    /// Retrieve the value from next item to specified key
    /// </summary>
    /// <param name="key">Key to find</param>
    /// <returns>Found key</returns>
    [Pure]
    public Option<A> FindSuccessor(A key) => Value.FindSuccessor(key);

    /// <summary>
    /// Retrieve the value from exact key, or if not found, the next item 
    /// </summary>
    /// <param name="key">Key to find</param>
    /// <returns>Found key</returns>
    [Pure]
    public Option<A> FindExactOrSuccessor(A key) => Value.FindOrSuccessor(key);

    /// <summary>
    /// Returns the elements that are in both this and other
    /// </summary>
    [Pure]
    public Set<A> Intersect(IEnumerable<A> other) =>
        Wrap(Value.Intersect(other));

    /// <summary>
    /// Returns this - other.  Only the items in this that are not in 
    /// other will be returned.
    /// </summary>
    [Pure]
    public Set<A> Except(IEnumerable<A> other) =>
        other is Set<A> set
            ? Except(set)
            : Wrap(Value.Except(other));

    /// <summary>
    /// Returns this - other.  Only the items in this that are not in 
    /// other will be returned.
    /// </summary>
    [Pure]
    public Set<A> Except(Set<A> other) =>
        Wrap(Value.Except(other.Value));

    /// <summary>
    /// Only items that are in one set or the other will be returned.
    /// If an item is in both, it is dropped.
    /// </summary>
    [Pure]
    public Set<A> SymmetricExcept(IEnumerable<A> other) =>
        other is Set<A> set
            ? SymmetricExcept(set)
            : Wrap(Value.SymmetricExcept(other));

    /// <summary>
    /// Only items that are in one set or the other will be returned.
    /// If an item is in both, it is dropped.
    /// </summary>
    [Pure]
    public Set<A> SymmetricExcept(Set<A> other) =>
        Wrap(Value.SymmetricExcept(other.Value));

    /// <summary>
    /// Finds the union of two sets and produces a new set with 
    /// the results
    /// </summary>
    /// <param name="other">Other set to union with</param>
    /// <returns>A set which contains all items from both sets</returns>
    [Pure]
    public Set<A> Union(IEnumerable<A> other) =>
        Wrap(Value.Union(other));

    /// <summary>
    /// Clears the set
    /// </summary>
    /// <returns>An empty set</returns>
    [Pure]
    public Set<A> Clear() =>
        Empty;

    /// <summary>
    /// Get enumerator
    /// </summary>
    /// <returns>IEnumerator T</returns>
    [Pure]
    public IEnumerator<A> GetEnumerator() =>
        Value.GetEnumerator();

    /// <summary>
    /// Get enumerator
    /// </summary>
    /// <returns>IEnumerator</returns>
    [Pure]
    IEnumerator IEnumerable.GetEnumerator() =>
        // ReSharper disable once NotDisposedResourceIsReturned
        Value.GetEnumerator();

    /// <summary>
    /// Removes an item from the set (if it exists)
    /// </summary>
    /// <param name="value">Value to remove</param>
    /// <returns>New set with item removed</returns>
    [Pure]
    public Set<A> Remove(A value) =>
        Wrap(Value.Remove(value));

    /// <summary>
    /// Removes a range of items from the set (if they exist)
    /// </summary>
    /// <param name="value">Value to remove</param>
    /// <returns>New set with items removed</returns>
    [Pure]
    public Set<A> RemoveRange(IEnumerable<A> values)
    {
        var set = Value;
        foreach(var x in values)
        {
            set = set.Remove(x);
        }
        return Wrap(set);
    }

    /// <summary>
    /// Impure iteration of the bound value in the structure
    /// </summary>
    /// <returns>
    /// Returns the original unmodified structure
    /// </returns>
    public Set<A> Do(Action<A> f)
    {
        this.Iter(f);
        return this;
    }

    /// <summary>
    /// Maps the values of this set into a new set of values using the
    /// mapper function to tranform the source values.
    /// </summary>
    /// <typeparam name="R">Mapped element type</typeparam>
    /// <param name="mapper">Mapping function</param>
    /// <returns>Mapped Set</returns>
    [Pure]
    public Set<B> Map<B>(Func<A, B> map) =>
        Wrap(Value.Map<OrdDefault<B>, B>(map));
    
    /// <summary>
    /// Map each element of a structure to an action, evaluate these actions from
    /// left to right, and collect the results.
    /// </summary>
    /// <param name="f"></param>
    /// <param name="ta">Traversable structure</param>
    /// <typeparam name="F">Applicative functor trait</typeparam>
    /// <typeparam name="B">Bound value (output)</typeparam>
    [Pure]
    public K<F, Set<B>> Traverse<F, B>(Func<A, K<F, B>> f) 
        where F : Applicative<F> =>
        F.Map(x => x.As(), Traversable.traverse(f, this));
        
    /// <summary>
    /// Map each element of a structure to an action, evaluate these actions from
    /// left to right, and collect the results.
    /// </summary>
    /// <param name="f"></param>
    /// <param name="ta">Traversable structure</param>
    /// <typeparam name="M">Monad trait</typeparam>
    /// <typeparam name="B">Bound value (output)</typeparam>
    [Pure]
    public K<M, Set<B>> TraverseM<M, B>(Func<A, K<M, B>> f) 
        where M : Monad<M> =>
        M.Map(x => x.As(), Traversable.traverseM(f, this));
    
    /// <summary>
    /// Filters items from the set using the predicate.  If the predicate
    /// returns True for any item then it remains in the set, otherwise
    /// it's dropped.
    /// </summary>
    /// <param name="pred">Predicate</param>
    /// <returns>Filtered enumerable</returns>
    [Pure]
    public Set<A> Filter(Func<A, bool> pred) =>
        Wrap(Value.Filter(pred));

    /// <summary>
    /// Check the existence of an item in the set using a 
    /// predicate.
    /// </summary>
    /// <remarks>Note this scans the entire set.</remarks>
    /// <param name="pred">Predicate</param>
    /// <returns>True if predicate returns true for any item</returns>
    [Pure]
    public bool Exists(Func<A, bool> pred) =>
        Value.Exists(pred);

    /// <summary>
    /// Returns True if the value is in the set
    /// </summary>
    /// <param name="value">Value to check</param>
    /// <returns>True if the item 'value' is in the Set 'set'</returns>
    [Pure]
    public bool Contains(A value) =>
        Value.Contains(value);

    /// <summary>
    /// Returns true if both sets contain the same elements
    /// </summary>
    /// <param name="other">Other distinct set to compare</param>
    /// <returns>True if the sets are equal</returns>
    [Pure]
    public bool SetEquals(IEnumerable<A> other) =>
        Value.SetEquals(other);

    /// <summary>
    /// Is the set empty
    /// </summary>
    [Pure]
    public bool IsEmpty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => value?.IsEmpty ?? true;
    }

    /// <summary>
    /// Number of items in the set
    /// </summary>
    [Pure]
    public int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => value?.Count ?? 0;
    }

    /// <summary>
    /// Alias of Count
    /// </summary>
    [Pure]
    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => value?.Count ?? 0;
    }
        
    /// <summary>
    /// Returns True if 'other' is a proper subset of this set
    /// </summary>
    /// <returns>True if 'other' is a proper subset of this set</returns>
    [Pure]
    public bool IsProperSubsetOf(IEnumerable<A> other) =>
        Value.IsProperSubsetOf(other);

    /// <summary>
    /// Returns True if 'other' is a proper superset of this set
    /// </summary>
    /// <returns>True if 'other' is a proper superset of this set</returns>
    [Pure]
    public bool IsProperSupersetOf(IEnumerable<A> other) =>
        Value.IsProperSupersetOf(other);

    /// <summary>
    /// Returns True if 'other' is a superset of this set
    /// </summary>
    /// <returns>True if 'other' is a superset of this set</returns>
    [Pure]
    public bool IsSubsetOf(IEnumerable<A> other) =>
        Value.IsSubsetOf(other);

    /// <summary>
    /// Returns True if 'other' is a superset of this set
    /// </summary>
    /// <returns>True if 'other' is a superset of this set</returns>
    [Pure]
    public bool IsSupersetOf(IEnumerable<A> other) =>
        Value.IsSupersetOf(other);

    /// <summary>
    /// Returns True if other overlaps this set
    /// </summary>
    /// <typeparam name="T">Element type</typeparam>
    /// <param name="setA">Set A</param>
    /// <param name="setB">Set B</param>
    /// <returns>True if other overlaps this set</returns>
    [Pure]
    public bool Overlaps(IEnumerable<A> other) =>
        Value.Overlaps(other);

    /// <summary>
    /// Copy the items from the set into the specified array
    /// </summary>
    /// <param name="array">Array to copy to</param>
    /// <param name="index">Index into the array to start</param>
    public void CopyTo(A[] array, int index) =>
        Value.CopyTo(array, index);

    /// <summary>
    /// Copy the items from the set into the specified array
    /// </summary>
    /// <param name="array">Array to copy to</param>
    /// <param name="index">Index into the array to start</param>
    public void CopyTo(Array array, int index) =>
        Value.CopyTo(array, index);

    /// <summary>
    /// Add operator + performs a union of the two sets
    /// </summary>
    /// <param name="lhs">Left hand side set</param>
    /// <param name="rhs">Right hand side set</param>
    /// <returns>Unioned set</returns>
    [Pure]
    public static Set<A> operator +(Set<A> lhs, Set<A> rhs) =>
        Wrap(lhs.Value + rhs.Value);

    /// <summary>
    /// Choice operator
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Set<A> operator |(Set<A> x, K<Set, A> y) =>
        x.Choose(y).As();

    /// <summary>
    /// Choice operator
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Set<A> operator |(K<Set, A> x, Set<A> y) =>
        x.Choose(y).As();

    /// <summary>
    /// Append performs a union of the two sets
    /// </summary>
    /// <param name="rhs">Right hand side set</param>
    /// <returns>Unioned set</returns>
    [Pure]
    public Set<A> Combine(Set<A> rhs) =>
        Wrap(Value.Append(rhs.Value));

    /// <summary>
    /// Subtract operator - performs a subtract of the two sets
    /// </summary>
    /// <param name="lhs">Left hand side set</param>
    /// <param name="rhs">Right hand side set</param>
    /// <returns>Subtractd set</returns>
    [Pure]
    public static Set<A> operator -(Set<A> lhs, Set<A> rhs) =>
        Wrap(lhs.Value - rhs.Value);

    /// <summary>
    /// Subtract operator - performs a subtract of the two sets
    /// </summary>
    /// <param name="rhs">Right hand side set</param>
    /// <returns>Subtracted set</returns>
    [Pure]
    public Set<A> Subtract(Set<A> rhs) =>
        Wrap(Value.Subtract(rhs.Value));

    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="other">Other set to test</param>
    /// <returns>True if sets are equal</returns>
    [Pure]
    public bool Equals(Set<A> other) =>
        Value.SetEquals(other.Value.AsIterable());

    /// <summary>
    /// Equality operator
    /// </summary>
    /// <param name="lhs">Left hand side set</param>
    /// <param name="rhs">Right hand side set</param>
    /// <returns>True if the two sets are equal</returns>
    [Pure]
    public static bool operator ==(Set<A> lhs, Set<A> rhs) =>
        lhs.Equals(rhs);

    /// <summary>
    /// Non-equality operator
    /// </summary>
    /// <param name="lhs">Left hand side set</param>
    /// <param name="rhs">Right hand side set</param>
    /// <returns>True if the two sets are equal</returns>
    [Pure]
    public static bool operator !=(Set<A> lhs, Set<A> rhs) =>
        !lhs.Equals(rhs);

    [Pure]
    public static bool operator <(Set<A> lhs, Set<A> rhs) =>
        lhs.CompareTo(rhs) < 0;

    [Pure]
    public static bool operator <=(Set<A> lhs, Set<A> rhs) =>
        lhs.CompareTo(rhs) <= 0;

    [Pure]
    public static bool operator >(Set<A> lhs, Set<A> rhs) =>
        lhs.CompareTo(rhs) > 0;

    [Pure]
    public static bool operator >=(Set<A> lhs, Set<A> rhs) =>
        lhs.CompareTo(rhs) >= 0;

    /// <summary>
    /// Equality override
    /// </summary>
    [Pure]
    public override bool Equals(object? obj) =>
        obj is Set<A> set && Equals(set);

    /// <summary>
    /// Get the hash code.  Calculated from all items in the set.
    /// </summary>
    /// <remarks>
    /// The hash-code is cached after the first read.
    /// </remarks>
    [Pure]
    public override int GetHashCode() =>
        Value.GetHashCode();

    [Pure]
    public int CompareTo(object? obj) =>
        obj is Set<A> t ? CompareTo(t) : 1;

    /// <summary>
    /// Format the collection as `[a, b, c, ...]`
    /// The ellipsis is used for collections over 50 items
    /// To get a formatted string with all the items, use `ToFullString`
    /// or `ToFullArrayString`.
    /// </summary>
    [Pure]
    public override string ToString() =>
        CollectionFormat.ToShortArrayString(this, Count);

    /// <summary>
    /// Format the collection as `a, b, c, ...`
    /// </summary>
    [Pure]
    public string ToFullString(string separator = ", ") =>
        CollectionFormat.ToFullString(this, separator);

    /// <summary>
    /// Format the collection as `[a, b, c, ...]`
    /// </summary>
    [Pure]
    public string ToFullArrayString(string separator = ", ") =>
        CollectionFormat.ToFullArrayString(this, separator);

    [Pure]
    public Seq<A> ToSeq() =>
        toSeq(this);

    [Pure]
    public Iterable<A> AsEnumerable() =>
        Iterable.createRange(this);

    [Pure]
    public Set<B> Select<B>(Func<A, B> f) =>
        Map(f);

    [Pure]
    public Set<A> Where(Func<A, bool> pred) =>
        Filter(pred);

    [Pure]
    public Set<B> Bind<B>(Func<A, Set<B>> f)
    {
        var self = this;

        IEnumerable<B> Yield()
        {
            foreach (var x in self.AsEnumerable())
            {
                foreach (var y in f(x))
                {
                    yield return y;
                }
            }
        }
        return new Set<B>(Yield(), true);
    }

    [Pure]
    public Set<C> SelectMany<B, C>(Func<A, Set<B>> bind, Func<A, B, C> project)
    {
        var self = this;

        IEnumerable<C> Yield()
        {
            foreach(var x in self.AsEnumerable())
            {
                foreach(var y in bind(x))
                {
                    yield return project(x, y);
                }
            }
        }
        return new Set<C>(Yield(), true);
    }

    [Pure]
    public Iterable<A> Skip(int amount) =>
        Value.Skip(amount);

    [Pure]
    public int CompareTo(Set<A> other) =>
        Value.CompareTo(other.Value);

    [Pure]
    public int CompareTo<OrdA>(Set<A> other) where OrdA : Ord<A> =>
        Value.CompareTo<OrdA>(other.Value);

    /// <summary>
    /// Implicit conversion from an untyped empty list
    /// </summary>
    [Pure]
    public static implicit operator Set<A>(SeqEmpty _) =>
        Empty;

    /// <summary>
    /// Creates a new map from a range/slice of this map
    /// </summary>
    /// <param name="keyFrom">Range start (inclusive)</param>
    /// <param name="keyTo">Range to (inclusive)</param>
    /// <returns></returns>
    [Pure]
    public Set<A> Slice(A keyFrom, A keyTo) =>
        new (FindRange(keyFrom, keyTo));

    /// <summary>
    /// Find the lowest ordered item in the set
    /// </summary>
    [Pure]
    public Option<A> Min => Value.Min;

    /// <summary>
    /// Find the highest ordered item in the set
    /// </summary>
    [Pure]
    public Option<A> Max => Value.Max;

    public static Set<A> AdditiveIdentity => 
        Empty;
}

using System;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// Immutable set
    /// AVL tree implementation
    /// AVL tree is a self-balancing binary search tree. 
    /// [wikipedia.org/wiki/AVL_tree](http://en.wikipedia.org/wiki/AVL_tree)
    /// </summary>
    /// <typeparam name="A">Set item type</typeparam>
    [Serializable]
    public readonly struct Set<OrdA, A> :
        IEnumerable<A>,
        IEquatable<Set<OrdA, A>>,
        IComparable<Set<OrdA, A>>
        where OrdA : struct, Ord<A>
    {
        public static readonly Set<OrdA, A> Empty = new Set<OrdA, A>(SetInternal<OrdA, A>.Empty);

        readonly SetInternal<OrdA, A> value;

        SetInternal<OrdA, A> Value => value ?? Empty.Value;

        /// <summary>
        /// Ctor from an enumerable 
        /// </summary>
        public Set(IEnumerable<A> items) : this(items, true)
        {
        }

        /// <summary>
        /// Default ctor
        /// </summary>
        internal Set(SetInternal<OrdA, A> set) =>
            value = set;

        /// <summary>
        /// Ctor that takes a root element
        /// </summary>
        /// <param name="root"></param>
        internal Set(SetItem<A> root) =>
            value = new SetInternal<OrdA, A>(root);

        /// <summary>
        /// Ctor that takes an initial (distinct) set of items
        /// </summary>
        /// <param name="items"></param>
        public Set(IEnumerable<A> items, bool tryAdd) =>
            value = new SetInternal<OrdA, A>(
                items, 
                tryAdd
                    ? SetModuleM.AddOpt.TryAdd
                    : SetModuleM.AddOpt.ThrowOnDuplicate);

        static Set<OrdA, A> Wrap(SetInternal<OrdA, A> set) =>
            new Set<OrdA, A>(set);

        static Set<OrdB, B> Wrap<OrdB, B>(SetInternal<OrdB, B> set) where OrdB : struct, Ord<B>  =>
            new Set<OrdB, B>(set);

        /// <summary>
        /// Reference version for use in pattern-matching
        /// </summary>
        /// <remarks>
        ///
        ///     Empty collection     = null
        ///     Singleton collection = A
        ///     More                 = (A, Seq<A>)   -- head and tail
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
        public object Case =>
            IsEmpty
                ? null
                : toSeq(Value).Case;

        /// <summary>
        /// Add an item to the set
        /// </summary>
        /// <param name="value">Value to add to the set</param>
        /// <returns>New set with the item added</returns>
        [Pure]
        public Set<OrdA, A> Add(A value) =>
            Wrap(Value.Add(value));

        /// <summary>
        /// Attempt to add an item to the set.  If an item already
        /// exists then return the Set as-is.
        /// </summary>
        /// <param name="value">Value to add to the set</param>
        /// <returns>New set with the item maybe added</returns>
        [Pure]
        public Set<OrdA, A> TryAdd(A value) =>
            Wrap(Value.TryAdd(value));

        /// <summary>
        /// Add an item to the set.  If an item already
        /// exists then replace it.
        /// </summary>
        /// <param name="value">Value to add to the set</param>
        /// <returns>New set with the item maybe added</returns>
        [Pure]
        public Set<OrdA, A> AddOrUpdate(A value) =>
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
        public Set<OrdA, A> AddRange(IEnumerable<A> range) =>
            Wrap(Value.AddRange(range));

        /// <summary>
        /// Atomically adds a range of items to the set.  If an item already exists, it's ignored.
        /// </summary>
        /// <remarks>Null is not allowed for a Key</remarks>
        /// <param name="range">Range of keys to add</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException if any of the keys are null</exception>
        /// <returns>New Set with the items added</returns>
        [Pure]
        public Set<OrdA, A> TryAddRange(IEnumerable<A> range) =>
            Wrap(Value.TryAddRange(range));

        /// <summary>
        /// Atomically adds a range of items to the set.  If an item already exists then replace it.
        /// </summary>
        /// <remarks>Null is not allowed for a Key</remarks>
        /// <param name="range">Range of keys to add</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException if any of the keys are null</exception>
        /// <returns>New Set with the items added</returns>
        [Pure]
        public Set<OrdA, A> AddOrUpdateRange(IEnumerable<A> range) =>
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
        /// Retrieve a range of values 
        /// </summary>
        /// <param name="keyFrom">Range start (inclusive)</param>
        /// <param name="keyTo">Range to (inclusive)</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keyFrom or keyTo are null</exception>
        /// <returns>Range of values</returns>
        [Pure]
        public IEnumerable<A> FindRange(A keyFrom, A keyTo) => Value.FindRange(keyFrom, keyTo);

        /// <summary>
        /// Returns the elements that are in both this and other
        /// </summary>
        [Pure]
        public Set<OrdA, A> Intersect(Set<OrdA, A> other) =>
            Wrap(Value.Intersect(other));

        /// <summary>
        /// Returns this - other.  Only the items in this that are not in 
        /// other will be returned.
        /// </summary>
        [Pure]
        public Set<OrdA, A> Except(Set<OrdA, A> other) =>
            Wrap(Value.Except(other.Value));

        /// <summary>
        /// Returns this - other.  Only the items in this that are not in 
        /// other will be returned.
        /// </summary>
        [Pure]
        public Set<OrdA, A> Except(IEnumerable<A> other) =>
            other is Set<OrdA, A> rhs
                ? Except(rhs)
                : Wrap(Value.Except(other));

        /// <summary>
        /// Only items that are in one set or the other will be returned.
        /// If an item is in both, it is dropped.
        /// </summary>
        [Pure]
        public Set<OrdA, A> SymmetricExcept(Set<OrdA, A> other) =>
            Wrap(Value.SymmetricExcept(other));

        /// <summary>
        /// Only items that are in one set or the other will be returned.
        /// If an item is in both, it is dropped.
        /// </summary>
        [Pure]
        public Set<OrdA, A> SymmetricExcept(IEnumerable<A> other) =>
            other is Set<OrdA, A> rhs
                ? SymmetricExcept(rhs)
                : Wrap(Value.SymmetricExcept(other));

        /// <summary>
        /// Finds the union of two sets and produces a new set with 
        /// the results
        /// </summary>
        /// <param name="other">Other set to union with</param>
        /// <returns>A set which contains all items from both sets</returns>
        [Pure]
        public Set<OrdA, A> Union(Set<OrdA, A> other) =>
            Wrap(Value.Union(other));

        /// <summary>
        /// Clears the set
        /// </summary>
        /// <returns>An empty set</returns>
        [Pure]
        public Set<OrdA, A> Clear() =>
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
            Value.GetEnumerator();

        /// <summary>
        /// Removes an item from the set (if it exists)
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <returns>New set with item removed</returns>
        [Pure]
        public Set<OrdA, A> Remove(A value) =>
            Wrap(Value.Remove(value));

        /// <summary>
        /// Applies a function 'folder' to each element of the collection, threading an accumulator 
        /// argument through the computation. The fold function takes the state argument, and 
        /// applies the function 'folder' to it and the first element of the set. Then, it feeds this 
        /// result into the function 'folder' along with the second element, and so on. It returns the 
        /// final result. (Aggregate in LINQ)
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <returns>Aggregate value</returns>
        [Pure]
        public S Fold<S>(S state, Func<S, A, S> folder) =>
            Value.Fold(state,folder);

        /// <summary>
        /// Applies a function 'folder' to each element of the collection (from last element to first), 
        /// threading an aggregate state through the computation. The fold function takes the state 
        /// argument, and applies the function 'folder' to it and the first element of the set. Then, 
        /// it feeds this result into the function 'folder' along with the second element, and so on. It 
        /// returns the final result.
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <returns>Aggregate value</returns>
        [Pure]
        public S FoldBack<S>(S state, Func<S, A, S> folder) =>
            Value.FoldBack(state, folder);

        /// <summary>
        /// Impure iteration of the bound values in the structure
        /// </summary>
        /// <returns>
        /// Returns the original unmodified structure
        /// </returns>
        public Set<OrdA, A> Do(Action<A> f)
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
        public Set<OrdB, B> Map<OrdB, B>(Func<A, B> map) where OrdB : struct, Ord<B> =>
            Wrap(Value.Map<OrdB, B>(map));

        /// <summary>
        /// Maps the values of this set into a new set of values using the
        /// mapper function to tranform the source values.
        /// </summary>
        /// <typeparam name="R">Mapped element type</typeparam>
        /// <param name="mapper">Mapping function</param>
        /// <returns>Mapped Set</returns>
        [Pure]
        public Set<OrdA, A> Map(Func<A, A> map) =>
            Wrap(Value.Map(map));

        /// <summary>
        /// Filters items from the set using the predicate.  If the predicate
        /// returns True for any item then it remains in the set, otherwise
        /// it's dropped.
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>Filtered enumerable</returns>
        [Pure]
        public Set<OrdA, A> Filter(Func<A, bool> pred) =>
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
        public bool SetEquals(Set<OrdA, A> other) =>
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
        public bool IsProperSubsetOf(Set<OrdA, A> other) =>
            Value.IsProperSubsetOf(other);

        /// <summary>
        /// Returns True if 'other' is a proper superset of this set
        /// </summary>
        /// <returns>True if 'other' is a proper superset of this set</returns>
        [Pure]
        public bool IsProperSupersetOf(Set<OrdA, A> other) =>
            Value.IsProperSupersetOf(other);

        /// <summary>
        /// Returns True if 'other' is a superset of this set
        /// </summary>
        /// <returns>True if 'other' is a superset of this set</returns>
        [Pure]
        public bool IsSubsetOf(Set<OrdA, A> other) =>
            Value.IsSubsetOf(other);

        /// <summary>
        /// Returns True if 'other' is a superset of this set
        /// </summary>
        /// <returns>True if 'other' is a superset of this set</returns>
        [Pure]
        public bool IsSupersetOf(Set<OrdA, A> other) =>
            Value.IsSupersetOf(other);

        /// <summary>
        /// Returns True if other overlaps this set
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="setA">Set A</param>
        /// <param name="setB">Set B</param>
        /// <returns>True if other overlaps this set</returns>
        [Pure]
        public bool Overlaps(Set<OrdA, A> other) =>
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
        public void CopyTo(System.Array array, int index) =>
            Value.CopyTo(array, index);

        /// <summary>
        /// Add operator - performs a union of the two sets
        /// </summary>
        /// <param name="lhs">Left hand side set</param>
        /// <param name="rhs">Right hand side set</param>
        /// <returns>Unioned set</returns>
        [Pure]
        public static Set<OrdA, A> operator +(Set<OrdA, A> lhs, Set<OrdA, A> rhs) =>
            Wrap(lhs.Value + rhs.Value);

        /// <summary>
        /// Add operator - performs a union of the two sets
        /// </summary>
        /// <param name="rhs">Right hand side set</param>
        /// <returns>Unioned set</returns>
        [Pure]
        public Set<OrdA, A> Append(Set<OrdA, A> rhs) =>
            Wrap(Value.Append(rhs.Value));

        /// <summary>
        /// Subtract operator - performs a subtract of the two sets
        /// </summary>
        /// <param name="lhs">Left hand side set</param>
        /// <param name="rhs">Right hand side set</param>
        /// <returns>Subtractd set</returns>
        [Pure]
        public static Set<OrdA, A> operator -(Set<OrdA, A> lhs, Set<OrdA, A> rhs) =>
            Wrap(lhs.Value - rhs.Value);

        /// <summary>
        /// Subtract operator - performs a subtract of the two sets
        /// </summary>
        /// <param name="rhs">Right hand side set</param>
        /// <returns>Subtracted set</returns>
        [Pure]
        public Set<OrdA, A> Subtract(Set<OrdA, A> rhs) =>
            Wrap(Value.Subtract(rhs.Value));

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="other">Other set to test</param>
        /// <returns>True if sets are equal</returns>
        [Pure]
        public bool Equals(Set<OrdA, A> other) =>
            Value.SetEquals(other.Value.AsEnumerable());

        /// <summary>
        /// Equality operator
        /// </summary>
        /// <param name="lhs">Left hand side set</param>
        /// <param name="rhs">Right hand side set</param>
        /// <returns>True if the two sets are equal</returns>
        [Pure]
        public static bool operator ==(Set<OrdA, A> lhs, Set<OrdA, A> rhs) =>
            lhs.Equals(rhs);

        /// <summary>
        /// Non-equality operator
        /// </summary>
        /// <param name="lhs">Left hand side set</param>
        /// <param name="rhs">Right hand side set</param>
        /// <returns>True if the two sets are equal</returns>
        [Pure]
        public static bool operator !=(Set<OrdA, A> lhs, Set<OrdA, A> rhs) =>
            !lhs.Equals(rhs);

        [Pure]
        public static bool operator <(Set<OrdA, A> lhs, Set<OrdA, A> rhs) =>
            lhs.CompareTo(rhs) < 0;

        [Pure]
        public static bool operator <=(Set<OrdA, A> lhs, Set<OrdA, A> rhs) =>
            lhs.CompareTo(rhs) <= 0;

        [Pure]
        public static bool operator >(Set<OrdA, A> lhs, Set<OrdA, A> rhs) =>
            lhs.CompareTo(rhs) > 0;

        [Pure]
        public static bool operator >=(Set<OrdA, A> lhs, Set<OrdA, A> rhs) =>
            lhs.CompareTo(rhs) >= 0;

        /// <summary>
        /// Equality override
        /// </summary>
        [Pure]
        public override bool Equals(object obj) =>
            obj is Set<OrdA, A> && 
            !ReferenceEquals(obj, null) &&
            Equals((Set<OrdA, A>)obj);

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
        public override string ToString() =>
            $"Set[{Count}]";

        [Pure]
        public Seq<A> ToSeq() =>
            Prelude.toSeq(this);

        [Pure]
        public IEnumerable<A> AsEnumerable() => this;

        [Pure]
        public Set<OrdA, A> Where(Func<A, bool> pred) =>
            Filter(pred);

        [Pure]
        public Set<OrdB, B> Bind<OrdB, B>(Func<A, Set<OrdB, B>> f) where OrdB : struct, Ord<B>
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
            return new Set<OrdB, B>(Yield(), true);
        }

        [Pure]
        public Set<OrdA, A> Bind(Func<A, Set<OrdA, A>> f)
        {
            var self = this;

            IEnumerable<A> Yield()
            {
                foreach (var x in self.AsEnumerable())
                {
                    foreach (var y in f(x))
                    {
                        yield return y;
                    }
                }
            }
            return new Set<OrdA, A>(Yield(), true);
        }

        [Pure]
        public IEnumerable<A> Skip(int amount) =>
            Value.Skip(amount);

        [Pure]
        public int CompareTo(Set<OrdA, A> other) =>
            Value.CompareTo(other.Value);

        /// <summary>
        /// Implicit conversion from an untyped empty list
        /// </summary>
        public static implicit operator Set<OrdA, A>(SeqEmpty _) =>
            Empty;

        /// <summary>
        /// Creates a new map from a range/slice of this map
        /// </summary>
        /// <param name="keyFrom">Range start (inclusive)</param>
        /// <param name="keyTo">Range to (inclusive)</param>
        /// <returns></returns>
        [Pure]
        public Set<OrdA, A> Slice(A keyFrom, A keyTo) =>
            new Set<OrdA, A>(FindRange(keyFrom, keyTo));

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
    }
}

using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using LanguageExt;
using static LanguageExt.Prelude;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    /// <summary>
    /// Immutable set
    /// AVL tree implementation
    /// AVL tree is a self-balancing binary search tree. 
    /// http://en.wikipedia.org/wiki/AVL_tree
    /// </summary>
    /// <typeparam name="A">Set item type</typeparam>
    [Serializable]
    public struct Set<A> :
        IEnumerable<A>,
        IEquatable<Set<A>>,
        IComparable<Set<A>>,
        IReadOnlyCollection<A>
    {
        public static readonly Set<A> Empty = new Set<A>(SetInternal<OrdDefault<A>, A>.Empty);

        readonly SetInternal<OrdDefault<A>, A> value;

        internal SetInternal<OrdDefault<A>, A> Value => value ?? Empty.Value;

        /// <summary>
        /// Ctor from an enumerable 
        /// </summary>
        public Set(IEnumerable<A> items) : this(items, true)
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
        /// Item at index lens
        /// </summary>
        [Pure]
        public static Lens<Set<A>, bool> item(A key) => Lens<Set<A>, bool>.New(
            Get: la => la.Contains(key),
            Set: a => la => a ? la.AddOrUpdate(key) : la.Remove(key)
            );

        /// <summary>
        /// Lens map
        /// </summary>
        [Pure]
        public static Lens<Set<A>, Set<A>> map<B>(Lens<A, A> lens) => Lens<Set<A>, Set<A>>.New(
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
            new Set<A>(set);

        static Set<B> Wrap<B>(SetInternal<OrdDefault<B>, B> set) =>
            new Set<B>(set);

        /// <summary>
        /// Number of items in the set
        /// </summary>
        [Pure]
        public int Count =>
            Value.Count;

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
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys already exist</exception>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException if any of the keys are null</exception>
        /// <returns>New Set with the items added</returns>
        [Pure]
        public Set<A> TryAddRange(IEnumerable<A> range) =>
            Wrap(Value.TryAddRange(range));

        /// <summary>
        /// Atomically adds a range of items to the set.  If any items already exist, they're ignored.
        /// </summary>
        /// <remarks>Null is not allowed for a Key</remarks>
        /// <param name="range">Range of keys to add</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys already exist</exception>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException if any of the keys are null</exception>
        /// <returns>New Set with the items added</returns>
        [Pure]
        public Set<A> AddOrUpdateRange(IEnumerable<A> range) =>
            Wrap(Value.AddOrUpdateRange(range));

        /// <summary>
        /// Get the number of elements in the set
        /// </summary>
        /// <returns>Number of elements</returns>
        [Pure]
        public int Length() =>
            Value.Count;

        /// <summary>
        /// Attempts to find an item in the set.  
        /// </summary>
        /// <param name="value">Value to find</param>
        /// <returns>Some(T) if found, None otherwise</returns>
        [Pure]
        public Option<A> Find(A value) =>
            Value.Find(value);

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
            Value.GetEnumerator();

        /// <summary>
        /// Removes an item from the set (if it exists)
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <returns>New set with item removed</returns>
        [Pure]
        public Set<A> Remove(A value) =>
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
        /// True if the set has no elements
        /// </summary>
        [Pure]
        public bool IsEmpty => 
            Value.IsEmpty;

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
        public void CopyTo(System.Array array, int index) =>
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
        /// Append performs a union of the two sets
        /// </summary>
        /// <param name="rhs">Right hand side set</param>
        /// <returns>Unioned set</returns>
        [Pure]
        public Set<A> Append(Set<A> rhs) =>
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
            Value.SetEquals(other.Value.AsEnumerable());

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
            lhs.Equals(rhs);

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
        public override bool Equals(object obj) =>
            obj is Set<A> && 
            !ReferenceEquals(obj, null) &&
            Equals((Set<A>)obj);

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
            Seq(this);

        [Pure]
        public IEnumerable<A> AsEnumerable() =>
            this;

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
        public IEnumerable<A> Skip(int amount) =>
            Value.Skip(amount);

        [Pure]
        public int CompareTo(Set<A> other) =>
            Value.CompareTo(other.Value);

        [Pure]
        public int CompareTo<OrdA>(Set<A> other) where OrdA : struct, Ord<A> =>
            Value.CompareTo<OrdA>(other.Value);

        /// <summary>
        /// Implicit conversion from an untyped empty list
        /// </summary>
        public static implicit operator Set<A>(SeqEmpty _) =>
            Empty;
    }
}

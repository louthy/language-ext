using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Threading;
using System.ComponentModel;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    /// <summary>
    /// Immutable set
    /// AVL tree implementation
    /// AVL tree is a self-balancing binary search tree. 
    /// http://en.wikipedia.org/wiki/AVL_tree
    /// </summary>
    /// <typeparam name="T">List item type</typeparam>
#if !COREFX
    [Serializable]
#endif
    public class Set<T> : 
        IEnumerable<T>, 
        IEnumerable, 
        IReadOnlyCollection<T>, 
        ICollection<T>, 
        ISet<T>, 
        ICollection, 
        IAppendable<Set<T>>,
        ISubtractable<Set<T>>,
        IMultiplicable<Set<T>>,
        IDivisible<Set<T>>,
        IEquatable<Set<T>>
    {
        public static readonly Set<T> Empty = new Set<T>();
        readonly SetItem<T> set;

        /// <summary>
        /// Default ctor
        /// </summary>
        internal Set()
        {
            set = SetItem<T>.Empty;
        }

        /// <summary>
        /// Ctor that takes a root element
        /// </summary>
        /// <param name="root"></param>
        internal Set(SetItem<T> root)
        {
            set = root;
        }

        /// <summary>
        /// Ctor that takes an initial (distinct) set of items
        /// </summary>
        /// <param name="items"></param>
        internal Set(IEnumerable<T> items, bool checkUniqueness = false)
        {
            set = SetItem<T>.Empty;

            if (checkUniqueness)
            {
                foreach (var item in items)
                {
                    set = SetModule.TryAdd(set, item, Comparer<T>.Default);
                }
            }
            else
            {
                foreach (var item in items)
                {
                    set = SetModule.Add(set, item, Comparer<T>.Default);
                }
            }
        }

        /// <summary>
        /// Number of items in the set
        /// </summary>
        [Pure]
        public int Count
        {
            get
            {
                return set.Count;
            }
        }

        /// <summary>
        /// Add an item to the set
        /// </summary>
        /// <param name="value">Value to add to the set</param>
        /// <returns>New set with the item added</returns>
        [Pure]
        public Set<T> Add(T value) =>
            new Set<T>(SetModule.Add(set,value,Comparer<T>.Default));

        /// <summary>
        /// Attempt to add an item to the set.  If an item already
        /// exists then return the Set as-is.
        /// </summary>
        /// <param name="value">Value to add to the set</param>
        /// <returns>New set with the item maybe added</returns>
        [Pure]
        public Set<T> TryAdd(T value) =>
            Contains(value)
                ? this
                : Add(value);

        /// <summary>
        /// Add an item to the set.  If an item already
        /// exists then replace it.
        /// </summary>
        /// <param name="value">Value to add to the set</param>
        /// <returns>New set with the item maybe added</returns>
        [Pure]
        public Set<T> AddOrUpdate(T value) =>
            new Set<T>(SetModule.AddOrUpdate(set, value, Comparer<T>.Default));

        /// <summary>
        /// Returns true if both sets contain the same elements
        /// </summary>
        [Pure]
        public bool Compare(Set<T> setB) =>
            SetEquals(setB);

        /// <summary>
        /// Get the number of elements in the set
        /// </summary>
        /// <returns>Number of elements</returns>
        [Pure]
        public int Length() =>
            Count;

        /// <summary>
        /// Returns this - setB.  Only the items in this that are not in 
        /// setB will be returned.
        /// </summary>
        [Pure]
        public Set<T> Difference(Set<T> setB) =>
            Except(setB);

        /// <summary>
        /// Attempts to find an item in the set.  
        /// </summary>
        /// <param name="value">Value to find</param>
        /// <returns>Some(T) if found, None otherwise</returns>
        [Pure]
        public Option<T> Find(T value) =>
            SetModule.TryFind(set, value, Comparer<T>.Default);

        /// <summary>
        /// Returns the elements that are in both this and other
        /// </summary>
        [Pure]
        public Set<T> Intersect(IEnumerable<T> other)
        {
            var res = new List<T>();
            foreach (var item in other)
            {
                if (Contains(item)) res.Add(item);
            }
            return new Set<T>(res);
        }

        /// <summary>
        /// Returns this - other.  Only the items in this that are not in 
        /// other will be returned.
        /// </summary>
        [Pure]
        public Set<T> Except(IEnumerable<T> other)
        {
            var self = this;
            foreach (var item in other)
            {
                if (self.Contains(item))
                {
                    self = self.Remove(item);
                }
            }
            return self;
        }

        /// <summary>
        /// Only items that are in one set or the other will be returned.
        /// If an item is in both, it is dropped.
        /// </summary>
        [Pure]
        public Set<T> SymmetricExcept(IEnumerable<T> other)
        {
            var rhs = new Set<T>(other);
            var res = new List<T>();

            foreach (var item in this)
            {
                if (!rhs.Contains(item))
                {
                    res.Add(item);
                }
            }

            foreach (var item in other)
            {
                if (!Contains(item))
                {
                    res.Add(item);
                }
            }

            return new Set<T>(res);
        }

        /// <summary>
        /// Finds the union of two sets and produces a new set with 
        /// the results
        /// </summary>
        /// <param name="other">Other set to union with</param>
        /// <returns>A set which contains all items from both sets</returns>
        [Pure]
        public Set<T> Union(IEnumerable<T> other)
        {
            var self = this;
            foreach (var item in other)
            {
                self = self.TryAdd(item);
            }
            return self;
        }

        /// <summary>
        /// Clears the set
        /// </summary>
        /// <returns>An empty set</returns>
        [Pure]
        public Set<T> Clear() =>
            new Set<T>();

        /// <summary>
        /// Get enumerator
        /// </summary>
        /// <returns>IEnumerator T</returns>
        [Pure]
        public IEnumerator<T> GetEnumerator() =>
            new SetModule.SetEnumerator<T>(set,false,0);

        /// <summary>
        /// Get enumerator
        /// </summary>
        /// <returns>IEnumerator</returns>
        [Pure]
        IEnumerator IEnumerable.GetEnumerator() =>
            new SetModule.SetEnumerator<T>(set, false, 0);

        /// <summary>
        /// Removes an item from the set (if it exists)
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <returns>New set with item removed</returns>
        [Pure]
        public Set<T> Remove(T value) =>
            new Set<T>(SetModule.Remove(set, value, Comparer<T>.Default));

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
        public S Fold<S>(S state, Func<S, T, S> folder) =>
            SetModule.Fold(set,state,folder);

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
        public S FoldBack<S>(S state, Func<S, T, S> folder) =>
            SetModule.FoldBack(set, state, folder);

        /// <summary>
        /// Maps the values of this set into a new set of values using the
        /// mapper function to tranform the source values.
        /// </summary>
        /// <typeparam name="R">Mapped element type</typeparam>
        /// <param name="mapper">Mapping function</param>
        /// <returns>Mapped Set</returns>
        [Pure]
        public Set<U> Map<U>(Func<T, U> map) =>
            new Set<U>(this.AsEnumerable().Select(map), true);

        /// <summary>
        /// Filters items from the set using the predicate.  If the predicate
        /// returns True for any item then it remains in the set, otherwise
        /// it's dropped.
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>Filtered enumerable</returns>
        [Pure]
        public Set<T> Filter(Func<T, bool> pred) =>
            new Set<T>(SetModule.Filter(set, pred));

        /// <summary>
        /// Check the existence of an item in the set using a 
        /// predicate.
        /// </summary>
        /// <remarks>Note this scans the entire set.</remarks>
        /// <param name="pred">Predicate</param>
        /// <returns>True if predicate returns true for any item</returns>
        [Pure]
        public bool Exists(Func<T, bool> pred) =>
            SetModule.Exists(set, pred);

        /// <summary>
        /// Returns True if the value is in the set
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <returns>True if the item 'value' is in the Set 'set'</returns>
        [Pure]
        public bool Contains(T value) =>
            SetModule.Contains(set, value, Comparer<T>.Default);

        /// <summary>
        /// Returns true if both sets contain the same elements
        /// </summary>
        /// <param name="other">Other distinct set to compare</param>
        /// <returns>True if the sets are equal</returns>
        [Pure]
        public bool SetEquals(IEnumerable<T> other)
        {
            var rhs = new Set<T>(other);
            if (rhs.Count() != Count) return false;
            foreach (var item in rhs)
            {
                if (!Contains(item)) return false;
            }
            return true;
        }

        /// <summary>
        /// True if the set has no elements
        /// </summary>
        [Pure]
        public bool IsEmpty => 
            Count == 0;

        /// <summary>
        /// IsReadOnly - Always true
        /// </summary>
        [Pure]
        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Syncronisation root
        /// </summary>
        [Pure]
        public object SyncRoot
        {
            get
            {
                return set;
            }
        }

        /// <summary>
        /// IsSynchronized - Always true
        /// </summary>
        [Pure]
        public bool IsSynchronized
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Returns True if 'other' is a proper subset of this set
        /// </summary>
        /// <returns>True if 'other' is a proper subset of this set</returns>
        [Pure]
        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            if (IsEmpty)
            {
                return other.Any();
            }

            var otherSet = new Set<T>(other);
            if (Count >= otherSet.Count)
            {
                return false;
            }

            int matches = 0;
            bool extraFound = false;
            foreach (T item in otherSet)
            {
                if (Contains(item))
                {
                    matches++;
                }
                else
                {
                    extraFound = true;
                }

                if (matches == Count && extraFound)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns True if 'other' is a proper superset of this set
        /// </summary>
        /// <returns>True if 'other' is a proper superset of this set</returns>
        [Pure]
        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            if (IsEmpty)
            {
                return false;
            }

            int matchCount = 0;
            foreach (T item in other)
            {
                matchCount++;
                if (!Contains(item))
                {
                    return false;
                }
            }

            return Count > matchCount;
        }

        /// <summary>
        /// Returns True if 'other' is a superset of this set
        /// </summary>
        /// <returns>True if 'other' is a superset of this set</returns>
        [Pure]
        public bool IsSubsetOf(IEnumerable<T> other)
        {
            if (IsEmpty)
            {
                return true;
            }

            var otherSet = new Set<T>(other);
            int matches = 0;
            foreach (T item in otherSet)
            {
                if (Contains(item))
                {
                    matches++;
                }
            }
            return matches == Count;
        }

        /// <summary>
        /// Returns True if 'other' is a superset of this set
        /// </summary>
        /// <returns>True if 'other' is a superset of this set</returns>
        [Pure]
        public bool IsSupersetOf(IEnumerable<T> other)
        {
            foreach (T item in other)
            {
                if (!Contains(item))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Returns True if other overlaps this set
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="setA">Set A</param>
        /// <param name="setB">Set B</param>
        /// <returns>True if other overlaps this set</returns>
        [Pure]
        public bool Overlaps(IEnumerable<T> other)
        {
            if (IsEmpty)
            {
                return false;
            }

            foreach (T item in other)
            {
                if (Contains(item))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Copy the items from the set into the specified array
        /// </summary>
        /// <param name="array">Array to copy to</param>
        /// <param name="index">Index into the array to start</param>
        public void CopyTo(T[] array, int index)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (index < 0 || index > array.Length) throw new IndexOutOfRangeException();
            if (index + Count > array.Length) throw new IndexOutOfRangeException();

            foreach (var element in this)
            {
                array[index++] = element;
            }
        }

        /// <summary>
        /// Copy the items from the set into the specified array
        /// </summary>
        /// <param name="array">Array to copy to</param>
        /// <param name="index">Index into the array to start</param>
        public void CopyTo(Array array, int index)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (index < 0 || index > array.Length) throw new IndexOutOfRangeException();
            if (index + Count > array.Length) throw new IndexOutOfRangeException();

            foreach (var element in this)
            {
                array.SetValue(element, index++);
            }
        }

        /// <summary>
        /// Add operator - performs a union of the two sets
        /// </summary>
        /// <param name="lhs">Left hand side set</param>
        /// <param name="rhs">Right hand side set</param>
        /// <returns>Unioned set</returns>
        [Pure]
        public static Set<T> operator +(Set<T> lhs, Set<T> rhs) =>
            lhs.Append(rhs);

        /// <summary>
        /// Add operator - performs a union of the two sets
        /// </summary>
        /// <param name="rhs">Right hand side set</param>
        /// <returns>Unioned set</returns>
        [Pure]
        public Set<T> Append(Set<T> rhs) =>
            Union(rhs);

        /// <summary>
        /// Subtract operator - performs a difference of the two sets
        /// </summary>
        /// <param name="lhs">Left hand side set</param>
        /// <param name="rhs">Right hand side set</param>
        /// <returns>Differenced set</returns>
        [Pure]
        public static Set<T> operator -(Set<T> lhs, Set<T> rhs) =>
            lhs.Subtract(rhs);

        /// <summary>
        /// Subtract operator - performs a difference of the two sets
        /// </summary>
        /// <param name="rhs">Right hand side set</param>
        /// <returns>Differenced set</returns>
        [Pure]
        public Set<T> Subtract(Set<T> rhs)
        {
            var self = this;
            foreach (var item in rhs)
            {
                self = self.Remove(item);
            }
            return self;
        }

        /// <summary>
        /// Multiply operator - runs through every combination of
        /// items in the two sets and performs a multiply operation on
        /// them; and then puts the result in a new distinct set.
        /// </summary>
        /// <param name="lhs">Left hand side set</param>
        /// <param name="rhs">Right hand side set</param>
        /// <returns>Product of the two sets</returns>
        [Pure]
        public static Set<T> operator *(Set<T> lhs, Set<T> rhs) =>
            lhs.Multiply(rhs);

        /// <summary>
        /// Multiply operator - runs through every combination of
        /// items in the two sets and performs a multiply operation on
        /// them; and then puts the result in a new distinct set.
        /// </summary>
        /// <param name="rhs">Right hand side set</param>
        /// <returns>Product of the two sets</returns>
        [Pure]
        public Set<T> Multiply(Set<T> rhs) =>
            new Set<T>((from x in this.AsEnumerable()
                        from y in rhs.AsEnumerable()
                        select TypeDesc.Multiply(x, y, TypeDesc<T>.Default)), true);

        /// <summary>
        /// Divide operator - runs through every combination of
        /// items in the two sets and performs a divide operation on
        /// them; and then puts the result in a new distinct set.
        /// </summary>
        /// <param name="lhs">Left hand side set</param>
        /// <param name="rhs">Right hand side set</param>
        /// <returns>Result of the division of the two sets</returns>
        [Pure]
        public static Set<T> operator /(Set<T> lhs, Set<T> rhs) =>
            lhs.Divide(rhs);

        /// <summary>
        /// Divide operator - runs through every combination of
        /// items in the two sets and performs a divide operation on
        /// them; and then puts the result in a new distinct set.
        /// </summary>
        /// <param name="rhs">Right hand side set</param>
        /// <returns>Result of the division of the two sets</returns>
        [Pure]
        public Set<T> Divide(Set<T> rhs) =>
            new Set<T>((from y in rhs.AsEnumerable()
                        from x in this.AsEnumerable()
                        select TypeDesc.Divide(x, y, TypeDesc<T>.Default)), true);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="other">Other set to test</param>
        /// <returns>True if sets are equal</returns>
        [Pure]
        public bool Equals(Set<T> other) =>
            SetEquals(other);

        [Obsolete("Remove can't be implemented because this type is immutable")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }

        [Obsolete("Add can't be implemented because this type is immutable")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        bool ISet<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        [Obsolete("UnionWith can't be implemented because this type is immutable")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void UnionWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        [Obsolete("IntersectWith can't be implemented because this type is immutable")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void IntersectWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        [Obsolete("ExceptWith can't be implemented because this type is immutable")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ExceptWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        [Obsolete("SymmetricExceptWith can't be implemented because this type is immutable")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        [Obsolete("ICollection<T>.Add can't be implemented because this type is immutable")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        [Obsolete("ICollection<T>.Clear can't be implemented because this type is immutable")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        void ICollection<T>.Clear()
        {
            throw new NotSupportedException();
        }
    }

    internal class SetItem<K>
    {
        public static readonly SetItem<K> Empty = new SetItem<K>(0, 0, default(K), null, null);

        public bool IsEmpty => Count == 0;
        public readonly int Count;
        public readonly byte Height;
        public readonly SetItem<K> Left;
        public readonly SetItem<K> Right;

        /// <summary>
        /// Ctor
        /// </summary>
        internal SetItem(byte height, int count, K key, SetItem<K> left, SetItem<K> right)
        {
            Count = count;
            Height = height;
            Key = key;
            Left = left;
            Right = right;
        }

        [Pure]
        internal int BalanceFactor =>
            Count == 0
                ? 0
                : ((int)Left.Height) - ((int)Right.Height);

        [Pure]
        public K Key
        {
            get;
            private set;
        }
    }

    internal static class SetModule
    {
        [Pure]
        public static S Fold<S, K>(SetItem<K> node, S state, Func<S, K, S> folder)
        {
            if (node.IsEmpty)
            {
                return state;
            }
            state = Fold(node.Left, state, folder);
            state = folder(state, node.Key);
            state = Fold(node.Right, state, folder);
            return state;
        }

        [Pure]
        public static S FoldBack<S, K>(SetItem<K> node, S state, Func<S, K, S> folder)
        {
            if (node.IsEmpty)
            {
                return state;
            }
            state = FoldBack(node.Right, state, folder);
            state = folder(state, node.Key);
            state = FoldBack(node.Left, state, folder);
            return state;
        }

        [Pure]
        public static bool ForAll<K>(SetItem<K> node, Func<K, bool> pred) =>
            node.IsEmpty
                ? true
                : pred(node.Key)
                    ? ForAll(node.Left, pred) && ForAll(node.Right, pred)
                    : false;

        [Pure]
        public static bool Exists<K>(SetItem<K> node, Func<K, bool> pred) =>
            node.IsEmpty
                ? false
                : pred(node.Key)
                    ? true
                    : Exists(node.Left, pred) || Exists(node.Right, pred);

        [Pure]
        public static SetItem<K> Filter<K>(SetItem<K> node, Func<K, bool> pred) =>
            node.IsEmpty
                ? node
                : pred(node.Key)
                    ? Balance(Make(node.Key, Filter(node.Left, pred), Filter(node.Right, pred)))
                    : Balance(Filter(AddTreeToRight(node.Left, node.Right), pred));

        [Pure]
        public static SetItem<K> Add<K>(SetItem<K> node, K key, Comparer<K> comparer)
        {
            if (node.IsEmpty)
            {
                return new SetItem<K>(1, 1, key, SetItem<K>.Empty, SetItem<K>.Empty);
            }
            var cmp = comparer.Compare(key, node.Key);
            if (cmp < 0)
            {
                return Balance(Make(node.Key, Add(node.Left, key, comparer), node.Right));
            }
            else if (cmp > 0)
            {
                return Balance(Make(node.Key, node.Left, Add(node.Right, key, comparer)));
            }
            else
            {
                throw new ArgumentException("An element with the same key already exists in the set");
            }
        }

        [Pure]
        public static SetItem<K> TryAdd<K>(SetItem<K> node, K key,  Comparer<K> comparer)
        {
            if (node.IsEmpty)
            {
                return new SetItem<K>(1, 1, key, SetItem<K>.Empty, SetItem<K>.Empty);
            }
            var cmp = comparer.Compare(key, node.Key);
            if (cmp < 0)
            {
                return Balance(Make(node.Key, TryAdd(node.Left, key, comparer), node.Right));
            }
            else if (cmp > 0)
            {
                return Balance(Make(node.Key, node.Left, TryAdd(node.Right, key, comparer)));
            }
            else
            {
                return node;
            }
        }

        [Pure]
        public static SetItem<K> AddOrUpdate<K>(SetItem<K> node, K key, Comparer<K> comparer)
        {
            if (node.IsEmpty)
            {
                return new SetItem<K>(1, 1, key, SetItem<K>.Empty, SetItem<K>.Empty);
            }
            var cmp = comparer.Compare(key, node.Key);
            if (cmp < 0)
            {
                return Balance(Make(node.Key, TryAdd(node.Left, key, comparer), node.Right));
            }
            else if (cmp > 0)
            {
                return Balance(Make(node.Key, node.Left, TryAdd(node.Right, key, comparer)));
            }
            else
            {
                return new SetItem<K>(node.Height, node.Count, key, node.Left, node.Right);
            }
        }

        [Pure]
        public static SetItem<K> AddTreeToRight<K>(SetItem<K> node, SetItem<K> toAdd) =>
            node.IsEmpty
                ? toAdd
                : Balance(Make(node.Key, node.Left, AddTreeToRight(node.Right, toAdd)));

        [Pure]
        public static SetItem<K> Remove<K>(SetItem<K> node, K key, Comparer<K> comparer)
        {
            if (node.IsEmpty)
            {
                return node;
            }
            var cmp = comparer.Compare(key, node.Key);
            if (cmp < 0)
            {
                return Balance(Make(node.Key, Remove(node.Left, key, comparer), node.Right));
            }
            else if (cmp > 0)
            {
                return Balance(Make(node.Key, node.Left, Remove(node.Right, key, comparer)));
            }
            else
            {
                return Balance(AddTreeToRight(node.Left, node.Right));
            }
        }

        [Pure]
        public static bool Contains<K>(SetItem<K> node, K key, Comparer<K> comparer)
        {
            if (node.IsEmpty)
            {
                return false;
            }
            var cmp = comparer.Compare(key, node.Key);
            if (cmp < 0)
            {
                return Contains(node.Left, key, comparer);
            }
            else if (cmp > 0)
            {
                return Contains(node.Right, key, comparer);
            }
            else
            {
                return true;
            }
        }

        [Pure]
        public static K Find<K>(SetItem<K> node, K key, Comparer<K> comparer)
        {
            if (node.IsEmpty)
            {
                throw new ArgumentException("Key not found in set");
            }
            var cmp = comparer.Compare(key, node.Key);
            if (cmp < 0)
            {
                return Find(node.Left, key, comparer);
            }
            else if (cmp > 0)
            {
                return Find(node.Right, key, comparer);
            }
            else
            {
                return node.Key;
            }
        }

        /// <summary>
        /// TODO: I suspect this is suboptimal, it would be better with a custom Enumerator 
        /// that maintains a stack of nodes to retrace.
        /// </summary>
        [Pure]
        public static IEnumerable<K> FindRange<K>(SetItem<K> node, K a, K b, Comparer<K> comparer)
        {
            if (node.IsEmpty)
            {
                yield break;
            }
            if (comparer.Compare(node.Key, a) < 0)
            {
                foreach (var item in FindRange(node.Right, a, b, comparer))
                {
                    yield return item;
                }
            }
            else if (comparer.Compare(node.Key, b) > 0)
            {
                foreach (var item in FindRange(node.Left, a, b, comparer))
                {
                    yield return item;
                }
            }
            else
            {
                foreach (var item in FindRange(node.Left, a, b, comparer))
                {
                    yield return item;
                }
                yield return node.Key;
                foreach (var item in FindRange(node.Right, a, b, comparer))
                {
                    yield return item;
                }
            }
        }

        [Pure]
        public static Option<K> TryFind<K>(SetItem<K> node, K key, Comparer<K> comparer)
        {
            if (node.IsEmpty)
            {
                return None;
            }
            var cmp = comparer.Compare(key, node.Key);
            if (cmp < 0)
            {
                return TryFind(node.Left, key, comparer);
            }
            else if (cmp > 0)
            {
                return TryFind(node.Right, key, comparer);
            }
            else
            {
                return Some(node.Key);
            }
        }

        [Pure]
        public static SetItem<K> Skip<K>(SetItem<K> node, int amount)
        {
            if (amount == 0 || node.IsEmpty)
            {
                return node;
            }
            if (amount >= node.Count)
            {
                return SetItem<K>.Empty;
            }
            if (!node.Left.IsEmpty && node.Left.Count == amount)
            {
                return Balance(Make(node.Key, SetItem<K>.Empty, node.Right));
            }
            if (!node.Left.IsEmpty && node.Left.Count == amount - 1)
            {
                return node.Right;
            }
            if (node.Left.IsEmpty)
            {
                return Skip(node.Right, amount - 1);
            }

            var newleft = Skip(node.Left, amount);
            var remaining = amount - node.Left.Count - newleft.Count;
            if (remaining > 0)
            {
                return Skip(Balance(Make(node.Key, newleft, node.Right)), remaining);
            }
            else
            {
                return Balance(Make(node.Key, newleft, node.Right));
            }
        }

        [Pure]
        public static SetItem<K> Make<K>(K k, SetItem<K> l, SetItem<K> r) =>
            new SetItem<K>((byte)(1 + Math.Max(l.Height, r.Height)), l.Count + r.Count + 1, k, l, r);

        [Pure]
        public static SetItem<K> Balance<K>(SetItem<K> node) =>
            node.BalanceFactor >= 2
                ? node.Left.BalanceFactor >= 1
                    ? RotRight(node)
                    : DblRotRight(node)
                : node.BalanceFactor <= -2
                    ? node.Left.BalanceFactor <= -1
                        ? RotLeft(node)
                        : DblRotLeft(node)
                    : node;

        [Pure]
        public static SetItem<K> RotRight<K>(SetItem<K> node) =>
            node.IsEmpty || node.Left.IsEmpty
                ? node
                : Make(node.Left.Key, node.Left.Left, Make(node.Key, node.Left.Right, node.Right));

        [Pure]
        public static SetItem<K> RotLeft<K>(SetItem<K> node) =>
            node.IsEmpty || node.Right.IsEmpty
                ? node
                : Make(node.Right.Key, Make(node.Key, node.Left, node.Right.Left), node.Right.Right);

        [Pure]
        public static SetItem<K> DblRotRight<K>(SetItem<K> node) =>
            node.IsEmpty
                ? node
                : RotRight(Make(node.Key, RotLeft(node.Left), node.Right));

        [Pure]
        public static SetItem<K> DblRotLeft<K>(SetItem<K> node) =>
            node.IsEmpty
                ? node
                : RotLeft(Make(node.Key, node.Left, RotRight(node.Right)));

        public class SetEnumerator<K> : IEnumerator<K>
        {
            static ObjectPool<Stack<SetItem<K>>> pool = new ObjectPool<Stack<SetItem<K>>>(32, () => new Stack<SetItem<K>>(32));

            Stack<SetItem<K>> stack;
            readonly SetItem<K> map;
            int left;
            readonly bool rev;
            readonly int start;

            public SetEnumerator(SetItem<K> root, bool rev, int start)
            {
                this.rev = rev;
                this.start = start;
                map = root;
                stack = pool.GetItem();
                Reset();
            }

            private SetItem<K> NodeCurrent
            {
                get;
                set;
            }

            public K Current => NodeCurrent.Key;
            object IEnumerator.Current => NodeCurrent.Key;

            public void Dispose()
            {
                if (stack != null)
                {
                    pool.Release(stack);
                    stack = null;
                }
            }

            private SetItem<K> Next(SetItem<K> node) =>
                rev ? node.Left : node.Right;

            private SetItem<K> Prev(SetItem<K> node) =>
                rev ? node.Right : node.Left;

            private void Push(SetItem<K> node)
            {
                while (!node.IsEmpty)
                {
                    stack.Push(node);
                    node = Prev(node);
                }
            }

            public bool MoveNext()
            {
                if (left > 0 && stack.Count > 0)
                {
                    NodeCurrent = stack.Pop();
                    Push(Next(NodeCurrent));
                    left--;
                    return true;
                }

                NodeCurrent = null;
                return false;
            }

            public void Reset()
            {
                var skip = rev ? map.Count - start - 1 : start;

                stack.Clear();
                NodeCurrent = map;
                left = map.Count;

                while (!NodeCurrent.IsEmpty && skip != Prev(NodeCurrent).Count)
                {
                    if (skip < Prev(NodeCurrent).Count)
                    {
                        stack.Push(NodeCurrent);
                        NodeCurrent = Prev(NodeCurrent);
                    }
                    else
                    {
                        skip -= Prev(NodeCurrent).Count + 1;
                        NodeCurrent = Next(NodeCurrent);
                    }
                }

                if (!NodeCurrent.IsEmpty)
                {
                    stack.Push(NodeCurrent);
                }
            }
        }
    }
}

using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using LanguageExt;
using static LanguageExt.Prelude;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;
using LanguageExt.ClassInstances;

namespace LanguageExt
{
    /// <summary>
    /// Immutable set
    /// AVL tree implementation
    /// AVL tree is a self-balancing binary search tree. 
    /// [wikipedia.org/wiki/AVL_tree](http://en.wikipedia.org/wiki/AVL_tree)
    /// </summary>
    /// <typeparam name="A">List item type</typeparam>
    [Serializable]
    internal class SetInternal<OrdA, A> :
        IEnumerable<A>,
        IEquatable<SetInternal<OrdA, A>>
        where OrdA : struct, Ord<A>
    {
        public static readonly SetInternal<OrdA, A> Empty = new SetInternal<OrdA, A>();
        readonly SetItem<A> set;
        int hashCode;

        /// <summary>
        /// Default ctor
        /// </summary>
        internal SetInternal()
        {
            set = SetItem<A>.Empty;
        }

        /// <summary>
        /// Ctor that takes a root element
        /// </summary>
        /// <param name="root"></param>
        internal SetInternal(SetItem<A> root)
        {
            set = root;
        }

        /// <summary>
        /// Ctor from an enumerable 
        /// </summary>
        public SetInternal(IEnumerable<A> items) : this(items, SetModuleM.AddOpt.TryAdd)
        {
        }

        public override int GetHashCode() =>
            hashCode == 0
                ? hashCode = FNV32.Hash<OrdA, A>(this.AsEnumerable())
                : hashCode;

        public IEnumerable<A> AsEnumerable()
        {
            IEnumerable<A> Yield()
            {
                var iter = GetEnumerator();
                while (iter.MoveNext())
                {
                    yield return iter.Current;
                }
            }
            return Yield();
        }

        public IEnumerable<A> Skip(int amount)
        {
            var iter = new SetModule.SetEnumerator<A>(set, false, amount);
            while (iter.MoveNext())
            {
                yield return iter.Current;
            }
        }

        /// <summary>
        /// Ctor that takes an initial (distinct) set of items
        /// </summary>
        /// <param name="items"></param>
        internal SetInternal(IEnumerable<A> items, SetModuleM.AddOpt option)
        {
            set = SetItem<A>.Empty;

            foreach (var item in items)
            {
                set = SetModuleM.Add<OrdA, A>(set, item, option);
            }
        }

        /// <summary>
        /// Number of items in the set
        /// </summary>
        [Pure]
        public int Count =>
            set.Count;

        [Pure]
        public Option<A> Min => 
            set.IsEmpty
                ? None
                : SetModule.Min(set);

        [Pure]
        public Option<A> Max =>
            set.IsEmpty
                ? None
                : SetModule.Max(set);

        /// <summary>
        /// Add an item to the set
        /// </summary>
        /// <param name="value">Value to add to the set</param>
        /// <returns>New set with the item added</returns>
        [Pure]
        public SetInternal<OrdA, A> Add(A value) =>
            new SetInternal<OrdA, A>(SetModule.Add<OrdA, A>(set,value));

        /// <summary>
        /// Attempt to add an item to the set.  If an item already
        /// exists then return the Set as-is.
        /// </summary>
        /// <param name="value">Value to add to the set</param>
        /// <returns>New set with the item maybe added</returns>
        [Pure]
        public SetInternal<OrdA, A> TryAdd(A value) =>
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
        public SetInternal<OrdA, A> AddOrUpdate(A value) =>
            new SetInternal<OrdA, A>(SetModule.AddOrUpdate<OrdA, A>(set, value));

        [Pure]
        public SetInternal<OrdA, A> AddRange(IEnumerable<A> xs)
        {
            if(xs == null)
            {
                return this;
            }

            if(Count == 0)
            {
                return new SetInternal<OrdA, A>(xs, SetModuleM.AddOpt.ThrowOnDuplicate);
            }

            var set = this;
            foreach(var x in xs)
            {
                set = set.Add(x);
            }
            return set;
        }

        [Pure]
        public SetInternal<OrdA, A> TryAddRange(IEnumerable<A> xs)
        {
            if (xs == null)
            {
                return this;
            }

            if (Count == 0)
            {
                return new SetInternal<OrdA, A>(xs, SetModuleM.AddOpt.TryAdd);
            }

            var set = this;
            foreach (var x in xs)
            {
                set = set.TryAdd(x);
            }
            return set;
        }

        [Pure]
        public SetInternal<OrdA, A> AddOrUpdateRange(IEnumerable<A> xs)
        {
            if (xs == null)
            {
                return this;
            }

            if (Count == 0)
            {
                return new SetInternal<OrdA, A>(xs, SetModuleM.AddOpt.TryUpdate);
            }

            var set = this;
            foreach (var x in xs)
            {
                set = set.AddOrUpdate(x);
            }
            return set;
        }

        /// <summary>
        /// Get the number of elements in the set
        /// </summary>
        /// <returns>Number of elements</returns>
        [Pure]
        public int Length() =>
            Count;

        /// <summary>
        /// Attempts to find an item in the set.  
        /// </summary>
        /// <param name="value">Value to find</param>
        /// <returns>Some(T) if found, None otherwise</returns>
        [Pure]
        public Option<A> Find(A value) =>
            SetModule.TryFind<OrdA, A>(set, value);

        /// <summary>
        /// Retrieve the value from predecessor item to specified key
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>Found key</returns>
        [Pure]
        public Option<A> FindPredecessor(A key) => SetModule.TryFindPredecessor<OrdA, A>(set, key);

        /// <summary>
        /// Retrieve the value from exact key, or if not found, the predecessor item 
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>Found key</returns>
        [Pure]
        public Option<A> FindOrPredecessor(A key) => SetModule.TryFindOrPredecessor<OrdA, A>(set, key);

        /// <summary>
        /// Retrieve the value from next item to specified key
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>Found key</returns>
        [Pure]
        public Option<A> FindSuccessor(A key) => SetModule.TryFindSuccessor<OrdA, A>(set, key);

        /// <summary>
        /// Retrieve the value from exact key, or if not found, the next item 
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>Found key</returns>
        [Pure]
        public Option<A> FindOrSuccessor(A key) => SetModule.TryFindOrSuccessor<OrdA, A>(set, key);

        /// <summary>
        /// Retrieve a range of values 
        /// </summary>
        /// <param name="keyFrom">Range start (inclusive)</param>
        /// <param name="keyTo">Range to (inclusive)</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keyFrom or keyTo are null</exception>
        /// <returns>Range of values</returns>
        [Pure]
        public IEnumerable<A> FindRange(A keyFrom, A keyTo)
        {
            if (isnull(keyFrom)) throw new ArgumentNullException(nameof(keyFrom));
            if (isnull(keyTo)) throw new ArgumentNullException(nameof(keyTo));
            return default(OrdA).Compare(keyFrom, keyTo) > 0
                ? SetModule.FindRange<OrdA, A>(set, keyTo, keyFrom)
                : SetModule.FindRange<OrdA, A>(set, keyFrom, keyTo);
        }


        /// <summary>
        /// Returns the elements that are in both this and other
        /// </summary>
        [Pure]
        public SetInternal<OrdA, A> Intersect(IEnumerable<A> other)
        {
            var root = SetItem<A>.Empty;
            foreach (var item in other)
            {
                if (Contains(item))
                {
                    root = SetModuleM.Add<OrdA, A>(root, item, SetModuleM.AddOpt.TryAdd);
                }
            }
            return new SetInternal<OrdA, A>(root);
        }

        /// <summary>
        /// Returns this - other.  Only the items in this that are not in 
        /// other will be returned.
        /// </summary>
        [Pure]
        public SetInternal<OrdA, A> Except(SetInternal<OrdA, A> rhs)
        {
            var root = SetItem<A>.Empty;
            foreach (var item in this)
            {
                if (!rhs.Contains(item))
                {
                    root = SetModuleM.Add<OrdA, A>(root, item, SetModuleM.AddOpt.TryAdd);
                }
            }
            return new SetInternal<OrdA, A>(root);
        }

        /// <summary>
        /// Returns this - other.  Only the items in this that are not in 
        /// other will be returned.
        /// </summary>
        [Pure]
        public SetInternal<OrdA, A> Except(IEnumerable<A> other) =>
            Except(new SetInternal<OrdA, A>(other));

        /// <summary>
        /// Only items that are in one set or the other will be returned.
        /// If an item is in both, it is dropped.
        /// </summary>
        [Pure]
        public SetInternal<OrdA, A> SymmetricExcept(SetInternal<OrdA, A> rhs)
        {
            var root = SetItem<A>.Empty;

            foreach (var item in this)
            {
                if (!rhs.Contains(item))
                {
                    root = SetModuleM.Add<OrdA, A>(root, item, SetModuleM.AddOpt.TryAdd);
                }
            }

            foreach (var item in rhs)
            {
                if (!Contains(item))
                {
                    root = SetModuleM.Add<OrdA, A>(root, item, SetModuleM.AddOpt.TryAdd);
                }
            }

            return new SetInternal<OrdA, A>(root);
        }

        /// <summary>
        /// Only items that are in one set or the other will be returned.
        /// If an item is in both, it is dropped.
        /// </summary>
        [Pure]
        public SetInternal<OrdA, A> SymmetricExcept(IEnumerable<A> other) =>
            SymmetricExcept(new SetInternal<OrdA, A>(other));

        /// <summary>
        /// Finds the union of two sets and produces a new set with 
        /// the results
        /// </summary>
        /// <param name="other">Other set to union with</param>
        /// <returns>A set which contains all items from both sets</returns>
        [Pure]
        public SetInternal<OrdA, A> Union(IEnumerable<A> other)
        {
            if(other == null || !other.Any()) return this;

            var root = SetItem<A>.Empty;

            foreach(var item in this)
            {
                root = SetModuleM.Add<OrdA, A>(root, item, SetModuleM.AddOpt.TryAdd);
            }

            foreach (var item in other)
            {
                root = SetModuleM.Add<OrdA, A>(root, item, SetModuleM.AddOpt.TryAdd);
            }

            return new SetInternal<OrdA, A>(root);
        }

        /// <summary>
        /// Clears the set
        /// </summary>
        /// <returns>An empty set</returns>
        [Pure]
        public SetInternal<OrdA, A> Clear() =>
            Empty;

        /// <summary>
        /// Get enumerator
        /// </summary>
        /// <returns>IEnumerator T</returns>
        [Pure]
        public IEnumerator<A> GetEnumerator() =>
            new SetModule.SetEnumerator<A>(set, false, 0);

        /// <summary>
        /// Removes an item from the set (if it exists)
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <returns>New set with item removed</returns>
        [Pure]
        public SetInternal<OrdA, A> Remove(A value) =>
            new SetInternal<OrdA, A>(SetModule.Remove<OrdA, A>(set, value));

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
        public S FoldBack<S>(S state, Func<S, A, S> folder) =>
            SetModule.FoldBack(set, state, folder);

        /// <summary>
        /// Maps the values of this set into a new set of values using the
        /// mapper function to tranform the source values.
        /// </summary>
        /// <typeparam name="R">Mapped element type</typeparam>
        /// <param name="f">Mapping function</param>
        /// <returns>Mapped Set</returns>
        [Pure]
        public SetInternal<OrdB, B> Map<OrdB, B>(Func<A, B> f) where OrdB : struct, Ord<B> =>
            new SetInternal<OrdB, B>(SetModule.Map(set, f));

        /// <summary>
        /// Maps the values of this set into a new set of values using the
        /// mapper function to tranform the source values.
        /// </summary>
        /// <typeparam name="R">Mapped element type</typeparam>
        /// <param name="f">Mapping function</param>
        /// <returns>Mapped Set</returns>
        [Pure]
        public SetInternal<OrdA, A> Map(Func<A, A> f) =>
            new SetInternal<OrdA, A>(SetModule.Map(set, f));

        /// <summary>
        /// Filters items from the set using the predicate.  If the predicate
        /// returns True for any item then it remains in the set, otherwise
        /// it's dropped.
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>Filtered enumerable</returns>
        [Pure]
        public SetInternal<OrdA, A> Filter(Func<A, bool> pred) =>
            new SetInternal<OrdA, A>(AsEnumerable().Filter(pred), SetModuleM.AddOpt.TryAdd);

        /// <summary>
        /// Check the existence of an item in the set using a 
        /// predicate.
        /// </summary>
        /// <remarks>Note this scans the entire set.</remarks>
        /// <param name="pred">Predicate</param>
        /// <returns>True if predicate returns true for any item</returns>
        [Pure]
        public bool Exists(Func<A, bool> pred) =>
            SetModule.Exists(set, pred);

        /// <summary>
        /// Returns True if the value is in the set
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <returns>True if the item 'value' is in the Set 'set'</returns>
        [Pure]
        public bool Contains(A value) =>
            SetModule.Contains<OrdA, A>(set, value);

        /// <summary>
        /// Returns true if both sets contain the same elements
        /// </summary>
        /// <param name="other">Other distinct set to compare</param>
        /// <returns>True if the sets are equal</returns>
        [Pure]
        public bool SetEquals(IEnumerable<A> other)
        {
            var rhs = new SetInternal<OrdA, A>(other);
            if (rhs.Count != Count) return false;
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
        /// Returns True if 'other' is a proper subset of this set
        /// </summary>
        /// <returns>True if 'other' is a proper subset of this set</returns>
        [Pure]
        public bool IsProperSubsetOf(IEnumerable<A> other)
        {
            if (IsEmpty)
            {
                return other.Any();
            }

            var otherSet = new Set<A>(other);
            if (Count >= otherSet.Count)
            {
                return false;
            }

            int matches = 0;
            bool extraFound = false;
            foreach (A item in otherSet)
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
        public bool IsProperSupersetOf(IEnumerable<A> other)
        {
            if (IsEmpty)
            {
                return false;
            }

            int matchCount = 0;
            foreach (A item in other)
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
        public bool IsSubsetOf(IEnumerable<A> other)
        {
            if (IsEmpty)
            {
                return true;
            }

            var otherSet = new SetInternal<OrdA, A>(other);
            int matches = 0;
            foreach (A item in otherSet)
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
        public bool IsSupersetOf(IEnumerable<A> other)
        {
            foreach (A item in other)
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
        public bool Overlaps(IEnumerable<A> other)
        {
            if (IsEmpty)
            {
                return false;
            }

            foreach (A item in other)
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
        public void CopyTo(A[] array, int index)
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
        public void CopyTo(System.Array array, int index)
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
        /// Add operator + performs a union of the two sets
        /// </summary>
        /// <param name="lhs">Left hand side set</param>
        /// <param name="rhs">Right hand side set</param>
        /// <returns>Unioned set</returns>
        [Pure]
        public static SetInternal<OrdA, A> operator +(SetInternal<OrdA, A> lhs, SetInternal<OrdA, A> rhs) =>
            lhs.Append(rhs);

        /// <summary>
        /// Append performs a union of the two sets
        /// </summary>
        /// <param name="rhs">Right hand side set</param>
        /// <returns>Unioned set</returns>
        [Pure]
        public SetInternal<OrdA, A> Append(SetInternal<OrdA, A> rhs) =>
            Union(rhs.AsEnumerable());

        /// <summary>
        /// Subtract operator - performs a subtract of the two sets
        /// </summary>
        /// <param name="lhs">Left hand side set</param>
        /// <param name="rhs">Right hand side set</param>
        /// <returns>Subtractd set</returns>
        [Pure]
        public static SetInternal<OrdA, A> operator -(SetInternal<OrdA, A> lhs, SetInternal<OrdA, A> rhs) =>
            lhs.Subtract(rhs);

        /// <summary>
        /// Subtract operator - performs a subtract of the two sets
        /// </summary>
        /// <param name="rhs">Right hand side set</param>
        /// <returns>Subtractd set</returns>
        [Pure]
        public SetInternal<OrdA, A> Subtract(SetInternal<OrdA, A> rhs)
        {
            if (Count == 0) return Empty;
            if (rhs.Count == 0) return this;

            if (rhs.Count < Count)
            {
                var self = this;
                foreach (var item in rhs)
                {
                    self = self.Remove(item);
                }
                return self;
            }
            else
            {
                var root = SetItem<A>.Empty;
                foreach (var item in this)
                {
                    if (!rhs.Contains(item))
                    {
                        root = SetModuleM.Add<OrdA, A>(root, item, SetModuleM.AddOpt.TryAdd);
                    }
                }
                return new SetInternal<OrdA, A>(root);
            }
        }

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="other">Other set to test</param>
        /// <returns>True if sets are equal</returns>
        [Pure]
        public bool Equals(SetInternal<OrdA, A> other) =>
            SetEquals(other.AsEnumerable());

        [Pure]
        public int CompareTo(SetInternal<OrdA, A> other)
        {
            var cmp = Count.CompareTo(other.Count);
            if (cmp != 0) return cmp;
            var iterA = GetEnumerator();
            var iterB = other.GetEnumerator();
            while (iterA.MoveNext() && iterB.MoveNext())
            {
                cmp = default(OrdA).Compare(iterA.Current, iterB.Current);
                if (cmp != 0) return cmp;
            }
            return 0;
        }

        [Pure]
        public int CompareTo<OrdAlt>(SetInternal<OrdA, A> other) where OrdAlt : struct, Ord<A>
        {
            var cmp = Count.CompareTo(other.Count);
            if (cmp != 0) return cmp;
            var iterA = GetEnumerator();
            var iterB = other.GetEnumerator();
            while (iterA.MoveNext() && iterB.MoveNext())
            {
                cmp = default(OrdAlt).Compare(iterA.Current, iterB.Current);
                if (cmp != 0) return cmp;
            }
            return 0;
        }

        IEnumerator IEnumerable.GetEnumerator() =>
            new SetModule.SetEnumerator<A>(set, false, 0);
    }

    internal class SetItem<K>
    {
        public static readonly SetItem<K> Empty = new SetItem<K>(0, 0, default(K), null, null);

        public bool IsEmpty => Count == 0;
        public int Count;
        public byte Height;
        public SetItem<K> Left;
        public SetItem<K> Right;

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
                : ((int)Right.Height) - ((int)Left.Height);

        [Pure]
        public K Key
        {
            get;
            internal set;
        }
    }

    internal static class SetModuleM
    {
        public enum AddOpt
        {
            ThrowOnDuplicate,
            TryAdd,
            TryUpdate
        }

        public static SetItem<K> Add<OrdK, K>(SetItem<K> node, K key, AddOpt option)
            where OrdK : struct, Ord<K>
        {
            if (node.IsEmpty)
            {
                return new SetItem<K>(1, 1, key, SetItem<K>.Empty, SetItem<K>.Empty);
            }
            var cmp = default(OrdK).Compare(key, node.Key);
            if (cmp < 0)
            {
                node.Left = Add<OrdK, K>(node.Left, key, option);
                return Balance(node);
            }
            else if (cmp > 0)
            {
                node.Right = Add<OrdK, K>(node.Right, key, option);
                return Balance(node);
            }
            else if (option == AddOpt.TryAdd)
            {
                // Already exists, but we don't care
                return node;
            }
            else if (option == AddOpt.TryUpdate)
            {
                // Already exists, and we want to update the content
                node.Key = key;
                return node;
            }
            else
            {
                throw new ArgumentException("An element with the same key already exists in the Map");
            }
        }

        public static SetItem<K> Balance<K>(SetItem<K> node)
        {
            node.Height = (byte)(1 + Math.Max(node.Left.Height, node.Right.Height));
            node.Count = 1 + node.Left.Count + node.Right.Count;

            return node.BalanceFactor >= 2
                ? node.Right.BalanceFactor < 0
                    ? DblRotLeft(node)
                    : RotLeft(node)
                : node.BalanceFactor <= -2
                    ? node.Left.BalanceFactor > 0
                        ? DblRotRight(node)
                        : RotRight(node)
                    : node;
        }

        public static SetItem<K> DblRotRight<K>(SetItem<K> node)
        {
            node.Left = RotLeft(node.Left);
            return RotRight(node);
        }

        public static SetItem<K> DblRotLeft<K>(SetItem<K> node)
        {
            node.Right = RotRight(node.Right);
            return RotLeft(node);
        }

        public static SetItem<K> RotRight<K>(SetItem<K> node)
        {
            if (node.IsEmpty || node.Left.IsEmpty) return node;

            var y = node;
            var x = y.Left;
            var t2 = x.Right;
            x.Right = y;
            y.Left = t2;
            y.Height = (byte)(1 + Math.Max(y.Left.Height, y.Right.Height));
            x.Height = (byte)(1 + Math.Max(x.Left.Height, x.Right.Height));
            y.Count = 1 + y.Left.Count + y.Right.Count;
            x.Count = 1 + x.Left.Count + x.Right.Count;

            return x;
        }

        public static SetItem<K> RotLeft<K>(SetItem<K> node)
        {
            if (node.IsEmpty || node.Right.IsEmpty) return node;

            var x = node;
            var y = x.Right;
            var t2 = y.Left;
            y.Left = x;
            x.Right = t2;
            x.Height = (byte)(1 + Math.Max(x.Left.Height, x.Right.Height));
            y.Height = (byte)(1 + Math.Max(y.Left.Height, y.Right.Height));
            x.Count = 1 + x.Left.Count + x.Right.Count;
            y.Count = 1 + y.Left.Count + y.Right.Count;

            return y;
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
        public static SetItem<K> Add<OrdK, K>(SetItem<K> node, K key) where OrdK : struct, Ord<K>
        {
            if (node.IsEmpty)
            {
                return new SetItem<K>(1, 1, key, SetItem<K>.Empty, SetItem<K>.Empty);
            }
            var cmp = default(OrdK).Compare(key, node.Key);
            if (cmp < 0)
            {
                return Balance(Make(node.Key, Add<OrdK, K>(node.Left, key), node.Right));
            }
            else if (cmp > 0)
            {
                return Balance(Make(node.Key, node.Left, Add<OrdK, K>(node.Right, key)));
            }
            else
            {
                throw new ArgumentException("An element with the same key already exists in the set");
            }
        }

        [Pure]
        public static SetItem<K> TryAdd<OrdK, K>(SetItem<K> node, K key) where OrdK : struct, Ord<K>
        {
            if (node.IsEmpty)
            {
                return new SetItem<K>(1, 1, key, SetItem<K>.Empty, SetItem<K>.Empty);
            }
            var cmp = default(OrdK).Compare(key, node.Key);
            if (cmp < 0)
            {
                return Balance(Make(node.Key, TryAdd<OrdK, K>(node.Left, key), node.Right));
            }
            else if (cmp > 0)
            {
                return Balance(Make(node.Key, node.Left, TryAdd<OrdK, K>(node.Right, key)));
            }
            else
            {
                return node;
            }
        }

        [Pure]
        public static SetItem<K> AddOrUpdate<OrdK, K>(SetItem<K> node, K key) where OrdK : struct, Ord<K>
        {
            if (node.IsEmpty)
            {
                return new SetItem<K>(1, 1, key, SetItem<K>.Empty, SetItem<K>.Empty);
            }
            var cmp = default(OrdK).Compare(key, node.Key);
            if (cmp < 0)
            {
                return Balance(Make(node.Key, TryAdd<OrdK, K>(node.Left, key), node.Right));
            }
            else if (cmp > 0)
            {
                return Balance(Make(node.Key, node.Left, TryAdd<OrdK, K>(node.Right, key)));
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
        public static SetItem<K> Remove<OrdK, K>(SetItem<K> node, K key) where OrdK : struct, Ord<K>
        {
            if (node.IsEmpty)
            {
                return node;
            }
            var cmp = default(OrdK).Compare(key, node.Key);
            if (cmp < 0)
            {
                return Balance(Make(node.Key, Remove<OrdK, K>(node.Left, key), node.Right));
            }
            else if (cmp > 0)
            {
                return Balance(Make(node.Key, node.Left, Remove<OrdK, K>(node.Right, key)));
            }
            else
            {
                return Balance(AddTreeToRight(node.Left, node.Right));
            }
        }

        [Pure]
        public static bool Contains<OrdK, K>(SetItem<K> node, K key) where OrdK : struct, Ord<K>
        {
            if (node.IsEmpty)
            {
                return false;
            }
            var cmp = default(OrdK).Compare(key, node.Key);
            if (cmp < 0)
            {
                return Contains<OrdK, K>(node.Left, key);
            }
            else if (cmp > 0)
            {
                return Contains<OrdK, K>(node.Right, key);
            }
            else
            {
                return true;
            }
        }

        [Pure]
        public static K Find<OrdK, K>(SetItem<K> node, K key) where OrdK : struct, Ord<K>
        {
            if (node.IsEmpty)
            {
                throw new ArgumentException("Key not found in set");
            }
            var cmp = default(OrdK).Compare(key, node.Key);
            if (cmp < 0)
            {
                return Find<OrdK, K>(node.Left, key);
            }
            else if (cmp > 0)
            {
                return Find<OrdK, K>(node.Right, key);
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
        public static IEnumerable<K> FindRange<OrdK, K>(SetItem<K> node, K a, K b) where OrdK : struct, Ord<K>
        {
            if (node.IsEmpty)
            {
                yield break;
            }
            if (default(OrdK).Compare(node.Key, a) < 0)
            {
                foreach (var item in FindRange<OrdK, K>(node.Right, a, b))
                {
                    yield return item;
                }
            }
            else if (default(OrdK).Compare(node.Key, b) > 0)
            {
                foreach (var item in FindRange<OrdK, K>(node.Left, a, b))
                {
                    yield return item;
                }
            }
            else
            {
                foreach (var item in FindRange<OrdK, K>(node.Left, a, b))
                {
                    yield return item;
                }
                yield return node.Key;
                foreach (var item in FindRange<OrdK, K>(node.Right, a, b))
                {
                    yield return item;
                }
            }
        }

        [Pure]
        public static Option<K> TryFind<OrdK, K>(SetItem<K> node, K key) where OrdK : struct, Ord<K>
        {
            if (node.IsEmpty)
            {
                return None;
            }
            var cmp = default(OrdK).Compare(key, node.Key);
            if (cmp < 0)
            {
                return TryFind<OrdK, K>(node.Left, key);
            }
            else if (cmp > 0)
            {
                return TryFind<OrdK, K>(node.Right, key);
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
                ? node.Right.BalanceFactor < 0
                    ? DblRotLeft(node)
                    : RotLeft(node)
                : node.BalanceFactor <= -2
                    ? node.Left.BalanceFactor > 0
                        ? DblRotRight(node)
                        : RotRight(node)
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

        [Pure]
        public static SetItem<B> Map<A, B>(SetItem<A> node, Func<A, B> f) =>
            node.IsEmpty
                ? SetItem<B>.Empty
                : new SetItem<B>(node.Height, node.Count, f(node.Key), Map(node.Left, f), Map(node.Right, f));

        internal static Option<A> Max<A>(SetItem<A> node) =>
            node.Right.IsEmpty
                ? node.Key
                : Max(node.Right);

        internal static Option<A> Min<A>(SetItem<A> node) =>
            node.Left.IsEmpty
                ? node.Key
                : Min(node.Left);

        internal static Option<A> TryFindPredecessor<OrdA, A>(SetItem<A> root, A key) where OrdA : struct, Ord<A>
        {
            Option<A> predecessor = None;
            var current = root;

            if (root.IsEmpty)
            {
                return None;
            }

            do
            {
                var cmp = default(OrdA).Compare(key, current.Key);
                if (cmp < 0)
                {
                    current = current.Left;
                }
                else if (cmp > 0)
                {
                    predecessor = current.Key;
                    current = current.Right;
                }
                else
                {
                    break;
                }
            }
            while (!current.IsEmpty);

            if(!current.IsEmpty && !current.Left.IsEmpty)
            {
                predecessor = Max(current.Left);
            }

            return predecessor;
        }

        internal static Option<A> TryFindOrPredecessor<OrdA, A>(SetItem<A> root, A key) where OrdA : struct, Ord<A>
        {
            Option<A> predecessor = None;
            var current = root;

            if (root.IsEmpty)
            {
                return None;
            }

            do
            {
                var cmp = default(OrdA).Compare(key, current.Key);
                if (cmp < 0)
                {
                    current = current.Left;
                }
                else if (cmp > 0)
                {
                    predecessor = current.Key;
                    current = current.Right;
                }
                else
                {
                    return current.Key;
                }
            }
            while (!current.IsEmpty);

            if (!current.IsEmpty && !current.Left.IsEmpty)
            {
                predecessor = Max(current.Left);
            }

            return predecessor;
        }

        internal static Option<A> TryFindSuccessor<OrdA, A>(SetItem<A> root, A key) where OrdA : struct, Ord<A>
        {
            Option<A> successor = None;
            var current = root;

            if (root.IsEmpty)
            {
                return None;
            }

            do
            {
                var cmp = default(OrdA).Compare(key, current.Key);
                if (cmp < 0)
                {
                    successor = current.Key;
                    current = current.Left;
                }
                else if (cmp > 0)
                {
                    current = current.Right;
                }
                else
                {
                    break;
                }
            }
            while (!current.IsEmpty);

            if (!current.IsEmpty && !current.Right.IsEmpty)
            {
                successor = Min(current.Right);
            }

            return successor;
        }

        internal static Option<A> TryFindOrSuccessor<OrdA, A>(SetItem<A> root, A key) where OrdA : struct, Ord<A>
        {
            Option<A> successor = None;
            var current = root;

            if (root.IsEmpty)
            {
                return None;
            }

            do
            {
                var cmp = default(OrdA).Compare(key, current.Key);
                if (cmp < 0)
                {
                    successor = current.Key;
                    current = current.Left;
                }
                else if (cmp > 0)
                {
                    current = current.Right;
                }
                else
                {
                    return current.Key;
                }
            }
            while (!current.IsEmpty);

            if (!current.IsEmpty && !current.Right.IsEmpty)
            {
                successor = Min(current.Right);
            }

            return successor;
        }

        public class SetEnumerator<K> : IEnumerator<K>
        {
            internal struct NewStack : New<SetItem<K>[]>
            {
                public SetItem<K>[] New() =>
                    new SetItem<K>[32];
            }

            int stackDepth;
            SetItem<K>[] stack;
            readonly SetItem<K> map;
            int left;
            readonly bool rev;
            readonly int start;

            public SetEnumerator(SetItem<K> root, bool rev, int start)
            {
                this.rev = rev;
                this.start = start;
                map = root;
                stack = Pool<NewStack, SetItem<K>[]>.Pop();
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
                    Pool<NewStack, SetItem<K>[]>.Push(stack);
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
                    stack[stackDepth] = node;
                    stackDepth++;
                    node = Prev(node);
                }
            }

            public bool MoveNext()
            {
                if (left > 0 && stackDepth > 0)
                {
                    stackDepth--;
                    NodeCurrent = stack[stackDepth];
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

                stackDepth = 0;
                NodeCurrent = map;
                left = map.Count;

                while (!NodeCurrent.IsEmpty && skip != Prev(NodeCurrent).Count)
                {
                    if (skip < Prev(NodeCurrent).Count)
                    {
                        stack[stackDepth] = NodeCurrent;
                        stackDepth++;
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
                    stack[stackDepth] = NodeCurrent;
                    stackDepth++;
                }
            }
        }
    }
}

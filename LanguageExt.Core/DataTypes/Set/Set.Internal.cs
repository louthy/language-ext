using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using LanguageExt;
using static LanguageExt.Prelude;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    /// <summary>
    /// Immutable set
    /// AVL tree implementation
    /// AVL tree is a self-balancing binary search tree. 
    /// http://en.wikipedia.org/wiki/AVL_tree
    /// </summary>
    /// <typeparam name="A">List item type</typeparam>
    [Serializable]
    internal class SetInternal<OrdA, A> :
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
        public SetInternal(IEnumerable<A> items) : this(items, true)
        {
        }

        public override int GetHashCode()
        {
            if (hashCode != 0) return hashCode;
            return hashCode = hash(this.AsEnumerable());
        }

        public Seq<A> AsEnumerable()
        {
            IEnumerable<A> Yield()
            {
                var iter = GetEnumerator();
                while (iter.MoveNext())
                {
                    yield return iter.Current;
                }
            }
            return Seq(Yield());
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
        internal SetInternal(IEnumerable<A> items, bool tryAdd)
        {
            set = SetItem<A>.Empty;

            if (tryAdd)
            {
                foreach (var item in items)
                {
                    set = SetModule.TryAdd<OrdA, A>(set, item);
                }
            }
            else
            {
                foreach (var item in items)
                {
                    set = SetModule.Add<OrdA, A>(set, item);
                }
            }
        }

        /// <summary>
        /// Number of items in the set
        /// </summary>
        [Pure]
        public int Count =>
            set.Count;

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
        /// Returns the elements that are in both this and other
        /// </summary>
        [Pure]
        public SetInternal<OrdA, A> Intersect(IEnumerable<A> other)
        {
            var res = new List<A>();
            foreach (var item in other)
            {
                if (Contains(item))
                    res.Add(item);
            }
            return new SetInternal<OrdA, A>(res);
        }

        /// <summary>
        /// Returns this - other.  Only the items in this that are not in 
        /// other will be returned.
        /// </summary>
        [Pure]
        public SetInternal<OrdA, A> Except(IEnumerable<A> other)
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
        public SetInternal<OrdA, A> SymmetricExcept(IEnumerable<A> other)
        {
            var rhs = new SetInternal<OrdA, A>(other);
            var res = new List<A>();

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

            return new SetInternal<OrdA, A>(res);
        }

        /// <summary>
        /// Finds the union of two sets and produces a new set with 
        /// the results
        /// </summary>
        /// <param name="other">Other set to union with</param>
        /// <returns>A set which contains all items from both sets</returns>
        [Pure]
        public SetInternal<OrdA, A> Union(IEnumerable<A> other)
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
        public SetInternal<OrdA, A> Clear() =>
            Empty;

        /// <summary>
        /// Get enumerator
        /// </summary>
        /// <returns>IEnumerator T</returns>
        [Pure]
        public IEnumerator<A> GetEnumerator() =>
            new SetModule.SetEnumerator<A>(set,false,0);

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
        /// <param name="mapper">Mapping function</param>
        /// <returns>Mapped Set</returns>
        [Pure]
        public SetInternal<OrdB, B> Map<OrdB, B>(Func<A, B> map) where OrdB : struct, Ord<B> =>
            new SetInternal<OrdB, B>(this.AsEnumerable().Select(map), true);

        /// <summary>
        /// Maps the values of this set into a new set of values using the
        /// mapper function to tranform the source values.
        /// </summary>
        /// <typeparam name="R">Mapped element type</typeparam>
        /// <param name="mapper">Mapping function</param>
        /// <returns>Mapped Set</returns>
        [Pure]
        public SetInternal<OrdA, A> Map(Func<A, A> map) =>
            new SetInternal<OrdA, A>(this.AsEnumerable().Select(map), true);

        /// <summary>
        /// Filters items from the set using the predicate.  If the predicate
        /// returns True for any item then it remains in the set, otherwise
        /// it's dropped.
        /// </summary>
        /// <param name="pred">Predicate</param>
        /// <returns>Filtered enumerable</returns>
        [Pure]
        public SetInternal<OrdA, A> Filter(Func<A, bool> pred) =>
            new SetInternal<OrdA, A>(SetModule.Filter(set, pred));

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

            var otherSet = new Set<A>(other);
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
            var self = this;
            foreach (var item in rhs)
            {
                self = self.Remove(item);
            }
            return self;
        }

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="other">Other set to test</param>
        /// <returns>True if sets are equal</returns>
        [Pure]
        public bool Equals(SetInternal<OrdA, A> other) =>
            SetEquals(other.AsEnumerable());
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

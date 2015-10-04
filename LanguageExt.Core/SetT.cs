using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Threading;

namespace LanguageExt
{
    /// <summary>
    /// Immutable set
    /// </summary>
    /// <typeparam name="T">List item type</typeparam>
    public class Set<T> : IEnumerable<T>, IEnumerable, IReadOnlyCollection<T>, ICollection<T>, ISet<T>, ICollection
    {
        public static readonly Set<T> Empty = new Set<T>();
        readonly SetItem<T> set;

        internal Set()
        {
            set = SetItem<T>.Empty;
        }

        internal Set(SetItem<T> root)
        {
            set = root;
        }

        internal Set(IEnumerable<T> items)
        {
            set = SetItem<T>.Empty;

            // TODO: Perf
            foreach (var item in items)
            {
                set = SetModule.Add(set, item, Comparer<T>.Default);
            }
        }

        public int Count
        {
            get
            {
                return set.Count;
            }
        }

        public Set<T> Add(T value) =>
            new Set<T>(SetModule.Add(set,value,Comparer<T>.Default));

        public Option<T> Find(T value) =>
            SetModule.Find(set, value, Comparer<T>.Default);

        public Set<T> Intersect(IEnumerable<T> other)
        {
            // TODO: Perf
            var res = new List<T>();
            foreach (var item in other)
            {
                if (Contains(item)) res.Add(item);
            }
            return new Set<T>(res);
        }

        public Set<T> Except(IEnumerable<T> other)
        {
            // TODO: Perf
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

        public Set<T> SymmetricExcept(IEnumerable<T> other)
        {
            // TODO: Perf
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

        public Set<T> Union(IEnumerable<T> other)
        {
            // TODO: Perf
            var self = this;
            foreach (var item in other)
            {
                if (!self.Contains(item))
                {
                    self = self.Add(item);
                }
            }
            return self;
        }

        public Set<T> Clear() =>
            new Set<T>();

        public IEnumerator<T> GetEnumerator() =>
            new SetModule.SetEnumerator<T>(set,false,0);

        public Set<T> Remove(T value) =>
            new Set<T>(SetModule.Remove(set, value, Comparer<T>.Default));

        IEnumerator IEnumerable.GetEnumerator() =>
            new SetModule.SetEnumerator<T>(set, false, 0);

        public S Fold<S>(S state, Func<S, T, S> folder) =>
            SetModule.Fold(set,state,folder);

        public Set<U> Map<U>(Func<T, U> map) =>
            new Set<U>(this.AsEnumerable().Select(map).Distinct());

        public Set<T> Filter(Func<T, bool> pred) =>
            new Set<T>(SetModule.Filter(set, pred));

        public bool Contains(T value) =>
            SetModule.Contains(set, value, Comparer<T>.Default);

        public bool SetEquals(IEnumerable<T> other)
        {
            // TODO: Perf

            var rhs = new Set<T>(other);
            if (rhs.Count() != Count) return false;
            foreach (var item in rhs)
            {
                if (!Contains(item)) return false;
            }
            return true;
        }

        public bool IsEmpty => 
            Count == 0;

        public bool IsReadOnly
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public object SyncRoot
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsSynchronized
        {
            get
            {
                throw new NotImplementedException();
            }
        }

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

        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        void ICollection<T>.Clear()
        {
            throw new NotSupportedException();
        }

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

        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }

        bool ISet<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        public void UnionWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

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

        internal int BalanceFactor =>
            Count == 0
                ? 0
                : ((int)Left.Height) - ((int)Right.Height);

        public K Key
        {
            get;
            private set;
        }
    }

    internal static class SetModule
    {
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

        public static bool ForAll<K>(SetItem<K> node, Func<K, bool> pred) =>
            node.IsEmpty
                ? true
                : pred(node.Key)
                    ? ForAll(node.Left, pred) && ForAll(node.Right, pred)
                    : false;

        public static bool Exists<K>(SetItem<K> node, Func<K, bool> pred) =>
            node.IsEmpty
                ? false
                : pred(node.Key)
                    ? true
                    : Exists(node.Left, pred) || Exists(node.Right, pred);

        public static SetItem<K> Filter<K>(SetItem<K> node, Func<K, bool> pred) =>
            node.IsEmpty
                ? node
                : pred(node.Key)
                    ? Balance(Make(node.Key, Filter(node.Left, pred), Filter(node.Right, pred)))
                    : Balance(Filter(AddTreeToRight(node.Left, node.Right), pred));

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

        public static SetItem<K> AddTreeToRight<K>(SetItem<K> node, SetItem<K> toAdd) =>
            node.IsEmpty
                ? toAdd
                : Balance(Make(node.Key, node.Left, AddTreeToRight(node.Right, toAdd)));

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

        public static SetItem<K> Make<K>(K k, SetItem<K> l, SetItem<K> r) =>
            new SetItem<K>((byte)(1 + Math.Max(l.Height, r.Height)), l.Count + r.Count + 1, k, l, r);

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

        public static SetItem<K> RotRight<K>(SetItem<K> node) =>
            node.IsEmpty || node.Left.IsEmpty
                ? node
                : Make(node.Left.Key, node.Left.Left, Make(node.Key, node.Left.Right, node.Right));

        public static SetItem<K> RotLeft<K>(SetItem<K> node) =>
            node.IsEmpty || node.Right.IsEmpty
                ? node
                : Make(node.Right.Key, Make(node.Key, node.Left, node.Right.Left), node.Right.Right);

        public static SetItem<K> DblRotRight<K>(SetItem<K> node) =>
            node.IsEmpty
                ? node
                : RotRight(Make(node.Key, RotLeft(node.Left), node.Right));

        public static SetItem<K> DblRotLeft<K>(SetItem<K> node) =>
            node.IsEmpty
                ? node
                : RotLeft(Make(node.Key, node.Left, RotRight(node.Right)));

        public class SetEnumerator<K> : IEnumerator<K>
        {
            static ObjectPool<Stack<SetItem<K>>> pool = new ObjectPool<Stack<SetItem<K>>>(32, () => new Stack<SetItem<K>>(32));

            Stack<SetItem<K>> stack;
            SetItem<K> map;
            int left;
            bool rev;
            int start;

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
                pool.Release(stack);
                stack = null;
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using System.Threading;

namespace LanguageExt
{
    /// <summary>
    /// Immutable list
    /// </summary>
    /// <typeparam name="T">Value type</typeparam>
    public class Lst<T> : IEnumerable<T>, IEnumerable, IReadOnlyList<T>, IReadOnlyCollection<T>
    {
        /// <summary>
        /// Empty list
        /// </summary>
        public static readonly Lst<T> Empty = new Lst<T>();

        internal readonly ListItem<T> Root;
        internal readonly bool Rev;

        /// <summary>
        /// Ctor
        /// </summary>
        internal Lst()
        {
            Root = ListItem<T>.Empty;
        }

        /// <summary>
        /// Ctor
        /// </summary>
        internal Lst(IEnumerable<T> initial)
        {
            var lst = new List<T>(initial); // TODO: Perf
            Root = ListModule.FromList(lst, 0, lst.Count());
            Rev = false;
        }

        /// <summary>
        /// Ctor
        /// </summary>
        internal Lst(ListItem<T> root, bool rev)
        {
            Root = root;
            Rev = rev;
        }

        /// <summary>
        /// Index accessor
        /// </summary>
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= Root.Count) throw new IndexOutOfRangeException();
                return ListModule.GetItem(Root, index);
            }
        }

        /// <summary>
        /// Number of items in the list
        /// </summary>
        public int Count
        {
            get
            {
                return Root.Count;
            }
        }


        int IReadOnlyCollection<T>.Count
        {
            get
            {
                return Count;
            }
        }

        T IReadOnlyList<T>.this[int index]
        {
            get
            {
                return this[index];
            }
        }

        /// <summary>
        /// Add an item to the end of the list
        /// </summary>
        public Lst<T> Add(T value)
        {
            return new Lst<T>(ListModule.Insert(Root, value, Rev ? 0 : Root.Count), Rev);
        }

        /// <summary>
        /// Add a range of items to the end of the list
        /// </summary>
        public Lst<T> AddRange(IEnumerable<T> items)
        {
            if (items == null) return this;
            var lst = new List<T>(Rev ? items.Reverse() : items); // Perf: not ideal
            var tree = ListModule.FromList(lst, 0, lst.Count);
            return new Lst<T>(ListModule.Insert(Root, tree, Rev ? 0 : Root.Count), Rev);
        }

        /// <summary>
        /// Clear the list
        /// </summary>
        public Lst<T> Clear() =>
            Empty;

        /// <summary>
        /// Get enumerator
        /// </summary>
        public IEnumerator<T> GetEnumerator() =>
            new ListModule.ListEnumerator<T>(Root,Rev,0);

        /// <summary>
        /// Find the index of an item
        /// </summary>
        public int IndexOf(T item, int index, int count, IEqualityComparer<T> equalityComparer)
        {
            if (count == 0) return -1;
            if (index < 0 || index >= Root.Count) throw new IndexOutOfRangeException();

            foreach (var x in this.Skip(index))
            {
                if (equalityComparer.Equals(x, item))
                {
                    return index;
                }
                index++;
                count--;
                if (count == 0) return -1;
            }
            return -1;
        }

        /// <summary>
        /// Insert value at specified index
        /// </summary>
        public Lst<T> Insert(int index, T value)
        {
            if (index < 0 || index > Root.Count) throw new IndexOutOfRangeException();
            return new Lst<T>(ListModule.Insert(Root, value, Rev ? Root.Count - index: index), Rev);
        }

        /// <summary>
        /// Insert range of values at specified index
        /// </summary>
        public Lst<T> InsertRange(int index, IEnumerable<T> items)
        {
            if (items == null) return this;
            if (index < 0 || index > Root.Count) throw new IndexOutOfRangeException();

            var lst = new List<T>(Rev ? items.Reverse() : items); // Perf: not ideal
            var tree = ListModule.FromList(lst, 0, lst.Count);
            return new Lst<T>(ListModule.Insert(Root, tree, Rev ? Root.Count - index : index), Rev);
        }

        /// <summary>
        /// Find the last index of an item in the list
        /// </summary>
        public int LastIndexOf(T item, int index, int count, IEqualityComparer<T> equalityComparer)
        {
            if (count == 0) return -1;
            if (index < 0 || index >= Root.Count) throw new IndexOutOfRangeException();

            var rev = new Lst<T>(Root, true);

            foreach (var x in rev.Skip(index))
            {
                if (equalityComparer.Equals(x, item))
                {
                    return index;
                }
                index++;
                count--;
                if (count == 0) return -1;
            }
            return -1;
        }

        /// <summary>
        /// Remove an item from the list
        /// </summary>
        public Lst<T> Remove(T value) => 
            Remove(value, Comparer<T>.Default);

        /// <summary>
        /// Remove an item from the list
        /// </summary>
        public Lst<T> Remove(T value, IComparer<T> equalityComparer)
        {
            var index = ListModule.Find(Root, value, 0, Count, equalityComparer);
            if (index >= 0 && index < Count)
            {
                return new Lst<T>(ListModule.Remove(Root, index), Rev);
            }
            else
            {
                return this;
            }
        }

        /// <summary>
        /// Remove all items that match a predicate
        /// </summary>
        public Lst<T> RemoveAll(Predicate<T> match)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Remove item at location
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Lst<T> RemoveAt(int index)
        {
            if (index < 0 || index >= Root.Count) throw new IndexOutOfRangeException();
            return new Lst<T>(ListModule.Remove(Root, Rev ? Count - index : index), Rev);
        }

        /// <summary>
        /// Remove a range of items
        /// </summary>
        public Lst<T> RemoveRange(int index, int count)
        {
            if (index < 0 || index >= Root.Count) throw new IndexOutOfRangeException();
            if (index + count >= Root.Count) throw new IndexOutOfRangeException();

            // TODO: Perf
            var self = this;
            for (; count > 0; count--)
            {
                self = self.RemoveAt(index);
            }
            return self;
        }

        /// <summary>
        /// Set an item at the specified index
        /// </summary>
        public Lst<T> SetItem(int index, T value)
        {
            if (value == null) throw new ArgumentNullException("'value' cannot be null.");
            if (index < 0 || index >= Root.Count) throw new IndexOutOfRangeException();
            return new Lst<T>(ListModule.SetItem(Root,value,index),Rev);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new ListModule.ListEnumerator<T>(Root, Rev, 0);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new ListModule.ListEnumerator<T>(Root, Rev, 0);
        }

        /// <summary>
        /// Reverse the order of the items in the list
        /// </summary>
        public Lst<T> Reverse() =>
            new Lst<T>(Root, !Rev);

        /// <summary>
        /// Fold
        /// </summary>
        public S Fold<S>(S state, Func<S, T, S> folder) =>
            ListModule.Fold(Root, state, folder);

        /// <summary>
        /// Map
        /// </summary>
        public Lst<U> Map<U>(Func<T, U> map) =>
            new Lst<U>(ListModule.Map(Root,map),Rev);

        /// <summary>
        /// Filter
        /// </summary>
        public Lst<T> Filter(Func<T, bool> pred)
        {
            var filtered = new List<T>();
            foreach (var item in this)
            {
                if (pred(item))
                {
                    filtered.Add(item);
                }
            }
            var root = ListModule.FromList(filtered, 0, filtered.Count);
            return new Lst<T>(root, Rev);
        }
    }

    internal class ListItem<T>
    {
        public static readonly ListItem<T> Empty = new ListItem<T>(0, 0, default(T), null, null);

        public bool IsEmpty => Count == 0;
        public readonly int Count;
        public readonly byte Height;
        public readonly ListItem<T> Left;
        public readonly ListItem<T> Right;

        /// <summary>
        /// Ctor
        /// </summary>
        internal ListItem(byte height, int count, T key, ListItem<T> left, ListItem<T> right)
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

        public T Key
        {
            get;
            private set;
        }
    }

    internal static class ListModule
    {
        public static S Fold<S, T>(ListItem<T> node, S state, Func<S, T, S> folder)
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

        public static bool ForAll<K, V>(MapItem<K, V> node, Func<K, V, bool> pred) =>
            node.IsEmpty
                ? true
                : pred(node.Key, node.Value)
                    ? ForAll(node.Left, pred) && ForAll(node.Right, pred)
                    : false;

        public static bool Exists<T>(ListItem<T> node, Func<T, bool> pred) =>
            node.IsEmpty
                ? false
                : pred(node.Key)
                    ? true
                    : Exists(node.Left, pred) || Exists(node.Right, pred);

        public static ListItem<U> Map<T, U>(ListItem<T> node, Func<T, U> mapper) =>
            node.IsEmpty
                ? ListItem<U>.Empty
                : new ListItem<U>(node.Height, node.Count, mapper(node.Key), Map(node.Left, mapper), Map(node.Right, mapper));

        public static ListItem<T> Insert<T>(ListItem<T> node, T key, int index)
        {
            if (node.IsEmpty)
            {
                return new ListItem<T>(1, 1, key, ListItem<T>.Empty, ListItem<T>.Empty);
            }
            if (index <= node.Left.Count)
            {
                return Balance(Make(node.Key, Insert(node.Left, key, index), node.Right));
            }
            else
            {
                return Balance(Make(node.Key, node.Left, Insert(node.Right, key, index)));
            }
        }

        public static ListItem<T> Insert<T>(ListItem<T> node, ListItem<T> insertNode, int index)
        {
            if (node.IsEmpty)
            {
                return insertNode;
            }
            if (index <= node.Left.Count)
            {
                return Balance(Make(node.Key, Insert(node.Left, insertNode, index), node.Right));
            }
            else
            {
                return Balance(Make(node.Key, node.Left, Insert(node.Right, insertNode, index)));
            }
        }

        public static ListItem<T> FromList<T>(IList<T> items, int start, int length)
        {
            if (length == 0)
            {
                return ListItem<T>.Empty;
            }
            int rightCount = (length - 1) / 2;
            int leftCount = (length - 1) - rightCount;
            var left = FromList(items, start, leftCount);
            var right = FromList(items, start + leftCount + 1, rightCount);
            return Make(items[start + leftCount], left, right);
        }

        public static ListItem<T> SetItem<T>(ListItem<T> node, T key, int index)
        {
            if (node.IsEmpty)
            {
                throw new ArgumentException("Index outside the bounds of the list");
            }

            if (index == node.Left.Count)
            {
                return new ListItem<T>(node.Height, node.Count, key, node.Left, node.Right);
            }
            else if (index < node.Left.Count)
            {
                return new ListItem<T>(node.Height, node.Count, node.Key, SetItem(node.Left, key, index), node.Right);
            }
            else
            {
                return new ListItem<T>(node.Height, node.Count, node.Key, SetItem(node.Right, key, index - node.Left.Count -1), node.Right);
            }
        }

        public static T GetItem<T>(ListItem<T> node, int index)
        {
            if (node.IsEmpty)
            {
                throw new ArgumentException("Index outside the bounds of the list");
            }

            if (index == node.Left.Count)
            {
                return node.Key;
            }
            else if (index < node.Left.Count)
            {
                return GetItem(node.Left, index);
            }
            else
            {
                return GetItem(node.Right, index - node.Left.Count - 1);
            }
        }

        public static ListItem<T> Remove<T>(ListItem<T> node, int index)
        {
            if (node.IsEmpty)
            {
                return node;
            }

            if (index == node.Left.Count)
            {
                if (node.Right.IsEmpty && node.Left.IsEmpty)
                {
                    return ListItem<T>.Empty;
                }
                else if (node.Right.IsEmpty && !node.Left.IsEmpty)
                {
                    return Balance(node.Left);
                }
                else if (!node.Right.IsEmpty && node.Left.IsEmpty)
                {
                    return Balance(node.Right);
                }
                else
                {
                    var next = node.Right;
                    while (!next.Left.IsEmpty)
                    {
                        next = next.Left;
                    }

                    var right = Remove(node.Right, 0);
                    return Balance(Make(node.Key, node.Left, right));
                }
            }
            else if (index < node.Left.Count)
            {
                var left = Remove(node.Left, index);
                return Balance(Make(node.Key,left,node.Right));
            }
            else
            {
                var right = Remove(node.Right, index - node.Left.Count - 1);
                return Balance(Make(node.Key,node.Left, right));
            }
        }

        public static int Find<T>(ListItem<T> node, T key, int index, int count, IComparer<T> comparer)
        {
            comparer = comparer ?? Comparer<T>.Default;

            if (node.IsEmpty || node.Count <= 0)
            {
                return ~index;
            }

            int nodeIndex = node.Left.Count;
            if (index + count <= nodeIndex)
            {
                return Find(node.Left, key, index, count, comparer);
            }
            else if (index > nodeIndex)
            {
                int result = Find(node.Right, key, index - nodeIndex - 1, count, comparer);
                int offset = nodeIndex + 1;
                return result < 0 ? result - offset : result + offset;
            }
            int compare = comparer.Compare(key, node.Key);
            if (compare == 0)
            {
                return nodeIndex;
            }
            else if (compare > 0)
            {
                int adjcount = count - (nodeIndex - index) - 1;
                int result = adjcount < 0 ? -1 : Find(node.Right,key, 0, adjcount, comparer);
                int offset = nodeIndex + 1;
                return result < 0 ? result - offset : result + offset;
            }
            else
            {
                if (index == nodeIndex)
                {
                    return ~index;
                }
                return Find(node.Left,key,index,count,comparer);
            }
        }

        public static ListItem<T> Skip<T>(ListItem<T> node, int amount)
        {
            if (amount == 0 || node.IsEmpty)
            {
                return node;
            }
            if (amount > node.Count)
            {
                return ListItem<T>.Empty;
            }
            if (!node.Left.IsEmpty && node.Left.Count == amount)
            {
                return Balance(Make(node.Key, ListItem<T>.Empty, node.Right));
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

        public static ListItem<T> Make<T>(T k, ListItem<T> l, ListItem<T> r) =>
            new ListItem<T>((byte)(1 + Math.Max(l.Height, r.Height)), l.Count + r.Count + 1, k, l, r);

        public static ListItem<T> Balance<T>(ListItem<T> node) =>
            node.BalanceFactor >= 2
                ? node.Left.BalanceFactor >= 1
                    ? RotRight(node)
                    : DblRotRight(node)
                : node.BalanceFactor <= -2
                    ? node.Left.BalanceFactor <= -1
                        ? RotLeft(node)
                        : DblRotLeft(node)
                    : node;

        public static ListItem<T> RotRight<T>(ListItem<T> node) =>
            node.IsEmpty || node.Left.IsEmpty
                ? node
                : Make(node.Left.Key, node.Left.Left, Make(node.Key, node.Left.Right, node.Right));

        public static ListItem<T> RotLeft<T>(ListItem<T> node) =>
            node.IsEmpty || node.Right.IsEmpty
                ? node
                : Make(node.Right.Key, Make(node.Key, node.Left, node.Right.Left), node.Right.Right);

        public static ListItem<T> DblRotRight<T>(ListItem<T> node) =>
            node.IsEmpty
                ? node
                : RotRight(Make(node.Key, RotLeft(node.Left), node.Right));

        public static ListItem<T> DblRotLeft<T>(ListItem<T> node) =>
            node.IsEmpty
                ? node
                : RotLeft(Make(node.Key, node.Left, RotRight(node.Right)));


        // TODO: Make the StackPool more robust rather than just a circular
        //       buffer hoping that no-one overlaps.
        public static class StackPool<T>
        {
            const int stackPoolSize = 1024;
            const int stackDepth = 32;
            static Stack<ListItem<T>>[] stackPool;
            static int poolIndex = -1;

            static StackPool()
            {
                stackPool = new Stack<ListItem<T>>[stackPoolSize];

                for (var i = 0; i < stackPoolSize; i++)
                {
                    stackPool[i] = new Stack<ListItem<T>>(stackDepth);
                }
            }

            public static Stack<ListItem<T>> Next()
            {
                Interlocked.CompareExchange(ref poolIndex, -1, stackPoolSize);
                return stackPool[Interlocked.Increment(ref poolIndex)];
            }
        }

        public class ListEnumerator<T> : IEnumerator<T>
        {
            Stack<ListItem<T>> stack;
            ListItem<T> map;
            int left;
            bool rev;
            int start;

            public ListEnumerator(ListItem<T> root, bool rev, int start)
            {
                this.rev = rev;
                this.start = start;
                map = root;
                stack = new Stack<ListItem<T>>(32); // StackPool<T>.Next();
                Reset();
            }

            private ListItem<T> NodeCurrent
            {
                get;
                set;
            }

            public T Current => NodeCurrent.Key;
            object IEnumerator.Current => NodeCurrent.Key;

            public void Dispose()
            {
            }

            private ListItem<T> Next(ListItem<T> node) =>
                rev ? node.Left : node.Right;

            private ListItem<T> Prev(ListItem<T> node) =>
                rev ? node.Right : node.Left;

            private void Push(ListItem<T> node)
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
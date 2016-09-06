using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Threading;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    /// <summary>
    /// Immutable list
    /// </summary>
    /// <typeparam name="T">Value type</typeparam>
#if !COREFX
    [Serializable]
#endif
    public class Lst<T> : 
        IEnumerable<T>, 
        IEnumerable, 
        IReadOnlyList<T>, 
        IReadOnlyCollection<T>, 
        IAppendable<Lst<T>>, 
        ISubtractable<Lst<T>>,
        IMultiplicable<Lst<T>>,
        IDivisible<Lst<T>>,
        IEquatable<Lst<T>>
    {
        /// <summary>
        /// Empty list
        /// </summary>
        public static readonly Lst<T> Empty = new Lst<T>();

        internal readonly ListItem<T> Root;
        internal readonly bool Rev;
        internal int HashCode;

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
            if (initial is Lst<T>)
            {
                var lst = (Lst<T>)initial;
                Root = lst.Root;
                Rev = lst.Rev;
            }
            else
            {
                var lst = new List<T>(initial);
                Root = ListModule.FromList(lst, 0, lst.Count());
                Rev = false;
            }
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
        [Pure]
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= Root.Count) throw new IndexOutOfRangeException();
                return ListModule.GetItem(Root, Rev ? Count - index - 1 : index);
            }
        }

        /// <summary>
        /// Number of items in the list
        /// </summary>
        [Pure]
        public int Count
        {
            get
            {
                return Root.Count;
            }
        }

        [Pure]
        int IReadOnlyCollection<T>.Count
        {
            get
            {
                return Count;
            }
        }

        [Pure]
        T IReadOnlyList<T>.this[int index]
        {
            get
            {
                if (index < 0 || index >= Root.Count) throw new IndexOutOfRangeException();
                return ListModule.GetItem(Root, Rev ? Count - index - 1 : index);
            }
        }

        /// <summary>
        /// Add an item to the end of the list
        /// </summary>
        [Pure]
        public Lst<T> Add(T value) =>
            new Lst<T>(ListModule.Insert(Root, value, Rev ? 0 : Root.Count), Rev);

        /// <summary>
        /// Add a range of items to the end of the list
        /// </summary>
        [Pure]
        public Lst<T> AddRange(IEnumerable<T> items)
        {
            if (items == null) return this;
            var lst = new List<T>(Rev ? items.Reverse() : items);
            var tree = ListModule.FromList(lst, 0, lst.Count);
            return new Lst<T>(ListModule.Insert(Root, tree, Rev ? 0 : Root.Count), Rev);
        }

        /// <summary>
        /// Clear the list
        /// </summary>
        [Pure]
        public Lst<T> Clear() =>
            Empty;

        /// <summary>
        /// Get enumerator
        /// </summary>
        [Pure]
        public IEnumerator<T> GetEnumerator() =>
            new ListModule.ListEnumerator<T>(Root,Rev,0);

        /// <summary>
        /// Find the index of an item
        /// </summary>
        [Pure]
        public int IndexOf(T item, int index = 0, int count = -1, IEqualityComparer<T> equalityComparer = null)
        {
            count = count == -1
                ? Count
                : count;

            equalityComparer = equalityComparer ?? EqualityComparer<T>.Default;

            if (count == 0) return -1;
            if (index < 0 || index >= Root.Count) throw new IndexOutOfRangeException();

            foreach (var x in Skip(index))
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
        [Pure]
        public Lst<T> Insert(int index, T value)
        {
            if (index < 0 || index > Root.Count) throw new IndexOutOfRangeException();
            return new Lst<T>(ListModule.Insert(Root, value, Rev ? Count - index - 1 : index), Rev);
        }

        /// <summary>
        /// Insert range of values at specified index
        /// </summary>
        [Pure]
        public Lst<T> InsertRange(int index, IEnumerable<T> items)
        {
            if (items == null) return this;
            if (index < 0 || index > Root.Count) throw new IndexOutOfRangeException();

            var lst = new List<T>(Rev ? items.Reverse() : items);
            var tree = ListModule.FromList(lst, 0, lst.Count);
            return new Lst<T>(ListModule.Insert(Root, tree, Rev ? Count - index - 1 : index), Rev);
        }

        /// <summary>
        /// Find the last index of an item in the list
        /// </summary>
        [Pure]
        public int LastIndexOf(T item, int index = 0, int count = -1, IEqualityComparer<T> equalityComparer = null) =>
            Count - Reverse().IndexOf(item, index, count, equalityComparer) - 1;

        /// <summary>
        /// Remove an item from the list
        /// </summary>
        [Pure]
        public Lst<T> Remove(T value) => 
            Remove(value, Comparer<T>.Default);

        /// <summary>
        /// Remove an item from the list
        /// </summary>
        [Pure]
        public Lst<T> Remove(T value, IComparer<T> equalityComparer)
        {
            var index = ListModule.Find(Root, value, 0, Count, equalityComparer);
            return index >= 0 && index < Count
                ? new Lst<T>(ListModule.Remove(Root, index), Rev)
                : this;
        }

        /// <summary>
        /// Remove all items that match a predicate
        /// </summary>
        [Pure]
        public Lst<T> RemoveAll(Predicate<T> pred)
        {
            var self = this;
            int index = 0;
            foreach (var item in this)
            {
                if (pred(item))
                {
                    self = self.RemoveAt(index);
                }
                else
                {
                    index++;
                }
            }
            return self;
        }

        /// <summary>
        /// Remove item at location
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        [Pure]
        public Lst<T> RemoveAt(int index)
        {
            if (index < 0 || index >= Root.Count) throw new IndexOutOfRangeException();
            return new Lst<T>(ListModule.Remove(Root, Rev ? Count - index - 1 : index), Rev);
        }

        /// <summary>
        /// Remove a range of items
        /// </summary>
        [Pure]
        public Lst<T> RemoveRange(int index, int count)
        {
            if (index < 0 || index >= Root.Count) throw new IndexOutOfRangeException();
            if (index + count >= Root.Count) throw new IndexOutOfRangeException();

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
        [Pure]
        public Lst<T> SetItem(int index, T value)
        {
            if (isnull(value)) throw new ArgumentNullException(nameof(value));
            if (index < 0 || index >= Root.Count) throw new IndexOutOfRangeException();
            return new Lst<T>(ListModule.SetItem(Root,value,index),Rev);
        }

        [Pure]
        IEnumerator IEnumerable.GetEnumerator() =>
            new ListModule.ListEnumerator<T>(Root, Rev, 0);

        [Pure]
        IEnumerator<T> IEnumerable<T>.GetEnumerator() =>
            new ListModule.ListEnumerator<T>(Root, Rev, 0);

        [Pure]
        public IEnumerable<T> Skip(int amount)
        {
            var iter = new ListModule.ListEnumerator<T>(Root, Rev, amount);
            while (iter.MoveNext())
            {
                yield return iter.Current;
            }
        }

        /// <summary>
        /// Reverse the order of the items in the list
        /// </summary>
        [Pure]
        public Lst<T> Reverse()
        {
            // This is currenty buggy, so going the safe (and less efficient) route for now
            // return new Lst<T>(Root, !Rev);
            return new Lst<T>(this.AsEnumerable().Reverse());
        }

        /// <summary>
        /// Fold
        /// </summary>
        [Pure]
        public S Fold<S>(S state, Func<S, T, S> folder)
        {
            foreach (var item in this)
            {
                state = folder(state, item);
            }
            return state;
        }

        /// <summary>
        /// Map
        /// </summary>
        [Pure]
        public Lst<U> Map<U>(Func<T, U> map) =>
            new Lst<U>(ListModule.Map(Root,map),Rev);

        /// <summary>
        /// Filter
        /// </summary>
        [Pure]
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

        [Pure]
        public static Lst<T> operator +(Lst<T> lhs, T rhs) =>
            lhs.Add(rhs);

        [Pure]
        public static Lst<T> operator +(T rhs, Lst<T> lhs) =>
            rhs.Cons(lhs);

        [Pure]
        public static Lst<T> operator +(Lst<T> lhs, Lst<T> rhs) =>
            lhs.Append(rhs);

        [Pure]
        public Lst<T> Append(Lst<T> rhs) =>
            AddRange(rhs);

        [Pure]
        public static Lst<T> operator -(Lst<T> lhs, Lst<T> rhs) =>
            lhs.Subtract(rhs);

        [Pure]
        public Lst<T> Subtract(Lst<T> rhs)
        {
            var self = this;
            foreach (var item in rhs)
            {
                self = self.Remove(item);
            }
            return self;
        }

        [Pure]
        public static Lst<T> operator *(Lst<T> lhs, Lst<T> rhs) =>
            lhs.Multiply(rhs);

        [Pure]
        public Lst<T> Multiply(Lst<T> rhs) =>
            (from x in this.AsEnumerable()
             from y in rhs.AsEnumerable()
             select TypeDesc.Multiply(x, y, TypeDesc<T>.Default)).Freeze();

        [Pure]
        public static Lst<T> operator /(Lst<T> lhs, Lst<T> rhs) =>
            lhs.Divide(rhs);

        [Pure]
        public Lst<T> Divide(Lst<T> rhs) =>
            (from y in rhs.AsEnumerable()
             from x in this.AsEnumerable()
             select TypeDesc.Divide(x, y, TypeDesc<T>.Default)).Freeze();

        [Pure]
        public override bool Equals(object obj) =>
            !ReferenceEquals(obj,null) && 
            obj is Lst<T> && 
            Equals((Lst<T>)obj);

        /// <summary>
        /// Get the hash code
        /// Lazily (and once only) calculates the hash from the elements in the list
        /// Empty list hash == 0
        /// </summary>
        [Pure]
        public override int GetHashCode()
        {
            if (Count == 0)
            {
                return 0;
            }
            else
            {
                if (HashCode == 0)
                {
                    unchecked
                    {
                        HashCode = Fold(7, (s, x) => s + x.GetHashCode() * 13);
                    }
                }
                return HashCode;
            }
        }

        [Pure]
        public bool Equals(Lst<T> other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (ReferenceEquals(other, null)) return false;
            var comparer = EqualityComparer<T>.Default;
            return Count == other.Count && this.Zip(other, (x, y) => comparer.Equals(x, y)).ForAll(x => x);
        }

        [Pure]
        public static bool operator ==(Lst<T> lhs, Lst<T> rhs) =>
              ReferenceEquals(lhs, null)  && ReferenceEquals(rhs, null) ? true
            : ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null)  ? false
            : lhs.Equals(rhs);

        [Pure]
        public static bool operator !=(Lst<T> lhs, Lst<T> rhs) =>
              ReferenceEquals(lhs, null) && ReferenceEquals(rhs, null) ? true
            : ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null) ? false
            : !lhs.Equals(rhs);
    }

#if !COREFX
    [Serializable]
#endif
    class ListItem<T>
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

    static class ListModule
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

        public class ListEnumerator<T> : IEnumerator<T>
        {
            static ObjectPool<Stack<ListItem<T>>> pool = new ObjectPool<Stack<ListItem<T>>>(32, () => new Stack<ListItem<T>>(32));

            Stack<ListItem<T>> stack = null;
            ListItem<T> map;
            int left;
            bool rev;
            int start;

            public ListEnumerator(ListItem<T> root, bool rev, int start)
            {
                this.rev = rev;
                this.start = start;
                map = root;
                stack = pool.GetItem();
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
                if (stack != null)
                {
                    pool.Release(stack);
                    stack = null;
                }
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
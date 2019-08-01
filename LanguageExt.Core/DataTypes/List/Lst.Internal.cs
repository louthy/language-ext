using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.Contracts;
using LanguageExt;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using LanguageExt.ClassInstances;

namespace LanguageExt
{
    /// <summary>
    /// Immutable list
    /// </summary>
    /// <typeparam name="A">Value type</typeparam>
    [Serializable]
    internal class LstInternal<A> : 
        IEnumerable<A>, 
        IReadOnlyList<A>,
        IReadOnlyCollection<A>,
        IEquatable<LstInternal<A>>
    {
        /// <summary>
        /// Empty list
        /// </summary>
        public static readonly LstInternal<A> Empty = new LstInternal<A>();

        internal ListItem<A> root;
        internal int hashCode;

        /// <summary>
        /// Ctor
        /// </summary>
        internal LstInternal(IEnumerable<A> items, Pred<A> pred)
        {
            hashCode = 0;
            root = ListItem<A>.Empty;
            if (items is Lst<A>)
            {
                var lst = (Lst<A>)items;
                root = lst.Value.Root;
            }
            else
            {
                root = ListModuleM.InsertMany(root, items, 0, pred);
            }
        }

        internal LstInternal(IEnumerable<A> items)
        {
            hashCode = 0;
            root = ListItem<A>.Empty;
            if (items is Lst<A>)
            {
                var lst = (Lst<A>)items;
                root = lst.Value.Root;
            }
            else
            {
                root = ListModuleM.InsertMany(root, items, 0);
            }
        }


        internal static LstInternal<A> Wrap(ListItem<A> list) =>
            new LstInternal<A>(list);

        /// <summary>
        /// Ctor
        /// </summary>
        internal LstInternal()
        {
            hashCode = 0;
            this.root = ListItem<A>.Empty;
        }

        /// <summary>
        /// Ctor
        /// </summary>
        internal LstInternal(ListItem<A> root)
        {
            hashCode = 0;
            this.root = root;
        }

        internal ListItem<A> Root =>
            root ?? ListItem<A>.Empty;

        /// <summary>
        /// Index accessor
        /// </summary>
        [Pure]
        public A this[int index]
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
        [Pure]
        public int Count =>
            Root.Count;

        [Pure]
        int IReadOnlyCollection<A>.Count =>
            Count;

        [Pure]
        A IReadOnlyList<A>.this[int index]
        {
            get
            {
                if (index < 0 || index >= Root.Count) throw new IndexOutOfRangeException();
                return ListModule.GetItem(Root, index);
            }
        }

        /// <summary>
        /// Add an item to the end of the list
        /// </summary>
        [Pure]
        public LstInternal<A> Add(A value) =>
            Wrap(ListModule.Insert(Root, value, Root.Count));

        /// <summary>
        /// Add a range of items to the end of the list
        /// </summary>
        [Pure]
        public LstInternal<A> AddRange(IEnumerable<A> items)
        {
            if (items == null) return this;
            if (Count == 0) return new LstInternal<A>(items);
            return Wrap(ListModuleM.InsertMany(Root, items, Count));
        }

        /// <summary>
        /// Clear the list
        /// </summary>
        [Pure]
        public LstInternal<A> Clear() =>
            LstInternal<A>.Empty;

        /// <summary>
        /// Get enumerator
        /// </summary>
        [Pure]
        public IEnumerator<A> GetEnumerator() =>
            new ListModule.ListEnumerator<A>(Root, false, 0);

        /// <summary>
        /// Find the index of an item
        /// </summary>
        [Pure]
        public int IndexOf(A item, int index = 0, int count = -1, IEqualityComparer<A> equalityComparer = null)
        {
            count = count == -1
                ? Count
                : count;

            equalityComparer = equalityComparer ?? EqualityComparer<A>.Default;

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
        public LstInternal<A> Insert(int index, A value)
        {
            if (index < 0 || index > Root.Count) throw new IndexOutOfRangeException();
            return Wrap(ListModule.Insert(Root, value, index));
        }

        /// <summary>
        /// Insert range of values at specified index
        /// </summary>
        [Pure]
        public LstInternal<A> InsertRange(int index, IEnumerable<A> items)
        {
            if (items == null) return this;
            if (index < 0 || index > Root.Count) throw new IndexOutOfRangeException();
            return Wrap(ListModuleM.InsertMany(Root, items, index));
        }

        /// <summary>
        /// Insert range of values at specified index
        /// </summary>
        [Pure]
        public LstInternal<A> InsertRange(int index, IEnumerable<A> items, Pred<A> pred)
        {
            if (items == null) return this;
            if (index < 0 || index > Root.Count) throw new IndexOutOfRangeException();
            return Wrap(ListModuleM.InsertMany(Root, items, index, pred));
        }

        /// <summary>
        /// Find the last index of an item in the list
        /// </summary>
        [Pure]
        public int LastIndexOf(A item, int index = 0, int count = -1, IEqualityComparer<A> equalityComparer = null) =>
            Count - Reverse().IndexOf(item, index, count, equalityComparer) - 1;

        /// <summary>
        /// Remove an item from the list
        /// </summary>
        [Pure]
        public LstInternal<A> Remove(A value) => 
            Remove(value, EqualityComparer<A>.Default);

        /// <summary>
        /// Remove all items that match `value` from the list
        /// </summary>
        [Pure]
        public LstInternal<A> Remove(A value, IEqualityComparer<A> equalityComparer) =>
            Wrap(ListModule.Remove(Root, value, equalityComparer));

        /// <summary>
        /// Remove all items that match a predicate
        /// </summary>
        [Pure]
        public LstInternal<A> RemoveAll(Func<A, bool> pred) =>
            Wrap(ListModule.Remove(Root, pred));

        /// <summary>
        /// Remove item at location
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        [Pure]
        public LstInternal<A> RemoveAt(int index)
        {
            if (index < 0 || index >= Root.Count) throw new IndexOutOfRangeException();
            return Wrap(ListModule.Remove(Root, index));
        }

        /// <summary>
        /// Remove a range of items
        /// </summary>
        [Pure]
        public LstInternal<A> RemoveRange(int index, int count)
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
        public LstInternal<A> SetItem(int index, A value)
        {
            if (isnull(value)) throw new ArgumentNullException(nameof(value));
            if (index < 0 || index >= Root.Count) throw new IndexOutOfRangeException();
            return new LstInternal<A>(ListModule.SetItem(Root, value, index));
        }

        [Pure]
        IEnumerator IEnumerable.GetEnumerator() =>
            new ListModule.ListEnumerator<A>(Root, false, 0);

        [Pure]
        IEnumerator<A> IEnumerable<A>.GetEnumerator() =>
            new ListModule.ListEnumerator<A>(Root, false, 0);

        [Pure]
        public IEnumerable<A> Skip(int amount)
        {
            var iter = new ListModule.ListEnumerator<A>(Root, false, amount);
            while (iter.MoveNext())
            {
                yield return iter.Current;
            }
        }

        /// <summary>
        /// Reverse the order of the items in the list
        /// </summary>
        [Pure]
        public LstInternal<A> Reverse() =>
            new LstInternal<A>(this.AsEnumerable().Reverse());

        /// <summary>
        /// Fold
        /// </summary>
        [Pure]
        public S Fold<S>(S state, Func<S, A, S> folder)
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
        public LstInternal<U> Map<U>(Func<A, U> map) =>
            new LstInternal<U>(ListModule.Map(Root, map));

        [Pure]
        public IEnumerable<A> FindRange(int index, int count)
        {
            if (index < 0 || index >= Count) throw new ArgumentOutOfRangeException(nameof(index));
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(index));
            var iter = new ListModule.ListEnumerator<A>(Root, false, index, count);
            while (iter.MoveNext())
            {
                yield return iter.Current;
            }
        }

        /// <summary>
        /// Filter
        /// </summary>
        [Pure]
        public LstInternal<A> Filter(Func<A, bool> pred)
        {
            IEnumerable<A> Yield()
            {
                foreach (var item in this)
                {
                    if (pred(item))
                    {
                        yield return item;
                    }
                }
            }
            return new LstInternal<A>(Yield());
        }

        [Pure]
        public static LstInternal<A> operator +(LstInternal<A> lhs, A rhs) =>
            lhs.Add(rhs);

        [Pure]
        public static LstInternal<A> operator +(A rhs, LstInternal<A> lhs) =>
            new LstInternal<A>(rhs.Cons(lhs));

        [Pure]
        public static LstInternal<A> operator +(LstInternal<A> lhs, LstInternal<A> rhs) =>
            lhs.Append(rhs);

        [Pure]
        public LstInternal<A> Append(LstInternal<A> rhs) =>
            AddRange(rhs);

        [Pure]
        public static LstInternal<A> operator -(LstInternal<A> lhs, LstInternal<A> rhs) =>
            lhs.Subtract(rhs);

        [Pure]
        public LstInternal<A> Subtract(LstInternal<A> rhs)
        {
            var self = this;
            foreach (var item in rhs)
            {
                self = self.Remove(item);
            }
            return self;
        }

        [Pure]
        public override bool Equals(object obj) =>
            !ReferenceEquals(obj,null) && 
            obj is LstInternal<A> && 
            Equals((LstInternal<A>)obj);

        /// <summary>
        /// Get the hash code
        /// Lazily (and once only) calculates the hash from the elements in the list
        /// Empty list hash == 0
        /// </summary>
        [Pure]
        public override int GetHashCode()
        {
            if (hashCode != 0) return hashCode;
            return hashCode = hash(this.AsEnumerable());
        }

        [Pure]
        public bool Equals(LstInternal<A> other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (ReferenceEquals(other, null)) return false;
            var comparer = EqualityComparer<A>.Default;
            return Count == other.Count && this.Zip(other, (x, y) => comparer.Equals(x, y)).ForAll(x => x);
        }

        [Pure]
        public static bool operator ==(LstInternal<A> lhs, LstInternal<A> rhs) =>
            lhs.Equals(rhs);

        [Pure]
        public static bool operator !=(LstInternal<A> lhs, LstInternal<A> rhs) =>
            !lhs.Equals(rhs);

        [Pure]
        public int CompareTo(LstInternal<A> other)
        {
            var cmp = Count.CompareTo(other.Count);
            if (cmp != 0) return cmp;
            var iterA = GetEnumerator();
            var iterB = other.GetEnumerator();
            while (iterA.MoveNext() && iterB.MoveNext())
            {
                cmp = default(OrdDefault<A>).Compare(iterA.Current, iterB.Current);
                if (cmp != 0) return cmp;
            }
            return 0;
        }

        [Pure]
        public int CompareTo<OrdA>(LstInternal<A> other) where OrdA : struct, Ord<A>
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
    }

    [Serializable]
    class ListItem<T>
    {
        public static readonly ListItem<T> Empty = new ListItem<T>(0, 0, null, default(T), null);

        public bool IsEmpty => Count == 0;
        public int Count;
        public byte Height;
        public ListItem<T> Left;
        public ListItem<T> Right;

        /// <summary>
        /// Ctor
        /// </summary>
        internal ListItem(byte height, int count, ListItem<T> left, T key, ListItem<T> right)
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
                : ((int)Right.Height) - ((int)Left.Height);

        public T Key
        {
            get;
            internal set;
        }

        public bool IsBalanced =>
            (uint)(BalanceFactor + 1) <= 2;

        public override string ToString() =>
            IsEmpty
                ? "(empty)"
                : Key.ToString();
    }

    internal static class ListModuleM
    {
        public static ListItem<A> InsertMany<A>(ListItem<A> node, IEnumerable<A> items, int index, Pred<A> pred)
        {
            var root = ListItem<A>.Empty;

            var subIndex = 0;
            foreach (var item in items)
            {
                if (!pred.True(item)) throw new ArgumentOutOfRangeException("item in items");
                root = Insert(root, new ListItem<A>(1, 1, ListItem<A>.Empty, item, ListItem<A>.Empty), subIndex);
                subIndex++;
            }
            return Insert(node, root, index);
        }

        public static ListItem<A> InsertMany<A>(ListItem<A> node, IEnumerable<A> items, int index)
        {
            var root = ListItem<A>.Empty;

            var subIndex = 0;
            foreach (var item in items)
            {
                root = Insert(root, new ListItem<A>(1, 1, ListItem<A>.Empty, item, ListItem<A>.Empty), subIndex);
                subIndex++;
            }
            return Insert(node, root, index);
        }

        public static ListItem<A> Insert<A>(ListItem<A> node, ListItem<A> insertNode, int index)
        {
            if (node.IsEmpty)
            {
                return insertNode;
            }
            else if (index == node.Left.Count)
            {
                insertNode.Left = node.Left;
                insertNode = Balance(insertNode);

                //var insertedLeft = Balance(Make(insertNode.Key, node.Left, ListItem<A>.Empty));

                node.Left = insertNode;
                node = Balance(node);

                //var newThis = Balance(Make(node.Key, insertedLeft, node.Right)); 

                return node;
            }
            else if (index < node.Left.Count)
            {
                node.Left = Insert(node.Left, insertNode, index);
                return Balance(node);
            }
            else
            {
                node.Right = Insert(node.Right, insertNode, index - node.Left.Count - 1);
                return Balance(node);
            }
        }

        public static ListItem<T> Balance<T>(ListItem<T> node)
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

        public static ListItem<T> DblRotRight<T>(ListItem<T> node)
        {
            node.Left = RotLeft(node.Left);
            return RotRight(node);
        }

        public static ListItem<T> DblRotLeft<T>(ListItem<T> node)
        {
            node.Right = RotRight(node.Right);
            return RotLeft(node);
        }

        public static ListItem<A> RotRight<A>(ListItem<A> node)
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

        public static ListItem<A> RotLeft<A>(ListItem<A> node)
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

        public static bool ForAll<A>(ListItem<A> node, Func<A, bool> pred) =>
            node.IsEmpty
                ? true
                : pred(node.Key)
                    ? ForAll(node.Left, pred) && ForAll(node.Right, pred)
                    : false;

        public static bool Exists<T>(ListItem<T> node, Func<T, bool> pred) =>
            node.IsEmpty
                ? false
                : pred(node.Key)
                    ? true
                    : Exists(node.Left, pred) || Exists(node.Right, pred);

        public static ListItem<U> Map<T, U>(ListItem<T> node, Func<T, U> f) =>
            node.IsEmpty
                ? ListItem<U>.Empty
                : new ListItem<U>(node.Height, node.Count, Map(node.Left, f), f(node.Key), Map(node.Right, f));

        public static ListItem<T> Insert<T>(ListItem<T> node, T key, int index)
        {
            if (node.IsEmpty)
            {
                return new ListItem<T>(1, 1, ListItem<T>.Empty, key, ListItem<T>.Empty);
            }
            else if (index == node.Left.Count)
            {
                var insertedLeft = Balance(Make(key, node.Left, ListItem<T>.Empty));
                var newThis = Balance(Make(node.Key, insertedLeft, node.Right));
                return newThis;
            }
            else if (index < node.Left.Count)
            {
                return Balance(Make(node.Key, Insert(node.Left, key, index), node.Right));
            }
            else
            {
                return Balance(Make(node.Key, node.Left, Insert(node.Right, key, index - node.Left.Count - 1)));
            }
        }

        public static ListItem<T> Insert<T>(ListItem<T> node, ListItem<T> insertNode, int index)
        {
            if (node.IsEmpty)
            {
                return insertNode;
            }
            else if (index == node.Left.Count)
            {
                var insertedLeft = Balance(Make(insertNode.Key, node.Left, ListItem<T>.Empty));
                var newThis = Balance(Make(node.Key, insertedLeft, node.Right));
                return newThis;
            }
            else if (index < node.Left.Count)
            {
                return Balance(Make(node.Key, Insert(node.Left, insertNode, index), node.Right));
            }
            else
            {
                return Balance(Make(node.Key, node.Left, Insert(node.Right, insertNode, index - node.Left.Count - 1)));
            }
        }

        public static ListItem<T> SetItem<T>(ListItem<T> node, T key, int index)
        {
            if (node.IsEmpty)
            {
                throw new ArgumentException("Index outside the bounds of the list");
            }

            if (index == node.Left.Count)
            {
                return new ListItem<T>(node.Height, node.Count, node.Left, key, node.Right);
            }
            else if (index < node.Left.Count)
            {
                return new ListItem<T>(node.Height, node.Count, SetItem(node.Left, key, index), node.Key, node.Right);
            }
            else
            {
                return new ListItem<T>(node.Height, node.Count, node.Left, node.Key, SetItem(node.Right, key, index - node.Left.Count - 1));
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


        public static ListItem<T> Remove<T>(ListItem<T> node, Func<T, bool> pred)
        {
            if (node.IsEmpty)
            {
                return node;
            }

            var result = node;

            var left = node.Left.IsEmpty ? node.Left : Remove(node.Left, pred);
            var right = node.Right.IsEmpty ? node.Right : Remove(node.Right, pred);

            if (pred(node.Key))
            {
                if (right.IsEmpty && left.IsEmpty)
                {
                    result = ListItem<T>.Empty;
                }
                else if (right.IsEmpty && !left.IsEmpty)
                {
                    result = left;
                }
                else if (!right.IsEmpty && left.IsEmpty)
                {
                    result = Balance(right);
                }
                else
                {
                    var next = right;
                    while (!next.Left.IsEmpty)
                    {
                        next = next.Left;
                    }

                    right = Remove(right, 0);
                    result = Balance(Make(next.Key, left, right));
                }
            }
            else
            {
                if (!ReferenceEquals(left, node.Left) || !ReferenceEquals(right, node.Right))
                {
                    result = Balance(Make(node.Key, left, right));
                }
            }

            return result.IsEmpty || result.IsBalanced ? result : Balance(result);
        }

        public static ListItem<T> Remove<T>(ListItem<T> node, T value, IEqualityComparer<T> compare)
        {
            if (node.IsEmpty)
            {
                return node;
            }

            var result = node;

            var left = node.Left.IsEmpty ? node.Left : Remove(node.Left, value, compare);
            var right = node.Right.IsEmpty ? node.Right : Remove(node.Right, value, compare);

            if (ReferenceEquals(node.Key, value) || compare.Equals(node.Key, value))
            {
                if (right.IsEmpty && left.IsEmpty)
                {
                    result = ListItem<T>.Empty;
                }
                else if (right.IsEmpty && !left.IsEmpty)
                {
                    result = left;
                }
                else if (!right.IsEmpty && left.IsEmpty)
                {
                    result = Balance(right);
                }
                else
                {
                    var next = right;
                    while (!next.Left.IsEmpty)
                    {
                        next = next.Left;
                    }

                    right = Remove(right, 0);
                    result = Balance(Make(next.Key, left, right));
                }
            }
            else
            {
                if(!ReferenceEquals(left, node.Left) || !ReferenceEquals(right, node.Right))
                {
                    result = Balance(Make(node.Key, left, right));
                }
            }

            return result.IsEmpty || result.IsBalanced ? result : Balance(result);
        }

        public static ListItem<T> Remove<T>(ListItem<T> node, int index)
        {
            if (node.IsEmpty)
            {
                return node;
            }

            var result = node;

            if (index == node.Left.Count)
            {
                if (node.Right.IsEmpty && node.Left.IsEmpty)
                {
                    result = ListItem<T>.Empty;
                }
                else if (node.Right.IsEmpty && !node.Left.IsEmpty)
                {
                    result = node.Left;
                }
                else if (!node.Right.IsEmpty && node.Left.IsEmpty)
                {
                    result = Balance(node.Right);
                }
                else
                {
                    var next = node.Right;
                    while (!next.Left.IsEmpty)
                    {
                        next = next.Left;
                    }

                    var right = Remove(node.Right, 0);
                    result = Balance(Make(next.Key, node.Left, right));
                }
            }
            else if (index < node.Left.Count)
            {
                var left = Remove(node.Left, index);
                result = Make(node.Key, left, node.Right);
            }
            else
            {
                var right = Remove(node.Right, index - node.Left.Count - 1);
                result = Make(node.Key, node.Left, right);
            }
            return result.IsEmpty || result.IsBalanced ? result : Balance(result);
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
            new ListItem<T>((byte)(1 + Math.Max(l.Height, r.Height)), l.Count + r.Count + 1, l, k, r);

        public static ListItem<T> Balance<T>(ListItem<T> node) =>
            node.BalanceFactor >= 2
                ? node.Right.BalanceFactor < 0
                    ? DblRotLeft(node)
                    : RotLeft(node)
                : node.BalanceFactor <= -2
                    ? node.Left.BalanceFactor > 0
                        ? DblRotRight(node)
                        : RotRight(node)
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
            internal struct NewStack : New<ListItem<T>[]>
            {
                public ListItem<T>[] New() =>
                    new ListItem<T>[32];
            }

            ListItem<T>[] stack = null;
            int stackDepth;
            ListItem<T> map;
            int left;
            bool rev;
            int start;
            int count;

            public ListEnumerator(ListItem<T> root, bool rev, int start, int count = Int32.MaxValue)
            {
                this.rev = rev;
                this.start = start;
                map = root;
                stack = Pool<NewStack, ListItem<T>[]>.Pop();
                this.count = count;
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
                    Pool<NewStack, ListItem<T>[]>.Push(stack);
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
                    stack[stackDepth] = node;
                    stackDepth++;
                    node = Prev(node);
                }
            }

            public bool MoveNext()
            {
                if (count > 0 && left > 0 && stackDepth > 0)
                {
                    stackDepth--;
                    NodeCurrent = stack[stackDepth];
                    Push(Next(NodeCurrent));
                    left--;
                    count--;
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

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// Immutable list
    /// </summary>
    /// <typeparam name="A">Value type</typeparam>
    [Serializable]
    public struct Lst<A> :
        IEnumerable<A>, 
        IReadOnlyList<A>,
        IReadOnlyCollection<A>
    {
        /// <summary>
        /// Empty list
        /// </summary>
        public static readonly Lst<A> Empty = new Lst<A>(new A[0] { });

        readonly LstInternal<A> value;

        internal LstInternal<A> Value => value ?? LstInternal<A>.Empty;

        /// <summary>
        /// Ctor
        /// </summary>
        public Lst(IEnumerable<A> initial)
        {
            value = new LstInternal<A>(initial);
        }

        /// <summary>
        /// Ctor
        /// </summary>
        internal Lst(ListItem<A> root, bool rev)
        {
            value = new LstInternal<A>(root, rev);
        }

        private ListItem<A> Root
        {
            get
            {
                return Value.Root ?? ListItem<A>.Empty;
            }
        }

        /// <summary>
        /// Index accessor
        /// </summary>
        [Pure]
        public A this[int index]
        {
            get
            {
                if (index < 0 || index >= Root.Count) throw new IndexOutOfRangeException();
                return ListModule.GetItem(Root, Value.Rev ? Count - index - 1 : index);
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
                return ListModule.GetItem(Root, Rev ? Count - index - 1 : index);
            }
        }

        internal bool Rev => Value.Rev;

        /// <summary>
        /// Add an item to the end of the list
        /// </summary>
        [Pure]
        public Lst<A> Add(A value) =>
            Value.Add(value);

        /// <summary>
        /// Add a range of items to the end of the list
        /// </summary>
        [Pure]
        public Lst<A> AddRange(IEnumerable<A> items) =>
            Value.AddRange(items);

        /// <summary>
        /// Clear the list
        /// </summary>
        [Pure]
        public Lst<A> Clear() =>
            Empty;

        /// <summary>
        /// Get enumerator
        /// </summary>
        [Pure]
        public IEnumerator<A> GetEnumerator() =>
            new ListModule.ListEnumerator<A>(Root,Rev,0);

        /// <summary>
        /// Find the index of an item
        /// </summary>
        [Pure]
        public int IndexOf(A item, int index = 0, int count = -1, IEqualityComparer<A> equalityComparer = null) =>
            Value.IndexOf(item, index, count, equalityComparer);

        /// <summary>
        /// Insert value at specified index
        /// </summary>
        [Pure]
        public Lst<A> Insert(int index, A value) =>
            Value.Insert(index, value);

        /// <summary>
        /// Insert range of values at specified index
        /// </summary>
        [Pure]
        public Lst<A> InsertRange(int index, IEnumerable<A> items) =>
            Value.InsertRange(index, items);

        /// <summary>
        /// Find the last index of an item in the list
        /// </summary>
        [Pure]
        public int LastIndexOf(A item, int index = 0, int count = -1, IEqualityComparer<A> equalityComparer = null) =>
            Value.LastIndexOf(item, index, count, equalityComparer);

        /// <summary>
        /// Remove an item from the list
        /// </summary>
        [Pure]
        public Lst<A> Remove(A value) =>
            Value.Remove(value);

        /// <summary>
        /// Remove an item from the list
        /// </summary>
        [Pure]
        public Lst<A> Remove(A value, IComparer<A> equalityComparer) =>
            Value.Remove(value, equalityComparer);

        /// <summary>
        /// Remove all items that match a predicate
        /// </summary>
        [Pure]
        public Lst<A> RemoveAll(Predicate<A> pred) =>
            Value.RemoveAll(pred);

        /// <summary>
        /// Remove item at location
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        [Pure]
        public Lst<A> RemoveAt(int index) =>
            Value.RemoveAt(index);

        /// <summary>
        /// Remove a range of items
        /// </summary>
        [Pure]
        public Lst<A> RemoveRange(int index, int count) =>
            Value.RemoveRange(index, count);

        /// <summary>
        /// Set an item at the specified index
        /// </summary>
        [Pure]
        public Lst<A> SetItem(int index, A value) =>
            Value.SetItem(index, value);

        [Pure]
        IEnumerator IEnumerable.GetEnumerator() =>
            new ListModule.ListEnumerator<A>(Root, Rev, 0);

        [Pure]
        IEnumerator<A> IEnumerable<A>.GetEnumerator() =>
            new ListModule.ListEnumerator<A>(Root, Rev, 0);

        [Pure]
        public IEnumerable<A> Skip(int amount) =>
            Value.Skip(amount);

        /// <summary>
        /// Reverse the order of the items in the list
        /// </summary>
        [Pure]
        public Lst<A> Reverse() =>
            Value.Reverse();

        /// <summary>
        /// Fold
        /// </summary>
        [Pure]
        public S Fold<S>(S state, Func<S, A, S> folder) =>
            Value.Fold(state, folder);

        /// <summary>
        /// Map
        /// </summary>
        [Pure]
        public Lst<U> Map<U>(Func<A, U> map) =>
            Value.Map(map);

        /// <summary>
        /// Filter
        /// </summary>
        [Pure]
        public Lst<A> Filter(Func<A, bool> pred) =>
            Value.Filter(pred);

        [Pure]
        public static Lst<A> operator +(Lst<A> lhs, A rhs) =>
            lhs.Add(rhs);

        [Pure]
        public static Lst<A> operator +(A lhs, Lst<A> rhs) =>
            lhs.Cons(rhs);

        [Pure]
        public static Lst<A> operator +(Lst<A> lhs, Lst<A> rhs) =>
            lhs.Append(rhs);

        [Pure]
        public Lst<A> Append(Lst<A> rhs) =>
            Value.Append(rhs);

        [Pure]
        public static Lst<A> operator -(Lst<A> lhs, Lst<A> rhs) =>
            lhs.Subtract(rhs);

        [Pure]
        public Lst<A> Subtract(Lst<A> rhs) =>
            Value.Subtract(rhs);

        [Pure]
        public override bool Equals(object obj) =>
            obj is Lst<A> && Equals((Lst<A>)obj);

        /// <summary>
        /// Get the hash code
        /// Lazily (and once only) calculates the hash from the elements in the list
        /// Empty list hash == 0
        /// </summary>
        [Pure]
        public override int GetHashCode() =>
            Value.GetHashCode();

        [Pure]
        public bool Equals(Lst<A> other) =>
            Value.Equals(other.Value);

        [Pure]
        public static bool operator ==(Lst<A> lhs, Lst<A> rhs) =>
            lhs.Value.Equals(rhs.Value);

        [Pure]
        public static bool operator !=(Lst<A> lhs, Lst<A> rhs) =>
            !(lhs == rhs);

        [Pure]
        public Arr<A> ToArray() =>
            toArray(this);
   }
}
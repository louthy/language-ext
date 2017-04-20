using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;
using LanguageExt.TypeClasses;
using LanguageExt.ClassInstances.Pred;

namespace LanguageExt
{
    /// <summary>
    /// Immutable list with validation predicate
    /// </summary>
    /// <typeparam name="PRED">Predicate instance to run when the type is constructed</typeparam>
    /// <typeparam name="A">Value type</typeparam>
    [Serializable]
    public struct Lst<PRED, A> :
        IEnumerable<A>, 
        IReadOnlyList<A>,
        IReadOnlyCollection<A>,
        IEquatable<Lst<PRED, A>>,
        ListInfo
        where PRED : struct, Pred<ListInfo>
    {
        readonly LstInternal<A> value;

        /// <summary>
        /// Ctor
        /// </summary>
        public Lst(IEnumerable<A> initial)
        {
            if (initial == null) throw new NullReferenceException(nameof(initial));
            value = new LstInternal<A>(initial, default(True<A>));
            if (!default(PRED).True(this)) throw new ArgumentOutOfRangeException(nameof(value));
        }

        /// <summary>
        /// Ctor
        /// </summary>
        Lst(ListItem<A> root, bool rev)
        {
            if (root == null) throw new NullReferenceException(nameof(root));
            value = new LstInternal<A>(root, rev);
            if (!default(PRED).True(this)) throw new ArgumentOutOfRangeException(nameof(value));
        }

        /// <summary>
        /// Ctor
        /// </summary>
        Lst(LstInternal<A> root)
        {
            value = root;
            if (root == null) throw new NullReferenceException(nameof(root));
            if (!default(PRED).True(this)) throw new ArgumentOutOfRangeException(nameof(value));
        }

        LstInternal<A> Value
        {
            get
            {
                if (value.IsNull())
                {
                    throw new BottomException();
                }
                return value;
            }
        }

        ListItem<A> Root =>
            Value.Root;

        Lst<PRED, A> Wrap(LstInternal<A> list) =>
            new Lst<PRED, A>(list);

        static Lst<PRED, T> Wrap<T>(LstInternal<T> list) =>
            new Lst<PRED, T>(list);

        /// <summary>
        /// Index accessor
        /// </summary>
        [Pure]
        public A this[int index]
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

        internal bool Rev => value?.Rev ?? false;

        /// <summary>
        /// Add an item to the end of the list
        /// </summary>
        [Pure]
        public Lst<PRED, A> Add(A value) =>
            Wrap(Value.Add(value));

        /// <summary>
        /// Add a range of items to the end of the list
        /// </summary>
        [Pure]
        public Lst<PRED, A> AddRange(IEnumerable<A> items) =>
            Wrap(Value.AddRange(items));

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
        public Lst<PRED, A> Insert(int index, A value) =>
            Wrap(Value.Insert(index, value));

        /// <summary>
        /// Insert range of values at specified index
        /// </summary>
        [Pure]
        public Lst<PRED, A> InsertRange(int index, IEnumerable<A> items) =>
            Wrap(Value.InsertRange(index, items, default(True<A>)));

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
        public Lst<PRED, A> Remove(A value) =>
            Wrap(Value.Remove(value));

        /// <summary>
        /// Remove an item from the list
        /// </summary>
        [Pure]
        public Lst<PRED, A> Remove(A value, IComparer<A> equalityComparer) =>
            Wrap(Value.Remove(value, equalityComparer));

        /// <summary>
        /// Remove all items that match a predicate
        /// </summary>
        [Pure]
        public Lst<PRED, A> RemoveAll(Predicate<A> pred) =>
            Wrap(Value.RemoveAll(pred));

        /// <summary>
        /// Remove item at location
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        [Pure]
        public Lst<PRED, A> RemoveAt(int index) =>
            Wrap(Value.RemoveAt(index));

        /// <summary>
        /// Remove a range of items
        /// </summary>
        [Pure]
        public Lst<PRED, A> RemoveRange(int index, int count) =>
            Wrap(Value.RemoveRange(index, count));

        /// <summary>
        /// Set an item at the specified index
        /// </summary>
        [Pure]
        public Lst<PRED, A> SetItem(int index, A value) =>
            Wrap(Value.SetItem(index, value));

        [Pure]
        IEnumerator IEnumerable.GetEnumerator() =>
            new ListModule.ListEnumerator<A>(Root, Rev, 0);

        [Pure]
        IEnumerator<A> IEnumerable<A>.GetEnumerator() =>
            new ListModule.ListEnumerator<A>(Root, Rev, 0);

        [Pure]
        public Seq<A> ToSeq() =>
            Seq(this);

        [Pure]
        public IEnumerable<A> AsEnumerable() =>
            this;

        [Pure]
        public IEnumerable<A> Skip(int amount) =>
            Value.Skip(amount);

        /// <summary>
        /// Reverse the order of the items in the list
        /// </summary>
        [Pure]
        public Lst<PRED, A> Reverse() =>
            Wrap(Value.Reverse());

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
        public Lst<PRED, U> Map<U>(Func<A, U> map) =>
            Wrap(Value.Map(map));

        /// <summary>
        /// Filter
        /// </summary>
        [Pure]
        public Lst<PRED, A> Filter(Func<A, bool> pred) =>
            Wrap(Value.Filter(pred));

        [Pure]
        public static Lst<PRED, A> operator +(Lst<PRED, A> lhs, A rhs) =>
            lhs.Add(rhs);

        [Pure]
        public static Lst<PRED, A> operator +(A lhs, Lst<PRED, A> rhs) =>
            new Lst<PRED, A>(lhs.Cons(rhs));

        [Pure]
        public static Lst<PRED, A> operator +(Lst<PRED, A> lhs, Lst<PRED, A> rhs) =>
            lhs.Append(rhs);

        [Pure]
        public Lst<PRED, A> Append(Lst<PRED, A> rhs) =>
            new Lst<PRED, A>(Value.Append(rhs));

        [Pure]
        public static Lst<PRED, A> operator -(Lst<PRED, A> lhs, Lst<PRED, A> rhs) =>
            lhs.Subtract(rhs);

        [Pure]
        public Lst<PRED, A> Subtract(Lst<PRED, A> rhs) =>
            Wrap(Value.Subtract(rhs.Value));

        [Pure]
        public override bool Equals(object obj) =>
            obj is Lst<PRED, A> && Equals((Lst<PRED, A>)obj);

        /// <summary>
        /// Get the hash code
        /// Lazily (and once only) calculates the hash from the elements in the list
        /// Empty list hash == 0
        /// </summary>
        [Pure]
        public override int GetHashCode() =>
            Value.GetHashCode();

        [Pure]
        public bool Equals(Lst<PRED, A> other) =>
            Value.Equals(other.Value);

        [Pure]
        public static bool operator ==(Lst<PRED, A> lhs, Lst<PRED, A> rhs) =>
            lhs.Value.Equals(rhs.Value);

        [Pure]
        public static bool operator !=(Lst<PRED, A> lhs, Lst<PRED, A> rhs) =>
            !(lhs == rhs);

        [Pure]
        public Arr<A> ToArray() =>
            toArray(this);

        [Pure]
        public static implicit operator Lst<PRED, A>(Lst<A> list) =>
            new Lst<PRED, A>(list.Value);

        [Pure]
        public static implicit operator Lst<A>(Lst<PRED, A> list) =>
            new Lst<A>(list.Value);
    }
}
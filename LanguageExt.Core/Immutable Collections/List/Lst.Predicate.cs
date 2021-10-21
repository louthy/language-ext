using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;
using LanguageExt.TypeClasses;
using LanguageExt.ClassInstances.Pred;
using LanguageExt.ClassInstances;
using System.Runtime.CompilerServices;

namespace LanguageExt
{
    /// <summary>
    /// Immutable list with validation predicate
    /// </summary>
    /// <typeparam name="PRED">Predicate instance to run when the type is constructed</typeparam>
    /// <typeparam name="A">Value type</typeparam>
    [Serializable]
    public class Lst<PRED, A> :
        IEnumerable<A>, 
        IReadOnlyList<A>,
        IReadOnlyCollection<A>,
        IEquatable<Lst<PRED, A>>,
        IComparable<Lst<PRED, A>>,
        IComparable
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
            if (!default(PRED).True(value)) throw new ArgumentOutOfRangeException(nameof(value));
        }

        /// <summary>
        /// Ctor
        /// </summary>
        Lst(LstInternal<A> root)
        {
            value = root;
            if (root == null) throw new NullReferenceException(nameof(root));
            if (!default(PRED).True(value)) throw new ArgumentOutOfRangeException(nameof(value));
        }

        internal LstInternal<A> Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => value ?? LstInternal<A>.Empty;
        }

        ListItem<A> Root
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Value.Root;
        }

        [Pure]
        public bool IsEmpty =>
            Count == 0;

        /// <summary>
        /// Reference version for use in pattern-matching
        /// </summary>
        /// <remarks>
        ///
        ///     Empty collection     = null
        ///     Singleton collection = A
        ///     More                 = (A, Seq<A>)   -- head and tail
        ///
        ///     var res = list.Case switch
        ///     {
        ///       
        ///        A value         => ...,
        ///        (var x, var xs) => ...,
        ///        _               => ...
        ///     }
        /// 
        /// </remarks>
        [Pure]
        public object Case =>
            IsEmpty 
                ? null
                : Count == 1
                    ? this[0]
                    : toSeq(this).Case;

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
        /// Find if a value is in the collection
        /// </summary>
        /// <param name="value">Value to test</param>
        /// <returns>True if collection contains value</returns>
        [Pure]
        public bool Contains(A value) =>
            Value.Find(a => default(EqDefault<A>).Equals(a, value)).IsSome;

        /// <summary>
        /// Contains with provided Eq class instance
        /// </summary>
        /// <typeparam name="EqA">Eq class instance</typeparam>
        /// <param name="value">Value to test</param>
        /// <returns>True if collection contains value</returns>
        [Pure]
        public bool Contains<EqA>(A value) where EqA : struct, Eq<A> =>
            Value.Find(a => default(EqA).Equals(a, value)).IsSome;

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
            new ListEnumerator<A>(Root, false, 0);

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
        /// Remove all items that match the value from the list
        /// </summary>
        [Pure]
        public Lst<PRED, A> Remove(A value) =>
            Wrap(Value.Remove(value));

        /// <summary>
        /// Remove all items that match the value from the list
        /// </summary>
        [Pure]
        public Lst<PRED, A> Remove(A value, IEqualityComparer<A> equalityComparer) =>
            Wrap(Value.Remove(value, equalityComparer));

        /// <summary>
        /// Remove all items that match a predicate
        /// </summary>
        [Pure]
        public Lst<PRED, A> RemoveAll(Func<A, bool> pred) =>
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

        /// <summary>
        /// Returns an enumerable range from the collection.  This is the fastest way of
        /// iterating sub-ranges of the collection.
        /// </summary>
        /// <param name="index">Index into the collection</param>
        /// <param name="count">Number of items to find</param>
        /// <returns>IEnumerable of items</returns>
        [Pure]
        public IEnumerable<A> FindRange(int index, int count) =>
            Value.FindRange(index, count);

        [Pure]
        IEnumerator IEnumerable.GetEnumerator() =>
            new ListEnumerator<A>(Root, false, 0);

        [Pure]
        IEnumerator<A> IEnumerable<A>.GetEnumerator() =>
            new ListEnumerator<A>(Root, false, 0);

        [Pure]
        public Seq<A> ToSeq() =>
            toSeq(this);

        /// <summary>
        /// Format the collection as `[a, b, c, ...]`
        /// The elipsis is used for collections over 50 items
        /// To get a formatted string with all the items, use `ToFullString`
        /// or `ToFullArrayString`.
        /// </summary>
        [Pure]
        public override string ToString() =>
            CollectionFormat.ToShortArrayString(this, Count);

        /// <summary>
        /// Format the collection as `a, b, c, ...`
        /// </summary>
        [Pure]
        public string ToFullString(string separator = ", ") =>
            CollectionFormat.ToFullString(this, separator);

        /// <summary>
        /// Format the collection as `[a, b, c, ...]`
        /// </summary>
        [Pure]
        public string ToFullArrayString(string separator = ", ") =>
            CollectionFormat.ToFullArrayString(this, separator);

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
        public override bool Equals(object obj) => obj switch 
        {
            Lst<PRED, A>   s => Equals(s),
            IEnumerable<A> e => Equals(new Lst<PRED, A>(e)),
            _                => false
        };

        /// <summary>
        /// Get the hash code
        /// Lazily (and once only) calculates the hash from the elements in the list
        /// Empty list hash == 0
        /// </summary>
        [Pure]
        public override int GetHashCode() =>
            Value.GetHashCode();

        [Pure]
        public int CompareTo(object obj) => obj switch 
        {
            Lst<PRED, A>   s => CompareTo(s),
            IEnumerable<A> e => CompareTo(new Lst<PRED, A>(e)),
            _                => 1
        };

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
        public static bool operator <(Lst<PRED, A> lhs, Lst<PRED, A> rhs) =>
            lhs.CompareTo(rhs) < 0;

        [Pure]
        public static bool operator <=(Lst<PRED, A> lhs, Lst<PRED, A> rhs) =>
            lhs.CompareTo(rhs) <= 0;

        [Pure]
        public static bool operator >(Lst<PRED, A> lhs, Lst<PRED, A> rhs) =>
            lhs.CompareTo(rhs) > 0;

        [Pure]
        public static bool operator >=(Lst<PRED, A> lhs, Lst<PRED, A> rhs) =>
            lhs.CompareTo(rhs) >= 0;

        [Pure]
        public Arr<A> ToArray() =>
            toArray(this);

        [Pure]
        public int CompareTo(Lst<PRED, A> other) =>
            Value.CompareTo(other.Value);

        [Pure]
        public static implicit operator Lst<PRED, A>(Lst<A> list) =>
            new Lst<PRED, A>(list.Value);

        [Pure]
        public static implicit operator Lst<A>(Lst<PRED, A> list) =>
            new Lst<A>(list.Value);
    }
}

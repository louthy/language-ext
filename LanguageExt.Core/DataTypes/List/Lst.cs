using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;
using LanguageExt.ClassInstances.Pred;
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using System.Runtime.CompilerServices;

namespace LanguageExt
{
    /// <summary>
    /// Immutable list
    /// </summary>
    /// <typeparam name="A">Value type</typeparam>
    [Serializable]
    public struct Lst<A> :
        IEnumerable<A>,
        IComparable<Lst<A>>,
        IReadOnlyList<A>,
        IReadOnlyCollection<A>,
        IEquatable<Lst<A>>
    {
        /// <summary>
        /// Empty list
        /// </summary>
        public static readonly Lst<A> Empty = new Lst<A>(new A[0] { });

        readonly LstInternal<A> value;

        internal LstInternal<A> Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => value ?? LstInternal<A>.Empty;
        }

        /// <summary>
        /// Ctor
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Lst(IEnumerable<A> initial) =>
            value = new LstInternal<A>(initial);

        /// <summary>
        /// Ctor
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        Lst(LstInternal<A> initial) =>
            value = initial;

        /// <summary>
        /// Ctor
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Lst(ListItem<A> root) =>
            value = new LstInternal<A>(root);

        ListItem<A> Root
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Value.Root;
        }

        [Pure]
        public bool IsEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Count == 0;
        }

        /// <summary>
        /// Reference version for use in pattern-matching
        /// </summary>
        [Pure]
        public SeqCase<A> Case
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Seq(Value).Case;
        }

        /// <summary>
        /// Head lens
        /// </summary>
        [Pure]
        public static Lens<Lst<A>, A> head
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Lens<Lst<A>, A>.New(
                Get: la => la.Count == 0 ? throw new IndexOutOfRangeException() : la[0],
                Set: a => la => la.Count == 0 ? throw new IndexOutOfRangeException() : la.SetItem(0, a));
        }

        /// <summary>
        /// Head or none lens
        /// </summary>
        [Pure]
        public static Lens<Lst<A>, Option<A>> headOrNone
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Lens<Lst<A>, Option<A>>.New(
                Get: la => la.Count == 0 ? None : Some(la[0]),
                Set: a => la => la.Count == 0 || a.IsNone ? la : la.SetItem(0, a.Value));
        }

        /// <summary>
        /// Tail lens
        /// </summary>
        [Pure]
        public static Lens<Lst<A>, A> tail
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Lens<Lst<A>, A>.New(
                Get: la => la.Count == 0 ? throw new IndexOutOfRangeException() : la[la.Count - 1],
                Set: a => la => la.Count == 0 ? throw new IndexOutOfRangeException() : la.SetItem(la.Count - 1, a));
        }

        /// <summary>
        /// Tail or none lens
        /// </summary>
        [Pure]
        public static Lens<Lst<A>, Option<A>> tailOrNone
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Lens<Lst<A>, Option<A>>.New(
                Get: la => la.Count == 0 ? None : Some(la[la.Count - 1]),
                Set: a => la => la.Count == 0 || a.IsNone ? la : la.SetItem(la.Count - 1, a.Value));
        }

        /// <summary>
        /// Item at index lens
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Lens<Lst<A>, A> item(int index) => Lens<Lst<A>, A>.New(
            Get: la => la.Count == 0 ? throw new IndexOutOfRangeException() : la[index],
            Set: a => la => la.Count == 0 ? throw new IndexOutOfRangeException() : la.SetItem(index, a)
            );

        /// <summary>
        /// Item or none at index lens
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Lens<Lst<A>, Option<A>> itemOrNone(int index) => Lens<Lst<A>, Option<A>>.New(
            Get: la => la.Count < index - 1 ? None : Some(la[index]),
            Set: a => la => la.Count < index - 1 || a.IsSome ? la : la.SetItem(index, a.Value)
            );

        /// <summary>
        /// Lens map
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Lens<Lst<A>, Lst<B>> map<B>(Lens<A, B> lens) => Lens<Lst<A>, Lst<B>>.New(
            Get: la => la.Map(lens.Get),
            Set: lb => la => la.Zip(lb).Map(ab => lens.Set(ab.Item2, ab.Item1)).Freeze()
            );

        /// <summary>
        /// Index accessor
        /// </summary>
        [Pure]
        public A this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Root.Count;
        }

        [Pure]
        int IReadOnlyCollection<A>.Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Count;
        }

        [Pure]
        A IReadOnlyList<A>.this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (index < 0 || index >= Root.Count) throw new IndexOutOfRangeException();
                return ListModule.GetItem(Root, index);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        Lst<A> Wrap(LstInternal<A> list) =>
            new Lst<A>(list);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Lst<T> Wrap<T>(LstInternal<T> list) =>
            new Lst<T>(list);

        /// <summary>
        /// Find if a value is in the collection
        /// </summary>
        /// <param name="value">Value to test</param>
        /// <returns>True if collection contains value</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(A value) =>
            Value.Find(a => default(EqDefault<A>).Equals(a,value)).IsSome;

        /// <summary>
        /// Contains with provided Eq class instance
        /// </summary>
        /// <typeparam name="EqA">Eq class instance</typeparam>
        /// <param name="value">Value to test</param>
        /// <returns>True if collection contains value</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains<EqA>(A value) where EqA : struct, Eq<A> =>
            Value.Find(a => default(EqA).Equals(a, value)).IsSome;

        /// <summary>
        /// Add an item to the end of the list
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Lst<A> Add(A value) =>
            Wrap(Value.Add(value));

        /// <summary>
        /// Add a range of items to the end of the list
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Lst<A> AddRange(IEnumerable<A> items) =>
            Wrap(Value.AddRange(items));

        /// <summary>
        /// Clear the list
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Lst<A> Clear() =>
            Empty;

        /// <summary>
        /// Get enumerator
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<A> GetEnumerator() =>
            new ListModule.ListEnumerator<A>(Root, false, 0);

        /// <summary>
        /// Find the index of an item
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(A item, int index = 0, int count = -1, IEqualityComparer<A> equalityComparer = null) =>
            Value.IndexOf(item, index, count, equalityComparer);

        /// <summary>
        /// Insert value at specified index
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Lst<A> Insert(int index, A value) =>
            Wrap(Value.Insert(index, value));

        /// <summary>
        /// Insert range of values at specified index
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Lst<A> InsertRange(int index, IEnumerable<A> items) =>
            Wrap(Value.InsertRange(index, items, default(True<A>)));

        /// <summary>
        /// Find the last index of an item in the list
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int LastIndexOf(A item, int index = 0, int count = -1, IEqualityComparer<A> equalityComparer = null) =>
            Value.LastIndexOf(item, index, count, equalityComparer);

        /// <summary>
        /// Remove all items that match the value from the list
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Lst<A> Remove(A value) =>
            Wrap(Value.Remove(value));

        /// <summary>
        /// Remove all items that match the value from the list
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Lst<A> Remove(A value, IEqualityComparer<A> equalityComparer) =>
            Wrap(Value.Remove(value, equalityComparer));

        /// <summary>
        /// Remove all items that match a predicate
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Lst<A> RemoveAll(Func<A, bool> pred) =>
            Wrap(Value.RemoveAll(pred));

        /// <summary>
        /// Remove item at location
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Lst<A> RemoveAt(int index) =>
            Wrap(Value.RemoveAt(index));

        /// <summary>
        /// Remove a range of items
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Lst<A> RemoveRange(int index, int count) =>
            Wrap(Value.RemoveRange(index, count));

        /// <summary>
        /// Set an item at the specified index
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Lst<A> SetItem(int index, A value) =>
            Wrap(Value.SetItem(index, value));

        /// <summary>
        /// Returns an enumerable range from the collection.  This is the fastest way of
        /// iterating sub-ranges of the collection.
        /// </summary>
        /// <param name="index">Index into the collection</param>
        /// <param name="count">Number of items to find</param>
        /// <returns>IEnumerable of items</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<A> FindRange(int index, int count) =>
            Value.FindRange(index, count);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() =>
            new ListModule.ListEnumerator<A>(Root, false, 0);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator<A> IEnumerable<A>.GetEnumerator() =>
            new ListModule.ListEnumerator<A>(Root, false, 0);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Seq<A> ToSeq() =>
            Seq(this);

        /// <summary>
        /// Format the collection as `[a, b, c, ...]`
        /// The elipsis is used for collections over 50 items
        /// To get a formatted string with all the items, use `ToFullString`
        /// or `ToFullArrayString`.
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString() =>
            CollectionFormat.ToShortArrayString(this, Count);

        /// <summary>
        /// Format the collection as `a, b, c, ...`
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToFullString(string separator = ", ") =>
            CollectionFormat.ToFullString(this, separator);

        /// <summary>
        /// Format the collection as `[a, b, c, ...]`
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToFullArrayString(string separator = ", ") =>
            CollectionFormat.ToFullArrayString(this, separator);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<A> AsEnumerable() =>
            this;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<A> Skip(int amount) =>
            Value.Skip(amount);

        /// <summary>
        /// Reverse the order of the items in the list
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Lst<A> Reverse() =>
            Wrap(Value.Reverse());

        /// <summary>
        /// Fold
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public S Fold<S>(S state, Func<S, A, S> folder) =>
            Value.Fold(state, folder);

        /// <summary>
        /// Impure iteration of the bound values in the structure
        /// </summary>
        /// <returns>
        /// Returns the original unmodified structure
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Lst<A> Do(Action<A> f)
        {
            this.Iter(f);
            return this;
        }

        /// <summary>
        /// Map
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Lst<U> Map<U>(Func<A, U> map) =>
            Wrap(Value.Map(map));

        /// <summary>
        /// Filter
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Lst<A> Filter(Func<A, bool> pred) =>
            Wrap(Value.Filter(pred));

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Lst<A> operator +(Lst<A> lhs, A rhs) =>
            lhs.Add(rhs);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Lst<A> operator +(A lhs, Lst<A> rhs) =>
            lhs.Cons(rhs);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Lst<A> operator +(Lst<A> lhs, Lst<A> rhs) =>
            lhs.Append(rhs);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Lst<A> Append(Lst<A> rhs) =>
            new Lst<A>(Value.Append(rhs));

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Lst<A> operator -(Lst<A> lhs, Lst<A> rhs) =>
            lhs.Subtract(rhs);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Lst<A> Subtract(Lst<A> rhs) =>
            Wrap(Value.Subtract(rhs.Value));

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj) =>
            obj is Lst<A> && Equals((Lst<A>)obj);

        /// <summary>
        /// Get the hash code
        /// Lazily (and once only) calculates the hash from the elements in the list
        /// Empty list hash == 0
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() =>
            Value.GetHashCode();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Lst<A> other) =>
            Value.Equals(other.Value);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Lst<A> lhs, Lst<A> rhs) =>
            lhs.Value.Equals(rhs.Value);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Lst<A> lhs, Lst<A> rhs) =>
            !(lhs == rhs);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(Lst<A> lhs, Lst<A> rhs) =>
            lhs.CompareTo(rhs) < 0;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(Lst<A> lhs, Lst<A> rhs) =>
            lhs.CompareTo(rhs) <= 0;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(Lst<A> lhs, Lst<A> rhs) =>
            lhs.CompareTo(rhs) > 0;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(Lst<A> lhs, Lst<A> rhs) =>
            lhs.CompareTo(rhs) >= 0;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Arr<A> ToArray() =>
            toArray(this);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(Lst<A> other) =>
            Value.CompareTo(other.Value);

        /// <summary>
        /// Implicit conversion from an untyped empty list
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Lst<A>(SeqEmpty _) =>
            Empty;
    }
}

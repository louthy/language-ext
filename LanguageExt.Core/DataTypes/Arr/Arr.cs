using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;
using LanguageExt.TypeClasses;
using LanguageExt.ClassInstances;
using System.Runtime.CompilerServices;

namespace LanguageExt
{
    /// <summary>
    /// Immutable array
    /// Native array O(1) read performance.  Modifications require copying of the entire 
    /// array to generate the newly mutated version.  This is will be very expensive 
    /// for large arrays.
    /// </summary>
    /// <typeparam name="A">Value type</typeparam>
    [Serializable]
    public struct Arr<A> :
        IEnumerable<A>,
        IReadOnlyList<A>,
        IReadOnlyCollection<A>,
        IEquatable<Arr<A>>,
        IComparable<Arr<A>>
    {
        /// <summary>
        /// Empty array
        /// </summary>
        public static readonly Arr<A> Empty = new Arr<A>(new A[0] { });
        readonly A[] value;
        int hashCode;

        internal A[] Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => value ?? Empty.Value;
        }

        /// <summary>
        /// Ctor
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Arr(IEnumerable<A> initial)
        {
            hashCode = 0;
            value = initial.ToArray();
        }

        /// <summary>
        /// Ctor
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Arr(A[] value)
        {
            hashCode = 0;
            this.value = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Arr<A>(A[] xs) =>
            new Arr<A>(xs);

        /// <summary>
        /// Head lens
        /// </summary>
        [Pure]
        public static Lens<Arr<A>, A> head => Lens<Arr<A>, A>.New(
            Get: la => la.Count == 0 ? throw new IndexOutOfRangeException() : la[0],
            Set: a => la => la.Count == 0 ? throw new IndexOutOfRangeException() : la.SetItem(0, a)
            );

        /// <summary>
        /// Head or none lens
        /// </summary>
        [Pure]
        public static Lens<Arr<A>, Option<A>> headOrNone => Lens<Arr<A>, Option<A>>.New(
            Get: la => la.Count == 0 ? None : Some(la[0]),
            Set: a => la => la.Count == 0 || a.IsNone ? la : la.SetItem(0, a.Value)
            );

        /// <summary>
        /// Last lens
        /// </summary>
        [Pure]
        public static Lens<Arr<A>, A> last => Lens<Arr<A>, A>.New(
            Get: la => la.Count == 0 ? throw new IndexOutOfRangeException() : la[la.Count - 1],
            Set: a => la => la.Count == 0 ? throw new IndexOutOfRangeException() : la.SetItem(la.Count - 1, a)
            );

        /// <summary>
        /// Last or none lens
        /// </summary>
        [Pure]
        public static Lens<Arr<A>, Option<A>> lastOrNone => Lens<Arr<A>, Option<A>>.New(
            Get: la => la.Count == 0 ? None : Some(la[la.Count - 1]),
            Set: a => la => la.Count == 0 || a.IsNone ? la : la.SetItem(la.Count - 1, a.Value)
            );

        /// <summary>
        /// Item at index lens
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Lens<Arr<A>, A> item(int index) => Lens<Arr<A>, A>.New(
            Get: la => la.Count == 0 ? throw new IndexOutOfRangeException() : la[index],
            Set: a => la => la.Count == 0 ? throw new IndexOutOfRangeException() : la.SetItem(index, a)
            );

        /// <summary>
        /// Item or none at index lens
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Lens<Arr<A>, Option<A>> itemOrNone(int index) => Lens<Arr<A>, Option<A>>.New(
            Get: la => la.Count < index - 1 ? None : Some(la[index]),
            Set: a => la => la.Count < index - 1 || a.IsSome ? la : la.SetItem(index, a.Value)
            );

        /// <summary>
        /// Lens map
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Lens<Arr<A>, Arr<B>> map<B>(Lens<A, B> lens) => Lens<Arr<A>, Arr<B>>.New(
            Get: la => la.Map(lens.Get),
            Set: lb => la => la.Zip(lb).Map(ab => lens.Set(ab.Item2, ab.Item1)).ToArr()
            );

        /// <summary>
        /// Index accessor
        /// </summary>
        [Pure]
        public A this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Value[index];
        }
        /// <summary>
        /// Reference version for use in pattern-matching
        /// </summary>
        [Pure]
        public SeqCase<A> Case
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Seq.FromArray(Value).Case;
        }

        /// <summary>
        /// Number of items in the array
        /// </summary>
        [Pure]
        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Value.Length;
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
            get => Value[index];
        }

        /// <summary>
        /// Add an item to the end of the array
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Arr<A> Add(A value)
        {
            var self = Value;
            if (self.Length == 0)
            {
                return new Arr<A>(new[] { value });
            }
            return Insert(self.Length, value);
        }

        /// <summary>
        /// Add a range of items to the end of the array
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Arr<A> AddRange(IEnumerable<A> items) =>
            InsertRange(Count, items);

        /// <summary>
        /// Clear the array
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Arr<A> Clear() =>
            Empty;

        /// <summary>
        /// Get enumerator
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator GetEnumerator() =>
            new Enumerator(this);

        public struct Enumerator
        {
            readonly A[] arr;
            int index;

            internal Enumerator(in Arr<A> arr)
            {
                this.arr = arr.Value;
                index = -1;
            }

            public readonly A Current => arr[index];

            public bool MoveNext() => ++index < arr.Length;
        }

        /// <summary>
        /// Find the index of an item
        /// </summary>
        [Pure]
        public int IndexOf(A item, int index = 0, int count = -1, IEqualityComparer<A> equalityComparer = null)
        {
            var eq = equalityComparer ?? EqualityComparer<A>.Default;

            var arr = Value;
            for (; index >= 0 && index < arr.Length && count != 0; index++, count--)
            {
                if (eq.Equals(item, arr[index])) return index;
            }
            return -1;
        }

        /// <summary>
        /// Find the index of an item
        /// </summary>
        [Pure]
        public int LastIndexOf(A item, int index = -1, int count = -1, IEqualityComparer<A> equalityComparer = null)
        {
            var eq = equalityComparer ?? EqualityComparer<A>.Default;

            var arr = Value;
            index = index < 0 ? arr.Length - 1 : index;

            for (; index >= 0 && index < arr.Length && count != 0; index--, count--)
            {
                if (eq.Equals(item, arr[index])) return index;
            }
            return -1;
        }

        /// <summary>
        /// Find the index of an item
        /// </summary>
        [Pure]
        public int IndexOf<EQ>(A item, int index = 0, int count = -1) where EQ : struct, Eq<A>
        {
            var eq = default(EQ);

            var arr = Value;
            for (; index < arr.Length && count != 0; index++, count--)
            {
                if (eq.Equals(item, arr[index])) return index;
            }
            return -1;
        }

        /// <summary>
        /// Find the index of an item
        /// </summary>
        [Pure]
        public int LastIndexOf<EQ>(A item, int index = -1, int count = -1) where EQ : struct, Eq<A>
        {
            var eq = default(EQ);

            var arr = Value;
            index = index < 0 ? arr.Length - 1 : index;

            for (; index >= 0 && index < arr.Length && count != 0; index--, count--)
            {
                if (eq.Equals(item, arr[index])) return index;
            }
            return -1;
        }

        /// <summary>
        /// Insert value at specified index
        /// </summary>
        [Pure]
        public Arr<A> Insert(int index, A value)
        {
            var arr = Value;
            if (index < 0 || index > Count) throw new IndexOutOfRangeException(nameof(index));
            if (arr.Length == 0)
            {
                return new Arr<A>(new A[1] { value });
            }

            A[] xs = new A[arr.Length + 1];
            xs[index] = value;

            if (index != 0)
            {
                System.Array.Copy(arr, 0, xs, 0, index);
            }
            if (index != arr.Length)
            {
                System.Array.Copy(arr, index, xs, index + 1, arr.Length - index);
            }
            return new Arr<A>(xs);
        }

        /// <summary>
        /// Insert range of values at specified index
        /// </summary>
        [Pure]
        public Arr<A> InsertRange(int index, IEnumerable<A> items)
        {
            items = items ?? new A[0];
            var arr = Value;
            if (index < 0 || index > arr.Length) throw new IndexOutOfRangeException(nameof(index));

            if (arr.Length == 0)
            {
                return new Arr<A>(items);
            }

            var insertArr = items.ToArray();

            int count = insertArr.Length;
            if (count == 0)
            {
                return this;
            }

            A[] newArray = new A[arr.Length + count];

            if (index != 0)
            {
                System.Array.Copy(arr, 0, newArray, 0, index);
            }
            if (index != arr.Length)
            {
                System.Array.Copy(arr, index, newArray, index + count, arr.Length - index);
            }
            insertArr.CopyTo(newArray, index);

            return new Arr<A>(newArray);
        }

        /// <summary>
        /// Remove an item from the array
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Arr<A> Remove(A value) =>
            Remove<EqDefault<A>>(value);

        /// <summary>
        /// Remove an item from the array
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Arr<A> Remove(A value, IEqualityComparer<A> equalityComparer)
        {
            int index = IndexOf(value, 0, -1, equalityComparer);
            return index < 0
                ? this
                : RemoveAt(index);
        }

        /// <summary>
        /// Remove an item from the array
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Arr<A> Remove<EQ>(A value) where EQ : struct, Eq<A>
        {
            int index = IndexOf<EQ>(value);
            return index < 0
                ? this
                : RemoveAt(index);
        }

        [Pure]
        public bool IsEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Value.Length == 0;
        }

        /// <summary>
        /// Remove all items that match a predicate
        /// </summary>
        [Pure]
        public Arr<A> RemoveAll(Predicate<A> pred)
        {
            var self = Value;
            pred = pred ?? (_ => true);
            if (IsEmpty) return this;

            List<int> removeIndices = null;
            for (int i = 0; i < self.Length; i++)
            {
                if (pred(self[i]))
                {
                    if (removeIndices == null)
                    {
                        removeIndices = new List<int>();
                    }
                    removeIndices.Add(i);
                }
            }

            return removeIndices != null
                ? RemoveAtRange(removeIndices)
                : this;
        }

        [Pure]
        private Arr<A> RemoveAtRange(List<int> remove)
        {
            var arr = Value;
            if (remove.Count == 0) return this;

            var newArray = new A[arr.Length - remove.Count];
            int copied = 0;
            int removed = 0;
            int lastIndexRemoved = -1;
            foreach (var item in remove)
            {
                int copyLength = lastIndexRemoved == -1 ? item : (item - lastIndexRemoved - 1);
                System.Array.Copy(arr, copied + removed, newArray, copied, copyLength);
                removed++;
                copied += copyLength;
                lastIndexRemoved = item;
            }
            System.Array.Copy(arr, copied + removed, newArray, copied, arr.Length - (copied + removed));
            return new Arr<A>(newArray);
        }

        /// <summary>
        /// Remove item at location
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Arr<A> RemoveAt(int index) =>
            RemoveRange(index, 1);

        /// <summary>
        /// Remove a range of items
        /// </summary>
        [Pure]
        public Arr<A> RemoveRange(int index, int count)
        {
            var arr = Value;
            if (index < 0 || index > arr.Length) throw new IndexOutOfRangeException(nameof(index));
            if (!(count >= 0 && index + count <= arr.Length)) throw new IndexOutOfRangeException(nameof(index));
            if (count == 0) return this;

            A[] newArray = new A[arr.Length - count];
            System.Array.Copy(arr, 0, newArray, 0, index);
            System.Array.Copy(arr, index + count, newArray, index, arr.Length - index - count);
            return new Arr<A>(newArray);
        }

        /// <summary>
        /// Set an item at the specified index
        /// </summary>
        [Pure]
        public Arr<A> SetItem(int index, A value)
        {
            var arr = Value;
            if (index < 0 || index >= arr.Length) throw new IndexOutOfRangeException(nameof(index));

            var newArray = new A[Count];
            arr.CopyTo(newArray, 0);
            newArray[index] = value;
            return new Arr<A>(newArray);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() =>
            Value.GetEnumerator();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator<A> IEnumerable<A>.GetEnumerator() =>
            Value.AsEnumerable().GetEnumerator();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<A> AsEnumerable() =>
            this;

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
        public IEnumerable<A> Skip(int amount) =>
            Value.Skip(amount);

        /// <summary>
        /// Reverse the order of the items in the array
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Arr<A> Reverse() =>
            new Arr<A>(Value.Reverse().ToArray());

        /// <summary>
        /// Fold
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public S Fold<S>(S state, Func<S, A, S> folder)
        {
            foreach (var item in Value)
            {
                state = folder(state, item);
            }
            return state;
        }

        /// <summary>
        /// Impure iteration of the bound values in the structure
        /// </summary>
        /// <returns>
        /// Returns the original unmodified structure
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Arr<A> Do(Action<A> f)
        {
            this.Iter(f);
            return this;
        }

        /// <summary>
        /// Map
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Arr<B> Map<B>(Func<A, B> map)
        {
            var arr = Value;
            var length = arr.Length;
            var newArray = new B[length];
            for (var i = 0; i < length; i++)
            {
                newArray[i] = map(arr[i]);
            }
            return new Arr<B>(newArray);
        }

        /// <summary>
        /// Filter
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Arr<A> Filter(Func<A, bool> pred) =>
            RemoveAll(x => !pred(x));

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Arr<A> operator +(Arr<A> lhs, A rhs) =>
            lhs.Add(rhs);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Arr<A> operator +(A lhs, Arr<A> rhs) =>
            rhs.Insert(0, lhs);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Arr<A> operator +(Arr<A> lhs, Arr<A> rhs) =>
            rhs.InsertRange(0, lhs);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Arr<A> Append(Arr<A> rhs) =>
            rhs.InsertRange(0, this);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj) =>
            obj is Arr<A> && Equals((Arr<A>)obj);

        /// <summary>
        /// Get the hash code
        /// Lazily (and once only) calculates the hash from the elements in the array
        /// Empty array hash == 0
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() =>
            hashCode == 0
                ? (hashCode = FNV32.Hash<HashableDefault<A>, A>(Value))
                : hashCode;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Arr<A> other) =>
            default(MArr<A>).Equals(this, other);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(Arr<A> other) =>
            default(MArr<A>).Compare(this, other);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Arr<A> lhs, Arr<A> rhs) =>
            lhs.Equals(rhs);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Arr<A> lhs, Arr<A> rhs) =>
            !(lhs == rhs);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Arr<B> Bind<B>(Func<A, Arr<B>> f)
        {
            var res = new List<B>();

            foreach (var t in this)
            {
                foreach (var u in f(t))
                {
                    res.Add(u);
                }
            }
            return res.ToArr();
        }

        /// <summary>
        /// Implicit conversion from an untyped empty list
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Arr<A>(SeqEmpty _) =>
            Empty;

    }
}

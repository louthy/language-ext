using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;
using LanguageExt.TypeClasses;
using LanguageExt.ClassInstances;

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
        IReadOnlyCollection<A>
    {
        /// <summary>
        /// Empty array
        /// </summary>
        public static readonly Arr<A> Empty = new Arr<A>(new A[0] { });

        readonly A[] value;
        internal A[] Value => value ?? Empty.Value;
        int hashCode;

        /// <summary>
        /// Ctor
        /// </summary>
        public Arr(IEnumerable<A> initial)
        {
            hashCode = 0;
            value = initial.ToArray();
        }

        /// <summary>
        /// Ctor
        /// </summary>
        internal Arr(A[] value)
        {
            hashCode = 0;
            this.value = value;
        }

        public static implicit operator Arr<A>(A[] xs) =>
            new Arr<A>(xs);

        /// <summary>
        /// Index accessor
        /// </summary>
        [Pure]
        public A this[int index] => Value[index];

        /// <summary>
        /// Number of items in the array
        /// </summary>
        [Pure]
        public int Count =>
            Value.Length;

        [Pure]
        int IReadOnlyCollection<A>.Count =>
            Count;

        [Pure]
        A IReadOnlyList<A>.this[int index] => Value[index];

        /// <summary>
        /// Add an item to the end of the array
        /// </summary>
        [Pure]
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
        public Arr<A> AddRange(IEnumerable<A> items) =>
            InsertRange(Count, items);

        /// <summary>
        /// Clear the array
        /// </summary>
        [Pure]
        public Arr<A> Clear() =>
            Empty;

        /// <summary>
        /// Get enumerator
        /// </summary>
        [Pure]
        public IEnumerator<A> GetEnumerator() =>
            Value.AsEnumerable().GetEnumerator();

        /// <summary>
        /// Find the index of an item
        /// </summary>
        [Pure]
        public int IndexOf(A item, int index = 0, int count = -1, IEqualityComparer<A> equalityComparer = null)
        {
            var eq = equalityComparer ?? EqualityComparer<A>.Default;

            var arr = Value;
            for (; index >=0 && index < arr.Length && count != 0; index++, count--)
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
            if( index < 0 || index > arr.Length) throw new IndexOutOfRangeException(nameof(index));

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
        public Arr<A> Remove(A value) =>
            Remove<EqDefault<A>>(value);

        /// <summary>
        /// Remove an item from the array
        /// </summary>
        [Pure]
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
        public Arr<A> Remove<EQ>(A value) where EQ : struct, Eq<A>
        {
            int index = IndexOf<EQ>(value);
            return index < 0
                ? this
                : RemoveAt(index);
        }

        [Pure]
        public bool IsEmpty =>
            Value.Length == 0;

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
        public Arr<A> RemoveAt(int index) =>
            RemoveRange(index, 1);

        /// <summary>
        /// Remove a range of items
        /// </summary>
        [Pure]
        public Arr<A> RemoveRange(int index, int count)
        {
            var arr = Value;
            if( index < 0 || index > arr.Length) throw new IndexOutOfRangeException(nameof(index));
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
        IEnumerator IEnumerable.GetEnumerator() =>
            Value.GetEnumerator();

        [Pure]
        IEnumerator<A> IEnumerable<A>.GetEnumerator() =>
            Value.AsEnumerable().GetEnumerator();

        [Pure]
        IEnumerable<A> AsEnumerable() =>
            this;

        [Pure]
        Seq<A> ToSeq() =>
            Seq(this);

        [Pure]
        public IEnumerable<A> Skip(int amount) =>
            Value.Skip(amount);

        /// <summary>
        /// Reverse the order of the items in the array
        /// </summary>
        [Pure]
        public Arr<A> Reverse() =>
            new Arr<A>(Value.Reverse().ToArray());

        /// <summary>
        /// Fold
        /// </summary>
        [Pure]
        public S Fold<S>(S state, Func<S, A, S> folder)
        {
            foreach(var item in Value)
            {
                state = folder(state, item);
            }
            return state;
        }

        /// <summary>
        /// Map
        /// </summary>
        [Pure]
        public Arr<B> Map<B>(Func<A, B> map)
        {
            var arr = Value;
            var length = arr.Length;
            var newArray = new B[length];
            for(var i = 0; i < length; i++)
            {
                newArray[i] = map(arr[i]);
            }
            return new Arr<B>(newArray);
        }

        /// <summary>
        /// Filter
        /// </summary>
        [Pure]
        public Arr<A> Filter(Func<A, bool> pred) =>
            RemoveAll(x => !pred(x));

        [Pure]
        public static Arr<A> operator +(Arr<A> lhs, A rhs) =>
            lhs.Add(rhs);

        [Pure]
        public static Arr<A> operator +(A lhs, Arr<A> rhs) =>
            rhs.Insert(0, lhs);

        [Pure]
        public static Arr<A> operator +(Arr<A> lhs, Arr<A> rhs) =>
            rhs.InsertRange(0, lhs);

        [Pure]
        public Arr<A> Append(Arr<A> rhs) =>
            rhs.InsertRange(0, this);

        [Pure]
        public override bool Equals(object obj) =>
            obj is Arr<A> && Equals((Arr<A>)obj);

        /// <summary>
        /// Get the hash code
        /// Lazily (and once only) calculates the hash from the elements in the array
        /// Empty array hash == 0
        /// </summary>
        [Pure]
        public override int GetHashCode() =>
            hashCode == 0
                ? hashCode = hash(Value)
                : hashCode;

        [Pure]
        public bool Equals(Lst<A> other) =>
            Value.Equals(other.Value);

        [Pure]
        public static bool operator ==(Arr<A> lhs, Arr<A> rhs) =>
            Equals(lhs, rhs);

        [Pure]
        public static bool operator !=(Arr<A> lhs, Arr<A> rhs) =>
            !(lhs == rhs);
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Threading;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    /// <summary>
    /// Unsorted immutable hash-set
    /// </summary>
    /// <typeparam name="A">Key type</typeparam>
    public struct HashSet<EqA, A> :
        IEnumerable<A>,
        IEquatable<HashSet<EqA, A>>
        where EqA : struct, Eq<A>
    {
        public static readonly HashSet<EqA, A> Empty = new HashSet<EqA, A>(HashSetInternal<EqA, A>.Empty);

        readonly HashSetInternal<EqA, A> value;
        HashSetInternal<EqA, A> Value => value ?? HashSetInternal<EqA, A>.Empty;

        internal HashSet(HashSetInternal<EqA, A> value)
        {
            this.value = value;
        }

        HashSet<EqA, A> Wrap(HashSetInternal<EqA, A> value) =>
            new HashSet<EqA, A>(value);

        static HashSet<EqB, B> Wrap<EqB, B>(HashSetInternal<EqB, B> value)
            where EqB : struct, Eq<B> =>
            new HashSet<EqB, B>(value);

        /// <summary>
        /// Ctor that takes an initial (distinct) set of items
        /// </summary>
        /// <param name="items"></param>
        internal HashSet(IEnumerable<A> items, bool tryAdd = false)
        {
            value = new HashSetInternal<EqA, A>(items, tryAdd);
        }

        /// <summary>
        /// 'this' accessor
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Optional value</returns>
        [Pure]
        public A this[A key] =>
            Value[key];

        /// <summary>
        /// Is the map empty
        /// </summary>
        [Pure]
        public bool IsEmpty =>
            Value.IsEmpty;

        /// <summary>
        /// Number of items in the set
        /// </summary>
        [Pure]
        public int Count =>
            Value.Count;

        /// <summary>
        /// Number of items in the set
        /// </summary>
        [Pure]
        public int Length =>
            Value.Length;

        /// <summary>
        /// Maps the values of this set into a new set of values using the
        /// mapper function to tranform the source values.
        /// </summary>
        /// <typeparam name="B">Mapped element type</typeparam>
        /// <param name="set">HSet</param>
        /// <param name="mapper">Mapping function</param>
        /// <returns>Mapped enumerable</returns>
        [Pure]
        public HashSet<EqA, A> Map(Func<A, A> mapper) =>
            Wrap(Value.Map<EqA, A>(mapper));

        /// <summary>
        /// Maps the values of this set into a new set of values using the
        /// mapper function to tranform the source values.
        /// </summary>
        /// <typeparam name="B">Mapped element type</typeparam>
        /// <param name="set">HSet</param>
        /// <param name="mapper">Mapping function</param>
        /// <returns>Mapped enumerable</returns>
        [Pure]
        public HashSet<EqB, B> Map<EqB, B>(Func<A, B> mapper) where EqB : struct, Eq<B> =>
            Wrap(Value.Map<EqB, B>(mapper));

        /// <summary>
        /// Filters items from the set using the predicate.  If the predicate
        /// returns True for any item then it remains in the set, otherwise
        /// it's dropped.
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="set">HSet</param>
        /// <param name="pred">Predicate</param>
        /// <returns>Filtered enumerable</returns>
        [Pure]
        public HashSet<EqA, A> Filter(Func<A, bool> pred) =>
            Wrap(Value.Filter(pred));

        /// <summary>
        /// Maps the values of this set into a new set of values using the
        /// mapper function to tranform the source values.
        /// </summary>
        /// <typeparam name="B">Mapped element type</typeparam>
        /// <param name="set">HSet</param>
        /// <param name="mapper">Mapping function</param>
        /// <returns>Mapped enumerable</returns>
        [Pure]
        public HashSet<EqA, A> Select(Func<A, A> mapper) =>
            Wrap(Value.Map<EqA, A>(mapper));

        /// <summary>
        /// Maps the values of this set into a new set of values using the
        /// mapper function to tranform the source values.
        /// </summary>
        /// <typeparam name="B">Mapped element type</typeparam>
        /// <param name="set">HSet</param>
        /// <param name="mapper">Mapping function</param>
        /// <returns>Mapped enumerable</returns>
        [Pure]
        public HashSet<EqB, B> Select<EqB, B>(Func<A, B> mapper) where EqB : struct, Eq<B> =>
            Wrap(Value.Map<EqB, B>(mapper));

        /// <summary>
        /// Filters items from the set using the predicate.  If the predicate
        /// returns True for any item then it remains in the set, otherwise
        /// it's dropped.
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="set">HSet</param>
        /// <param name="pred">Predicate</param>
        /// <returns>Filtered enumerable</returns>
        [Pure]
        public HashSet<EqA, A> Where(Func<A, bool> pred) =>
            Wrap(Value.Filter(pred));

        /// <summary>
        /// Add an item to the set
        /// </summary>
        /// <param name="value">Value to add to the set</param>
        /// <returns>New set with the item added</returns>
        [Pure]
        public HashSet<EqA, A> Add(A key) =>
            Wrap(Value.Add(key));

        /// <summary>
        /// Attempt to add an item to the set.  If an item already
        /// exists then return the Set as-is.
        /// </summary>
        /// <param name="value">Value to add to the set</param>
        /// <returns>New set with the item maybe added</returns>
        [Pure]
        public HashSet<EqA, A> TryAdd(A key) =>
            Wrap(Value.TryAdd(key));

        /// <summary>
        /// Add an item to the set.  If an item already
        /// exists then replace it.
        /// </summary>
        /// <param name="value">Value to add to the set</param>
        /// <returns>New set with the item maybe added</returns>
        [Pure]
        public HashSet<EqA, A> AddOrUpdate(A key) =>
            Wrap(Value.AddOrUpdate(key));

        /// <summary>
        /// Atomically adds a range of items to the set.
        /// </summary>
        /// <remarks>Null is not allowed for a Key</remarks>
        /// <param name="range">Range of keys to add</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys already exist</exception>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException if any of the keys are null</exception>
        /// <returns>New HSet with the items added</returns>
        [Pure]
        public HashSet<EqA, A> AddRange(IEnumerable<A> range) =>
            Wrap(Value.AddRange(range));

        /// <summary>
        /// Atomically adds a range of items to the set.  If an item already exists, it's ignored.
        /// </summary>
        /// <remarks>Null is not allowed for a Key</remarks>
        /// <param name="range">Range of keys to add</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys already exist</exception>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException if any of the keys are null</exception>
        /// <returns>New HSet with the items added</returns>
        [Pure]
        public HashSet<EqA, A> TryAddRange(IEnumerable<A> range) =>
            Wrap(Value.TryAddRange(range));

        /// <summary>
        /// Atomically adds a range of items to the set.  If any items already exist, they're ignored.
        /// </summary>
        /// <remarks>Null is not allowed for a Key</remarks>
        /// <param name="range">Range of keys to add</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys already exist</exception>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException if any of the keys are null</exception>
        /// <returns>New HSet with the items added</returns>
        [Pure]
        public HashSet<EqA, A> AddOrUpdateRange(IEnumerable<A> range) =>
            Wrap(Value.AddOrUpdateRange(range));

        /// <summary>
        /// Atomically removes an item from the set
        /// If the key doesn't exists, the request is ignored.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>New map with the item removed</returns>
        [Pure]
        public HashSet<EqA, A> Remove(A key) =>
            Wrap(Value.Remove(key));

        /// <summary>
        /// Retrieve a value from the set by key
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>Found value</returns>
        [Pure]
        public Option<A> Find(A key) =>
            Value.Find(key);

        /// <summary>
        /// Retrieve a value from the set by key as an enumerable
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>Found value</returns>
        [Pure]
        public IEnumerable<A> FindSeq(A key) =>
            Value.FindSeq(key);

        /// <summary>
        /// Retrieve a value from the set by key and pattern match the
        /// result.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>Found value</returns>
        [Pure]
        public R Find<R>(A key, Func<A, R> Some, Func<R> None) =>
            Value.Find(key, Some, None);

        /// <summary>
        /// Atomically updates an existing item
        /// </summary>
        /// <remarks>Null is not allowed for a key</remarks>
        /// <param name="key">Key</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException if the key is null</exception>
        /// <returns>New HSet with the item added</returns>
        [Pure]
        public HashSet<EqA, A> SetItem(A key) =>
            Wrap(Value.SetItem(key));

        /// <summary>
        /// Atomically updates an existing item, unless it doesn't exist, in which case 
        /// it is ignored
        /// </summary>
        /// <remarks>Null is not allowed for a key</remarks>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException if the key is null</exception>
        /// <returns>New HSet with the item added</returns>
        [Pure]
        public HashSet<EqA, A> TrySetItem(A key) =>
            Wrap(Value.TrySetItem(key));

        /// <summary>
        /// Checks for existence of a key in the set
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True if an item with the key supplied is in the set</returns>
        [Pure]
        public bool Contains(A key) =>
            Value.Contains(key);

        /// <summary>
        /// Clears all items from the set
        /// </summary>
        /// <remarks>Functionally equivalent to calling HSet.empty as the original structure is untouched</remarks>
        /// <returns>Empty HSet</returns>
        [Pure]
        public HashSet<EqA, A> Clear() =>
            Empty;

        /// <summary>
        /// Atomically sets a series of items
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys aren't in the map</exception>
        /// <returns>New HSet with the items set</returns>
        [Pure]
        public HashSet<EqA, A> SetItems(IEnumerable<A> items) =>
            Wrap(Value.SetItems(items));

        /// <summary>
        /// Atomically sets a series of items.  If any of the items don't exist then they're silently ignored.
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <returns>New HSet with the items set</returns>
        [Pure]
        public HashSet<EqA, A> TrySetItems(IEnumerable<A> items) =>
            Wrap(Value.TrySetItems(items));

        /// <summary>
        /// Atomically removes a list of keys from the set
        /// </summary>
        /// <param name="keys">Keys to remove</param>
        /// <returns>New HSet with the items removed</returns>
        [Pure]
        public HashSet<EqA, A> RemoveRange(IEnumerable<A> keys) =>
            Wrap(Value.RemoveRange(keys));

        /// <summary>
        /// GetEnumerator - IEnumerable interface
        /// </summary>
        [Pure]
        public IEnumerator<A> GetEnumerator() =>
            Value.GetEnumerator();

        /// <summary>
        /// GetEnumerator - IEnumerable interface
        /// </summary>
        [Pure]
        IEnumerator IEnumerable.GetEnumerator() =>
            Value.GetEnumerator();

        [Pure]
        public Seq<A> ToSeq() =>
            Seq(this);

        [Pure]
        public IEnumerable<A> AsEnumerable() =>
            this;

        [Pure]
        public static HashSet<EqA, A> operator +(HashSet<EqA, A> lhs, HashSet<EqA, A> rhs) =>
            lhs.Append(rhs);

        [Pure]
        public HashSet<EqA, A> Append(HashSet<EqA, A> rhs) =>
            Wrap(Value.Append(rhs.Value));

        [Pure]
        public static HashSet<EqA, A> operator -(HashSet<EqA, A> lhs, HashSet<EqA, A> rhs) =>
            lhs.Subtract(rhs);

        [Pure]
        public HashSet<EqA, A> Subtract(HashSet<EqA, A> rhs) =>
            Wrap(Value.Subtract(rhs.Value));

        [Pure]
        public static bool operator ==(HashSet<EqA, A> lhs, HashSet<EqA, A> rhs) =>
            lhs.Equals(rhs);

        [Pure]
        public static bool operator !=(HashSet<EqA, A> lhs, HashSet<EqA, A> rhs) =>
            !(lhs == rhs);

        [Pure]
        public bool Equals(HashSet<EqA, A> other) =>
            Value.Equals(other.Value);

        /// <summary>
        /// Returns True if 'other' is a proper subset of this set
        /// </summary>
        /// <returns>True if 'other' is a proper subset of this set</returns>
        [Pure]
        public bool IsProperSubsetOf(HashSet<EqA, A> other) =>
            Value.IsProperSubsetOf(other);

        /// <summary>
        /// Returns True if 'other' is a proper superset of this set
        /// </summary>
        /// <returns>True if 'other' is a proper superset of this set</returns>
        [Pure]
        public bool IsProperSupersetOf(HashSet<EqA, A> other) =>
            Value.IsProperSupersetOf(other);

        /// <summary>
        /// Returns True if 'other' is a superset of this set
        /// </summary>
        /// <returns>True if 'other' is a superset of this set</returns>
        [Pure]
        public bool IsSubsetOf(HashSet<EqA, A> other) =>
            Value.IsSubsetOf(other);

        /// <summary>
        /// Returns True if 'other' is a superset of this set
        /// </summary>
        /// <returns>True if 'other' is a superset of this set</returns>
        [Pure]
        public bool IsSupersetOf(HashSet<EqA, A> other) =>
            Value.IsSupersetOf(other);

        /// <summary>
        /// Returns True if other overlaps this set
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="setA">Set A</param>
        /// <param name="setB">Set B</param>
        /// <returns>True if other overlaps this set</returns>
        [Pure]
        public bool Overlaps(HashSet<EqA, A> other) =>
            Value.Overlaps(other);

        /// <summary>
        /// Returns the elements that are in both this and other
        /// </summary>
        [Pure]
        public HashSet<EqA, A> Intersect(HashSet<EqA, A> other)
        {
            var res = new List<A>();
            foreach (var item in other)
            {
                if (Contains(item)) res.Add(item);
            }
            return new HashSet<EqA, A>(res);
        }

        /// <summary>
        /// Returns this - other.  Only the items in this that are not in 
        /// other will be returned.
        /// </summary>
        [Pure]
        public HashSet<EqA, A> Except(HashSet<EqA, A> other)
        {
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

        /// <summary>
        /// Only items that are in one set or the other will be returned.
        /// If an item is in both, it is dropped.
        /// </summary>
        [Pure]
        public HashSet<EqA, A> SymmetricExcept(HashSet<EqA, A> other)
        {
            var rhs = new Set<A>(other);
            var res = new List<A>();

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

            return new HashSet<EqA, A>(res);
        }

        /// <summary>
        /// Finds the union of two sets and produces a new set with 
        /// the results
        /// </summary>
        /// <param name="other">Other set to union with</param>
        /// <returns>A set which contains all items from both sets</returns>
        [Pure]
        public HashSet<EqA, A> Union(HashSet<EqA, A> other)
        {
            var self = this;
            foreach (var item in other)
            {
                self = self.TryAdd(item);
            }
            return self;
        }

        public bool SetEquals(HashSet<EqA, A> other) =>
            Value.SetEquals(other);

        /// <summary>
        /// Copy the items from the set into the specified array
        /// </summary>
        /// <param name="array">Array to copy to</param>
        /// <param name="index">Index into the array to start</param>
        public void CopyTo(A[] array, int index) => 
            Value.CopyTo(array, index);

        /// <summary>
        /// Copy the items from the set into the specified array
        /// </summary>
        /// <param name="array">Array to copy to</param>
        /// <param name="index">Index into the array to start</param>
        public void CopyTo(System.Array array, int index) =>
            Value.CopyTo(array, index);

        [Pure]
        public override bool Equals(object obj) =>
            !obj.IsNull() && obj is HashSet<EqA, A> && Equals(this, (HashSet<EqA, A>)obj);

        [Pure]
        public override int GetHashCode() =>
            Value.GetHashCode();

        [Pure]
        public HashSet<EqB, B> Bind<EqB, B>(Func<A, HashSet<EqB, B>> f) where EqB : struct, Eq<B>
        {
            var self = this;

            IEnumerable<B> Yield()
            {
                foreach (var x in self.AsEnumerable())
                {
                    foreach (var y in f(x))
                    {
                        yield return y;
                    }
                }
            }
            return new HashSet<EqB, B>(Yield(), true);
        }

        [Pure]
        public HashSet<EqA, A> Bind(Func<A, HashSet<EqA, A>> f)
        {
            var self = this;

            IEnumerable<A> Yield()
            {
                foreach (var x in self.AsEnumerable())
                {
                    foreach (var y in f(x))
                    {
                        yield return y;
                    }
                }
            }
            return new HashSet<EqA, A>(Yield(), true);
        }
    }
}

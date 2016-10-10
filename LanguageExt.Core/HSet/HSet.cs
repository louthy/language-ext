﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Threading;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    /// <summary>
    /// Unsorted immutable hash-set
    /// </summary>
    /// <typeparam name="T">Key type</typeparam>
    public struct HSet<T> :
        IReadOnlyCollection<T>,
        ICollection<T>,
        ISet<T>,
        ICollection,
        IAppendable<HSet<T>>,
        ISubtractable<HSet<T>>,
        IEquatable<HSet<T>>
    {
        public static readonly HSet<T> Empty = new HSet<T>(HSetInternal<T>.Empty);

        readonly HSetInternal<T> value;
        HSetInternal<T> Value => value ?? HSetInternal<T>.Empty;

        internal HSet(HSetInternal<T> value)
        {
            this.value = value;
        }

        HSet<T> Wrap(HSetInternal<T> value) =>
            new HSet<T>(value);

        static HSet<U> Wrap<U>(HSetInternal<U> value) =>
            new HSet<U>(value);

        /// <summary>
        /// Ctor that takes an initial (distinct) set of items
        /// </summary>
        /// <param name="items"></param>
        internal HSet(IEnumerable<T> items, bool checkUniqueness = false)
        {
            value = new HSetInternal<T>(items, checkUniqueness);
        }

        /// <summary>
        /// 'this' accessor
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Optional value</returns>
        [Pure]
        public T this[T key] =>
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

        public object SyncRoot => this;
        public bool IsSynchronized => true;
        public bool IsReadOnly => true;

        /// <summary>
        /// Maps the values of this set into a new set of values using the
        /// mapper function to tranform the source values.
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <typeparam name="R">Mapped element type</typeparam>
        /// <param name="set">HSet</param>
        /// <param name="mapper">Mapping function</param>
        /// <returns>Mapped enumerable</returns>
        [Pure]
        public HSet<R> Map<R>(Func<T, R> mapper) =>
            Wrap(Value.Map(mapper));

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
        public HSet<T> Filter(Func<T, bool> pred) =>
            Wrap(Value.Filter(pred));

        /// <summary>
        /// Maps the values of this set into a new set of values using the
        /// mapper function to tranform the source values.
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <typeparam name="R">Mapped element type</typeparam>
        /// <param name="set">HSet</param>
        /// <param name="mapper">Mapping function</param>
        /// <returns>Mapped enumerable</returns>
        [Pure]
        public HSet<R> Select<R>(Func<T, R> mapper) =>
            Wrap(Value.Map(mapper));

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
        public HSet<T> Where(Func<T, bool> pred) =>
            Wrap(Value.Filter(pred));

        /// <summary>
        /// Add an item to the set
        /// </summary>
        /// <param name="value">Value to add to the set</param>
        /// <returns>New set with the item added</returns>
        [Pure]
        public HSet<T> Add(T key) =>
            Wrap(Value.Add(key));

        /// <summary>
        /// Attempt to add an item to the set.  If an item already
        /// exists then return the Set as-is.
        /// </summary>
        /// <param name="value">Value to add to the set</param>
        /// <returns>New set with the item maybe added</returns>
        [Pure]
        public HSet<T> TryAdd(T key) =>
            Wrap(Value.TryAdd(key));

        /// <summary>
        /// Add an item to the set.  If an item already
        /// exists then replace it.
        /// </summary>
        /// <param name="value">Value to add to the set</param>
        /// <returns>New set with the item maybe added</returns>
        [Pure]
        public HSet<T> AddOrUpdate(T key) =>
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
        public HSet<T> AddRange(IEnumerable<T> range) =>
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
        public HSet<T> TryAddRange(IEnumerable<T> range) =>
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
        public HSet<T> AddOrUpdateRange(IEnumerable<T> range) =>
            Wrap(Value.AddOrUpdateRange(range));

        /// <summary>
        /// Atomically removes an item from the set
        /// If the key doesn't exists, the request is ignored.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>New map with the item removed</returns>
        [Pure]
        public HSet<T> Remove(T key) =>
            Wrap(Value.Remove(key));

        /// <summary>
        /// Retrieve a value from the set by key
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>Found value</returns>
        [Pure]
        public Option<T> Find(T key) =>
            Value.Find(key);

        /// <summary>
        /// Retrieve a value from the set by key as an enumerable
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>Found value</returns>
        [Pure]
        public IEnumerable<T> FindSeq(T key) =>
            Value.FindSeq(key);

        /// <summary>
        /// Retrieve a value from the set by key and pattern match the
        /// result.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>Found value</returns>
        [Pure]
        public R Find<R>(T key, Func<T, R> Some, Func<R> None) =>
            Value.Find(key, Some, None);

        /// <summary>
        /// Atomically updates an existing item
        /// </summary>
        /// <remarks>Null is not allowed for a key</remarks>
        /// <param name="key">Key</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException if the key is null</exception>
        /// <returns>New HSet with the item added</returns>
        [Pure]
        public HSet<T> SetItem(T key) =>
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
        public HSet<T> TrySetItem(T key) =>
            Wrap(Value.TrySetItem(key));

        /// <summary>
        /// Checks for existence of a key in the set
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True if an item with the key supplied is in the set</returns>
        [Pure]
        public bool Contains(T key) =>
            Value.Contains(key);

        /// <summary>
        /// Clears all items from the set
        /// </summary>
        /// <remarks>Functionally equivalent to calling HSet.empty as the original structure is untouched</remarks>
        /// <returns>Empty HSet</returns>
        [Pure]
        public HSet<T> Clear() =>
            Empty;

        /// <summary>
        /// Atomically sets a series of items
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys aren't in the map</exception>
        /// <returns>New HSet with the items set</returns>
        [Pure]
        public HSet<T> SetItems(IEnumerable<T> items) =>
            Wrap(Value.SetItems(items));

        /// <summary>
        /// Atomically sets a series of items.  If any of the items don't exist then they're silently ignored.
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <returns>New HSet with the items set</returns>
        [Pure]
        public HSet<T> TrySetItems(IEnumerable<T> items) =>
            Wrap(Value.TrySetItems(items));

        /// <summary>
        /// Atomically removes a list of keys from the set
        /// </summary>
        /// <param name="keys">Keys to remove</param>
        /// <returns>New HSet with the items removed</returns>
        [Pure]
        public HSet<T> RemoveRange(IEnumerable<T> keys) =>
            Wrap(Value.RemoveRange(keys));

        #region IEnumerable interface

        /// <summary>
        /// GetEnumerator - IEnumerable interface
        /// </summary>
        public IEnumerator<T> GetEnumerator() =>
            Value.GetEnumerator();

        /// <summary>
        /// GetEnumerator - IEnumerable interface
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator() =>
            Value.GetEnumerator();

        public IEnumerable<T> AsEnumerable() =>
            Value.AsEnumerable();

        #endregion

        [Pure]
        public static HSet<T> operator +(HSet<T> lhs, HSet<T> rhs) =>
            lhs.Append(rhs);

        [Pure]
        public HSet<T> Append(HSet<T> rhs) =>
            Wrap(Value.Append(rhs.Value));

        [Pure]
        public static HSet<T> operator -(HSet<T> lhs, HSet<T> rhs) =>
            lhs.Subtract(rhs);

        [Pure]
        public HSet<T> Subtract(HSet<T> rhs) =>
            Wrap(Value.Subtract(rhs.Value));

        public bool Equals(HSet<T> other) =>
            Value.Equals(other.Value);


        /// <summary>
        /// Returns True if 'other' is a proper subset of this set
        /// </summary>
        /// <returns>True if 'other' is a proper subset of this set</returns>
        [Pure]
        public bool IsProperSubsetOf(IEnumerable<T> other) =>
            Value.IsProperSubsetOf(other);

        /// <summary>
        /// Returns True if 'other' is a proper superset of this set
        /// </summary>
        /// <returns>True if 'other' is a proper superset of this set</returns>
        [Pure]
        public bool IsProperSupersetOf(IEnumerable<T> other) =>
            Value.IsProperSupersetOf(other);

        /// <summary>
        /// Returns True if 'other' is a superset of this set
        /// </summary>
        /// <returns>True if 'other' is a superset of this set</returns>
        [Pure]
        public bool IsSubsetOf(IEnumerable<T> other) =>
            Value.IsSubsetOf(other);

        /// <summary>
        /// Returns True if 'other' is a superset of this set
        /// </summary>
        /// <returns>True if 'other' is a superset of this set</returns>
        [Pure]
        public bool IsSupersetOf(IEnumerable<T> other) =>
            Value.IsSupersetOf(other);

        /// <summary>
        /// Returns True if other overlaps this set
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="setA">Set A</param>
        /// <param name="setB">Set B</param>
        /// <returns>True if other overlaps this set</returns>
        [Pure]
        public bool Overlaps(IEnumerable<T> other) =>
            Value.Overlaps(other);

        /// <summary>
        /// Returns the elements that are in both this and other
        /// </summary>
        [Pure]
        public HSet<T> Intersect(IEnumerable<T> other)
        {
            var res = new List<T>();
            foreach (var item in other)
            {
                if (Contains(item)) res.Add(item);
            }
            return new HSet<T>(res);
        }

        /// <summary>
        /// Returns this - other.  Only the items in this that are not in 
        /// other will be returned.
        /// </summary>
        [Pure]
        public HSet<T> Except(IEnumerable<T> other)
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
        public HSet<T> SymmetricExcept(IEnumerable<T> other)
        {
            var rhs = new Set<T>(other);
            var res = new List<T>();

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

            return new HSet<T>(res);
        }

        /// <summary>
        /// Finds the union of two sets and produces a new set with 
        /// the results
        /// </summary>
        /// <param name="other">Other set to union with</param>
        /// <returns>A set which contains all items from both sets</returns>
        [Pure]
        public HSet<T> Union(IEnumerable<T> other)
        {
            var self = this;
            foreach (var item in other)
            {
                self = self.TryAdd(item);
            }
            return self;
        }

        bool ISet<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        public void UnionWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        public bool SetEquals(IEnumerable<T> other) =>
            Value.SetEquals(other);

        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        void ICollection<T>.Clear()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Copy the items from the set into the specified array
        /// </summary>
        /// <param name="array">Array to copy to</param>
        /// <param name="index">Index into the array to start</param>
        public void CopyTo(T[] array, int index) => 
            Value.CopyTo(array, index);

        /// <summary>
        /// Copy the items from the set into the specified array
        /// </summary>
        /// <param name="array">Array to copy to</param>
        /// <param name="index">Index into the array to start</param>
        public void CopyTo(Array array, int index) =>
            Value.CopyTo(array, index);

        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }
    }
}

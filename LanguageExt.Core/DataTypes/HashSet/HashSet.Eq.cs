using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

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
        public static readonly HashSet<EqA, A> Empty = new HashSet<EqA, A>(TrieSet<EqA, A>.Empty);

        readonly TrieSet<EqA, A> value;
        TrieSet<EqA, A> Value => value ?? TrieSet<EqA, A>.Empty;

        internal HashSet(TrieSet<EqA, A> value)
        {
            this.value = value;
        }

        HashSet<EqA, A> Wrap(TrieSet<EqA, A> value) =>
            new HashSet<EqA, A>(value);

        static HashSet<B> Wrap<B>(TrieSet<EqDefault<B>, B> value) =>
            new HashSet<B>(value);

        /// <summary>
        /// Ctor that takes an initial (distinct) set of items
        /// </summary>
        /// <param name="items"></param>
        internal HashSet(IEnumerable<A> items) : this(items, true)
        {
        }

        /// <summary>
        /// Ctor that takes an initial (distinct) set of items
        /// </summary>
        internal HashSet(IEnumerable<A> items, bool tryAdd) =>
            value = new TrieSet<EqA, A>(items, tryAdd);

        /// <summary>
        /// Item at index lens
        /// </summary>
        [Pure]
        public static Lens<HashSet<EqA, A>, bool> item(A key) => Lens<HashSet<EqA, A>, bool>.New(
            Get: la => la.Contains(key),
            Set: a => la => a ? la.AddOrUpdate(key) : la.Remove(key)
            );

        /// <summary>
        /// Lens map
        /// </summary>
        [Pure]
        public static Lens<HashSet<EqA, A>, HashSet<EqA, A>> map<B>(Lens<A, A> lens) => 
            Lens<HashSet<EqA, A>, HashSet<EqA, A>>.New(
                Get: la => la.Map<EqA, A>(lens.Get),
                Set: lb => la =>
                {
                    foreach (var item in lb)
                    {
                        la = la.Find(item).Match(Some: x => la.AddOrUpdate(lens.Set(x, item)), None: () => la);
                    }
                    return la;
                });

        /// <summary>
        /// Reference version for use in pattern-matching
        /// </summary>
        [Pure]
        public SeqCase<A> Case =>
            Seq(this).Case;

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
            Value.Count;

        /// <summary>
        /// Impure iteration of the bound values in the structure
        /// </summary>
        /// <returns>
        /// Returns the original unmodified structure
        /// </returns>
        public HashSet<EqA, A> Do(Action<A> f)
        {
            this.Iter(f);
            return this;
        }

        /// <summary>
        /// Maps the values of this set into a new set of values using the
        /// mapper function to tranform the source values.
        /// </summary>
        /// <typeparam name="R">Mapped element type</typeparam>
        /// <param name="set">HSet</param>
        /// <param name="mapper">Mapping function</param>
        /// <returns>Mapped enumerable</returns>
        [Pure]
        public HashSet<EqR, R> Map<EqR, R>(Func<A, R> mapper) where EqR : struct, Eq<R>
        {
            IEnumerable<R> Yield(TrieSet<EqA, A> map, Func<A, R> f)
            {
                foreach (var item in map)
                {
                    yield return f(item);
                }
            }
            return new HashSet<EqR, R>(Yield(Value, mapper));
        }

        /// <summary>
        /// Maps the values of this set into a new set of values using the
        /// mapper function to tranform the source values.
        /// </summary>
        /// <param name="set">HSet</param>
        /// <param name="mapper">Mapping function</param>
        /// <returns>Mapped enumerable</returns>
        [Pure]
        public HashSet<EqA, A> Map(Func<A, A> mapper) =>
            Map<EqA, A>(mapper);

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
        public HashSet<EqA, A> Filter(Func<A, bool> pred)
        {
            IEnumerable<A> Yield(TrieSet<EqA, A> map, Func<A, bool> f)
            {
                foreach (var item in map)
                {
                    if (f(item))
                    {
                        yield return item;
                    }
                }
            }
            return new HashSet<EqA, A>(Yield(Value, pred));
        }
        /// <summary>
        /// Maps the values of this set into a new set of values using the
        /// mapper function to tranform the source values.
        /// </summary>
        /// <typeparam name="R">Mapped element type</typeparam>
        /// <param name="set">HSet</param>
        /// <param name="mapper">Mapping function</param>
        /// <returns>Mapped enumerable</returns>
        [Pure]
        public HashSet<EqR, R> Select<EqR, R>(Func<A, R> mapper) where EqR : struct, Eq<R> =>
            Map<EqR, R>(mapper);

        /// <summary>
        /// Filters items from the set using the predicate.  If the predicate
        /// returns True for any item then it remains in the set, otherwise
        /// it's dropped.
        /// </summary>
        /// <param name="set">HSet</param>
        /// <param name="pred">Predicate</param>
        /// <returns>Filtered enumerable</returns>
        [Pure]
        public HashSet<EqA, A> Where(Func<A, bool> pred) =>
            Filter(pred);

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
            Find(key).ToSeq();

        /// <summary>
        /// Retrieve a value from the set by key and pattern match the
        /// result.
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>Found value</returns>
        [Pure]
        public R Find<R>(A key, Func<A, R> Some, Func<R> None) =>
            Find(key).Match(Some, None);

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
            Value.ContainsKey(key);

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
            Prelude.Seq(this);

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

        /// <summary>
        /// Implicit conversion from an untyped empty list
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator HashSet<EqA, A>(SeqEmpty _) =>
            Empty;

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
        public bool IsProperSubsetOf(IEnumerable<A> other) =>
            Value.IsProperSubsetOf(other);

        /// <summary>
        /// Returns True if 'other' is a proper superset of this set
        /// </summary>
        /// <returns>True if 'other' is a proper superset of this set</returns>
        [Pure]
        public bool IsProperSupersetOf(IEnumerable<A> other) =>
            Value.IsProperSupersetOf(other);

        /// <summary>
        /// Returns True if 'other' is a superset of this set
        /// </summary>
        /// <returns>True if 'other' is a superset of this set</returns>
        [Pure]
        public bool IsSubsetOf(IEnumerable<A> other) =>
            Value.IsSubsetOf(other);

        /// <summary>
        /// Returns True if 'other' is a superset of this set
        /// </summary>
        /// <returns>True if 'other' is a superset of this set</returns>
        [Pure]
        public bool IsSupersetOf(IEnumerable<A> rhs) =>
            Value.IsSupersetOf(rhs);

        /// <summary>
        /// Returns the elements that are in both this and other
        /// </summary>
        [Pure]
        public HashSet<EqA, A> Intersect(IEnumerable<A> rhs) =>
            Wrap(Value.Intersect(rhs));

        /// <summary>
        /// Returns True if other overlaps this set
        /// </summary>
        [Pure]
        public bool Overlaps(IEnumerable<A> other) =>
            Value.Overlaps(other);

        /// <summary>
        /// Returns this - other.  Only the items in this that are not in 
        /// other will be returned.
        /// </summary>
        [Pure]
        public HashSet<EqA, A> Except(IEnumerable<A> rhs) =>
            Wrap(Value.Except(rhs));

        /// <summary>
        /// Only items that are in one set or the other will be returned.
        /// If an item is in both, it is dropped.
        /// </summary>
        [Pure]
        public HashSet<EqA, A> SymmetricExcept(HashSet<EqA, A> rhs) =>
            Wrap(Value.SymmetricExcept(rhs.Value));

        /// <summary>
        /// Only items that are in one set or the other will be returned.
        /// If an item is in both, it is dropped.
        /// </summary>
        [Pure]
        public HashSet<EqA, A> SymmetricExcept(IEnumerable<A> rhs) =>
            Wrap(Value.SymmetricExcept(rhs));

        /// <summary>
        /// Finds the union of two sets and produces a new set with 
        /// the results
        /// </summary>
        /// <param name="other">Other set to union with</param>
        /// <returns>A set which contains all items from both sets</returns>
        [Pure]
        public HashSet<EqA, A> Union(IEnumerable<A> rhs) =>
            this.TryAddRange(rhs);

        /// <summary>
        /// Copy the items from the set into the specified array
        /// </summary>
        /// <param name="array">Array to copy to</param>
        /// <param name="index">Index into the array to start</param>
        public Unit CopyTo(A[] array, int index)
        {
            var max = array.Length;
            var iter = GetEnumerator();
            for (var i = index; i < max && iter.MoveNext(); i++)
            {
                array[i] = iter.Current;
            }
            return default;
        }

        /// <summary>
        /// Copy the items from the set into the specified array
        /// </summary>
        /// <param name="array">Array to copy to</param>
        /// <param name="index">Index into the array to start</param>
        public Unit CopyTo(System.Array array, int index)
        {
            var max = array.Length;
            var iter = GetEnumerator();
            for (var i = index; i < max && iter.MoveNext(); i++)
            {
                array.SetValue(iter.Current, i);
            }
            return default;
        }

        [Pure]
        public override bool Equals(object obj) =>
            obj is HashSet<EqA, A> hs && Equals(hs);

        [Pure]
        public override int GetHashCode() =>
            Value.GetHashCode();

        [Pure]
        public HashSet<B> Bind<B>(Func<A, HashSet<B>> f)
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
            return new HashSet<B>(Yield(), true);
        }

        [Pure]
        public HashSet<C> SelectMany<B, C>(Func<A, HashSet<B>> bind, Func<A, B, C> project)
        {
            var self = this;

            IEnumerable<C> Yield()
            {
                foreach (var x in self.AsEnumerable())
                {
                    foreach (var y in bind(x))
                    {
                        yield return project(x, y);
                    }
                }
            }
            return new HashSet<C>(Yield(), true);
        }
    }
}

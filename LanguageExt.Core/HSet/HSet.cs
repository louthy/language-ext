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
    public struct HSet<A> :
        Monoid<HSet<A>>,
        MonadPlus<A>,
        Difference<HSet<A>>,
        Eq<HSet<A>>,
        IReadOnlyCollection<A>,
        IEquatable<HSet<A>>,
        ICollection<A>,
        ISet<A>,
        ICollection
    {
        public static readonly HSet<A> Empty = new HSet<A>(HSetInternal<A>.Empty);

        readonly HSetInternal<A> value;
        HSetInternal<A> Value => value ?? HSetInternal<A>.Empty;

        internal HSet(HSetInternal<A> value)
        {
            this.value = value;
        }

        HSet<A> Wrap(HSetInternal<A> value) =>
            new HSet<A>(value);

        static HSet<U> Wrap<U>(HSetInternal<U> value) =>
            new HSet<U>(value);

        /// <summary>
        /// Ctor that takes an initial (distinct) set of items
        /// </summary>
        /// <param name="items"></param>
        internal HSet(IEnumerable<A> items, bool checkUniqueness = false)
        {
            value = new HSetInternal<A>(items, checkUniqueness);
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
        public HSet<R> Map<R>(Func<A, R> mapper) =>
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
        public HSet<A> Filter(Func<A, bool> pred) =>
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
        public HSet<R> Select<R>(Func<A, R> mapper) =>
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
        public HSet<A> Where(Func<A, bool> pred) =>
            Wrap(Value.Filter(pred));

        /// <summary>
        /// Add an item to the set
        /// </summary>
        /// <param name="value">Value to add to the set</param>
        /// <returns>New set with the item added</returns>
        [Pure]
        public HSet<A> Add(A key) =>
            Wrap(Value.Add(key));

        /// <summary>
        /// Attempt to add an item to the set.  If an item already
        /// exists then return the Set as-is.
        /// </summary>
        /// <param name="value">Value to add to the set</param>
        /// <returns>New set with the item maybe added</returns>
        [Pure]
        public HSet<A> TryAdd(A key) =>
            Wrap(Value.TryAdd(key));

        /// <summary>
        /// Add an item to the set.  If an item already
        /// exists then replace it.
        /// </summary>
        /// <param name="value">Value to add to the set</param>
        /// <returns>New set with the item maybe added</returns>
        [Pure]
        public HSet<A> AddOrUpdate(A key) =>
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
        public HSet<A> AddRange(IEnumerable<A> range) =>
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
        public HSet<A> TryAddRange(IEnumerable<A> range) =>
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
        public HSet<A> AddOrUpdateRange(IEnumerable<A> range) =>
            Wrap(Value.AddOrUpdateRange(range));

        /// <summary>
        /// Atomically removes an item from the set
        /// If the key doesn't exists, the request is ignored.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>New map with the item removed</returns>
        [Pure]
        public HSet<A> Remove(A key) =>
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
        public HSet<A> SetItem(A key) =>
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
        public HSet<A> TrySetItem(A key) =>
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
        public HSet<A> Clear() =>
            Empty;

        /// <summary>
        /// Atomically sets a series of items
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys aren't in the map</exception>
        /// <returns>New HSet with the items set</returns>
        [Pure]
        public HSet<A> SetItems(IEnumerable<A> items) =>
            Wrap(Value.SetItems(items));

        /// <summary>
        /// Atomically sets a series of items.  If any of the items don't exist then they're silently ignored.
        /// </summary>
        /// <param name="items">Items to set</param>
        /// <returns>New HSet with the items set</returns>
        [Pure]
        public HSet<A> TrySetItems(IEnumerable<A> items) =>
            Wrap(Value.TrySetItems(items));

        /// <summary>
        /// Atomically removes a list of keys from the set
        /// </summary>
        /// <param name="keys">Keys to remove</param>
        /// <returns>New HSet with the items removed</returns>
        [Pure]
        public HSet<A> RemoveRange(IEnumerable<A> keys) =>
            Wrap(Value.RemoveRange(keys));

        #region IEnumerable interface

        /// <summary>
        /// GetEnumerator - IEnumerable interface
        /// </summary>
        public IEnumerator<A> GetEnumerator() =>
            Value.GetEnumerator();

        /// <summary>
        /// GetEnumerator - IEnumerable interface
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator() =>
            Value.GetEnumerator();

        public IEnumerable<A> AsEnumerable() =>
            Value.AsEnumerable();

        #endregion

        [Pure]
        public static HSet<A> operator +(HSet<A> lhs, HSet<A> rhs) =>
            lhs.Append(rhs);

        [Pure]
        public HSet<A> Append(HSet<A> rhs) =>
            Wrap(Value.Append(rhs.Value));

        [Pure]
        public static HSet<A> operator -(HSet<A> lhs, HSet<A> rhs) =>
            lhs.Subtract(rhs);

        [Pure]
        public HSet<A> Subtract(HSet<A> rhs) =>
            Wrap(Value.Subtract(rhs.Value));

        [Pure]
        public static bool operator ==(HSet<A> lhs, HSet<A> rhs) =>
            lhs.Equals(rhs);

        [Pure]
        public static bool operator !=(HSet<A> lhs, HSet<A> rhs) =>
            !(lhs == rhs);

        [Pure]
        public bool Equals(HSet<A> other) =>
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
        public bool IsSupersetOf(IEnumerable<A> other) =>
            Value.IsSupersetOf(other);

        /// <summary>
        /// Returns True if other overlaps this set
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="setA">Set A</param>
        /// <param name="setB">Set B</param>
        /// <returns>True if other overlaps this set</returns>
        [Pure]
        public bool Overlaps(IEnumerable<A> other) =>
            Value.Overlaps(other);

        /// <summary>
        /// Returns the elements that are in both this and other
        /// </summary>
        [Pure]
        public HSet<A> Intersect(IEnumerable<A> other)
        {
            var res = new List<A>();
            foreach (var item in other)
            {
                if (Contains(item)) res.Add(item);
            }
            return new HSet<A>(res);
        }

        /// <summary>
        /// Returns this - other.  Only the items in this that are not in 
        /// other will be returned.
        /// </summary>
        [Pure]
        public HSet<A> Except(IEnumerable<A> other)
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
        public HSet<A> SymmetricExcept(IEnumerable<A> other)
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

            return new HSet<A>(res);
        }

        /// <summary>
        /// Finds the union of two sets and produces a new set with 
        /// the results
        /// </summary>
        /// <param name="other">Other set to union with</param>
        /// <returns>A set which contains all items from both sets</returns>
        [Pure]
        public HSet<A> Union(IEnumerable<A> other)
        {
            var self = this;
            foreach (var item in other)
            {
                self = self.TryAdd(item);
            }
            return self;
        }

        public bool SetEquals(IEnumerable<A> other) =>
            Value.SetEquals(other);

        [Obsolete]
        [EditorBrowsable(EditorBrowsableState.Never)]
        bool ISet<A>.Add(A item)
        {
            throw new NotSupportedException();
        }

        [Obsolete]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void UnionWith(IEnumerable<A> other)
        {
            throw new NotSupportedException();
        }

        [Obsolete]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void IntersectWith(IEnumerable<A> other)
        {
            throw new NotSupportedException();
        }

        [Obsolete]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ExceptWith(IEnumerable<A> other)
        {
            throw new NotSupportedException();
        }

        [Obsolete]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SymmetricExceptWith(IEnumerable<A> other)
        {
            throw new NotSupportedException();
        }

        [Obsolete]
        [EditorBrowsable(EditorBrowsableState.Never)]
        void ICollection<A>.Add(A item)
        {
            throw new NotSupportedException();
        }

        [Obsolete]
        [EditorBrowsable(EditorBrowsableState.Never)]
        void ICollection<A>.Clear()
        {
            throw new NotSupportedException();
        }

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
        public void CopyTo(Array array, int index) =>
            Value.CopyTo(array, index);

        bool ICollection<A>.Remove(A item)
        {
            throw new NotSupportedException();
        }

        HSet<A> As(Monad<A> ma) => (HSet<A>)ma;
        HSet<A> As(MonadPlus<A> ma) => (HSet<A>)ma;
        HSet<A> As(Foldable<A> ma) => (HSet<A>)ma;
        HSet<A> As(Functor<A> ma) => (HSet<A>)ma;

        [Pure]
        HSet<A> Monoid<HSet<A>>.Empty() =>
            HSet<A>.Empty;

        [Pure]
        public HSet<A> Append(HSet<A> x, HSet<A> y) =>
            x.Append(y);

        [Pure]
        public MonadPlus<A> Plus(MonadPlus<A> a, MonadPlus<A> b) =>
            As(a).Append(As(b));

        [Pure]
        public MonadPlus<A> Zero() =>
            HSet<A>.Empty;

        [Pure]
        public Monad<A> Return(A x, params A[] xs) =>
            new HSet<A>(x.Cons(xs));

        [Pure]
        public Monad<A> Return(IEnumerable<A> xs) =>
            new HSet<A>(xs);

        [Pure]
        public MB Bind<MB, B>(Monad<A> ma, Func<A, MB> f) where MB : struct, Monad<B> =>
            TypeClass.Return<MB, B>(BindSeq<MB, B>(ma, f));

        [Pure]
        public Monad<B> Bind<B>(Monad<A> ma, Func<A, Monad<B>> f) =>
            new HSet<B>(BindSeq(ma, f));

        [Pure]
        IEnumerable<B> BindSeq<MB, B>(Monad<A> ma, Func<A, MB> f) where MB : struct, Monad<B>
        {
            var xs = As(ma);
            foreach (var x in xs)
            {
                var b = f(x);
                foreach (var y in TypeClass.toSeq(b as Foldable<B>))
                {
                    yield return y;
                }
            }
        }

        [Pure]
        IEnumerable<B> BindSeq<B>(Monad<A> ma, Func<A, Monad<B>> f)
        {
            var xs = As(ma);
            foreach (var x in xs)
            {
                var b = f(x);
                foreach (var y in TypeClass.toSeq(b as Foldable<B>))
                {
                    yield return y;
                }
            }
        }

        [Pure]
        public Monad<A> Fail(Exception err = null) =>
            HSet<A>.Empty;

        [Pure]
        public Monad<A> Fail<F>(F err = default(F)) =>
            HSet<A>.Empty;

        [Pure]
        public Functor<B> Map<B>(Functor<A> fa, Func<A, B> f) =>
            As(fa).Map(f);

        [Pure]
        public S Fold<S>(Foldable<A> fa, S state, Func<S, A, S> f) =>
            As(fa).Fold(state, f);

        [Pure]
        public S FoldBack<S>(Foldable<A> fa, S state, Func<S, A, S> f) =>
            As(fa).Reverse().Fold(state, f);

        [Pure]
        public HSet<A> Difference(HSet<A> x, HSet<A> y) =>
            As(x) - As(y);

        [Pure]
        public bool Equals(HSet<A> x, HSet<A> y) =>
            As(x) == As(y);

        [Pure]
        public override bool Equals(object obj) =>
            !ReferenceEquals(obj, null) && obj is HSet<A> && Equals(this, (HSet<A>)obj);

        [Pure]
        public override int GetHashCode() =>
            Value.GetHashCode();
    }
}

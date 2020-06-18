using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    /// <summary>
    /// Map type indexed both on the A and B for rapid lookup of either
    /// </summary>
    /// <typeparam name="A">A</typeparam>
    /// <typeparam name="B">B</typeparam>
    [Serializable]
    public struct BiMap<A, B> :
        IEnumerable<(A Left, B Right)>,
        IComparable<BiMap<A, B>>,
        IEquatable<BiMap<A, B>>
    {
        readonly Map<A, B> Left;
        readonly Map<B, A> Right;

        BiMap(Map<A, B> left, Map<B, A> right)
        {
            Left = left;
            Right = right;
        }

        public BiMap(IEnumerable<(A Left, B Right)> items) : this(items, true)
        { }

        public BiMap(IEnumerable<(A Left, B Right)> items, bool tryAdd) : 
            this(new Map<A, B>(items), new Map<B, A>(items.Map(pair => (pair.Right, pair.Left)))) { }


        /// <summary>
        /// Atomically adds a new item to the map
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if the key already exists</exception>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the key or value are null</exception>
        /// <returns>New Map with the item added</returns>
        [Pure]
        public BiMap<A, B> Add(A left, B right) =>
            new BiMap<A, B>(Left.Add(left, right), Right.Add(right, left));

        /// <summary>
        /// Atomically adds a new item to the map.
        /// If the key already exists, then the new item is ignored
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the key or value are null</exception>
        /// <returns>New Map with the item added</returns>
        [Pure]
        public BiMap<A, B> TryAdd(A left, B right) =>
            new BiMap<A, B>(Left.TryAdd(left, right), Right.TryAdd(right, left));

        /// <summary>
        /// Atomically adds a range of items to the map.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of tuples to add</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys already exist</exception>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public BiMap<A, B> AddRange(IEnumerable<(A, B)> range) =>
            new BiMap<A, B>(Left.AddRange(range), Right.AddRange(range.Map(pair => (pair.Item2, pair.Item1))));

        /// <summary>
        /// Atomically adds a range of items to the map.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of tuples to add</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys already exist</exception>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public BiMap<A, B> AddRange(IEnumerable<Tuple<A, B>> range) =>
            new BiMap<A, B>(Left.AddRange(range), Right.AddRange(range.Map(pair => (pair.Item2, pair.Item1))));

        /// <summary>
        /// Atomically adds a range of items to the map.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of tuples to add</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys already exist</exception>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public BiMap<A, B> AddRange(IEnumerable<KeyValuePair<A, B>> range) =>
            new BiMap<A, B>(Left.AddRange(range), Right.AddRange(range.Map(pair => (pair.Value, pair.Key))));

        /// <summary>
        /// Atomically adds a range of items to the map.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of tuples to add</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys already exist</exception>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public BiMap<A, B> TryAddRange(IEnumerable<(A, B)> range) =>
            new BiMap<A, B>(Left.TryAddRange(range), Right.TryAddRange(range.Map(pair => (pair.Item2, pair.Item1))));

        /// <summary>
        /// Atomically adds a range of items to the map.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of tuples to add</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys already exist</exception>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public BiMap<A, B> TryAddRange(IEnumerable<Tuple<A, B>> range) =>
            new BiMap<A, B>(Left.TryAddRange(range), Right.TryAddRange(range.Map(pair => (pair.Item2, pair.Item1))));

        /// <summary>
        /// Atomically adds a range of items to the map.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of tuples to add</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if any of the keys already exist</exception>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public BiMap<A, B> TryAddRange(IEnumerable<KeyValuePair<A, B>> range) =>
            new BiMap<A, B>(Left.TryAddRange(range), Right.TryAddRange(range.Map(pair => (pair.Value, pair.Key))));


        /// <summary>
        /// Atomically adds a range of items to the map.  If any of the keys exist already
        /// then they're replaced.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of tuples to add</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public BiMap<A, B> AddOrUpdateRange(IEnumerable<Tuple<A, B>> range) =>
            new BiMap<A, B>(Left.AddOrUpdateRange(range), Right.AddOrUpdateRange(range.Map(pair => (pair.Item2, pair.Item1))));

        /// <summary>
        /// Atomically adds a range of items to the map.  If any of the keys exist already
        /// then they're replaced.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of tuples to add</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public BiMap<A, B> AddOrUpdateRange(IEnumerable<(A, B)> range) =>
            new BiMap<A, B>(Left.AddOrUpdateRange(range), Right.AddOrUpdateRange(range.Map(pair => (pair.Item2, pair.Item1))));

        /// <summary>
        /// Atomically adds a range of items to the map.  If any of the keys exist already
        /// then they're replaced.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="range">Range of KeyValuePairs to add</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keys or values are null</exception>
        /// <returns>New Map with the items added</returns>
        [Pure]
        public BiMap<A, B> AddOrUpdateRange(IEnumerable<KeyValuePair<A, B>> range) =>
            new BiMap<A, B>(Left.AddOrUpdateRange(range), Right.AddOrUpdateRange(range.Map(pair => (pair.Value, pair.Key))));

        /// <summary>
        /// Atomically removes an item from the map
        /// If the key doesn't exists, the request is ignored.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>New map with the item removed</returns>
        [Pure]
        public BiMap<A, B> Remove(A left) =>
            new BiMap<A, B>(Left.Remove(left), Left.Find(left).Map(Right.Remove).IfNone(Right));

        /// <summary>
        /// Atomically removes an item from the map
        /// If the key doesn't exists, the request is ignored.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>New map with the item removed</returns>
        [Pure]
        public BiMap<A, B> Remove(B right) =>
            new BiMap<A, B>(Right.Find(right).Map(Left.Remove).IfNone(Left), Right.Remove(right));


        /// <summary>
        /// 'this' accessor
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>value</returns>
        [Pure]
        public A this[B value] => Right[value];

        /// <summary>
        /// 'this' accessor
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>value</returns>
        [Pure]
        public B this[A value] => Left[value];

        /// <summary>
        /// Is the map empty
        /// </summary>
        [Pure]
        public bool IsEmpty => Left.IsEmpty;

        /// <summary>
        /// Number of items in the map
        /// </summary>
        [Pure]
        public int Count => Left.Count;

        /// <summary>
        /// Alias of Count
        /// </summary>
        [Pure]
        public int Length => Left.Length;

        /// <summary>
        /// Atomically updates an existing item
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the key or value are null</exception>
        /// <returns>New Map with the item added</returns>
        [Pure]
        public BiMap<A, B> SetItem(A left, B right) =>
            new BiMap<A, B>(Left.SetItem(left, right), Right.SetItem(right, left));

        /// <summary>
        /// Atomically updates an existing item, unless it doesn't exist, in which case 
        /// it is ignored
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the value is null</exception>
        /// <returns>New Map with the item added</returns>
        [Pure]
        public BiMap<A, B> TrySetItem(A left, B right) =>
            new BiMap<A, B>(Left.TrySetItem(left, right), Right.SetItem(right, left));

        /// <summary>
        /// Atomically adds a new item to the map.
        /// If the key already exists, the new item replaces it.
        /// </summary>
        /// <remarks>Null is not allowed for a Key or a Value</remarks>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException the key or value are null</exception>
        /// <returns>New Map with the item added</returns>
        [Pure]
        public BiMap<A, B> AddOrUpdate(A left, B right) =>
            new BiMap<A, B>(Left.AddOrUpdate(left, right), Right.AddOrUpdate(right, left));




        /// <summary>
        /// Checks for existence of a key in the map
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True if an item with the key supplied is in the map</returns>

        [Pure]
        public bool ContainsKey(A value) => Left.ContainsKey(value);

        /// <summary>
        /// Checks for existence of a key in the map
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True if an item with the key supplied is in the map</returns>
        [Pure]
        public bool ContainsKey(B value) => Right.ContainsKey(value);

        /// <summary>
        /// Clears all items from the map 
        /// </summary>
        /// <remarks>Functionally equivalent to calling Map.empty as the original structure is untouched</remarks>
        /// <returns>Empty map</returns>
        [Pure]
        public BiMap<A, B> Clear() => default;

        /// Enumerable of map lefts in-order
        /// </summary>
        [Pure]
        public IEnumerable<A> LeftKeys => Left.Keys;

        /// <summary>
        /// Enumerable of map rights in-order
        /// </summary>
        [Pure]
        public IEnumerable<B> RightKeys => Right.Keys;

        /// Enumerable of map lefts in-rights-order
        /// </summary>
        [Pure]
        public IEnumerable<B> LeftValues => Left.Values;

        /// <summary>
        /// Enumerable of map rights in-lefts-order
        /// </summary>
        [Pure]
        public IEnumerable<A> RightValues => Right.Values;

        /// <summary>
        /// Convert the map to an `IReadOnlyDictionary`
        /// </summary>
        /// <returns></returns>
        [Pure]
        public IReadOnlyDictionary<A, B> ToDictionaryLeft() => Left.ToDictionary();

        /// <summary>
        /// Convert the map to an `IReadOnlyDictionary`
        /// </summary>
        /// <returns></returns>
        [Pure]
        public IReadOnlyDictionary<B, A> ToDictionaryRight() => Right.ToDictionary();

        /// <summary>
        /// Enumerable of in-order tuples that make up the map
        /// </summary>
        /// <returns>Tuples</returns>
        [Pure]
        [Obsolete("Use Pairs instead")]
        public IEnumerable<(A Key, B Value)> Tuples =>
            Left.Pairs;

        /// <summary>
        /// Enumerable of in-order tuples that make up the map
        /// </summary>
        /// <returns>Tuples</returns>
        [Pure]
        public IEnumerable<(A Key, B Value)> Pairs =>
            Left.Pairs;

        [Pure]
        public Seq<(A Key, B Value)> ToSeq() =>
            Prelude.Seq(this);

        [Pure]
        public bool Equals(BiMap<A, B> y) =>
            Left.Equals(y.Left);



        [Pure]
        public Option<B> Find(A value) => Left.Find(value);

        [Pure]
        public Option<A> Find(B value) => Right.Find(value);

        [Pure]
        public MapEnumerator<A, B> GetEnumerator() =>
            Left.GetEnumerator();

        [Pure]
        IEnumerator<(A Left, B Right)> IEnumerable<(A Left, B Right)>.GetEnumerator() =>
            Left.GetEnumerator();

        [Pure]
        IEnumerator IEnumerable.GetEnumerator() =>
            Left.GetEnumerator();

        [Pure]
        public int CompareTo(BiMap<A, B> rhs) =>
            Left.CompareTo(rhs.Left);

        [Pure]
        public override bool Equals(object obj) =>
            obj is BiMap<A, B> bm && Equals(bm);

        [Pure]
        public static bool operator ==(BiMap<A, B> lhs, BiMap<A, B> rhs) =>
            lhs.Left.Equals(rhs.Left);

        [Pure]
        public static bool operator !=(BiMap<A, B> lhs, BiMap<A, B> rhs) =>
            !(lhs == rhs);

        [Pure]
        public static bool operator <(BiMap<A, B> lhs, BiMap<A, B> rhs) =>
            lhs.CompareTo(rhs) < 0;

        [Pure]
        public static bool operator <=(BiMap<A, B> lhs, BiMap<A, B> rhs) =>
            lhs.CompareTo(rhs) <= 0;

        [Pure]
        public static bool operator >(BiMap<A, B> lhs, BiMap<A, B> rhs) =>
            lhs.CompareTo(rhs) > 0;

        [Pure]
        public static bool operator >=(BiMap<A, B> lhs, BiMap<A, B> rhs) =>
            lhs.CompareTo(rhs) >= 0;

        [Pure]
        public static BiMap<A, B> operator +(BiMap<A, B> lhs, BiMap<A, B> rhs) =>
            new BiMap<A, B>(lhs.Left + rhs.Left, lhs.Right + rhs.Right);

        [Pure]
        public static BiMap<A, B> operator -(BiMap<A, B> lhs, BiMap<A, B> rhs) =>
            new BiMap<A, B>(lhs.Left - rhs.Left, lhs.Right - rhs.Right);

        [Pure]
        public override int GetHashCode() =>
            Left.GetHashCode();



        /// <summary>
        /// Implicit conversion from an untyped empty list
        /// </summary>
        public static implicit operator BiMap<A, B>(SeqEmpty _) =>
            default;
    }
}

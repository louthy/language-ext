using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using static LanguageExt.Prelude;
using LanguageExt.TypeClasses;
using LanguageExt.ClassInstances;
using System.Runtime.CompilerServices;
using LanguageExt.ClassInstances.Pred;

namespace LanguageExt;

/// <summary>
/// Immutable list with validation predicate
/// </summary>
/// <typeparam name="PredItem">Predicate instance to run when the type is constructed</typeparam>
/// <typeparam name="A">Value type</typeparam>
[Serializable]
public class Lst<PredList, PredItem, A> :
    IReadOnlyList<A>,
    IEquatable<Lst<PredList, PredItem, A>>,
    IComparable<Lst<PredList, PredItem, A>>,
    IComparable,
    ListInfo
    where PredList : Pred<ListInfo>
    where PredItem : Pred<A>
{
    readonly LstInternal<True<A>, A> value;

    /// <summary>
    /// Ctor
    /// </summary>
    public Lst(IEnumerable<A> initial)
    {
        if (initial == null) throw new ArgumentNullException(nameof(initial));
        value = new LstInternal<True<A>, A>(initial);
        if (!PredList.True(this)) throw new ArgumentOutOfRangeException(nameof(value));
    }

    /// <summary>
    /// Ctor
    /// </summary>
    Lst(LstInternal<True<A>, A> root)
    {
        value = root;
        if (root is null) throw new ArgumentNullException(nameof(root));
        if (!PredList.True(this)) throw new ArgumentOutOfRangeException(nameof(value));
    }

    internal LstInternal<True<A>, A> Value
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => value ?? LstInternal<True<A>, A>.Empty;
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
    public object? Case =>
        IsEmpty 
            ? null
            : Count == 1
                ? this[0]
                : toSeq(this).Case;

    Lst<PredList, PredItem, A> Wrap(LstInternal<True<A>, A> list)=>
        new (list);

    static Lst<PredList, PREDITEM, T> Wrap<PREDITEM, T>(LstInternal<True<T>, T> list) 
        where PREDITEM : Pred<T> =>
        new (list);

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
        Value.Find(a => EqDefault<A>.Equals(a, value)).IsSome;

    /// <summary>
    /// Contains with provided Eq class instance
    /// </summary>
    /// <typeparam name="EqA">Eq class instance</typeparam>
    /// <param name="value">Value to test</param>
    /// <returns>True if collection contains value</returns>
    [Pure]
    public bool Contains<EqA>(A value) where EqA : Eq<A> =>
        Value.Find(a => EqA.Equals(a, value)).IsSome;

    /// <summary>
    /// Add an item to the end of the list
    /// </summary>
    [Pure]
    public Lst<PredList, PredItem, A> Add(A value) =>
        PredItem.True(value)
            ? Wrap(Value.Add(value))
            : throw new ArgumentOutOfRangeException(nameof(value));

    /// <summary>
    /// Add a range of items to the end of the list
    /// </summary>
    [Pure]
    public Lst<PredList, PredItem, A> AddRange(IEnumerable<A> items)
    {
        var self = Value;
        foreach(var item in items)
        {
            if(!PredItem.True(item)) throw new ArgumentOutOfRangeException(nameof(items));
            self = self.Add(item);
        }
        return new Lst<PredList, PredItem, A>(self);
    }

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

    /// <summary>
    /// Get enumerator
    /// </summary>
    [Pure]
    public IEnumerator<A> GetEnumerator() =>
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

    /// <summary>
    /// Find the index of an item
    /// </summary>
    [Pure]
    public int IndexOf(A item, int index = 0, int count = -1, IEqualityComparer<A>? equalityComparer = null) =>
        Value.IndexOf(item, index, count, equalityComparer);

    /// <summary>
    /// Insert value at specified index
    /// </summary>
    [Pure]
    public Lst<PredList, PredItem, A> Insert(int index, A value) =>
        PredItem.True(value)
            ? Wrap(Value.Insert(index, value))
            : throw new ArgumentOutOfRangeException(nameof(value));

    /// <summary>
    /// Insert range of values at specified index
    /// </summary>
    [Pure]
    public Lst<PredList, PredItem, A> InsertRange(int index, IEnumerable<A> items) =>
        Wrap(Value.InsertRange(index, items));

    /// <summary>
    /// Find the last index of an item in the list
    /// </summary>
    [Pure]
    public int LastIndexOf(A item, int index = 0, int count = -1, IEqualityComparer<A>? equalityComparer = null) =>
        Value.LastIndexOf(item, index, count, equalityComparer);

    /// <summary>
    /// Remove all items that match the value from the list
    /// </summary>
    [Pure]
    public Lst<PredList, PredItem, A> Remove(A value) =>
        Wrap(Value.Remove(value));

    /// <summary>
    /// Remove all items that match the value from the list
    /// </summary>
    [Pure]
    public Lst<PredList, PredItem, A> Remove(A value, IEqualityComparer<A> equalityComparer) =>
        Wrap(Value.Remove(value, equalityComparer));

    /// <summary>
    /// Remove all items that match a predicate
    /// </summary>
    [Pure]
    public Lst<PredList, PredItem, A> RemoveAll(Func<A, bool> pred) =>
        Wrap(Value.RemoveAll(pred));

    /// <summary>
    /// Remove item at location
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    [Pure]
    public Lst<PredList, PredItem, A> RemoveAt(int index) =>
        Wrap(Value.RemoveAt(index));

    /// <summary>
    /// Remove a range of items
    /// </summary>
    [Pure]
    public Lst<PredList, PredItem, A> RemoveRange(int index, int count) =>
        Wrap(Value.RemoveRange(index, count));

    /// <summary>
    /// Set an item at the specified index
    /// </summary>
    [Pure]
    public Lst<PredList, PredItem, A> SetItem(int index, A value) =>
        PredItem.True(value)
            ? Wrap(Value.SetItem(index, value))
            : throw new ArgumentOutOfRangeException(nameof(value));

    [Pure]
    IEnumerator IEnumerable.GetEnumerator() =>
        new ListEnumerator<A>(Root, false, 0);

    [Pure]
    IEnumerator<A> IEnumerable<A>.GetEnumerator() =>
        new ListEnumerator<A>(Root, false, 0);

    [Pure]
    public IEnumerable<A> Skip(int amount) =>
        Value.Skip(amount);

    /// <summary>
    /// Reverse the order of the items in the list
    /// </summary>
    [Pure]
    public Lst<PredList, PredItem, A> Reverse() =>
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
    public Lst<PredList, PREDU, U> Map<PREDU, U>(Func<A, U> map) where PREDU : Pred<U> =>
        new(Value.Map(map));

    /// <summary>
    /// Map
    /// </summary>
    [Pure]
    public Lst<PredList, PredItem, A> Map(Func<A, A> map) =>
        new(Value.AsEnumerable().Map(map));

    /// <summary>
    /// Filter
    /// </summary>
    [Pure]
    public Lst<PredList, PredItem, A> Filter(Func<A, bool> pred) =>
        Wrap(Value.Filter(pred));

    [Pure]
    public static Lst<PredList, PredItem, A> operator +(Lst<PredList, PredItem, A> lhs, A rhs) =>
        lhs.Add(rhs);

    [Pure]
    public static Lst<PredList, PredItem, A> operator +(A lhs, Lst<PredList, PredItem, A> rhs) =>
        new (lhs.Cons(rhs));

    [Pure]
    public static Lst<PredList, PredItem, A> operator +(Lst<PredList, PredItem, A> lhs, Lst<PredList, PredItem, A> rhs) =>
        lhs.Append(rhs);

    [Pure]
    public Lst<PredList, PredItem, A> Append(Lst<PredList, PredItem, A> rhs) =>
        new (Value.Append(rhs));

    [Pure]
    public static Lst<PredList, PredItem, A> operator -(Lst<PredList, PredItem, A> lhs, Lst<PredList, PredItem, A> rhs) =>
        lhs.Subtract(rhs);

    [Pure]
    public Lst<PredList, PredItem, A> Subtract(Lst<PredList, PredItem, A> rhs) =>
        Wrap(Value.Subtract(rhs.Value));

    [Pure]
    public override bool Equals(object? obj) =>
        obj switch
        {
            Lst<PredList, PredItem, A> s => Equals(s),
            IEnumerable<A> e             => Equals(new Lst<PredList, PredItem, A>(e)),
            _                            => false
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
    public int CompareTo(object? obj) =>
        obj switch
        {
            Lst<PredList, PredItem, A> s => CompareTo(s),
            IEnumerable<A> e             => CompareTo(new Lst<PredList, PredItem, A>(e)),
            _                            => 1
        };

    [Pure]
    public bool Equals(Lst<PredList, PredItem, A>? other) =>
        other is not null && Value.Equals(other.Value);

    [Pure]
    public static bool operator ==(Lst<PredList, PredItem, A> lhs, Lst<PredList, PredItem, A> rhs) =>
        lhs.Value.Equals(rhs.Value);

    [Pure]
    public static bool operator !=(Lst<PredList, PredItem, A> lhs, Lst<PredList, PredItem, A> rhs) =>
        !(lhs == rhs);

    [Pure]
    public static bool operator <(Lst<PredList, PredItem, A> lhs, Lst<PredList, PredItem, A> rhs) =>
        lhs.CompareTo(rhs) < 0;

    [Pure]
    public static bool operator <=(Lst<PredList, PredItem, A> lhs, Lst<PredList, PredItem, A> rhs) =>
        lhs.CompareTo(rhs) <= 0;

    [Pure]
    public static bool operator >(Lst<PredList, PredItem, A> lhs, Lst<PredList, PredItem, A> rhs) =>
        lhs.CompareTo(rhs) > 0;

    [Pure]
    public static bool operator >=(Lst<PredList, PredItem, A> lhs, Lst<PredList, PredItem, A> rhs) =>
        lhs.CompareTo(rhs) >= 0;

    [Pure]
    public Arr<A> ToArray() =>
        toArray(this);

    [Pure]
    public int CompareTo(Lst<PredList, PredItem, A>? other) =>
        other is null ? 1 : Value.CompareTo(other.Value);

    [Pure]
    public static implicit operator Lst<PredList, PredItem, A>(Lst<A> list)
    {
        foreach(var item in list)
        {
            if (!PredItem.True(item)) throw new InvalidCastException("Implicit converson has failed the PredItem constraint");
        }
        return new Lst<PredList, PredItem, A>(list.Value);
    }

    [Pure]
    public static implicit operator Lst<A>(Lst<PredList, PredItem, A> list) =>
        new (list.Value);
}

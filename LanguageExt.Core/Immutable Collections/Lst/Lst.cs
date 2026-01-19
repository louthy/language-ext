using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Numerics;
using static LanguageExt.Prelude;
using LanguageExt.ClassInstances;
using LanguageExt.Traits;
using System.Runtime.CompilerServices;

namespace LanguageExt;

/// <summary>
/// Immutable list
/// </summary>
/// <typeparam name="A">Value type</typeparam>
[Serializable]
[CollectionBuilder(typeof(Lst), nameof(Lst.createRange))]
public readonly struct Lst<A> :
    IComparable<Lst<A>>,
    IComparable,
    IEnumerable<A>,
    IEquatable<Lst<A>>,
    IComparisonOperators<Lst<A>, Lst<A>, bool>,
    IAdditionOperators<Lst<A>, Lst<A>, Lst<A>>,
    ISubtractionOperators<Lst<A>, Lst<A>, Lst<A>>,
    IAdditiveIdentity<Lst<A>, Lst<A>>,
    Monoid<Lst<A>>,
    K<Lst, A>
{
    /// <summary>
    /// Empty list
    /// </summary>
    public static Lst<A> Empty { get; } = new (System.Array.Empty<A>().AsSpan());
    
    readonly LstInternal<A>? value;
    internal LstInternal<A> Value
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => value ?? LstInternal<A>.Empty;
    }

    /// <summary>
    /// Ctor
    /// </summary>
    public Lst(IEnumerable<A> initial) =>
        value = new LstInternal<A>(initial);

    /// <summary>
    /// Ctor
    /// </summary>
    public Lst(Iterator<A> initial) =>
        value = new LstInternal<A>(initial);

    /// <summary>
    /// Ctor
    /// </summary>
    public Lst(ReadOnlySpan<A> initial) =>
        value = new LstInternal<A>(initial);

    /// <summary>
    /// Ctor
    /// </summary>
    Lst(LstInternal<A> initial) =>
        value = initial;

    /// <summary>
    /// Ctor
    /// </summary>
    internal Lst(ListItem<A> root) =>
        value = new LstInternal<A>(root);

    public static Lst<A> FromFoldable<T, FS>(K<T, A> items)
        where T : Foldable<T, FS>
        where FS : allows ref struct =>
        Wrap(LstInternal<A>.FromFoldable<T, FS>(items));

    public static Lst<A> FromFoldableBack<T, FS>(K<T, A> items)
        where T : FoldableBack<T, FS>
        where FS : allows ref struct =>
        Wrap(LstInternal<A>.FromFoldableBack<T, FS>(items));
    
    ListItem<A> Root
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Value.Root;
    }

    [Pure]
    public bool IsEmpty =>
        Count == 0;

    /// <summary>
    /// Head lens
    /// </summary>
    [Pure]
    public static Lens<Lst<A>, A> head =>
        Lens<Lst<A>, A>.New(
            Get: la => la.Count == 0 ? throw new IndexOutOfRangeException() : la[0],
            Set: a => la => la.Count == 0 ? throw new IndexOutOfRangeException() : la.SetItem(0, a));

    /// <summary>
    /// Head or none lens
    /// </summary>
    [Pure]
    public static Lens<Lst<A>, Option<A>> headOrNone =>
        Lens<Lst<A>, Option<A>>.New(
            Get: la => la.Count == 0 ? None : Some(la[0]),
            Set: a => la => la.Count == 0 || a.IsNone ? la : la.SetItem(0, a.Value!));

    /// <summary>
    /// Tail lens
    /// </summary>
    [Pure]
    public static Lens<Lst<A>, A> tail =>
        Lens<Lst<A>, A>.New(
            Get: la => la.Count == 0 ? throw new IndexOutOfRangeException() : la[la.Count - 1],
            Set: a => la => la.Count == 0 ? throw new IndexOutOfRangeException() : la.SetItem(la.Count - 1, a));

    /// <summary>
    /// Tail or none lens
    /// </summary>
    [Pure]
    public static Lens<Lst<A>, Option<A>> tailOrNone =>
        Lens<Lst<A>, Option<A>>.New(
            Get: la => la.Count == 0 ? None : Some(la[la.Count - 1]),
            Set: a => la => la.Count == 0 || a.IsNone ? la : la.SetItem(la.Count - 1, a.Value!));

    /// <summary>
    /// Item at index lens
    /// </summary>
    [Pure]
    public static Lens<Lst<A>, A> item(int index) => Lens<Lst<A>, A>.New(
        Get: la => la.Count == 0 ? throw new IndexOutOfRangeException() : la[index],
        Set: a => la => la.Count == 0 ? throw new IndexOutOfRangeException() : la.SetItem(index, a));

    /// <summary>
    /// Item or none at index lens
    /// </summary>
    [Pure]
    public static Lens<Lst<A>, Option<A>> itemOrNone(int index) => Lens<Lst<A>, Option<A>>.New(
        Get: la => la.Count < index - 1 ? None : Some(la[index]),
        Set: a => la => la.Count < index - 1 || a.IsSome ? la : la.SetItem(index, a.Value!));

    /// <summary>
    /// Lens map
    /// </summary>
    [Pure]
    public static Lens<Lst<A>, Lst<B>> map<B>(Lens<A, B> lens) => Lens<Lst<A>, Lst<B>>.New(
        Get: la => la.Map(lens.Get).ToLst(),
        Set: lb => la => la.Zip(lb).Map(ab => lens.Set(ab.Second, ab.First)).ToLst());

    /// <summary>
    /// Index accessor
    /// </summary>
    [Pure]
    public A this[long index]
    {
        get
        {
            if (index < 0 || index >= Root.Count) throw new IndexOutOfRangeException();
            return ListModule.GetItem(Root, index);
        }
    }

    /// <summary>
    /// Safe index accessor
    /// </summary>
    [Pure]
    public Option<A> At(long index)
    {
        if (index < 0 || index >= Root.Count) return default;
        return ListModule.GetItem(Root, index);
    }

    /// <summary>
    /// Number of items in the list
    /// </summary>
    [Pure]
    public long Count =>
        Root.Count;

    /// <summary>
    /// Reverse the order of the items in the list
    /// </summary>
    [Pure]
    public Lst<A> Reverse()
    {
        var           root      = ListItem<A>.EmptyM;
        var           subIndex  = 0L;
        var           fa        = (K<Lst, A>)this;

        var foldState = fa.StepBackSetup<Lst, Lst.FoldState, A>();
        while (fa.StepBack(ref foldState, out var item))
        {
            root = ListModuleM.Insert(root, new ListItem<A>(1, 1, ListItem<A>.Empty, item, ListItem<A>.Empty), subIndex);
            subIndex++;
        }
        
        return new Lst<A>(new LstInternal<A>(root));
    }

    Lst<A> Wrap(LstInternal<A> list) =>
        new (list);

    static Lst<X> Wrap<X>(LstInternal<X> list) =>
        new (list);

    /// <summary>
    /// Find if a value is in the collection
    /// </summary>
    /// <param name="value">Value to test</param>
    /// <returns>True if collection contains value</returns>
    [Pure]
    public bool Contains(A value) =>
        Value.AsIterable().Find(a => EqDefault<A>.Equals(a, value)).IsSome;

    /// <summary>
    /// Contains with provided Eq class instance
    /// </summary>
    /// <typeparam name="EqA">Eq class instance</typeparam>
    /// <param name="value">Value to test</param>
    /// <returns>True if collection contains value</returns>
    [Pure]
    public bool Contains<EqA>(A value) where EqA : Eq<A> =>
        Value.AsIterable().Find(a => EqA.Equals(a, value)).IsSome;

    /// <summary>
    /// Add an item to the end of the list
    /// </summary>
    [Pure]
    public Lst<A> Add(A value) =>
        Wrap(Value.Add(value));

    /// <summary>
    /// Add a range of items to the end of the list
    /// </summary>
    [Pure]
    public Lst<A> AddRange(IEnumerable<A> items) =>
        Wrap(Value.AddRange(items));

    /// <summary>
    /// Clear the list
    /// </summary>
    [Pure]
    public Lst<A> Clear() =>
        Empty;
    
    /// <summary>
    /// Find the index of an item
    /// </summary>
    [Pure]
    public long IndexOf(A item, long index = 0, long count = -1, IEqualityComparer<A>? equalityComparer = null) =>
        Value.IndexOf(item, index, count, equalityComparer);

    /// <summary>
    /// Insert value at specified index
    /// </summary>
    [Pure]
    public Lst<A> Insert(long index, A value) =>
        Wrap(Value.Insert(index, value));

    /// <summary>
    /// Insert range of values at specified index
    /// </summary>
    [Pure]
    public Lst<A> InsertRange(long index, IEnumerable<A> items) =>
        Wrap(Value.InsertRange(index, items));

    /// <summary>
    /// Find the last index of an item in the list
    /// </summary>
    [Pure]
    public long LastIndexOf(A item, long index = 0, long count = -1, IEqualityComparer<A>? equalityComparer = null) =>
        Value.LastIndexOf(item, index, count, equalityComparer);

    /// <summary>
    /// Remove all items that match the value from the list
    /// </summary>
    [Pure]
    public Lst<A> Remove(A value) =>
        Wrap(Value.Remove(value));

    /// <summary>
    /// Remove all items that match the value from the list
    /// </summary>
    [Pure]
    public Lst<A> Remove(A value, IEqualityComparer<A> equalityComparer) =>
        Wrap(Value.Remove(value, equalityComparer));

    /// <summary>
    /// Remove all items that match a predicate
    /// </summary>
    [Pure]
    public Lst<A> RemoveAll(Func<A, bool> pred) =>
        Wrap(Value.RemoveAll(pred));

    /// <summary>
    /// Remove item at location
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    [Pure]
    public Lst<A> RemoveAt(long index) =>
        Wrap(Value.RemoveAt(index));

    /// <summary>
    /// Remove a range of items
    /// </summary>
    [Pure]
    public Lst<A> RemoveRange(long index, long count) =>
        Wrap(Value.RemoveRange(index, count));

    /// <summary>
    /// Set an item at the specified index
    /// </summary>
    [Pure]
    public Lst<A> SetItem(long index, A value) =>
        Wrap(Value.SetItem(index, value));

    /// <summary>
    /// Returns an enumerable range from the collection.  This is the fastest way of
    /// iterating sub-ranges of the collection.
    /// </summary>
    /// <param name="index">Index into the collection</param>
    /// <param name="count">Number of items to find</param>
    /// <returns>IEnumerable of items</returns>
    [Pure]
    public Iterable<A> FindRange(long index, long count) =>
        Value.FindRange(index, count);

    [Pure]
    IEnumerator IEnumerable.GetEnumerator() =>
        new ListEnumerator<A>(Root, 0);

    [Pure]
    IEnumerator<A> IEnumerable<A>.GetEnumerator() =>
        new ListEnumerator<A>(Root, 0);

    [Pure]
    public ListEnumerator<A> GetEnumerator() =>
        new (Root, 0);

    [Pure]
    public ListEnumeratorBack<A> GetEnumeratorBack() =>
        new (Root, 0);

    [Pure]
    public Seq<A> ToSeq() =>
        toSeq(this);

    /// <summary>
    /// Format the collection as `[a, b, c, ...]`
    /// The ellipsis is used for collections over 50 items
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
    public Iterable<A> AsIterable() =>
        Iterable.createRange(this);

    [Pure]
    public Lst<A> Skip(long amount) =>
        Value.Skip(amount);

    /// <summary>
    /// Impure iteration of the bound values in the structure
    /// </summary>
    /// <returns>
    /// Returns the original unmodified structure
    /// </returns>
    public Lst<A> Do(Action<A> f)
    {
        this.Iter(f);
        return this;
    }

    /// <summary>
    /// Map
    /// </summary>
    [Pure]
    public Lst<U> Map<U>(Func<A, U> map) =>
        Value.Map(map);
    
    /// <summary>
    /// Map each element of a structure to an action, evaluate these actions from
    /// left to right, and collect the results.
    /// </summary>
    /// <param name="f"></param>
    /// <param name="ta">Traversable structure</param>
    /// <typeparam name="F">Applicative functor trait</typeparam>
    /// <typeparam name="B">Bound value (output)</typeparam>
    [Pure]
    public K<F, Lst<B>> Traverse<F, B>(Func<A, K<F, B>> f) 
        where F : Applicative<F> =>
        F.Map(x => x.As(), Traversable.traverse(f, this));
    
    /// <summary>
    /// Map each element of a structure to an action, evaluate these actions from
    /// left to right, and collect the results.
    /// </summary>
    /// <param name="f"></param>
    /// <param name="ta">Traversable structure</param>
    /// <typeparam name="M">Monad trait</typeparam>
    /// <typeparam name="B">Bound value (output)</typeparam>
    [Pure]
    public K<M, Lst<B>> TraverseM<M, B>(Func<A, K<M, B>> f) 
        where M : Monad<M> =>
        M.Map(x => x.As(), Traversable.traverseM(f, this));
    
    /// <summary>
    /// Filter
    /// </summary>
    [Pure]
    public Lst<A> Filter(Func<A, bool> pred) =>
        Value.Filter(pred);

    [Pure]
    public static Lst<A> operator +(Lst<A> lhs, A rhs) =>
        lhs.Add(rhs);

    [Pure]
    public static Lst<A> operator +(A lhs, Lst<A> rhs) =>
        lhs.Cons(rhs);

    [Pure]
    public static Lst<A> operator +(Lst<A> lhs, Lst<A> rhs) =>
        lhs.Combine(rhs);

    /// <summary>
    /// Choice operator
    /// </summary>
    [Pure]
    public static Lst<A> operator |(Lst<A> x, K<Lst, A> y) =>
        x.Choose(y).As();

    /// <summary>
    /// Choice operator
    /// </summary>
    [Pure]
    public static Lst<A> operator |(K<Lst, A> x, Lst<A> y) =>
        x.Choose(y).As();
    
    [Pure]
    public Lst<A> Combine(Lst<A> rhs) =>
        new (Value.Combine(rhs.Value));
    
    [Pure]
    public static Lst<A> operator -(Lst<A> lhs, Lst<A> rhs) =>
        lhs.Subtract(rhs);

    [Pure]
    public Lst<A> Subtract(Lst<A> rhs) =>
        Wrap(Value.Subtract(rhs.Value));

    [Pure]
    public override bool Equals(object? obj) =>
        obj switch
        {
            Lst<A> s         => Equals(s),
            IEnumerable<A> e => Equals(e.AsIterable().ToLst()),
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
    public int CompareTo(object? obj) =>
        obj switch
        {
            Lst<A> s         => CompareTo(s),
            IEnumerable<A> e => CompareTo(e.AsIterable().ToLst()),
            _                => 1
        };

    [Pure]
    public bool Equals(Lst<A> other) =>
        Value.Equals(other.Value);

    [Pure]
    public static bool operator ==(Lst<A> lhs, Lst<A> rhs) =>
        lhs.Value.Equals(rhs.Value);

    [Pure]
    public static bool operator !=(Lst<A> lhs, Lst<A> rhs) =>
        !(lhs == rhs);

    [Pure]
    public static bool operator <(Lst<A> lhs, Lst<A> rhs) =>
        lhs.CompareTo(rhs) < 0;

    [Pure]
    public static bool operator <=(Lst<A> lhs, Lst<A> rhs) =>
        lhs.CompareTo(rhs) <= 0;

    [Pure]
    public static bool operator >(Lst<A> lhs, Lst<A> rhs) =>
        lhs.CompareTo(rhs) > 0;

    [Pure]
    public static bool operator >=(Lst<A> lhs, Lst<A> rhs) =>
        lhs.CompareTo(rhs) >= 0;

    [Pure]
    public Arr<A> ToArr() =>
        toArr(this);

    [Pure]
    internal A[] ToArray() =>
        Value.ToArray();

    [Pure]
    public int CompareTo(Lst<A> other) =>
        Value.CompareTo(other.Value);

    /// <summary>
    /// Implicit conversion from an untyped empty list
    /// </summary>
    public static implicit operator Lst<A>(UnitCollection _) =>
        Empty;

    public static Lst<A> AdditiveIdentity => 
        Empty;
}

using System;
using System.Linq;
using System.Numerics;
using LanguageExt.Traits;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;
using LanguageExt.ClassInstances;
using System.Runtime.CompilerServices;

namespace LanguageExt;

/// <summary>
/// An immutable array
/// </summary>
/// <remarks>
/// Native array O(1) read performance.  Modifications require copying of the entire backing array to generate the
/// newly transformed collection. This will be expensive for large collections but potentially much faster than any
/// other data structure for smaller collections: use `Seq` if you need array-like performance and the ability to
/// transform larger collections efficiently.</remarks>
/// <remarks>
/// Two methods that don't suffer this fate are `Take` and `Skip` which will do splicing of the backing array, like
/// splicing of `Span` and `ReadOnlySpan`.  That makes those operations incredibly fast, but be aware that can mean
/// old data behind held longer than you may like (a space leak). If that's the case, use `Clone` to just take the
/// snapshot/view data you want so the old references can be collected by the GC.
/// </remarks>
/// <typeparam name="A">Value type</typeparam>
[Serializable]
[CollectionBuilder(typeof(Arr), nameof(Arr.create))]
public readonly partial struct Arr<A> :
    IEquatable<Arr<A>>,
    IEnumerable<A>,
    IComparable<Arr<A>>,
    Monoid<Arr<A>>,
    IComparisonOperators<Arr<A>, Arr<A>, bool>,
    IAdditionOperators<Arr<A>, Arr<A>, Arr<A>>,
    IAdditiveIdentity<Arr<A>, Arr<A>>,
    TokenStream<Arr<A>, A>,
    IComparable,
    K<Arr, A>
{
    /// <summary>
    /// Empty array
    /// </summary>
    public static Arr<A> Empty { get; } = new (System.Array.Empty<A>());

    readonly A[]? value;
    readonly long start;
    readonly long length;
    readonly Atom<int>? hashCode;

    internal A[] Value => 
        value ?? Empty.Value;

    /// <summary>
    /// Ctor
    /// </summary>
    public Arr(IEnumerable<A> initial)
    {
        hashCode = Atom(0);
        value = initial.ToArray();
        start = 0;
        length = value.Length;
    }

    /// <summary>
    /// Ctor
    /// </summary>
    public Arr(ReadOnlySpan<A> initial)
    {
        hashCode = Atom(0);
        value = initial.ToArray();
        start = 0;
        length = value.Length;
    }

    /// <summary>
    /// Ctor
    /// </summary>
    internal Arr(A[] value)
    {
        hashCode = Atom(0);
        this.value = value;
        start = 0;
        length = value.Length;
    }

    /// <summary>
    /// Ctor
    /// </summary>
    internal Arr(A[] value, long start, long length)
    {
        if(start < 0) throw new ArgumentOutOfRangeException(nameof(start));
        if(start + length > value.Length) throw new ArgumentOutOfRangeException(nameof(length));
        hashCode = Atom(0);
        this.value = value;
        this.start = start;
        this.length = length;
    }
    
    /// <summary>
    /// Create a readonly span of this array.  This doesn't do any copying, so it is very fast.   
    /// </summary>
    /// <param name="start">Offset from the beginning of the array</param>
    /// <param name="count">The number of items to take. This will be clamped
    /// to the maximum number of items available</param>
    /// <returns>A read-only span of values</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown If the start index is outside the range of the array</exception>
    [Pure]
    public ReadOnlySpan<A> AsSpan() =>
        start > int.MaxValue
            ? throw new InvalidOperationException("Backing array is too big to return a view")
            : length > int.MaxValue
                ? throw new InvalidOperationException("Backing array is too big to return a view")
                : new (Value, (int)start, (int)length);

    /// <summary>
    /// Create a readonly sub-span of this array.  This doesn't do any copying, so is very fast, but be aware that any
    /// items outside the splice are still active.   
    /// </summary>
    /// <param name="start">Offset from the beginning of the array</param>
    /// <returns>A read-only span of values</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown If the start index is outside the range of the array</exception>
    [Pure]
    public ReadOnlySpan<A> AsSpan(int start)
    {
        if (start < 0 || start >= length) throw new IndexOutOfRangeException(nameof(start));
        var t = Math.Max(0, length - start);
        return this.start + start > int.MaxValue
                   ? throw new InvalidOperationException("Backing collection is too big to return a view")
                   : t > int.MaxValue
                       ? throw new InvalidOperationException("Backing collection is too big to return a view")
                       : new(Value, (int)(this.start + start), (int)t);
    }

    /// <summary>
    /// Create a readonly sub-span of this array.  This doesn't do any copying, so is very fast, but be aware that any
    /// items outside the splice are still active.   
    /// </summary>
    /// <param name="start">Offset from the beginning of the array</param>
    /// <param name="count">The number of items to take. This will be clamped to the maximum number of items available</param>
    /// <returns>A read-only span of values</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown If the count is less than zero</exception>
    /// <exception cref="IndexOutOfRangeException">Thrown If the start index is outside the range of the array</exception>
    [Pure]
    public ReadOnlySpan<A> AsSpan(int start, int count)
    {
        if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
        if (start < 0 || start >= length) throw new IndexOutOfRangeException(nameof(start));
        var t = Math.Max(0, Math.Min(count, length - start));
        return this.start + start > int.MaxValue
                   ? throw new InvalidOperationException("Backing collection is too big to return a view")
                   : new(Value, (int)(this.start + start), (int)t);
    }
    
    /// <summary>
    /// Create a subarray of this array.  This doesn't do any copying, so is very fast, but be aware that any items
    /// outside the splice are still active.   
    /// </summary>
    /// <param name="start">Offset from the beginning of the array</param>
    /// <returns></returns>
    /// <exception cref="IndexOutOfRangeException">Thrown If the start index is outside the range of the array</exception>
    [Pure]
    public Arr<A> Slice(long start)
    {
        var arr = Value;
        if (start < 0 || start >= length) throw new IndexOutOfRangeException(nameof(start));
        var t = Math.Max(0, length - start);
        return new Arr<A>(arr, this.start + start, t);   
    }
    
    /// <summary>
    /// Create a subarray of this array.  This doesn't do any copying, so is very fast, but be aware that any items
    /// outside the splice are still active.   
    /// </summary>
    /// <param name="start">Offset from the beginning of the array</param>
    /// <param name="count">The number of items to take. This will be clamped to the maximum number of items available</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown If the count is less than zero</exception>
    /// <exception cref="IndexOutOfRangeException">Thrown If the start index is outside the range of the array</exception>
    [Pure]
    public Arr<A> Slice(long start, long count)
    {
        var arr = Value;
        if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
        if (start < 0 || start >= length) throw new IndexOutOfRangeException(nameof(start));
        var t = Math.Max(0, Math.Min(count, length - start));
        return new Arr<A>(arr, this.start + start, t);   
    }
    
    /// <summary>
    /// Operations like `Take` or `Skip` can result in a lot of unused backing buffers, so this method
    /// allows you to make a copy of just the active buffer and create a new instance with it.  The old
    /// reference can then be nulled, allowing the GC to collect it. 
    /// </summary>
    /// <returns>A copy of this instance but with any fat trimmed</returns>
    [Pure]
    public Arr<A> Clone() =>
        new(AsSpan().ToArray());
    
    /// <summary>
    /// Equivalent to `Splice(1, length - 1)`
    /// </summary>
    [Pure]
    public Arr<A> Tail =>
        IsEmpty
            ? this
            : Slice(1, length - 1);
    
    /// <summary>
    /// Equivalent to `Splice(0, length - 1)`
    /// </summary>
    [Pure]
    public Arr<A> Init =>
        IsEmpty
            ? this
            : Slice(0, length - 1);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Arr<A>(A[] xs) =>
        new (xs);

    /// <summary>
    /// Head lens
    /// </summary>
    [Pure]
    public static Lens<Arr<A>, A> head => Lens<Arr<A>, A>.New(
        Get: la => la.Count == 0 ? throw new IndexOutOfRangeException() : la[0],
        Set: a => la => la.Count == 0 ? throw new IndexOutOfRangeException() : la.SetItem(0, a));

    /// <summary>
    /// Head or none lens
    /// </summary>
    [Pure]
    public static Lens<Arr<A>, Option<A>> headOrNone => Lens<Arr<A>, Option<A>>.New(
        Get: la => la.Count == 0 ? None : Some(la[0]),
        Set: a => la => la.Count == 0 || a.IsNone ? la : la.SetItem(0, a.Value!));

    /// <summary>
    /// Last lens
    /// </summary>
    [Pure]
    public static Lens<Arr<A>, A> last => Lens<Arr<A>, A>.New(
        Get: la => la.Count == 0 ? throw new IndexOutOfRangeException() : la[^1],
        Set: a => la => la.Count == 0 ? throw new IndexOutOfRangeException() : la.SetItem(la.Count - 1, a));

    /// <summary>
    /// Last or none lens
    /// </summary>
    [Pure]
    public static Lens<Arr<A>, Option<A>> lastOrNone => Lens<Arr<A>, Option<A>>.New(
        Get: la => la.Count == 0 ? None : Some(la[^1]),
        Set: a => la => la.Count == 0 || a.IsNone ? la : la.SetItem(la.Count - 1, a.Value!));

    /// <summary>
    /// Item at index lens
    /// </summary>
    [Pure]
    public static Lens<Arr<A>, A> item(int index) => Lens<Arr<A>, A>.New(
        Get: la => la.Count == 0 ? throw new IndexOutOfRangeException() : la[index],
        Set: a => la => la.Count == 0 ? throw new IndexOutOfRangeException() : la.SetItem(index, a));

    /// <summary>
    /// Item or none at index lens
    /// </summary>
    [Pure]
    public static Lens<Arr<A>, Option<A>> itemOrNone(int index) => Lens<Arr<A>, Option<A>>.New(
        Get: la => la.Count < index - 1 ? None : Some(la[index]),
        Set: a => la => la.Count < index - 1 || a.IsSome ? la : la.SetItem(index, a.Value!));

    /// <summary>
    /// Lens map
    /// </summary>
    [Pure]
    public static Lens<Arr<A>, Arr<B>> map<B>(Lens<A, B> lens) => Lens<Arr<A>, Arr<B>>.New(
        Get: la => la.Map(lens.Get),
        Set: lb => la => la.Zip(lb).Map(ab => lens.Set(ab.Item2, ab.Item1)).ToArr());

    /// <summary>
    /// Is the stack empty
    /// </summary>
    [Pure]
    public bool IsEmpty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => value == null || length == 0;
    }

    /// <summary>
    /// Number of items in the stack
    /// </summary>
    [Pure]
    public long Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => value == null ? 0L : length;
    }

    /// <summary>
    /// Returns the number of items in the sequence (potentially truncated).
    /// </summary>
    /// <summary>
    /// Prefer the use of `Count` as it supports the full long range.  This is kept here to enable list
    /// pattern-matching to work - which looks for a member called `Count` or `Length` that
    /// is an `int`. Yep, they were that stupid.
    /// </summary>
    public int Length => 
        (int)Count;
    
    /// <summary>
    /// Indexer
    /// </summary>
    /// <summary>
    /// This is kept here to enable list pattern-matching to work - which looks for a `this` member that supports
    /// `Index` and `Index` only supports `int`. Yep, they were that stupid.
    /// </summary>
    /// <exception cref="IndexOutOfRangeException">Thrown when the index is out of the range of the structure</exception>
    public A this[Index index] =>
        index.IsFromEnd
            ? this[Count - index.Value] 
            : this[(long)index.Value];
    
    /// <summary>
    /// Indexer
    /// </summary>
    /// <summary>
    /// This is kept here to enable list pattern-matching to work - which looks for a `this` member that supports
    /// `Index` and `Index` only supports `int`. Yep, they were that stupid.
    /// </summary>
    /// <exception cref="IndexOutOfRangeException">Thrown when the index is out of the range of the structure</exception>
    public A this[LongIndex index] =>
        index.IsFromEnd
            ? this[Count - index.Value] 
            : this[index.Value];

    /// <summary>
    /// Index accessor
    /// </summary>
    [Pure]
    public A this[int index] => 
        Value[start + index];

    /// <summary>
    /// Index accessor
    /// </summary>
    [Pure]
    public A this[long index] => 
        Value[start + index];

    /// <summary>
    /// Add an item to the end of the array
    /// </summary>
    [Pure]
    public Arr<A> Add(A valueToAdd)
    {
        var self = Value;
        return self.Length == 0 
                   ? new Arr<A>([valueToAdd]) 
                   : Insert(self.Length, valueToAdd);
    }

    /// <summary>
    /// Add a range of items to the end of the array
    /// </summary>
    [Pure]
    public Arr<A> AddRange(IEnumerable<A> items) =>
        InsertRange(Count, items);

    /// <summary>
    /// Get enumerator
    /// </summary>
    [Pure]
    IEnumerator IEnumerable.GetEnumerator() =>
        AsEnumerable().GetEnumerator();

    /// <summary>
    /// Get enumerator
    /// </summary>
    [Pure]
    IEnumerator<A> IEnumerable<A>.GetEnumerator() =>
        AsEnumerable().GetEnumerator();
    
    /// <summary>
    /// Get enumerator
    /// </summary>
    [Pure]
    public Enumerator GetEnumerator() =>
        new (this);
    
    /// <summary>
    /// Get enumerator
    /// </summary>
    [Pure]
    public IEnumerable<A> AsEnumerable()
    {
        var iter = new Enumerator(this);
        while (iter.MoveNext())
        {
            yield return iter.Current;
        }
    }

    /// <summary>
    /// Insert value at specified index
    /// </summary>
    [Pure]
    public Arr<A> Insert(long index, A valueToInsert)
    {
        var arr = Value;
        if (index < 0 || index > Count) throw new IndexOutOfRangeException(nameof(index));
        if (length == 0)
        {
            return new Arr<A>([valueToInsert]);
        }

        var xs = new A[length + 1];
        xs[index] = valueToInsert;

        if (index != 0)
        {
            System.Array.Copy(arr, start, xs, 0, index);
        }
        if (index != arr.LongLength)
        {
            System.Array.Copy(arr, start + index, xs, index + 1, length - index);
        }
        return new Arr<A>(xs);
    }

    /// <summary>
    /// Insert range of values at specified index
    /// </summary>
    [Pure]
    public Arr<A> InsertRange(long index, IEnumerable<A> items)
    {
        var arr = Value;
        if (index < 0 || index > Count) throw new IndexOutOfRangeException(nameof(index));

        if (length == 0)
        {
            return new Arr<A>(items);
        }

        var insertArr = items.ToArray();

        var count = insertArr.LongLength;
        if (count == 0)
        {
            return this;
        }

        var newArray = new A[length + count];

        if (index != 0)
        {
            System.Array.Copy(arr, start, newArray, 0, index);
        }
        if (index != arr.LongLength)
        {
            System.Array.Copy(arr, start + index, newArray, index + count, length - index);
        }
        insertArr.CopyTo(newArray, index);

        return new Arr<A>(newArray);
    }

    /// <summary>
    /// Remove an item from the array
    /// </summary>
    [Pure]
    public Arr<A> Remove(A valueToRemove) =>
        Remove<EqDefault<A>>(valueToRemove);

    /// <summary>
    /// Remove an item from the array
    /// </summary>
    [Pure]
    public Arr<A> Remove(A valueToRemove, IEqualityComparer<A> equalityComparer)
    {
        var index = this.IndexOf(valueToRemove, equalityComparer);
        return index.IsNone
                   ? this
                   : RemoveAt((long)index);
    }

    /// <summary>
    /// Remove an item from the array
    /// </summary>
    [Pure]
    public Arr<A> Remove<EQ>(A valueToRemove) where EQ : Eq<A>
    {
        var index = this.IndexOf<EQ, Arr, A>(valueToRemove);
        return index.IsNone
                   ? this
                   : RemoveAt((long)index);
    }

    [Pure]
    Arr<A> RemoveAtRange(params ReadOnlySpan<long> remove)
    {
        var arr = Value;
        if (remove.Length == 0) return this;

        var newArray         = new A[length - remove.Length];
        var copied           = 0L;
        var removed          = 0L;
        var lastIndexRemoved = -1L;
        foreach (var item in remove)
        {
            var copyLength = lastIndexRemoved == -1 ? item : (item - lastIndexRemoved - 1);
            System.Array.Copy(arr, start + copied + removed, newArray, copied, copyLength);
            removed++;
            copied += copyLength;
            lastIndexRemoved = item;
        }
        System.Array.Copy(arr, start + copied + removed, newArray, copied, length - (copied + removed));
        return new Arr<A>(newArray);
    }

    /// <summary>
    /// Remove item at location
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Arr<A> RemoveAt(long index) =>
        RemoveRange(index, 1);

    /// <summary>
    /// Remove a range of items
    /// </summary>
    [Pure]
    public Arr<A> RemoveRange(long index, long count)
    {
        var arr = Value;
        if (index < 0 || index > Count) throw new IndexOutOfRangeException(nameof(index));
        if (!(count >= 0 && index + count <= Count)) throw new IndexOutOfRangeException(nameof(index));
        if (count == 0) return this;

        var newArray = new A[length - count];
        System.Array.Copy(arr, start, newArray, 0, index);
        System.Array.Copy(arr, start + index + count, newArray, index, length - index - count);
        return new Arr<A>(newArray);
    }

    /// <summary>
    /// Set an item at the specified index
    /// </summary>
    [Pure]
    public Arr<A> SetItem(long index, A valueToSet)
    {
        var arr = Value;
        if (index < 0 || index >= arr.Length) throw new IndexOutOfRangeException(nameof(index));

        var newArray = new A[Count];
        System.Array.Copy(arr, start, newArray, 0, length);
        newArray[index] = valueToSet;
        return new Arr<A>(newArray);
    }

    [Pure]
    public Iterable<A> AsIterable() =>
        Iterable.create<Arr, A>(this);

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

    /// <summary>
    /// Reverse the order of the items in the array
    /// </summary>
    [Pure]
    public Arr<A> Reverse()
    {
        var l = Count;
        var v = Value;
        var s = start;
        var m = new A[l];
        var i = 0;
        for (var j = s + l - 1; j >= s; j--, i++)
        {
            m[i] = v[j];
        }
        return new Arr<A>(m);
    }

    /// <summary>
    /// Impure iteration of the bound values in the structure
    /// </summary>
    /// <returns>
    /// Returns the original unmodified structure
    /// </returns>
    public Arr<A> Do(Action<A> f)
    {
        this.Iter(f);
        return this;
    }

    /// <summary>
    /// Map
    /// </summary>
    [Pure]
    public Arr<B> Map<B>(Func<A, B> f)
    {
        var ma     = this;
        var writer = ArrayWriterRef<B>.Init();
        var fs     = IterableK.stepSetup<Arr, Arr.FoldState, A>(ma);

        while (IterableK.step(ma, ref fs, out var x))
        {
            writer.Add(f(x));
        }
        return writer.ToArr();
    }
    
    /// <summary>
    /// Map each element of a structure to an action, evaluate these actions from
    /// left to right, and collect the results.
    /// </summary>
    /// <param name="f"></param>
    /// <param name="ta">Traversable structure</param>
    /// <typeparam name="F">Applicative functor trait</typeparam>
    /// <typeparam name="B">Bound value (output)</typeparam>
    [Pure]
    public K<F, Arr<B>> Traverse<F, B>(Func<A, K<F, B>> f) 
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
    public K<M, Arr<B>> TraverseM<M, B>(Func<A, K<M, B>> f) 
        where M : Monad<M> =>
        M.Map(x => x.As(), Traversable.traverseM(f, this));

    /// <summary>
    /// Filter
    /// </summary>
    [Pure]
    public Arr<A> Filter(Func<A, bool> f)
    {
        var ma     = this;
        var writer = ArrayWriterRef<A>.Init();
        var fs     = IterableK.stepSetup<Arr, Arr.FoldState, A>(ma);

        while (IterableK.step(ma, ref fs, out var x))
        {
            if (f(x))
            {
                writer.Add(x);
            }
        }
        return writer.ToArr();
    }

    [Pure]
    public static Arr<A> operator +(Arr<A> lhs, A rhs) =>
        lhs.Add(rhs);

    [Pure]
    public static Arr<A> operator +(A lhs, Arr<A> rhs) =>
        rhs.Insert(0, lhs);

    [Pure]
    public static Arr<A> operator +(Arr<A> lhs, Arr<A> rhs) =>
        rhs.InsertRange(0, lhs);

    /// <summary>
    /// Choice operator
    /// </summary>
    [Pure]
    public static Arr<A> operator |(Arr<A> x, K<Arr, A> y) =>
        x.Choose(y).As();

    /// <summary>
    /// Choice operator
    /// </summary>
    [Pure]
    public static Arr<A> operator |(K<Arr, A> x, Arr<A> y) =>
        x.Choose(y).As();

    [Pure]
    public Arr<A> Combine(Arr<A> rhs) =>
        rhs.InsertRange(0, this);

    [Pure]
    public override bool Equals(object? obj) =>
        obj is Arr<A> @as && Equals(@as);

    /// <summary>
    /// Get the hash code
    /// Lazily (and once only) calculates the hash from the elements in the array
    /// Empty array hash == 0
    /// </summary>
    [Pure]
    public override int GetHashCode()
    {
        if (hashCode is null)
            return CalcHashCode();
        
        var self = this;
        return hashCode == 0
            ? hashCode.Swap(_ => self.CalcHashCode())
            : hashCode;
    }

    int CalcHashCode() =>
        FNV32.Hash<HashableDefault<A>, A>(Value, start, length);

    [Pure]
    public int CompareTo(object? obj) =>
        obj is Arr<A> t ? CompareTo(t) : 1;

    [Pure]
    public bool Equals(Arr<A> other)
    {
        if (Count != other.Count) return false;

        var ia = GetEnumerator();
        var ib = other.GetEnumerator();
        while (ia.MoveNext() && ib.MoveNext())
        {
            if (!EqDefault<A>.Equals(ia.Current, ib.Current)) return false;
        }
        return true;
    }

    [Pure]
    public int CompareTo(Arr<A> other)
    {
        if (Count < other.Count) return -1;
        if (Count > other.Count) return 1;

        var ia = GetEnumerator();
        var ib = other.GetEnumerator();
        while (ia.MoveNext() && ib.MoveNext())
        {
            var cmp = OrdDefault<A>.Compare(ia.Current, ib.Current);
            if (cmp != 0) return cmp;
        }
        return 0;
    }

    [Pure]
    public static bool operator ==(Arr<A> lhs, Arr<A> rhs) =>
        lhs.Equals(rhs);

    [Pure]
    public static bool operator !=(Arr<A> lhs, Arr<A> rhs) =>
        !(lhs == rhs);

    [Pure]
    public Arr<B> Bind<B>(Func<A, Arr<B>> f)
    {
        var ma     = this;
        var writer = ArrayWriterRef<B>.Init();

        foreach (var a in ma.ForwardIteratorRef<Arr, Arr.FoldState, A>())
        {
            var mb = f(a);
            foreach (var b in mb.ForwardIteratorRef<Arr, Arr.FoldState, B>())
            {
                writer.Add(b);
            }
        }
        return writer.ToArr();
    }

    [Pure]
    public Arr<B> Bind<B>(Func<A, K<Arr, B>> f)
    {
        var ma     = this;
        var writer = ArrayWriterRef<B>.Init();

        foreach (var a in ma.ForwardIteratorRef<Arr, Arr.FoldState, A>())
        {
            var mb = +f(a);
            foreach (var b in mb.ForwardIteratorRef<Arr, Arr.FoldState, B>())
            {
                writer.Add(b);
            }
        }
        return writer.ToArr();
    }

    [Pure]
    public Arr<A> Take(long amount) =>
        amount switch
        {
            0                      => [],
            _ when amount >= Count => this,
            _                      => Slice(0, amount)
        };

    [Pure]
    public Arr<A> Skip(long amount) =>
        amount switch
        {
            0                      => this,
            _ when amount >= Count => [],
            _                      => Slice(amount, Count - amount)
        };

    /// <summary>
    /// Convert to a queryable 
    /// </summary>
    [Pure]
    public IQueryable<A> AsQueryable() =>
        // NOTE TO FUTURE ME: Don't delete this thinking it's not needed!
        // NOTE FROM FUTURE ME: Next time you leave a message for your future self, explain your reasoning.
        AsEnumerable().AsQueryable();    

    /// <summary>
    /// Implicit conversion from an untyped empty list
    /// </summary>
    public static implicit operator Arr<A>(UnitCollection _) =>
        Empty;
    
    public static bool operator >(Arr<A> left, Arr<A> right) =>
        left.CompareTo(right) > 0;
    
    public static bool operator >=(Arr<A> left, Arr<A> right) =>
        left.CompareTo(right) >= 0;
    
    public static bool operator <(Arr<A> left, Arr<A> right) =>
        left.CompareTo(right) < 0;
    
    public static bool operator <=(Arr<A> left, Arr<A> right) =>
        left.CompareTo(right) <= 0;

    public static Arr<A> AdditiveIdentity => 
        Empty;

    static bool TokenStream<Arr<A>, A>.IsTab(A token) =>
        false;

    static bool TokenStream<Arr<A>, A>.IsNewline(A token) => 
        false;

    static ReadOnlySpan<char> TokenStream<Arr<A>, A>.TokenToString(A token) => 
        (token?.ToString() ?? "").AsSpan() ;

    static Arr<A> TokenStream<Arr<A>, A>.TokenToChunk(in A token) => 
        Arr.singleton(token);

    static Arr<A> TokenStream<Arr<A>, A>.TokensToChunk(in ReadOnlySpan<A> token) => 
        [..token];

    static ReadOnlySpan<A> TokenStream<Arr<A>, A>.ChunkToTokens(in Arr<A> tokens) => 
        tokens.As().AsSpan();

    static long TokenStream<Arr<A>, A>.ChunkLength(in Arr<A> tokens) => 
        tokens.As().Count;

    static bool TokenStream<Arr<A>, A>.Take1(in Arr<A> stream, out A head, out Arr<A> tail)
    {
        var s = stream.As();
        if (s.IsEmpty)
        {
            head = default!;
            tail = stream;
            return false;
        }
        else
        {
            head = s[0];
            tail = s.Tail;
            return true;
        }
    }

    static bool TokenStream<Arr<A>, A>.Take(long amount, in Arr<A> stream, out Arr<A> head, out Arr<A> tail)
    {
        // If the requested length `amount` is 0 (or less), `false` should
        // not be returned, instead `true` and `(out Empty, out stream)` should be returned.
        if (amount <= 0)
        {
            head = Empty;
            tail = stream;
            return true;
        }

        // If the requested length is greater than 0 and the stream is
        // empty, `false` should be returned indicating end-of-input.
        if (stream.Count <= 0)
        {
            head = Empty;
            tail = stream;
            return false;
        }
        
        // In other cases, take chunk of length `amount` (or shorter if the
        // stream is not long enough) from the input stream and return the
        // chunk along with the rest of the stream.
        amount = Math.Min(amount, stream.Length);
        var start = stream.start;
        var value = stream.Value;
        head = new Arr<A>(value, start, amount);
        tail = new Arr<A>(value, start + amount, stream.Count - amount);
        return true;
    }

    static void TokenStream<Arr<A>, A>.TakeWhile(Func<A, bool> predicate, in Arr<A> stream, out Arr<A> head, out Arr<A> tail)
    {
        var s       = stream.As();
        var array   = s.Value;
        var start   = s.start;
        var length  = s.length;
        var current = start;
        var offset  = 0;
        while(current < length)
        {
            if (predicate(array[current]))
            {
                current++;
                offset++;
            }
            else
            {
                head = new Arr<A>(array, start, current - start);
                tail = new Arr<A>(array, current, length - offset);
                return;
            }
        }
        head = stream;
        tail = Empty;
    }

    public Iterator<A> GetIterator()
    {
        return Count <= 0
                   ? Iterator.empty<A>()
                   : go(Value, start, Count)();
        
        Func<Iterator<A>> go(A[] array, long index, long remaining) =>
            () => remaining == 0 
                      ? Iterator.empty<A>() 
                      : Iterator.cons(array[index], go(array, index + 1, remaining - 1));
    }
}

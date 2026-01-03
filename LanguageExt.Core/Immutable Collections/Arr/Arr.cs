using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Numerics;
using static LanguageExt.Prelude;
using LanguageExt.Traits;
using LanguageExt.ClassInstances;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace LanguageExt;

/// <summary>
/// Immutable array
/// Native array O(1) read performance.  Modifications require copying of the entire 
/// array to generate the newly mutated version.  This is will be very expensive 
/// for large arrays.
/// </summary>
/// <typeparam name="A">Value type</typeparam>
[Serializable]
[CollectionBuilder(typeof(Arr), nameof(Arr.create))]
public readonly struct Arr<A> :
    IReadOnlyList<A>,
    IEquatable<Arr<A>>,
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
    readonly int start;
    readonly int length;
    readonly Atom<int>? hashCode;

    A[] Value
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
        hashCode = Atom(0);
        value = initial.ToArray();
        start = 0;
        length = value.Length;
    }

    /// <summary>
    /// Ctor
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Arr(A[] value, int start, int length)
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<A> AsSpan() =>
        new (Value, start, length);

    /// <summary>
    /// Create a readonly sub-span of this array.  This doesn't do any copying, so is very fast, but be aware that any
    /// items outside the splice are still active.   
    /// </summary>
    /// <param name="start">Offset from the beginning of the array</param>
    /// <returns>A read-only span of values</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown If the start index is outside the range of the array</exception>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<A> AsSpan(int start)
    {
        if (start < 0 || start >= length) throw new IndexOutOfRangeException(nameof(start));
        var t = Math.Max(0, length - start);
        return new(Value, this.start + start, t);
    }

    /// <summary>
    /// Create a readonly sub-span of this array.  This doesn't do any copying, so is very fast, but be aware that any
    /// items outside the splice are still active.   
    /// </summary>
    /// <param name="start">Offset from the beginning of the array</param>
    /// <param name="count">The number of items to take. This will be clamped to the maximum number of items available</param>
    /// <returns>A read-only span of values</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown If the start index is outside the range of the array</exception>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<A> AsSpan(int start, int count)
    {
        if (start < 0 || start >= length) throw new IndexOutOfRangeException(nameof(start));
        var t = Math.Max(0, Math.Min(count, length - start));
        return new(Value, this.start + start, t);
    }
    
    /// <summary>
    /// Create a subarray of this array.  This doesn't do any copying, so is very fast, but be aware that any items
    /// outside the splice are still active.   
    /// </summary>
    /// <param name="start">Offset from the beginning of the array</param>
    /// <returns></returns>
    /// <exception cref="IndexOutOfRangeException">Thrown If the start index is outside the range of the array</exception>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Arr<A> Splice(int start)
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
    /// <exception cref="IndexOutOfRangeException">Thrown If the start index is outside the range of the array</exception>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Arr<A> Splice(int start, int count)
    {
        var arr = Value;
        if (start < 0 || start >= length) throw new IndexOutOfRangeException(nameof(start));
        var t = Math.Max(0, Math.Min(count, length - start));
        return new Arr<A>(arr, this.start + start, t);   
    }
    
    [Pure]
    public Arr<A> Tail =>
        IsEmpty
            ? this
            : Splice(1, length - 1);

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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Lens<Arr<A>, A> item(int index) => Lens<Arr<A>, A>.New(
        Get: la => la.Count == 0 ? throw new IndexOutOfRangeException() : la[index],
        Set: a => la => la.Count == 0 ? throw new IndexOutOfRangeException() : la.SetItem(index, a));

    /// <summary>
    /// Item or none at index lens
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Lens<Arr<A>, Option<A>> itemOrNone(int index) => Lens<Arr<A>, Option<A>>.New(
        Get: la => la.Count < index - 1 ? None : Some(la[index]),
        Set: a => la => la.Count < index - 1 || a.IsSome ? la : la.SetItem(index, a.Value!));

    /// <summary>
    /// Lens map
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Lens<Arr<A>, Arr<B>> map<B>(Lens<A, B> lens) => Lens<Arr<A>, Arr<B>>.New(
        Get: la => la.Map(lens.Get),
        Set: lb => la => la.Zip(lb).Map(ab => lens.Set(ab.Item2, ab.Item1)).ToArr());

    /// <summary>
    /// Index accessor
    /// </summary>
    [Pure]
    public A this[Index index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => index.IsFromEnd switch
               {
                   false => Value[start          + index.Value],
                   true  => Value[start + length - index.Value - 1]
               };
    }

    /// <summary>
    /// Reference version for use in pattern-matching
    /// </summary>
    /// <remarks>
    ///
    ///     Empty collection     = result is null
    ///     Singleton collection = result is A
    ///     More                 = result is (A, Seq〈A〉) -- head and tail
    ///
    ///  Example:
    ///
    ///     var res = arr.Case switch
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
    public int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => value == null ? 0 : length;
    }

    /// <summary>
    /// Alias of Count
    /// </summary>
    [Pure]
    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => value == null ? 0 : length;
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
        get => Value[start + index];
    }

    /// <summary>
    /// Add an item to the end of the array
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        new (this);

    public struct Enumerator
    {
        readonly A[] arr;
        int index;
        int end;

        internal Enumerator(in Arr<A> arr)
        {
            this.arr = arr.Value;
            index = arr.start - 1;
            end = arr.start   + arr.length;
        }

        public readonly A Current => arr[index];

        public bool MoveNext() => ++index < end;
    }

    /// <summary>
    /// Find the index of an item
    /// </summary>
    [Pure]
    public int IndexOf(A item, int index = 0, int count = -1, IEqualityComparer<A>? equalityComparer = null)
    {
        var eq = equalityComparer ?? EqualityComparer<A>.Default;

        for (; index >= 0 && index < length && count != 0; index++, count--)
        {
            if (eq.Equals(item, this[index])) return index;
        }
        return -1;
    }

    /// <summary>
    /// Find the index of an item
    /// </summary>
    [Pure]
    public int LastIndexOf(A item, int index = -1, int count = -1, IEqualityComparer<A>? equalityComparer = null)
    {
        var eq = equalityComparer ?? EqualityComparer<A>.Default;

        var arr = Value;
        index = index < 0 ? length - 1 : index;

        for (; index >= 0 && index < length && count != 0; index--, count--)
        {
            if (eq.Equals(item, this[index])) return index;
        }
        return -1;
    }

    /// <summary>
    /// Find the index of an item
    /// </summary>
    [Pure]
    public int IndexOf<EQ>(A item, int index = 0, int count = -1) where EQ : Eq<A>
    {
        var arr = Value;
        for (; index < length && count != 0; index++, count--)
        {
            if (EQ.Equals(item, this[index])) return index;
        }
        return -1;
    }

    /// <summary>
    /// Find the index of an item
    /// </summary>
    [Pure]
    public int LastIndexOf<EQ>(A item, int index = -1, int count = -1) where EQ : Eq<A>
    {
        var arr = Value;
        index = index < 0 ? length - 1 : index;

        for (; index >= 0 && index < length && count != 0; index--, count--)
        {
            if (EQ.Equals(item, this[index])) return index;
        }
        return -1;
    }

    /// <summary>
    /// Insert value at specified index
    /// </summary>
    [Pure]
    public Arr<A> Insert(int index, A valueToInsert)
    {
        var arr = Value;
        if (index < 0 || index > Length) throw new IndexOutOfRangeException(nameof(index));
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
        if (index != arr.Length)
        {
            System.Array.Copy(arr, start + index, xs, index + 1, length - index);
        }
        return new Arr<A>(xs);
    }

    /// <summary>
    /// Insert range of values at specified index
    /// </summary>
    [Pure]
    public Arr<A> InsertRange(int index, IEnumerable<A> items)
    {
        var arr = Value;
        if (index < 0 || index > Length) throw new IndexOutOfRangeException(nameof(index));

        if (length == 0)
        {
            return new Arr<A>(items);
        }

        var insertArr = items.ToArray();

        var count = insertArr.Length;
        if (count == 0)
        {
            return this;
        }

        var newArray = new A[length + count];

        if (index != 0)
        {
            System.Array.Copy(arr, start, newArray, 0, index);
        }
        if (index != arr.Length)
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Arr<A> Remove(A valueToRemove) =>
        Remove<EqDefault<A>>(valueToRemove);

    /// <summary>
    /// Remove an item from the array
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Arr<A> Remove(A valueToRemove, IEqualityComparer<A> equalityComparer)
    {
        var index = IndexOf(valueToRemove, 0, -1, equalityComparer);
        return index < 0
                   ? this
                   : RemoveAt(index);
    }

    /// <summary>
    /// Remove an item from the array
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Arr<A> Remove<EQ>(A valueToRemove) where EQ : Eq<A>
    {
        var index = IndexOf<EQ>(valueToRemove);
        return index < 0
                   ? this
                   : RemoveAt(index);
    }

    /// <summary>
    /// Remove all items that match a predicate
    /// </summary>
    [Pure]
    public Arr<A> RemoveAll(Predicate<A> pred)
    {
        if (IsEmpty) return this;

        List<int>? removeIndices = null;
        for (var i = 0; i < Length; i++)
        {
            if (pred(this[i]))
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

        var newArray         = new A[length - remove.Count];
        var copied           = 0;
        var removed          = 0;
        var lastIndexRemoved = -1;
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
    public Arr<A> RemoveAt(int index) =>
        RemoveRange(index, 1);

    /// <summary>
    /// Remove a range of items
    /// </summary>
    [Pure]
    public Arr<A> RemoveRange(int index, int count)
    {
        var arr = Value;
        if (index < 0 || index > Length) throw new IndexOutOfRangeException(nameof(index));
        if (!(count >= 0 && index + count <= Length)) throw new IndexOutOfRangeException(nameof(index));
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
    public Arr<A> SetItem(int index, A valueToSet)
    {
        var arr = Value;
        if (index < 0 || index >= arr.Length) throw new IndexOutOfRangeException(nameof(index));

        var newArray = new A[Length];
        System.Array.Copy(arr, start, newArray, 0, length);
        newArray[index] = valueToSet;
        return new Arr<A>(newArray);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator IEnumerable.GetEnumerator() =>
        Value.GetEnumerator();

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator<A> IEnumerable<A>.GetEnumerator() =>
        // ReSharper disable once NotDisposedResourceIsReturned
        Value.AsEnumerable().GetEnumerator();

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Iterable<A> AsIterable() =>
        Iterable.createRange(this);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Seq<A> ToSeq() =>
        toSeq(this);

    /// <summary>
    /// Format the collection as `[a, b, c, ...]`
    /// The ellipsis is used for collections over 50 items
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
    public Iterable<A> Skip(int amount)
    {
        var s = start;
        var e = start + length;
        return Iterable.createRange(Go(s, e, Value));
        static IEnumerable<A> Go(int s, int e, A[] v)
        {
            for (var i = s; i < e; i++)
            {
                yield return v[i];
            }
        }
    }
        //Value.Skip(start + amount).AsIterable();

    /// <summary>
    /// Reverse the order of the items in the array
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Arr<A> Reverse()
    {
        var l = Count;
        var n = new A[l];
        var v = Value;
        var i = 0;
        var j = l - 1;
        for (; i < l; i++, j--)
        {
            n[i] = v[j + start];
        }
        return new Arr<A>(n);
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
        var arr      = Value;
        var newArray = new B[length];
        for (var i = 0; i < length; i++)
        {
            newArray[i] = map(arr[start + i]);
        }
        return new Arr<B>(newArray);
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

    /// <summary>
    /// Choice operator
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Arr<A> operator |(Arr<A> x, K<Arr, A> y) =>
        x.Choose(y).As();

    /// <summary>
    /// Choice operator
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Arr<A> operator |(K<Arr, A> x, Arr<A> y) =>
        x.Choose(y).As();

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Arr<A> Combine(Arr<A> rhs) =>
        rhs.InsertRange(0, this);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals(object? obj) =>
        obj is Arr<A> @as && Equals(@as);

    /// <summary>
    /// Get the hash code
    /// Lazily (and once only) calculates the hash from the elements in the array
    /// Empty array hash == 0
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo(object? obj) =>
        obj is Arr<A> t ? CompareTo(t) : 1;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        return new Arr<B>(res);
    }

    /// <summary>
    /// Implicit conversion from an untyped empty list
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Arr<A>(SeqEmpty _) =>
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

    static int TokenStream<Arr<A>, A>.ChunkLength(in Arr<A> tokens) => 
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

    static bool TokenStream<Arr<A>, A>.Take(int amount, in Arr<A> stream, out Arr<A> head, out Arr<A> tail)
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
        if (stream.Length <= 0)
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
        tail = new Arr<A>(value, start + amount, stream.Length - amount);
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
}

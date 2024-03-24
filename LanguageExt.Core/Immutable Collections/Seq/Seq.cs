#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using LanguageExt.ClassInstances;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

/// <summary>
/// Cons sequence
/// Represents a sequence of values in a similar way to IEnumerable, but without the
/// issues of multiple evaluation for key LINQ operators like Skip, Count, etc.
/// </summary>
/// <typeparam name="A">Type of the values in the sequence</typeparam>
[CollectionBuilder(typeof(Seq), nameof(Seq.createRange))]
public readonly struct Seq<A> :
    IEnumerable<A>,
    IComparable<Seq<A>>, 
    IEquatable<Seq<A>>, 
    IComparable,
    IComparisonOperators<Seq<A>, Seq<A>, bool>,
    IAdditionOperators<Seq<A>, Seq<A>, Seq<A>>,
    IAdditiveIdentity<Seq<A>, Seq<A>>,
    Monoid<Seq<A>>,
    K<Seq, A>
{
    /// <summary>
    /// Empty sequence
    /// </summary>
    public static Seq<A> Empty { get; } = new(SeqEmptyInternal<A>.Default);

    /// <summary>
    /// Internal representation of the sequence (SeqStrict|SeqLazy|SeqEmptyInternal)
    /// </summary>
    readonly ISeqInternal<A> value;

    /// <summary>
    /// Internal value accessor - protects against `default`
    /// </summary>
    internal ISeqInternal<A> Value
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => value ?? SeqEmptyInternal<A>.Default;
    }

    /// <summary>
    /// Constructor from lazy sequence
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Seq(IEnumerable<A> ma) : this(new SeqLazy<A>(ma)) { }

    /// <summary>
    /// Constructor from lazy sequence
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Seq(ReadOnlySpan<A> ma) : this(Seq.FromArray(ma.ToArray())) { }

    /// <summary>
    /// Constructor
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Seq(ISeqInternal<A> value) =>
        this.value = value;

    /// <summary>
    /// Reference version for use in pattern-matching
    /// </summary>
    [Pure]
    public object? Case =>
        IsEmpty 
            ? null
            : Tail.IsEmpty 
                ? Head.Value
                : (Head.Value, Tail);

    public void Deconstruct(out A head, out Seq<A> tail)
    {
        head = Head.IfNone(() => throw new InvalidOperationException("sequence is empty"));
        tail = Tail;
    }

    /// <summary>
    /// Head lens
    /// </summary>
    public static Lens<Seq<A>, A> head => Lens<Seq<A>, A>.New(
        Get: la => la.IsEmpty ? throw new IndexOutOfRangeException() : la[0],
        Set: a => la => la.IsEmpty ? throw new IndexOutOfRangeException() : a.Cons(la.Tail)
    );

    /// <summary>
    /// Head or none lens
    /// </summary>
    public static Lens<Seq<A>, Option<A>> headOrNone => Lens<Seq<A>, Option<A>>.New(
        Get: la => la.Head,
        Set: a => la => la.IsEmpty || a.IsNone ? la : a.Value.Cons(la.Tail!)!
    );

    /// <summary>
    /// Tail lens
    /// </summary>
    public static Lens<Seq<A>, Seq<A>> tail => Lens<Seq<A>, Seq<A>>.New(
        Get: la => la.IsEmpty ? Empty : la.Tail,
        Set: a => la => la.IsEmpty ? a : ((A)la.Head).Cons(a)
    );

    /// <summary>
    /// Last lens
    /// </summary>
    public static Lens<Seq<A>, A> last => Lens<Seq<A>, A>.New(
        Get: la => la.IsEmpty ? throw new IndexOutOfRangeException() : (A)la.Last,
        Set: a => la => la.IsEmpty ? throw new IndexOutOfRangeException() : la.Take(la.Count - 1).Add(a)
    );

    /// <summary>
    /// Last or none lens
    /// </summary>
    public static Lens<Seq<A>, Option<A>> lastOrNone => Lens<Seq<A>, Option<A>>.New(
        Get: la => la.Last,
        Set: a => la => la.IsEmpty || a.IsNone ? la : la.Take(la.Count - 1).Add(a.Value!));

    /// <summary>
    /// Lens map
    /// </summary>
    [Pure]
    public static Lens<Seq<A>, Seq<B>> map<B>(Lens<A, B> lens) => Lens<Seq<A>, Seq<B>>.New(
        Get: la => la.Map(lens.Get),
        Set: lb => la => la.Zip(lb).Map(ab => lens.Set(ab.Item2, ab.Item1))
    );

    /// <summary>
    /// Indexer
    /// </summary>
    /// <exception cref="IndexOutOfRangeException"></exception>
    public A this[Index index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Value[index.IsFromEnd ? index.GetOffset(Count) : index.Value];
    }

    /// <summary>
    /// Add an item to the end of the sequence
    /// </summary>
    /// <remarks>
    /// Forces evaluation of the entire lazy sequence so the item 
    /// can be appended
    /// </remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Seq<A> Add(A value) =>
        new (Value.Add(value));

    /// <summary>
    /// Add a range of items to the end of the sequence
    /// </summary>
    /// <remarks>
    /// Forces evaluation of the entire lazy sequence so the items
    /// can be appended.  
    /// </remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Seq<A> Concat(IEnumerable<A> items) =>
        items switch
        {
            Lst<A> lst              => Concat(lst),
            Set<A> set              => Concat(set),
            HashSet<A> hset         => Concat(hset),
            Arr<A> arr              => Concat(arr),
            Stck<A> stck            => Concat(stck),
            IReadOnlyList<A> rolist => Concat(rolist),
            _                       => new Seq<A>(EnumerableOptimal.ConcatFast(this, items))
        };
                           
    /// <summary>
    /// Add a range of items to the end of the sequence
    /// </summary>
    /// <remarks>
    /// Forces evaluation of the entire lazy sequence so the items
    /// can be appended.  
    /// </remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Seq<A> Concat(in Lst<A> items)
    {
        if (items.Count == 0)
        {
            return this;
        }
        var arr = items.Value.ToArray();
        return Concat(Seq.FromArray(arr));
    }
                           
    /// <summary>
    /// Add a range of items to the end of the sequence
    /// </summary>
    /// <remarks>
    /// Forces evaluation of the entire lazy sequence so the items
    /// can be appended.  
    /// </remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Seq<A> Concat(in ReadOnlySpan<A> items)
    {
        if (items.Length == 0)
        {
            return this;
        }
        var arr = items.ToArray();
        return Concat(Seq.FromArray(arr));
    }
        
    /// <summary>
    /// Add a range of items to the end of the sequence
    /// </summary>
    /// <remarks>
    /// Forces evaluation of the entire lazy sequence so the items
    /// can be appended.  
    /// </remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Seq<A> Concat(in Set<A> items)
    {
        if (items.Count == 0)
        {
            return this;
        }
        var arr = items.Value.ToArray();
        return Concat(Seq.FromArray(arr));
    }
                
    /// <summary>
    /// Add a range of items to the end of the sequence
    /// </summary>
    /// <remarks>
    /// Forces evaluation of the entire lazy sequence so the items
    /// can be appended.  
    /// </remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Seq<A> Concat(in HashSet<A> items)
    {
        if (items.Count == 0)
        {
            return this;
        }
        var arr = items.ToArray();
        return Concat(Seq.FromArray(arr));
    }
                
    /// <summary>
    /// Add a range of items to the end of the sequence
    /// </summary>
    /// <remarks>
    /// Forces evaluation of the entire lazy sequence so the items
    /// can be appended.  
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Seq<A> Concat(in Arr<A> items)
    {
        if (items.Count == 0)
        {
            return this;
        }
        return Concat(Seq.FromArray(items.Value));
    }
        
    /// <summary>
    /// Add a range of items to the end of the sequence
    /// </summary>
    /// <remarks>
    /// Forces evaluation of the entire lazy sequence so the items
    /// can be appended.  
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Seq<A> Concat(in Stck<A> items)
    {
        if (items.Count == 0)
        {
            return this;
        }
        var arr = items.ToArray();
        return Concat(Seq.FromArray(arr));
    }

    /// <summary>
    /// Add a range of items to the end of the sequence
    /// </summary>
    /// <remarks>
    /// Forces evaluation of the entire lazy sequence so the items
    /// can be appended.  
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Seq<A> Concat(IReadOnlyCollection<A> items)
    {
        if (items.Count == 0)
        {
            return this;
        }

        var arr = items.ToArray();
        return Concat(Seq.FromArray(arr));
    }
        
    /// <summary>
    /// Add a range of items to the end of the sequence
    /// </summary>
    /// <remarks>
    /// Forces evaluation of the entire lazy sequence so the items
    /// can be appended.  
    /// </remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Seq<A> Concat(in Seq<A> rhs)
    {
        switch(Value.Type)
        {
            case SeqType.Empty:
                // lhs is empty, so just return rhs
                return rhs;

            case SeqType.Lazy:

                switch (rhs.Value.Type)
                {
                    // lhs lazy, rhs empty
                    // return lhs
                    case SeqType.Empty:
                        return this;

                    // lhs lazy, rhs lazy
                    // return SeqConcat
                    case SeqType.Lazy:
                        return new Seq<A>(new SeqConcat<A>(Seq(value, rhs.value)));

                    // lhs lazy, rhs strict
                    // force lhs to be strict and concat the two 
                    case SeqType.Strict:
                        return new Seq<A>(((SeqStrict<A>)value.Strict()).Append((SeqStrict<A>)rhs.value));

                    // lhs lazy, rhs concat
                    // prepend rhs with lhs
                    case SeqType.Concat:
                        return new Seq<A>(((SeqConcat<A>)rhs.value).ConsSeq(value));
                }
                break;

            case SeqType.Strict:

                switch (rhs.Value.Type)
                {
                    // lhs strict, rhs empty
                    // return lhs
                    case SeqType.Empty:
                        return this;

                    // lhs strict, rhs lazy
                    // return SeqConcat
                    case SeqType.Lazy:
                        return new Seq<A>(new SeqConcat<A>(Seq(value, rhs.value)));

                    // lhs strict, rhs strict
                    // append the two
                    case SeqType.Strict:
                        return new Seq<A>(((SeqStrict<A>)value).Append((SeqStrict<A>)rhs.value));

                    // lhs strict, rhs concat
                    // prepend rhs with lhs
                    case SeqType.Concat:
                        return new Seq<A>(((SeqConcat<A>)rhs.value).ConsSeq(value));
                }
                break;

            case SeqType.Concat:

                switch (rhs.Value.Type)
                {
                    // lhs concat, rhs empty
                    // return lhs
                    case SeqType.Empty:
                        return this;

                    // lhs concat, rhs lazy || lhs concat, rhs strict
                    // add rhs to concat
                    case SeqType.Lazy:
                    case SeqType.Strict:
                        return new Seq<A>(((SeqConcat<A>)value).AddSeq(rhs.value));

                    // lhs concat, rhs concat
                    // add rhs to concat
                    case SeqType.Concat:
                        return new Seq<A>(((SeqConcat<A>)value).AddSeqRange(((SeqConcat<A>)rhs.value).ms));
                }
                break;
        }
        throw new NotSupportedException();
    }

    /// <summary>
    /// Prepend an item to the sequence
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Seq<A> Cons(A value) =>
        new (Value.Cons(value));

    /// <summary>
    /// Head item in the sequence.  NOTE:  If `IsEmpty` is true then Head 
    /// is undefined.  Call HeadOrNone() if for maximum safety.
    /// </summary>
    public Option<A> Head
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Value.IsEmpty 
                   ? None
                   : Value.Head;
    }

    /// <summary>
    /// Tail of the sequence
    /// </summary>
    public Seq<A> Tail
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new (Value.Tail);
    }

    /// <summary>
    /// Get all items except the last one
    /// </summary>
    public Seq<A> Init
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new (Value.Init);
    }

    /// <summary>
    /// Last item in sequence
    /// </summary>
    public Option<A> Last
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Value.IsEmpty
                   ? None
                   : Some(Value.Last);
    }

    /// <summary>
    /// Returns true if the sequence is empty
    /// </summary>
    /// <remarks>
    /// For lazy streams this will have to peek at the first 
    /// item.  So, the first item will be consumed.
    /// </remarks>
    public bool IsEmpty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => value?.IsEmpty ?? true;
    }

    /// <summary>
    /// Returns the number of items in the sequence
    /// </summary>
    /// <returns>Number of items in the sequence</returns>
    public int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => value?.Count ?? 0;
    }
        
    /// <summary>
    /// Alias of `Count`
    /// </summary>
    /// <returns>Number of items in the sequence</returns>
    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => value?.Count ?? 0;
    }

    /// <summary>
    /// Stream as an enumerable
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public EnumerableM<A> AsEnumerable() => 
        new(Value);

    /// <summary>
    /// Match empty sequence, or multi-item sequence
    /// </summary>
    /// <typeparam name="B">Return value type</typeparam>
    /// <param name="Empty">Match for an empty list</param>
    /// <param name="Tail">Match for a non-empty</param>
    /// <returns>Result of match function invoked</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public B Match<B>(
        Func<B> Empty,
        Func<A, Seq<A>, B> Tail) =>
        IsEmpty
            ? Empty()
            : Tail((A)Head, this.Tail);

    /// <summary>
    /// Match empty sequence, or one item sequence, or multi-item sequence
    /// </summary>
    /// <typeparam name="B">Return value type</typeparam>
    /// <param name="Empty">Match for an empty list</param>
    /// <param name="Tail">Match for a non-empty</param>
    /// <returns>Result of match function invoked</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public B Match<B>(
        Func<B> Empty,
        Func<A, B> Head,
        Func<A, Seq<A>, B> Tail) =>
        IsEmpty
            ? Empty()
            : this.Tail.IsEmpty
                ? Head((A)this.Head)
                : Tail((A)this.Head, this.Tail);

    /// <summary>
    /// Match empty sequence, or multi-item sequence
    /// </summary>
    /// <typeparam name="B">Return value type</typeparam>
    /// <param name="Empty">Match for an empty list</param>
    /// <param name="Sequence">Match for a non-empty</param>
    /// <returns>Result of match function invoked</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public B Match<B>(
        Func<B> Empty,
        Func<Seq<A>, B> Seq) =>
        IsEmpty
            ? Empty()
            : Seq(this);

    /// <summary>
    /// Match empty sequence, or one item sequence, or multi-item sequence
    /// </summary>
    /// <typeparam name="B">Return value type</typeparam>
    /// <param name="Empty">Match for an empty list</param>
    /// <param name="Tail">Match for a non-empty</param>
    /// <returns>Result of match function invoked</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public B Match<B>(
        Func<B> Empty,
        Func<A, B> Head,
        Func<Seq<A>, B> Tail) =>
        IsEmpty
            ? Empty()
            : this.Tail.IsEmpty
                ? Head((A)this.Head)
                : Tail(this.Tail);

    /// <summary>
    /// Impure iteration of the bound values in the structure
    /// </summary>
    /// <returns>
    /// Returns the original unmodified structure
    /// </returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Seq<A> Do(Action<A> f)
    {
        this.Iter(f);
        return this;
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
    public K<F, Seq<B>> Traverse<F, B>(Func<A, K<F, B>> f) 
        where F : Applicative<F> =>
        F.Map(x => x.As(), Traversable.traverse(f, this));
    
    /// <summary>
    /// Map the sequence using the function provided
    /// </summary>
    /// <typeparam name="B"></typeparam>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped sequence</returns>
    [Pure]
    public Seq<B> Map<B>(Func<A, B> f)
    {
        return new Seq<B>(new SeqLazy<B>(Yield(this)));
        IEnumerable<B> Yield(Seq<A> items)
        {
            foreach (var item in items)
            {
                yield return f(item);
            }
        }
    }
        
    /// <summary>
    /// Map the sequence using the function provided
    /// </summary>
    /// <typeparam name="B"></typeparam>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped sequence</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Seq<B> Select<B>(Func<A, B> f) =>
        Map(f);

    /// <summary>
    /// Monadic bind (flatmap) of the sequence
    /// </summary>
    /// <typeparam name="B">Bound return value type</typeparam>
    /// <param name="f">Bind function</param>
    /// <returns>Flat-mapped sequence</returns>
    [Pure]
    public Seq<B> Bind<B>(Func<A, Seq<B>> f)
    {
        static IEnumerable<B> Yield(Seq<A> ma, Func<A, Seq<B>> bnd)
        {
            foreach (var a in ma)
            {
                foreach (var b in bnd(a))
                {
                    yield return b;
                }
            }
        }
        return new Seq<B>(Yield(this, f));
    }

    /// <summary>
    /// Monadic bind (flatmap) of the sequence
    /// </summary>
    /// <typeparam name="B">Bound return value type</typeparam>
    /// <param name="f">Bind function</param>
    /// <returns>Flat-mapped sequence</returns>
    [Pure]
    public Seq<B> Bind<B>(Func<A, K<Seq, B>> f)
    {
        static IEnumerable<B> Yield(K<Seq, A> ma, Func<A, K<Seq, B>> bnd)
        {
            foreach (var a in ma.As())
            {
                foreach (var b in bnd(a).As())
                {
                    yield return b;
                }
            }
        }
        return new Seq<B>(Yield(this, f));
    }

    /// <summary>
    /// Monadic bind (flatmap) of the sequence
    /// </summary>
    /// <typeparam name="B">Bound return value type</typeparam>
    /// <param name="bind">Bind function</param>
    /// <returns>Flat-mapped sequence</returns>
    [Pure]
    public Seq<C> SelectMany<B, C>(Func<A, Seq<B>> bind, Func<A, B, C> project)
    {
        static IEnumerable<C> Yield(Seq<A> ma, Func<A, Seq<B>> bnd, Func<A, B, C> prj)
        {
            foreach (var a in ma)
            {
                foreach (var b in bnd(a))
                {
                    yield return prj(a, b);
                }
            }
        }
        return new Seq<C>(Yield(this, bind, project));
    }

    /// <summary>
    /// Filter the items in the sequence
    /// </summary>
    /// <param name="f">Predicate to apply to the items</param>
    /// <returns>Filtered sequence</returns>
    [Pure]
    public Seq<A> Filter(Func<A, bool> f)
    {
        return new Seq<A>(new SeqLazy<A>(Yield(this, f)));
        static IEnumerable<A> Yield(Seq<A> items, Func<A, bool> f)
        {
            foreach (var item in items)
            {
                if (f(item))
                {
                    yield return item;
                }
            }
        }
    }
        
    /// <summary>
    /// Filter the items in the sequence
    /// </summary>
    /// <param name="f">Predicate to apply to the items</param>
    /// <returns>Filtered sequence</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Seq<A> Where(Func<A, bool> f) =>
        Filter(f);

    /// <summary>
    /// Returns true if the supplied predicate returns true for any
    /// item in the sequence.  False otherwise.
    /// </summary>
    /// <param name="f">Predicate to apply</param>
    /// <returns>True if the supplied predicate returns true for any
    /// item in the sequence.  False otherwise.</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Exists(Func<A, bool> f) =>
        Value.Exists(f);

    /// <summary>
    /// Returns true if the supplied predicate returns true for all
    /// items in the sequence.  False otherwise.  If there is an 
    /// empty sequence then true is returned.
    /// </summary>
    /// <param name="f">Predicate to apply</param>
    /// <returns>True if the supplied predicate returns true for all
    /// items in the sequence.  False otherwise.  If there is an 
    /// empty sequence then true is returned.</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ForAll(Func<A, bool> f) =>
        Value.ForAll(f);

    /// <summary>
    /// Returns true if the sequence has items in it
    /// </summary>
    /// <returns>True if the sequence has items in it</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Any() =>
        !IsEmpty;

    /// <summary>
    /// Inject a value in between each item in the sequence 
    /// </summary>
    /// <param name="ma">Sequence to inject values into</param>
    /// <param name="value">Item to inject</param>
    /// <typeparam name="A">Bound type</typeparam>
    /// <returns>A sequence with the values injected</returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Seq<A> Intersperse(A value) =>
        toSeq(Value.Intersperse(value));

    /// <summary>
    /// Get the hash code for all of the items in the sequence, or 0 if empty
    /// </summary>
    /// <returns></returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() =>
        Value.GetHashCode();

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo(object? obj) =>
        obj switch
        {
            Seq<A> s         => CompareTo(s),
            IEnumerable<A> e => CompareTo(toSeq(e)),
            _                => 1
        };

    /// <summary>
    /// Format the collection as `[a, b, c, ...]`
    /// The ellipsis is used for collections over 50 items
    /// To get a formatted string with all the items, use `ToFullString`
    /// or `ToFullArrayString`.
    /// </summary>
    [Pure]
    public override string ToString() =>
        Value is SeqLazy<A>
            ? CollectionFormat.ToShortArrayString(this)
            : CollectionFormat.ToShortArrayString(this, Count);

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
    public Seq<A> Combine(Seq<A> y) =>
        this + y;

    /// <summary>
    /// Append operator
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Seq<A> operator +(Seq<A> x, Seq<A> y) =>
        x.Concat(y);

    /// <summary>
    /// Ordering operator
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(Seq<A> x, Seq<A> y) =>
        x.CompareTo(y) > 0;

    /// <summary>
    /// Ordering operator
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(Seq<A> x, Seq<A> y) =>
        x.CompareTo(y) >= 0;

    /// <summary>
    /// Ordering  operator
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(Seq<A> x, Seq<A> y) =>
        x.CompareTo(y) < 0;

    /// <summary>
    /// Ordering  operator
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(Seq<A> x, Seq<A> y) =>
        x.CompareTo(y) <= 0;

    /// <summary>
    /// Equality operator
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Seq<A> x, Seq<A> y) =>
        x.Equals(y);

    /// <summary>
    /// Non-equality operator
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Seq<A> x, Seq<A> y) =>
        !(x == y);

    /// <summary>
    /// Equality test
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals(object? obj) =>
        obj switch
        {
            Seq<A> s         => Equals(s),
            IEnumerable<A> e => Equals(toSeq(e)),
            _                => false
        };

    /// <summary>
    /// Equality test
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Seq<A> rhs) =>
        Equals<EqDefault<A>>(rhs);

    /// <summary>
    /// Equality test
    /// </summary>
    [Pure]
    public bool Equals<EqA>(Seq<A> rhs) where EqA : Eq<A>
    {
        // Differing lengths?
        if(Count != rhs.Count) return false;

        // If the hash code has been calculated on both sides then 
        // check for differences
        if (GetHashCode() != rhs.GetHashCode())
        {
            return false;
        }

        // Iterate through both sides
        using var iterA = GetEnumerator();
        using var iterB = rhs.GetEnumerator();
        while (iterA.MoveNext() && iterB.MoveNext())
        {
            if (!EqA.Equals(iterA.Current, iterB.Current))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Skip count items
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Seq<A> Skip(int amount) =>
        amount < 1
            ? this
            : new Seq<A>(Value.Skip(amount));

    /// <summary>
    /// Take count items
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Seq<A> Take(int amount) =>
        amount < 1
            ? Empty
            : new Seq<A>(Value.Take(amount));

    /// <summary>
    /// Iterate the sequence, yielding items if they match the predicate 
    /// provided, and stopping as soon as one doesn't
    /// </summary>
    /// <returns>A new sequence with the first items that match the 
    /// predicate</returns>
    [Pure]
    public Seq<A> TakeWhile(Func<A, bool> pred)
    {
        return new Seq<A>(new SeqLazy<A>(Yield(Value, pred)));
        IEnumerable<A> Yield(IEnumerable<A> xs, Func<A, bool> f)
        {
            foreach (var x in xs)
            {
                if (!f(x)) break;
                yield return x;
            }
        }
    }

    /// <summary>
    /// Iterate the sequence, yielding items if they match the predicate 
    /// provided, and stopping as soon as one doesn't.  An index value is 
    /// also provided to the predicate function.
    /// </summary>
    /// <returns>A new sequence with the first items that match the 
    /// predicate</returns>
    [Pure]
    public Seq<A> TakeWhile(Func<A, int, bool> pred)
    {
        return new Seq<A>(new SeqLazy<A>(Yield(Value, pred)));
        IEnumerable<A> Yield(IEnumerable<A> xs, Func<A, int, bool> f)
        {
            var i = 0;
            foreach (var x in xs)
            {
                if (!f(x, i)) break;
                yield return x;
                i++;
            }
        }
    }

    /// <summary>
    /// Returns all initial segments of the sequence, shortest first
    /// </summary>
    /// <remarks>
    /// Including the empty sequence
    /// </remarks>
    /// <example>
    ///
    ///      Seq("a", "b", "c").Inits
    ///
    ///      > Seq(Seq(), Seq("a"), Seq("a", "b"), Seq("a", "b", "c"))  
    ///     
    /// </example>
    /// <returns>Initial segments of the sequence</returns>
    public Seq<Seq<A>> Inits =>
        [Seq<A>()] + NonEmptyInits;

    /// <summary>
    /// Returns all initial segments of the sequence, shortest first.
    /// </summary>
    /// <remarks>
    /// Not including the empty sequence
    /// </remarks>
    /// <example>
    ///
    ///      Seq("a", "b", "c").Inits
    ///
    ///      > Seq(Seq("a"), Seq("a", "b"), Seq("a", "b", "c"))  
    ///     
    /// </example>
    /// <returns>Initial segments of the sequence</returns>
    public Seq<Seq<A>> NonEmptyInits
    {
        get
        {
            var mma = Seq<Seq<A>>();
            for (var i = 1; i <= Count; i++)
            {
                mma = mma.Add(Take(i));
            }
            return mma;
        }
    }
        
    /// <summary>
    /// Returns all final segments of the argument, longest first.
    /// </summary>
    /// <remarks>
    /// Including the empty sequence
    /// </remarks>
    /// <example>
    ///
    ///      Seq("a", "b", "c").Tails
    ///
    ///      > Seq(Seq("a", "b", "c"), Seq("a", "b"), Seq("a"), Seq())  
    ///     
    /// </example>
    /// <returns>Initial segments of the sequence</returns>
    public Seq<Seq<A>> Tails
    {
        get
        {
            return new Seq<Seq<A>>(go(this)); 
            static IEnumerable<Seq<A>> go(Seq<A> ma)
            {
                while (!ma.IsEmpty)
                {
                    yield return ma;
                    ma = ma.Tail;
                }
                yield return Empty;
            }
        }
    }
        
    /// <summary>
    /// Returns all final segments of the argument, longest first.
    /// </summary>
    /// <remarks>
    /// Not including the empty sequence
    /// </remarks>
    /// <example>
    ///
    ///      Seq("a", "b", "c").Tails
    ///
    ///      > Seq(Seq("a", "b", "c"), Seq("a", "b"), Seq("a"))  
    ///     
    /// </example>
    /// <returns>Initial segments of the sequence</returns>
    public Seq<Seq<A>> NonEmptyTails
    {
        get
        {
            return new Seq<Seq<A>>(go(this)); 
            static IEnumerable<Seq<A>> go(Seq<A> ma)
            {
                while (!ma.IsEmpty)
                {
                    yield return ma;
                    ma = ma.Tail;
                }
            }
        }
    }
        
    /// <summary>
    /// Partition a list into two based on  a predicate
    /// </summary>
    /// <param name="predicate">True if the item goes in the first list, false for the second list</param>
    /// <returns>Pair of lists</returns>
    public (Seq<A> First, Seq<A> Second) Partition(Func<A, bool> predicate)
    {
        var f = Seq<A>();
        var s = Seq<A>();
        foreach (var item in this)
        {
            if (predicate(item))
            {
                f = f.Add(item);
            }
            else
            {
                s = s.Add(item);
            }
        }
        return (f, s);
    }

    /// <summary>
    /// Compare to another sequence
    /// </summary>
    [Pure]
    public int CompareTo(Seq<A> rhs) =>
        CompareTo<OrdDefault<A>>(rhs);

    /// <summary>
    /// Compare to another sequence
    /// </summary>
    [Pure]
    public int CompareTo<OrdA>(Seq<A> rhs) where OrdA : Ord<A>
    {
        // Differing lengths?
        var cmp = Count.CompareTo(rhs.Count);
        if (cmp != 0) return cmp;

        // Iterate through both sides
        using var iterA = GetEnumerator();
        using var iterB = rhs.GetEnumerator();
        while (iterA.MoveNext() && iterB.MoveNext())
        {
            cmp = OrdA.Compare(iterA.Current, iterB.Current);
            if (cmp != 0) return cmp;
        }

        return 0;
    }

    /// <summary>
    /// Force all items lazy to stream
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Seq<A> Strict() => 
        new (Value.Strict());

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerator<A> GetEnumerator() =>
        Value.GetEnumerator();

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator IEnumerable.GetEnumerator() =>
        Value.GetEnumerator();

    /// <summary>
    /// Implicit conversion from an untyped empty list
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Seq<A>(SeqEmpty _) =>
        Empty;

    [Pure]
    public Seq<B> Cast<B>()
    {
        IEnumerable<B> Yield(Seq<A> ma)
        {
            foreach (object? item in ma)
            {
                if( item is B b) yield return b;
            }
        }

        return Value is IEnumerable<B> mb
                   ? new Seq<B>(mb)
                   : new Seq<B>(Yield(this));
    }

    public static Seq<A> AdditiveIdentity => 
        Empty;
}

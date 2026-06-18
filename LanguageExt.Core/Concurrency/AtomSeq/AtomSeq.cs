using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using LanguageExt.ClassInstances;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

/// <summary>
/// Atoms provide a way to manage shared, synchronous, independent state without 
/// locks.  `AtomSeq` wraps the language-ext `Seq`, and makes sure all operations are atomic and thread-safe
/// without resorting to locking
/// </summary>
/// <remarks>
/// See the [concurrency section](https://github.com/louthy/language-ext/wiki/Concurrency) of the wiki for more info.
/// </remarks>
/// <typeparam name="A">Item value type</typeparam>
public class AtomSeq<A> : 
    IComparable<AtomSeq<A>>, 
    IEquatable<AtomSeq<A>>, 
    IComparable<Seq<A>>, 
    IEquatable<Seq<A>>,
    IEnumerable<A>,
    IComparable,
    K<AtomSeq, A>
{
    /// <summary>
    /// Empty sequence
    /// </summary>
    public static AtomSeq<A> Empty => new (SeqEmptyInternal<A>.Default);

    /// <summary>
    /// Internal representation of the sequence (SeqStrict|SeqLazy|SeqEmptyInternal)
    /// </summary>
    volatile ISeqInternal<A> items;

    /// <summary>
    /// Constructor
    /// </summary>
    public AtomSeq(IEnumerable<A> ma) : 
        this(ma.AsIteratorStrict()) { }

    /// <summary>
    /// Constructor
    /// </summary>
    public AtomSeq(Seq<A> ma) : 
        this(ma.Value) { }

    /// <summary>
    /// Constructor
    /// </summary>
    public AtomSeq(Iterable<A> ma) : 
        this(new SeqIterator<A>(ma.ForwardIterator())) { }

    /// <summary>
    /// Constructor
    /// </summary>
    public AtomSeq(Iterator<A> ma) : 
        this(new SeqIterator<A>(ma)) { }

    /// <summary>
    /// Constructor
    /// </summary>
    public AtomSeq(ReadOnlySpan<A> ma) : 
        this(Seq.FromArray(ma.ToArray())) { }
    
    /// <summary>
    /// Constructor
    /// </summary>
    internal AtomSeq(ISeqInternal<A> items) =>
        this.items = items;
    
    /// <summary>
    /// Take an immutable snapshot of the current state of the collection.  This can be called multiple times
    /// to get snapshots of the state of the collection over time.
    /// </summary>
    /// <remarks>This is effectively a zero-cost operation because the backing value is of this type</remarks>
    [Pure]
    public Seq<A> Snapshot() =>
        new (items);

    /// <summary>
    /// Reference version for use in pattern-matching
    /// </summary>
    [Pure]
    public object? Case
    {
        get
        {
            var xs = items;
            return xs.IsEmpty
                       ? null
                       : xs.Tail.IsEmpty
                           ? xs.Head
                           : (xs.Head, xs.Tail);
        }
    }

    public void Deconstruct(out A head, out Seq<A> tail)
    {
        var xs = items;
        head = xs.Head;
        tail = new Seq<A>(xs.Tail);
    }
        
    /// <summary>
    /// Indexer
    /// </summary>
    /// <exception cref="IndexOutOfRangeException">Thrown when the index is out of the range of the structure</exception>
    public A this[long index] => 
        items[index];

    /// <summary>
    /// Indexer
    /// </summary>
    public A this[int index] => 
        items[index];
    
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
    /// Atomically swap the underlying Seq.  Allows for multiple operations on the Seq in an entirely
    /// transactional and atomic way.
    /// </summary>
    /// <param name="swap">Swap function, maps the current state of the AtomSeq to a new state</param>
    /// <remarks>Any functions passed as arguments may be run multiple times if there are multiple threads competing
    /// to update this data structure.  Therefore the functions must spend as little time performing the injected
    /// behaviours as possible to avoid repeated attempts</remarks>
    public IO<Unit> SwapIO(Func<Seq<A>, Seq<A>> swap) =>
        IO.lift(_ => Swap(swap));
        
    /// <summary>
    /// Atomically swap the underlying Seq.  Allows for multiple operations on the Seq in an entirely
    /// transactional and atomic way.
    /// </summary>
    /// <param name="swap">Swap function, maps the current state of the AtomSeq to a new state</param>
    /// <remarks>Any functions passed as arguments may be run multiple times if there are multiple threads competing
    /// to update this data structure.  Therefore the functions must spend as little time performing the injected
    /// behaviours as possible to avoid repeated attempts</remarks>
    public Unit Swap(Func<Seq<A>, Seq<A>> swap)
    {
        SpinWait sw = default;
        while (true)
        {
            var oitems = items;
            var nitems = swap(new Seq<A>(oitems)).Value;
            if(ReferenceEquals(oitems, nitems))
            {
                // no change
                return default;
            }
            if (ReferenceEquals(Interlocked.CompareExchange(ref items, nitems, oitems), oitems))
            {
                return default;
            }
            else
            {
                sw.SpinOnce();
            }
        }
    }
        
    /// <summary>
    /// Atomically swap the underlying Seq.  Allows for multiple operations on the Seq in an entirely
    /// transactional and atomic way.
    /// </summary>
    /// <param name="swap">Swap function, maps the current state of the AtomSeq to a new state</param>
    /// <remarks>Any functions passed as arguments may be run multiple times if there are multiple threads competing
    /// to update this data structure.  Therefore the functions must spend as little time performing the injected
    /// behaviours as possible to avoid repeated attempts</remarks>
    internal Unit SwapInternal(Func<ISeqInternal<A>, ISeqInternal<A>> swap)
    {
        SpinWait sw = default;
        while (true)
        {
            var oitems = items;
            var nitems = swap(oitems);
            if(ReferenceEquals(oitems, nitems))
            {
                // no change
                return default;
            }
            if (ReferenceEquals(Interlocked.CompareExchange(ref items, nitems, oitems), oitems))
            {
                return default;
            }
            else
            {
                sw.SpinOnce();
            }
        }
    }
        
    /// <summary>
    /// Add an item to the end of the sequence
    /// </summary>
    /// <remarks>
    /// Forces evaluation of the entire lazy sequence so the item 
    /// can be appended
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Unit Add(A value)
    {
        SpinWait sw = default;
        while (true)
        {
            var oitems = items;
            var nitems = oitems.Add(value);
            if (ReferenceEquals(Interlocked.CompareExchange(ref items, nitems, oitems), oitems))
            {
                return default;
            }
            else
            {
                sw.SpinOnce();
            }
        }
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
    public Unit Concat(IEnumerable<A> items) => items switch
                                                {
                                                    Lst<A> lst              => Concat(lst),
                                                    Set<A> set              => Concat(set),
                                                    HashSet<A> hset         => Concat(hset),
                                                    Arr<A> arr              => Concat(arr),
                                                    Stck<A> stck            => Concat(stck),
                                                    IReadOnlyList<A> rolist => Concat(rolist),
                                                    _                       => Concat(toSeq(items))
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
    public Unit Concat(Lst<A> items)
    {
        if (items.Count == 0)
        {
            return default;
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
    public Unit Concat(Set<A> items)
    {
        if (items.Count == 0)
        {
            return default;
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
    public Unit Concat(HashSet<A> items)
    {
        if (items.Count == 0)
        {
            return default;
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
    public Unit Concat(Stck<A> items)
    {
        if (items.Count == 0)
        {
            return default;
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
    public Unit Concat(IReadOnlyCollection<A> items)
    {
        if (items.Count == 0)
        {
            return default;
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
    public Unit Concat(Seq<A> rhs) =>
        Swap(lhs => lhs + rhs);

    /// <summary>
    /// Prepend an item to the sequence
    /// </summary>
    internal Unit Cons(A value)
    {
        SpinWait sw = default;
        while (true)
        {
            var oitems = items;
            var nitems = oitems.Cons(value);
            if (ReferenceEquals(Interlocked.CompareExchange(ref items, nitems, oitems), oitems))
            {
                return default;
            }
            else
            {
                sw.SpinOnce();
            }
        }
    }

    /// <summary>
    /// Head item in the sequence.  NOTE:  If `IsEmpty` is true then Head 
    /// is undefined.  Call HeadOrNone() if for maximum safety.
    /// </summary>
    public A Head
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => items.Head;
    }

    /// <summary>
    /// Tail of the sequence
    /// </summary>
    public Seq<A> Tail
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new Seq<A>(items.Tail);
    }

    /// <summary>
    /// Get all items except the last one
    /// </summary>
    public Seq<A> Init
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new Seq<A>(items.Init);
    }

    /// <summary>
    /// Head of the sequence if this node isn't the empty node
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Option<A> HeadOrNone()
    {
        var xs = items;
        return xs.IsEmpty
                   ? None
                   : Some(xs.Head);
    }

    /// <summary>
    /// Last item in sequence.  Throws if no items in sequence
    /// </summary>
    public A Last
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => items.Last;
    }

    /// <summary>
    /// Last item in sequence.
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Option<A> LastOrNone()
    {
        var xs = items;
        return xs.IsEmpty
                   ? None
                   : Some(xs.Last);
    }

    /// <summary>
    /// Last item in sequence.
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Either<L, A> LastOrLeft<L>(L Left)
    {
        var xs = items;
        return xs.IsEmpty
                   ? Either.Left<L, A>(Left)
                   : Either.Right<L, A>(xs.Last);
    }

    /// <summary>
    /// Last item in sequence.
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Either<L, A> LastOrLeft<L>(Func<L> Left)
    {
        var xs = items;
        return xs.IsEmpty
                   ? Either.Left<L, A>(Left())
                   : Either.Right<L, A>(xs.Last);
    }

    /// <summary>
    /// Head of the sequence if this node isn't the empty node or left
    /// </summary>
    /// <typeparam name="L"></typeparam>
    /// <param name="left">Left case</param>
    /// <returns>Head of the sequence or left</returns>
    [Pure]
    public Either<L, A> HeadOrLeft<L>(L left)
    {
        var xs = items;
        return xs.IsEmpty
                   ? Left<L, A>(left)
                   : Right<L, A>(xs.Head);
    }

    /// <summary>
    /// Head of the sequence if this node isn't the empty node
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Either<L, A> HeadOrLeft<L>(Func<L> Left)
    {
        var xs = items;
        return xs.IsEmpty
                   ? Left<L, A>(Left())
                   : Right<L, A>(xs.Head);
    }

    /// <summary>
    /// Returns true if the sequence is empty
    /// </summary>
    /// <remarks>
    /// For lazy streams this will have to peek at the first 
    /// item.  So, the first item will be consumed.
    /// </remarks>
    public bool IsEmpty => 
        items.IsEmpty;

    /// <summary>
    /// Returns the number of items in the sequence
    /// </summary>
    /// <returns>Number of items in the sequence</returns>
    public long Count => 
        items.Count;

    /// <summary>
    /// Returns the number of items in the sequence (potentially truncated).
    /// </summary>
    /// <summary>
    /// Prefer to use `Count` as it supports the full long range.  This is kept here to enable list
    /// pattern-matching to work - which looks for a member called `Count` or `Length` that
    /// is an `int`. Yep, they were that stupid.
    /// </summary>
    public int Length => 
        (int)Count;

    /// <summary>
    /// Stream as an enumerable
    /// </summary>
    [Pure]
    public Iterable<A> AsIterable() => 
        items.GetIterator().AsIterable();

    /// <summary>
    /// Match empty sequence, or multi-item sequence
    /// </summary>
    /// <typeparam name="B">Return value type</typeparam>
    /// <param name="Empty">Match for an empty list</param>
    /// <param name="Tail">Match for a non-empty</param>
    /// <returns>Result of match function invoked</returns>
    [Pure]
    public B Match<B>(Func<B> Empty, Func<A, Seq<A>, B> Tail)
    {
        var xs = items;
        return xs.IsEmpty
                   ? Empty()
                   : Tail(xs.Head, new Seq<A>(xs.Tail));
    }

    /// <summary>
    /// Match empty sequence, or one item sequence, or multi-item sequence
    /// </summary>
    /// <typeparam name="B">Return value type</typeparam>
    /// <param name="Empty">Match for an empty list</param>
    /// <param name="Tail">Match for a non-empty</param>
    /// <returns>Result of match function invoked</returns>
    [Pure]
    public B Match<B>(
        Func<B> Empty,
        Func<A, B> Head,
        Func<A, Seq<A>, B> Tail)
    {
        var xs = items;
        return xs.IsEmpty
                   ? Empty()
                   : xs.Tail.IsEmpty
                       ? Head(xs.Head)
                       : Tail(xs.Head, new Seq<A>(xs.Tail));
    }

    /// <summary>
    /// Match empty sequence, or multi-item sequence
    /// </summary>
    /// <typeparam name="B">Return value type</typeparam>
    /// <param name="Empty">Match for an empty list</param>
    /// <param name="Sequence">Match for a non-empty</param>
    /// <returns>Result of match function invoked</returns>
    [Pure]
    public B Match<B>(
        Func<B> Empty,
        Func<Seq<A>, B> Seq)
    {
        var xs = items;
        return xs.IsEmpty
                   ? Empty()
                   : Seq(new Seq<A>(xs));
    }

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
        Func<Seq<A>, B> Tail)
    {
        var xs = items;
        return xs.IsEmpty
                   ? Empty()
                   : xs.Tail.IsEmpty
                       ? Head(xs.Head)
                       : Tail(new Seq<A>(xs.Tail));
    }

    /// <summary>
    /// Map the sequence using the function provided
    /// </summary>
    /// <typeparam name="B"></typeparam>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped sequence</returns>
    [Pure]
    public Unit Map(Func<A, A> f) =>
        Swap(xs => xs.Map(f));
    
    /// <summary>
    /// Map the sequence using the function provided
    /// </summary>
    /// <typeparam name="B"></typeparam>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped sequence</returns>
    [Pure]
    public Unit Select(Func<A, A> f) =>
        Map(f);

    /// <summary>
    /// Monadic bind (flatmap) of the sequence
    /// </summary>
    /// <typeparam name="B">Bound return value type</typeparam>
    /// <param name="f">Bind function</param>
    /// <returns>Flat-mapped sequence</returns>
    public Unit Bind(Func<A, Seq<A>> f) =>
        Swap(xs => xs.Bind(f));

    /// <summary>
    /// Monadic bind (flatmap) of the sequence
    /// </summary>
    /// <typeparam name="B">Bound return value type</typeparam>
    /// <param name="bind">Bind function</param>
    [Pure]
    public Unit SelectMany<B>(Func<A, Seq<B>> bind, Func<A, B, A> project) =>
        Swap(xs => xs.SelectMany(bind, project));
        
    /// <summary>
    /// Filter the items in the sequence
    /// </summary>
    /// <param name="f">Predicate to apply to the items</param>
    /// <returns>Filtered sequence</returns>
    public Unit Filter(Func<A, bool> f) =>
        Swap(xs => xs.Filter(f));

    /// <summary>
    /// Returns true if the sequence has items in it
    /// </summary>
    /// <returns>True if the sequence has items in it</returns>
    [Pure]
    public bool Any() =>
        !IsEmpty;

    /// <summary>
    /// Inject a value in between each item in the sequence 
    /// </summary>
    /// <param name="ma">Sequence to inject values into</param>
    /// <param name="sep">Separator value to inject</param>
    /// <typeparam name="A">Bound type</typeparam>
    /// <returns>A sequence with the values injected</returns>
    [Pure]
    public Unit Intersperse(A sep) =>
        Swap(xs => xs.Intersperse(sep));

    [Pure]
    public override int GetHashCode() =>
        items.GetHashCode();

    [Pure]
    public int CompareTo(object? obj) =>
        obj switch
        {
            AtomSeq<A> s     => CompareTo(s),
            Seq<A> s         => CompareTo(s),
            IEnumerable<A> e => CompareTo(toSeq(e)),
            _                => 1
        };

    /// <summary>
    /// Format the collection as `[a, b, c, ...]`
    /// The elipsis is used for collections over 50 items
    /// To get a formatted string with all the items, use `ToFullString`
    /// or `ToFullArrayString`.
    /// </summary>
    [Pure]
    public override string ToString() =>
        items.ToString() ?? "[]";

    /// <summary>
    /// Ordering operator
    /// </summary>
    public static bool operator >(AtomSeq<A> x, AtomSeq<A> y) =>
        x.CompareTo(y) > 0;

    /// <summary>
    /// Ordering operator
    /// </summary>
    public static bool operator >=(AtomSeq<A> x, AtomSeq<A> y) =>
        x.CompareTo(y) >= 0;

    /// <summary>
    /// Ordering  operator
    /// </summary>
    public static bool operator <(AtomSeq<A> x, AtomSeq<A> y) =>
        x.CompareTo(y) < 0;

    /// <summary>
    /// Ordering  operator
    /// </summary>
    public static bool operator <=(AtomSeq<A> x, AtomSeq<A> y) =>
        x.CompareTo(y) <= 0;

    /// <summary>
    /// Equality operator
    /// </summary>
    public static bool operator ==(AtomSeq<A> x, AtomSeq<A> y) =>
        x.Equals(y);

    /// <summary>
    /// Non-equality operator
    /// </summary>
    public static bool operator !=(AtomSeq<A> x, AtomSeq<A> y) =>
        !(x == y);

    /// <summary>
    /// Equality test
    /// </summary>
    public override bool Equals(object? obj) =>
        obj switch
        {
            AtomSeq<A> s     => Equals(s),
            Seq<A> s         => Equals(s),
            IEnumerable<A> e => Equals(toSeq(e)),
            _                => false
        };

    /// <summary>
    /// Equality test
    /// </summary>
    [Pure]
    public bool Equals(Seq<A> rhs) =>
        new Seq<A>(items).Equals(rhs);

    /// <summary>
    /// Equality test
    /// </summary>
    [Pure]
    public bool Equals(AtomSeq<A>? rhs) =>
        rhs is not null && Equals<EqDefault<A>>(rhs);

    /// <summary>
    /// Equality test
    /// </summary>
    [Pure]
    public bool Equals<EqA>(Seq<A> rhs) 
        where EqA : Eq<A> =>
         new Seq<A>(items).Equals<EqA>(rhs);

    /// <summary>
    /// Equality test
    /// </summary>
    [Pure]
    public bool Equals<EqA>(AtomSeq<A> rhs) where EqA : Eq<A> =>
        Equals<EqA>(new Seq<A>(rhs.items));
    
    /// <summary>
    /// Skip count items
    /// </summary>
    public Unit Skip(int amount) =>
        Swap(xs => xs.Skip(amount));
    
    /// <summary>
    /// Keep skipping items while the predicate is satisfied.
    /// </summary>
    /// <param name="f">predicate</param>
    public Unit SkipWhile(Func<A, bool> f) =>
        Swap(xs => xs.SkipWhile(f));
    
    /// <summary>
    /// Keep skipping items until the predicate is satisfied.
    /// </summary>
    /// <param name="f">predicate</param>
    public Unit SkipUntil(Func<A, bool> f) =>
        Swap(xs => xs.SkipUntil(f));
        
    /// <summary>
    /// Take count items
    /// </summary>
    public Unit Take(int amount) =>
        Swap(xs => xs.Take(amount));
        
    /// <summary>
    /// Take the specified number of items while the predicate is satisfied.
    /// </summary>
    /// <param name="f">predicate</param>
    public Unit TakeWhile(Func<A, bool> f) =>
        Swap(xs => xs.TakeWhile(f));
        
    /// <summary>
    /// Take the specified number of items until the predicate is satisfied.
    /// </summary>
    /// <param name="f">predicate</param>
    public Unit TakeUntil(Func<A, bool> f) =>
        Swap(xs => xs.TakeUntil(f));

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
    public Seq<Seq<A>> NonEmptyInits =>
        Snapshot().NonEmptyInits;

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
    public Seq<Seq<A>> Tails =>
        Snapshot().Tails;

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
    public Seq<Seq<A>> NonEmptyTails =>
        Snapshot().NonEmptyTails;

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
    public int CompareTo(AtomSeq<A>? rhs) =>
        rhs is null 
            ? 1
            : CompareTo<OrdDefault<A>>(rhs);

    /// <summary>
    /// Compare to another sequence
    /// </summary>
    [Pure]
    public int CompareTo<OrdA>(Seq<A> rhs) where OrdA : Ord<A> =>
        new Seq<A>(items).CompareTo<OrdA>(rhs);
        
    /// <summary>
    /// Compare to another sequence
    /// </summary>
    [Pure]
    public int CompareTo<OrdA>(AtomSeq<A> rhs) where OrdA : Ord<A> =>
        new Seq<A>(items).CompareTo<OrdA>(new Seq<A>(rhs.items));
    
    /// <summary>
    /// Force all items lazy to stream
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Unit Strict() => 
        ignore(items.Strict());

    [Pure]
    public IteratorEnumerator<A> GetEnumerator() =>
        items.GetEnumerator();
    
    [Pure]
    IEnumerator<A> IEnumerable<A>.GetEnumerator() =>
        items.GetEnumerator().GetEnumerator();

    [Pure]
    IEnumerator IEnumerable.GetEnumerator() =>
        items.GetEnumerator().GetEnumerator();
        
    [Obsolete("Use Map instead.  If you want Map that returns a new sequence rather than mutating in-place, then call ToSeq().Map()")]
    public Unit MapInPlace(Func<A, A> f) =>
        Swap(xs => xs.Map(f));

    [Obsolete("Use Bind instead.  If you want Bind that returns a new sequence rather than mutating in-place, then call ToSeq().Bind()")]
    public Unit BindInPlace(Func<A, Seq<A>> f) =>
        Swap(xs => xs.Bind(f));
 
    [Obsolete("Use Filter instead.  If you want a Filter that returns a new sequence rather than mutating in-place, then call ToSeq().Filter()")]
    public Unit FilterInPlace(Func<A, bool> f) =>
        Swap(xs => xs.Filter(f));
    
    [Obsolete("Use Snapshot() instead, I'm looking to standardise the way the Atom* types yield their backing value")]
    public Seq<A> ToSeq() =>
        new (items);
}

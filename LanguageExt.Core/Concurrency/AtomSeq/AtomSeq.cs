#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt
{
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
        IComparable
    {
        /// <summary>
        /// Empty sequence
        /// </summary>
        public static readonly Seq<A> Empty = new Seq<A>(SeqEmptyInternal<A>.Default);

        /// <summary>
        /// Internal representation of the sequence (SeqStrict|SeqLazy|SeqEmptyInternal)
        /// </summary>
        volatile ISeqInternal<A> items;

        /// <summary>
        /// Constructor from lazy sequence
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AtomSeq(IEnumerable<A> ma) : this(new SeqLazy<A>(ma ?? new A [0])) { }

        /// <summary>
        /// Constructor
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal AtomSeq(ISeqInternal<A> items) =>
            this.items = items ?? SeqEmptyInternal<A>.Default;

        /// <summary>
        /// Convert to an immutable sequence
        /// </summary>
        /// <remarks>This is effectively a zero cost operation, not even a single allocation</remarks>
        [Pure]
        public Seq<A> ToSeq() =>
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
                        ? (object?)xs.Head
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
        public A this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => items[index];
        }
        
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
        public Unit Concat(Arr<A> items)
        {
            if (items.Count == 0)
            {
                return default;
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
        public Unit Concat(Seq<A> rhs)
        {
            SpinWait sw = default;
            while (true)
            {
                var oitems = items;

                var nitems = oitems.Type switch
                             {
                                 SeqType.Empty =>

                                     // lhs is empty, so just return rhs
                                     rhs.Value,

                                 SeqType.Lazy =>
                                     rhs.Value.Type switch
                                     {
                                         // lhs lazy, rhs empty
                                         // return lhs
                                         SeqType.Empty => oitems,

                                         // lhs lazy, rhs lazy
                                         // return SeqConcat
                                         SeqType.Lazy => new SeqConcat<A>(Seq(oitems, rhs.Value)),

                                         // lhs lazy, rhs strict
                                         // force lhs to be strict and concat the two 
                                         SeqType.Strict =>
                                             ((SeqStrict<A>)oitems.Strict()).Append((SeqStrict<A>)rhs.Value),

                                         // lhs lazy, rhs concat
                                         // prepend rhs with lhs
                                         SeqType.Concat =>
                                             ((SeqConcat<A>)rhs.Value).ConsSeq(oitems),

                                         _ => throw new NotSupportedException()
                                     },

                                 SeqType.Strict =>
                                     rhs.Value.Type switch
                                     {
                                         // lhs strict, rhs empty
                                         // return lhs
                                         SeqType.Empty => oitems,

                                         // lhs strict, rhs lazy
                                         // return SeqConcat
                                         SeqType.Lazy =>
                                             new SeqConcat<A>(Seq(oitems, rhs.Value)),

                                         // lhs strict, rhs strict
                                         // append the two
                                         SeqType.Strict =>
                                             ((SeqStrict<A>)oitems).Append((SeqStrict<A>)rhs.Value),

                                         // lhs strict, rhs concat
                                         // prepend rhs with lhs
                                         SeqType.Concat =>
                                             ((SeqConcat<A>)rhs.Value).ConsSeq(oitems),

                                         _ => throw new NotSupportedException()
                                     },

                                 SeqType.Concat =>
                                     rhs.Value.Type switch
                                     {
                                         // lhs concat, rhs empty
                                         // return lhs
                                         SeqType.Empty =>
                                             oitems,

                                         // lhs concat, rhs lazy || lhs concat, rhs strict
                                         // add rhs to concat
                                         SeqType.Lazy =>
                                             ((SeqConcat<A>)oitems).AddSeq(rhs.Value),

                                         SeqType.Strict =>
                                             ((SeqConcat<A>)oitems).AddSeq(rhs.Value),

                                         // lhs concat, rhs concat
                                         // add rhs to concat
                                         SeqType.Concat =>
                                             ((SeqConcat<A>)oitems).AddSeqRange(((SeqConcat<A>)rhs.Value).ms),

                                         _ => throw new NotSupportedException()
                                     },

                                 _ => throw new NotSupportedException()
                             };

                if (ReferenceEquals(oitems, nitems))
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
                       ? Either<L, A>.Left(Left)
                       : Either<L, A>.Right(xs.Last);
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
                       ? Either<L, A>.Left(Left())
                       : Either<L, A>.Right(xs.Last);
        }

        /// <summary>
        /// Last item in sequence.
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Validation<F, A> LastOrInvalid<F>(F Fail)
        {
            var xs = items;
            return xs.IsEmpty
                       ? Validation<F, A>.Fail(Seq1(Fail))
                       : Validation<F, A>.Success(xs.Last);
        }

        /// <summary>
        /// Last item in sequence.
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Validation<F, A> LastOrInvalid<F>(Func<F> Fail)
        {
            var xs = items;
            return xs.IsEmpty
                       ? Validation<F, A>.Fail(Seq1(Fail()))
                       : Validation<F, A>.Success(xs.Last);
        }

        /// <summary>
        /// Last item in sequence.
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Validation<MonoidFail, F, A> LastOrInvalid<MonoidFail, F>(F Fail) where MonoidFail : struct, Monoid<F>, Eq<F>
        {
            var xs = items;
            return xs.IsEmpty
                       ? Validation<MonoidFail, F, A>.Fail(Fail)
                       : Validation<MonoidFail, F, A>.Success(xs.Last);
        }

        /// <summary>
        /// Last item in sequence.
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Validation<MonoidFail, F, A> LastOrInvalid<MonoidFail, F>(Func<F> Fail) where MonoidFail : struct, Monoid<F>, Eq<F>
        {
            var xs = items;
            return xs.IsEmpty
                       ? Validation<MonoidFail, F, A>.Fail(Fail())
                       : Validation<MonoidFail, F, A>.Success(xs.Last);
        }

        /// <summary>
        /// Head of the sequence if this node isn't the empty node or fail
        /// </summary>
        /// <typeparam name="Fail"></typeparam>
        /// <param name="fail">Fail case</param>
        /// <returns>Head of the sequence or fail</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Validation<Fail, A> HeadOrInvalid<Fail>(Fail fail)
        {
            var xs = items;
            return xs.IsEmpty
                       ? Fail<Fail, A>(fail)
                       : Success<Fail, A>(xs.Head);
        }

        /// <summary>
        /// Head of the sequence
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Validation<MonoidFail, Fail, A> HeadOrInvalid<MonoidFail, Fail>(Fail fail) where MonoidFail : struct, Monoid<Fail>, Eq<Fail>
        {
            var xs = items;
            return xs.IsEmpty
                       ? Fail<MonoidFail, Fail, A>(fail)
                       : Success<MonoidFail, Fail, A>(xs.Head);
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
        public bool IsEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => items.IsEmpty;
        }

        /// <summary>
        /// Returns the number of items in the sequence
        /// </summary>
        /// <returns>Number of items in the sequence</returns>
        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => items.Count;
        }
        
        /// <summary>
        /// Alias of `Count`
        /// </summary>
        /// <returns>Number of items in the sequence</returns>
        public int Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => items.Count;
        }

        /// <summary>
        /// Stream as an enumerable
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<A> AsEnumerable() => 
            items;

        /// <summary>
        /// Match empty sequence, or multi-item sequence
        /// </summary>
        /// <typeparam name="B">Return value type</typeparam>
        /// <param name="Empty">Match for an empty list</param>
        /// <param name="Tail">Match for a non-empty</param>
        /// <returns>Result of match function invoked</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        /// Impure iteration of the bound values in the structure
        /// </summary>
        /// <returns>
        /// Returns the original unmodified structure
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Unit Iter(Action<A> f) =>
            items.Iter(f);

        /// <summary>
        /// Map the sequence using the function provided
        /// </summary>
        /// <typeparam name="B"></typeparam>
        /// <param name="f">Mapping function</param>
        /// <returns>Mapped sequence</returns>
        [Pure]
        public Seq<B> Map<B>(Func<A, B> f)
        {
            return new Seq<B>(new SeqLazy<B>(Yield(items)));
            IEnumerable<B> Yield(ISeqInternal<A> items)
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
        /// <remarks>Any functions passed as arguments may be run multiple times if there are multiple threads competing
        /// to update this data structure.  Therefore the functions must spend as little time performing the injected
        /// behaviours as possible to avoid repeated attempts</remarks>
        [Pure]
        public Unit MapInPlace(Func<A, A> f)
        {
            SpinWait sw = default;
            while (true)
            {
                var oitems = items;
                var nitems = new SeqLazy<A>(oitems.Map(f));
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
        /// <returns>Flatmapped sequence</returns>
        [Pure]
        public Seq<B> Bind<B>(Func<A, Seq<B>> f)
        {
            static IEnumerable<B> Yield(ISeqInternal<A> ma, Func<A, Seq<B>> bnd)
            {
                foreach (var a in ma)
                {
                    foreach (var b in bnd(a))
                    {
                        yield return b;
                    }
                }
            }
            return new Seq<B>(Yield(items, f));
        }
        
        /// <summary>
        /// Monadic bind (flatmap) of the sequence
        /// </summary>
        /// <typeparam name="B">Bound return value type</typeparam>
        /// <param name="f">Bind function</param>
        /// <returns>Flat-mapped sequence</returns>
        /// <remarks>Any functions passed as arguments may be run multiple times if there are multiple threads competing
        /// to update this data structure.  Therefore the functions must spend as little time performing the injected
        /// behaviours as possible to avoid repeated attempts</remarks>
        public Unit BindInPlace<B>(Func<A, Seq<A>> f)
        {
            SpinWait sw = default;
            while (true)
            {
                var oitems = items;
                var nitems = new Seq<A>(oitems).Bind(f);
                if (ReferenceEquals(Interlocked.CompareExchange(ref items, nitems.Value, oitems), oitems))
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
        /// Monadic bind (flatmap) of the sequence
        /// </summary>
        /// <typeparam name="B">Bound return value type</typeparam>
        /// <param name="bind">Bind function</param>
        /// <returns>Flatmapped sequence</returns>
        [Pure]
        public Seq<C> SelectMany<B, C>(Func<A, Seq<B>> bind, Func<A, B, C> project)
        {
            static IEnumerable<C> Yield(ISeqInternal<A> ma, Func<A, Seq<B>> bnd, Func<A, B, C> prj)
            {
                foreach (var a in ma)
                {
                    foreach (var b in bnd(a))
                    {
                        yield return prj(a, b);
                    }
                }
            }
            return new Seq<C>(Yield(items, bind, project));
        }

        /// <summary>
        /// Filter the items in the sequence
        /// </summary>
        /// <param name="f">Predicate to apply to the items</param>
        /// <returns>Filtered sequence</returns>
        [Pure]
        public Seq<A> Filter(Func<A, bool> f)
        {
            return new Seq<A>(new SeqLazy<A>(Yield(items, f)));
            static IEnumerable<A> Yield(ISeqInternal<A> items, Func<A, bool> f)
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
        /// <remarks>Any functions passed as arguments may be run multiple times if there are multiple threads competing
        /// to update this data structure.  Therefore the functions must spend as little time performing the injected
        /// behaviours as possible to avoid repeated attempts</remarks>
        public Unit FilterInPlace(Func<A, bool> f)
        {
            SpinWait sw = default;
            while (true)
            {
                var oitems = items;
                var nitems = new SeqLazy<A>(oitems.Filter(f));
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
        /// Filter the items in the sequence
        /// </summary>
        /// <param name="f">Predicate to apply to the items</param>
        /// <returns>Filtered sequence</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Seq<A> Where(Func<A, bool> f) =>
            Filter(f);

        /// <summary>
        /// Fold the sequence from the first item to the last
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="f">Fold function</param>
        /// <returns>Aggregated state</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public S Fold<S>(S state, Func<S, A, S> f) =>
            items.Fold(state, f);

        /// <summary>
        /// Fold the sequence from the last item to the first.  For 
        /// sequences that are not lazy and are less than 5000 items
        /// long, FoldBackRec is called instead, because it is faster.
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="f">Fold function</param>
        /// <returns>Aggregated state</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public S FoldBack<S>(S state, Func<S, A, S> f) =>
            items.FoldBack(state, f);

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
            items.Exists(f);

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
            items.ForAll(f);

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
        public Unit Intersperse(A value)
        {
            SpinWait sw = default;
            while (true)
            {
                var oitems = items;
                var nitems = new SeqLazy<A>(oitems.Intersperse(value));
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
        /// Get the hash code for all of the items in the sequence, or 0 if empty
        /// </summary>
        /// <returns></returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() =>
            items.GetHashCode();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(object? obj) => obj switch 
        {
            AtomSeq<A>     s => CompareTo(s),
            Seq<A>         s => CompareTo(s),
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
            items is SeqLazy<A> lz
                ? CollectionFormat.ToShortArrayString(lz)
                : CollectionFormat.ToShortArrayString(items, Count);

        /// <summary>
        /// Format the collection as `a, b, c, ...`
        /// </summary>
        [Pure]
        public string ToFullString(string separator = ", ") =>
            CollectionFormat.ToFullString(items, separator);

        /// <summary>
        /// Format the collection as `[a, b, c, ...]`
        /// </summary>
        [Pure]
        public string ToFullArrayString(string separator = ", ") =>
            CollectionFormat.ToFullArrayString(items, separator);

        /// <summary>
        /// Ordering operator
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(AtomSeq<A> x, AtomSeq<A> y) =>
            x.CompareTo(y) > 0;

        /// <summary>
        /// Ordering operator
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(AtomSeq<A> x, AtomSeq<A> y) =>
            x.CompareTo(y) >= 0;

        /// <summary>
        /// Ordering  operator
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(AtomSeq<A> x, AtomSeq<A> y) =>
            x.CompareTo(y) < 0;

        /// <summary>
        /// Ordering  operator
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(AtomSeq<A> x, AtomSeq<A> y) =>
            x.CompareTo(y) <= 0;

        /// <summary>
        /// Equality operator
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(AtomSeq<A> x, AtomSeq<A> y) =>
            x.Equals(y);

        /// <summary>
        /// Non-equality operator
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(AtomSeq<A> x, AtomSeq<A> y) =>
            !(x == y);

        /// <summary>
        /// Equality test
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object? obj) => obj switch 
        {
            AtomSeq<A>     s => Equals(s),
            Seq<A>         s => Equals(s),
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(AtomSeq<A>? rhs) =>
            rhs is not null && Equals<EqDefault<A>>(rhs);

        /// <summary>
        /// Equality test
        /// </summary>
        [Pure]
        public bool Equals<EqA>(Seq<A> rhs) where EqA : struct, Eq<A>
        {
            var lhs = items;
            
            // Differing lengths?
            if(lhs.Count != rhs.Count) return false;

            // If the hash code has been calculated on both sides then 
            // check for differences
            if (lhs.GetHashCode() != rhs.GetHashCode())
            {
                return false;
            }

            // Iterate through both sides
            using var iterA = lhs.GetEnumerator();
            using var iterB = rhs.GetEnumerator();
            while (iterA.MoveNext() && iterB.MoveNext())
            {
                if (!default(EqA).Equals(iterA.Current, iterB.Current))
                {
                    return false;
                }
            }

            return true;
        }
        
        /// <summary>
        /// Equality test
        /// </summary>
        [Pure]
        public bool Equals<EqA>(AtomSeq<A> rhs) where EqA : struct, Eq<A>
        {
            var lhs = items;
            
            // Differing lengths?
            if(lhs.Count != rhs.Count) return false;

            // If the hash code has been calculated on both sides then 
            // check for differences
            if (lhs.GetHashCode() != rhs.GetHashCode())
            {
                return false;
            }

            // Iterate through both sides
            using var iterA = lhs.GetEnumerator();
            using var iterB = rhs.GetEnumerator();
            while (iterA.MoveNext() && iterB.MoveNext())
            {
                if (!default(EqA).Equals(iterA.Current, iterB.Current))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Skip count items
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Unit Skip(int amount)
        {
            SpinWait sw = default;
            while (true)
            {
                var oitems = items;
                var nitems = new SeqLazy<A>(oitems.Skip(amount));
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
        /// Take count items
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Unit Take(int amount)
        {
            SpinWait sw = default;
            while (true)
            {
                var oitems = items;
                var nitems = new SeqLazy<A>(oitems.Take(amount));
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
        /// Iterate the sequence, yielding items if they match the predicate 
        /// provided, and stopping as soon as one doesn't
        /// </summary>
        /// <returns>A new sequence with the first items that match the 
        /// predicate</returns>
        /// <remarks>Any functions passed as arguments may be run multiple times if there are multiple threads competing
        /// to update this data structure.  Therefore the functions must spend as little time performing the injected
        /// behaviours as possible to avoid repeated attempts</remarks>
        public Unit TakeWhile(Func<A, bool> pred)
        {
            SpinWait sw = default;
            while (true)
            {
                var oitems = items;
                var nitems = new SeqLazy<A>(oitems.TakeWhile(pred));
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
        /// Iterate the sequence, yielding items if they match the predicate 
        /// provided, and stopping as soon as one doesn't.  An index value is 
        /// also provided to the predicate function.
        /// </summary>
        /// <returns>A new sequence with the first items that match the 
        /// predicate</returns>
        /// <remarks>Any functions passed as arguments may be run multiple times if there are multiple threads competing
        /// to update this data structure.  Therefore the functions must spend as little time performing the injected
        /// behaviours as possible to avoid repeated attempts</remarks>
        public Unit TakeWhile(Func<A, int, bool> pred)
        {
            SpinWait sw = default;
            while (true)
            {
                var oitems = items;
                var nitems = new SeqLazy<A>(oitems.TakeWhile(pred));
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
            Seq1<Seq<A>>(Seq<A>()) + NonEmptyInits;

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
            ToSeq().NonEmptyInits;

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
            ToSeq().Tails;

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
            ToSeq().NonEmptyTails;

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
        public int CompareTo<OrdA>(Seq<A> rhs) where OrdA : struct, Ord<A>
        {
            var lhs = items;
            
            // Differing lengths?
            var cmp = lhs.Count.CompareTo(rhs.Count);
            if (cmp != 0) return cmp;

            // Iterate through both sides
            using var iterA = lhs.GetEnumerator();
            using var iterB = rhs.GetEnumerator();
            while (iterA.MoveNext() && iterB.MoveNext())
            {
                cmp = default(OrdA).Compare(iterA.Current, iterB.Current);
                if (cmp != 0) return cmp;
            }

            return 0;
        }
        
        
        /// <summary>
        /// Compare to another sequence
        /// </summary>
        [Pure]
        public int CompareTo<OrdA>(AtomSeq<A> rhs) where OrdA : struct, Ord<A>
        {
            var lhs = items;
            
            // Differing lengths?
            var cmp = lhs.Count.CompareTo(rhs.Count);
            if (cmp != 0) return cmp;

            // Iterate through both sides
            using var iterA = lhs.GetEnumerator();
            using var iterB = rhs.GetEnumerator();
            while (iterA.MoveNext() && iterB.MoveNext())
            {
                cmp = default(OrdA).Compare(iterA.Current, iterB.Current);
                if (cmp != 0) return cmp;
            }

            return 0;
        }

        /// <summary>
        /// Force all items lazy to stream
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Unit Strict() => 
            ignore(items.Strict());

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<A> GetEnumerator() =>
            items.GetEnumerator();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() =>
            items.GetEnumerator();

        [Pure]
        public Seq<B> Cast<B>() =>
            ToSeq().Cast<B>();
    }
}
#nullable disable

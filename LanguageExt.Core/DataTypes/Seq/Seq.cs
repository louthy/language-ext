using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;

namespace LanguageExt
{
    /// <summary>
    /// Cons sequence
    /// Represents a sequence of values in a similar way to IEnumerable, but without the
    /// issues of multiple evaluation for key LINQ operators like Skip, Count, etc.
    /// </summary>
    /// <typeparam name="A">Type of the values in the sequence</typeparam>
    public class Seq<A> : IEnumerable<A>, ISeq<A>, IComparable<Seq<A>>, IEquatable<Seq<A>>
    {
        const int DefaultCapacity = 8;
        const int NoCons = 1;
        const int NoAdd = 1;

        /// <summary>
        /// Empty sequence
        /// </summary>
        public static Seq<A> Empty => new Seq<A>(new A[DefaultCapacity], 4, 0, 0, 0, 0, null);

        /// <summary>
        /// Backing data
        /// </summary>
        A[] data;

        /// <summary>
        /// Index into data where the Head is
        /// </summary>
        readonly int start;

        /// <summary>
        /// Known size of the sequence - 0 means unknown
        /// </summary>
        int count;

        /// <summary>
        /// Index into data where the lazy sequence starts
        /// </summary>
        readonly int seqStart;

        /// <summary>
        /// 1 if no more consing is allowed
        /// </summary>
        int consDisallowed;

        /// <summary>
        /// 1 if no more adding is allowed
        /// </summary>
        int addDisallowed;

        /// <summary>
        /// Lazy sequence
        /// </summary>
        Enum<A> seq;

        /// <summary>
        /// Cached hash code
        /// </summary>
        int hash;

        /// <summary>
        /// Constructor from lazy sequence
        /// </summary>
        public Seq(IEnumerable<A> seq)
        {
            this.data = new A[DefaultCapacity];
            this.start = 4;
            this.count = 0;
            this.seqStart = 4;
            this.seq = new Enum<A>(seq);
            this.hash = 0;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Seq(A[] data, int start, int count, int seqStart, int noCons, int noAdd, Enum<A> seq)
        {
            this.data = data;
            this.start = start;
            this.count = count;
            this.seqStart = seqStart;
            this.seq = seq;
            this.consDisallowed = noCons;
            this.addDisallowed = noAdd;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        Seq(A[] data, int start, int count, int noCons, int noAdd)
        {
            this.data = data;
            this.start = start;
            this.count = count;
            this.consDisallowed = noCons;
            this.addDisallowed = noAdd;
        }

        public void Deconstruct(out A head, out Seq<A> tail)
        {
            head = Head;
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
            Get: la => la.HeadOrNone(),
            Set: a => la => la.IsEmpty || a.IsNone ? la : a.Value.Cons(la.Tail)
            );

        /// <summary>
        /// Tail lens
        /// </summary>
        public static Lens<Seq<A>, Seq<A>> tail => Lens<Seq<A>, Seq<A>>.New(
            Get: la => la.IsEmpty ? Seq<A>.Empty : la.Tail,
            Set: a => la => la.IsEmpty ? a : la.Head.Cons(a)
            );

        /// <summary>
        /// Last lens
        /// </summary>
        public static Lens<Seq<A>, A> last => Lens<Seq<A>, A>.New(
            Get: la => la.IsEmpty ? throw new IndexOutOfRangeException() : la.Last,
            Set: a => la => la.IsEmpty ? throw new IndexOutOfRangeException() : la.Take(la.Count - 1).Add(a)
            );

        /// <summary>
        /// Last or none lens
        /// </summary>
        public static Lens<Seq<A>, Option<A>> lastOrNone => Lens<Seq<A>, Option<A>>.New(
            Get: la => la.IsEmpty ? None : Some(la.Last),
            Set: a => la => la.IsEmpty || a.IsNone ? la : la.Take(la.Count - 1).Add(a.Value)
            );

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
        public A this[int index]
        {
            get
            {
                if (index < 0)
                {
                    throw new IndexOutOfRangeException();
                }
                if (index >= count)
                {
                    if (seq == null)
                    {
                        throw new IndexOutOfRangeException();
                    }
                    else
                    {
                        StreamNextItems(index - count + 1);
                        return this[index];
                    }
                }
                else
                {
                    return data[start + index];
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
        public Seq<A> Add(A value)
        {
            if (seq != null)
            {
                // Can't add to the end of a sequence unless we know
                // where the end is.  So, we must stream all lazy items
                // first.
                Strict();
            }

            var end = start + count;
            if (Interlocked.Exchange(ref addDisallowed, 1) == 1 || end == data.Length)
            {
                return CloneAdd(value);
            }
            else
            {
                data[end] = value;
                return new Seq<A>(data, start, count + 1, NoCons, 0);
            }
        }

        /// <summary>
        /// Add a range of items to the end of the sequence
        /// </summary>
        /// <remarks>
        /// Forces evaluation of the entire lazy sequence so the items
        /// can be appended.  
        /// </remarks>
        public Seq<A> Concat(IEnumerable<A> items)
        {
            if (seq != null)
            {
                // Can't add to the end of a sequence unless we know
                // where the end is.  So, we must stream all lazy items
                // first.
                Strict();
            }

            switch (items)
            {
                case Seq<A> seq:
                    seq = seq.Strict();
                    return Concat(seq.data, seq.start, seq.count);

                case A[] arr:
                    return Concat(arr, 0, arr.Length);

                case Arr<A> arr:
                    return Concat(arr.Value, 0, arr.Count);

                default:
                    var ndata = items.ToArray();
                    return Concat(ndata, 0, ndata.Length);
            }
        }

        /// <summary>
        /// Add a range of items to the end of the sequence
        /// </summary>
        Seq<A> Concat(A[] items, int itemsStart, int itemsCount)
        {
            var end = start + count;
            if (Interlocked.Exchange(ref addDisallowed, 1) == 1 || (end + itemsCount >= data.Length))
            {
                return CloneAddRange(items, itemsStart, itemsCount);
            }
            else
            {
                System.Array.Copy(items, itemsStart, data, end, itemsCount);
                return new Seq<A>(data, start, count + itemsCount, 0, NoCons, 0, null);
            }
        }

        /// <summary>
        /// Prepend an item to the sequence
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Seq<A> Cons(A value)
        {
            if (Interlocked.Exchange(ref consDisallowed, 1) == 1 || start == 0)
            {
                return CloneCons(value);
            }
            else
            {
                var nstart = start - 1;
                data[nstart] = value;
                return new Seq<A>(data, start - 1, count + 1, seqStart, 0, NoAdd, seq);
            }
        }

        Seq<A> CloneCons(A value)
        {
            if (start == 0)
            {
                // Find the new size of the data array
                var nlength = Math.Max(data.Length << 1, 1);

                // Allocate it
                var ndata = new A[nlength];

                // Copy the old data block to the second half of the new one
                // so we have space on the left-hand-side to put the cons'd
                // value
                System.Array.Copy(data, 0, ndata, data.Length, data.Length);

                // If we have a seq != null then we need to offset where it's
                // streaming in to.
                var nseqStart = seqStart + data.Length;

                // The new head position will be 1 cell to to left of the 
                // middle of the newly allocated block.
                var nstart = data.Length == 0
                                ? 0
                                : data.Length - 1;

                // We have one more item
                var ncount = count + 1;

                // Set the value in the new data block
                ndata[nstart] = value;

                // Return everything 
                return new Seq<A>(ndata, nstart, ncount, data.Length == 0 ? 1 : nseqStart, 0, 0, seq);
            }
            else
            {
                // We're cloning because there are multiple cons operations
                // from the same Seq.  We can't keep walking along the same 
                // array, so we clone with the exact same settings and insert

                var ndata = new A[data.Length];
                var nstart = start - 1;

                System.Array.Copy(data, start, ndata, start, count);

                ndata[nstart] = value;

                return new Seq<A>(ndata, nstart, count + 1, seqStart, 0, 0, seq);
            }
        }

        Seq<A> CloneAdd(A value)
        {
            // Find the new size of the data array
            var nlength = Math.Max(data.Length << 1, 1);

            // Allocate it
            var ndata = new A[nlength];

            // Copy the old data block to the first half of the new one
            // so we have space on the right-hand-side to put the added
            // value
            System.Array.Copy(data, 0, ndata, 0, data.Length);

            // Set the value in the new data block
            ndata[data.Length] = value;

            // Return everything 
            return new Seq<A>(ndata, start, count + 1, 0, 0, 0, null);
        }

        Seq<A> CloneAddRange(A[] values, int valuesStart, int valuesCount)
        {
            var end = start + count;

            // Find the new size of the data array
            var nlength = Math.Max(Math.Max(data.Length << 1, 1), end + valuesCount);

            // Allocate it
            var ndata = new A[nlength];

            // Copy the old data block to the first half of the new one
            // so we have space on the right-hand-side to put the added
            // value
            System.Array.Copy(data, 0, ndata, 0, end);

            // Set the value in the new data block
            System.Array.Copy(values, valuesStart, ndata, end, valuesCount);

            // Return everything 
            return new Seq<A>(ndata, start, count + valuesCount, 0, 0, 0, null);
        }

        /// <summary>
        /// Head item in the sequence.  NOTE:  If `IsEmpty` is true then Head 
        /// is undefined.  Call HeadOrNone() if for maximum safety.
        /// </summary>
        public A Head
        {
            get
            {
                if (count == 0)
                {
                    var (success, value) = StreamNextItem();

                    return success
                        ? value
                        : throw new InvalidOperationException("Sequence is empty");
                }
                else
                {
                    return data[start];
                }
            }
        }

        /// <summary>
        /// Tail of the sequence
        /// </summary>
        public Seq<A> Tail
        {
            get
            {
                if (count == 0)
                {
                    var (success, value) = StreamNextItem();

                    return success
                        ? new Seq<A>(data, start + 1, count - 1, seqStart, NoCons, NoAdd, seq)
                        : Empty;
                }
                else
                {
                    return new Seq<A>(data, start + 1, count - 1, seqStart, NoCons, NoAdd, seq);
                }
            }
        }

        /// <summary>
        /// Head of the sequence if this node isn't the empty node
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Option<A> HeadOrNone() =>
            IsEmpty
                ? None
                : Some(Head);

        /// <summary>
        /// Last item in sequence.  Throws if no items in sequence
        /// </summary>
        public A Last
        {
            get
            {
                if (seq != null)
                {
                    Strict();
                }
                return IsEmpty
                  ? throw new InvalidOperationException("Sequence is empty")
                  : data[start + count - 1];
            }
        }

        /// <summary>
        /// Last item in sequence.
        /// </summary>
        public Option<A> LastOrNone()
        {
            if (seq != null)
            {
                Strict();
            }
            return IsEmpty
                ? None
                : Some(data[start + count - 1]);
        }

        /// <summary>
        /// Last item in sequence.
        /// </summary>
        public Either<L, A> LastOrLeft<L>(L Left)
        {
            if (seq != null)
            {
                Strict();
            }
            return IsEmpty
                ? Either<L, A>.Left(Left)
                : Either<L, A>.Right(data[start + count - 1]);
        }

        /// <summary>
        /// Last item in sequence.
        /// </summary>
        public Either<L, A> LastOrLeft<L>(Func<L> Left)
        {
            if (seq != null)
            {
                Strict();
            }
            return IsEmpty
                ? Either<L, A>.Left(Left())
                : Either<L, A>.Right(data[start + count - 1]);
        }

        /// <summary>
        /// Last item in sequence.
        /// </summary>
        public Validation<F, A> LastOrInvalid<F>(F Fail)
        {
            if (seq != null)
            {
                Strict();
            }
            return IsEmpty
                ? Validation<F, A>.Fail(Seq1(Fail))
                : Validation<F, A>.Success(data[start + count - 1]);
        }

        /// <summary>
        /// Last item in sequence.
        /// </summary>
        public Validation<F, A> LastOrInvalid<F>(Func<F> Fail)
        {
            if (seq != null)
            {
                Strict();
            }
            return IsEmpty
                ? Validation<F, A>.Fail(Seq1(Fail()))
                : Validation<F, A>.Success(data[start + count - 1]);
        }

        /// <summary>
        /// Last item in sequence.
        /// </summary>
        public Validation<MonoidFail, F, A> LastOrInvalid<MonoidFail, F>(F Fail) where MonoidFail : struct, Monoid<F>, Eq<F>
        {
            if (seq != null)
            {
                Strict();
            }
            return IsEmpty
                ? Validation<MonoidFail, F, A>.Fail(Fail)
                : Validation<MonoidFail, F, A>.Success(data[start + count - 1]);
        }

        /// <summary>
        /// Last item in sequence.
        /// </summary>
        public Validation<MonoidFail, F, A> LastOrInvalid<MonoidFail, F>(Func<F> Fail) where MonoidFail : struct, Monoid<F>, Eq<F>
        {
            if (seq != null)
            {
                Strict();
            }
            return IsEmpty
                ? Validation<MonoidFail, F, A>.Fail(Fail())
                : Validation<MonoidFail, F, A>.Success(data[start + count - 1]);
        }

        /// <summary>
        /// Head of the sequence if this node isn't the empty node or fail
        /// </summary>
        /// <typeparam name="Fail"></typeparam>
        /// <param name="fail">Fail case</param>
        /// <returns>Head of the sequence or fail</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Validation<Fail, A> HeadOrInvalid<Fail>(Fail fail) =>
            IsEmpty
                ? Fail<Fail, A>(fail)
                : Success<Fail, A>(Head);

        /// <summary>
        /// Head of the sequence
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Validation<MonoidFail, Fail, A> HeadOrInvalid<MonoidFail, Fail>(Fail fail) where MonoidFail : struct, Monoid<Fail>, Eq<Fail> =>
            IsEmpty
                ? Fail<MonoidFail, Fail, A>(fail)
                : Success<MonoidFail, Fail, A>(Head);

        /// <summary>
        /// Head of the sequence if this node isn't the empty node or left
        /// </summary>
        /// <typeparam name="L"></typeparam>
        /// <param name="left">Left case</param>
        /// <returns>Head of the sequence or left</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Either<L, A> HeadOrLeft<L>(L left) =>
            IsEmpty
                ? Left<L, A>(left)
                : Right<L, A>(Head);

        /// <summary>
        /// Returns true if the sequence is empty
        /// </summary>
        /// <remarks>
        /// For lazy streams this will have to peek at the first 
        /// item.  So, the first item will be consumed.
        /// </summary>
        public bool IsEmpty =>
            seq == null
                ? count == 0
                : count == 0 && !StreamNextItem().Success;

        /// <summary>
        /// Returns the number of items in the sequence
        /// </summary>
        /// <returns>Number of items in the sequence</returns>
        public int Count
        {
            get
            {
                if (seq != null)
                {
                    Strict();
                }
                return count;
            }
        }

        /// <summary>
        /// Stream as an enumerable
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<A> AsEnumerable() =>
            this;

        /// <summary>
        /// Match empty sequence, or multi-item sequence
        /// </summary>
        /// <typeparam name="B">Return value type</typeparam>
        /// <param name="Empty">Match for an empty list</param>
        /// <param name="Tail">Match for a non-empty</param>
        /// <returns>Result of match function invoked</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public B Match<B>(
            Func<B> Empty,
            Func<A, Seq<A>, B> Tail) =>
            IsEmpty
                ? Empty()
                : Tail(this.Head, this.Tail);

        /// <summary>
        /// Match empty sequence, or one item sequence, or multi-item sequence
        /// </summary>
        /// <typeparam name="B">Return value type</typeparam>
        /// <param name="Empty">Match for an empty list</param>
        /// <param name="Tail">Match for a non-empty</param>
        /// <returns>Result of match function invoked</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public B Match<B>(
            Func<B> Empty,
            Func<A, B> Head,
            Func<A, Seq<A>, B> Tail) =>
            IsEmpty
                ? Empty()
                : this.Tail.IsEmpty
                    ? Head(this.Head)
                    : Tail(this.Head, this.Tail);

        /// <summary>
        /// Match empty sequence, or multi-item sequence
        /// </summary>
        /// <typeparam name="B">Return value type</typeparam>
        /// <param name="Empty">Match for an empty list</param>
        /// <param name="Sequence">Match for a non-empty</param>
        /// <returns>Result of match function invoked</returns>
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public B Match<B>(
            Func<B> Empty,
            Func<A, B> Head,
            Func<Seq<A>, B> Tail) =>
            IsEmpty
                ? Empty()
                : this.Tail.IsEmpty
                    ? Head(this.Head)
                    : Tail(this.Tail);

        /// <summary>
        /// Impure iteration of the bound values in the structure
        /// </summary>
        /// <returns>
        /// Returns the original unmodified structure
        /// </returns>
        public Seq<A> Do(Action<A> f)
        {
            this.Iter(f);
            return this;
        }
        
        /// <summary>
        /// Map the sequence using the function provided
        /// </summary>
        /// <typeparam name="B"></typeparam>
        /// <param name="f">Mapping function</param>
        /// <returns>Mapped sequence</returns>
        public Seq<B> Map<B>(Func<A, B> f)
        {
            IEnumerable<B> Yield(Seq<A> ma, Func<A, B> map)
            {
                foreach (var item in ma)
                {
                    yield return map(item);
                }
            }
            return new Seq<B>(Yield(this, f));
        }

        /// <summary>
        /// Map the sequence using the function provided
        /// </summary>
        /// <typeparam name="B"></typeparam>
        /// <param name="f">Mapping function</param>
        /// <returns>Mapped sequence</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Seq<B> Select<B>(Func<A, B> f) =>
            Map(f);

        /// <summary>
        /// Monadic bind (flatmap) of the sequence
        /// </summary>
        /// <typeparam name="B">Bound return value type</typeparam>
        /// <param name="f">Bind function</param>
        /// <returns>Flatmapped sequence</returns>
        public Seq<B> Bind<B>(Func<A, Seq<B>> f)
        {
            IEnumerable<B> Yield(Seq<A> ma, Func<A, Seq<B>> bnd)
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
        /// <param name="bind">Bind function</param>
        /// <returns>Flatmapped sequence</returns>
        public Seq<C> SelectMany<B, C>(Func<A, Seq<B>> bind, Func<A, B, C> project)
        {
            IEnumerable<C> Yield(Seq<A> ma, Func<A, Seq<B>> bnd, Func<A, B, C> prj)
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
        public Seq<A> Filter(Func<A, bool> f)
        {
            IEnumerable<A> Yield(Seq<A> ma, Func<A, bool> pred)
            {
                foreach (var item in ma)
                {
                    if (pred(item)) yield return item;
                }
            }
            return new Seq<A>(Yield(this, f));
        }

        /// <summary>
        /// Filter the items in the sequence
        /// </summary>
        /// <param name="f">Predicate to apply to the items</param>
        /// <returns>Filtered sequence</returns>
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public S Fold<S>(S state, Func<S, A, S> f)
        {
            foreach (var item in this)
            {
                state = f(state, item);
            }
            return state;
        }

        /// <summary>
        /// Fold the sequence from the last item to the first.  For 
        /// sequences that are not lazy and are less than 5000 items
        /// long, FoldBackRec is called instead, because it is faster.
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="f">Fold function</param>
        /// <returns>Aggregated state</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public S FoldBack<S>(S state, Func<S, A, S> f)
        {
            if (seq != null)
            {
                Strict();
            }

            for (var i = start + count - 1; i >= start; i--)
            {
                state = f(state, data[i]);
            }

            return state;
        }

        /// <summary>
        /// Returns true if the supplied predicate returns true for any
        /// item in the sequence.  False otherwise.
        /// </summary>
        /// <param name="f">Predicate to apply</param>
        /// <returns>True if the supplied predicate returns true for any
        /// item in the sequence.  False otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Exists(Func<A, bool> f) =>
            AsEnumerable().Exists(f);

        /// <summary>
        /// Returns true if the supplied predicate returns true for all
        /// items in the sequence.  False otherwise.  If there is an 
        /// empty sequence then true is returned.
        /// </summary>
        /// <param name="f">Predicate to apply</param>
        /// <returns>True if the supplied predicate returns true for all
        /// items in the sequence.  False otherwise.  If there is an 
        /// empty sequence then true is returned.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ForAll(Func<A, bool> f) =>
            AsEnumerable().ForAll(f);

        /// <summary>
        /// Returns true if the sequence has items in it
        /// </summary>
        /// <returns>True if the sequence has items in it</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Any() =>
            !IsEmpty;

        /// <summary>
        /// Get the hash code for all of the items in the sequence, or 0 if empty
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() =>
            hash = hash == 0 
                ? hash(this) 
                : hash;

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
        public static bool operator ==(Seq<A> x, Seq<A> y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            return x.Equals(y);
        }

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
        public override bool Equals(object obj) =>
            obj is ISeq<A> seq && Equals(seq);

        /// <summary>
        /// Equality test
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(ISeq<A> rhs) =>
            Enumerable.SequenceEqual(this, rhs);

        /// <summary>
        /// Equality test
        /// </summary>
        public bool Equals(Seq<A> rhs) =>
            !ReferenceEquals(rhs, null) &&
            this.seq == null && rhs.seq == null &&
            this.Count == rhs.Count &&
            this.GetHashCode() == rhs.GetHashCode() &&
            Enumerable.SequenceEqual(this, rhs)
                 ? true
                 : Enumerable.SequenceEqual(this, rhs);

        /// <summary>
        /// Skip count items
        /// </summary>
        public Seq<A> Skip(int amount)
        {
            if (amount < 1)
            {
                return this;
            }

            var end = start + count;
            var virtualEnd = start + amount;

            if (virtualEnd > end)
            {
                if (seq == null)
                {
                    return Empty;
                }
                else
                {
                    StreamNextItems(virtualEnd - end);
                    return Skip(amount);
                }
            }
            else
            {
                return new Seq<A>(data, start + amount, count - amount, seqStart, NoCons, NoAdd, seq);
            }
        }

        /// <summary>
        /// Take count items
        /// </summary>
        public Seq<A> Take(int amount)
        {
            if (amount < 1)
            {
                return Empty;
            }

            var end = start + count;
            var virtualEnd = start + amount;

            if (virtualEnd > end)
            {
                if (seq == null)
                {
                    return this;
                }
                else
                {
                    StreamNextItems(virtualEnd - end);
                    return Take(amount);
                }
            }
            else
            {
                var nlength = Math.Min(virtualEnd, end) - start;
                return new Seq<A>(data, start, nlength, 0, NoCons, NoAdd, null);
            }
        }

        /// <summary>
        /// Iterate the sequence, yielding items if they match the predicate 
        /// provided, and stopping as soon as one doesn't
        /// </summary>
        /// <returns>A new sequence with the first items that match the 
        /// predicate</returns>
        public Seq<A> TakeWhile(Func<A, bool> pred)
        {
            var halfCap = DefaultCapacity >> 1;
            var data = new A[DefaultCapacity];
            var index = halfCap;

            foreach (var item in this)
            {
                if (pred(item))
                {
                    if (index == data.Length)
                    {
                        var ndata = new A[Math.Max(1, data.Length << 1)];
                        System.Array.Copy(data, ndata, data.Length);
                        data = ndata;
                    }

                    data[index] = item;
                    index++;
                }
            }
            return index == halfCap
                ? Empty
                : new Seq<A>(data, halfCap, index, 0, 0, 0, null);
        }

        /// <summary>
        /// Iterate the sequence, yielding items if they match the predicate 
        /// provided, and stopping as soon as one doesn't.  An index value is 
        /// also provided to the predicate function.
        /// </summary>
        /// <returns>A new sequence with the first items that match the 
        /// predicate</returns>
        public Seq<A> TakeWhile(Func<A, int, bool> pred)
        {
            var halfCap = DefaultCapacity >> 1;
            var data = new A[DefaultCapacity];
            var index = halfCap;

            foreach (var item in this)
            {
                if (pred(item, index))
                {
                    if (index == data.Length)
                    {
                        var ndata = new A[Math.Max(1, data.Length << 1)];
                        System.Array.Copy(data, ndata, data.Length);
                        data = ndata;
                    }

                    data[index] = item;
                    index++;
                }
            }
            return index == halfCap
                ? Empty
                : new Seq<A>(data, halfCap, index, 0, 0, 0, null);
        }

        /// <summary>
        /// Compare to another sequence
        /// </summary>
        public int CompareTo(ISeq<A> other) =>
            CompareTo<OrdDefault<A>>(other);

        /// <summary>
        /// Compare to another sequence
        /// </summary>
        public int CompareTo<OrdA>(ISeq<A> other) where OrdA : struct, Ord<A>
        {
            var x = this;
            var y = other;
            var cmp = 0;

            var iterX = x.GetEnumerator();
            var iterY = y.GetEnumerator();

            while (iterX.MoveNext() && iterY.MoveNext())
            {
                cmp = default(OrdA).Compare(iterX.Current, iterY.Current);
                if (cmp != 0) return cmp;
            }
            return cmp;
        }

        /// <summary>
        /// Compare to another sequence
        /// </summary>
        public int CompareTo(Seq<A> other) =>
            CompareTo<OrdDefault<A>>(other);

        /// <summary>
        /// Compare to another sequence
        /// </summary>
        public int CompareTo<OrdA>(Seq<A> other) where OrdA : struct, Ord<A>
        {
            if (ReferenceEquals(other, null)) return 1;

            var x = this;
            var y = other;
            var cmp = 0;

            if (x.seq == null && y.seq == null)
            {
                cmp = x.Count.CompareTo(y.Count);
                if (cmp != 0) return cmp;
            }

            var iterX = x.GetEnumerator();
            var iterY = y.GetEnumerator();

            while (iterX.MoveNext() && iterY.MoveNext())
            {
                cmp = default(OrdA).Compare(iterX.Current, iterY.Current);
                if (cmp != 0) return cmp;
            }
            return cmp;
        }

        /// <summary>
        /// Force all items lazy to stream
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Seq<A> Strict() =>
            StreamNextItems(Int32.MaxValue);

        /// <summary>
        /// Stream the next lazy item
        /// </summary>
        (bool Success, A Value) StreamNextItem()
        {
            if (seq == null)
            {
                // Nothing left to stream, so we result Fail
                return (false, default(A));
            }

            var localCount = count;
            lock (seq)
            {
                if (seq == null)
                {
                    // Nothing left to stream, so we result Fail
                    return localCount < count
                        ? (true, data[start + localCount])
                        : (false, default(A));
                }
                else
                {
                    var end = start + count;
                    if (end < seqStart)
                    {
                        // We're trying to stream something before
                        // the seq, so let's just honour the item
                        return (true, data[end]);
                    }
                    else
                    {
                        var (success, value) = seq.Get(end - seqStart);
                        if (success)
                        {
                            if (data.Length == end)
                            {
                                var ndata = new A[Math.Max(end << 1, 1)];
                                System.Array.Copy(data, ndata, data.Length);
                                data = ndata;
                            }
                            data[end] = value;
                            count++;
                            return (true, value);
                        }
                        else
                        {
                            seq = null;
                            return (false, value);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Force all items lazy to stream
        /// </summary>
        Seq<A> StreamNextItems(int amount)
        {
            if (seq == null)
            {
                // Nothing left to stream
                return this;
            }

            lock (seq)
            {
                if (seq == null)
                {
                    // Nothing left to stream, so we result Fail
                    return this;
                }
                else
                {
                    var end = Math.Max(start + count, seqStart);

                    while (amount > 0)
                    {
                        amount--;

                        var (success, value) = seq.Get(end - seqStart);
                        if (success)
                        {
                            if (data.Length == end)
                            {
                                var ndata = new A[Math.Max(end << 1, 1)];
                                System.Array.Copy(data, ndata, data.Length);
                                data = ndata;
                            }
                            data[end] = value;
                            count++;
                            end++;
                        }
                        else
                        {
                            seq = null;
                            return this;
                        }
                    }
                    return this;
                }
            }
        }

        /// <summary>
        /// Enumerator
        /// </summary>
        public IEnumerator<A> GetEnumerator()
        {
            var end = start + count;
            var i = 0;

            // First yield the already cached items
            for (i = start; i < end; i++)
            {
                yield return data[i];
            }

            if (seq == null)
            {
                end = start + count;
                for (; i < end; i++)
                {
                    yield return data[i];
                }
                yield break;
            }

            lock (seq)
            {
                bool success = true;
                A value = default(A);
                while (success)
                {
                    end = start + count;

                    // First yield the already cached items
                    if (i < end)
                    {
                        yield return data[i];
                    }
                    else if (seq == null)
                    {
                        yield break;
                    }
                    else
                    {
                        (success, value) = StreamNextItem();
                        if (success)
                        {
                            yield return value;
                        }
                    }
                    i++;
                }
            }
        }

        /// <summary>
        /// Enumerator
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        /// <summary>
        /// Implicit conversion from an untyped empty list
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Seq<A>(SeqEmpty _) =>
            Empty;
    }
}

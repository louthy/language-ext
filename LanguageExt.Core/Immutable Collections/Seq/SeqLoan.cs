using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.ClassInstances;
using System.Threading;

namespace LanguageExt
{
    /// <summary>
    /// Represents a sequence on loan from an ArrayPool
    /// </summary>
    /// <remarks>
    /// This supports rapid reading of data for use in streaming situations.  As soon as any transformations are made 
    /// the backing data is baked into a Seq of A.  This involves cloning the original array (because obviously the rented
    /// array will be returned to the pool eventually).
    ///
    /// You can manually call Dispose() to release the Array, however it also supports a finaliser to return the backing
    /// array back to its pool of origin.
    ///
    /// NOTE: If you call Dispose() whilst using this structure on another thread, behaviour is undefined.
    /// </remarks>
    /// <typeparam name="A">Bound value</typeparam>
    public class SeqLoan<A> : IDisposable
    {
        /// <summary>
        /// Backing data
        /// </summary>
        internal readonly A[] data;

        /// <summary>
        /// The pool the date came from
        /// </summary>
        internal readonly ArrayPool<A> pool;

        /// <summary>
        /// Start of the sequence
        /// </summary>
        internal readonly int start;
        
        /// <summary>
        /// Known size of the sequence
        /// </summary>
        internal readonly int count;

        /// <summary>
        /// Flags whether the rented array has been freed
        /// </summary>
        internal int freed = 0;

        /// <summary>
        /// Cached hash code
        /// </summary>
        int selfHash = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal SeqLoan(A[] data, ArrayPool<A> pool, int start, int count)
        {
            this.data  = data;
            this.pool  = pool;
            this.start = start;
            this.count = count;
        }

        /// <summary>
        /// Create a newly rented array
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SeqLoan<A> Rent(ArrayPool<A> pool, int size) =>
            new SeqLoan<A>(pool.Rent(size), pool, 0, size);

        /// <summary>
        /// Create a newly rented array
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SeqLoan<A> Rent(int size) =>
            Rent(ArrayPool<A>.Shared, size);
        
        /// <summary>
        /// Indexer
        /// </summary>
        public A this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => freed == 1 || index < 0 || index >= count
                       ? throw new IndexOutOfRangeException()
                       : data[start + index];
        }

        /// <summary>
        /// Head item in the sequence.  NOTE:  If `IsEmpty` is true then Head
        /// is undefined.  Call HeadOrNone() if for maximum safety.
        /// </summary>
        [Pure]
        public A Head
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => freed == 1 || count == 0
                ? throw new InvalidOperationException("Sequence is empty")
                : data[start];
        }

        /// <summary>
        /// Clone to a Seq
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Seq<A> ToSeq()
        {
            if(freed == 1) return Seq<A>.Empty;
            var ndata = new A[data.Length];
            System.Array.Copy(data, start, ndata, start, count);
            return new Seq<A>(new SeqStrict<A>(ndata, start, count, 0, 0));
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<A> ToReadOnlySpan() =>
            freed == 1 
                ? new ReadOnlySpan<A>(new A [0], 0, 0)
                : new ReadOnlySpan<A>(data, start, count);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<A> ToSpan() =>
            freed == 1 
                ? new Span<A>(new A [0], 0, 0)
                : new Span<A>(data, start, count);

        /// <summary>
        /// Tail of the sequence
        /// </summary>
        [Pure]
        public Seq<A> Tail
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => freed == 1 
                       ? Seq<A>.Empty
                       : ToSeq().Tail;
        }

        [Pure]
        public Seq<A> Init
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => freed == 1 
                      ? Seq<A>.Empty
                      : ToSeq().Tail;
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
            get => freed == 1 || count == 0;
        }

        /// <summary>
        /// Last item in sequence.  Throws if no items in sequence
        /// </summary>
        public A Last
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get =>
                freed == 1 || IsEmpty
                    ? throw new InvalidOperationException("Sequence is empty")
                    : data[start + count - 1];
        }

        /// <summary>
        /// Returns the number of items in the sequence
        /// </summary>
        /// <returns>Number of items in the sequence</returns>
        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => freed == 1 ? 0 : count;
        }

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
            if (freed == 1) return state;
            var end = start + count;
            for(var i = start; i < end; i++)
            {
                state = f(state, data[i]);
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
            if (freed == 1) return state;
            for (var i = start + count - 1; i >= start; i--)
            {
                state = f(state, data[i]);
            }
            return state;
        }

        /// <summary>
        /// Skip count items
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Seq<A> Skip(int amount) =>
            freed == 1
                ? Seq<A>.Empty
                : ToSeq().Skip(amount);
        
        /// <summary>
        /// Take count items
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Seq<A> Take(int amount) =>
            freed == 1
                ? Seq<A>.Empty
                : ToSeq().Take(amount);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator GetEnumerator() =>
            freed == 1 
                ? new Enumerator(null, 0, 0)
                : new Enumerator(data, start, count);

        public Unit Iter(Action<A> f)
        {
            var end = start + count;
            for (var i = start; i < end; i++)
            {
                f(data[i]);
            }
            return default;
        }

        public bool Exists(Func<A, bool> f)
        {
            if (freed == 1) return false;
            var end = start + count;
            for (var i = start; i < end; i++)
            {
                if(f(data[i]))
                {
                    return true;
                }
            }
            return false;
        }

        public bool ForAll(Func<A, bool> f)
        {
            if (freed == 1) return false;
            var end = start + count;
            for (var i = start; i < end; i++)
            {
                if (!f(data[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public Seq<A> Append(Seq<A> right) =>
            freed == 1
                ? right
                : ToSeq() + right;

        public Seq<A> Append(SeqLoan<A> right) =>
            freed == 1
                ? right.ToSeq()
                : ToSeq() + right.ToSeq();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() =>
            freed == 1
                ? 0
                : selfHash == 0
                    ? selfHash = GetHashCode(FNV32.OffsetBasis)
                    : selfHash;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int GetHashCode(int offsetBasis) =>
            FNV32.Hash<HashableDefault<A>, A>(data, start, count, offsetBasis);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose() =>
            Free();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        ~SeqLoan() =>
            Free();
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Free()
        {
            if (Interlocked.CompareExchange(ref freed, 1, 0) == 0)
            {
                pool.Return(data, ClearArray);
            }
        }

        static readonly bool ClearArray = 
            !typeof(A).IsValueType;
        
        public ref struct Enumerator
        {
            readonly A[] data;
            readonly int count;
            int index;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Enumerator(A[] data, int index, int count)
            {
                this.data  = data;
                this.count = count;
                this.index = index;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                var nindex = index + 1;
                if (nindex < count)
                {
                    index = nindex;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public ref A Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => ref data[index];
            }
        }        
    }
}

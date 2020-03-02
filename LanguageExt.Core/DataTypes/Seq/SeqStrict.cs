using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using LanguageExt.ClassInstances;

namespace LanguageExt
{
    internal class SeqStrict<A> : ISeqInternal<A>
    {
        const int DefaultCapacity = 8;
        const int HalfDefaultCapacity = DefaultCapacity >> 1;

        const int NoCons = 1;
        const int NoAdd = 1;

        /// <summary>
        /// Backing data
        /// </summary>
        internal readonly A[] data;

        /// <summary>
        /// Index into data where the Head is
        /// </summary>
        internal readonly int start;

        /// <summary>
        /// Known size of the sequence
        /// </summary>
        internal readonly int count;

        /// <summary>
        /// 1 if no more consing is allowed
        /// </summary>
        int consDisallowed;

        /// <summary>
        /// 1 if no more adding is allowed
        /// </summary>
        int addDisallowed;

        /// <summary>
        /// Constructor
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SeqStrict(A[] data, int start, int count, int consDisallowed, int addDisallowed)
        {
            this.data = data;
            this.start = start;
            this.count = count;
            this.consDisallowed = consDisallowed;
            this.addDisallowed = addDisallowed;
        }

        /// <summary>
        /// Add constructor (called in the Add function only)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SeqStrict(A[] data, int start, int count)
        {
            this.data = data;
            this.start = start;
            this.count = count;
            this.consDisallowed = NoCons;
        }

        /// <summary>
        /// Indexer
        /// </summary>
        public A this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => index < 0 || index >= count
                       ? throw new IndexOutOfRangeException()
                       : data[start + index];
        }

        /// <summary>
        /// Add an item to the end of the sequence
        /// </summary>
        /// <remarks>
        /// Forces evaluation of the entire lazy sequence so the item 
        /// can be appended
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ISeqInternal<A> Add(A value)
        {
            var end = start + count;
            if (1 == Interlocked.Exchange(ref addDisallowed, 1) || end == data.Length)
            {
                return CloneAdd(value);
            }
            else
            {
                data[end] = value;
                return new SeqStrict<A>(data, start, count + 1);
            }
        }

        /// <summary>
        /// Add a range of items to the end of the sequence
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        SeqStrict<A> Concat(A[] items, int itemsStart, int itemsCount)
        {
            var end = start + count;
            if (1 == Interlocked.Exchange(ref addDisallowed, 1) || (end + itemsCount >= data.Length))
            {
                return CloneAddRange(items, itemsStart, itemsCount);
            }
            else
            {
                System.Array.Copy(items, itemsStart, data, end, itemsCount);
                return new SeqStrict<A>(data, start, count + itemsCount, NoCons, 0);
            }
        }

        /// <summary>
        /// Prepend an item to the sequence
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ISeqInternal<A> Cons(A value)
        {
            if (1 == Interlocked.Exchange(ref consDisallowed, 1) || start == 0)
            {
                return CloneCons(value);
            }
            else
            {
                var nstart = start - 1;
                data[nstart] = value;
                return new SeqStrict<A>(data, start - 1, count + 1, 0, NoAdd);
            }
        }

        SeqStrict<A> CloneCons(A value)
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
                return new SeqStrict<A>(ndata, nstart, ncount, 0, 0);
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

                return new SeqStrict<A>(ndata, nstart, count + 1, 0, 0);
            }
        }

        SeqStrict<A> CloneAdd(A value)
        {
            var end = start + count;

            // Find the new size of the data array
            var nlength = data.Length == end
                ? Math.Max(data.Length << 1, 1)
                : data.Length;

            // Allocate it
            var ndata = new A[nlength];

            // Copy the old data block to the first half of the new one
            // so we have space on the right-hand-side to put the added
            // value
            System.Array.Copy(data, 0, ndata, 0, data.Length);

            // Set the value in the new data block
            ndata[end] = value;

            // Return everything 
            return new SeqStrict<A>(ndata, start, count + 1, 0, 0);
        }

        SeqStrict<A> CloneAddRange(A[] values, int valuesStart, int valuesCount)
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
            return new SeqStrict<A>(ndata, start, count + valuesCount, 0, 0);
        }

        /// <summary>
        /// Head item in the sequence.  NOTE:  If `IsEmpty` is true then Head
        /// is undefined.  Call HeadOrNone() if for maximum safety.
        /// </summary>
        public A Head
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => count == 0
                ? throw new InvalidOperationException("Sequence is empty")
                : data[start];
        }

        /// <summary>
        /// Tail of the sequence
        /// </summary>
        public ISeqInternal<A> Tail
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => count < 1
                ? SeqEmptyInternal<A>.Default
                : new SeqStrict<A>(data, start + 1, count - 1, NoCons, NoAdd);
        }

        public ISeqInternal<A> Init
        {
            get
            {
                var take = count - 1;

                return take <= 0
                    ? SeqEmptyInternal<A>.Default
                    : new SeqStrict<A>(data, start, take, NoCons, NoAdd);
            }
        }

        /// <summary>
        /// Returns true if the sequence is empty
        /// </summary>
        /// <remarks>
        /// For lazy streams this will have to peek at the first 
        /// item.  So, the first item will be consumed.
        /// </summary>
        public bool IsEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => count == 0;
        }

        /// <summary>
        /// Last item in sequence.  Throws if no items in sequence
        /// </summary>
        public A Last
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return IsEmpty
                  ? throw new InvalidOperationException("Sequence is empty")
                  : data[start + count - 1];
            }
        }

        /// <summary>
        /// Returns the number of items in the sequence
        /// </summary>
        /// <returns>Number of items in the sequence</returns>
        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => count;
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
        public ISeqInternal<A> Skip(int amount)
        {
            if (amount < 1)
            {
                return this;
            }

            var end = start + count;
            var newStart = start + amount;
            return newStart < end
                ? new SeqStrict<A>(data, newStart, count - amount, NoCons, NoAdd)
                : SeqEmptyInternal<A>.Default;
        }

        /// <summary>
        /// Take count items
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ISeqInternal<A> Take(int amount) =>
            amount < count
                ? new SeqStrict<A>(data, start, amount, NoCons, NoAdd)
                : this;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ISeqInternal<A> Strict() =>
            this;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static SeqStrict<A> FromSingleValue(A value) =>
            new SeqStrict<A>( new[] {
                default,
                default,
                default,
                default,
                value,
                default,
                default,
                default
            }, 4, 1, 0, 0);

        public IEnumerator<A> GetEnumerator()
        {
            var end = start + count;
            for (var i = start; i < end; i++)
            {
                yield return data[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            var end = start + count;
            for (var i = start; i < end; i++)
            {
                yield return data[i];
            }
        }

        public SeqStrict<A> SetItem(int index, A value)
        {
            var ndata = new A[data.Length];
            Array.Copy(data, start, ndata, start, count);
            ndata[index] = value;
            return new SeqStrict<A>(data, start, count);
        }

        public ISeqInternal<A> Filter(Func<A, bool> f)
        {
            var ndata = new A[data.Length];
            var end = start + count;
            var ncount = 0;
            for(var i = start; i < end; i++)
            {
                var item = data[i];
                if (f(data[i]))
                {
                    ndata[start + ncount] = item;
                    ncount++;
                }
            }

            return new SeqStrict<A>(ndata, start, ncount);
        }

        public ISeqInternal<B> Map<B>(Func<A, B> f)
        {
            var ndata = new B[data.Length];
            var end = start + count;
            for (var i = start; i < end; i++)
            {
                ndata[i] = f(data[i]);
            }
            return new SeqStrict<B>(ndata, start, count);
        }

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

        public SeqType Type => SeqType.Strict;

        public SeqStrict<A> Append(SeqStrict<A> right)
        {
            var end = start + count + right.count;
            if (end > data.Length || 1 == Interlocked.Exchange(ref addDisallowed, 1))
            {
                // Clone
                var nsize = 8;
                while(nsize < end)
                {
                    nsize = nsize << 1;
                }

                var ndata = new A[nsize];
                Array.Copy(data, start, ndata, start, count);
                Array.Copy(right.data, right.start, ndata, start + count, right.count);
                return new SeqStrict<A>(ndata, start, count + right.count, 0, 0);
            }
            else
            {
                Array.Copy(right.data, right.start, data, start + count, right.count);
                return new SeqStrict<A>(data, start, count + right.count, NoCons, 0);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetHashCode(int offsetBasis) =>
            FNV32.Hash<HashableDefault<A>, A>(data, start, count, offsetBasis);
    }
}

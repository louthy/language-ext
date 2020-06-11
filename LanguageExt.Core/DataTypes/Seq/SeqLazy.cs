using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using LanguageExt.ClassInstances;

namespace LanguageExt
{
    internal class SeqLazy<A> : ISeqInternal<A>
    {
        const int DefaultCapacity = 8;
        const int NoCons = 1;

        /// <summary>
        /// Backing data
        /// </summary>
        readonly A[] data;

        /// <summary>
        /// Index into data where the Head is
        /// </summary>
        readonly int start;

        /// <summary>
        /// Known size of the sequence - 0 means unknown
        /// </summary>
        readonly int count;

        /// <summary>
        /// 1 if no more consing is allowed
        /// </summary>
        int consDisallowed;

        /// <summary>
        /// Lazy sequence
        /// </summary>
        readonly Enum<A> seq;

        /// <summary>
        /// Start position in sequence
        /// </summary>
        readonly int seqStart;

        /// <summary>
        /// Constructor
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal SeqLazy(IEnumerable<A> ma) : this(new A[DefaultCapacity], DefaultCapacity, 0, 0, new Enum<A>(ma), 0)
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        SeqLazy(A[] data, int start, int count, int noCons, Enum<A> seq, int seqStart)
        {
            this.data = data;
            this.start = start;
            this.count = count;
            this.seq = seq;
            this.seqStart = seqStart;
            this.consDisallowed = noCons;
        }

        public A this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (index < 0) throw new IndexOutOfRangeException();
                if (index < count) return data[data.Length - count];
                var lazyIndex = index - count + seqStart;
                var (succ, val) = StreamTo(lazyIndex);
                return succ
                    ? val
                    : throw new IndexOutOfRangeException();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        (bool Success, A Value) StreamTo(int index)
        {
            if(index < seq.Count) return seq.Get(index);
            while (seq.Count <= index && seq.Get(seq.Count).Success) ;
            return index < seq.Count
                ? (true, seq.Data[index])
                : (false, default);
        }


        public A Head
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if(count > 0)
                {
                    return data[data.Length - count];
                }
                else if(seq.Count > seqStart)
                {
                    return seq.Data[seqStart];
                }
                else
                {
                    var (succ, val) = seq.Get(seqStart);
                    return succ
                        ? val
                        : throw new InvalidOperationException("Sequence is empty");
                }
            }
        }

        public ISeqInternal<A> Tail
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if(count > 0)
                {
                    return new SeqLazy<A>(data, start + 1, count - 1, NoCons, seq, seqStart);
                }
                else if(seq.Count > seqStart)
                {
                    return new SeqLazy<A>(data, start, count, NoCons, seq, seqStart + 1);
                }
                else
                {
                    var (succ, _) = StreamTo(seq.Count);
                    if(succ)
                    {
                        return new SeqLazy<A>(data, start, count, NoCons, seq, seqStart + 1);
                    }
                    else
                    {
                        return SeqEmptyInternal<A>.Default;
                    }
                }
            }
        }

        public bool IsEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => !(count > 0 || (seq.Count - seqStart) > 0 || seq.Get(seqStart).Success);
        }

        public A Last
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                InternalStrict();
                return seq.Count > seqStart ? seq.Data[seq.Count - 1]
                     : count > 0            ? data[data.Length - 1]
                     : throw new InvalidOperationException("Sequence is empty");
            }
        }

        public ISeqInternal<A> Init
        {
            get
            {
                var take = Count - 1;
                return take <= 0
                    ? SeqEmptyInternal<A>.Default
                    : Take(take);
            }
        }

        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                InternalStrict();
                return count + seq.Count - seqStart;
            }
        }

        public ISeqInternal<A> Add(A value)
        {
            InternalStrict();
            var seqCount = seq.Count - seqStart;
            var total = count + seqCount + 1;
            var len = DefaultCapacity;
            while (len < total) len = len << 1;

            var ndata = new A[len];

            if (count > 0)
            {
                Array.Copy(data, data.Length - count, ndata, 0, count);
            }
            if (seqCount > 0)
            {
                Array.Copy(seq.Data, seqStart, ndata, count, seqCount);
            }
            ndata[count + seqCount] = value;

            return new SeqStrict<A>(ndata, 0, total, 0, 0);
        }

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
                return new SeqLazy<A>(data, start - 1, count + 1, 0, seq, seqStart);
            }
        }

        SeqLazy<A> CloneCons(A value)
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
                var nstart = data.Length - 1;

                // We have one more item
                var ncount = count + 1;

                // Set the value in the new data block
                ndata[nstart] = value;

                // Return everything 
                return new SeqLazy<A>(ndata, nstart, ncount, 0, seq, seqStart);
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

                return new SeqLazy<A>(ndata, nstart, count + 1, 0, seq, seqStart);
            }
        }

        public S Fold<S>(S state, Func<S, A, S> f)
        {
            InternalStrict();
            if (count > 0)
            {
                for (var i = data.Length - count; i < data.Length; i++)
                {
                    state = f(state, data[i]);
                }
            }
            if (seq.Count - seqStart > 0)
            {
                var scount = seq.Count;
                var sdata = seq.Data;
                for (var i = seqStart; i < scount; i++)
                {
                    state = f(state, sdata[i]);
                }
            }
            return state;
        }

        public S FoldBack<S>(S state, Func<S, A, S> f)
        {
            InternalStrict();
            if (seq.Count - seqStart > 0)
            {
                var sdata = seq.Data;
                for (var i = seq.Count - 1; i >= seqStart; i--)
                {
                    state = f(state, sdata[i]);
                }
            }
            if (count > 0)
            {
                var nstart = data.Length - count;
                for (var i = data.Length - 1; i >= nstart; i--)
                {
                    state = f(state, data[i]);
                }
            }
            return state;
        }

        public ISeqInternal<A> Skip(int amount)
        {
            if(amount < count)
            {
                return new SeqLazy<A>(data, start + amount, count - amount, NoCons, seq, seqStart);
            }
            else if (amount == count)
            {
                return new SeqLazy<A>(new A[DefaultCapacity], DefaultCapacity, 0, 0, seq, seqStart);
            }
            else
            {
                var namount = amount - count;
                var end = seqStart + namount;
                if (end > seq.Count)
                {
                    for (var i = seqStart; i < end && seq.Get(i).Success; i++) ;
                }

                if(seq.Count >= end)
                {
                    return new SeqLazy<A>(new A[DefaultCapacity], DefaultCapacity, 0, 0, seq, end);
                }
                else
                {
                    return SeqEmptyInternal<A>.Default;
                }
            }
        }

        ISeqInternal<A> InternalStrict()
        {
            while (seq.Get(seq.Count).Success) ;
            return this;
        }

        public ISeqInternal<A> Strict()
        {
            InternalStrict();

            var len = DefaultCapacity;
            var ncount = count + seq.Count - seqStart;
            while (len < ncount) len = len << 1;

            var nstart = (len - ncount) >> 1;

            var ndata = new A[len];
            if (count > 0)
            {
                Array.Copy(data, data.Length - count, ndata, nstart, count);
            }
            if (seq.Count > 0)
            {
                Array.Copy(seq.Data, seqStart, ndata, nstart + count, seq.Count - seqStart);
            }
            return new SeqStrict<A>(ndata, nstart, ncount, 0, 0);
        }

        public ISeqInternal<A> Take(int amount)
        {
            if(amount <= count)
            {
                var ndata = new A[data.Length];
                var nstart = data.Length - count;
                Array.Copy(data, nstart, ndata, nstart, data.Length);
                return new SeqStrict<A>(ndata, start, amount, 0, 0);
            }
            else
            {
                var namount = amount - count;
                var end = seqStart + namount;
                for (var i = seqStart; i < end && seq.Get(i).Success; i++) ;
                var seqLen = seq.Count - seqStart;

                var len = DefaultCapacity;
                while (len < amount) len = len << 1;

                var ndata = new A[len];
                var nstart = (len - amount) >> 1;
                if (count > 0)
                {
                    Array.Copy(data, data.Length - count, ndata, nstart, count);
                }
                if (seq.Count - seqStart > 0)
                {
                    Array.Copy(seq.Data, seqStart, ndata, nstart + count, amount - count);
                }
                return new SeqStrict<A>(ndata, nstart, amount, 0, 0);
            }
        }

        public IEnumerator<A> GetEnumerator()
        {
            int nstart = data.Length - count;
            int nend = data.Length;

            for (var i = nstart; i < nend; i++)
            {
                yield return data[i];
            }
            for(var i = seqStart; ; i++)
            {
                var (succ, val) = seq.Get(i);
                if(succ)
                {
                    yield return val;
                }
                else
                {
                    yield break;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            int nstart = data.Length - count;
            int nend = data.Length;

            for (var i = nstart; i < nend; i++)
            {
                yield return data[i];
            }
            for (var i = seqStart; ; i++)
            {
                var (succ, val) = seq.Get(i);
                if (succ)
                {
                    yield return val;
                }
                else
                {
                    yield break;
                }
            }
        }

        public Unit Iter(Action<A> f)
        {
            foreach(var item in this)
            {
                f(item);
            }
            return default;
        }

        public bool Exists(Func<A, bool> f)
        {
            foreach(var item in this)
            {
                if (f(item))
                {
                    return true;
                }
            }
            return false;
        }

        public bool ForAll(Func<A, bool> f)
        {
            foreach (var item in this)
            {
                if (!f(item))
                {
                    return false;
                }
            }
            return true;
        }

        public SeqType Type => 
            SeqType.Lazy;

        public int GetHashCode(int hash)
        {
            InternalStrict();
            if (count > 0)
            {
                hash = FNV32.Hash<HashableDefault<A>, A>(data, start, count, hash);
            }
            if (seq.Count > 0)
            {
                hash = FNV32.Hash<HashableDefault<A>, A>(seq.Data, 0, seq.Count, hash);
            }
            return hash;
        }
    }
}

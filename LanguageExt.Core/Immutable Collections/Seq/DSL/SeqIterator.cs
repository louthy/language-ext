using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using LanguageExt.ClassInstances;
using LanguageExt.Common;

namespace LanguageExt;

class SeqIterator<A> : ISeqInternal<A>
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
    readonly Iter<A> seq;

    /// <summary>
    /// Start position in sequence
    /// </summary>
    readonly int lazyStart;

    /// <summary>
    /// Cached hash code
    /// </summary>
    int selfHash;

    public ReadOnlySpan<A> AsSpan() =>
        Strict().AsSpan();

    public Seq.FoldState InitFoldState() =>
        // ReSharper disable once GenericEnumeratorNotDisposed
        Seq.FoldState.FromEnumerator(GetEnumerator());

    /// <summary>
    /// Constructor
    /// </summary>
    internal SeqIterator(Iterator<A> ma) : 
        this(new A[DefaultCapacity], DefaultCapacity, 0, 0, new Iter<A>(ma), 0)
    { }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="data">Raw backing buffer</param>
    /// <param name="start">Start position in the backing buffer - all list data starts here</param>
    /// <param name="count">The number of strict list-items acquired so-far</param>
    /// <param name="noCons">1 if the Seq doesn't support consing - because another instance is sharing the backing buffer and is using it</param>
    /// <param name="seq">The lazy iterator</param>
    /// <param name="lazyStart">The start position for the lazy items as they come in</param>
    SeqIterator(A[] data, int start, int count, int noCons, Iter<A> seq, int lazyStart)
    {
        this.data = data;
        this.start = start;
        this.count = count;
        this.seq = seq;
        this.lazyStart = lazyStart;
        consDisallowed = noCons;
    }

    public A this[int index]
    {
        get
        {
            var r = At(index);
            if (r.IsSome) return r.Value!;
            throw new IndexOutOfRangeException();
        }
    }

    public Option<A> At(int index)
    {
        if (index < 0) return default;
        if (index < count) return data[start + count];
        
        var lazyIndex = index - count + lazyStart;
        var (succ, val) = StreamTo(lazyIndex);
        return succ
                   ? val
                   : default(Option<A>);
    }

    (bool Success, A? Value) StreamTo(int index)
    {
        if(index < seq.Count) return seq.Get(index);
        while (seq.Count <= index && seq.Get(seq.Count).Success)
        {
            // this is empty intentionally
        }
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
                return data[^count];
            }
            else if(seq.Count > lazyStart)
            {
                return seq.Data[lazyStart];
            }
            else
            {
                var (succ, val) = seq.Get(lazyStart);
                return succ
                           ? val!
                           : throw Exceptions.SequenceEmpty;

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
                return new SeqIterator<A>(data, start + 1, count - 1, NoCons, seq, lazyStart);
            }
            else if(seq.Count > lazyStart)
            {
                return new SeqIterator<A>(data, start, count, NoCons, seq, lazyStart + 1);
            }
            else
            {
                var (succ, _) = StreamTo(seq.Count);
                if(succ)
                {
                    return new SeqIterator<A>(data, start, count, NoCons, seq, lazyStart + 1);
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
        get => !(count > 0 || seq.Count - lazyStart > 0 || seq.Get(lazyStart).Success);
    }

    public A Last
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            InternalStrict();
            return seq.Count > lazyStart ? seq.Data[seq.Count - 1]
                   : count   > 0        ? data[^1]
                   : throw Exceptions.SequenceEmpty;
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
            return count + seq.Count - lazyStart;
        }
    }

    public ISeqInternal<A> Add(A value)
    {
        InternalStrict();
        var seqCount = seq.Count - lazyStart;
        var total    = count     + seqCount + 1;
        var len      = DefaultCapacity;
        while (len < total) len <<= 1;

        var ndata = new A[len];

        if (count > 0)
        {
            Array.Copy(data, data.Length - count, ndata, 0, count);
        }
        if (seqCount > 0)
        {
            Array.Copy(seq.Data, lazyStart, ndata, count, seqCount);
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
            return new SeqIterator<A>(data, start - 1, count + 1, 0, seq, lazyStart);
        }
    }

    SeqIterator<A> CloneCons(A value)
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
            Array.Copy(data, 0, ndata, data.Length, data.Length);

            // The new head position will be 1 cell to to left of the 
            // middle of the newly allocated block.
            var nstart = data.Length - 1;

            // We have one more item
            var ncount = count + 1;

            // Set the value in the new data block
            ndata[nstart] = value;

            // Return everything 
            return new SeqIterator<A>(ndata, nstart, ncount, 0, seq, lazyStart);
        }
        else
        {
            // We're cloning because there are multiple cons operations
            // from the same Seq.  We can't keep walking along the same 
            // array, so we clone with the exact same settings and insert

            var ndata  = new A[data.Length];
            var nstart = start - 1;

            Array.Copy(data, start, ndata, start, count);

            ndata[nstart] = value;

            return new SeqIterator<A>(ndata, nstart, count + 1, 0, seq, lazyStart);
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
        if (seq.Count - lazyStart > 0)
        {
            var scount = seq.Count;
            var sdata  = seq.Data;
            for (var i = lazyStart; i < scount; i++)
            {
                state = f(state, sdata[i]);
            }
        }
        return state;
    }

    public S FoldBack<S>(S state, Func<S, A, S> f)
    {
        InternalStrict();
        if (seq.Count - lazyStart > 0)
        {
            var sdata = seq.Data;
            for (var i = seq.Count - 1; i >= lazyStart; i--)
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
            return new SeqIterator<A>(data, start + amount, count - amount, NoCons, seq, lazyStart);
        }
        else if (amount == count)
        {
            return new SeqIterator<A>(new A[DefaultCapacity], DefaultCapacity, 0, 0, seq, lazyStart);
        }
        else
        {
            var namount = amount   - count;
            var end     = lazyStart + namount;
            if (end > seq.Count)
            {
                for (var i = lazyStart; i < end && seq.Get(i).Success; i++)
                {
                    // this is empty intentionally
                }
            }

            if(seq.Count >= end)
            {
                return new SeqIterator<A>(new A[DefaultCapacity], DefaultCapacity, 0, 0, seq, end);
            }
            else
            {
                return SeqEmptyInternal<A>.Default;
            }
        }
    }

    void InternalStrict()
    {
        while (seq.Get(seq.Count).Success)
        {
            // this is empty intentionally
        }
    }

    public ISeqInternal<A> Strict()
    {
        InternalStrict();

        var len    = DefaultCapacity;
        var ncount = count + seq.Count - lazyStart;
        while (len < ncount) len <<= 1;

        var nstart = (len - ncount) >> 1;

        var ndata = new A[len];
        if (count > 0)
        {
            Array.Copy(data, data.Length - count, ndata, nstart, count);
        }
        if (seq.Count > 0)
        {
            Array.Copy(seq.Data, lazyStart, ndata, nstart + count, seq.Count - lazyStart);
        }
        return new SeqStrict<A>(ndata, nstart, ncount, 0, 0);
    }

    public ISeqInternal<A> Take(int amount)
    {
        if(amount <= count)
        {
            var ndata  = new A[data.Length];
            var nstart = data.Length - count;
            Array.Copy(data, nstart, ndata, nstart, data.Length);
            return new SeqStrict<A>(ndata, start, amount, 0, 0);
        }
        else
        {
            var namount = amount   - count;
            var end     = lazyStart + namount;
            for (var i = lazyStart; i < end && seq.Get(i).Success; i++)
            {
                // this is empty intentionally
            }
            var seqLen = seq.Count - lazyStart;

            amount = Math.Min(seqLen + count, amount);

            if (amount == 0)
            {
                // Empty
                var edata = new A[DefaultCapacity];
                return new SeqStrict<A>(edata, DefaultCapacity >> 1, 0, 0, 0);
            }
            else
            {
                var len = DefaultCapacity;
                while (len < amount) len <<= 1;

                var ndata  = new A[len];
                var nstart = (len - amount) >> 1;
                if (count > 0)
                {
                    Array.Copy(data, data.Length - count, ndata, nstart, count);
                }

                if (seq.Count - lazyStart > 0)
                {
                    Array.Copy(seq.Data, lazyStart, ndata, nstart + count, amount - count);
                }

                return new SeqStrict<A>(ndata, nstart, amount, 0, 0);
            }
        }
    }

    public IEnumerator<A> GetEnumerator()
    {
        var nstart = data.Length - count;
        var nend   = data.Length;

        for (var i = nstart; i < nend; i++)
        {
            yield return data[i];
        }
        for(var i = lazyStart; ; i++)
        {
            var (succ, val) = seq.Get(i);
            if(succ)
            {
                yield return val!;
            }
            else
            {
                yield break;
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        var nstart = data.Length - count;
        var nend   = data.Length;

        for (var i = nstart; i < nend; i++)
        {
            yield return data[i];
        }
        for (var i = lazyStart; ; i++)
        {
            var (succ, val) = seq.Get(i);
            if (succ)
            {
                yield return val!;
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() =>
        selfHash == 0
            ? selfHash = GetHashCode(FNV32.OffsetBasis)
            : selfHash;

    public int GetHashCode(int hash)
    {
        InternalStrict();
        if (count > 0)
        {
            hash = FNV32.Hash<HashableDefault<A>, A>(data, start, count, hash);
        }
        if (seq.Count - lazyStart > 0)
        {
            hash = FNV32.Hash<HashableDefault<A>, A>(seq.Data, lazyStart, seq.Count - lazyStart, hash);
        }
        return hash;
    }
}

using System;
using System.Threading;
using System.Runtime.CompilerServices;
using LanguageExt.ClassInstances;
using LanguageExt.Common;

namespace LanguageExt;

class SeqStrict<A> : ISeqInternal<A>
{
    public const int DefaultCapacity = 8;

    const int NoCons = 1;
    const int NoAdd = 1;

    /// <summary>
    /// Backing data
    /// </summary>
    internal readonly A[] data;

    /// <summary>
    /// Index into data where the Head is
    /// </summary>
    internal readonly long start;

    /// <summary>
    /// Known size of the sequence
    /// </summary>
    internal readonly long count;

    /// <summary>
    /// 1 if no more consing is allowed
    /// </summary>
    int consDisallowed;

    /// <summary>
    /// 1 if no more adding is allowed
    /// </summary>
    int addDisallowed;

    /// <summary>
    /// Cached hash code
    /// </summary>
    int selfHash;

    public ReadOnlySpan<A> AsSpan() =>
        start + count > int.MaxValue
            ? throw new ArgumentOutOfRangeException(nameof(count), "Sequence length exceeds maximum span size")
            : new(data, (int)start, (int)count);

    /// <summary>
    /// Constructor
    /// </summary>
    public SeqStrict(A[] data, long start, long count, int consDisallowed, int addDisallowed)
    {
        this.data = data;
        this.start = start;
        this.count = count;
        this.consDisallowed = consDisallowed;
        this.addDisallowed = addDisallowed;
    }

    public static SeqStrict<A> Empty
    {
        get => new (new A[DefaultCapacity], 4, 0, 0, 0);
    }

    /// <summary>
    /// Add constructor (called in the Add function only)
    /// </summary>
    public SeqStrict(A[] data, long start, long count)
    {
        this.data = data;
        this.start = start;
        this.count = count;
        consDisallowed = NoCons;
    }

    /// <summary>
    /// Indexer
    /// </summary>
    public A this[long index] =>
        index < 0 || index >= count
            ? throw new IndexOutOfRangeException()
            : data[start + index];

    /// <summary>
    /// Indexer
    /// </summary>
    public Option<A> At(long index) =>
        index < 0 || index >= count
            ? default(Option<A>)
            : data[start + index];

    /// <summary>
    /// Add an item to the end of the sequence
    /// </summary>
    /// <remarks>
    /// Forces evaluation of the entire lazy sequence so the item 
    /// can be appended
    /// </remarks>
    public ISeqInternal<A> Add(A value)
    {
        var end = start + count;
        if (1 == Interlocked.Exchange(ref addDisallowed, 1) || end == data.LongLength)
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
    /// Add an item to the end of the sequence
    /// </summary>
    /// <remarks>
    /// Forces evaluation of the entire lazy sequence so the item 
    /// can be appended
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SeqStrict<A> Add2(A value)
    {
        var end = start + count;
        if (1 == Interlocked.Exchange(ref addDisallowed, 1) || end == data.LongLength)
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
    SeqStrict<A> Concat(A[] items, long itemsStart, long itemsCount)
    {
        var end = start + count;
        if (1 == Interlocked.Exchange(ref addDisallowed, 1) || end + itemsCount >= data.LongLength)
        {
            return CloneAddRange(items, itemsStart, itemsCount);
        }
        else
        {
            Array.Copy(items, itemsStart, data, end, itemsCount);
            return new SeqStrict<A>(data, start, count + itemsCount, NoCons, 0);
        }
    }

    /// <summary>
    /// Prepend an item to the sequence
    /// </summary>
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
            var nlength = Math.Max(data.LongLength << 1, 1);

            // Allocate it
            var ndata = new A[nlength];

            // Copy the old data block to the second half of the new one
            // so we have space on the left-hand-side to put the cons'd
            // value
            Array.Copy(data, 0, ndata, data.LongLength, data.LongLength);

            // The new head position will be 1 cell to the left of the 
            // middle of the newly allocated block.
            var nstart = data.LongLength == 0
                             ? 0
                             : data.LongLength - 1;

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

            var ndata  = new A[data.LongLength];
            var nstart = start - 1;

            Array.Copy(data, start, ndata, start, count);

            ndata[nstart] = value;

            return new SeqStrict<A>(ndata, nstart, count + 1, 0, 0);
        }
    }

    SeqStrict<A> CloneAdd(A value)
    {
        var end = start + count;

        // Find the new size of the data array
        var nlength = data.LongLength == end
                          ? Math.Max(data.LongLength << 1, 1)
                          : data.LongLength;

        // Allocate it
        var ndata = new A[nlength];

        // Copy the old data block to the first half of the new one
        // so we have space on the right-hand-side to put the added
        // value
        Array.Copy(data, 0, ndata, 0, data.LongLength);

        // Set the value in the new data block
        ndata[end] = value;

        // Return everything 
        return new SeqStrict<A>(ndata, start, count + 1, 0, 0);
    }

    SeqStrict<A> CloneAddRange(A[] values, long valuesStart, long valuesCount)
    {
        var end = start + count;

        // Find the new size of the data array
        var nlength = Math.Max(Math.Max(data.LongLength << 1, 1), end + valuesCount);

        // Allocate it
        var ndata = new A[nlength];

        // Copy the old data block to the first half of the new one
        // so we have space on the right-hand-side to put the added
        // value
        Array.Copy(data, 0, ndata, 0, end);

        // Set the value in the new data block
        Array.Copy(values, valuesStart, ndata, end, valuesCount);

        // Return everything 
        return new SeqStrict<A>(ndata, start, count + valuesCount, 0, 0);
    }

    /// <summary>
    /// Head item in the sequence.  NOTE:  If `IsEmpty` is true then Head
    /// is undefined.  Call HeadOrNone() if for maximum safety.
    /// </summary>
    public A Head =>
        count == 0
            ? throw Exceptions.SequenceEmpty
            : data[start];

    /// <summary>
    /// Tail of the sequence
    /// </summary>
    public ISeqInternal<A> Tail =>
        count < 1
            ? SeqEmptyInternal<A>.Default
            : new SeqStrict<A>(data, start + 1, count - 1, NoCons, NoAdd);

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
    /// </remarks>
    public bool IsEmpty => 
        count == 0;

    /// <summary>
    /// Last item in sequence.  Throws if no items in sequence
    /// </summary>
    public A Last =>
        IsEmpty
            ? throw Exceptions.SequenceEmpty
            : data[start + count - 1];

    /// <summary>
    /// Returns the number of items in the sequence
    /// </summary>
    /// <returns>Number of items in the sequence</returns>
    public long Count => 
        count;

    /// <summary>
    /// Skip count items
    /// </summary>
    public ISeqInternal<A> Skip(long amount)
    {
        if (amount < 1)
        {
            return this;
        }

        var end      = start + count;
        var newStart = start + amount;
        return newStart < end
                   ? new SeqStrict<A>(data, newStart, count - amount, NoCons, NoAdd)
                   : SeqEmptyInternal<A>.Default;
    }

    /// <summary>
    /// Take count items
    /// </summary>
    public ISeqInternal<A> Take(long amount) =>
        amount < count
            ? new SeqStrict<A>(data, start, amount, NoCons, NoAdd)
            : this;

    public ISeqInternal<A> Strict() =>
        this;

    internal static SeqStrict<A> FromSingleValue(A value) =>
        new ([default!, default!, default!, default!, value, default!, default!, default!], 4, 1, 0, 0);

    public Iterator<A> GetIterator() =>
        Iterator.forward(data, start, count);

    public SeqType Type => 
        SeqType.Strict;

    public SeqStrict<A> Append(SeqStrict<A> right)
    {
        var end = start + count + right.count;
        if (end > data.LongLength || 1 == Interlocked.Exchange(ref addDisallowed, 1))
        {
            // Clone
            var nsize = 8L;
            while(nsize < end)
            {
                nsize <<= 1;
            }

            var ndata = new A[nsize];
            Array.Copy(data, start, ndata, start, count);
            Array.Copy(right.data, right.start, ndata, start + count, right.count);
            return new SeqStrict<A>(ndata, start, count      + right.count, 0, 0);
        }
        else
        {
            Array.Copy(right.data, right.start, data, start + count, right.count);
            return new SeqStrict<A>(data, start, count      + right.count, NoCons, 0);
        }
    }

    public override int GetHashCode() =>
        selfHash == 0
            ? selfHash = GetHashCode(FNV32.OffsetBasis)
            : selfHash;

    public int GetHashCode(int offsetBasis) =>
        FNV32.Hash<HashableDefault<A>, A>(data, start, count, offsetBasis);

    public Seq.FoldState InitFoldState() =>
        start + count > int.MaxValue
            ? Seq.FoldState.FromIterator(GetIterator())
            : Seq.FoldState.FromSpan(AsSpan());        
}

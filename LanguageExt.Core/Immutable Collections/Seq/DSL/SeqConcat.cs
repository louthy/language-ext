using System;
using System.Linq;

namespace LanguageExt;

class SeqConcat<A> : ISeqInternal<A>
{
    readonly Iterator<A> iterator;
    public readonly Seq<ISeqInternal<A>> seqs;
    
    int selfHash;

    public SeqConcat(Seq<ISeqInternal<A>> sequences)  
    {
        seqs = DeConcat(sequences);
        iterator = seqs.IsEmpty
                       ? Iterator<A>.Empty
                       : seqs.Tail.Fold((xs, seq) => xs + seq.GetIterator(), seqs[0].GetIterator());
    }

    static Seq<ISeqInternal<A>> DeConcat(Seq<ISeqInternal<A>> seqs) =>
        seqs.Bind(DeConcat);

    static Seq<ISeqInternal<A>> DeConcat(ISeqInternal<A> seq) =>
        seq is SeqConcat<A> sc 
            ? sc.seqs.Bind(DeConcat) 
            : [seq];

    public ReadOnlySpan<A> AsSpan() =>
        Strict().AsSpan();

    public A this[long index]
    {
        get
        {
            var r = At(index);
            if (r.IsSome) return r.Value!;
            throw new IndexOutOfRangeException();
        }
    }

    public Option<A> At(long index)
    {
        foreach (var seq in seqs)
        {
            switch (seq.Type)
            {
                case SeqType.Empty:
                    // Empty streams yield no values
                    return default;
                
                case SeqType.Lazy:
                    // This should stream up to the required index and no more; or if the index
                    // lies beyond the sequence, it will consume everything in the lazy stream,
                    // which is expected.
                    var ox = seq.At(index);
                    
                    // If we found our element, return
                    if (ox.IsSome) return ox;
                    
                    // Otherwise, we've consumed the entire lazy stream, so we're able to read the 
                    // Count value and have it be meaningful.  Use that to move onto the next sequence
                    // in the concatenation.
                    index -= seq.Count;
                    break;
                
                case SeqType.Strict when index < seq.Count:
                    // We're within the strict sequence, so return the element
                    return seq[index];
                    
                case SeqType.Strict:
                    // We're beyond the strict sequence, so move onto the next one
                    index -= seq.Count;
                    continue;

                case SeqType.Concat:
                    // We are removing the SeqConcat values from the sequence in the constructor.
                    // So, this should never happen.
                    throw new InvalidOperationException("Concatenated sequences not supported: should have been flattened in the constructor");
                
                default:
                    throw new InvalidOperationException("Unexpected sequence type");
            }
        }
        // Index out of range
        return default;
    }

    public SeqType Type =>
        SeqType.Concat;

    public A Head
    {
        get
        {
            foreach (var seq in seqs)
            {
                if(seq.IsEmpty) continue;
                return seq.Head;
            }
            throw new InvalidOperationException("Empty sequence");
        }
    }

    public Iterator<A> GetIterator() =>
        iterator;

    public ISeqInternal<A> Tail
    {
        get
        {
            var first = true;
            Seq<ISeqInternal<A>> nseqs = [];
            foreach (var seq in seqs)
            {
                if(seq.IsEmpty) continue;
                nseqs = nseqs.Add(first ? seq.Tail : seq);
                first = false;
            }
            return nseqs.IsEmpty 
                       ? SeqEmptyInternal<A>.Default 
                       : new SeqConcat<A>(nseqs);
        }
    }

    public bool IsEmpty => 
        iterator.ForAll(_ => false);

    public ISeqInternal<A> Init
    {
        get
        {
            var arr   = iterator.ToArray(SeqStrict<A>.DefaultCapacity >> 1);
            var count = Math.Max(0, arr.Count - 1);
            if(count <= 1) return SeqEmptyInternal<A>.Default;
            return new SeqStrict<A>(arr.Buffer, arr.Start, count, 0, 0);
        }
    }

    public A Last
    {
        get
        {
            var last = seqs.Last;
            return last.IsSome
                       ? last.Value!.Last
                       : throw new InvalidOperationException("Empty sequence");
        }
    }

    public long Count
    {
        get
        {
            var count = 0L;
            foreach (var seq in seqs)
            {
                count += seq.Count;
            }
            return count;
        }
    }

    public SeqConcat<A> AddSeq(ISeqInternal<A> ma) =>
        new (seqs.Add(ma));

    public SeqConcat<A> AddSeqRange(Seq<ISeqInternal<A>> ma) =>
        new (seqs.Concat(ma));

    public SeqConcat<A> ConsSeq(ISeqInternal<A> ma) =>
        new (ma.Cons(seqs));

    public ISeqInternal<A> Add(A value)
    {
        if (seqs.IsEmpty) Seq.FromSingleValue(value);
        var     arrs = seqs.ToArray();
        ref var last = ref arrs[^1];
        last = last.Add(value);
        return new SeqConcat<A>(Seq.FromArray(arrs));
    }

    public ISeqInternal<A> Cons(A value)
    {
        if (seqs.IsEmpty) Seq.FromSingleValue(value);
        var     arrs = seqs.ToArray();
        ref var first = ref arrs[0];
        first = first.Cons(value);
        return new SeqConcat<A>(Seq.FromArray(arrs));
    }

    public Unit Iter(Action<A> f)
    {
        foreach (var s in seqs)
        {
            foreach (var a in s)
            {
                f(a);
            }
        }
        return default;
    }

    public ISeqInternal<A> Skip(long amount) =>
        new SeqIterator<A>(GetIterator().Skip(1));
    
    public ISeqInternal<A> Strict()
    {
        foreach(var s in seqs)
        {
            s.Strict();
        }
        return this;
    }

    public ISeqInternal<A> Take(long amount) =>
        new SeqIterator<A>(iterator.Take(amount));

    public Seq.FoldState InitFoldState() =>
        Seq.FoldState.FromIterator(iterator);

    public override int GetHashCode() =>
        selfHash == 0
            ? selfHash = GetHashCode(FNV32.OffsetBasis)
            : selfHash;        

    public int GetHashCode(int hash)
    {
        foreach (var seq in seqs)
        {
            hash = seq.GetHashCode(hash);
        }
        return hash;
    }
}

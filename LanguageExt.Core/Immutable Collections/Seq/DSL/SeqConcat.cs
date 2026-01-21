using System;
using System.Linq;

namespace LanguageExt;

class SeqConcat<A> : ISeqInternal<A>
{
    readonly Iterator<A> iterator;
    public readonly Seq<ISeqInternal<A>> seqs;
    
    int selfHash;

    public SeqConcat(Seq<ISeqInternal<A>> ms)
    {
        seqs = ms;
        iterator = seqs.IsEmpty
                       ? Iterator<A>.Empty
                       : seqs.Tail.Fold((xs, seq) => xs + seq.GetIterator(), seqs[0].GetIterator());
    }

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

    public Option<A> At(long index) =>
        iterator.At(index);

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

    ISeqInternal<A> Flatten()
    {
        var total = 0L;
        foreach (var s in seqs)
        {
            s.Strict();
            total = s.Count;
        }

        var cap = 8L;
        while(cap < total)
        {
            cap <<= 1;
        }

        var data    = new A[cap];
        var start   = (cap - total) >> 1;
        var current = start;

        foreach(var s in seqs)
        {
            var strict = (SeqStrict<A>)s;
            Array.Copy(strict.data, strict.start, data, current, strict.count);
            current += strict.count;
        }
        return new SeqStrict<A>(data, start, total, 0, 0);
    }
        

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

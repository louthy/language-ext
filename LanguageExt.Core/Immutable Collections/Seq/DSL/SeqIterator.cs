using System;

namespace LanguageExt;

class SeqIterator<A> : ISeqInternal<A>
{
    const int MinimumSize = 8;
    readonly IteratorMemo<A> memo;

    public SeqIterator(Iterator<A> iterator) =>
        memo = new IteratorMemo<A>(iterator);

    public Iterator<A> GetIterator() => 
        memo;

    public SeqType Type => 
        SeqType.Lazy;

    public A this[long index] =>
        memo.Get(index) switch
        {
            { IsSome: true, Value: var value } => value!,
            _                                  => throw new IndexOutOfRangeException()
        };

    public Option<A> At(long index) => 
        memo.Get(index) switch
        {
            { IsSome: true, Value: var value } => value!,
            _                                  => throw new IndexOutOfRangeException()
        };

    public ISeqInternal<A> Add(A value) => 
        new SeqConcat<A>([this, SeqStrict<A>.FromSingleValue(value)]);

    public ISeqInternal<A> Cons(A value) => 
        new SeqConcat<A>([SeqStrict<A>.FromSingleValue(value), this]);

    public A Head => 
        this[0];

    public ISeqInternal<A> Tail =>
        memo switch
        {
            (Exist<A>, var tail) => new SeqIterator<A>(tail),
            _                    => SeqEmptyInternal<A>.Default
        };

    public bool IsEmpty =>
        memo switch
        {
            (Exist<A>, _) => false,
            _             => true
        };

    public ISeqInternal<A> Init =>
        Strict().Init;

    public A Last
    {
        get
        {
            memo.GetAll();
            return memo.Get(memo.Count - 1) switch
                   {
                       { IsSome: true, Value: var value } => value!,
                       _                                  => throw new IndexOutOfRangeException()
                   };
        }        
    }

    public long Count
    {
        get
        {
            memo.GetAll();
            return memo.Count;
        }
    }

    public ISeqInternal<A> Skip(long amount) => 
        new SeqIterator<A>(memo.Skip(amount));

    public ISeqInternal<A> Take(long amount) => 
        new SeqIterator<A>(memo.Take(amount));

    public ISeqInternal<A> Strict()
    {
        memo.GetAll();
        var size = PowerOf2(AssertMinOwnedSize(memo.Count));
        var arr  = new A[size];
        if (memo.Count > int.MaxValue)
        {
            Array.Copy(memo.Data, arr, memo.Count);
        }
        else
        {
            var fs = new ReadOnlySpan<A>(memo.Data, 0, (int)memo.Count);
            var ts = new Span<A>(arr, 0, (int)memo.Count);
            fs.CopyTo(ts);
        }
        return new SeqStrict<A>(arr, 0, memo.Count, 0, 0);
    }
    
    static long AssertMinOwnedSize(long size) =>
        size < MinimumSize 
            ? MinimumSize 
            : size;
    
    static long PowerOf2(long size)
    {
        size--;
        size |= size >> 1;
        size |= size >> 2;
        size |= size >> 4;
        size |= size >> 8;
        size |= size >> 16;
        size |= size >> 32;
        size++;
        return size;
    }

    public int GetHashCode(int offsetBasis) => 
        memo.GetHashCode();

    public ReadOnlySpan<A> AsSpan() 
    {
        memo.GetAll();
        if (memo.Count > int.MaxValue)
        {
            throw new InvalidOperationException("Backing collection is too big to return a view");
        }
        else
        {
            return new ReadOnlySpan<A>(memo.Data, 0, (int)memo.Count);
        }
    }

    public Seq.FoldState InitFoldState() =>
        Seq.FoldState.FromIterator(memo);
}

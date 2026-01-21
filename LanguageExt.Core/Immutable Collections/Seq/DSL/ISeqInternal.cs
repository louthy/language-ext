using System;
using System.Collections;
using System.Collections.Generic;

namespace LanguageExt;

enum SeqType
{
    Empty,
    Lazy,
    Strict,
    Concat
}

interface ISeqInternal<A> : IEnumerable<A>
{
    SeqType Type { get; }
    A this[long index] { get; }
    Option<A> At(long index);
    ISeqInternal<A> Add(A value);
    ISeqInternal<A> Cons(A value);
    A Head { get; }
    ISeqInternal<A> Tail { get; }
    bool IsEmpty { get; }
    ISeqInternal<A> Init { get; }
    A Last { get; }
    long Count { get; }
    ISeqInternal<A> Skip(long amount);
    ISeqInternal<A> Take(long amount);
    ISeqInternal<A> Strict();
    int GetHashCode(int offsetBasis);
    ReadOnlySpan<A> AsSpan();
    Seq.FoldState InitFoldState();
    
    SeqConcat<A> AddSeq(ISeqInternal<A> ma) =>
        ma is SeqConcat<A> ritems
            ? new SeqConcat<A>(this.Cons(ritems.seqs))
            : new SeqConcat<A>([this, ma]);

    SeqConcat<A> AddSeqRange(Seq<ISeqInternal<A>> ma) =>
        new (this.Cons(ma));

    SeqConcat<A> ConsSeq(ISeqInternal<A> ma) =>
        ma is SeqConcat<A> ritems
            ? new SeqConcat<A>(ritems.seqs.Add(this))
            : new SeqConcat<A>([ma, this]);
    
    Iterator<A> GetIterator();
    
    new IteratorEnumerator<A> GetEnumerator() =>
        GetIterator().GetEnumerator();

    IEnumerator<A> IEnumerable<A>.GetEnumerator() => 
        GetEnumerator().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => 
        GetEnumerator().GetEnumerator();
}
